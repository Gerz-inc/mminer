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

        public work_item()
        {
            InitializeComponent();
        }

        public void init(work_item_struct data_, bool is_running)
        {
            data = data_;

            pictureBox1.Image = Image.FromFile(dlls.get_coin_image(data.coin));
            if (is_running) pictureBox2.Image = Properties.Resources.start;
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
        }

        public int get_id()
        {
            return data.id;
        }

        public string get_coin_stat_name()
        {
            return data.statistic;
        }

        public void set_diff(double abs_diff, double rel_diff)
        {
            if (abs_diff != -1)
            {
                label7.Text = ((int)abs_diff).ToString();
                data.coin_stat_diff = rel_diff;
                data.abs_diff = abs_diff;
            }
            else
            {
                label7.Text = "-";
                data.coin_stat_diff = -1;
                data.abs_diff = -1;
            }

            string diff_rel = data.get_diff().ToString();
            if (diff_rel.Length > 3) diff_rel = diff_rel.Substring(0, 3);
            label9.Text = diff_rel;

            switch (data.get_coin_stat_state())
            {
                case work_item_struct.coin_stat_state.manual: pictureBox3.Image = Properties.Resources.gray; break;
                case work_item_struct.coin_stat_state.stat_ok: pictureBox3.Image = Properties.Resources.green; break;
                case work_item_struct.coin_stat_state.stat_error: pictureBox3.Image = Properties.Resources.red; break;
            }
        }

    }
}
