using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using baseFunc;

namespace ccminer
{
    class Call
    {
        run r = new run();

        public void ShowSettings()
        {
            MessageBox.Show("2");
        }

        public Dictionary<string, string> get_available_miners()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            var d = Directory.GetDirectories(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\ccminers");
            foreach (var dd in d)
            {
                var dirName = new DirectoryInfo(dd).Name;
                ret.Add(dirName, "ccminer");
            }

            return ret;
        }

        public void run(string name, string args, int current_id_running, Control c, ref RichTextBox console, ref_bool is_miner_running, ref_bool is_running, string_pipe pipe)
        {
            r.c = c;
            r.console = console;
            r.run_thread(name, args, current_id_running, is_miner_running, is_running, pipe);
        }
    }

    
}
