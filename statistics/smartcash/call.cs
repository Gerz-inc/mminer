using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using baseFunc;
using Newtonsoft.Json.Linq;

namespace smartcash
{
    public class Call
    {
        private string dll_name;
        private string ini_file_name;

        public string api_key;
        public int work_pool = 0;
        public List<string> pools = new List<string>();
        public double diff_min = 100;
        public double diff_max = 400;

        private int req_timeout = 10000;
        private DateTime req_date;
        private DateTime res_date;
        private JObject response;

        public Call()
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            dll_name = Assembly.GetExecutingAssembly().GetName().Name;
            ini_file_name = Path.Combine(assemblyFolder, dll_name + ".ini");

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
            var ini = new IniFile(ini_file_name);

            // Pools
            pools.Clear();
            while (ini.KeyExists("Pools", "pool." + pools.Count))
            {
                string pool = ini.Read("Pools", "pool." + pools.Count);
                pools.Add(pool);
            }

            // API
            api_key = ini.Read("API", "key");

            // Difficulty
            Double.TryParse(ini.Read("Difficulty", "min").Replace(".", ","), out diff_min);
            Double.TryParse(ini.Read("Difficulty", "max").Replace(".", ","), out diff_max);

            return api_key.Length > 0;
        }

        public void SaveSettings()
        {
            var ini = new IniFile(ini_file_name);

            // Pools
            int pool_i = 0;
            while (ini.KeyExists("Pools", "pool." + pool_i) || pool_i < pools.Count)
            {
                if (pool_i < pools.Count)
                    ini.Write("Pools", "pool." + pool_i, pools[pool_i]);
                else ini.DeleteKey("Pools", "pool." + pool_i);
                pool_i += 1;
            }

            // API
            ini.Write("API", "key", api_key);

            // Difficulty
            ini.Write("Difficulty", "diff_min", diff_min.ToString());
            ini.Write("Difficulty", "diff_max", diff_max.ToString());
        }

        #endregion

        #region API

        private bool GetDataIfNeeded()
        {
            if ((DateTime.Now - res_date).TotalSeconds < 60)
                return true;

            return GetData(out work_pool);
        }

        private bool GetData(out int pool_i)
        {
            int pool = work_pool;
            if (pool > pools.Count || pool == -1) pool = 0;
            if (pool == pools.Count) pool = -1;
            int prev_pool = pool;
            bool first = true;

            while ((first || prev_pool != pool) && pool >= 0)
            {
                first = false;
                req_date = new DateTime();
                string query = String.Format("https://{0}/index.php?page=api&action=getpoolstatus&api_key={1}", pools[pool], api_key);
                if (baseFunc.Json.Request(query, out response, false, req_timeout) == true)
                {
                    res_date = new DateTime();
                    pool_i = pool;
                    return true;
                }
                pool = pool < pools.Count - 1 ? ++pool : 0;
            }

            pool_i = -1;
            return false;
        }

        public double GetDifficulty(out double rate)
        {
            if (GetDataIfNeeded())
            {
                int rate_max = 5,
                    rate_min = 0;

                double diff = response.SelectToken("getpoolstatus.data.networkdiff").Value<double>() / 1000;
                rate = (rate_max - rate_min) / (diff_max - diff_min) * (diff - diff_min);
                return diff;
            }
            else
            {
                rate = 0;
                return -1;
            }
        }

        #endregion
    }
}
