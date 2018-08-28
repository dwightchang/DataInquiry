using DataInquiry.Assistant.Assistant;
using DataInquiry.Assistant.Data;
using DataInquiry.Assistant.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace DataInquiry.Assistant
{
    public partial class InqPage : UserControl, IMaster
    {
        //[DllImport("user32.dll", SetLastError = true)]
        //static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);

        private string _guid = null;
        /// <summary>
        /// Default SQL editor font
        /// </summary>
        //private Font inqFont = new Font("Courier New", 10.0F, FontStyle.Regular, GraphicsUnit.Point, (byte) 1);

        public ArrayList _childInquiries = new ArrayList();
        private InqPage _parentInquiry = null;
        private string _id = "";  //  存在DB中的ID
        private InqForm _inqForm = null;
        private TabPage _tabPage = null;

        /// <summary>
        /// 在結果清單中，被選擇的列
        /// </summary>
        private int _selectedRow = 0;

        /// <summary>
        /// 在結果清單中，被選擇的行
        /// </summary>
        private int _selectedCol = 0;

        public static string _connstr = "";
        public static object _engine;
        public static string _sqlErrorMessage = "";

        /// <summary>
        /// 欄位型態為 binary 時顯示圖型
        /// </summary>
        private Image _binaryData = null;

        /// <summary>
        /// 目前SQL中使用的table名稱
        /// </summary>
        private string currentTableName = "";

        public static object ddlConnectionValue = "";

        /// <summary>
        /// 執行SQL影響的資料筆數
        /// </summary>
        private int _alertNoOfRows = 0;

        /// <summary>
        /// 取得連線時是否顯示訊息
        /// </summary>
        public static bool getConnectionSilent = false;

        private Thread _threadGetData = null;
        private Hashtable _result = new Hashtable();

        /// <summary>
        /// table/view 清單, 用於 code 輔助
        /// </summary>
        ///
        private DataTable _dtTableViewList = null;

        /// <summary>
        /// 資料庫清單
        /// </summary>
        private DataSet databaseList = null;

        private DataGridView currentDataGrid;

        private bool enableCodeAssist = true;

        #region 是否觸發事件

        private bool fireSelectedContentTextChanged = true;
        private bool fireRedSqlTextChanged = true;

        #endregion 是否觸發事件

        private ArrayList buildinFunctions = null;

        public InqPage()
        {
            InitializeComponent();

            initUIComponent();

            lblInfo.Text = "";

            this._guid = System.Guid.NewGuid().ToString();
            SqliteConn localdb = new SqliteConn();

            currentDataGrid = this.dgResult;

            InqPage.getConnectionSilent = true;

            try
            {
                _alertNoOfRows = int.Parse(System.Configuration.ConfigurationManager.AppSettings["AlertNoOfRows"]);

                dgResult.AutoSize = true;

                reloadConnections();
                reloadGroups();

                if (GlobalClass.specifiedDBConnStr == "")   // 從清單中選擇的連線
                {
                    DataTable list = GlobalClass.getDBlist();

                    if (list.Select("ID='999'").Length == 0)
                    {
                        DataRow drNew = list.NewRow();
                        drNew["name"] = "[開啟連線設定檔]";
                        drNew["ID"] = "999";
                        list.Rows.Add(drNew);
                    }

                    //string favConn = GlobalClass.favoriteDBConn;   // keep this value
                    this.ddlConn.Items.Clear();
                    this.ddlConn.DataSource = list;
                    this.ddlConn.DisplayMember = "name";
                    this.ddlConn.ValueMember = "ID";
                    //GlobalClass.favoriteDBConn = favConn;

                    Reader lastConnReader = localdb.getLastConnName();

                    if (lastConnReader.Read())
                    {
                        this.ddlConn.Text = lastConnReader[0].ToString();
                        this.ddlDB.Text = lastConnReader[1].ToString();
                    }
                }
                else        // 自行輸入的連線
                {
                    this.ddlConn.Text = GlobalClass.specifiedDBConnStr;
                }

                this.timer1.Interval = 200;
                this.timer1.Enabled = true;

                this.dgResult.CellFormatting += new DataGridViewCellFormattingEventHandler(dgResult_CellFormatting);

                // load sql build-in functions
                buildinFunctions = new ArrayList();
                using (System.IO.StreamReader funcReader = new StreamReader("functionList.txt", true))
                {
                    while (!funcReader.EndOfStream)
                    {
                        buildinFunctions.Add(funcReader.ReadLine().Trim().ToUpper());
                    }
                }

                invokeCleanLogThread();

                ToolTip tip = new ToolTip();
                tip.AutoPopDelay = 10000;
                tip.InitialDelay = 100;
                tip.ReshowDelay = 100;
                tip.ShowAlways = true;
                tip.SetToolTip(this.chkTransaction, "使用Transaction");
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
                MessageBox.Show(ex.Message);
            }
            finally
            {
                InqPage.getConnectionSilent = true;
            }
        }

        private void invokeCleanLogThread()
        {
            Thread th = new Thread(new ParameterizedThreadStart(threadCleanLog));
            th.Start(null);
        }

        // 刪除過期log檔
        private void threadCleanLog(object param)
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\LogFolder";
            string[] files = System.IO.Directory.GetFiles(path);

            for (int j = 0; j < files.Length; j++)
            {
                DateTime dtWrite = System.IO.File.GetLastWriteTime(files[j]);  // 上次寫入時間
                DateTime dtCreate = System.IO.File.GetCreationTime(files[j]);  // 建立時間,若用複製的,則會變更建立時間,不會改變寫入時間

                DateTime dt = (dtCreate.CompareTo(dtWrite) > 0) ? dtCreate : dtWrite;

                DateTime lastDate = DateTime.Now.AddDays(-3);

                if (dt.CompareTo(lastDate) < 0)
                {
                    try
                    {
                        System.IO.File.Delete(files[j]);
                    }
                    catch (Exception ex) { }
                }
            }
        }

        private void initUIComponent()
        {
            this.icsSqlEditor.ActiveTextAreaControl.TextArea.Click += TextArea_Click;
            this.icsSqlEditor.ActiveTextAreaControl.TextArea.KeyUp += redSql_KeyUp;
            this.icsSqlEditor.ActiveTextAreaControl.TextArea.KeyDown += redSql_KeyDown;
            this.icsSqlEditor.ActiveTextAreaControl.TextArea.DoProcessDialogKey += TextArea_DoProcessDialogKey;
            this.icsSqlEditor.ActiveTextAreaControl.TextArea.TextChanged += TextArea_TextChanged;
            this.icsSqlEditor.ActiveTextAreaControl.TextArea.MouseUp += redSql_MouseUp;
            this.icsSqlEditor.Document.HighlightingStrategy = ICSharpCode.TextEditor.Document.HighlightingStrategyFactory.CreateHighlightingStrategy("SQL");

            // 在 TextEditor 上放一個透明 panel
            //this.extendedPanel.Top = this.icsSqlEditor.Top;
            //this.extendedPanel.Left = this.icsSqlEditor.Left;
            //this.extendedPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            //this.tabPage1.Controls.Add(this.extendedPanel);
            //this.extendedPanel.BringToFront();
        }

        private bool TextArea_DoProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                listCodeAssist.Hide();

                return true;
            }

            if (keyData == Keys.Up || keyData == Keys.Down)
            {
                if (listCodeAssist.Visible)
                {
                    listCodeAssist.Focus();
                    listCodeAssist.SelectedIndex = 1;

                    return true;
                }
            }

            if (keyData == Keys.Enter || keyData == Keys.Tab)
            {
                if (listCodeAssist.Visible)
                {
                    codeCompleting();
                    return true;
                }
            }

            return false;
        }

        private string lastText = "";

        private void TextArea_TextChanged(object sender, EventArgs e)
        {
            if (lastText == this.icsSqlEditor.Text)
            {
                return;
            }

            if (!fireRedSqlTextChanged)
            {
                return;
            }

            lastText = this.icsSqlEditor.Text;

            //if (enableCodeAssist)
            //{
            //    codeAssist();
            //}
        }

        private void TextArea_Click(object sender, EventArgs e)
        {
            redSql_Click(sender, e);
        }

        private void redSql_MouseWheel(object sender, MouseEventArgs e)
        {
            //int newY = -(this.pnlBar.AutoScrollPosition.Y + e.Delta);

            //int origin = this.pnlBar.AutoScrollPosition.Y;

            //this.pnlBar.AutoScrollPosition = new Point(this.pnlBar.AutoScrollPosition.X, newY);

            //if (origin == this.pnlBar.AutoScrollPosition.Y)
            //{
            //    if (e.Delta < 0)
            //    {
            //        // 沒有變,表示到底了
            //        return;
            //    }
            //    else
            //    {
            //        if (this.redSql.Top == 0)
            //        {
            //            return;
            //        }
            //    }
            //}

            //int top = this.redSql.Top;

            //top += e.Delta;

            //if (top > 0)
            //{
            //    top = 0;
            //}

            //this.redSql.Top = top;
        }

        public void runSql(string sql)
        {
        }

        public DataSet getDataSet()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(_dtTableViewList);
            return ds;
        }

        public Control getMasterUIControl()
        {
            return null;
        }

        private void dgResult_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dg = (DataGridView)sender;

            DataTable dt = dg.DataSource as DataTable; //ds.Tables[0];

            if (dt.Columns[e.ColumnIndex].DataType == typeof(Byte[]))
            {
                e.Value = binImg();
            }
        }

        private DataGridView newResultGrid()
        {
            DataGridView dg = new DataGridView();
            dg.AutoSize = true;
            dg.BorderStyle = BorderStyle.Fixed3D;
            dg.CellBorderStyle = DataGridViewCellBorderStyle.Raised;
            dg.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
            dg.DefaultCellStyle.BackColor = SystemColors.Window;
            dg.DefaultCellStyle.Font = new Font("Courier New", 9.0F);
            dg.DefaultCellStyle.ForeColor = SystemColors.ControlText;
            dg.DefaultCellStyle.SelectionBackColor = SystemColors.GradientActiveCaption;
            dg.DefaultCellStyle.SelectionForeColor = SystemColors.ControlText;
            dg.RowHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dg.RowHeadersDefaultCellStyle.Font = new Font("新細明體", 9.0F);
            dg.RowHeadersDefaultCellStyle.ForeColor = SystemColors.WindowText;
            dg.RowHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
            dg.RowHeadersDefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
            dg.EditMode = DataGridViewEditMode.EditOnF2;
            dg.ScrollBars = ScrollBars.Both;
            //dg.Width = tabPage2.Width;
            //dg.Height = tabPage2.Height;
            //dg.MaximumSize = new Size(tabPage2.Width, tabPage2.Height);
            //dg.MinimumSize = new Size(dg.Width, tabPage2.Height);
            dg.AllowUserToAddRows = false;

            // events
            dg.CellClick += new DataGridViewCellEventHandler(dgResult_CellClick);
            dg.CellContextMenuStripNeeded += new DataGridViewCellContextMenuStripNeededEventHandler(dgResult_CellContextMenuStripNeeded);
            dg.CellDoubleClick += new DataGridViewCellEventHandler(dgResult_CellDoubleClick);
            dg.CellValueChanged += new DataGridViewCellEventHandler(dgResult_CellValueChanged);
            dg.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(dgResult_DataBindingComplete);
            dg.KeyDown += new KeyEventHandler(dgResult_KeyDown);
            dg.KeyUp += new KeyEventHandler(dgResult_KeyUp);
            dg.RowEnter += new DataGridViewCellEventHandler(dgResult_RowEnter);
            dg.CellFormatting += new DataGridViewCellFormattingEventHandler(dgResult_CellFormatting);
            //dg.CellContentClick += new DataGridViewCellEventHandler(dgResult_CellContentClick);
            dg.ColumnWidthChanged += new DataGridViewColumnEventHandler(dgResult_ColumnWidthChanged);

            return dg;
        }

        private void dgResultOld_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataTable dt = (DataTable)this.dgResultOld.DataSource;

            if (dt.Columns[e.ColumnIndex].DataType == typeof(Byte[]))
            {
                e.Value = binImg();
            }
        }

        private Image binImg()
        {
            if (_binaryData != null)
            {
                return _binaryData;
            }

            _binaryData = Image.FromFile("binaryData.jpg");
            return _binaryData;
        }

        private void setTabPagename(TabPage pPage, string pName)
        {
            if (pName != "")
            {
                pPage.Text = pName;
            }
            else
            {
                pPage.Text = "[No Name]";
            }
        }

        public void setData(string id, string group, string name, string content, string shortKey, InqForm inq, TabPage tabpage)
        {
            if (id == null)
            {
                return;
            }

            this._id = id;
            this._inqForm = inq;
            this._tabPage = tabpage;

            setTabPagename(tabpage, name);
            this.edFormName.Text = name;

            if (this.icsSqlEditor.Text.EndsWith("\r\n") == false && this.icsSqlEditor.Text.Length != 0)
            {
                content = "\r\n\r\n" + content;
            }

            if (content.Length > 0)
            {
                pasteText(content);
            }

            this.ddlGroup.Text = group;
            this.edShortKey.Text = shortKey;

            // get childs
            SqliteConn acc = new SqliteConn();

            try
            {
                if (id != "")
                {
                    Reader r = acc.getDataReader("select id, groupName, inqName, content, shortKey from Inquiry where parentId = '" + id + "' ");

                    while (r.Read())
                    {
                        InqPage newInq = new InqPage();
                        TabPage tp = this._inqForm.newInqPage(newInq);
                        tp.Text = r[2].ToString();                        

                        newInq.setParentInq(this);
                        newInq.setData(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString(), r[4].ToString(), this._inqForm, tp);
                        addChild(newInq);
                    }
                }

                if (isSubInquiry() == false) // first tabpage
                {
                    this.btnCloseTab.Visible = false;
                }
                this._inqForm.tabInqs_Resize(null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //finally
            //{
            //    acc.close();
            //}
        }

        private void btnNewInquiry_Click(object sender, EventArgs e)
        {
            InqPage newInq = new InqPage();
            TabPage tp = this._inqForm.newInqPage(newInq);

            newInq.setParentInq(this);

            newInq.setData(this._id, this.ddlGroup.Text, this.edFormName.Text, "", "", this._inqForm, tp);

            addChild(newInq);
        }

        private void addChild(InqPage inq)
        {
            _childInquiries.Add(inq);
        }

        public void setParentInq(InqPage pInq)
        {
            _parentInquiry = pInq;

            if (isSubInquiry() == false)
            {
                this.btnSaveSql.Visible = true;
                this.BackColor = Color.LightSteelBlue;
                this.statusStrip1.BackColor = Color.LightSteelBlue;

                lblGroup.Visible = true;
                ddlGroup.Visible = true;
                lblFormName.Visible = true;
                edFormName.Visible = true;

                //chkAutoRun.Visible = false;
            }
            else
            {
                this.btnSaveSql.Visible = false;
                this.BackColor = System.Drawing.SystemColors.Control;
                this.statusStrip1.BackColor = System.Drawing.SystemColors.Control;

                lblGroup.Visible = false;
                ddlGroup.Visible = false;
                lblFormName.Visible = false;
                edFormName.Visible = false;

                //chkAutoRun.Visible = true;
            }
        }

        private bool isSubInquiry()
        {
            if (_parentInquiry == null)
            {
                return false;
            }

            return true;
        }

        private void reloadConnections()
        {
            string selected = this.ddlConn.SelectedText;

            this.ddlConn.Items.Clear();
            DataTable dt = GlobalClass.connectionsData();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                this.ddlConn.Items.Add(dt.Rows[i]["dbcName"]);
            }

            this.ddlConn.SelectedText = selected;
        }

        private void reloadGroups()
        {
            ArrayList list = GlobalClass.getGroups();

            this.ddlGroup.Items.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                this.ddlGroup.Items.Add(list[i].ToString());
            }
        }

        private void checkDBDropDown()
        {
            if (ddlDB.Items.Count > 0)
            {
                return;
            }

            if (databaseList != null)
            {
                updateDBList(databaseList.Tables[0]);
            }
        }

        private void ddlDB_Click(object sender, EventArgs e)
        {
            //if (ddlDB.Items.Count > 0)
            //{
            //    return;
            //}

            //if (databaseList != null)
            //{
            //    updateDBList(databaseList.Tables[0]);
            //}

            //DBConn conn = getConnection(this.ddlConn.SelectedValue.ToString()) as DBConn;
            //if (conn == null)
            //{
            //    return;
            //}

            //string sql = "select name from sys.databases where database_id > 4 order by [name]";

            //runSqlByStatement(InqPage.DS_TYPE_DBLIST, sql, this.ddlConn.SelectedValue.ToString(), false);

            //this.ddlDB.DropDownHeight = 120;
            //this.ddlDB.DataSource = dt;
            //this.ddlDB.DisplayMember = "name";
            //this.ddlDB.ValueMember = "name";
            //this.ddlDB.DroppedDown = true;  // 因combobox會refresh導致縮起來
        }

        public DBConn getConnection()
        {
            return getConnection(this.ddlConn.SelectedValue.ToString());
        }

        /// <summary>
        /// 取得連線
        /// </summary>
        /// <returns></returns>
        public DBConn getConnection(string pConnId)
        {
            try
            {
                Thread t;

                Hashtable param = new Hashtable();

                if (GlobalClass.specifiedDBConnStr == "")
                {
                    param["connStr"] = pConnId;
                }
                else
                {
                    param["connStr"] = GlobalClass.specifiedDBConnStr;
                }

                param["GUID"] = pConnId;

                object ret = GlobalClass.getRetValue(pConnId);

                if (ret != null)
                {
                    return (DBConn)ret;
                }
                else
                {
                    GlobalClass.setRetValue(pConnId, null);
                    t = new Thread(new ParameterizedThreadStart(threadGetEngine));
                    t.Start(param);
                }

                for (int i = 0; i < 3; i++)
                {
                    if (GlobalClass.valueReturnning(pConnId))
                    {
                        Thread.Sleep(100);
                    }
                    else
                    {
                        break;
                    }
                }

                return (DBConn)GlobalClass.getRetValue(pConnId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return null;
        }

        public void threadGetEngine(object pParam)
        {
            string connstr = "";
            string guid = "";

            try
            {
                Hashtable param = (Hashtable)pParam;

                connstr = (string)param["connStr"];
                guid = (string)param["GUID"];

                object obj = GlobalClass.getEngine(connstr);

                GlobalClass.setRetValue(guid, obj);
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(connstr);
                GlobalClass.errorLog(ex.ToString());
            }
        }

        private bool _ddlDBChanged = false;

        private void ddlDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            setTabPagename(this._tabPage, this.ddlDB.Text);

            if (this._ddlDBChanged == false)
            {
                return;
            }

            SqliteConn acc = new SqliteConn();

            try
            {
                acc.updateFavorite(this.ddlConn.Text, this.ddlDB.Text);

                // update last used db
                string useddb = ddlDB.Text;
                string connname = ddlConn.Text;
                if (useddb.Equals("") || useddb.Equals("System.Data.DataRow") || useddb.Equals("System.Data.DataRowView")
                    || connname.Equals(""))
                {
                    return;
                }

                //Reader r = acc.getDataReader(string.Format("select * from UsedDB where DBConnName = '{0}'", connname));
                Reader r = acc.getDataReader(string.Format("select DBConnName from FavoriteDb where DBConnName = '{0}' and DBName = '{1}'",
                    connname, useddb));

                if (r.Read())
                {
                    //acc.executeSQL(string.Format("update UsedDB set LastUsedDB = '{0}' where DBConnName = '{1}'", useddb, connname));
                    acc.executeSQL(string.Format("update FavoriteDb set ModifiedDate = '{0}' where DBConnName = '{1}' and DBName = '{2}'",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), connname, useddb));
                }
                else
                {
                    //acc.executeSQL(string.Format("insert into UsedDB (DBConnName, LastUsedDB) values('{0}','{1}') ", connname, useddb));
                    acc.executeSQL(string.Format("insert into FavoriteDb (DBConnName, DBName, ModifiedDate) values('{0}','{1}', '{2}') ",
                        connname, useddb, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                }

                this._tabPage.Text = ddlDB.Text;
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                _ddlDBChanged = false;
            }
        }

        private void cleanCodeAssistList()
        {
            GlobalClass.debugLog("cleanCodeAssistList", "start");

            this.alertAssistantReady = false;
            _dtTableViewList = null;
            //spFunctionList = null;
            databaseList = null;

            stopGettingTableViews();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                GlobalClass.logTime("query", "query", "start", true);
                runStatement();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool afterRunSql = false;

        private void runStatement()
        {
            try
            {
                this.progress.Visible = true;
                listCandidate.Hide();

                if (isQueryStatement())
                {
                    GlobalClass.logTime("query", "query", "runSql s", false);
                    runSql();
                    GlobalClass.logTime("query", "query", "runSql e", false);
                    afterRunSql = true;
                }
                else
                {
                    execSql();
                }
            }
            catch (System.Data.SqlClient.SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
                throw ex;
            }
            finally
            {
                //this.progress.Visible = false;
            }
        }

        private bool isQueryStatement()
        {
            string text = " " + getStatement().ToLower().Replace("\r\n", " ");

            if (text.Trim().StartsWith("update ") ||
                text.Trim().StartsWith("delete ") ||
                text.Trim().StartsWith("insert "))
            {
                return false;
            }

            return true;
        }

        private void transferCodeToSQL()
        {
            StringBuilder sb = new StringBuilder();

            string text = Clipboard.GetText();

            string[] lines = text.Split("\r\n".ToCharArray());

            for (int i = 0; i < lines.Length; i++)
            {
                string line = trimCode(lines[i]);

                if (line.Length == 0)
                {
                    continue;
                }

                sb.AppendLine(line);
            }
        }

        private string trimCode(string str)
        {
            StringBuilder sb = new StringBuilder();

            str = str.Trim().TrimEnd(");".ToCharArray());   // 移除尾部程式碼
            str = str.Trim().TrimEnd(";".ToCharArray());

            for (int i = 0; i < str.Length; i++)
            {
                int doubleQuotes = str.IndexOf('"', i);

                if (doubleQuotes < 0)  // 沒有雙引號，結束了
                {
                    break;
                }

                i = doubleQuotes;

                if (str[i] == '"')  // 將雙引號內的字串取出
                {
                    int nextDoubleQuotes = str.IndexOf('"', i + 1);

                    if (nextDoubleQuotes < 0)
                    {
                        nextDoubleQuotes = str.Length - 1;
                    }

                    string script = str.Substring(i + 1, nextDoubleQuotes - i - 1);
                    script = script.Replace("\\r\\n", "\r\n");

                    sb.Append(script);

                    i = nextDoubleQuotes;
                }

                // 到這裏應該是程式的變數
                string var = str.Substring(i).TrimStart('"');

                int nextQ = var.IndexOf('"');
                if (nextQ > 0)
                {
                    var = var.Substring(0, nextQ);
                }

                var = var.Trim().Trim('+').Trim();

                if (var.Length != 0)
                {
                    sb.Append("@[" + var + "]");
                }
            } // end for

            return sb.ToString();
        }

        private bool isString(string str, int idx, string sample)
        {
            string targetStr = str.Substring(idx);

            if (targetStr.StartsWith(sample))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///  取得選取的字串，或者全部文字
        /// </summary>
        /// <returns></returns>
        private string getStatement()
        {
            string sql = "";

            if (this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.SelectedText.Length == 0)
            {
                sql = this.icsSqlEditor.Text;
            }
            else
            {
                sql = this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.SelectedText;
            }

            sql = sql.Trim();
            sql = sql.TrimStart("\r\n".ToCharArray());
            sql = sql.Trim();

            return sql;
        }

        /// <summary>
        /// 找出游標選的範圍後，加大範圍到整個列
        /// </summary>
        /// <returns></returns>
        //private string findSelectionLines()
        //{
        //    this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.
        //    int start = this.redSql.SelectionStart;
        //    int end = start + this.redSql.SelectionLength;
        //    int idxEndLine = this.redSql.Text.IndexOf("\n", end);
        //    if (idxEndLine == -1)
        //    {
        //        idxEndLine = this.redSql.Text.Length;
        //    }

        //    int idxStartLine = start;
        //    while (idxStartLine > 0)
        //    {
        //        --idxStartLine;
        //        if (this.redSql.Text[idxStartLine] == '\n')
        //        {
        //            ++idxStartLine;
        //            break;
        //        }
        //    }

        //    return this.redSql.Text.Substring(idxStartLine, idxEndLine - idxStartLine);
        //}

        private TimeSpan _lastSpanTime = TimeSpan.Zero;

        private void runSql()
        {
            if (isQueryStatement() == false)
            {
                return;
            }

            string sql = "";

            try
            {
                sql = compileSql(getStatement());
                GlobalClass.logTime("query", "query", "compiledSql", false);

                this.progress.Visible = true;
                Application.DoEvents();
                GlobalClass.logTime("query", "query", "DoEvents", false);

                runSqlByStatement(InqPage.DS_TYPE_GETDATA, sql, this.ddlConn.SelectedValue.ToString(), InqPage.getConnectionSilent);
                //updateQueryResult(ds, sql);
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
                throw ex;
            }
        }

        private DataTable _lastDataSet;
        private int[] _lastColumnsWidth;

        /// <summary>
        /// 只記住上次結果，不立即更新結果
        /// </summary>
        private void copyResultToSecondResult()
        {
            dgResultOld.DataSource = null;
            _lastDataSet = (DataTable)dgResult.DataSource;
            _lastColumnsWidth = new int[dgResult.Columns.Count];

            for (int i = 0; i < dgResult.Columns.Count; i++)
            {
                _lastColumnsWidth[i] = dgResult.Columns[i].Width;
            }

            tabInquiry.TabPages[3].Text = tabInquiry.TabPages[1].Text.Replace("查詢結果", "上次結果");
        }

        /// <summary>
        /// 點選[上次結果]時，才顯示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showLastResult()
        {
            if (dgResultOld.DataSource != null)
            {
                return;
            }

            if (_lastDataSet == null)
            {
                return;
            }

            //dgResultOld.Columns.Clear();
            dgResultOld.Dispose();
            dgResultOld = newResultGrid();

            dgResultOld.DataSource = _lastDataSet;

            tabPage4.Controls.Clear();
            tabPage4.Controls.Add(dgResultOld);
            //dgResultOld.DataMember = "temp";

            for (int i = 0; i < _lastColumnsWidth.Length; i++)
            {
                dgResultOld.Columns[i].Width = _lastColumnsWidth[i];
            }
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 將資料集更新到"查詢結果"
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="sql"></param>
        private void updateQueryResult(DataSet ds, string sql)
        {
            if (ds == null)
            {
                return;
            }

            try
            {
                if (listCodeAssist.Visible)
                {
                    listCodeAssist.Hide();
                }

                _lastSpanTime = InqPage._spanTime;

                copyResultToSecondResult();

                if (ds.Tables[0].Rows.Count == 0)
                {
                    DataRow dr = ds.Tables[0].NewRow();
                    ds.Tables[0].Rows.Add(dr);
                }
                GlobalClass.logTime("query", "query", "result 0", false);

                dgResult.Dispose();
                dgResult = null;

                tabPage2.Controls.Clear();                

                int dgHeight = -1;                
                dgHeight = tabPage2.Height / ds.Tables.Count;
                
                

                int top = 0;
                foreach (DataTable dt in ds.Tables)
                {
                    DataGridView dg = newResultGrid();
                    dg.DataSource = dt;
                    
                    if (dgResult == null)
                    {
                        dgResult = dg;
                        this.currentDataGrid = dgResult;
                    }

                    dg.Width = tabPage2.Width;
                    dg.Height = dgHeight;
                    dg.MaximumSize = new Size(tabPage2.Width, dgHeight);
                    dg.Top = top;                              

                    top += dg.Height + 10;
                    tabPage2.Controls.Add(dg);                                       
                }
                
                tabPage2.Controls.Add(listCandidate);
                listCandidate.BringToFront();

                if (ds != null)
                {
                    tabInquiry.SelectTab(1);
                    tabInquiry.TabPages[1].Text = "查詢結果(" + ds.Tables[0].Rows.Count.ToString() + ")";

                    if (_lastSpanTime != null)
                    {
                        showSpanTime(true);
                    }
                }

                GlobalClass.logTime("query", "query", "result 3", false);
                getTableName(sql);
                GlobalClass.logTime("query", "query", "result 4", false);
                resetChangeList();
                GlobalClass.logTime("query", "query", "result 5", false);

                _searchStr = "";
                dgResult.Focus();
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
                throw ex;
            }
            finally
            {
                //dgResult.Visible = true;
            }
        }

        private void verifyGridFormat(DataGridView dg)
        {
            DataTable dt = (DataTable)dg.DataSource;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dt.Columns[i].DataType == typeof(DateTime))
                {
                    dg.Columns[dt.Columns[i].ColumnName].DefaultCellStyle.Format = "yyyy/MM/dd HH:mm:ss.fff";
                }
            }
        }        

        private string compileSql(string sql)
        {            
            if (isQueryStatement())
            {
                // 快速用法，不輸入 select * from 時，自動補上

                MatchCollection matchs = Regex.Matches(sql, "^[\\s]*[_0-9a-z]+[\\s]*\\z", RegexOptions.IgnoreCase);
                MatchCollection matchs2 = Regex.Matches(sql, "^[\\s]*[_0-9a-z]+\\.[0-9a-z]*\\.[_0-9a-z]+[\\s]*\\z", RegexOptions.IgnoreCase);

                if ((matchs.Count == 1 || matchs2.Count == 1) && sql.ToLower().Contains("select") == false)
                {
                    sql = "select top 30 * from " + sql;
                }
            }

            sql = applyUsedDatabase(sql, this.ddlDB.Text);

            return sql;
        }

        private string applyUsedDatabase(string sql)
        {
            return applyUsedDatabase(sql, this.ddlDB.Text);
        }

        private string applyUsedDatabase(string sql, object pDbName)
        {
            if (pDbName != null && pDbName.ToString() != "")
            {
                sql = "USE [" + pDbName.ToString().Trim() + "];  \r\n" + sql;
            }

            return sql;
        }

        private void execSql()
        {
            string sql = "";

            try
            {
                DBConn engine = null;

                engine = getConnection(this.ddlConn.SelectedValue.ToString()) as DBConn; 
                
                if (engine == null)
                {
                    return;
                }

                sql = compileSql(getStatement());

                executeSql(engine, sql);

                this.edSqlMessage.Text = engine.getMessage();
            }
            catch (Exception ex)
            {                
                throw ex;
            }
            finally
            {
                this.progress.Visible = false;
            }
        }

        private void executeSql(DBConn engine, string sql)
        {
            if(this.chkTransaction.Checked)
            {
                executeSqlTran(engine, sql);
            }
            else
            {
                executeSqlWithoutTran(engine, sql);
            }
        }

        private void executeSqlTran(DBConn engine, string sql)
        {
            engine.startTransaction(IsolationLevel.ReadUncommitted);

            try
            {
                int rows = engine.executeSQL(sql);

                if (rows >= _alertNoOfRows)
                {
                    DialogResult result = MessageBox.Show("影響的筆數有" + rows.ToString() + "筆，確定嗎?", "Confirm", MessageBoxButtons.YesNo);

                    if (result == DialogResult.No)
                    {
                        engine.rollback();
                        return;
                    }
                }

                engine.commit();
                engine.close();
                MessageBox.Show(rows.ToString() + " rows were affected");
            }
            catch (Exception ex)
            {
                engine.rollback();
                throw ex;
            }
        }

        private void executeSqlWithoutTran(DBConn engine, string sql)
        {            
            try
            {
                int rows = engine.executeSQL(sql);
                                
                engine.close();
                MessageBox.Show(rows.ToString() + " rows were affected");
            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="resultName">result的key</param>
        /// <param name="sqlStatement"></param>
        /// <param name="pConn"></param>
        /// <param name="silent"></param>
        /// <returns></returns>
        private DataSet runSqlByStatement(string resultName, string sqlStatement, string pConn, bool silent)
        {
            try
            {
                DBConn engine = getConnection(pConn) as DBConn;
                GlobalClass.logTime("query", "query", "getConnection", false);

                if (engine == null)
                {
                    return null;
                }

                Thread tGetDataSet = new Thread(new ParameterizedThreadStart(threadGetDataSet));
                InqPage._sql = sqlStatement;
                InqPage._engine = engine;
                InqPage._hsQueryResult[resultName] = null;

                _result = new Hashtable();
                _result["engine"] = engine;
                _result["sql"] = sqlStatement;
                _result["resultName"] = resultName;

                tGetDataSet.Start(_result);

                if (resultName.Equals(InqPage.DS_TYPE_GETDATA) ||
                    resultName.Equals(InqPage.DS_TYPE_DBLIST))
                {
                    // 不等待

                    this._threadGetData = tGetDataSet;
                    return null;
                }

                GlobalClass.logTime("query", "query", "while s", false);
                while (InqPage._hsQueryResult[resultName] == null)
                {
                    Thread.Sleep(200);

                    if (InqPage._sqlErrorMessage != "" && silent == false)
                    {
                        MessageBox.Show(InqPage._sqlErrorMessage);
                        break;
                    }

                    if (isCanceled)
                    {
                        isCanceled = false;
                        engine.close();
                        break;
                    }

                    if (this._inqForm.IsDisposed)  // 程式已關閉
                    {
                        break;
                    }
                }

                tGetDataSet.Abort();

                return InqPage._hsQueryResult[resultName] as DataSet;
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// 回傳目前選取列中,指定欄位的內容值
        /// </summary>
        /// <param name="colname"></param>
        /// <returns></returns>
        public string getData(string colname)
        {
            try
            {
                DataTable ds = (DataTable)currentDataGrid.DataSource;

                if (ds == null)
                {
                    MessageBox.Show("無法取得" + colname + "的資料");
                    return null;
                }

                if (_selectedRow + 1 > ds.Rows.Count)
                {
                    MessageBox.Show("沒有選取資料列");
                    return null;
                }

                DataRow dr = ds.Rows[_selectedRow];
                return toStr(dr[colname]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return null;
        }

        private string toStr(object str)
        {
            return str == null ? "" : str.ToString();
        }

        public static Hashtable _hsQueryResult = new Hashtable();
        public const string DS_TYPE_GETDATA = "0";
        public const string DS_TYPE_QUERY = "1";
        public const string DS_TYPE_TABLE_VIEW = "2";
        public const string DS_TYPE_FUNC_SP = "3";
        public const string DS_TYPE_DBLIST = "4";  // 取得DB清單

        public static string _sql = "";
        public static TimeSpan _spanTime;

        public void threadGetDataSet(object param)
        {
            Hashtable pParam = (Hashtable)param;

            try
            {
                InqPage._sqlErrorMessage = "";
                DBConn engine = pParam["engine"] as DBConn;
                string sql = pParam["sql"] as string;
                string resultName = pParam["resultName"] as string;

                DateTime t1, t2;
                t1 = DateTime.Now;
                pParam["startTime"] = t1;
                DataSet result = engine.getDataSet(sql, "temp");
                InqPage._hsQueryResult[resultName] = result;
                pParam["result"] = result;
                t2 = DateTime.Now;

                InqPage._spanTime = t2.Subtract(t1);
                pParam["spanTime"] = t2.Subtract(t1);
                pParam["mssqlMessage"] = engine.getMessage();
            }
            catch (Exception ex)
            {
                InqPage._sqlErrorMessage = ex.Message + "\n" + InqPage._sql;
                pParam["message"] = ex.Message + "\n" + InqPage._sql;
            }
        }

        private void increaseProgress(int inc)
        {
            if (this.progress.Value + inc <= 100)
            {
                this.progress.Value += inc;
            }

            //Application.DoEvents();
        }

        private void InqPage_Resize(object sender, EventArgs e)
        {
            this.tabInquiry.Width = this.Width - 10;
            this.tabInquiry.Height = this.Height - 95;

            this.tabInquiry.TabPages[1].Width = this.tabInquiry.Width - 10;
            this.tabInquiry.TabPages[1].Height = this.tabInquiry.Height - 30;

            this.icsSqlEditor.Width = this.tabInquiry.Width - 17;
            this.icsSqlEditor.Height = this.tabInquiry.Height - 60;            

            this.edSelectedContent.Width = this.tabInquiry.Width - 20;
            this.edSelectedContent.Height = this.tabInquiry.Height - 50;

            this.edSqlMessage.Width = this.tabInquiry.Width - 20;
            this.edSqlMessage.Height = this.tabInquiry.Height - 50;
        }      

        /// <summary>
        /// 找出需重新上色的區域
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private int[] findRange(int pos, string text)
        {
            if (pos < 0)
            {
                pos = 0;
            }

            // find left index
            int left = pos - 1;
            if (left < 0)
            {
                left = 0;
            }

            if (text.Substring(left, 1) == " " && left > 0)
            {
                --left;
            }

            while (left > 0)
            {
                if (text.Substring(left, 1) == " ")
                {
                    break;
                }

                --left;
            }

            // find right indx
            int right = pos;
            if (right > text.Length - 1)
            {
                right = text.Length - 1;
            }

            if (text.Substring(right, 1) == " " && right < text.Length - 1)
            {
                ++right;
            }

            while (right < text.Length - 1)
            {
                if (text.Substring(right, 1) == " ")
                {
                    break;
                }

                ++right;
            }

            int[] range = new int[2];
            range[0] = left;
            range[1] = right;

            return range;
        }

        private MatchCollection _lastMatchCollection = null;
        /// <summary>
        /// 給常數上色
        /// </summary>
        //private void paintConstant(int idxStart, int idxEnd)
        //{
        //    MatchCollection collection = Regex.Matches(this.redSql.Text, "'[^']{0,}'", RegexOptions.Singleline);  // 'xxx'

        //    IEnumerator ien = collection.GetEnumerator();

        //    while (ien.MoveNext())
        //    {
        //        Match match = (Match)ien.Current;

        //        if (match.Success == false)
        //        {
        //            continue;
        //        }

        //        bool inRange = false;

        //        if (match.Index >= idxStart && match.Index <= idxEnd)
        //        {
        //            inRange = true;
        //        }

        //        int matchEndIdx = match.Index + match.Length - 1;
        //        if (matchEndIdx >= idxStart && matchEndIdx <= idxEnd)
        //        {
        //            inRange = true;
        //        }

        //        if (!inRange && matchSavedConstant(match))  // 若在range內，還是要重新上色
        //        {
        //            continue;
        //        }

        //        this.redSql.Select(match.Index, match.Length);
        //        this.redSql.SelectionColor = Color.Brown;

        //    }

        //    _lastMatchCollection = collection;
        //}

        private bool matchSavedConstant(Match match)
        {
            if (_lastMatchCollection == null)
            {
                return false;
            }

            IEnumerator ienum = _lastMatchCollection.GetEnumerator();
            while (ienum.MoveNext())
            {
                Match savedMatch = (Match)ienum.Current;

                if (savedMatch.Value == match.Value)
                {
                    return true;
                }
            }

            return false;
        }

        //private void paintKeyWord(string pText, int startIdx, int endIdx)
        //{
        //    paintIndex = startIdx;

        //    string redsql = this.redSql.Text;

        //    for (int i = 0; i < 1000; i++)
        //    {
        //        string sqlText = redsql.Substring(startIdx, endIdx - startIdx + 1);
        //        Match m = Regex.Match(" " + sqlText + " ", "[\\s\t\n]" + pText + "[\\s\t\n]", RegexOptions.IgnoreCase);

        //        if (m.Success)
        //        {
        //            this.redSql.Select(startIdx + m.Index, m.Length - 1);
        //            this.redSql.SelectionColor = Color.Blue;

        //            startIdx += m.Index + m.Length;
        //        }
        //        else
        //        {
        //            break;
        //        }

        //        if (startIdx >= redsql.Length || startIdx >= endIdx)
        //        {
        //            break;
        //        }
        //    }

        //}

        private void selectBlock()
        {
            int[] range = findBlock();

            icsEditorSelect(range[0], range[1]);
        }

        private void icsEditorSelect(int a1, int a2)
        {
            ICSharpCode.TextEditor.TextLocation loc1 = this.icsSqlEditor.Document.OffsetToPosition(a1);
            ICSharpCode.TextEditor.TextLocation loc2 = this.icsSqlEditor.Document.OffsetToPosition(a2);
            this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.SetSelection(loc1, loc2);
        }

        private int[] findBlock()
        {
            int currentPosition = this.icsSqlEditor.ActiveTextAreaControl.Caret.Offset;

            int len = this.icsSqlEditor.Text.Length;
            string strCurrentPositionText = this.icsSqlEditor.Document.GetText(0, currentPosition);
            string strCurrentToEndText = this.icsSqlEditor.Document.GetText(currentPosition, len - currentPosition);

            int blockEnd;
            Match matchEnd = Regex.Match(strCurrentToEndText, "[\\s\t]*\n[\\s\t]*\n", RegexOptions.Singleline);  // 找出連續空白(區塊邊界)

            if (matchEnd.Success)
            {
                blockEnd = strCurrentPositionText.Length + matchEnd.Index;
            }
            else
            {
                blockEnd = len;
            }

            int blockStart = 0;

            MatchCollection matchStart = Regex.Matches(strCurrentPositionText, "[\\s\t]*\n[\\s\t]*\n", RegexOptions.Singleline);  // 找出連續空白(區塊邊界)

            if (matchStart.Count > 0)
            {
                int last = matchStart.Count - 1;
                blockStart = matchStart[last].Index + matchStart[last].Length;
            }

            return new int[] { blockStart, blockEnd };
        }

        private void redSql_Enter(object sender, EventArgs e)
        {
            messageStatus("(ctrl +) F6 = 語法輔助, F7 = highlight選取文字, F9 = 執行, F12 = 程式碼->SQL, Ctrl+Alt = 區塊選取, Alt+滑鼠 = 區塊選取並執行");
        }

        private void disableScroll()
        {
            //redSqlOriginalHeight = this.redSql.Height;
            //this.redSql.Height = 1000;
        }

        private void enableScroll()
        {
            //this.redSql.Height = redSqlOriginalHeight;
        }

        //private void pasteTextWOAssist(string appendedText)
        //{
        //    disableScroll();
        //    this.enableCodeAssist = false;
        //    pasteText(appendedText);
        //    this.enableCodeAssist = true;

        //    enableScroll();
        //}

        /// <summary>
        /// 貼上文字，貼上時判斷顏色
        /// </summary>
        /// <param name="appendedText"></param>
        private void pasteText(string appendedText)
        {
            ICSharpCode.TextEditor.TextAreaControl textAreaControl = this.icsSqlEditor.ActiveTextAreaControl;
            if (this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.SelectedText.Length > 0)
            {
                textAreaControl.Caret.Position = textAreaControl.SelectionManager.SelectionCollection[0].StartPosition;
                textAreaControl.SelectionManager.RemoveSelectedText();
            }

            textAreaControl.TextArea.InsertString(appendedText);
        }

        //private void pasteText(string appendedText)
        //{
        //    try
        //    {
        //        fireRedSqlTextChanged = false;

        //        _beforePasteText = Clipboard.GetText();

        //        if (appendedText.Equals(""))
        //        {
        //            //Clipboard.Clear();
        //            return;
        //        }

        //        int procIdx = 0;
        //        ArrayList arrMatch = new ArrayList();

        //        // 收集註解
        //        MatchCollection ms = Regex.Matches(appendedText, "--[^\r\n]*", RegexOptions.IgnoreCase);
        //        for (int i = 0; i < ms.Count; i++)
        //        {
        //            arrMatch.Add(ms[i]);
        //        }
        //        ms = Regex.Matches(appendedText, "/[*].*[*][/]", RegexOptions.Multiline);
        //        for (int i = 0; i < ms.Count; i++)
        //        {
        //            arrMatch.Add(ms[i]);
        //        }

        //        // 收集關鍵字
        //        ms = Regex.Matches(appendedText, "\\w+", RegexOptions.IgnoreCase);
        //        for (int i = 0; i < ms.Count; i++)
        //        {
        //            arrMatch.Add(ms[i]);
        //        }

        //        // 收集SQL字串
        //        ms = Regex.Matches(appendedText, "'[^']{0,}'", RegexOptions.Singleline);
        //        for (int i = 0; i < ms.Count; i++)
        //        {
        //            arrMatch.Add(ms[i]);
        //        }

        //        arrMatch.Sort(new MatchComparer());

        //        // 處理

        //        Color newColor = Color.Black;
        //        string otherText;

        //        for (int i = 0; i < arrMatch.Count; i++)
        //        {
        //            Match aMatch = (Match)arrMatch[i];

        //            if (aMatch.Index < procIdx)  // 被別的區塊理過了
        //            {
        //                continue;
        //            }

        //            newColor = findTextColor(aMatch.Value);

        //            if (aMatch.Index != procIdx)
        //            {
        //                otherText = appendedText.Substring(procIdx, aMatch.Index - procIdx);

        //                redSql.SelectionColor = Color.Black;

        //                pasteTextString(Color.Black, inqFont, Color.Empty, otherText);
        //            }

        //            pasteTextString(newColor, inqFont, Color.White, aMatch.Value);

        //            redSql.SelectionColor = Color.Black;

        //            procIdx = aMatch.Index + aMatch.Length;
        //        }

        //        if (procIdx <= appendedText.Length - 1)
        //        {
        //            pasteTextString(Color.Black, inqFont, Color.Empty, appendedText.Substring(procIdx, appendedText.Length - procIdx));
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        fireRedSqlTextChanged = true;
        //        redSql_TextChanged(null, null);
        //    }
        //}

        private Color findTextColor(string text)
        {
            if (text.StartsWith("--") || text.StartsWith("/*"))
            {
                return Color.Green;
            }

            if (text.StartsWith("'"))  // SQL字串
            {
                return Color.Brown;
            }

            string keywordFormat = verifyText(text);
            if (keywordFormat == InqPage.TEXT_COMMAND)
            {
                return Color.Blue;
            }

            if (keywordFormat == InqPage.TEXT_OPERATOR)
            {
                return Color.Gray;
            }

            if (keywordFormat == InqPage.TEXT_FUNCTION)
            {
                return Color.Fuchsia;
            }

            return Color.Black;
        }

        //private void pasteTextString(Color frontColor, Font textFont, Color backColor, string text)
        //{
        //    if(backColor.Equals(Color.Empty) == false)
        //    {
        //        redSql.SelectionBackColor = backColor;
        //    }

        //    for (int i = 0; i < text.Length; i++)
        //    {
        //        string ch = text.Substring(i, 1);

        //        if(ch.Equals("\r"))
        //        {
        //            continue;
        //        }

        //        redSql.SelectionColor = frontColor;
        //        redSql.SelectionFont = textFont;
        //        redSql.SelectedText = ch;
        //    }
        //}

        private string getNextText(string str, ref int idx)
        {
            if (idx >= str.Length)
            {
                return "";
            }

            string text = "";
            int endidx = idx;

            if (str.Substring(endidx, 1).Equals("\t"))
            {
                ++idx;
                return "\t";
            }

            if (str.Substring(idx, 1).Equals("\r"))  // 判斷換行
            {
                if (str.Substring(idx + 1, 1).Equals("\n"))
                {
                    idx += 2;
                    return "\r\n";
                }
            }

            if (str.Substring(idx, 1).Equals("-"))  // 註解
            {
                if (str.Substring(idx + 1, 1).Equals("-"))
                {
                    idx += 2;
                    return "--";
                }
            }

            if (str.Substring(endidx, 1).Equals(" ") || str.Substring(endidx, 1).Equals("\t"))
            {
                while (str.Substring(endidx, 1).Equals(" ") || str.Substring(endidx, 1).Equals("\t"))
                {
                    ++endidx;

                    if (endidx >= str.Length)
                    {
                        break;
                    }
                }

                --endidx;

                if (endidx >= idx)  // it's spaces
                {
                    text = str.Substring(idx, endidx - idx + 1);
                    idx = endidx + 1;

                    return text;
                }
            }

            // a word
            bool isChineseWord = GlobalClass.CheckChineseString(str, endidx);
            while (str.Substring(endidx, 1).Equals(" ") == false &&
                str.Substring(endidx, 1).Equals("\t") == false)
            {
                ++endidx;

                if (endidx >= str.Length)
                {
                    break;
                }

                if (isChineseWord != GlobalClass.CheckChineseString(str, endidx))
                {
                    break;
                }
            }

            --endidx;

            if (endidx >= idx)
            {
                text = str.Substring(idx, endidx - idx + 1);
                idx = endidx + 1;

                return text;
            }

            return "";
        }

        public const string TEXT_COMMAND = "TEXT_COMMAND";
        public const string TEXT_OPERATOR = "TEXT_OPERATOR";
        public const string TEXT_FUNCTION = "TEXT_FUNCTION";
        public const string TEXT_UNKNOWN = "TEXT_UNKNOWN";

        private string verifyText(string text)
        {
            text = text.Trim().ToUpper();

            // command
            string[] commands = { "SELECT", "FROM", "TOP", "WHERE", "CASE", "WHEN", "LEFT", "JOIN",
                "ORDER", "BY", "ASC", "DESC", "GROUP", "HAVING", "ON",
                "USE", "DELETE", "UPDATE", "IN", "INTO" };
            for (int i = 0; i < commands.Length; i++)
            {
                if (text.Equals(commands[i]))
                {
                    return InqPage.TEXT_COMMAND;
                }
            }

            // 檢查是否有 sql 語法
            string[] keys = { "AND", "OR", "IS", "NOT", "NULL" };

            for (int i = 0; i < keys.Length; i++)
            {
                if (text.Equals(keys[i]))
                {
                    return InqPage.TEXT_OPERATOR;
                }
            }

            // 檢查是否有 function
            for (int i = 0; i < buildinFunctions.Count; i++)
            {
                if (text.Equals(buildinFunctions[i]))
                {
                    return InqPage.TEXT_FUNCTION;
                }
            }

            return InqPage.TEXT_UNKNOWN;
        }

        //private string finLastWord()
        //{
        //    string text = this.redSql.Text;
        //    int endindex = this.redSql.SelectionStart;
        //    int index = endindex - 1;

        //    while (index >= 0)
        //    {
        //        if (text.Substring(index, 1).Equals(" "))
        //        {
        //            ++index;
        //            break;
        //        }

        //        --index;
        //    }

        //    if (index >= endindex)
        //    {
        //        return "";
        //    }

        //    if (index < 0)
        //    {
        //        return "";
        //    }

        //    return text.Substring(index, endindex - index);
        //}

        private void redSql_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Shift == false)
                {
                    _holdShift = false;
                }

                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down ||
                    e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                {
                    //paintParentheses();
                    return;
                }

                if (e.KeyCode == Keys.F7)
                {
                    string selected = this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.SelectedText;

                    if (selected.Length > 0)
                    {
                        //highLightSelectedText(false);
                    }
                    return;
                }

                if (e.KeyCode == Keys.F9 || e.KeyCode == Keys.F5)
                {
                    runStatement();
                    return;
                }

                if (e.KeyCode == Keys.F6)
                {
                    sqlAssist();
                    return;
                }

                if (e.KeyCode == Keys.F12)
                {
                    transferCodeToSQL();
                    return;
                }

                if (e.Alt && e.KeyCode == Keys.J)
                {
                    GlobalClass.logTime("query", "query", "start", true);
                    _holdCtrl = false;
                    selectBlock();
                    runSql();

                    return;
                }

                if (e.Control && e.KeyCode == Keys.F)
                {
                    edTxtSearch.Text = "";
                    edTxtSearch.Focus();
                    return;
                }

                if (e.Control && e.KeyCode == Keys.I)
                {
                    MessageBox.Show(GlobalClass.connInfo());
                    return;
                }

                if (enableCodeAssist)
                {
                    codeAssist();
                }

                //checkPainting();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
            }
        }

        public void InqPageKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Alt)
            {
                executeShortKeySql(e.KeyCode);
            }
        }

        private void executeShortKeySql(Keys code)
        {
            string shortKey = ((char)code).ToString();

            SqliteConn acc = new SqliteConn();

            try
            {
                Reader r = acc.getDataReader(
                    "select content from Inquiry where shortKey='" + shortKey + "' ");

                if (r.Read())
                {
                    string sql = r[0].ToString();

                    sql = compileParameter(sql);
                    sql = compileSql(sql);

                    //sql = applyUsedDatabase(sql, this.ddlDB.Text);
                    runSqlByStatement(InqPage.DS_TYPE_GETDATA, sql, this.ddlConn.SelectedValue.ToString(), false);

                    //updateQueryResult(ds, sql);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw ex;
            }
            finally
            {
                //acc.close();
                _holdCtrl = false;
                _holdShift = false;
                this.progress.Visible = false;
            }
        }

        private string compileParameter(string sql)
        {
            Match m = Regex.Match(sql, "[{][?][}]");

            while (m.Success)
            {
                ParamInput paramForm = new ParamInput();
                paramForm.paramText = "參數內容";
                paramForm.ShowDialog();

                sql = sql.Substring(0, m.Index) + paramForm.paramValue + sql.Substring(m.Index + m.Length);
                m = Regex.Match(sql, "[{][?][}]");
            }

            return sql;
        }

        private bool _holdShift = false;
        private bool _holdCtrl = false;
        private bool _holdAlt = false;
        private string _beforePasteText = "";

        private void redSql_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.K)
                {
                    e.SuppressKeyPress = true;
                    info_Click(null, null);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private DateTime _lastCodeAssistDateTime = DateTime.MinValue;
        private double _lastCodeAssistInterval = 200f;   // 這個時間內不要更新code輔助

        /// <summary>
        /// 文字輔助，列出相符的文字清單
        /// </summary>
        private void codeAssist()
        {
            if (DateTime.Now.Subtract(_lastCodeAssistDateTime).TotalMilliseconds <= _lastCodeAssistInterval)
            {
                return;
            }

            _lastCodeAssistDateTime = DateTime.Now;

            int curpos = this.icsSqlEditor.Document.PositionToOffset(this.icsSqlEditor.ActiveTextAreaControl.Caret.Position);
            string text = this.icsSqlEditor.Text.Substring(0, curpos).ToLower();

            int index;

            listCodeAssist.Items.Clear();

            if (needTableAssistance(text, out index))
            {
                string name = text.Substring(index + 6).Trim();
                tableViewAssistant(name, listCodeAssist);
            }

            string codename = "";
            Match m = needColAssistance(text, out codename);
            if (m.Success)
            {
                ArrayList colList = getColumnsAssist(m, codename);

                for (int i = 0; i < colList.Count; i++)
                {
                    listCodeAssist.Items.Add((string)colList[i]);
                }
            }

            // syntax
            SyntaxAssistant syntax = new SyntaxAssistant();
            ArrayList arrSyntax = syntax.getAssist(text);
            for (int i = 0; i < arrSyntax.Count; i++)
            {
                listCodeAssist.Items.Add((string)arrSyntax[i]);
            }

            if (listCodeAssist.Items.Count > 0)
            {
                listCodeAssist.SelectedIndex = 0;

                Point p = new Point(this.icsSqlEditor.ActiveTextAreaControl.Caret.ScreenPosition.X,
                        this.icsSqlEditor.ActiveTextAreaControl.Caret.ScreenPosition.Y);

                listCodeAssist.Location = new Point(p.X + 30, p.Y + 40);
                listCodeAssist.Show();
            }
            else
            {
                listCodeAssist.Hide();
            }
        }

        private bool needTableAssistance(string text, out int index)
        {
            index = 0;

            // [ex.] from Users
            Match m = Regex.Match(text, "\\sfrom\\s+[_0-9a-z]+\\z", RegexOptions.Singleline);

            if (m.Success)
            {
                index = m.Index;
                return true;
            }

            // [ex.] from NaNa..WebApplication
            m = Regex.Match(text, "\\sfrom\\s+[_0-9a-z]+[.][.][_0-9a-z]+\\z", RegexOptions.Singleline);
            if (m.Success)
            {
                index = m.Index;
                return true;
            }

            // [ex.] join Users
            m = Regex.Match(text, "\\sjoin\\s+[_0-9a-z]+\\z", RegexOptions.Singleline);
            if (m.Success)
            {
                index = m.Index;
                return true;
            }

            // [ex.] join NaNa..WebApplication
            m = Regex.Match(text, "\\sjoin\\s+[_0-9a-z]+[.][.][_0-9a-z]+\\z", RegexOptions.Singleline);
            if (m.Success)
            {
                index = m.Index;
                return true;
            }

            return m.Success;
        }

        private Match needColAssistance(string text, out string name)
        {
            int index = -1;
            Match m = null;

            for (int i = 0; i < 1; i++)
            {
                m = Regex.Match(text, "\\swhere\\s+[\\._0-9a-z]+\\z", RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    index = m.Index + 7;
                    break;
                }

                m = Regex.Match(text, "\\sand\\s+[_0-9a-z]+\\z", RegexOptions.Singleline);
                if (m.Success)
                {
                    index = m.Index + 5;
                    break;
                }

                m = Regex.Match(text, "\\sor\\s+[_0-9a-z]+\\z", RegexOptions.Singleline);
                if (m.Success)
                {
                    index = m.Index + 4;
                    break;
                }

                m = Regex.Match(text, "\\sorder by\\s+[_0-9a-z]+\\z", RegexOptions.Singleline);
                if (m.Success)
                {
                    index = m.Index + 10;
                    break;
                }
            }

            name = "";
            if (index >= 0)
            {
                name = text.Substring(index).Trim();
            }

            return m;
        }

        private void tableViewAssistant(string name, ListBox box)
        {
            string dbName = this.ddlDB.Text;
            name = name.ToLower();

            List<TableInfo> list = CodeAssistant.getTables(this.ddlConn.Text, dbName, name, getConnection());

            if (list.Count == 0)
            {
                messageStatus("Code輔助尚未就緒");
                return;
            }

            // for compute width
            int width = 100, itemWidth;
            Graphics g = box.CreateGraphics();

            #region tables and views

            ArrayList tablelist2 = new ArrayList();
            int count = 0;

            for (int i = 0; i < list.Count && count <= 10; i++)
            {
                bool found = false;
                string table = list[i].TableName;

                if (table.ToLower().Equals(name))
                {
                    return; // 已完成，不用再輔助
                }

                if (table.ToLower().StartsWith(name))
                {
                    box.Items.Add(table);
                    found = true;
                }
                else if (table.ToLower().Contains(name))
                {
                    found = true;
                    tablelist2.Add(table);
                }

                if (found)
                {
                    ++count;

                    // computing width
                    itemWidth = Convert.ToInt32(g.MeasureString(table, box.Font).Width) + 30;
                    width = Math.Max(width, itemWidth);
                    box.Width = width;
                }
            }

            for (int i = 0; i < tablelist2.Count; i++)
            {
                box.Items.Add((string)tablelist2[i]);
            }

            #endregion tables and views

            #region db list

            //count = 0;
            //DataTable dblist = this.ddlDB.DataSource as DataTable;

            //for (int i = 0;dblist != null && i < dblist.Rows.Count && count <= 10; i++)
            //{
            //    string dbname = dblist.Rows[i][0].ToString();

            //    if (dbname.ToLower().StartsWith(name))
            //    {
            //        box.Items.Add(dbname);
            //        ++count;

            //        // computing width
            //        itemWidth = Convert.ToInt32(g.MeasureString(dbname, box.Font).Width) + 30;
            //        width = Math.Max(width, itemWidth);
            //    }
            //}

            if (box.Items.Count == 1 && box.Items[0].ToString().ToLower().Equals(name))
            {
                box.Items.Clear();
            }

            #endregion db list
        }

        private Hashtable _columnsAssist = new Hashtable();

        private ArrayList getColumnsAssist(Match m, string name)
        {
            try
            {
                ArrayList list = new ArrayList();

                int dotidx = name.IndexOf(".");
                if (dotidx >= 0)
                {
                    name = name.Substring(dotidx + 1);
                }

                int[] blockRange = findBlock();

                string text = this.icsSqlEditor.Text.Substring(blockRange[0], blockRange[1] - blockRange[0]);

                MatchCollection ms1 = Regex.Matches(text, "\\sfrom\\s+[\\._0-9a-z]+\\s+", RegexOptions.IgnoreCase);
                MatchCollection ms2 = Regex.Matches(text, "\\sjoin\\s+[\\._0-9a-z]+\\s+", RegexOptions.IgnoreCase);

                addColtoList(ms1, list, text, name);
                addColtoList(ms2, list, text, name);

                return list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return null;
        }

        private void addColtoList(MatchCollection ms, ArrayList list, string text, string name)
        {
            try
            {
                ColInfoAssistant colAssist = new ColInfoAssistant();

                // 取出符合的table
                for (int i = 0; i < ms.Count; i++)
                {
                    string tablename = getName(text, ms[i]);
                    List<ColumnInfo> colList = CodeAssistant.getColumnInfo(this.ddlConn.Text, this.ddlDB.Text, tablename, getConnection());

                    ArrayList secondList = new ArrayList();
                    if (colList.Count > 0)
                    {
                        for (int j = 0; j < colList.Count; j++)
                        {
                            string colname = colList[j].ColName;

                            if (colname.ToLower().Equals(name))
                            {
                                continue;
                            }

                            if (colname.ToLower().StartsWith(name.ToLower()))
                            {
                                list.Add(colname);
                            }
                            else if (colname.ToLower().Contains(name.ToLower()))
                            {
                                secondList.Add(colname);
                            }
                        }

                        for (int j = 0; j < secondList.Count; j++)
                        {
                            list.Add((string)secondList[j]);
                        }
                    }
                }  // end for
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private string getName(string text, Match m)
        {
            string name = text.Substring(m.Index + 6);
            name = name.Replace("\n", " ").Replace("\r", " ");

            int endidx = name.IndexOf(" ");

            if (endidx < 0)
            {
                return "";
            }

            return name.Substring(0, endidx);
        }

        private void sqlAssist()
        {
            string sql = "";
            if (Control.ModifierKeys == Keys.Control)
            {
                sql = "Select * from ";
            }
            else
            {
                sql = "Select top 10 * from ";
            }

            pasteText(sql);
        }

        private void redSql_Leave(object sender, EventArgs e)
        {
            messageStatus("");
        }

        /// <summary>
        /// 點選grid中資料時，資料的內容
        /// </summary>
        private string _selectedData = "";

        private void dgResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            
            string val = "";
            object objVal = dgv.SelectedCells[0].Value;

            if(objVal.GetType() == typeof(DateTime))
            {
                val = ((DateTime)objVal).ToString("yyyy/MM/dd HH:mm:ss.fff");
            }
            else
            {
                val = GlobalClass.str(objVal);
            }

            _selectedRow = e.RowIndex;
            _selectedCol = e.ColumnIndex;
            selectedRowChanged();

            if (Control.ModifierKeys == Keys.Alt)
            {
                _selectedData = val;
            }

            fireSelectedContentTextChanged = false;
            this.edSelectedContent.Text = val;
            fireSelectedContentTextChanged = true;

            messageStatus("右鍵:複製該欄位成為查詢條件，F8: 複製欄位名稱， F11: 匯出資料， F12: 匯出CSV檔，輸入文字:欄位名稱搜尋");
        }

        private bool changeAreProcessing = false;

        private void selectedRowChanged()
        {
        }

        private void dgResult_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            DataGridViewSelectedCellCollection selectedCells = currentDataGrid.SelectedCells;

            if (selectedCells.Count == 0)
            {
                return;
            }

            DataTable dt = this.currentDataGrid.DataSource as DataTable;

            string text = "";
            for (int i = selectedCells.Count - 1; i >= 0; i--)
            {
                int colindex = selectedCells[i].ColumnIndex;
                string colname = dt.Columns[colindex].ColumnName;

                ExportedValue expValue = exportValue(selectedCells[i].Value, dt.Columns[colindex].DataType);
                string value = expValue.value; //selectedCells[i].Value.ToString();

                text += colname + " = " + value + " and ";
            }

            if (!text.Equals(""))
            {
                text = text.Substring(0, text.Length - 4);
            }

            Clipboard.Clear();

            try
            {
                Clipboard.SetText(text, TextDataFormat.UnicodeText);
                messageStatusHighlight("已將條件複製到剪貼簿");
            }
            catch { }
        }

        private void getTableName(string sql)
        {
            // find table name
            string statement = ""; // getStatement().Replace("\r", " ").Replace("\n", " ");

            if (sql != null)
            {
                statement = sql;
            }
            else
            {
                statement = getStatement();
            }

            statement = statement.Replace("\r", " ").Replace("\n", " ");

            string[] items = statement.Split(" ".ToCharArray());

            string tableName = "";
            bool foundKeyword = false;
            for (int i = 0; i < items.Length; i++)
            {
                if (foundKeyword)
                {
                    if (items[i].Trim() == "")
                    {
                        continue;
                    }
                    else
                    {
                        tableName = items[i];
                        break;
                    }
                }
                else if (items[i].ToLower().Trim() == "from")
                {
                    foundKeyword = true;
                }
            }

            this.currentTableName = tableName;
        }

        private void makeInsertStatement(string tablename)
        {
            DataGridViewSelectedRowCollection rows = this.currentDataGrid.SelectedRows;

            DataTable dt = this.currentDataGrid.DataSource as DataTable;

            int i;
            bool includeIdentity = false, askedIdentity = false;

            string preStatement = "Insert into " + tablename + " (";

            for (i = 0; i < dt.Columns.Count; i++)
            {
                if (dt.Columns[i].AutoIncrement && !askedIdentity)
                {
                    DialogResult r = MessageBox.Show("是否包含IDENTITY欄位?", "", MessageBoxButtons.YesNo);

                    askedIdentity = true;

                    if (r == DialogResult.Yes)
                    {
                        includeIdentity = true;
                    }
                }

                if (!dt.Columns[i].AutoIncrement ||   // 不是IDENTITY
                    (dt.Columns[i].AutoIncrement && includeIdentity)  // IDENTITY
                )
                {
                    preStatement += dt.Columns[i].ColumnName + ",";
                }
            }

            preStatement = preStatement.TrimEnd(",".ToCharArray()) + ") ";

            ExportedValue expValue;
            string statement;
            StringBuilder finalStatement = new StringBuilder();

            for (i = 0; i < rows.Count; i++)
            {
                DataGridViewRow row = rows[i];
                statement = preStatement + "\r\nvalues(";

                //object objValue;

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Columns[j].AutoIncrement && !includeIdentity)  // IDENTITY
                    {
                        continue;
                    }

                    string val = GlobalClass.str(row.Cells[j].Value);
                    //objValue = row.Cells[j].Value;

                    expValue = exportValue(row.Cells[j].Value, dt.Columns[j].DataType);

                    statement += expValue.value + ",";
                }
                statement = statement.TrimEnd(",".ToCharArray()) + ");\r\n";

                finalStatement.Append(statement);
            }

            Clipboard.Clear();
            Clipboard.SetText(finalStatement.ToString(), TextDataFormat.UnicodeText);

            messageStatus("已複製到剪貼簿");
        }

        private ExportedValue exportValue(object objValue, Type columnType)
        {
            string val = GlobalClass.str(objValue);

            ExportedValue expValue = new ExportedValue() { needQuote = false, isNull = false };

            if (objValue == DBNull.Value)
            {
                expValue.isNull = true;
                return expValue;
            }

            if (val.Equals("") && columnType == typeof(Decimal))
            {
                expValue.isNull = true;
                return expValue;
            }
            else if (columnType == typeof(DateTime))
            {
                if (objValue != null && objValue.ToString().Length != 0)
                {
                    DateTime dtValue = (DateTime)objValue;
                    string strValue = dtValue.ToString("yyyy-MM-dd HH:mm:ss.fff").TrimEnd('0').TrimEnd('.');

                    expValue.needQuote = true;
                    expValue.rawValue = strValue;
                    return expValue;
                }
                else
                {
                    expValue.isNull = true;
                    return expValue;
                }
            }
            else if (columnType == typeof(int))
            {
                if (objValue != null && objValue.ToString().Length != 0)
                {
                    int ival = (int)objValue;

                    expValue.rawValue = ival.ToString();
                    return expValue;
                }
                else
                {
                    expValue.isNull = true;
                    return expValue;
                }
            }
            else if (columnType == typeof(bool))
            {
                if (objValue != null && objValue.ToString().Length != 0)
                {
                    bool bval = (bool)objValue;
                    expValue.rawValue = (bval ? "1" : "0");
                    return expValue;
                }
                else
                {
                    expValue.isNull = true;
                    return expValue;
                }
            }
            else
            {
                expValue.needQuote = true;
                expValue.rawValue = val.Replace("'", "''");
                return expValue;
            }
        }

        private void exportCSV()
        {
            DataGridViewSelectedRowCollection rows = this.currentDataGrid.SelectedRows;

            DataTable dt = this.currentDataGrid.DataSource as DataTable;

            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                sb.AppendFormat("\"{0}\",", dt.Columns[j].ColumnName);
            }

            sb.Remove(sb.Length - 1, 1);
            sb.AppendLine();

            ExportedValue expValue;
            string strValue;

            for (int i = rows.Count - 1; i >= 0; i--)
            {
                DataGridViewRow row = rows[i];

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    //object obj = ((DataRowView)rows[i].DataBoundItem)[j];
                    expValue = exportValue(row.Cells[j].Value, dt.Columns[j].DataType);

                    strValue = expValue.isNull ? "" : expValue.rawValue;

                    sb.AppendFormat("\"{0}\",", strValue.Replace("\"", "\"\""));
                }

                sb.Remove(sb.Length - 1, 1);
                sb.AppendLine();
            }

            fileDialog.DefaultExt = "csv";
            fileDialog.FileName = this.currentTableName; // getTableName(null);

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string fname = fileDialog.FileName;

                System.IO.StreamWriter sw;

                try
                {
                    sw = new System.IO.StreamWriter(fname, false, Encoding.UTF8);
                }
                catch (IOException ioe)
                {
                    MessageBox.Show("無法寫入檔案，檔案可能已被其他程序鎖住");
                    return;
                }

                sw.Write(sb.ToString());
                sw.Close();

                MessageBox.Show("匯出完成");
            }
        }

        private void dgResult_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            _selectedRow = e.RowIndex;
            selectedRowChanged();
        }

        private void btnShowTables_Click(object sender, EventArgs e)
        {
            DataSet ds = null;

            //ds = tableViewList;

            //if (ds == null)
            //{
            string sql = "use [" + this.ddlDB.Text + "] select * from INFORMATION_SCHEMA.TABLES with(nolock) order by TABLE_TYPE, TABLE_NAME";
            sql = applyUsedDatabase(sql, this.ddlDB.Text);

            ds = runSqlByStatement(InqPage.DS_TYPE_TABLE_VIEW, sql, this.ddlConn.SelectedValue.ToString(), InqPage.getConnectionSilent);
            //}

            updateQueryResult(ds, "");

            tabInquiry.SelectTab(1);
        }

        private void btnShowRoutines_Click(object sender, EventArgs e)
        {
            DataSet ds = null; // spFunctionList;

            //if (ds == null)
            //{
            string sql = "use [" + ddlDB.Text + "] select ROUTINE_SCHEMA,ROUTINE_NAME,ROUTINE_TYPE,ROUTINE_DEFINITION " +
                         "from INFORMATION_SCHEMA.ROUTINES with(nolock) order by ROUTINE_TYPE, ROUTINE_NAME";
            sql = applyUsedDatabase(sql, this.ddlDB.Text);
            ds = runSqlByStatement(InqPage.DS_TYPE_FUNC_SP, sql, this.ddlConn.SelectedValue.ToString(), InqPage.getConnectionSilent);
            //}

            updateQueryResult(ds, "");

            tabInquiry.SelectTab(1);
        }

        private void ddlConn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._ddlConnChanged == false)
            {
                return;
            }

            try
            {
                if (this.ddlConn.SelectedValue.ToString().Equals("999"))
                {
                    string dblistPath = System.Configuration.ConfigurationManager.AppSettings["DbList"];
                    System.Diagnostics.Process.Start(dblistPath);

                    return;
                }

                GlobalClass.debugLog("ddlConn_SelectedIndexChanged", "start");
                if (this.ddlConn.SelectedValue.GetType().Equals(typeof(string)) == false)
                {
                    return;
                }

                InqPage.getConnectionSilent = true;

                getConnection(this.ddlConn.SelectedValue.ToString());
                this.ddlDB.DataSource = new DataTable();
                this.ddlDB.Text = "";

                InqPage.getConnectionSilent = false;

                cleanCodeAssistList();

                // get last used db
                SqliteConn acc = new SqliteConn();

                string connname = ddlConn.Text;

                //Reader r = acc.getDataReader(string.Format("select LastUsedDB from UsedDB where DBConnName = '{0}' ", connname));
                Reader r = acc.getDataReader(string.Format("select DBName from FavoriteDb where DBConnName = '{0}' order by ModifiedDate desc limit 0,1",
                    connname));

                if (r.Read())
                {
                    this.ddlDB.Text = r[0].ToString();
                }
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
            }
            finally
            {
                this._ddlConnChanged = true;
            }
        }

        private void btnSaveSql_Click(object sender, EventArgs e)
        {
            saveContent("");    // 第一層才有儲存按鈕,因此沒有parent id
        }

        private void saveContent(string parentId)
        {
            if (this.Visible)
            {
                _id = GlobalClass.saveInq(_id, parentId, this.ddlGroup.Text, this.edFormName.Text, this.icsSqlEditor.Text, this.edShortKey.Text);
            }
            else
            {
                GlobalClass.deleteInq(_id);
            }

            for (int i = 0; i < _childInquiries.Count; i++)
            {
                InqPage inq = _childInquiries[i] as InqPage;

                if (inq == null)
                {
                    continue;
                }

                inq.saveContent(this._id);
            }
        }

        private void btnFormat_Click(object sender, EventArgs e)
        {
            string sql = "";

            sql = getStatement();

            PoorMansTSqlFormatterLib.SqlFormattingManager sqlm = new PoorMansTSqlFormatterLib.SqlFormattingManager();
            bool erren = false;
            string formattedSql = sqlm.Format(sql, ref erren);

            #region 將select後面多個欄位串在一起

            string regPattern = "\n[\\s\t]*,[^,\n]+";
            string patternText = "";
            for (int i = 0; i < 7; i++)   // 連續出現次數
            {
                patternText += regPattern;
            }

            MatchCollection ms = Regex.Matches(formattedSql, patternText, RegexOptions.Singleline);
            int procIdx = 0;

            string formattedSql2 = "";
            for (int i = 0; i < ms.Count; i++)
            {
                Match m = ms[i];

                if (m.Index > procIdx)
                {
                    formattedSql2 += formattedSql.Substring(procIdx, m.Index - procIdx).TrimEnd("\r".ToCharArray());
                }

                formattedSql2 += "\r\n" + m.Value.Replace("\r", "").Replace("\n", "").Replace("\t", "");

                procIdx = m.Index + m.Length;
            }

            if (procIdx < formattedSql.Length)
            {
                formattedSql2 += formattedSql.Substring(procIdx, formattedSql.Length - procIdx);
            }

            #endregion 將select後面多個欄位串在一起

            enableCodeAssist = false;

            if (this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.HasSomethingSelected == false)
            {
                this.icsSqlEditor.Text = "";
            }

            pasteText(formattedSql2);

            enableCodeAssist = true;
        }

        #region 中括號計算

        //ArrayList _parenthesesList;
        //Match _lastMatch = null;

        //private void findParentheses()
        //{
        //    string text = this.icsSqlEditor.Text;
        //    MatchCollection matchs = null;

        //    _parenthesesList = new ArrayList();

        //    while (true)
        //    {
        //        matchs = Regex.Matches(text, "[(][^()]{0,}[)]", RegexOptions.Singleline);
        //        if (matchs.Count == 0)
        //        {
        //            break;
        //        }

        //        _parenthesesList.Add(matchs);

        //        IEnumerator ienum = matchs.GetEnumerator();
        //        while (ienum.MoveNext())
        //        {
        //            Match m = (Match)ienum.Current;

        //            string str1, str2, str3 = "";
        //            str1 = text.Substring(0, m.Index);
        //            str2 = text.Substring(m.Index, m.Length);

        //            if (m.Index + m.Length < text.Length)
        //            {
        //                str3 = text.Substring(m.Index + m.Length);
        //            }

        //            text = str1 + str2.Replace("(", "$").Replace(")", "$") + str3;
        //        }
        //    }

        //}

        //private Match findEnteredSegment()
        //{
        //    if (_parenthesesList == null)
        //    {
        //        return null;
        //    }

        //    Match foundMatch = null;

        //    for (int i = 0; i < _parenthesesList.Count; i++)
        //    {
        //        MatchCollection collection = (MatchCollection)_parenthesesList[i];
        //        IEnumerator ienum = collection.GetEnumerator();

        //        while (ienum.MoveNext())
        //        {
        //            Match match = (Match)ienum.Current;
        //            if (match.Success == false)
        //            {
        //                continue;
        //            }

        //            int endidx = match.Index + match.Length - 1;

        //            if (this.redSql.SelectionStart >= match.Index && this.redSql.SelectionStart <= endidx)
        //            {
        //                if (foundMatch == null)
        //                {
        //                    foundMatch = match;
        //                }
        //                else
        //                {
        //                    if (match.Length < foundMatch.Length)
        //                    {
        //                        foundMatch = match;
        //                    }
        //                }
        //            }
        //        } // end while
        //    } // end for

        //    return foundMatch;
        //}

        //private void paintParentheses()
        //{
        //    if (Control.ModifierKeys == Keys.Shift)
        //    {
        //        return;
        //    }

        //    int start = this.redSql.SelectionStart;

        //    if (chkParentheses.Checked == false)
        //    {
        //        return;
        //    }

        //    Match foundMatch = findEnteredSegment();

        //    if (foundMatch != null)
        //    {
        //        bool needPaint = false;
        //        if (_lastMatch == null)
        //        {
        //            needPaint = true;
        //        }
        //        else if (foundMatch.Index == _lastMatch.Index &&
        //               foundMatch.Length == _lastMatch.Length)
        //        {
        //            needPaint = false;
        //        }
        //        else
        //        {
        //            needPaint = true;
        //        }

        //        if (needPaint)
        //        {
        //            recoverLastParentheses();

        //            this.redSql.Select(foundMatch.Index, foundMatch.Length);
        //            this.redSql.SelectionBackColor = Color.PaleGoldenrod; //Color.Khaki;
        //            _lastMatch = foundMatch;
        //        }
        //    }
        //    else
        //    {
        //        recoverLastParentheses();
        //    }

        //    this.redSql.SelectionStart = start;
        //    this.redSql.SelectionLength = 0;
        //}

        //private void recoverLastParentheses()
        //{
        //    if (_lastMatch == null)
        //    {
        //        return;
        //    }

        //    this.redSql.Select(_lastMatch.Index, _lastMatch.Length);
        //    this.redSql.SelectionBackColor = Color.White;
        //    this.redSql.SelectionLength = 0;

        //    _lastMatch = null;
        //}

        #endregion 中括號計算

        private int _accumulatedInterval = 0;
        private bool _queryResultUpdating = false;

        private void timer1_Tick(object sender, EventArgs e)
        {
            checkDBDropDown();

            if (this._queryResultUpdating)
            {
                if (afterRunSql)
                {
                    GlobalClass.logTime("query", "query", "_queryResultUpdating", false);
                }
                return;
            }

            if (InqPage._engine != null)
            {
                this.lblConnReady.Text = "已連線";
            }
            else
            {
                this.lblConnReady.Text = "";
            }

            #region get data set

            if (afterRunSql)
            {
                string msg = string.Format("_t {0}, _s {1}, _d {2}",
                    (this._threadGetData == null) ? "null" : "Y",
                    (this._threadGetData != null) ? this._threadGetData.ThreadState.ToString() : "null",
                    (this._result["result"] == null) ? "null" : "Y");
                GlobalClass.logTime("query", "query", msg, false);
            }

            if (this._threadGetData != null)
            {
                if (this._threadGetData.ThreadState != ThreadState.Running)
                {
                    this._threadGetData.Abort();
                    this._threadGetData = null;
                    this.progress.Visible = false;

                    DataSet ds = this._result["result"] as DataSet;
                    GlobalClass.logTime("query", "query", "_result", false);

                    if (ds != null)
                    {
                        afterRunSql = false;

                        string sql = this._result["sql"] as string;

                        try
                        {
                            this._queryResultUpdating = true;

                            if (_result["resultName"].ToString().Equals(InqPage.DS_TYPE_DBLIST))
                            {
                                //updateDBList(ds.Tables[0]);
                            }
                            else
                            {
                                if (ds.Tables.Count > 0)
                                {
                                    GlobalClass.logTime("query", "query", "updateQueryResult s", false);
                                    updateQueryResult(ds, sql);
                                    GlobalClass.logTime("query", "query", "updateQueryResult e", false);
                                }
                                else
                                {
                                    MessageBox.Show("完成");
                                }
                            }

                            this.edSqlMessage.Text = GlobalClass.str(_result["mssqlMessage"]);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            this._queryResultUpdating = false;
                        }
                    }

                    string msg = GlobalClass.str(this._result["message"]);
                    if (!msg.Equals(""))
                    {
                        MessageBox.Show(msg);
                    }
                }
                else  // 還在執行
                {
                    object objT1 = this._result["startTime"];
                    if (objT1 != null)
                    {
                        DateTime t1 = (DateTime)objT1;
                        DateTime t2 = DateTime.Now;
                        this._lastSpanTime = t2.Subtract(t1);
                        showSpanTime(false);
                    }
                }
            }

            if (isCanceled)
            {
                isCanceled = false;
                this.progress.Visible = false;
                DBConn engine = this._result["engine"] as DBConn;

                if (engine != null)
                {
                    engine.cancelCommand();
                }
                this._threadGetData = null;
            }

            #endregion get data set

            _accumulatedInterval += this.timer1.Interval;

            // running every 5 seconds
            //if (_accumulatedInterval >= 5000)
            //{
            //    _accumulatedInterval = 0;

            //    if (_textChanged)
            //    {
            //        _textChanged = false;
            //    }

            //    checkTableViews(true);
            //}

            checkTableViews(true);

            int[] connCnt = GlobalClass.connCount();
            lblConnCnt.Text = "Total: " + connCnt[0].ToString() + ", Using: " + connCnt[1].ToString();
        }

        private void updateDBList(DataTable dt)
        {
            string selected = this.ddlDB.Text;

            sortByFavorite(dt);
            dt.DefaultView.Sort = "Fav DESC";

            this.ddlDB.DropDownHeight = 120;
            this.ddlDB.DataSource = dt;
            this.ddlDB.DisplayMember = "name";
            this.ddlDB.ValueMember = "name";

            if (selected.Equals("") == false)
            {
                this.ddlDB.Text = selected;
            }
        }

        private void sortByFavorite(DataTable dt)
        {
            SqliteConn db = new SqliteConn();

            dt.Columns.Add("Fav");

            Reader reader = db.getDataReader(string.Format("select DBName from FavoriteDb where DBConnName = '{0}' limit 0, 3", this.ddlConn.Text));

            while (reader.Read())
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string favDBName = reader[0].ToString();
                    if (dt.Rows[i]["name"].ToString().Equals(favDBName))
                    {
                        dt.Rows[i]["Fav"] = "1";
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Code輔助清單是否已就緒
        /// </summary>
        private bool alertAssistantReady = false;

        private Thread thGettingTables = null;

        /// <summary>
        /// 檢查是否有取過table/view/sp/function 資訊
        /// </summary>
        private void checkTableViews(bool silent)
        {
            try
            {
                if (silent)
                {
                    InqPage.getConnectionSilent = true;
                }

                //if (_dtTableViewList != null)
                //{
                //    if (this.alertAssistantReady == false)
                //    {
                //        this.alertAssistantReady = true;
                //        messageStatus("Code輔助已就緒");
                //    }
                //    return;   // ok
                //}

                //if (this.ddlDB.Text == "")
                //{
                //    return;
                //}

                if (_threadGetTableViewsRunning)
                {
                    return;
                }

                if (databaseList != null)
                {
                    return;
                }

                thGettingTables = new Thread(new ParameterizedThreadStart(threadGetTableViews));

                Hashtable ht = new Hashtable();
                ht["conn"] = ddlConn.SelectedValue;
                ht["silent"] = InqPage.getConnectionSilent;
                thGettingTables.Start(ht);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                InqPage.getConnectionSilent = false;
            }
        }

        private void stopGettingTableViews()
        {
            //if (thGettingTables != null)
            //{
            //    if (thGettingTables.ThreadState == ThreadState.Running ||
            //        thGettingTables.ThreadState == ThreadState.WaitSleepJoin)
            //    {
            //        thGettingTables.Abort();
            //    }
            //}

            //_threadGetTableViewsRunning = false;
        }

        public static bool _threadGetTableViewsRunning = false;

        public void threadGetTableViews(object param)
        {
            try
            {
                _threadGetTableViewsRunning = true;

                GlobalClass.debugLog("InqPage", "threadGetTableViews, start");

                InqPage.getConnectionSilent = true;

                Hashtable ht = (Hashtable)param;

                string pDdlConn = ht["conn"].ToString();
                bool silent = (bool)ht["silent"];

                DBConn engine = GlobalClass.getEngine(pDdlConn);

                string sqlDbList = "select name from master..sysdatabases with(nolock) where name not in ('master','tempdb','model','msdb') ";

                DataSet dsDbList = engine.getDataSet(sqlDbList, "temp");

                databaseList = dsDbList;
            }
            catch (System.Data.SqlClient.SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
            }
            finally
            {
                InqPage.getConnectionSilent = false;
                _threadGetTableViewsRunning = false;
            }
        }

        private void btnLoadSql_Click(object sender, EventArgs e)
        {
            Main sqlPanel = new Main();
            sqlPanel.ShowDialog();

            setData(sqlPanel._selected[0], sqlPanel._selected[1], sqlPanel._selected[2], sqlPanel._selected[3], sqlPanel._selected[4], this._inqForm, this._tabPage);
        }

        private void edFormName_TextChanged(object sender, EventArgs e)
        {
            setTabPagename(this._tabPage, this.edFormName.Text);
        }

        private void redSql_MouseDown(object sender, MouseEventArgs e)
        {
            //int pos = this.redSql.SelectionStart;
            //int len = this.redSql.SelectionLength;
            //clearHighlight();

            //this.redSql.SelectionStart = pos;
            //this.redSql.SelectionLength = len;
        }

        private void redSql_MouseUp(object sender, MouseEventArgs e)
        {
            if (listCodeAssist.Visible)
            {
                listCodeAssist.Hide();
            }
        }

        private void redSql_Click(object sender, EventArgs e)
        {
            if (isSubInquiry())
            {
                messageStatus("在子查詢視窗中，可將上一層視窗中的查詢結果，將其欄位資料帶入本查詢中，例如 - select * from AA where id={欄位代號} ");
            }

            if (Control.ModifierKeys == Keys.Alt)
            {
                selectBlock();

                if (Control.ModifierKeys == Keys.Control)
                {
                    _holdCtrl = false;
                    _holdAlt = false;
                    return;
                }

                try
                {
                    GlobalClass.logTime("query", "query", "start", true);
                    _holdAlt = false;
                    runSql();
                }
                catch (Exception ex)
                {
                    GlobalClass.errorLog(ex.ToString());
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    _holdAlt = false;
                    _holdCtrl = false;
                    _holdShift = false;
                }
                //_holdCtrl = false;
            }
        }

        private void btnPaint_Click(object sender, EventArgs e)
        {
            //painting();
        }

        private void ddlConn_SelectedValueChanged(object sender, EventArgs e)
        {
            if (this.ddlConn.SelectedValue.ToString().Equals("999"))
            {
                return;
            }

            updateConnectStrContent();
            this.lblConnReady.Text = "";
        }

        private void updateConnectStrContent()
        {
            if (ddlConn.SelectedValue == null)
            {
                return;
            }

            string connstr = GlobalClass.getEngineConnStr(ddlConn.SelectedValue.ToString());

            fireSelectedContentTextChanged = false;
            this.edSelectedContent.Text = connstr;
            fireSelectedContentTextChanged = true;

            messageStatus("選取的連線字串，已顯示在內容頁籤");
        }

        private void btnCloseTab_Click(object sender, EventArgs e)
        {
            DialogResult dResTest = MessageBox.Show("確定嗎?", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dResTest == DialogResult.Yes)
            {
                this._inqForm.closeTab(this._tabPage);
                this.Visible = false;
            }
        }

        private void dgResult_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.tabInquiry.SelectTab(2);
        }

        private void tabInquiry_Selected(object sender, TabControlEventArgs e)
        {
            showSpanTime(true);
        }

        private void showSpanTime(bool showDecimal)
        {
            string msg = string.Format("花費時間 {0}:{1}",
                this._lastSpanTime.Minutes.ToString(),
                this._lastSpanTime.Seconds.ToString());

            if (showDecimal)
            {
                msg += "." + this._lastSpanTime.Milliseconds.ToString();
            }
            messageStatus(msg);
        }

        private void messageStatusHighlight(string msg)
        {
            this.toolStripStatusLabel1.ForeColor = Color.Red;
            this.toolStripStatusLabel1.Text = msg;
        }

        private void messageStatus(string msg)
        {
            if (_selectedData.Equals("") == false)
            {
                string extendedMsg = _selectedData;
                if (extendedMsg.Length > 20)
                {
                    extendedMsg = extendedMsg.Replace("\r\n", "\\r\\n");
                    extendedMsg = extendedMsg.Substring(0, 20) + "...";
                }

                msg += "  {@}=" + extendedMsg;
            }

            this.toolStripStatusLabel1.ForeColor = Color.Black;
            this.toolStripStatusLabel1.Text = msg;
        }

        private string _searchStr = "";

        private void dgResult_KeyDown(object sender, KeyEventArgs e)
        {
            //_holdCtrl = e.Control;

            if (e.KeyCode == Keys.F11)
            {
                string tableName = this.currentTableName; // getTableName(null);

                // confirm table name
                if (this.currentDataGrid.SelectedRows.Count > 0)
                {
                    CopyData cd = new CopyData();
                    cd.setTableName(tableName);
                    cd.ShowDialog();

                    // make insert statement
                    if (cd.OK)
                    {
                        makeInsertStatement(cd._tablename);
                    }
                }
                else
                {
                    MessageBox.Show("沒有選擇資料列");
                }

                return;
            }

            if (e.KeyCode == Keys.F12)
            {
                if (this.currentDataGrid.SelectedRows.Count > 0)
                {
                    exportCSV();
                }
                else
                {
                    MessageBox.Show("沒有選擇資料列");
                }

                return;
            }

            if (e.KeyCode == Keys.F8)
            {
                DataGridViewSelectedCellCollection selectedCells = currentDataGrid.SelectedCells;

                if (selectedCells.Count == 0)
                {
                    MessageBox.Show("沒有選擇資料");
                    return;
                }

                DataTable dt = this.currentDataGrid.DataSource as DataTable;
                string text = "";

                for (int i = 0; i < selectedCells.Count; i++)
                {
                    int colindex = selectedCells[i].ColumnIndex;
                    string colName = dt.Columns[colindex].ColumnName;

                    text += colName + ", ";
                }

                text = text.Trim().TrimEnd(',');

                Clipboard.Clear();
                Clipboard.SetText(text, TextDataFormat.UnicodeText);

                messageStatusHighlight("已複製欄位名稱");

                return;
            }

            if (e.KeyCode == Keys.Back)
            {
                _searchStr = "";
                listCandidate.Visible = false;
            }

            if (e.KeyCode == Keys.Escape)
            {
                listCandidate.Visible = false;
            }
        }

        private void dgResult_KeyUp(object sender, KeyEventArgs e)
        {
            // character
            string input = getInput(e);

            //_holdCtrl = e.Control;

            if (input.Length == 1)
            {
                _searchStr += input.ToLower();

                listCandidate.Items.Clear();

                for (int i = 0; i < currentDataGrid.Columns.Count; i++)
                {
                    string name = currentDataGrid.Columns[i].Name.ToLower();

                    currentDataGrid.Columns[i].DefaultCellStyle.BackColor = Color.White;
                    if (name.Contains(_searchStr))
                    {
                        if (listCandidate.Items.Count == 0)  // first
                        {
                            //currentDataGrid.FirstDisplayedScrollingColumnIndex = i;
                            //currentDataGrid.Columns[i].DefaultCellStyle.BackColor = Color.LightYellow;
                            jumpToGridColumn(currentDataGrid, i);
                        }

                        listCandidate.Items.Add(currentDataGrid.Columns[i].Name);
                    }
                }

                if (listCandidate.Items.Count > 1)
                {
                    listCandidate.Visible = true;
                }
            }

            if (e.Control && e.KeyCode == Keys.K)
            {
                //MessageBox.Show(GlobalClass.logString("query"));
            }

            messageStatus("尋找 " + _searchStr + " (按倒退鍵清除)");
        }

        private void jumpToGridColumn(DataGridView grid, int colidx)
        {
            grid.FirstDisplayedScrollingColumnIndex = colidx;
            grid.Columns[colidx].DefaultCellStyle.BackColor = Color.LightYellow;
        }

        private string getInput(KeyEventArgs e)
        {
            if (e.Control || e.Alt)
            {
                return "";
            }

            int code = e.KeyValue;

            if (code >= 65 && code <= 90)
            {
                return ((char)code).ToString();
            }

            if (code >= 48 && code <= 57)
            {
                // digital number
                return ((char)code).ToString();
            }

            if (code == 189 && e.Shift == true)
            {
                return "_";
            }

            return "";
        }

        private bool saveColumnWidth = true;

        private void dgResult_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            string dbname = this.ddlDB.Text;
            string table = this.currentTableName;
            string column;

            string sql = string.Format("select width from ColumnSetting where dbName = '{0}' and tableName = '{1}' ",
                dbname, table) + "and col = '{0}' ";

            SqliteConn acc = null;

            try
            {
                acc = new SqliteConn();
                saveColumnWidth = false;

                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    column = dgv.Columns[i].Name;

                    int width = dgv.Columns[i].GetPreferredWidth(DataGridViewAutoSizeColumnMode.DisplayedCells, true);
                    if (width > 300)
                    {
                        width = 300;
                    }

                    int savedWidth = 0;
                    Reader r = acc.getDataReader(string.Format(sql, column));
                    if (r.Read())
                    {
                        savedWidth = int.Parse(r[0].ToString());
                    }

                    if (savedWidth > 0)
                    {
                        width = savedWidth;
                    }

                    dgv.Columns[i].Width = width;
                }

                verifyGridFormat(dgv);
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
            }
            finally
            {
                saveColumnWidth = true;

                //if (acc != null)
                //{
                //    acc.close();
                //}
            }
        }

        private void tocodeItem_Click(object sender, EventArgs e)
        {
            string sql = getStatement().Trim();

            string[] items = sql.Split('\n');

            string code = "string sql = ";
            for (int i = 0; i < items.Length; i++)
            {
                if (i != 0)
                {
                    code += "             ";
                }

                code += "\"" + items[i].Trim() + " \" +\r\n";
            }

            code = code.TrimEnd(" +\r\n".ToCharArray()) + ";";

            Clipboard.Clear();
            Clipboard.SetText(code, TextDataFormat.UnicodeText);

            messageStatusHighlight("已複制到剪貼簿");
        }

        private void toSql_Click(object sender, EventArgs e)
        {
            transferCodeToSQL();
        }

        private Hashtable changedList = new Hashtable();

        private void dgResult_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // find the pk
                string key = getCurrentRowKey(e.RowIndex);

                Hashtable changedColumns;
                if (changedList.Contains(key))
                {
                    changedColumns = (Hashtable)changedList[key];
                }
                else
                {
                    changedColumns = new Hashtable();
                    changedList.Add(key, changedColumns);
                }

                // add modified column name
                if (changedColumns.Contains(dgResult.Columns[e.ColumnIndex].Name) == false)
                {
                    changedColumns.Add(dgResult.Columns[e.ColumnIndex].Name, "");
                }
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 取得指定資料列的key值
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        private string getCurrentRowKey(int rowIndex)
        {
            string[] pks = getPKColumns();

            string pk = "";
            for (int i = 0; i < pks.Length; i++)
            {
                for (int j = 0; j < currentDataGrid.Columns.Count; j++)
                {
                    if (currentDataGrid.Columns[j].Name == pks[i])
                    {
                        pk += currentDataGrid.Rows[rowIndex].Cells[j].Value.ToString() + "§";
                    }
                }
            }

            return pk.TrimEnd('§');
        }

        private string[] pkColumns = null;

        /// <summary>
        /// 取得目前table的PK欄位名稱
        /// </summary>
        /// <returns></returns>
        private string[] getPKColumns()
        {
            if (pkColumns != null && pkColumns.Length != 0)
            {
                return this.pkColumns;
            }

            string table = this.currentTableName; // getTableName(null);

            if (table == "" || table.Contains("(") || table.Contains("'"))
            {
                return new string[0];
            }

            DBConn engine = getConnection(this.ddlConn.SelectedValue.ToString()) as DBConn;
            if (engine == null)
            {
                MessageBox.Show("無法取得連線");
                return new string[0];
            }

            string sql = "use [" + this.ddlDB.Text + "] select c.COLUMN_NAME " +
             "from 	INFORMATION_SCHEMA.TABLE_CONSTRAINTS pk , " +
             "INFORMATION_SCHEMA.KEY_COLUMN_USAGE c " +
             "where 	pk.TABLE_NAME = '" + table + "' " +
             "and	CONSTRAINT_TYPE = 'PRIMARY KEY' " +
             "and	c.TABLE_NAME = pk.TABLE_NAME " +
             "and	c.CONSTRAINT_NAME = pk.CONSTRAINT_NAME ";

            DataTable dt = engine.getData(sql);

            if (dt.Rows.Count > 0)
            {
                this.pkColumns = new string[dt.Rows.Count];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    this.pkColumns[i] = dt.Rows[i][0].ToString();
                }
            }
            else
            {
                CopyData dialog = new CopyData();
                dialog.Text = "指定PK";
                dialog.Desc = "PK欄位(逗號隔開)";

                dialog.ShowDialog();

                if (dialog._tablename.Equals(""))
                {
                    this.pkColumns = new string[0];
                }
                else
                {
                    this.pkColumns = dialog._tablename.Split(',');
                }
            }

            return this.pkColumns;
        }

        private void btnUpdateChange_Click(object sender, EventArgs e)
        {
        }

        private string compileUpdateSql()
        {
            IDictionaryEnumerator ide = this.changedList.GetEnumerator();

            string setStatement, whereStatement;
            string updateStatement = "";
            while (ide.MoveNext())
            {
                string key = (string)ide.Key;
                Hashtable changedColumns = (Hashtable)ide.Value;

                // get where statement
                string[] pks = getPKColumns();
                string[] pkValues = key.Split('§');

                whereStatement = "";
                for (int i = 0; i < pks.Length; i++)
                {
                    whereStatement += pks[i] + " = '" + pkValues[i].Replace("'", "''") + "' and ";
                }
                whereStatement = whereStatement.TrimEnd("and ".ToCharArray());

                // get set statement
                IDictionaryEnumerator cols = changedColumns.GetEnumerator();
                setStatement = "";
                while (cols.MoveNext())
                {
                    string colName = (string)cols.Key;
                    string colVal = getGridValue(key, colName);

                    setStatement += colName + " = " + colVal + ", ";
                }
                setStatement = setStatement.TrimEnd(", ".ToCharArray());

                // get update statement
                updateStatement += string.Format("update {0} set {1} where {2};\r\n", currentTableName, setStatement, whereStatement);
            }

            return updateStatement;
        }

        private string compileDeleteSql()
        {
            string whereStatement;
            string delStatement = "";

            // get where statement
            string[] pks = getPKColumns();
            string strPK = string.Join("§", pks);

            whereStatement = "";
            for (int i = 0; i < pks.Length; i++)
            {
                string value = getData(pks[i]);

                if (value == null)
                {
                    return "";
                }

                whereStatement += pks[i] + " = '" + value.Replace("'", "''") + "' and ";
            }
            whereStatement = whereStatement.TrimEnd("and ".ToCharArray());

            // get update statement
            delStatement += string.Format("delete {0} where {1};\r\n", currentTableName, whereStatement);

            return delStatement;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key">PK欄位對應的值</param>
        /// <param name="colname">回傳的欄位</param>
        /// <returns></returns>
        private string getGridValue(string key, string colname)
        {
            string[] keys = key.Split('§');
            string[] pkColumns = getPKColumns();

            if (pkColumns.Length == 0)
            {
                //GlobalClass.mylog((new Exception("找不到PK")).ToString());
                MessageBox.Show("找不到PK");
                return null;
            }

            string whereClause = "";
            for (int i = 0; i < keys.Length; i++)
            {
                whereClause += pkColumns[i] + "='" + keys[i] + "' and ";
            }

            whereClause = whereClause.TrimEnd("and ".ToCharArray());

            DataTable ds = (DataTable)this.currentDataGrid.DataSource;
            DataRow[] rows = ds.Select(whereClause);

            if (rows.Length != 1)
            {
                MessageBox.Show("計算有誤(" + whereClause + ")");
                return null;
            }

            ExportedValue val = exportValue(rows[0][colname], ds.Columns[colname].DataType);
            return val.value;
        }

        private void dgResultUpdate(object sender, EventArgs e)
        {
            try
            {
                string sql = compileUpdateSql();

                DialogResult r = MessageBox.Show(sql + "\r\n更新已修改的資料列，直接更新請按[是]，複製到剪貼簿請按[否]", "Confirm", MessageBoxButtons.YesNoCancel);

                if (r == DialogResult.Yes)
                {
                    DBConn engine = getConnection(this.ddlConn.SelectedValue.ToString()) as DBConn;

                    executeSql(engine, applyUsedDatabase(sql));
                }

                if (r == DialogResult.No)
                {
                    Clipboard.Clear();

                    if (sql.Length != 0)
                    {
                        Clipboard.SetText(sql, TextDataFormat.UnicodeText);
                    }
                    MessageBox.Show("已複製到剪貼簿");
                }

                resetChangeList();
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
                MessageBox.Show(ex.ToString());
            }
        }

        private void dgResultDelete(object sender, EventArgs e)
        {
            string sql = compileDeleteSql();

            DialogResult r = MessageBox.Show(sql + "\r\n直接刪除請按[是]，複製到剪貼簿請按[否]", "Confirm", MessageBoxButtons.YesNoCancel);

            if (r == DialogResult.Yes)
            {
                DBConn engine = getConnection(this.ddlConn.SelectedValue.ToString()) as DBConn;

                executeSql(engine, applyUsedDatabase(sql));
            }

            if (r == DialogResult.No)
            {
                Clipboard.Clear();

                if (sql.Length != 0)
                {
                    Clipboard.SetText(sql, TextDataFormat.UnicodeText);
                }
                MessageBox.Show("已複製到剪貼簿");
            }
        }

        /// <summary>
        /// 清除修改記錄，已記錄的PK欄位也清除
        /// </summary>
        private void resetChangeList()
        {
            this.changedList.Clear();
            this.pkColumns = null;
        }

        private void edSelectedContent_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (fireSelectedContentTextChanged == false)
                {
                    return;
                }

                if (_selectedCol >= this.currentDataGrid.Columns.Count || _selectedCol < 0 ||
                    _selectedRow < 0)  // less than 0 when selecting a row
                {
                    return;
                }

                this.currentDataGrid.Rows[_selectedRow].Cells[_selectedCol].Value = this.edSelectedContent.Text;
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
                MessageBox.Show(ex.ToString());
            }
        }

        private void cmCodeAssist_Click(object sender, EventArgs e)
        {
        }

        private void cmCodeAssist_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //this.redSql.SelectedText = e.ClickedItem.Text;
        }

        private void listCodeAssist_Click(object sender, EventArgs e)
        {
            codeCompleting();
        }

        private void codeCompleting()
        {
            if (this.listCodeAssist.SelectedItem == null)
            {
                return;
            }

            int curPos = this.icsSqlEditor.ActiveTextAreaControl.Caret.Offset;
            string text = this.icsSqlEditor.Text.Substring(0, curPos).ToLower();

            Match m = Regex.Match(text, "\\s[\\._0-9a-z]*\\z", RegexOptions.Singleline);

            if (m.Success)
            {
                int dotidx = m.Value.IndexOf(".");
                if (dotidx >= 0)
                {
                    int newidx = m.Value.IndexOf(".", dotidx + 1);
                    if (newidx >= 0)
                    {
                        dotidx = newidx;
                    }
                }
                else
                {
                    dotidx = 0;
                }

                string item = this.listCodeAssist.SelectedItem.ToString();

                // 選取已輸入的文字，取代成新的
                ICSharpCode.TextEditor.TextLocation loc1 = this.icsSqlEditor.Document.OffsetToPosition(m.Index + 1 + dotidx);
                ICSharpCode.TextEditor.TextLocation loc2 = this.icsSqlEditor.Document.OffsetToPosition(curPos);
                this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.SetSelection(loc1, loc2);

                updateFavoriteObject(item);
                pasteText(item + " ");  // 加空白，停止code assist
                this.icsSqlEditor.Focus();
            }

            this.listCodeAssist.Hide();
        }

        public void updateFavoriteObject(string item)
        {
            try
            {
                SqliteConn slite = new SqliteConn();
                string sql = string.Format("update TableInfo set Favorite = '1' where TableName = '{0}'", item);

                slite.executeSQL(sql);
            }
            catch (Exception e)
            {
                GlobalClass.errorLog(e.ToString());
            }
        }

        private void listCodeAssist_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                codeCompleting();
            }

            if (e.KeyCode == Keys.Escape)
            {
                this.listCodeAssist.Hide();

                //int pos = this.redSql.SelectionStart;
                this.icsSqlEditor.Focus();
                //this.redSql.SelectionStart = pos;
            }
        }

        /// <summary>
        /// 選取整個文字，包含底線
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void redSql_DoubleClick(object sender, EventArgs e)
        {
        }

        private bool isCanceled = false;

        private void btnCancel_Click(object sender, EventArgs e)
        {
            isCanceled = true;
        }

        private void edTxtSearch_TextChanged(object sender, EventArgs e)
        {
        }

        private DataSet getTableInfo(string table, string db)
        {
            string sql = string.Format(@"use [{2}] select COLUMN_NAME,Remark,

                DATA_TYPE + case when CHARACTER_MAXIMUM_LENGTH is null and NUMERIC_PRECISION is not null then ' ('+convert(varchar(10),NUMERIC_PRECISION) + ','+convert(varchar(10),NUMERIC_SCALE) + ')'
                when CHARACTER_MAXIMUM_LENGTH is not null then ' ('+convert(varchar(10),CHARACTER_MAXIMUM_LENGTH) + ')'
                else ''
                end [Data Type],

                case when p.is_identity=1 then 'IDENTITY' else '' end [Identity],

                case when IS_NULLABLE = 'YES' then 'Y' else '' end [Nullable],
                COLUMN_DEFAULT as [Default]
                from
                (select COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,
                NUMERIC_PRECISION,NUMERIC_SCALE,IS_NULLABLE,COLUMN_DEFAULT, TABLE_CATALOG, ORDINAL_POSITION
                from INFORMATION_SCHEMA.COLUMNS icols with(nolock)
                where icols.TABLE_NAME = '{0}' and TABLE_CATALOG='{1}'
                ) s
                left join
                (SELECT ep.value remark, c.name colname, c.is_identity
                FROM sys.columns AS c with(nolock)
                left JOIN sys.all_objects O with(nolock) ON c.object_id = O.object_id
                left join sys.extended_properties EP with(nolock) ON ep.major_id = c.object_id AND ep.minor_id = c.column_id
                left JOIN sys.schemas S with(nolock) on O.schema_id = S.schema_id
                where O.name = '{0}') p on s.COLUMN_NAME = p.colname

                union all

                SELECT '<<資料表>>' as [COLUMN_NAME], ep.value, '', '', '', ''
                    FROM sys.extended_properties EP with(nolock)
                    INNER JOIN sys.all_objects O with(nolock) ON ep.major_id = O.object_id
                    INNER JOIN sys.schemas S with(nolock) on O.schema_id = S.schema_id
                    left JOIN sys.columns AS c with(nolock) ON ep.major_id = c.object_id AND ep.minor_id = c.column_id
                    left join INFORMATION_SCHEMA.COLUMNS icols with(nolock) on c.name = COLUMN_NAME
                    where O.name = '{0}' and c.name is null
               order by COLUMN_NAME
               ", table, db, db);

            try
            {
                DataSet ds = runSqlByStatement(InqPage.DS_TYPE_QUERY, sql, this.ddlConn.SelectedValue.ToString(), false);
                return ds;
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// 取得table欄位資訊
        /// </summary>
        private void info_Click(object sender, EventArgs e)
        {           
            try
            {
                DataSet ds = getTableInfo(this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.SelectedText.Trim(), this.ddlDB.Text);                                        
                updateQueryResult(ds, "");
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
                throw ex;
            }
        }

        private void edTxtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //searchText(edTxtSearch.Text, true);
            }
        }

        private int _lastMouseClick = -1;

        private void redSql_MouseClick(object sender, MouseEventArgs e)
        {
            //_lastMouseClick = this.redSql.SelectionStart;
            //selectWord("[\\w-\\/]");
        }

        private void redSql_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                // 客製選取範圍
                //selectWord("[\\w-\\/]");

                //if (this.redSql.SelectionLength > 0)
                //{
                //    int selStart = this.redSql.SelectionStart;
                //    int sellen = this.redSql.SelectionLength;

                //    int cnt = searchText(this.redSql.SelectedText, false);

                //    this.redSql.SelectionStart = selStart;
                //    this.redSql.SelectionLength = sellen;

                //}
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
                MessageBox.Show(ex.ToString());
            }
        }

        private void selectWord(string pattern)
        {
            int idx = _lastMouseClick;
            int end = idx;

            if (this.icsSqlEditor.Text.Length == 0 || idx >= this.icsSqlEditor.Text.Length)
            {
                return;
            }

            while (idx >= 0)
            {
                Match m = Regex.Match(this.icsSqlEditor.Text.Substring(idx, 1), pattern, RegexOptions.IgnoreCase);

                if (m.Success == false)
                {
                    ++idx;
                    break;
                }

                --idx;
            }

            if (idx < 0)
            {
                idx = 0;
            }

            while (end < this.icsSqlEditor.Text.Length)
            {
                Match m = Regex.Match(this.icsSqlEditor.Text.Substring(end, 1), pattern, RegexOptions.IgnoreCase);

                if (m.Success == false)
                {
                    end--;
                    break;
                }

                ++end;
            }

            if (end >= this.icsSqlEditor.Text.Length)
            {
                end = this.icsSqlEditor.Text.Length - 1;
            }

            //this.redSql.Select(idx, end - idx + 1);
        }

        private void toolCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.SelectedText);
        }

        private void toolPaste_Click(object sender, EventArgs e)
        {
            //this.redSql.SelectedText = Clipboard.GetText();
        }

        //private void hideSelection2(bool hide)
        //{
        //    Message m = new Message();
        //    m.HWnd = this.redSql.Handle;
        //    m.Msg = 0x400 + 63;
        //    m.WParam = (IntPtr) (hide ? 1 : 0);
        //    m.LParam = (IntPtr) 0;

        //    DefWndProc(ref m);
        //}

        private void tabInquiry_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnTranslate.Visible = false;

            if (tabInquiry.SelectedIndex == 1)
            {
                currentDataGrid = this.dgResult;
                btnTranslate.Visible = true;
            }

            btnXmlFormate.Visible = false;

            if (tabInquiry.SelectedIndex == 2)
            {
                btnXmlFormate.Visible = true;
            }

            if (tabInquiry.SelectedIndex == 3)
            {
                currentDataGrid = this.dgResultOld;
                showLastResult();
            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
        }

        private void statusStrip1_Click(object sender, EventArgs e)
        {
            _selectedData = "";
            messageStatus("{@} 已清除");
        }

        private string getColumnsString(string tablename)
        {
            string sql = string.Format("select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS with(nolock)" +
             "where TABLE_NAME = '{0}' and TABLE_CATALOG='{1}' " +
             "order by ORDINAL_POSITION ", tablename, this.ddlDB.Text);

            sql = applyUsedDatabase(sql, this.ddlDB.Text);

            try
            {
                DataSet ds = runSqlByStatement(InqPage.DS_TYPE_QUERY, sql, this.ddlConn.SelectedValue.ToString(), false);

                string text = "";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    text += ds.Tables[0].Rows[i][0].ToString() + ",";

                    if (i != 0 && i % 5 == 0)
                    {
                        text += "\r\n";
                    }
                }

                return text.TrimEnd(',');
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
                throw ex;
            }
        }

        private void copyColToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //string text = getColumnsString(this.redSql.SelectedText.Trim());
                DBConn engine = getConnection();
                DataSet ds = engine.getColumns(this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.SelectedText.Trim(), this.ddlDB.Text);

                string text = "";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    text += ds.Tables[0].Rows[i][0].ToString() + ",";

                    if (i != 0 && i % 5 == 0)
                    {
                        text += "\r\n";
                    }
                }

                if (text.Equals("") == false)
                {
                    Clipboard.SetText(text.TrimEnd(','));
                    MessageBox.Show("複製完成");
                }
                else
                {
                    MessageBox.Show("無欄位可複製");
                }
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
                throw ex;
            }
        }

        private void totalCount_Click(object sender, EventArgs e)
        {
            string sql = string.Format("use [" + this.ddlDB.Text + "] select count(1) from {0} with(nolock) ",
                this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.SelectedText.Trim());

            try
            {
                DataSet ds = runSqlByStatement(InqPage.DS_TYPE_QUERY, sql, this.ddlConn.SelectedValue.ToString(), false);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    MessageBox.Show(GlobalClass.str(ds.Tables[0].Rows[0][0]));
                }
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
                throw ex;
            }
        }

        private int cnt1 = 0;

        private void tabInquiry_KeyUp(object sender, KeyEventArgs e)
        {
            _holdCtrl = e.Control;
            _holdShift = e.Shift;
            _holdAlt = e.Alt;
        }

        private void tabInquiry_KeyDown(object sender, KeyEventArgs e)
        {
            _holdCtrl = e.Control;
            _holdShift = e.Shift;
            _holdAlt = e.Alt;
        }

        private void dgResult_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
        }

        private void dgResult_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            //string name = this.currentDataGrid.Columns[e.Column.Index].Name;
            int idx = e.Column.Index;

            if (idx >= this.currentDataGrid.Columns.Count)
            {
                return;
            }

            if (saveColumnWidth == false)  // dataBinding 時不要記錄
            {
                return;
            }

            string dbname = this.ddlDB.Text;
            string table = this.currentTableName;
            string column = this.currentDataGrid.Columns[idx].Name;
            string width = this.currentDataGrid.Columns[idx].Width.ToString();

            SqliteConn acc = null;
            try
            {
                acc = new SqliteConn();

                string sqldel = string.Format("delete from ColumnSetting where dbName = '{0}' and tableName = '{1}' and col = '{2}' ",
                    dbname, table, column);
                acc.executeSQL(sqldel);
                acc.executeSQL(string.Format("delete from ColumnSetting where ModifiedDate < '{0}' or ModifiedDate is null", DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss")));

                string sqlins = string.Format("insert into ColumnSetting (dbName,tableName,col, width, ModifiedDate) values('{0}','{1}','{2}','{3}','{4}') ",
                    dbname, table, column, width, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                acc.executeSQL(sqlins);

                messageStatus(string.Format("{0}.{1} width = {2}", table, column, width));
            }
            catch (Exception ex)
            {
                GlobalClass.errorLog(ex.ToString());
            }
            finally
            {
                //if (acc != null)
                //{
                //    acc.close();
                //}
            }
        }

        private void pnlSql_Scroll(object sender, ScrollEventArgs e)
        {
        }

        private ArrayList analyzeSelectedLines()
        {
            ArrayList aryLines = new ArrayList();

            int leftIdx;

            if (this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.SelectedText.Length == 0)   //  沒有選取
            {
                int currentPosition = this.icsSqlEditor.ActiveTextAreaControl.Caret.Offset;

                leftIdx = findLineStart(this.icsSqlEditor.Text, currentPosition);

                if (leftIdx >= 0)
                {
                    aryLines.Add(leftIdx);
                }

                return aryLines;
            }

            // 有選取
            List<ICSharpCode.TextEditor.Document.ISelection> selection = this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.SelectionCollection;
            int selCount = selection.Count;

            int end = selection[selCount - 1].EndOffset;
            int firstLineStart = end - selection[0].Length;  //findLineStart(this.icsSqlEditor.Text, this.icsSqlEditor.ActiveTextAreaControl.Caret.Offset);

            leftIdx = findLineStart(this.icsSqlEditor.Text, end);
            while (leftIdx >= 0)
            {
                aryLines.Add(leftIdx);

                if (leftIdx == 0)
                {
                    break;
                }

                if (leftIdx < firstLineStart)
                {
                    break;
                }

                leftIdx = findLineStart(this.icsSqlEditor.Text, leftIdx - 1);
                
            }

            return aryLines;
        }

        private int findLineStart(string text, int idx)
        {
            int left = idx;

            --left;
            while (left >= 0)
            {
                if (text.Substring(left, 1).Equals("\n"))
                {
                    return left + 1;
                }

                --left;
            }

            return 0;
        }

        private int findLineEnd(string text, int idx)
        {
            int right = idx;

            ++right;
            while (right < text.Length)
            {
                if (text.Substring(right, 1).Equals("\n"))
                {
                    return right;
                }

                ++right;
            }

            return text.Length - 1;
        }

        private void btnMark_Click(object sender, EventArgs e)
        {
            int left = this.icsSqlEditor.ActiveTextAreaControl.Caret.Offset;
            string text = this.icsSqlEditor.Text;

            ArrayList lines = analyzeSelectedLines();
            lines.Sort();

            for (int i = lines.Count - 1; i >= 0; i--)
            {
                int leftIdx = (int)lines[i];  // 此行最左邊的index
                int idx = leftIdx;

                while (idx < text.Length)   // 從這行的頭找到尾
                {
                    string twoChars = getChar(text, idx) + getChar(text, idx + 1);

                    if (twoChars.Equals("--"))  // 已註記，移除
                    {
                        int lineEnd = findLineEnd(text, idx);
                        int pastedLen = lineEnd - (idx + 2) + 1;
                        string pastedText = text.Substring(idx + 2, pastedLen);

                        icsEditorSelect(idx, idx + pastedLen + 2);
                        //this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.RemoveSelectedText();

                        pasteText(pastedText);
                        break;
                    }
                    else if (text.Substring(idx, 1).Equals("\n") // 這行已掃完
                        || idx == 0                              // 這是第一行
                        || idx == text.Length - 1)               // 這是最後一行
                    {
                        // 沒有註記，加上註記

                        int lineEnd = findLineEnd(text, leftIdx);

                        if (lineEnd != -1)
                        {
                            int pastedLen = lineEnd - leftIdx + 1;
                            string pastedText = text.Substring(leftIdx, pastedLen);

                            if (pastedText.Trim().Equals(""))
                            {
                                break;
                            }

                            pastedText = "--" + pastedText;

                            icsEditorSelect(leftIdx, leftIdx + pastedLen);
                            //this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.RemoveSelectedText();

                            pasteText(pastedText);
                        }
                        break;
                    }

                    ++idx;
                }
            }
        }

        private string getChar(string str, int idx)
        {
            if (idx >= str.Length)
            {
                return "";
            }

            return str.Substring(idx, 1);
        }

        private void chkParentheses_Click(object sender, EventArgs e)
        {
            //if (chkParentheses.Checked == false)
            //{
            //    int start = this.redSql.SelectionStart;
            //    recoverLastParentheses();
            //    this.redSql.SelectionStart = start;
            //    this.redSql.SelectionLength = 0;
            //}
        }

        private Point _lastPosition;

        private void pnlSql_Leave(object sender, EventArgs e)
        {
            //_lastPosition = this.pnlSql.AutoScrollPosition;
        }

        private void pnlSql_Enter(object sender, EventArgs e)
        {
            //this.pnlSql.AutoScrollPosition = _lastPosition;
        }

        private void edFormName_Click(object sender, EventArgs e)
        {
            //this.pnlSql.Focus();
        }

        /// <summary>
        /// 拉動捲軸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void pnlBar_Scroll(object sender, ScrollEventArgs e)
        //{
        //    int delta = e.NewValue - e.OldValue;

        //    if (delta == 0)
        //    {
        //        return;
        //    }

        //    this.redSql.Top -= delta;

        //    checkRedSqlTop();
        //}

        //private void checkRedSqlTop()
        //{
        //    int top = 0;
        //    if (this.redSql.Top > top)
        //    {
        //        this.redSql.Top = top;
        //    }
        //}

        private void deleteSelected_Click(object sender, EventArgs e)
        {
            string sql = this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.SelectedText.Trim();

            if (sql.Equals(""))
            {
                MessageBox.Show("請選取SQL片段");
                return;
            }

            sql = applyUsedDatabase("delete " + sql);
            sql = compileSql(sql);

            DBConn engine = getConnection() as DBConn;

            executeSql(engine, sql);
        }

        /// <summary>
        /// 產生資料匯出語法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportSql_Click(object sender, EventArgs e)
        {
            string tablename = this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.SelectedText.Trim();

            if (tablename.Equals(""))
            {
                MessageBox.Show("請選取Table");
                return;
            }

            DBConn engine = getConnection();
            DataSet ds = engine.getColumns(tablename, this.ddlDB.Text);

            if (ds.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("無欄位資訊");
                return;
            }

            // 選擇key值欄位
            SelectKeys selectkeyDialogue = new SelectKeys();
            selectkeyDialogue.datasource = ds.Tables[0];
            selectkeyDialogue.ShowDialog();

            StringBuilder insSql = new StringBuilder();
            StringBuilder values = new StringBuilder();
            StringBuilder delSql = new StringBuilder();
            insSql.AppendFormat("'Insert into {0} (", tablename);
            delSql.AppendFormat("'Delete {0} where ", tablename);

            for (int i = 0; i < selectkeyDialogue.selectedCols.Length; i++)
            {
                if (i != 0)
                {
                    delSql.Append(" and ");
                }

                delSql.AppendFormat("{0} = '''+convert(nvarchar(max),{0})+''' ", selectkeyDialogue.selectedCols[i]);
            }
            delSql.Append("; '");

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                // column name
                string colName = ds.Tables[0].Rows[i][0].ToString();
                insSql.Append(colName);

                // values
                values.AppendFormat("'+(case when {0} is null then 'NULL' else '''' + convert(nvarchar(max),{0}) + '''' end)+'", colName);

                if (i != ds.Tables[0].Rows.Count - 1)
                {
                    insSql.Append(",");
                    values.Append(",");
                }
            }

            insSql.Append(") values(");
            insSql.Append(values.ToString() + ")' ");
            insSql.AppendFormat("from {0} ", tablename);

            Clipboard.SetText("select " + delSql.ToString() + "+" + insSql.ToString());

            messageStatus("指令匯出完成");
        }

        private void translateToGridView(DataTable dt)
        {
            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("Column");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dtNew.Columns.Add("Row " + (i + 1).ToString());
            }

            for (int colidx = 0; colidx < dt.Columns.Count; colidx++)
            {
                DataRow row = dtNew.NewRow();

                row[0] = dt.Columns[colidx].Caption;
                for (int rowidx = 0; rowidx < dt.Rows.Count; rowidx++)
                {
                    object val = dt.Rows[rowidx][colidx];
                    if (val.GetType() == typeof(DateTime))
                    {
                        DateTime dtVal = (DateTime)val;
                        val = dtVal.ToString("yyyy/MM/dd HH:mm:ss.ffff");
                    }

                    row[rowidx + 1] = GlobalClass.str(val);
                }

                dtNew.Rows.Add(row);
            }

            dgResult.DataSource = dtNew;
            dgResult.Columns[0].Frozen = true;
            dgResult.Columns[0].DefaultCellStyle.BackColor = Color.PeachPuff;

            for (int i = 1; i < dgResult.Columns.Count; i++)
            {
                dgResult.Columns[i].Width = 200;
            }
        }

        private void dgResultTranslate(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dgResult.DataSource;

            if (dt.Rows.Count > 20)
            {
                MessageBox.Show("資料筆數需在20筆以內");
                return;
            }

            translateToGridView((DataTable)dgResult.DataSource);
        }

        private void dgResultStrip_Opened(object sender, EventArgs e)
        {
            messageStatus("轉置：將[行]與[列]對調顯示");
        }

        private void chkTranslate_Click(object sender, EventArgs e)
        {
            messageStatus("自動轉置：將[行]與[列]對調顯示");
        }

        private void btnXmlFormate_Click(object sender, EventArgs e)
        {
            fireSelectedContentTextChanged = false;

            string text = edSelectedContent.Text;
            text = text.Replace("<![CDATA[", "<CDATA_START />").Replace("]]>", "<CDATA_END />");
            text = GlobalClass.PrintXML(text);
            text = text.Replace("<CDATA_START />", "<![CDATA[").Replace("<CDATA_END />", "]]>");

            edSelectedContent.Text = text;

            fireSelectedContentTextChanged = true;
        }

        private void btnXmlFormate_MouseEnter(object sender, EventArgs e)
        {
            messageStatus("將內容格式化");
        }

        private void btnXmlFormate_MouseLeave(object sender, EventArgs e)
        {
            messageStatus("");
        }

        private void listCandidate_Click(object sender, EventArgs e)
        {
            if (listCandidate.SelectedItem == null)
            {
                return;
            }

            string selectedCol = listCandidate.SelectedItem.ToString().ToUpper();

            for (int i = 0; i < currentDataGrid.Columns.Count; i++)
            {
                currentDataGrid.Columns[i].DefaultCellStyle.BackColor = Color.White;
                if (currentDataGrid.Columns[i].Name.ToUpper().Equals(selectedCol))
                {
                    listCandidate.Visible = false;
                    jumpToGridColumn(currentDataGrid, i);
                }
            }

            _searchStr = "";
        }

        private void ddlDB_SelectionChangeCommitted(object sender, EventArgs e)
        {
            this._ddlDBChanged = true;
        }

        private bool _ddlConnChanged = false;

        private void ddlConn_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _ddlConnChanged = true;
        }

        private void ddlDB_DrawItem(object sender, DrawItemEventArgs e)
        {
            int a = 0;
        }

        private void ExpSP(object sender, EventArgs e)
        {
            string tableName = this.icsSqlEditor.ActiveTextAreaControl.SelectionManager.SelectedText.Trim();
            DataSet ds = getTableInfo(tableName, this.ddlDB.Text);
            DataTable dt = ds.Tables[0];

            StringBuilder sb = new StringBuilder();
            sb.Append(@"USE [ABC]
GO 


-- ========================================================
/*
Author:			
Create date:	
Description:	
Modified By:	Modification Date		Modification Description


測試語法: 
exec [dbo].[%%%%%]
*/
-- ========================================================
create PROCEDURE [dbo].[%%%%%]" + "\n");

            StringBuilder SpParams = new StringBuilder();
            StringBuilder localVariables = new StringBuilder();
            StringBuilder setVariables = new StringBuilder();
            StringBuilder uptVariables = new StringBuilder();
            StringBuilder insCols = new StringBuilder();
            StringBuilder insVals = new StringBuilder();
            
            foreach(DataRow dr in dt.Rows)
            {
                string colName = dr["COLUMN_NAME"].ToString();

                // 參數宣告
                if (colName.Equals("<<資料表>>"))
                {
                    continue;
                }
                
                string dataType = dr["Data Type"].ToString();
                if(dataType.StartsWith("int"))
                {
                    dataType = "int";
                }

                SpParams.AppendFormat("@{0} {1},\n", colName, dataType);

                // 內部變數
                localVariables.AppendFormat("@_{0} {1},\n", colName, dataType);

                // 指定內部變數
                setVariables.AppendFormat("@_{0} = @{0},\n", colName);

                // update
                uptVariables.AppendFormat("s.{0} = isnull(@_{0}, s.{0}),\n", colName);

                // insert
                insCols.AppendFormat("{0},", colName);
                insVals.AppendFormat("@_{0},", colName);
            }
            sb.Append(SpParams.ToString().TrimEnd(',', '\n') + "\n\n");

            sb.Append(@"AS
BEGIN TRY 
	BEGIN
	SET NOCOUNT ON;" + "\n");

            sb.Append("declare " + localVariables.ToString().TrimEnd(',','\n') + "\n\n");
            sb.Append("select " + setVariables.ToString().TrimEnd(',', '\n') + "\n\n");

            sb.Append(@"if @_xxxxx is not null
    begin
	        update s
		    set " + uptVariables.ToString() + "\n");

            sb.AppendFormat(@"from dbo.{0} as s with(nolock)
	    where xxxxx = @_xxxxx
	end
	else
	begin
		insert into dbo.{0} (", tableName);

            sb.Append(insCols.ToString().TrimEnd(',', '\n'));
            sb.Append(@")
                values (" + insVals.ToString().TrimEnd(',', '\n') + ")");

            sb.AppendLine();
            sb.Append(@"	end
	
    END
    END TRY 
    BEGIN CATCH 
	    EXEC dbo.uspRaiseError;
    END CATCH 	
    GO

    GRANT EXECUTE ON [dbo].[%%%%%] TO [ABC]
    GO");

            Clipboard.SetText(sb.ToString());
            MessageBox.Show("OK");
        }
    }
}