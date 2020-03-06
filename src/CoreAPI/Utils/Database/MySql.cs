using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Record = System.Collections.Generic.Dictionary<string, string>;
using RecordCollection = System.Collections.Generic.List<System.Collections.Generic.Dictionary<string,string>>;

namespace CoreAPI.Utils.Database
{
    public class MySql
    {
        public static MySqlConnection GetConnection()
        {
            string connection = "Server=localhost;Database=mydb;Uid=root;Pwd=root;SslMode=Preferred;";
            return new MySqlConnection(connection);
        }

        public static RecordCollection Query(string stmt)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            using (MySqlConnection conn = MySql.GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(stmt,conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Record record = new Record();
                        List<string> lst = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            record.Add(reader.GetName(i), reader.GetValue(i) + "");
                        }
                        result.Add(record);
                    }
                }
            }
            return result;
        }

        public static string SaltedPassword(string password, string salt)
        {
            SHA384 sha = SHA384.Create();

            byte[] bHash = sha.ComputeHash(Encoding.ASCII.GetBytes(password));
            string hash = BitConverter.ToString(bHash).Replace("-", "");
            
            string p1 = salt.Substring(0, 32);
            string p2 = salt.Substring(48, 32);
            byte[] mixed = Encoding.ASCII.GetBytes(p2 + hash + p1);

            return BitConverter.ToString(sha.ComputeHash(mixed)).Replace("-","");
        }
    }
}
