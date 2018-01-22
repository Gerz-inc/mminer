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
    public partial class sets : Form
    {
        db_sqlite db = new db_sqlite();

        public sets()
        {
            InitializeComponent();
        }

        private void sets_Load(object sender, EventArgs e)
        {
            refresh();
        }

        private void refresh()
        {
            panel1.Controls.Clear();

            db.select("select a.*, b.name as algo_name, c.name as wallet_name, d.name as exchange_name, e.url " +
                      "from sets a " +
                      "left join algo b on e.id_algo = b.id " +
                      "left join wallets c on e.id_wallet = c.id " +
                      "left join exchanges d on c.id_exchange = d.id " +
                      "left join pools e on a.id_pool = e.id " +
                      "; ", new db_sqlite.dell((System.Data.Common.DbDataRecord record) =>
            {
                

                return true;
            }));
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            db.update_or_insert("insert into sets (enabled) values (0); ");
            refresh();
        }
    }
}
