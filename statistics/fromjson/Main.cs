using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static fromJSON.Plugin;

namespace fromJSON
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
                item.SubItems.Add("-");
                coinsListView.Items.Add(item);
            }

            sel_coin = -1;
            modifyButton.Visible = false;
            checkButton.Visible = false;
            rateLabel.Text = "";
        }

        private void coinsListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            #region //определение элемента листвью
            int curr_item = 0;
            try
            {
                Point mousePositioni = coinsListView.PointToClient(Control.MousePosition);
                ListViewHitTestInfo hiti = coinsListView.HitTest(mousePositioni);
                curr_item = hiti.Item.Index;
            }
            catch (Exception ex) { }

            //three step to detected which of columns of items to was clicked
            int columnindex = 0;
            try
            {
                Point mousePosition = coinsListView.PointToClient(Control.MousePosition);
                ListViewHitTestInfo hit = coinsListView.HitTest(mousePosition);
                columnindex = hit.Item.SubItems.IndexOf(hit.SubItem);
            }
            catch (Exception ex) { }

            if (coinsListView.Items.Count == 0) return;
            #endregion

            // Remove
            if (columnindex == 4)
            {
                plugin.coins.RemoveAt(curr_item);
                plugin.SaveSettings();
                saved = true;

                reloadPoolsList();
                return;
            }

            sel_coin = curr_item;

            CoinSettings coin = plugin.coins[curr_item];
            coinTextBox.Text = coin.name.ToUpper();
            diffMinTextBox.Text = coin.diff_min.ToString();
            diffMaxTextBox.Text = coin.diff_max.ToString();
            urlTextBox.Text = coin.url;
            diffPathTextBox.Text = coin.diff_path;
            minTextBox.Text = coin.diff_work_min.ToString();

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
            Double.TryParse(minTextBox.Text, out coin.diff_work_min);
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

        private void modifyButton_Click(object sender, EventArgs e)
        {
            if (sel_coin < 0 || coinTextBox.Text == "") return;

            CoinSettings coin = coinFromFields();
            plugin.coins[sel_coin] = coin;
            reloadPoolsList();

            plugin.SaveSettings();
            saved = true;
        }

        private void ClearForm()
        {
            coinTextBox.Text = "";
            diffMinTextBox.Text = "";
            diffMaxTextBox.Text = "";
            urlTextBox.Text = "";
            diffPathTextBox.Text = "";
            minTextBox.Text = "";
        }


        private void addButton_Click(object sender, EventArgs e)
        {
            if (coinTextBox.Text == "") return;

            CoinSettings coin = coinFromFields();
            plugin.coins.Add(coin);

            sel_coin = -1;
            ClearForm();
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
