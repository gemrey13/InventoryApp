using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.DAL
{
    public class DatabaseManager
    {
        public SqlConnection GetConnection()
        {
            return new SqlConnection("Data Source=DESKTOP-7MH9GC7\\SQLEXPRESS;Initial Catalog=InventoryDB;Integrated Security=True;Encrypt=False");
        }

    }
}
