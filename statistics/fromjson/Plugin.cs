using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace fromJSON
{
    public class Plugin
    {
        private string plugin_name;
        private string json_file_name;

        public struct CoinSettings
        {
            public string name;
            public double diff_min;
            public double diff_max;
            public string url;
            public string diff_path;

            public DateTime req_date;
            public DateTime res_date;
            public JObject response;
            public string response_raw;
        }
        
        public List<CoinSettings> coins = new List<CoinSettings>();
        private int req_timeout = 10000;

        public Plugin()
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            plugin_name = Assembly.GetExecutingAssembly().GetName().Name;
            json_file_name = Path.Combine(assemblyFolder, plugin_name + ".json");

            ReadSettings();
        }

        #region Settings

        public void ShowSettings()
        {
            new Main(this).ShowDialog();
        }

        public bool ReadSettings()
        {
            try
            {
                string json_str = File.ReadAllText(json_file_name);
                JObject response = JObject.Parse(json_str);
                JArray coins = response.SelectToken("coins").Value<JArray>();

                this.coins.Clear();
                foreach (var item in coins)
                {
                    var coin = new CoinSettings();
                    coin.name = item.SelectToken("name").Value<string>();
                    coin.url = item.SelectToken("url").Value<string>();
                    coin.diff_min = item.SelectToken("diff_min").Value<double>();
                    coin.diff_max = item.SelectToken("diff_max").Value<double>();
                    coin.diff_path = item.SelectToken("diff_path").Value<string>();
                    this.coins.Add(coin);
                }
            }
            catch (Exception ex) {}

            return true;
        }

        public void SaveSettings()
        {
            JObject response;
            string json_str;

            try
            { 
                json_str = File.ReadAllText(json_file_name);
                response = JObject.Parse(json_str);
            }
            catch (Exception ex)
            {
                response = new JObject();
            }

            JArray coins = new JArray();
            foreach (CoinSettings coin in this.coins)
            {
                JObject item = new JObject();
                item["name"] = coin.name;
                item["url"] = coin.url;
                item["diff_min"] = coin.diff_min;
                item["diff_max"] = coin.diff_max;
                item["diff_path"] = coin.diff_path;
                coins.Add(item);
            }
            response["coins"] = coins;

            json_str = JsonConvert.SerializeObject(response, Formatting.Indented);
            File.WriteAllText(json_file_name, json_str);
        }

        #endregion
        
        #region API

        private bool GetCoinDataIfNeeded(ref CoinSettings coin)
        {
            if ((DateTime.Now - coin.res_date).TotalSeconds < 10)
                return true;

            return GetCoinData(ref coin);
        }

        private bool GetCoinData(ref CoinSettings coin)
        {
            coin.req_date = DateTime.Now;

            bool sts = false;
            bool? req_str = baseFunc.Json.Request(coin.url, out coin.response, out coin.response_raw, false, req_timeout);
            if (req_str == true || coin.response_raw.Length > 0)
            {
                coin.res_date = DateTime.Now;
                sts = true;
            }

            return sts;
        }

        public KeyValuePair<double, double> GetCoinDifficulty(ref CoinSettings coin)
        {
            if (GetCoinDataIfNeeded(ref coin))
            {
                try
                {
                    int rate_max = 5,
                        rate_min = 0;

                    double diff = 0;
                    if (coin.response != null)
                    {
                        string str = coin.response.SelectToken(coin.diff_path).ToString();
                        Double.TryParse(str.Replace(".", ","), out diff);
                    }
                    else if (!Double.TryParse(coin.response_raw.Replace(".", ","), out diff))
                        return new KeyValuePair<double, double>(-1, 0);

                    diff /= 1000; // to xxx.xx
                    double rate = (rate_max - rate_min) / (coin.diff_max - coin.diff_min) * (diff - coin.diff_min);
                    rate = rate > rate_max ? rate_max : (rate < rate_min ? rate_min : rate);
                    return new KeyValuePair<double, double>(diff, rate);
                }
                catch (Exception ex) { }
            }
            return new KeyValuePair<double, double>(-1, 0);
        }

        public KeyValuePair<double, double> GetCoinDifficultyByName(string coin_name)
        {
            // Select all url for this coin
            for (int i = 0; i < this.coins.Count; ++i)
            {
                CoinSettings coin = coins[i];
                if (coin.name.ToLower() == coin_name.ToLower())
                {
                    var diff = GetCoinDifficulty(ref coin);
                    this.coins[i] = coin; // save

                    if (diff.Key >= 0)
                        return diff;
                }
            }
            return new KeyValuePair<double, double>(-1, 0);
        }

        #endregion
    }
}
