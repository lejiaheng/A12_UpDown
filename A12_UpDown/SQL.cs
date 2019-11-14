using System;
using System.Data;
using System.Data.SqlClient;

namespace A12_UpDown
{
    internal class SQL
    {
        private string Path = AppDomain.CurrentDomain.BaseDirectory;

        public DataTable ExecuteQuery(string sqlStr)  //用于查询
        {
            string MySqlCon = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Path + "A12Date.mdf;Integrated Security=True;Connect Timeout=30";
            //string MySqlCon = @"data source=(LocalDB)\MSSQLLocalDB;initial catalog=" + Path + "A12Date.mdf;user id=sa;pwd=123456";
            SqlConnection con = new SqlConnection(MySqlCon);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sqlStr;
            DataTable dt = new DataTable();
            SqlDataAdapter msda;
            msda = new SqlDataAdapter(cmd);
            msda.Fill(dt);
            con.Close();
            return dt;
        }

        public int ExecuteUpdate(string sqlStr)      //用于增删改;
        {
            string MySqlCon = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Path + "A12Date.mdf;Integrated Security=True;Connect Timeout=30";
            //string MySqlCon = @"data source=(LocalDB)\MSSQLLocalDB;initial catalog=" + Path + "A12Date.mdf;user id=sa;pwd=123456";
            SqlConnection con = new SqlConnection(MySqlCon);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sqlStr;
            int iud = 0;
            iud = cmd.ExecuteNonQuery();
            con.Close();
            return iud;
        }
    }
}