using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mminer
{
    public partial class work_item : UserControl
    {
        work_item_struct data;
        db_sqlite db = new db_sqlite();

        public work_item()
        {
            InitializeComponent();
        }

        public void init(ref work_item_struct data_)
        {
            data = data_;
            refresh();
        }

        public int get_id()
        {
            return data.id;
        }

        public string get_coin_stat_name()
        {
            return data.statistic;
        }

        public void refresh()
        {
            pictureBox1.Image = Image.FromFile(dlls.get_coin_image(data.coin));
            if (data.is_running) pictureBox2.Image = Properties.Resources.start;
            else pictureBox2.Image = Properties.Resources.stop;

            switch (data.get_coin_stat_state())
            {
                case work_item_struct.coin_stat_state.manual: pictureBox3.Image = Properties.Resources.gray; break;
                case work_item_struct.coin_stat_state.stat_ok: pictureBox3.Image = Properties.Resources.green; break;
                case work_item_struct.coin_stat_state.stat_error: pictureBox3.Image = Properties.Resources.red; break;
            }

            label1.Text = data.coin;
            label2.Text = data.exchange_name;
            label3.Text = data.pool_name;
            label4.Text = data.miner;
            if (data.abs_diff == -1) label7.Text = "-";
            else label7.Text = ((int)data.abs_diff).ToString();
            string diff_rel = data.get_diff().ToString();
            if (diff_rel.Length > 3) diff_rel = diff_rel.Substring(0, 3);
            label9.Text = diff_rel;
            label13.Text = data.min_running.ToString();
            if (data.min_running != 0 && data.is_running && data.start_running != DateTime.MinValue)
            {
                int running_min = data.min_running - (int)(DateTime.Now - data.start_running).TotalMinutes;
                if (running_min < 0) running_min = 0;
                label15.Text = running_min.ToString();
            }
            else label15.Text = "-";

            DateTime curr_date = DateTime.Now;

            db.select("select count(*) from times where id_pool = " + data.id + " and dat >='" + curr_date.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss") + "'; ", new db_sqlite.dell((System.Data.Common.DbDataRecord record) => 
            {
                int m = baseFunc.base_func.ParseInt32(record[0]);
                int min = m % 60;
                int h = (m - min) / 60;

                string min_str = min.ToString();
                string h_str = h.ToString();
                if (min_str.Length == 1) min_str = "0" + min_str;
                if (h_str.Length == 1) h_str = "0" + h_str;

                int perc = (int)((double)(m * 100) / 1440.0);

                label11.Text = h_str + ":" + min_str + " (" + perc + "%)";
                return true;
            }));

            if (data.cant_run)
            {
                pictureBox2.Image = Properties.Resources.warn;
            }
            else if (data.stopped_before != DateTime.MinValue)
            {
                pictureBox2.Image = Properties.Resources.warn;
            }
        }

        private void pictureBox2_MouseHover(object sender, EventArgs e)
        {
            if (data.cant_run)
            {
                toolTip1.SetToolTip(pictureBox2, "Miner not supported on your system");
            }
            else if (data.stopped_before != DateTime.MinValue)
            {
                pictureBox2.Image = Properties.Resources.warn;
                toolTip1.SetToolTip(pictureBox2, "Miner stopped before " + data.stopped_before.ToString("yyyy-MM-dd HH:mm:ss") + ". Reason: " + data.stopped_msg);
            }
        }
    }
}
