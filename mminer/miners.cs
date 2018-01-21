using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Net;

namespace mminer
{
    public partial class miners : Form
    {
        public miners()
        {
            InitializeComponent();
        }

        private void miners_Load(object sender, EventArgs e)
        {
            foreach (string it in dlls.get_miners())
            {
                ListViewItem item = new ListViewItem(it);
                listView1.Items.Add(item);
            }
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

            System.Reflection.Assembly DLL = null;
            string p = Application.StartupPath + dlls.PATH_MINERS + id + ".dll";
            if (File.Exists(p))
            {
                DLL = System.Reflection.Assembly.LoadFile(p);
                if (DLL != null)
                {
                    Type t = DLL.GetType(id + ".Call");
                    var m = t.GetMethod("ShowSettings");
                    if (m == null)
                    {
                        MessageBox.Show("Method ShowSettings was not found in " + id + ".dll");
                    }
                    else
                    {
                        var a = Activator.CreateInstance(t);
                        var result = m.Invoke(a, new Object[] { });
                    }
                }
            }
            else
            {
                MessageBox.Show("File " + p + " not found");
            }
        }
    }
}
