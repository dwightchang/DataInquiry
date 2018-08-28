using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using DataInquiry.Assistant.Data;
using System.Xml;
using System.Data.SQLite;

namespace DataInquiry.Assistant
{
    class GlobalClass
    {
        /// <summary>
        /// �s�u�]�w��Form
        /// </summary>
        public static DBConnForm dbConnForm = new DBConnForm();
        public static DataTable dblist = null;

        /// <summary>
        /// ��������s�u
        /// </summary>
        //public static string favoriteDBConn = "";
        //public static string favoriteUsedDb = "";

        /// <summary>
        /// �ۭq���s�u�r��A���O�q��椤�����
        /// </summary>
        public static string specifiedDBConnStr = "";
        
        /// <summary>
        /// �s�u�]�w��Datatable
        /// </summary>
        private static DataTable dtDbConnections = null;
        public static EventCenter eventCenter = new EventCenter();

        /// <summary>
        /// �w���o��table/view
        /// </summary>
        //public static DataSet tableViewList = null;
        /// <summary>
        /// �w���o��sp/function
        /// </summary>
        //public static DataSet spFunctionList = null;

        public static Hashtable retValue = new Hashtable();

        public static DataTable connPool = null;

        // log time
        private static DateTime lastTime = DateTime.MinValue;        
        /// <summary>
        /// log�O��,�HlogId��KEY
        /// </summary>
        public static Hashtable _LogText = new Hashtable();
        /// <summary>
        /// logId ���ɶ��W�O
        /// </summary>
        public static Hashtable _LogTimestamp = new Hashtable();
        /// <summary>
        /// �ɶ��O��,�HtimeId��KEY
        /// </summary>
        public static Hashtable _TimeLogs = new Hashtable();

        public static DataTable connectionsData()
        {
            if (GlobalClass.dtDbConnections == null)
            {
                GlobalClass.dtDbConnections = new DataTable();
                GlobalClass.dtDbConnections.Columns.Add("dbcDB");
                GlobalClass.dtDbConnections.Columns.Add("dbcName");
                GlobalClass.dtDbConnections.Columns.Add("dbcHost");
                GlobalClass.dtDbConnections.Columns.Add("dbcAccount");
                GlobalClass.dtDbConnections.Columns.Add("dbcPassword");
            }

            return GlobalClass.dtDbConnections;
        }

        public static void showDBConnForm()
        {                  
            GlobalClass.dbConnForm.Show();
            GlobalClass.dbConnForm.Focus();
        }

        /// <summary>
        /// ���o�{���� Access DB
        /// </summary>
        /// <returns></returns>
        public static OleDbConnection getAccessDB()
        {
            OleDbConnection conn = null;
            conn = new OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0; Data Source=DI.mdb");
            conn.Open();

            return conn;
        }        

        public static DataTable getDBlist()
        {
            OleDbConnection conn = null;            

            if (dblist == null)
            {
                dblist = new DataTable();                
                dblist.Columns.Add(new DataColumn("ID", typeof(string)));  
                dblist.Columns.Add(new DataColumn("name", typeof(string)));
                dblist.Columns.Add(new DataColumn("Connstr", typeof(string)));
                dblist.Columns.Add(new DataColumn("DBType", typeof(string)));
                dblist.Columns.Add(new DataColumn("engine", typeof(object)));

                try
                {
                    string dblistPath = System.Configuration.ConfigurationManager.AppSettings["DbList"];
                    if (File.Exists(dblistPath) == false)
                    {
                        throw new Exception("�䤣��" + dblistPath + ", �нT�{App.config��DbList�O�_�]�w���T");
                    }

                    FileStream fs = new FileStream(dblistPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader r = new StreamReader(fs);

                    int id = 0;

                    while (r.EndOfStream == false)
                    {
                        string line = r.ReadLine();

                        int idx = line.IndexOf('=');

                        if (idx < 0)
                        {
                            continue;
                        }

                        string name = line.Substring(0, idx);
                        string dbstr = line.Substring(idx).TrimStart('=');

                        DataRow row = dblist.NewRow();

                        row["ID"] = id.ToString();
                        row["name"] = name.Trim();
                        row["Connstr"] = dbstr.Trim();
                        row["DBType"] = "SQL";
                        row["engine"] = null;

                        dblist.Rows.Add(row);

                        ++id;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }

            return dblist;
        }

        public static DBConn getEngineByConnStr(string connstr)
        {            
            return new SqlConn(connstr);
        }

        /// <summary>
        /// ���o�s�u�r���r
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        public static string getEngineConnStr(string connId)
        {
            for (int i = 0; i < GlobalClass.dblist.Rows.Count; i++)
            {
                if (GlobalClass.dblist.Rows[i]["ID"].ToString() == connId)
                {
                    return GlobalClass.dblist.Rows[i]["Connstr"].ToString();
                }
            }

            return "";
        }

        /// <summary>
        /// ���oDB�s�u
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        public static DBConn getEngine(string connId)
        {            

            if (GlobalClass.connPool == null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ID", typeof(string));
                dt.Columns.Add("GUID", typeof(string));
                dt.Columns.Add("engine", typeof(object));
                GlobalClass.connPool = dt;
            }

            DataRow[] dr = GlobalClass.connPool.Select("ID='"+connId+"'");

            DBConn conn = null;
            for (int i = 0; i < dr.Length; i++)
            {
                DBConn currConn = dr[i]["engine"] as DBConn;

                if (currConn.isOpen())
                {
                    continue;
                }
                else
                {
                    conn = currConn;
                }
            }

            if (conn == null)
            {
                // �ؤ@�ӷs��
                string connstr = GlobalClass.getEngineConnStr(connId);
                conn = new SqlConn(connstr);
                DataRow newRow = GlobalClass.connPool.NewRow();
                newRow["ID"] = connId;
                newRow["GUID"] = System.Guid.NewGuid().ToString();
                newRow["engine"] = conn;

                GlobalClass.connPool.Rows.Add(newRow);
                GlobalClass.connPool.AcceptChanges();
            }

            //conn.open();

            return conn;
        }

        public static int[] connCount()
        {
            DataTable dt = GlobalClass.connPool;
            int[] cnt = new int[2];

            cnt[0] = 0;
            cnt[1] = 0;

            if (dt == null)
            {
                return cnt;
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DBConn conn = dt.Rows[i]["engine"] as DBConn;

                if (conn.isOpen())
                {
                    cnt[1]++;
                }
            }

            cnt[0] = dt.Rows.Count;

            return cnt;
        }

        public static string connInfo()
        {
            DataTable dt = GlobalClass.connPool;
            int[] cnt = new int[2];

            cnt[0] = 0;
            cnt[1] = 0;

            if (dt == null)
            {
                return "";
            }

            string ret = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DBConn conn = dt.Rows[i]["engine"] as DBConn;

                SqlConn sconn = (SqlConn)conn;

                ret += sconn._conn.ConnectionString;

                if (sconn._cmd != null)
                {
                    ret += "," + GlobalClass.str(sconn._cmd.CommandText).Replace("\r\n", "");
                }
                ret += ", " + (conn.isOpen() ? "opened" : "closed") + "\r\n";
            }

            return ret;
        }

        public static void closeAllConnection()
        {
            DataTable dt = GlobalClass.connPool;

            if (dt == null)
            {
                return;
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DBConn conn = dt.Rows[i]["engine"] as DBConn;

                SqlConn sconn = (SqlConn)conn;

                if (sconn.isOpen())
                {
                    sconn.close();
                }
            }
        }

        public static string dbValue(string str)
        {
            return str.Replace("'", "''");
        }

        public static string saveInq(string id, string parentId, string group, string name, string content, string shortKey)
        {
            SqliteConn acc = new SqliteConn();

            try
            {
                Reader rCheck = acc.getDataReader(string.Format("select * from Inquiry where groupName = '{0}' and inqName = '{1}' ",
                    group, name));


                if (rCheck.Read() == false)   // new
                {
                    int rows = acc.executeSQL(string.Format("insert into Inquiry(groupName, inqName, content, parentId, shortKey) values('{0}','{1}','{2}', '{3}','{4}'); ",
                                 group, name, dbValue(content), parentId, shortKey));

                    if (rows != 0)
                    {
                        Reader r = acc.getDataReader("select max(id) as mid from Inquiry");

                        if (r.Read())
                        {
                            return r[0].ToString();
                        }
                    }
                }
                else  // update
                {
                    DialogResult r = MessageBox.Show("�л\�즳�����e?", "Confirm",  MessageBoxButtons.YesNo);

                    if (r == DialogResult.Yes)
                    {

                        string uptSql = string.Format("update Inquiry set groupName='{0}', inqName='{1}', content='{2}', parentId='{3}', shortKey='{5}' where id={4} ",
                            group, name, dbValue(content), parentId, id, shortKey);

                        acc.executeSQL(uptSql);
                    }
                    return id;
                }

                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }          
        }

        public static void deleteInq(string id)
        {
            SqliteConn acc = new SqliteConn();

            try
            {
                if (id == "")   // new
                {
                    return;
                }
                else  
                {
                    string delSql = string.Format("delete from Inquiry where id={0} ", id);

                    acc.executeSQL(delSql);                    
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ArrayList getGroups()
        {
            SqliteConn db = new SqliteConn();

            try
            {
                Reader r = db.getDataReader("select distinct groupName from Inquiry where groupName <> '' ");

                ArrayList list = new ArrayList();

                while (r.Read())
                {
                    list.Add(r[0]);
                }

                return list;
            }
            catch(Exception e)
            {
                GlobalClass.errorLog(e.ToString());
            }            

            return null;
        }

        public static string str(object obj)
        {
            return obj == null ? "" : obj.ToString();
        }

        public static string logLevel()
        {
            return System.Configuration.ConfigurationManager.AppSettings["LogLevel"];
        }

        public static void debugLog(string name, string msg)
        {
            try
            {
                if (GlobalClass.logLevel().Equals("DEBUG") == false)
                {
                    return;
                }

                string fname = DateTime.Now.ToString("yyyyMMdd");
                fname = System.AppDomain.CurrentDomain.BaseDirectory + @"\LogFolder\" + fname + "_" + name + ".log";

                if (Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\LogFolder\") == false)
                {
                    Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + @"\LogFolder\");
                }

                System.IO.StreamWriter sw = new System.IO.StreamWriter(fname, true);

                sw.WriteLine("***************");
                sw.Write(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " log:");
                sw.WriteLine(msg);
                sw.Close();
            }
            catch  { }
        }

        public static void errorLog(string msg)
        {
            try
            {
                string fname = DateTime.Now.ToString("yyyyMMdd");
                fname = System.AppDomain.CurrentDomain.BaseDirectory + @"\LogFolder\" + fname + "_Err.log";

                if (Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\LogFolder\") == false)
                {
                    Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + @"\LogFolder\");
                }

                System.IO.StreamWriter sw = new System.IO.StreamWriter(fname, true);

                sw.WriteLine("***************");
                sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " log:");
                sw.WriteLine(msg);
                sw.Close();
            }
            catch  { }
        }

        public static void setRetValue(string pGUID, object pRetVal)
        {
            Hashtable ret = GlobalClass.retValue;

            ret[pGUID] = pRetVal;
        }

        /// <summary>
        /// ������B�z���A���ݦ^�ǭ�
        /// </summary>
        /// <param name="pGUID"></param>
        /// <returns></returns>
        public static bool valueReturnning(string pGUID)
        {
            if (GlobalClass.retValue.Contains(pGUID) == false)
            {
                return false;
            }

            if (GlobalClass.retValue[pGUID] == null)
            {
                return true;
            }

            return false;
        }

        public static object getRetValue(string pGUID)
        {
            Hashtable ret = GlobalClass.retValue;            

            object retObj = ret[pGUID];

            if(retObj != null && ret.Contains(pGUID))
            {
                ret.Remove(pGUID);
            }

            return retObj;
        }

        public static bool CheckChineseString(string strInputString, int intIndexNumber)
        {
            int intCode = 0;

            //����d��]0x4e00 - 0x9fff�^�ഫ��int�]intChineseFrom - intChineseEnd�^
            int intChineseFrom = Convert.ToInt32("4e00", 16);
            int intChineseEnd = Convert.ToInt32("9fff", 16);
            if (strInputString != "")
            {
                //���oinput�r�ꤤ���w�P�_��index�r����unicode�X
                intCode = Char.ConvertToUtf32(strInputString, intIndexNumber);

                if (intCode >= intChineseFrom && intCode <= intChineseEnd)
                {
                    return true;     //�p�G�O�d�򤺪��ƭȴN�^��true
                }
                else
                {
                    return false;    //�p�G�O�d��~���ƭȴN�^��true
                }
            }
            return false;
        }


        /// <summary>
        /// �O���ɶ�
        /// </summary>
        /// </summary>
        /// <param name="logId">log �ߤ@�ѧO</param>
        /// <param name="timeId">time span �ѧO</param>
        /// <param name="action">�ʧ@�y�z</param>
        /// <param name="clean">���槹��O�_�M���O��</param>
        /// 
        public static decimal logTime(string logId, string timeId, string action, bool clean)
        {
            try
            {
                if (clean)
                {
                    GlobalClass._LogText.Remove(timeId);
                }

                decimal span = 0;
                string[] text = new string[4];                

                if (GlobalClass._LogText.Contains(logId) == false && logId.Length != 36)  // �Ӧ�PublicFunc ��query, executeSql, ... ��������
                {
                    GlobalClass._LogText[logId] = new ArrayList();
                }

                if (GlobalClass._TimeLogs[timeId] == null)
                {
                    GlobalClass._TimeLogs[timeId] = DateTime.Now;

                }
                else
                {
                    DateTime last = (DateTime)GlobalClass._TimeLogs[timeId];
                    TimeSpan diff = DateTime.Now.Subtract(last);
                    span = ((decimal)diff.TotalMilliseconds) / ((decimal)1000);
                    GlobalClass._TimeLogs[timeId] = DateTime.Now;

                   
                    // log �g��O����
                    if (logId.Length != 36)
                    {
                        text[0] = timeId;
                        text[1] = action;
                        text[2] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                        text[3] = span.ToString();

                        ArrayList ar = (ArrayList)_LogText[logId];
                        ar.Add(text);

                        if (ar.Count > 300)
                        {
                            ar.RemoveAt(0);
                        }
                    }
                    
                }

                return span;
            }
            catch 
            {
                return -1;
            }
        }

        public static string logString(string logid)
        {
            ArrayList arrLogtext = (ArrayList)GlobalClass._LogText[logid];

            if (arrLogtext == null)
            {
                return "";
            }
            

            string[] text = null;
            string result = "";
            for (int i = 0; i < arrLogtext.Count; i++)
            {
                text = (string[])arrLogtext[i];

                result += text[0] + "," + text[1] + "," + text[2] + "," + text[3] + "\n";
            }

            return result;
        }

        public static void DoubleBuffered(System.Windows.Forms.DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            System.Reflection.PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                  System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        /// <summary>
        /// �ˬd�O�_������r
        /// </summary>
        /// <param name="strChinese"></param>
        /// <returns></returns>
        public static bool isChinese(string strChinese)
        {
            bool bresult = true;
            int dRange = 0;
            int dstringmax = Convert.ToInt32("9fff", 16);
            int dstringmin = Convert.ToInt32("4e00", 16);

            for (int i = 0; i < strChinese.Length; i++)
            {
                dRange = Convert.ToInt32(Convert.ToChar(strChinese.Substring(i, 1)));
                if (dRange >= dstringmin && dRange <dstringmax )
                {
                    bresult = true;
                    break;
                }
                else
                {
                    bresult = false;
                }
            }

            return bresult;
        }

        public static string PrintXML(String XML)
        {
            String Result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.                
                document.LoadXml(XML);

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                String FormattedXML = sReader.ReadToEnd();

                Result = FormattedXML;
            }
            catch (XmlException e)
            {
                return XML;
            }
            finally
            {
                try
                {
                    mStream.Close();
                    writer.Close();
                }
                catch { }
            }

            return Result;
        }

        public static string now()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
