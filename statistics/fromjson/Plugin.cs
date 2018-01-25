using System;
using System.Collections.Generic;
using System.IO;
using baseFunc;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace fromjson
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
        }
        
        public List<CoinSettings> coins = new List<CoinSettings>();
        private int req_timeout = 10000;

        public Plugin()
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            plugin_name = Assembly.GetExecutingAssembly().GetName().Name;
            json_file_name = Path.Combine(assemblyFolder, plugin_name + ".json");

            if (ReadSettings())
                GetDataIfNeeded();
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

        private bool GetDataIfNeeded()
        {
            /*if ((DateTime.Now - res_date).TotalSeconds < 60)
                return true;

            return GetData(out work_pool);*/
            return true;
        }

        private bool GetData(out int pool_i)
        {
            /*int pool = work_pool;
            if (pool > pools.Count || pool == -1) pool = 0;
            if (pool == pools.Count) pool = -1;
            int prev_pool = pool;
            bool first = true;

            while ((first || prev_pool != pool) && pool >= 0)
            {
                first = false;
                req_date = new DateTime();
                string query = pools[pool];
                if (baseFunc.Json.Request(query, out response, false, req_timeout) == true)
                {
                    res_date = new DateTime();
                    pool_i = pool;
                    return true;
                }
                pool = pool < pools.Count - 1 ? ++pool : 0;
            }*/

            pool_i = -1;
            return false;
        }

        public KeyValuePair<double, double> GetDifficulty(string coin)
        {
            /*if (GetDataIfNeeded())
            {
                int rate_max = 5,
                    rate_min = 0;

                double diff = response.SelectToken("getpoolstatus.data.networkdiff").Value<double>() / 1000;
                double rate = (rate_max - rate_min) / (diff_max - diff_min) * (diff - diff_min);
                rate = rate > rate_max ? rate_max : (rate < rate_min ? rate_min : rate);
                return new KeyValuePair<double, double>(diff, rate);
            }
            else*/
            {
                return new KeyValuePair<double, double>(-1, 0);
            }
        }

        #endregion
    }
}
