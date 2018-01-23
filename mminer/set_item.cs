using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using baseFunc;

namespace mminer
{
    public partial class set_item : UserControl
    {
        db_sqlite db = new db_sqlite();
        public delegate void dell();
        dell del;

        private int id;

        public set_item()
        {
            InitializeComponent();
            foreach(var p in sets.pools)
            {
                base_func.ComboBoxItem it = new base_func.ComboBoxItem();
                it.Text = p.Value;
                it.Value = p.Key;

                comboBox1.Items.Add(it);
            }

            foreach (var it in dlls.get_miners())
            {
                comboBox2.Items.Add(it.Key);
            }

            comboBox5.Items.Add("Manual");
            foreach (string it in dlls.get_statistics())
            {
                comboBox5.Items.Add(it);
            }
        }

        public void set(int id_, bool enabled, int id_pool, string miner, string diff, string manual_diff, dell del_)
        {
            id = id_;
            del = del_;

            checkBox1.Checked = enabled;
            for (int i = 0; i < comboBox1.Items.Count; ++i)
            {
                base_func.ComboBoxItem it = (base_func.ComboBoxItem)comboBox1.Items[i];
                if ((int)it.Value == id_pool)
                {
                    comboBox1.SelectedIndex = i;
                    break;
                }
            }

            for (int i = 0; i < comboBox2.Items.Count; ++i)
            {
                var it = comboBox2.Items[i];
                if (it.ToString() == miner)
                {
                    comboBox2.SelectedIndex = i;
                    break;
                }
            }

            for (int i = 0; i < comboBox5.Items.Count; ++i)
            {
                var it = comboBox5.Items[i];
                if (it.ToString() == diff)
                {
                    comboBox5.SelectedIndex = i;
                    break;
                }
            }

            textBox1.Text = manual_diff;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Remove set?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                db.update_or_insert("delete from sets where id = " + id + "; ");
                del();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Select pool");
                return;
            }

            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Select miner");
                return;
            }

            if (comboBox5.SelectedIndex == -1)
            {
                MessageBox.Show("Select difficulty");
                return;
            }

            if (textBox1.Text == "")
            {
                MessageBox.Show("Type manual difficulty");
                return;
            }

            base_func.ComboBoxItem t = (base_func.ComboBoxItem)comboBox1.Items[comboBox1.SelectedIndex];

            string m = (baseFunc.base_func.ParseDouble(textBox1.Text).ToString()).Replace(",", ".");
            string enabled = checkBox1.Checked ? "1" : "0";

            db.update_or_insert("update sets set miner = '" + comboBox2.SelectedItem.ToString() + "', statistic = '" + comboBox5.SelectedItem.ToString() + "', id_pool = " + t.Value.ToString() + ", manual_diff = " + m + ", enabled = " + enabled + " where id = " + id + "; ");
            del();
        }
    }
}
