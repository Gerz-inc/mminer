using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Windows.Forms;

namespace mminer
{
    /// <summary>
    /// not thread-safe
    /// </summary>
    class db_sqlite
    {
        public delegate bool dell(System.Data.Common.DbDataRecord r);

        private SQLiteConnection connection_db_ram;

        public bool select(string qry, dell del)
        {
            SQLiteConnection c = get_db_ram_connection();

            bool ok = true;

            try
            {
                SQLiteCommand command = new SQLiteCommand(qry, c);
                SQLiteDataReader reader = command.ExecuteReader();

                foreach (System.Data.Common.DbDataRecord record in reader)
                {
                    try
                    {
                        if (!del(record)) break;
                    }
                    catch(Exception ex) { MessageBox.Show("Error in db_sqlite: " + ex.Message + "\n" + ex.StackTrace); }
                }
            }
            catch(Exception ex) { ok = false; MessageBox.Show(ex.Message); }

            return ok;
        }

        public bool update_or_insert(string qry)
        {
            SQLiteConnection c = get_db_ram_connection();

            bool ok = true;

            try
            {
                SQLiteCommand command_u = c.CreateCommand();
                SQLiteTransaction transaction = c.BeginTransaction();
                command_u.CommandText = qry;
                command_u.ExecuteNonQuery();
                transaction.Commit();
                command_u.Dispose();
            }
            catch(Exception ex) { ok = false; MessageBox.Show(ex.Message); }

            return ok;
        }

        private SQLiteConnection get_db_ram_connection()
        {
            if (connection_db_ram != null) return connection_db_ram;

            try
            {
                connection_db_ram = new SQLiteConnection("Data Source=data.db");
                connection_db_ram.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine("[AP] get_db_ram_connection(): " + ex.Message);
                connection_db_ram = null;
            }

            return connection_db_ram;
        }
    }
}
