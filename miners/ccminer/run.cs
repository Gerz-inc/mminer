using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using baseFunc;

namespace ccminer
{
    class run
    {
        public Control c;
        public RichTextBox console;

        public delegate void dell(string text, Color col);
        public delegate void dell2(string text);

        public void run_thread(string name, string args, int current_id_running, ref_bool is_miner_running, ref_bool is_running, string_pipe pipe)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((x) =>
            {
                string exe = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\ccminers\\" + name + "\\" + name + ".exe";

                if (File.Exists(exe))
                {
                    AppendText("\n[miner] ", Color.Green);
                    AppendText("Starting " + exe + " " + args);

                    var proc = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = exe,
                            Arguments = args,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };

                    //is not supported

                    proc.Start();
                    while (!proc.StandardOutput.EndOfStream && is_running.Value)
                    {
                        string line = proc.StandardOutput.ReadLine();

                        if (line.Contains("is not supported"))
                        {
                            pipe.write("" + current_id_running.ToString() + "|not_supported");
                        }

                        AppendText("\n[miner] ", Color.Yellow);
                        AppendText(line);
                    }
                    try
                    {
                        proc.Kill();
                    }
                    catch (Exception ex) { }
                }
                else
                {
                    AppendText("\n[miner] ", Color.Red);
                    AppendText("File does not exists: " + exe);
                }


                AppendText("\n[miner] ", Color.Green);
                AppendText("exit");

                is_miner_running.Value = false;
            }));
        }

        private void AppendText(string text_, Color col_)
        {
            c.Invoke(new dell((string text, Color col) => 
            {
                console.AppendText(text, col);
            }), new Object[] { text_, col_ });
        }

        private void AppendText(string text_)
        {
            c.Invoke(new dell2((string text) =>
            {
                console.AppendText(text);
            }), new Object[] { text_ });
        }
    }

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
}
