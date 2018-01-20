using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using baseFunc;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace smartcash
{
    public class Call
    {
        private static string dll_name;
        private static string ini_file_name;

        private static string api_key;
        private static int work_pool = 0;
        private static List<string> pools = new List<string>();
        private static double diff_min = 100;
        private static double diff_max = 400;

        private static int req_timeout = 10000;
        private static DateTime req_date;
        private static DateTime res_date;
        private static JObject response;

        public Call()
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            dll_name = Assembly.GetExecutingAssembly().GetName().Name;
            ini_file_name = Path.Combine(assemblyFolder, dll_name + ".ini");

            if (ReadSettings())
                GetData(out work_pool);
        }

        #region Settings

        public void ShowSettings()
        {
            new Main(this).ShowDialog();

        }

        private static bool ReadSettings()
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

        private void SaveSettings()
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

        public bool GetData(out int pool_i)
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

        public double GetDifficulty()
        {
            //GetData(out work_pool);


            /*

                        {
                            pool_i = pool;

                            // Проверяем статус ответа
                            string status = (string)json_ar.SelectToken("status").Value<string>();
                            int status_int = -1; Int32.TryParse(status, out status_int);
                            if (status_int == 1) return true;
                            else
                            {
                                if (response.SelectToken("error") != null)
                                    MessageBox.Show(response.SelectToken("error").ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                                Trace.WriteLine("Request response: " + query + "\n" + response.ToString());
                            }

                            return true;
                        }

                        getpoolstatus data networkdiff
                        //json_ar = JObject.Parse(json_str);
                        */

            return 0;
        }

        



        #endregion
    }
}
