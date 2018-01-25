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
using static fromjson.Plugin;

namespace fromjson
{
    public partial class Main : Form
    {
        private Plugin plugin;
        private int sel_coin = -1;
        private bool saved = true;

        public Main(Plugin plugin)
        {
            InitializeComponent();
            this.plugin = plugin;

            deleteButton.Visible = false;
            modifyButton.Visible = false;
            checkButton.Visible = false;
            rateLabel.Text = "";
        }

        private void Main_Load(object sender, EventArgs e)
        {
            plugin.ReadSettings();
            reloadPoolsList();
        }

        private void reloadPoolsList()
        {
            coinsListView.Items.Clear();
            foreach (var coin in plugin.coins)
            {
                ListViewItem item = new ListViewItem(coin.name.ToUpper());
                item.SubItems.Add(String.Format("{0:0.00} - {1:0.00}", coin.diff_min, coin.diff_max));
                item.SubItems.Add(coin.url);
                item.SubItems.Add(coin.diff_path);
                coinsListView.Items.Add(item);
            }

            sel_coin = -1;
            deleteButton.Visible = false;
            modifyButton.Visible = false;
            checkButton.Visible = false;
            rateLabel.Text = "";
        }
        
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var idxs = (sender as ListView).SelectedIndices;
            int idx = idxs.Count > 0 ? idxs[0] : -1;
            sel_coin = idx;
            if (idx < 0) return;

            CoinSettings coin = plugin.coins[idx];
            coinTextBox.Text = coin.name.ToUpper();
            diffMinTextBox.Text = coin.diff_min.ToString();
            diffMaxTextBox.Text = coin.diff_max.ToString();
            urlTextBox.Text = coin.url;
            diffPathTextBox.Text = coin.diff_path;

            deleteButton.Visible = sel_coin >= 0;
            modifyButton.Visible = sel_coin >= 0;
            checkButton.Visible = sel_coin >= 0;
            rateLabel.Text = "";
        }

        private CoinSettings coinFromFields()
        {
            CoinSettings coin = new CoinSettings();
            coin.name = coinTextBox.Text.ToLower();
            Double.TryParse(diffMinTextBox.Text, out coin.diff_min);
            Double.TryParse(diffMaxTextBox.Text, out coin.diff_max);
            coin.url = urlTextBox.Text;
            coin.diff_path = diffPathTextBox.Text;
            return coin;
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
            plugin.SaveSettings();
            saved = true;

            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void modifyButton_Click(object sender, EventArgs e)
        {
            if (sel_coin < 0 || coinTextBox.Text == "") return;

            CoinSettings coin = coinFromFields();
            plugin.coins[sel_coin] = coin;
            reloadPoolsList();
            saved = false;
        }
        
        private void addButton_Click(object sender, EventArgs e)
        {
            if (coinTextBox.Text == "") return;

            CoinSettings coin = coinFromFields();
            plugin.coins.Add(coin);
            reloadPoolsList();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (sel_coin < 0) return;

            plugin.coins.RemoveAt(sel_coin);
            reloadPoolsList();
        }

        private void checkButton_Click(object sender, EventArgs e)
        {
            if (sel_coin < 0) return;
            CoinSettings coin = plugin.coins[sel_coin];
            var diff = plugin.GetCoinDifficulty(ref coin);
            if (diff.Key >= 0)
            {
                string txt = String.Format("Difficulty {0:0.000} [{1:0.00}]", diff.Key, diff.Value);
                rateLabel.Text = txt;
            }
            else rateLabel.Text = "Something wrong";
            plugin.coins[sel_coin] = coin;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Button bt = (sender as Button);
            if (bt == null || bt.Name != "closeButton")
            {
                if (!saved)
                {
                    if (MessageBox.Show(this, "Save changes?", "Attention!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        plugin.SaveSettings();
                }
            }
        }
    }
}
