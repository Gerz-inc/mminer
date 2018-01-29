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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace uminer
{
    class run
    {
        public Control c;
        public RichTextBox console;

        public delegate void dell(string text, Color col);
        public delegate void dell2(string text);
        public delegate void delle(object sendingProcess, DataReceivedEventArgs outLine);

        private string cmd_line;

        public void run_thread(string name, List<string> args, int current_id_running, ref_bool is_miner_running, ref_bool is_running, string_pipe pipe)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((x) =>
            {
                //check ini
                string ini = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\uminers\\" + name + "\\" + name + ".json";
                if (File.Exists(ini))
                {
                    if (ReadSettings(ini))
                    {
                        string[] exp_url = base_func.explode(":", args[2]);

                        string args_ = cmd_line;
                        args_ = args_.Replace("$ALGO", args[0]);
                        args_ = args_.Replace("$CONNECTION_TYPE", args[1]);
                        args_ = args_.Replace("$URL", exp_url[0]);
                        args_ = args_.Replace("$PORT", exp_url[1]);
                        args_ = args_.Replace("$USER", args[3]);
                        args_ = args_.Replace("$PASS", args[4]);

                        string exe = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\uminers\\" + name + "\\" + name + ".exe";

                        if (File.Exists(exe))
                        {
                            AppendText("\n[miner] ", Color.Green);
                            AppendText("Starting " + exe + " " + args_);

                            var proc = new Process
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = exe,
                                    Arguments = args_,
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,

                                    CreateNoWindow = true
                                }
                            };

                            delle d = new delle((object sendingProcess, DataReceivedEventArgs outLine) =>
                            {
                                string line = outLine.Data;

                                if (line != null)
                                {
                                    if (line.Contains("is not supported")) pipe.write("" + current_id_running.ToString() + "|not_supported");
                                    else if (line.Contains("connection interrupted")) pipe.write("" + current_id_running.ToString() + "|connection interrupted");
                                    else if (line.Contains("Failed to connect")) pipe.write("" + current_id_running.ToString() + "|Failed to connect");
                                    else if (line.Contains("waiting for data")) pipe.write("" + current_id_running.ToString() + "|waiting for data");
                                    else if (line.Contains("Could not resolve host")) pipe.write("" + current_id_running.ToString() + "|Could not resolve host");

                                    AppendText("\n[miner] ", Color.Yellow);
                                    AppendText(line);
                                }
                            });

                            proc.OutputDataReceived += new DataReceivedEventHandler(d);
                            proc.ErrorDataReceived += new DataReceivedEventHandler(d);

                            proc.Start();

                            proc.BeginOutputReadLine();
                            proc.BeginErrorReadLine();

                            //is not supported

                            try
                            {
                                while (is_running.Value)
                                {
                                    if (proc.WaitForExit(100)) break;
                                }
                            }
                            catch (Exception ex) { }

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
                    }
                    else
                    {
                        AppendText("\n[miner] ", Color.Red);
                        AppendText("Failed to load json settings from: " + ini);
                    }
                }
                else
                {
                    AppendText("\n[miner] ", Color.Red);
                    AppendText("File does not exists: " + ini);
                }


                AppendText("\n[miner] ", Color.Green);
                AppendText("exit");

                is_miner_running.Value = false;
            }));
            
        }

        private bool ReadSettings(string f)
        {
            try
            {
                string json_str = File.ReadAllText(f);
                JObject js = JObject.Parse(json_str);
                cmd_line = base_func.ParseString(js["cmd_line"]);
            }
            catch (Exception ex) { return false; }

            return true;
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
