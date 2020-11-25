using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace EStudio
{
    class DBhelper
    {
        public static string ConnStr = @"server=127.0.0.1;uid=root;pwd=123456;database=words;charset=utf8";
        public static string wrongconnstr = @"server=127.0.0.1;uid=root;pwd=123456;database=wrongwords;charset=utf8";
        public static int wrongwordid = 1;
        //打开数据库链接
        public static MySqlConnection Open_Conn(string ConnStr)
        {
            MySqlConnection Conn = new MySqlConnection(ConnStr);
            Conn.Open();
            return Conn;
        }
        //关闭数据库链接
        public static void Close_Conn(MySqlConnection Conn)
        {
            if (Conn != null)
            {
                Conn.Close();
                Conn.Dispose();
            }
            GC.Collect();
        }
        //运行MySql语句
        public static int Run_SQL(string SQL, string ConnStr)
        {
            MySqlConnection Conn = Open_Conn(ConnStr);
            MySqlCommand Cmd = Create_Cmd(SQL, Conn);
            try
            {
                int result_count = Cmd.ExecuteNonQuery();
                Close_Conn(Conn);
                return result_count;
            }
            catch
            {
                Close_Conn(Conn);
                return 0;
            }
        }
        // 生成Command对象 
        public static MySqlCommand Create_Cmd(string SQL, MySqlConnection Conn)
        {
            MySqlCommand Cmd = new MySqlCommand(SQL, Conn);
            return Cmd;
        }
        // 运行MySql语句返回 DataTable
        public static DataTable ReadAllData(string SQL, string ConnStr)
        {
            MySqlDataAdapter Da = Get_Adapter(SQL, ConnStr);
            DataTable dt = new DataTable();
            Da.Fill(dt);
            return dt;
        }



        public static void InsertData(string word, string explain)
        {
            int flag = 0;
            //String sql = "insert into wrongword(words,trans,flag)values( '" + word + "' , '" + explain + "' , '" + flag + "')" + " FROM DUAL WHERE NOT EXISTS(SELECT * FROM wrongword WHERE words = '" + word + "')";
            //string sql = "INSERT INTO cs(sex,gin) SELECT '" + word + "', '" + explain + "'FROM DUAL WHERE NOT EXISTS(SELECT * FROM cs WHERE sex = '" + word + "')";

          
            MySqlConnection Conn = Open_Conn(wrongconnstr);
            string counts = "select count(*) from wrongword";
            MySqlCommand cmd = new MySqlCommand(counts, Conn);
            int id = Convert.ToInt32(cmd.ExecuteScalar())+1;
            string sql = "INSERT INTO wrongword(id,words,trans,flag) SELECT '" + id + "', '" + word + "', '" + explain + "', '" + flag + "'FROM DUAL WHERE NOT EXISTS(SELECT * FROM wrongword WHERE words = '" + word + "')";
            cmd = new MySqlCommand(sql, Conn);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            if(count != 0)
            {
                return;
            }
            cmd.ExecuteNonQuery();
        }


        public static void DeleteData(string ID)
        {
            String sql = "delete from cs where[sex] =" + ID;
            MySqlConnection Conn = Open_Conn(ConnStr);
            MySqlCommand cmd = new MySqlCommand(sql, Conn);

            cmd.ExecuteNonQuery();
        }



        // 运行MySql语句返回 MySqlDataReader对象
        public static MySqlDataReader Get_Reader(string SQL, string ConnStr)
        {
            MySqlConnection Conn = Open_Conn(ConnStr);
            MySqlCommand Cmd = Create_Cmd(SQL, Conn);
            MySqlDataReader Dr;
            try
            {
                Dr = Cmd.ExecuteReader(CommandBehavior.Default);
            }
            catch
            {
                throw new Exception(SQL);
            }
            Close_Conn(Conn);
            return Dr;
        }
        // 运行MySql语句返回 MySqlDataAdapter对象 
        public static MySqlDataAdapter Get_Adapter(string SQL, string ConnStr)
        {
            MySqlConnection Conn = Open_Conn(ConnStr);
            MySqlDataAdapter Da = new MySqlDataAdapter(SQL, Conn);
            return Da;
        }
        // 运行MySql语句,返回DataSet对象
        public static DataSet Get_DataSet(string SQL, string ConnStr, DataSet Ds)
        {
            MySqlDataAdapter Da = Get_Adapter(SQL, ConnStr);
            try
            {
                Da.Fill(Ds);
            }
            catch (Exception Err)
            {
                throw Err;
            }
            return Ds;
        }
        // 运行MySql语句,返回DataSet对象
        public static DataSet Get_DataSet(string SQL, string ConnStr, DataSet Ds, string tablename)
        {
            MySqlDataAdapter Da = Get_Adapter(SQL, ConnStr);
            try
            {
                Da.Fill(Ds, tablename);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return Ds;
        }
        // 运行MySql语句,返回DataSet对象，将数据进行了分页
        public static DataSet Get_DataSet(string SQL, string ConnStr, DataSet Ds, int StartIndex, int PageSize, string tablename)
        {
            MySqlConnection Conn = Open_Conn(ConnStr);
            MySqlDataAdapter Da = Get_Adapter(SQL, ConnStr);
            try
            {
                Da.Fill(Ds, StartIndex, PageSize, tablename);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            Close_Conn(Conn);
            return Ds;
        }
        // 返回MySql语句执行结果的第一行第一列
        public static string Get_Row1_Col1_Value(string SQL, string ConnStr)
        {
            MySqlConnection Conn = Open_Conn(ConnStr);
            string result;
            MySqlDataReader Dr;
            try
            {
                Dr = Create_Cmd(SQL, Conn).ExecuteReader();
                if (Dr.Read())
                {
                    result = Dr[0].ToString();
                    Dr.Close();
                }
                else
                {
                    result = "";
                    Dr.Close();
                }
            }
            catch
            {
                throw new Exception(SQL);
            }
            Close_Conn(Conn);
            return result;
        }
    }
}
