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
    public partial class statistics : Form
    {
        public statistics()
        {
            InitializeComponent();
        }

        private void statistics_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Reflection.Assembly DLL = null;
            string p = Application.StartupPath + "\\statistics\\smartcash.dll";
            if (File.Exists(p))
            {
                DLL = System.Reflection.Assembly.LoadFile(p);
                if (DLL != null)
                {
                    Type t = DLL.GetType("smartcash.call");
                    var m = t.GetMethod("main");
                    if (m == null)
                    {
                        //exception
                    }
                    else
                    {
                        var a = Activator.CreateInstance(t);
                        var result = m.Invoke(a, new Object[]{});
                    }
                }
            }
        }
    }
}
