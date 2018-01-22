using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace mminer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// pools
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            new pools().ShowDialog();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            new exchanges().ShowDialog();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            new wallets().ShowDialog();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            new statistics().ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dlls.load_ddls();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            new miners().ShowDialog();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            var mm = dlls.get_miners();
            string f = "";

            string id = mm.First().Value;
            string exe = mm.First().Key;

            System.Reflection.Assembly DLL = null;
            string p = Application.StartupPath + dlls.PATH_MINERS + id + ".dll";
            if (File.Exists(p))
            {
                DLL = System.Reflection.Assembly.LoadFile(p);
                if (DLL != null)
                {
                    Type t = DLL.GetType(id + ".Call");
                    var m = t.GetMethod("run");
                    if (m == null)
                    {
                        MessageBox.Show("Method run was not found in " + id + ".dll");
                    }
                    else
                    {
                        var a = Activator.CreateInstance(t);
                        var result = m.Invoke(a, new Object[] { exe, richTextBox1 });
                    }
                }
            }
            else
            {
                MessageBox.Show("File " + p + " not found");
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            // set the current caret position to the end
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            // scroll it automatically
            richTextBox1.ScrollToCaret();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            new sets().ShowDialog();
        }
    }
}
