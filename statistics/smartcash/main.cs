using baseFunc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace smartcash
{
    public partial class Main : Form
    {
        private Call parent_dll;

        public Main(Call dll)
        {
            InitializeComponent();
            rateLabel.Text = "";

            parent_dll = dll;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            parent_dll.ReadSettings();

            diffMinTextBox.Text = parent_dll.diff_min.ToString();
            diffMaxTextBox.Text = parent_dll.diff_max.ToString();

            poolsListView.Items.Clear();
            foreach (var it in parent_dll.pools)
                poolsListView.Items.Add(it);
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            string txt = (sender as TextBox).Text;
            if (!Char.IsDigit(number) && number != 8 && number != 44)
            {
                e.Handled = true;
            }
            else if (number == 44 && txt.Contains(","))
            {
                e.Handled = true;
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (sender as TextBox);
            Regex rgx = new Regex("[^0-9,]");
            tb.Text = rgx.Replace(tb.Text, "");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Double.TryParse(diffMinTextBox.Text, out parent_dll.diff_min);
            Double.TryParse(diffMaxTextBox.Text, out parent_dll.diff_max);
            parent_dll.SaveSettings();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            double rate = 0;
            var diff = parent_dll.GetDifficulty();
            if (diff.Key >= 0)
                rateLabel.Text = String.Format("Difficulty {0:0.###} [{1:0.##}]", diff, rate);
            else rateLabel.Text = "Something wrong";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var idxs = (sender as ListView).SelectedIndices;
            int idx = idxs.Count > 0 ? idxs[0] : -1;
            pollAddressTextBox.Text = idx > -1 ? parent_dll.pools[idx] : "";
        }
    }
}
