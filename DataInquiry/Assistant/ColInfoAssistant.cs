using DataInquiry.Assistant.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataInquiry.Assistant
{
    public class ColInfoAssistant
    {
        private SqliteConn _db;
        private static object _obj = new object();  // critical section obj

        public ColInfoAssistant()
        {
            _db = new SqliteConn();
        }

        public DataTable getColumnInfo(string dbconnName, string dbName, string tableName, DBConn engine)
        {
            GlobalClass.debugLog("ColInfoAssistant", string.Format("getColumnInfo, dbconnName:{0},dbName:{1},tableName:{2}, dbstr:{3}", 
                    dbconnName, dbName, tableName, engine.Dbstr));
            _db.executeSQL(string.Format("delete from ColumnInfo where TableInfoSn in (select distinct TableInfoSn ColumnInfo where ModifiedDate < '{0}') ", this.EffectiveDate));

            DataTable dt = new DataTable();
            dt.Columns.Add("ColumnName");

            // 取得 TableInfoSn
            string sqlGetSn = string.Format(@"select sn from TableInfo 
                where DBConnName = '{0}' and DBName = '{1}' and TableName = '{2}' ",
                dbconnName, dbName, tableName);

            Reader r = _db.getDataReader(sqlGetSn);

            if(r.Read() == false)
            {                
                return dt;
            }

            string tableSn = r[0].ToString();

            // 找出columns
            string sql = string.Format(@"select ColName from ColumnInfo where TableInfoSn = '{0}' ", tableSn);

            r = _db.getDataReader(sql);

            while (r.Read())
            {
                DataRow dr = dt.NewRow();

                dr["ColumnName"] = r[0].ToString();
                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();

            if (dt.Rows.Count == 0)
            {
                GlobalClass.debugLog("ColInfoAssistant", "getColumnInfo, invokeColumnInfoThread");
                invokeColumnInfoThread(dbName, tableName, tableSn, engine);
            }

            return dt;
        }

        private static Thread _columnThread = null;
        private static Hashtable _columnThreadParam;
        private void invokeColumnInfoThread(string dbName, string tableName, string tableSn, DBConn engine)
        {
            lock (ColInfoAssistant._obj)
            {
                if (ColInfoAssistant._columnThread != null)
                {
                    GlobalClass.debugLog("ColInfoAssistant", "invokeColumnInfoThread, running");
                    return;  // running;
                }

                ColInfoAssistant._columnThread = new Thread(new ParameterizedThreadStart(threadGetColInfo));
                ColInfoAssistant._columnThreadParam = new Hashtable();
                ColInfoAssistant._columnThreadParam["engine"] = engine;
                ColInfoAssistant._columnThreadParam["dbName"] = dbName;
                ColInfoAssistant._columnThreadParam["tableName"] = tableName;
                ColInfoAssistant._columnThreadParam["tableSn"] = tableSn;

                ColInfoAssistant._columnThread.Start(ColInfoAssistant._columnThreadParam);
            }
        }

        private void threadGetColInfo(object param)
        {
            Hashtable pParam = (Hashtable)param;

            try
            {
                GlobalClass.debugLog("ColInfoAssistant", "threadGetColInfo start");

                DBConn engine = pParam["engine"] as DBConn;
                string dbName = pParam["dbName"] as string;
                string tableName = pParam["tableName"] as string;
                string tableSn = pParam["tableSn"] as string;

                string sql = string.Format("use [" + dbName + "]; " +
                            "select COLUMN_NAME " +
                            "from INFORMATION_SCHEMA.COLUMNS with(nolock) " +
                            "where TABLE_NAME = '{0}' and TABLE_CATALOG='{1}' " 
                            , tableName, dbName);
                DataTable result = engine.getData(sql);

                SqliteConn lite = new SqliteConn();                

                string now = GlobalClass.now();
                string insSql = @"insert into ColumnInfo (TableInfoSn, ColName, ModifiedDate) 
                                values('{0}','{1}','{2}')";
                for (int i = 0; i < result.Rows.Count; i++)
                {
                    string colName = result.Rows[i]["COLUMN_NAME"].ToString();                    

                    lite.executeSQL(string.Format(insSql,
                        tableSn, colName, now ));
                }

                GlobalClass.debugLog("ColInfoAssistant", "threadGetColInfo end, rows: " + result.Rows.Count.ToString());
            }
            catch (Exception e)
            {
                GlobalClass.debugLog("ColInfoAssistant", "threadGetColInfo " + e.ToString());
                pParam["message"] = e.ToString();
            }
            finally
            {
                ColInfoAssistant._columnThread = null;
            }
        }

        public string EffectiveDate
        {
            get
            {
                return DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
}
