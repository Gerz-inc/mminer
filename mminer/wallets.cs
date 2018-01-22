using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using baseFunc;
using EXControls;
using System.Xml;
using System.Xml.Linq;
using System.Collections;
using System.Net;
using System.Diagnostics;

namespace mminer
{
    public partial class wallets : Form
    {
        db_sqlite db = new db_sqlite();

        bool is_redact_ = false;
        bool is_redact
        {
            get
            {
                return is_redact_;
            }
            set
            {
                if (is_redact_ != value)
                {
                    if (value)
                    {
                        button2.Visible = true;
                        button1.Text = "Cancel";
                    }
                    else
                    {
                        id_edit = "";
                        button2.Visible = false;
                        button1.Text = "Add";
                    }

                    is_redact_ = value;
                }
            }
        }
        string id_edit = "";

        public wallets()
        {
            InitializeComponent();
        }

        private void wallets_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            db.select("select * from exchanges; ", new db_sqlite.dell((System.Data.Common.DbDataRecord record) =>
            {
                base_func.ComboBoxItem it = new base_func.ComboBoxItem();
                it.Text = record["name"].ToString();
                it.Value = base_func.ParseInt32(record["id"]);

                comboBox1.Items.Add(it);
                return true;
            }));

            comboBox2.Items.Clear();
            foreach (var c in dlls.get_coins())
            {
                base_func.ComboBoxItem it = new base_func.ComboBoxItem();
                it.Text = c.Value.name;
                it.Value = c.Value.name;

                comboBox2.Items.Add(it);
            }

            refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (is_redact)
            {
                is_redact = false;
                clear_fields();
            }
            else
            {
                if (comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("Select exchange");
                    return;
                }

                if (comboBox2.SelectedIndex == -1)
                {
                    MessageBox.Show("Select coin");
                    return;
                }

                if (textBox1.Text == "")
                {
                    MessageBox.Show("Type wallet address");
                    return;
                }

                base_func.ComboBoxItem t = (base_func.ComboBoxItem)comboBox1.Items[comboBox1.SelectedIndex];
                base_func.ComboBoxItem c = (base_func.ComboBoxItem)comboBox2.Items[comboBox2.SelectedIndex];

                db.update_or_insert("insert into wallets (coin, name, id_exchange) values ('" + t.Value.ToString() + "', '" + textBox1.Text + "', " + t.Value.ToString() + "); ");

                refresh();
                clear_fields();
            }
        }

        private void clear_fields()
        {
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            textBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Select exchange");
                return;
            }

            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Select coin");
                return;
            }

            if (textBox1.Text == "")
            {
                MessageBox.Show("Type wallet address");
                return;
            }

            base_func.ComboBoxItem t = (base_func.ComboBoxItem)comboBox1.Items[comboBox1.SelectedIndex];
            base_func.ComboBoxItem c = (base_func.ComboBoxItem)comboBox2.Items[comboBox2.SelectedIndex];

            db.update_or_insert("update wallets set coin = '" + c.Value.ToString() + "', id_exchange = " + t.Value.ToString() + ", name = '" + textBox1.Text + "' where id = " + id_edit + "; ");

            is_redact = false;
            refresh();
            clear_fields();
        }

        private void refresh()
        {
            listView1.Items.Clear();

            db.select("select * from wallets; ", new db_sqlite.dell((System.Data.Common.DbDataRecord record) =>
            {
                ListViewItem item = new ListViewItem(record["id"].ToString());

                //image
                var coins = dlls.get_coins();
                if (coins.ContainsKey(record["coin"].ToString()))
                {
                    item.SubItems.Add(new EXMultipleImagesListViewSubItem(new ArrayList(new object[] { Image.FromFile(dlls.get_coin_image(coins[record["coin"].ToString()].name)) })));
                }
                else
                {
                    item.SubItems.Add("");
                }

                item.SubItems.Add(record["coin"].ToString());

                for (int i = 0; i < comboBox1.Items.Count; ++i)
                {
                    base_func.ComboBoxItem it = (base_func.ComboBoxItem)comboBox1.Items[i];
                    if (it.Value.ToString() == record["id_exchange"].ToString())
                    {
                        item.SubItems.Add(it.Text);
                        break;
                    }
                }

                item.SubItems.Add(record["name"].ToString());
                item.SubItems.Add("-");

                listView1.Items.Add(item);

                return true;
            }));
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            #region //определение элемента листвью
            int curr_item = 0;
            try
            {
                Point mousePositioni = listView1.PointToClient(Control.MousePosition);
                ListViewHitTestInfo hiti = listView1.HitTest(mousePositioni);
                curr_item = hiti.Item.Index;
            }
            catch (Exception ex) { }

            //three step to detected which of columns of items to was clicked
            int columnindex = 0;
            try
            {
                Point mousePosition = listView1.PointToClient(Control.MousePosition);
                ListViewHitTestInfo hit = listView1.HitTest(mousePosition);
                columnindex = hit.Item.SubItems.IndexOf(hit.SubItem);
            }
            catch (Exception ex) { }

            if (listView1.Items.Count == 0) return;
            #endregion

            string id = listView1.Items[curr_item].Text;

            if (columnindex == 5) //remove
            {
                if (MessageBox.Show("Remove wallet?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    db.update_or_insert("delete from wallets where id = " + id + "; ");
                    refresh();
                }
            }
            else
            {
                id_edit = id;
                is_redact = true;

                comboBox1.SelectedIndex = -1;
                comboBox2.SelectedIndex = -1;

                for (int i = 0; i < comboBox1.Items.Count; ++i)
                {
                    base_func.ComboBoxItem it = (base_func.ComboBoxItem)comboBox1.Items[i];
                    if (it.Text.ToString() == listView1.Items[curr_item].SubItems[3].Text)
                    {
                        comboBox1.SelectedIndex = i;
                        break;
                    }
                }

                for (int i = 0; i < comboBox2.Items.Count; ++i)
                {
                    base_func.ComboBoxItem it = (base_func.ComboBoxItem)comboBox2.Items[i];
                    if (it.Text.ToString() == listView1.Items[curr_item].SubItems[2].Text)
                    {
                        comboBox2.SelectedIndex = i;
                        break;
                    }
                }

                textBox1.Text = listView1.Items[curr_item].SubItems[4].Text;
            }
        }
    }
}
