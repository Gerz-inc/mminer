using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using baseFunc;

namespace mminer
{
    public partial class algo : Form
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

        public algo()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Type exchange name");
                return;
            }

            db.update_or_insert("update algo set name = '" + textBox1.Text + "' where id = " + id_edit + "; ");

            is_redact = false;
            refresh();
            clear_fields();
        }

        private void clear_fields()
        {
            textBox1.Text = "";
        }

        private void refresh()
        {
            listView1.Items.Clear();

            db.select("select * from algo; ", new db_sqlite.dell((System.Data.Common.DbDataRecord record) =>
            {
                ListViewItem item = new ListViewItem(record["id"].ToString());

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

            if (columnindex == 2) //remove
            {
                if (MessageBox.Show("Remove exchange?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    db.update_or_insert("delete from algo where id = " + id + "; ");
                    refresh();
                }
            }
            else
            {
                id_edit = id;
                is_redact = true;

                textBox1.Text = listView1.Items[curr_item].SubItems[1].Text;
            }
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
                if (textBox1.Text == "")
                {
                    MessageBox.Show("Type exchange name");
                    return;
                }

                db.update_or_insert("insert into algo (name) values ('" + textBox1.Text + "'); ");

                refresh();
                clear_fields();
            }
        }

        private void algo_Load(object sender, EventArgs e)
        {
            refresh();
        }
    }
}
