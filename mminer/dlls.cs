using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace mminer
{
    class dlls
    {
        public class coin
        {
            public string name;
            public string abr;
            public string file;
        }

        public const string PATH_STATISTICS = "\\statistics\\";
        public const string PATH_MINERS = "\\miners\\";
        public const string PATH_COINS = "\\coins\\";

        private static List<string> statistics = new List<string>();
        private static Dictionary<string, string> miners = new Dictionary<string, string>();
        private static Dictionary<string, coin> coins = new Dictionary<string, coin>();

        public static void load_ddls()
        {
            //coin statistic
            string[] files = System.IO.Directory.GetFiles(Application.StartupPath + PATH_STATISTICS, "*.dll");
            foreach (var f in files) statistics.Add(Path.GetFileName(f).Replace(".dll", ""));

            //miners
            files = System.IO.Directory.GetFiles(Application.StartupPath + PATH_MINERS, "*.dll");
            foreach (var f in files)
            {
                string name = Path.GetFileName(f).Replace(".dll", "");

                System.Reflection.Assembly DLL = null;
                if (File.Exists(f))
                {
                    DLL = System.Reflection.Assembly.LoadFile(f);
                    if (DLL != null)
                    {
                        Type t = DLL.GetType(name + ".Call");
                        var m = t.GetMethod("get_available_miners");
                        if (m == null)
                        {
                            MessageBox.Show("Method get_available_miners was not found in " + name + ".dll");
                        }
                        else
                        {
                            var a = Activator.CreateInstance(t);
                            Dictionary<string, string> result =(Dictionary<string, string>) m.Invoke(a, new Object[] { });
                            foreach (var d in result) miners.Add(d.Key, d.Value);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("File " + f + " not found");
                }
            }

            //coins
            files = System.IO.Directory.GetFiles(Application.StartupPath + PATH_COINS, "*.png");
            foreach (var f in files)
            {
                string name = Path.GetFileName(f).Replace(".png", "");
                string[] exp = baseFunc.base_func.explode("_", name);
                if (exp.Length >= 2)
                {
                    coin c = new coin();
                    c.name = exp[0];
                    c.abr = exp[1];
                    c.file = Path.GetFileName(f);
                    coins.Add(exp[0], c);
                }
            }

        }

        public static List<string> get_statistics()
        {
            return statistics;
        }

        public static Dictionary<string, string> get_miners()
        {
            return miners;
        }

        public static Dictionary<string, coin> get_coins()
        {
            return coins;
        }

        public static string get_coin_image(string name)
        {
            if (coins.ContainsKey(name))
            {
                return Application.StartupPath + PATH_COINS + coins[name].file;
            }
            return "";
        }
    }
}
