using DataInquiry.Assistant.Assistant;
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
    public class TableInfoAssistant
    {
        private SqliteConn _db;
        private static object _obj = new object();  // critical section obj

        public TableInfoAssistant()
        {
            _db = new SqliteConn();
        }

        /// <summary>
        /// 取得table/view
        /// </summary>
        /// <param name="dbconnName"></param>
        /// <returns></returns>
        public DataTable getTables(string dbconnName, string dbName, DBConn engine)
        {
            GlobalClass.debugLog("TableInfoAssistant", string.Format("getTables, dbconnName={0}, dbName={1}, dbstr={2}", dbconnName, dbName, engine.Dbstr));

            CodeRefresh cr = new CodeRefresh();
            cr.refreshTableInfo(dbconnName, dbName, engine);

            // 取得local儲存的table
            string sql = string.Format(@"select TableName from TableInfo 
                where DBConnName = '{0}' and DBName = '{1}' order by Favorite desc, TableName asc ",
                dbconnName, dbName);

            Reader r = _db.getDataReader(sql);            

            DataTable dt = new DataTable();
            dt.Columns.Add("TableName");            

            while(r.Read())
            {
                DataRow dr = dt.NewRow();

                dr["TableName"] = r[0].ToString();
                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();
                       

            return dt;
        }   

        public string EffectiveDate
        {
            get
            {
                return DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        //private static Thread _tableThread = null;
        //private static Hashtable _tableThreadParam;
        //private void invokeTableThread(string dbconnName, string dbName, string tableName, DBConn engine)
        //{
        //    try
        //    {
        //        lock (TableInfoAssistant._obj)
        //        {
        //            if (TableInfoAssistant._tableThread != null)
        //            {
        //                GlobalClass.debugLog("TableInfoAssistant", "invokeTableThread, running");
        //                return;  // running;
        //            }

        //            TableInfoAssistant._tableThread = new Thread(new ParameterizedThreadStart(threadGetTable));
        //            TableInfoAssistant._tableThreadParam = new Hashtable();
        //            TableInfoAssistant._tableThreadParam["engine"] = engine;
        //            TableInfoAssistant._tableThreadParam["dbconnName"] = dbconnName;
        //            TableInfoAssistant._tableThreadParam["dbName"] = dbName;
        //            TableInfoAssistant._tableThreadParam["tableName"] = tableName;

        //            TableInfoAssistant._tableThread.Start(TableInfoAssistant._tableThreadParam);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        GlobalClass.errorLog(e.ToString());
        //    }
            
        //}

//        private void threadGetTable(object param)
//        {
//            Hashtable pParam = (Hashtable)param;

//            try
//            {                
//                DBConn engine = pParam["engine"] as DBConn;
//                string dbconnName = pParam["dbconnName"] as string;
//                string dbName = pParam["dbName"] as string;

//                GlobalClass.debugLog("TableInfoAssistant", "threadGetTable start" + engine.Dbstr);

//                string sql = "select * from " + dbName + ".INFORMATION_SCHEMA.TABLES with(nolock) where TABLE_NAME not like 'syncobj_%'";
//                DataTable result = engine.getData(sql);

//                SqliteConn lite = new SqliteConn();

//                lite.executeSQL(string.Format("delete from TableInfo where DBConnName = '{0}' and DBName = '{1}'", dbconnName, dbName));

//                string now = GlobalClass.now();
//                string insSql = @"insert into TableInfo (DBConnName, DBName, TableName, ModifiedDate, TableType) 
//                                values('{0}','{1}','{2}','{3}','{4}')";
//                for(int i=0;i<result.Rows.Count;i++)
//                {
//                    string tbname = result.Rows[i]["TABLE_NAME"].ToString();
//                    string tbtype = result.Rows[i]["TABLE_TYPE"].ToString();

//                    lite.executeSQL(string.Format(insSql,
//                        dbconnName, dbName, tbname, now, tbtype
//                        ));
//                }

//                GlobalClass.debugLog("TableInfoAssistant", "threadGetTable end, rows: " + result.Rows.Count.ToString());
//            }
//            catch(Exception e)
//            {
//                GlobalClass.debugLog("TableInfoAssistant", "threadGetTable, " + e.ToString());
//                pParam["message"] = e.ToString();
//            }
//            finally
//            {
//                TableInfoAssistant._tableThread = null;
//            }
//        }
    }
}
