using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
    }
}
