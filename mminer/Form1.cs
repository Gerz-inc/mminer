using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using baseFunc;
using System.Threading;

namespace mminer
{
    public partial class Form1 : Form
    {
        ref_bool is_running = new ref_bool(false);
        ref_bool is_miner_running = new ref_bool(false);

        bool is_stat_thread_running = false;
        object lock_is_stat_thread_running = new object();

        db_sqlite db = new db_sqlite();

        private string qry = "select a.id, a.miner, a.statistic, a.manual_diff, " +
                             "b.name as pool_name, b.url, b.pass, " +
                             "c.name as connection_type_name, " +
                             "d.name as algo_name, d.ccminer as algo_name_ccminer, " +
                             "e.name as wallet_name, e.coin, " +
                             "g.name as exchange_name " +
                             "from sets a " +
                             "left join pools b on a.id_pool = b.id " +
                             "left join connection_types c on b.id_connection_type = c.id " +
                             "left join algo d on b.id_algo = d.id " +
                             "left join wallets e on b.id_wallet = e.id " +
                             "left join exchanges g on e.id_exchange = g.id " +
                             "where a.enabled = 1; ";

        public delegate void dell(bool b);

        public Form1()
        {
            InitializeComponent();
            is_miner_running.set_on_chhange(new ref_bool.dell((bool new_val_) => 
            {
                Invoke(new dell((bool new_val) => 
                {
                    if (new_val)
                    {
                        richTextBox2.AppendText("\n[miner] ", Color.Green);
                        richTextBox2.AppendText("Starting ...");
                        toolStripButton6.Text = "Stop";
                        toolStripButton6.Image = Properties.Resources.stop;
                    }
                    else
                    {
                        if (is_running.Value) //restart miner
                        {
                            run_miner();
                        }
                        else
                        {
                            richTextBox2.AppendText("\n[miner] ", Color.Red);
                            richTextBox2.AppendText("Stopped");
                            toolStripButton6.Text = "Start";
                            toolStripButton6.Image = Properties.Resources.start;
                        }
                    }
                }), new Object[] { new_val_ });
                
            }));
        }

        /// <summary>
        /// pools
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            new pools().ShowDialog();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            new exchanges().ShowDialog();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            new wallets().ShowDialog();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            new statistics().ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dlls.load_ddls();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            new miners().ShowDialog();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (is_running.Value && is_miner_running.Value)
            {
                is_running.Value = false;
            }
            else if (!is_running.Value && is_miner_running.Value)
            {
                richTextBox2.AppendText("\n[miner] ", Color.Red);
                richTextBox2.AppendText("Waiting for the miner to be stopped");
            }
            else
            {
                run_miner();
            }
        }

        private void run_miner()
        {
            var mm = dlls.get_miners();

            string id = mm.First().Value;
            string exe = mm.First().Key;

            System.Reflection.Assembly DLL = null;
            string p = Application.StartupPath + dlls.PATH_MINERS + id + ".dll";
            if (File.Exists(p))
            {
                DLL = System.Reflection.Assembly.LoadFile(p);
                if (DLL != null)
                {
                    Type t = DLL.GetType(id + ".Call");
                    var m = t.GetMethod("run");
                    if (m == null)
                    {
                        MessageBox.Show("Method run was not found in " + id + ".dll");
                    }
                    else
                    {
                        is_running.Value = true;
                        is_miner_running.Value = true;

                        var a = Activator.CreateInstance(t);
                        var result = m.Invoke(a, new Object[] { exe, this, richTextBox1, is_miner_running });
                    }
                }
            }
            else
            {
                MessageBox.Show("File " + p + " not found");
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            RichTextBox r = sender as RichTextBox;
            r.SelectionStart = r.Text.Length;
            r.ScrollToCaret();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            new sets().ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (is_running.Value && is_miner_running.Value)
            {
                int max_len = 100000;

                string r1 = richTextBox1.Text;
                string r2 = richTextBox2.Text;

                if (r1.Length > max_len)
                {
                    richTextBox1.Select(0, r1.Length - max_len);
                    richTextBox1.SelectedText = "";
                }

                if (r2.Length > max_len)
                {
                    richTextBox2.Select(0, r2.Length - max_len);
                    richTextBox2.SelectedText = "";
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (is_running.Value && is_miner_running.Value)
            {
                lock (lock_is_stat_thread_running)
                {
                    if (is_stat_thread_running) return;
                    is_stat_thread_running = true;
                }

                //check current difficulty
                ThreadPool.QueueUserWorkItem(new WaitCallback((x) =>
                {
                    Dictionary<string, bool> check = new Dictionary<string, bool>();

                    db.select(qry, new db_sqlite.dell((System.Data.Common.DbDataRecord record) =>
                    {
                        string statistic = record["statistic"].ToString();
                        if (!check.ContainsKey(statistic)) check.Add(statistic, false);
                        return true;
                    }));

                    foreach (var it in check)
                    {
                        System.Reflection.Assembly DLL = null;
                        string p = Application.StartupPath + dlls.PATH_STATISTICS + it.Key + ".dll";
                        if (File.Exists(p))
                        {
                            DLL = System.Reflection.Assembly.LoadFile(p);
                            if (DLL != null)
                            {
                                Type t = DLL.GetType(it.Key + ".Call");
                                var m = t.GetMethod("GetDifficulty");
                                if (m != null)
                                {
                                    var a = Activator.CreateInstance(t);
                                    KeyValuePair<double, double> result = (KeyValuePair<double, double>)m.Invoke(a, new Object[] { });

                                    if (result.Key != -1) //ok key = absolute, value = relative
                                    {
                                        MessageBox.Show("" + result.Key + " " + result.Value);
                                    }
                                    else //fail
                                    {

                                    }
                                }
                            }
                        }
                    }

                    lock (lock_is_stat_thread_running)
                    {
                        is_stat_thread_running = false;
                    }
                }));
            }
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
