using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInquiry.Assistant.Data
{
    public class SqliteConn
    {
        //private SQLiteConnection _liteConn;
        private SQLiteCommand _command;
        private string _connstr = "Data source=SqliteData.db";
        private static object _lock = new object();

        public SqliteConn()
        {            
            //_liteConn = new SQLiteConnection("Data source=SqliteData.db");            
        }
        public int executeSQL(string sql)
        {
            try
            {
                int cnt = 0;

                lock(_lock)
                {
                    using (var connection = new SQLiteConnection(_connstr))
                    {
                        connection.Open();

                        using (_command = new SQLiteCommand(sql, connection))
                        {                            
                            cnt = _command.ExecuteNonQuery();

                        }
                    }
                }                

                return cnt;
            }
            catch (Exception e)
            {
                throw e;
            }            
        }

        public Reader getDataReader(string sql)
        {
            try
            {
                SQLiteDataReader reader = null;
                Reader result = null;

                lock (_lock)
                {
                    using (var connection = new SQLiteConnection(_connstr))
                    {
                        connection.Open();

                        using (_command = new SQLiteCommand(sql, connection))
                        {
                            reader = _command.ExecuteReader();

                            result = new Reader(reader);
                        }
                    }                    
                    
                }

                return result;
            }
            catch (Exception e)
            {
                GlobalClass.errorLog(e.ToString());
                return new Reader();
            }            
        }

        public Reader getLastConnName()
        {
            Reader r = this.getDataReader("select DBConnName, DBName from FavoriteDb order by ModifiedDate desc limit 0,1");

            return r;
        }

        public void updateFavorite(string DBConnName, string DBName)
        {
            string sql = string.Format("select DBConnName from FavoriteDb where DBConnName = '{0}' and DBName = '{1}'",
                DBConnName, DBName);

            Reader r = this.getDataReader(sql);

            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if(r.Read())
            {
                sql = string.Format("update FavoriteDb set ModifiedDate = '{0}' where DBConnName = '{1}' and DBName = '{2}'",
                    now, DBConnName, DBName);
                this.executeSQL(sql);
            }
            else
            {
                sql = string.Format(@"Insert into FavoriteDb (DBConnName,DBName,ModifiedDate) 
                               values ('{0}','{1}','{2}') ", DBConnName, DBName, now);
                this.executeSQL(sql);
            }            
        }
    }
}
