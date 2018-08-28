using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.IO;

namespace DataInquiry.Assistant.Data
{
    class AccDB
    {
        private OleDbConnection _conn = null;

        protected AccDB()
        {
            string path = System.Configuration.ConfigurationManager.AppSettings["AccessDbPath"];

            if (File.Exists(path) == false)
            {
                throw new Exception("找不到" + path + ", 請確認App.config中AccessDbPath是否設定正確");
            }

            _conn = new OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + path);
            _conn.Open();
        }

        public int executeSQL(string sql)
        {
            OleDbCommand cmd = new OleDbCommand(sql, _conn);
            int rows = cmd.ExecuteNonQuery();

            return rows;
        }

        public OleDbDataReader getDataReader(string sql)
        {
            OleDbCommand cmd = new OleDbCommand(sql, _conn);
            OleDbDataReader r = cmd.ExecuteReader();

            return r;
        }

        public void close()
        {
            if (_conn != null)
            {
                _conn.Close();
            }
        }
    }
}
