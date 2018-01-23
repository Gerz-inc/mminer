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
        private Dictionary<int, work_item_struct> enabled_workers = new Dictionary<int, work_item_struct>(); 

        ref_bool is_running = new ref_bool(false);
        ref_bool is_miner_running = new ref_bool(false);
        int current_id_running = 0;

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
            refresh_workers();
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
            var miner_settings = enabled_workers[current_id_running];

            if (mm.ContainsKey(miner_settings.miner))
            {
                string id = mm[miner_settings.miner];
                string exe = miner_settings.miner;

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

                            string args = "-a " + miner_settings.algo_name_ccminer + " -o " + miner_settings.connection_type_name +
                                          miner_settings.url + " -u " + miner_settings.wallet_name + " -p " + miner_settings.pass;

                            var a = Activator.CreateInstance(t);
                            var result = m.Invoke(a, new Object[] { exe, args, this, richTextBox1, is_miner_running, is_running });
                        }
                    }
                }
                else
                {
                    MessageBox.Show("File " + p + " not found");
                }
            }
            else
            {
                is_miner_running.Value = false;
                is_running.Value = false;

                richTextBox2.AppendText("\n[miner] ", Color.Red);
                richTextBox2.AppendText("Miner not found: " + miner_settings.miner);
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
            if (is_running.Value || is_miner_running.Value)
            {
                MessageBox.Show("Stop mining before");
                return;
            }

            new sets().ShowDialog();

            refresh_workers();
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

                                    Invoke(new dell((bool i) =>
                                    {
                                        foreach (Control c in panel1.Controls)
                                        {
                                            if (c is work_item)
                                            {
                                                work_item item = c as work_item;
                                                if (item.get_coin_stat_name() == it.Key)
                                                {
                                                    item.set_diff(result.Key, result.Value);
                                                }
                                            }
                                        }

                                        foreach (var c in enabled_workers)
                                        {
                                            if (c.Value.statistic == it.Key)
                                            {
                                                if (result.Key != -1)
                                                {
                                                    c.Value.coin_stat_diff = result.Value;
                                                    c.Value.abs_diff = result.Key;
                                                }
                                                else
                                                {
                                                    c.Value.coin_stat_diff = -1;
                                                    c.Value.abs_diff = -1;
                                                }
                                            }
                                        }

                                        sort_workers();

                                    }), new Object[] { false });

                                    
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

        private void refresh_workers()
        {
            enabled_workers.Clear();
            db.select(qry, new db_sqlite.dell((System.Data.Common.DbDataRecord record) =>
            {
                work_item_struct item = new work_item_struct();

                item.id = base_func.ParseInt32(record["id"]);
                item.statistic = record["statistic"].ToString();
                item.miner = record["miner"].ToString();
                item.manual_diff = base_func.ParseDouble(record["manual_diff"]);
                item.pool_name = record["pool_name"].ToString();
                item.url = record["url"].ToString();
                item.pass = record["pass"].ToString();
                item.connection_type_name = record["connection_type_name"].ToString();
                item.algo_name = record["algo_name"].ToString();
                item.algo_name_ccminer = record["algo_name_ccminer"].ToString();
                item.wallet_name = record["wallet_name"].ToString();
                item.coin = record["coin"].ToString();
                item.exchange_name = record["exchange_name"].ToString();

                enabled_workers.Add(item.id, item);

                return true;
            }));

            sort_workers();
        }

        private void sort_workers()
        {
            var lnq = from it in enabled_workers
                      orderby it.Value.get_diff()
                      select it;

            panel1.Controls.Clear();

            bool first = true;
            bool change_coin = false;
            foreach (var it in lnq)
            {
                if (first)
                {
                    if (it.Key != current_id_running)
                    {
                        change_coin = true;
                        current_id_running = it.Key;
                    }
                }

                work_item item = new work_item();
                item.init(it.Value, first);
                item.Left = 3;
                item.Top = (panel1.Controls.Count * (item.Height + 3)) + 3;
                panel1.Controls.Add(item);

                first = false;
            }

            if (change_coin)
            {
                richTextBox2.AppendText("\n[strategy] ", Color.Green);
                richTextBox2.AppendText("Changing coin");
                is_miner_running.Value = false;
            }
        }
    }

    public class work_item_struct
    {
        public enum coin_stat_state
        {
            stat_ok,
            stat_error,
            manual
        };

        public int id;
        public string statistic;
        public string miner;
        public double manual_diff;
        public double abs_diff;
        public string pool_name;
        public string url;
        public string pass;
        public string connection_type_name;
        public string algo_name;
        public string algo_name_ccminer;
        public string wallet_name;
        public string coin;
        public string exchange_name;
        public double coin_stat_diff = -1;

        public double get_diff()
        {
            if (coin_stat_diff != -1) return coin_stat_diff;
            return manual_diff;
        }

        public coin_stat_state get_coin_stat_state()
        {
            if (statistic == "" || statistic == "Manual") return coin_stat_state.manual;
            if (coin_stat_diff != -1) return coin_stat_state.stat_ok;
            return coin_stat_state.stat_error;
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
