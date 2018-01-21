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
        public const string PATH_STATISTICS = "\\statistics\\";
        public const string PATH_MINERS = "\\miners\\";

        private static List<string> statistics = new List<string>();
        private static List<string> miners = new List<string>();

        public static void load_ddls()
        {
            string[] files = System.IO.Directory.GetFiles(Application.StartupPath + PATH_STATISTICS, "*.dll");
            foreach (var f in files) statistics.Add(Path.GetFileName(f).Replace(".dll", ""));

            files = System.IO.Directory.GetFiles(Application.StartupPath + PATH_MINERS, "*.dll");
            foreach (var f in files) miners.Add(Path.GetFileName(f).Replace(".dll", ""));
        }

        public static List<string> get_statistics()
        {
            return statistics;
        }

        public static List<string> get_miners()
        {
            return miners;
        }
    }
}
