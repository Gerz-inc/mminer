using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace smartcash
{
    public partial class Main : Form
    {
        private Plugin plugin;
        private int sel_pool = -1;

        public Main(Plugin plugin)
        {
            InitializeComponent();
            this.plugin = plugin;

            deleteButton.Visible = false;
            modifyButton.Visible = false;
            rateLabel.Text = "";
        }

        private void Main_Load(object sender, EventArgs e)
        {
            plugin.ReadSettings();

            diffMinTextBox.Text = plugin.diff_min.ToString();
            diffMaxTextBox.Text = plugin.diff_max.ToString();

            reloadPoolsList();
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
            Double.TryParse(diffMinTextBox.Text, out plugin.diff_min);
            Double.TryParse(diffMaxTextBox.Text, out plugin.diff_max);
            plugin.SaveSettings();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var diff = plugin.GetDifficulty();
            if (diff.Key >= 0)
            {
                string txt = String.Format("Difficulty {0:0.000} [{1:0.00}]", diff.Key, diff.Value);
                rateLabel.Text = txt;
            }
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
            addressRichTextBox.Text = idx > -1 ? plugin.pools[idx] : "";
            sel_pool = idx;

            deleteButton.Visible = sel_pool >= 0;
            modifyButton.Visible = sel_pool >= 0;
        }

        private void modifyButton_Click(object sender, EventArgs e)
        {
            if (sel_pool < 0 || addressRichTextBox.Text == "") return;
            plugin.pools[sel_pool] = addressRichTextBox.Text;
            
            reloadPoolsList();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (addressRichTextBox.Text == "") return;
            plugin.pools.Add(addressRichTextBox.Text);
            
            reloadPoolsList();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (sel_pool < 0) return;
            plugin.pools.RemoveAt(sel_pool);

            reloadPoolsList();
        }

        private void reloadPoolsList()
        {
            sel_pool = -1;

            poolsListView.Items.Clear();
            foreach (var it in plugin.pools)
                poolsListView.Items.Add(it);

            deleteButton.Visible = false;
            modifyButton.Visible = false;
        }
    }
}
