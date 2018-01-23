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

        public static Dictionary<int, string> pools = new Dictionary<int, string>();

        public sets()
        {
            InitializeComponent();

            pools.Clear();

            db.select("select a.*, b.name as algo_name, c.name as wallet_name, c.coin, d.name as exchange_name " +
                      "from pools a " +
                      "left join algo b on a.id_algo = b.id " +
                      "left join wallets c on a.id_wallet = c.id " +
                      "left join exchanges d on c.id_exchange = d.id " +
                      "; ", new db_sqlite.dell((System.Data.Common.DbDataRecord record) =>
                      {
                          int id = baseFunc.base_func.ParseInt32(record["id"]);
                          string pool = record["name"].ToString() + ", " + record["coin"].ToString() + " on " + record["exchange_name"].ToString() + " (" + record["wallet_name"].ToString() + ")";

                          pools.Add(id, pool);

                          return true;
                      }));
        }

        private void sets_Load(object sender, EventArgs e)
        {
            refresh();
        }

        private void refresh()
        {
            panel1.Controls.Clear();

            db.select("select * from sets; ", new db_sqlite.dell((System.Data.Common.DbDataRecord record) =>
            {
                int id = baseFunc.base_func.ParseInt32(record["id"]);

                int en = baseFunc.base_func.ParseInt32(record["enabled"]);

                bool enabled = en == 1 ? true : false;
                int id_pool = baseFunc.base_func.ParseInt32(record["id_pool"]);

                set_item item = new set_item();
                item.set(id, enabled, id_pool, record["miner"].ToString(), record["statistic"].ToString(), record["manual_diff"].ToString(), new set_item.dell(() => 
                {
                    refresh();
                }));

                item.Left = 3;
                item.Top = (panel1.Controls.Count * (item.Height + 3)) + 3;

                panel1.Controls.Add(item);

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
