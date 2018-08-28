using DataInquiry.Assistant.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataInquiry.Assistant.Assistant
{
    public class CodeRefresh
    {
        public static DateTime LastRefreshTime = DateTime.MinValue;
        public static object _lock = new object();

        private int _refreshInterval = 30 * 60;  // seconds

        private SqliteConn _db;

        private static Thread _tableThread = null;
        private static Hashtable _tableThreadParam;

        public CodeRefresh()
        {
            _db = new SqliteConn();

            if (CodeRefresh.LastRefreshTime.Equals(DateTime.MinValue))
            {
                CodeRefresh.LastRefreshTime = DateTime.Now.AddSeconds(-1 * (_refreshInterval + 1));
            }
        }        

        public void refreshTableInfo(string dbconnName, string dbName, DBConn engine)
        {            
            lock(CodeRefresh._lock)
            {                
                double diffSeconds = DateTime.Now.Subtract(CodeRefresh.LastRefreshTime).TotalSeconds;
                
                if ((int)diffSeconds <= _refreshInterval)
                {
                    GlobalClass.debugLog("CodeRefresh", "not exceed refresh time");
                    return;
                }

                CodeRefresh.LastRefreshTime = DateTime.Now;

                // 檢查是否需要更新
                string chkSql1 = string.Format(@"select TableName from TableInfo where DBConnName = '{0}' and DBName = '{1}' ",
                    dbconnName, dbName);

                Reader r = _db.getDataReader(chkSql1);

                // 有table資料，再檢查是否過期
                if(r.Count > 0)
                {
                    string chkSql2 = string.Format(@"SELECT distinct DBConnName,DBName FROM TableInfo 
                                where DBConnName = '{0}' and DBName = '{1}' 
                                and ModifiedDate < '{2}'", dbconnName, dbName, this.EffectiveDate);
                    r = _db.getDataReader(chkSql2);

                    if (r.Count == 0)  // 沒有過期的
                    {
                        GlobalClass.debugLog("CodeRefresh", "no data expired");
                        return;
                    }
                }                                

                // 更新
                if (CodeRefresh._tableThread != null)
                {
                    GlobalClass.debugLog("CodeRefresh", "thread still running");
                    return;  // running;
                }

                CodeRefresh._tableThread = new Thread(new ParameterizedThreadStart(threadRefreshTable));
                CodeRefresh._tableThreadParam = new Hashtable();
                CodeRefresh._tableThreadParam["engine"] = engine;
                CodeRefresh._tableThreadParam["dbconnName"] = dbconnName;
                CodeRefresh._tableThreadParam["dbName"] = dbName;

                GlobalClass.debugLog("CodeRefresh", "invoke thread");
                CodeRefresh._tableThread.Start(CodeRefresh._tableThreadParam);
            }  // end lock
        }

        private void threadRefreshTable(object param)
        {
            Hashtable pParam = (Hashtable)param;

            try
            {
                DBConn engine = pParam["engine"] as DBConn;
                string dbconnName = pParam["dbconnName"] as string;
                string dbName = pParam["dbName"] as string;

                GlobalClass.debugLog("TableInfoAssistant", "threadGetTable start" + engine.Dbstr);

                // 取得新table
                string sql = "select * from " + dbName + ".INFORMATION_SCHEMA.TABLES with(nolock) where TABLE_NAME not like 'syncobj_%'";
                DataTable result = engine.getData(sql);

                SqliteConn lite = new SqliteConn();

                string sqlCheckTable = "select TableName from TableInfo where DBConnName='" + dbconnName + "' and DBName = '" + dbName + "' and TableName = '{0}'";
                string insSql = @"insert into TableInfo (DBConnName, DBName, TableName, ModifiedDate, TableType) 
                                values('{0}','{1}','{2}','{3}','{4}')";
                string uptSql = @"update TableInfo set ModifiedDate = '{0}' where DBConnName='" + dbconnName + "' and DBName = '" + dbName + "' and TableName = '{1}'";
                string now = GlobalClass.now();

                for (int i = 0; i < result.Rows.Count; i++)
                {
                    // 逐筆更新 TableInfo
                    string tbname = result.Rows[i]["TABLE_NAME"].ToString();
                    string tbtype = result.Rows[i]["TABLE_TYPE"].ToString();

                    Reader r = lite.getDataReader(string.Format(sqlCheckTable, tbname));

                    if(r.Count > 0)
                    {
                        // update
                        lite.executeSQL(string.Format(uptSql, now, tbname));
                    }
                    else
                    {
                        // insert
                        lite.executeSQL(string.Format(insSql, dbconnName, dbName, tbname, now, tbtype
                        ));
                    }

                    Thread.Sleep(300);  // 不要造成負擔
                }

                // 移除不存在的table
                string delSql = string.Format("delete FROM TableInfo where DBConnName='{0}' and DBName = '{1}' and ModifiedDate < '{2}'",
                    dbconnName, dbName, now);
                lite.executeSQL(delSql);
            }
            catch (Exception e)
            {
                GlobalClass.debugLog("CodeRefresh", "threadRefreshTable, " + e.ToString());
                pParam["message"] = e.ToString();
            }
            finally
            {
                CodeRefresh._tableThread = null;
            }
        }

        public string EffectiveDate
        {
            get
            {
                return DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public static void close()
        {
            if(CodeRefresh._tableThread != null)
            {
                if(CodeRefresh._tableThread.ThreadState != ThreadState.Stopped)
                {
                    try
                    {
                        CodeRefresh._tableThread.Abort();
                    }
                    catch {}
                }
            }
        }
    }
}
