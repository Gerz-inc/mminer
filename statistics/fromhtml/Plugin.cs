﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace fromHTML
{
    public class Plugin
    {
        private string plugin_name;
        private string json_file_name;
    
        public struct HostTemplate
        {
            public string name;
            public Dictionary<string, string> req_headers;
            public Dictionary<string, string> req_cookies;
            
            public string diff_row_regex;
            public string diff_data_regex;
            public int diff_data_i;
        }

        public struct CoinSettings
        {
            public string name;
            public string url;
            public string template;

            public double diff_min;
            public double diff_max;
            public double diff_work_min;

            public DateTime req_date;
            public DateTime res_date;
            public string response;
        }

        public List<HostTemplate> templates = new List<HostTemplate>();
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

                JArray templates = response.SelectToken("templates").Value<JArray>();
                this.templates.Clear();
                foreach (var item in templates)
                {
                    var templ = new HostTemplate();
                    templ.name = item.SelectToken("name").Value<string>();

                    JObject headers = item.SelectToken("req_headers").Value<JObject>();
                    templ.req_headers = new Dictionary<string, string>();
                    foreach (var item_h in headers)
                        templ.req_headers[item_h.Key] = item_h.Value.Value<string>();
                    JObject cookies = item.SelectToken("req_cookies").Value<JObject>();
                    templ.req_cookies = new Dictionary<string, string>();
                    foreach (var item_c in cookies)
                        templ.req_cookies[item_c.Key] = item_c.Value.Value<string>();

                    templ.diff_row_regex = item.SelectToken("diff_row_regex").Value<string>();
                    templ.diff_data_regex = item.SelectToken("diff_data_regex").Value<string>();
                    templ.diff_data_i = item.SelectToken("diff_data_i").Value<int>();

                    this.templates.Add(templ);
                }

                JArray coins = response.SelectToken("coins").Value<JArray>();
                this.coins.Clear();
                foreach (var item in coins)
                {
                    var coin = new CoinSettings();
                    coin.name = item.SelectToken("name").Value<string>();
                    coin.url = item.SelectToken("url").Value<string>();
                    coin.template = item.SelectToken("template").Value<string>();

                    coin.diff_min = item.SelectToken("diff_min").Value<double>();
                    coin.diff_max = item.SelectToken("diff_max").Value<double>();
                    coin.diff_work_min = item.SelectToken("diff_work_min").Value<double>();

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
                item["template"] = coin.template;

                item["diff_min"] = coin.diff_min;
                item["diff_max"] = coin.diff_max;
                item["diff_work_min"] = coin.diff_work_min;

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
            var templ_i = GetTemplateByName(coin.template);
            bool? req_str = Request(coin, templ_i.Value.Value, out coin.response, false, req_timeout);
            if (req_str == true)
            {
                coin.res_date = DateTime.Now;
                sts = true;
            }

            return sts;
        }

        public static bool? Request(CoinSettings coin, HostTemplate templ, out string response, bool escaped = false, int timeout = 10000)
        {
            try
            {
                string query = coin.url;
                if (!escaped) query = Uri.EscapeUriString(coin.url);
                Trace.WriteLine("Request: " + query);

                // Send request
                Uri url = new Uri(query);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                foreach (var item_h in templ.req_headers)
                    switch (item_h.Key)
                    {
                        case "User-Agent": req.UserAgent = item_h.Value; break;
                    }

                if (req.CookieContainer == null)
                    req.CookieContainer = new CookieContainer();
                foreach (var item_c in templ.req_cookies)
                    req.CookieContainer.Add(new Cookie(item_c.Key, item_c.Value, "/", url.Host));
            
                req.Timeout = timeout;
                WebResponse resp = req.GetResponse();
                Stream stream = resp.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                response = sr.ReadToEnd();

                return response != null && response.Length > 0;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Request error: " + coin.url + "\n" + ex.Message);

                response = null;
                return null;
            }
        }

        public KeyValuePair<double, double> GetCoinDifficulty(ref CoinSettings coin, HostTemplate? templ)
        {
            if (GetCoinDataIfNeeded(ref coin))
            {
                try
                {
                    int rate_max = 5,
                        rate_min = 0;

                    double diff = 0;
                    if (coin.response != null && templ != null)
                    {
                        // Ищем строки
                        foreach (Match m in Regex.Matches(coin.response, templ.Value.diff_row_regex, RegexOptions.IgnoreCase))
                        {
                            if (m.Success)
                            {
                                string row_html = m.Groups[1].Captures[0].ToString();
                                if (!row_html.ToLower().Contains(coin.name.ToLower())) continue;

                                int idx_ = 0;
                                foreach (Match m2 in Regex.Matches(row_html, templ.Value.diff_data_regex, RegexOptions.IgnoreCase))
                                {
                                    if (m2.Success && idx_ == templ.Value.diff_data_i)
                                    {
                                        string str = m2.Groups[1].Captures[0].ToString();
                                        Double.TryParse(str.Replace(".", ","), out diff);
                                        if (coin.diff_work_min >= diff)
                                            return new KeyValuePair<double, double>(-1, 0);

                                        double rate = (rate_max - rate_min) / (coin.diff_max - coin.diff_min) * (diff - coin.diff_min);
                                        return new KeyValuePair<double, double>(diff == -1.00 ? -1.01 : diff, rate);
                                    }
                                    idx_ += 1;
                                }
                            }
                        }
                    }
                    else return new KeyValuePair<double, double>(-1, 0);
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
                    HostTemplate? templ = templates.Find(x => x.name == coin.template);
                    if (templ == null) return new KeyValuePair<double, double>(-1, 0);

                    var diff = GetCoinDifficulty(ref coin, templ);
                    this.coins[i] = coin; // save

                    if (diff.Key >= 0)
                        return diff;
                }
            }
            return new KeyValuePair<double, double>(-1, 0);
        }

        public KeyValuePair<int, HostTemplate>? GetTemplateByName(string name)
        {
            for (int i = 0; i < templates.Count; i++)
            {
                var templ = this.templates[i];
                if (templ.name == name)
                    return new KeyValuePair<int, HostTemplate>(i, templ);
            }
            return null;
        }

        #endregion
    }
}
