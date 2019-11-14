namespace DataInquiry.Assistant
{
    partial class InqPage
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該公開 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InqPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.ddlConn = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ddlDB = new System.Windows.Forms.ComboBox();
            this.btnNewInquiry = new System.Windows.Forms.Button();
            this.tabInquiry = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.chkTransaction = new System.Windows.Forms.CheckBox();
            this.listCodeAssist = new System.Windows.Forms.ListBox();
            this.icsSqlEditor = new ICSharpCode.TextEditor.TextEditorControl();
            this.editMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.formatItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tocodeItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toSql = new System.Windows.Forms.ToolStripMenuItem();
            this.info = new System.Windows.Forms.ToolStripMenuItem();
            this.toolCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.複製欄位名稱ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.totalCount = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.產生SPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnLoadSql = new System.Windows.Forms.Button();
            this.ddlGroup = new System.Windows.Forms.ComboBox();
            this.edShortKey = new System.Windows.Forms.TextBox();
            this.edTxtSearch = new System.Windows.Forms.TextBox();
            this.btnRangeMark = new System.Windows.Forms.Button();
            this.btnMark = new System.Windows.Forms.Button();
            this.edFormName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblFormName = new System.Windows.Forms.Label();
            this.lblGroup = new System.Windows.Forms.Label();
            this.btnSaveSql = new System.Windows.Forms.Button();
            this.btnFormat = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dgResultStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.更新ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.刪除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listCandidate = new System.Windows.Forms.ListBox();
            this.dgResult = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.edSelectedContent = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.dgResultOld = new System.Windows.Forms.DataGridView();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.edSqlMessage = new System.Windows.Forms.TextBox();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.btnCancel = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnCloseTab = new System.Windows.Forms.Button();
            this.fileDialog = new System.Windows.Forms.SaveFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.lblConnCnt = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnXmlFormate = new System.Windows.Forms.Button();
            this.btnTranslate = new System.Windows.Forms.Button();
            this.txtSpName = new System.Windows.Forms.TextBox();
            this.btnSpQuery = new System.Windows.Forms.Button();
            this.selectFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.tabInquiry.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.editMenu.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.dgResultStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgResult)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgResultOld)).BeginInit();
            this.tabPage5.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "使用連線";
            // 
            // ddlConn
            // 
            this.ddlConn.Font = new System.Drawing.Font("新細明體", 10F);
            this.ddlConn.FormattingEnabled = true;
            this.ddlConn.Location = new System.Drawing.Point(77, 9);
            this.ddlConn.Name = "ddlConn";
            this.ddlConn.Size = new System.Drawing.Size(168, 21);
            this.ddlConn.TabIndex = 4;
            this.ddlConn.SelectedIndexChanged += new System.EventHandler(this.ddlConn_SelectedIndexChanged);
            this.ddlConn.SelectionChangeCommitted += new System.EventHandler(this.ddlConn_SelectionChangeCommitted);
            this.ddlConn.SelectedValueChanged += new System.EventHandler(this.ddlConn_SelectedValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "資料庫";
            // 
            // ddlDB
            // 
            this.ddlDB.Font = new System.Drawing.Font("新細明體", 10F);
            this.ddlDB.FormattingEnabled = true;
            this.ddlDB.Location = new System.Drawing.Point(77, 36);
            this.ddlDB.MaxDropDownItems = 4;
            this.ddlDB.Name = "ddlDB";
            this.ddlDB.Size = new System.Drawing.Size(168, 21);
            this.ddlDB.TabIndex = 12;
            this.ddlDB.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ddlDB_DrawItem);
            this.ddlDB.SelectedIndexChanged += new System.EventHandler(this.ddlDB_SelectedIndexChanged);
            this.ddlDB.SelectionChangeCommitted += new System.EventHandler(this.ddlDB_SelectionChangeCommitted);
            this.ddlDB.Click += new System.EventHandler(this.ddlDB_Click);
            // 
            // btnNewInquiry
            // 
            this.btnNewInquiry.Location = new System.Drawing.Point(513, 7);
            this.btnNewInquiry.Name = "btnNewInquiry";
            this.btnNewInquiry.Size = new System.Drawing.Size(75, 23);
            this.btnNewInquiry.TabIndex = 14;
            this.btnNewInquiry.Text = "新查詢";
            this.btnNewInquiry.UseVisualStyleBackColor = true;
            this.btnNewInquiry.Click += new System.EventHandler(this.btnNewInquiry_Click);
            // 
            // tabInquiry
            // 
            this.tabInquiry.Controls.Add(this.tabPage1);
            this.tabInquiry.Controls.Add(this.tabPage2);
            this.tabInquiry.Controls.Add(this.tabPage3);
            this.tabInquiry.Controls.Add(this.tabPage4);
            this.tabInquiry.Controls.Add(this.tabPage5);
            this.tabInquiry.ImeMode = System.Windows.Forms.ImeMode.On;
            this.tabInquiry.Location = new System.Drawing.Point(9, 65);
            this.tabInquiry.Name = "tabInquiry";
            this.tabInquiry.SelectedIndex = 0;
            this.tabInquiry.Size = new System.Drawing.Size(985, 418);
            this.tabInquiry.TabIndex = 16;
            this.tabInquiry.SelectedIndexChanged += new System.EventHandler(this.tabInquiry_SelectedIndexChanged);
            this.tabInquiry.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabInquiry_Selected);
            this.tabInquiry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tabInquiry_KeyDown);
            this.tabInquiry.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tabInquiry_KeyUp);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.chkTransaction);
            this.tabPage1.Controls.Add(this.listCodeAssist);
            this.tabPage1.Controls.Add(this.icsSqlEditor);
            this.tabPage1.Controls.Add(this.btnLoadSql);
            this.tabPage1.Controls.Add(this.ddlGroup);
            this.tabPage1.Controls.Add(this.edShortKey);
            this.tabPage1.Controls.Add(this.edTxtSearch);
            this.tabPage1.Controls.Add(this.btnRangeMark);
            this.tabPage1.Controls.Add(this.btnMark);
            this.tabPage1.Controls.Add(this.edFormName);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.lblFormName);
            this.tabPage1.Controls.Add(this.lblGroup);
            this.tabPage1.Controls.Add(this.btnSaveSql);
            this.tabPage1.Controls.Add(this.btnFormat);
            this.tabPage1.ImeMode = System.Windows.Forms.ImeMode.On;
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(977, 392);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "SQL";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // chkTransaction
            // 
            this.chkTransaction.AutoSize = true;
            this.chkTransaction.Checked = true;
            this.chkTransaction.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTransaction.Location = new System.Drawing.Point(838, 7);
            this.chkTransaction.Name = "chkTransaction";
            this.chkTransaction.Size = new System.Drawing.Size(46, 16);
            this.chkTransaction.TabIndex = 24;
            this.chkTransaction.Text = "Tran";
            this.chkTransaction.UseVisualStyleBackColor = true;
            // 
            // listCodeAssist
            // 
            this.listCodeAssist.FormattingEnabled = true;
            this.listCodeAssist.ItemHeight = 12;
            this.listCodeAssist.Location = new System.Drawing.Point(455, 199);
            this.listCodeAssist.Name = "listCodeAssist";
            this.listCodeAssist.Size = new System.Drawing.Size(120, 88);
            this.listCodeAssist.TabIndex = 10;
            this.listCodeAssist.TabStop = false;
            this.listCodeAssist.Visible = false;
            this.listCodeAssist.Click += new System.EventHandler(this.listCodeAssist_Click);
            this.listCodeAssist.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listCodeAssist_KeyUp);
            // 
            // icsSqlEditor
            // 
            this.icsSqlEditor.BackColor = System.Drawing.Color.Transparent;
            this.icsSqlEditor.ContextMenuStrip = this.editMenu;
            this.icsSqlEditor.IsReadOnly = false;
            this.icsSqlEditor.Location = new System.Drawing.Point(10, 34);
            this.icsSqlEditor.Name = "icsSqlEditor";
            this.icsSqlEditor.ShowVRuler = false;
            this.icsSqlEditor.Size = new System.Drawing.Size(609, 319);
            this.icsSqlEditor.TabIndex = 2;
            this.icsSqlEditor.TextChanged += new System.EventHandler(this.TextArea_TextChanged);
            this.icsSqlEditor.Click += new System.EventHandler(this.redSql_Click);
            // 
            // editMenu
            // 
            this.editMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.formatItem,
            this.tocodeItem,
            this.toSql,
            this.info,
            this.toolCopy,
            this.toolPaste,
            this.複製欄位名稱ToolStripMenuItem,
            this.totalCount,
            this.deleteSelected,
            this.exportToolStripMenuItem,
            this.產生SPToolStripMenuItem});
            this.editMenu.Name = "editMenu";
            this.editMenu.Size = new System.Drawing.Size(186, 246);
            // 
            // formatItem
            // 
            this.formatItem.Name = "formatItem";
            this.formatItem.Size = new System.Drawing.Size(185, 22);
            this.formatItem.Text = "格式化";
            this.formatItem.Click += new System.EventHandler(this.btnFormat_Click);
            // 
            // tocodeItem
            // 
            this.tocodeItem.Name = "tocodeItem";
            this.tocodeItem.Size = new System.Drawing.Size(185, 22);
            this.tocodeItem.Text = "To Code";
            this.tocodeItem.Click += new System.EventHandler(this.tocodeItem_Click);
            // 
            // toSql
            // 
            this.toSql.Name = "toSql";
            this.toSql.Size = new System.Drawing.Size(185, 22);
            this.toSql.Text = "To SQL";
            this.toSql.Click += new System.EventHandler(this.toSql_Click);
            // 
            // info
            // 
            this.info.Name = "info";
            this.info.Size = new System.Drawing.Size(185, 22);
            this.info.Text = "查看資訊 (ctrl+K)";
            this.info.Click += new System.EventHandler(this.info_Click);
            // 
            // toolCopy
            // 
            this.toolCopy.Name = "toolCopy";
            this.toolCopy.Size = new System.Drawing.Size(185, 22);
            this.toolCopy.Text = "複製";
            this.toolCopy.Click += new System.EventHandler(this.toolCopy_Click);
            // 
            // toolPaste
            // 
            this.toolPaste.Name = "toolPaste";
            this.toolPaste.Size = new System.Drawing.Size(185, 22);
            this.toolPaste.Text = "貼上";
            this.toolPaste.Click += new System.EventHandler(this.toolPaste_Click);
            // 
            // 複製欄位名稱ToolStripMenuItem
            // 
            this.複製欄位名稱ToolStripMenuItem.Name = "複製欄位名稱ToolStripMenuItem";
            this.複製欄位名稱ToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.複製欄位名稱ToolStripMenuItem.Text = "複製欄位名稱";
            this.複製欄位名稱ToolStripMenuItem.Click += new System.EventHandler(this.copyColToolStripMenuItem_Click);
            // 
            // totalCount
            // 
            this.totalCount.Name = "totalCount";
            this.totalCount.Size = new System.Drawing.Size(185, 22);
            this.totalCount.Text = "資料筆數";
            this.totalCount.Click += new System.EventHandler(this.totalCount_Click);
            // 
            // deleteSelected
            // 
            this.deleteSelected.Name = "deleteSelected";
            this.deleteSelected.Size = new System.Drawing.Size(185, 22);
            this.deleteSelected.Text = "Delete + [Selected]";
            this.deleteSelected.Click += new System.EventHandler(this.deleteSelected_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.ExportSql_Click);
            // 
            // 產生SPToolStripMenuItem
            // 
            this.產生SPToolStripMenuItem.Name = "產生SPToolStripMenuItem";
            this.產生SPToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.產生SPToolStripMenuItem.Text = "產生SP";
            this.產生SPToolStripMenuItem.Click += new System.EventHandler(this.ExpSP);
            // 
            // btnLoadSql
            // 
            this.btnLoadSql.Location = new System.Drawing.Point(630, 4);
            this.btnLoadSql.Name = "btnLoadSql";
            this.btnLoadSql.Size = new System.Drawing.Size(75, 23);
            this.btnLoadSql.TabIndex = 8;
            this.btnLoadSql.Text = "選擇";
            this.btnLoadSql.UseVisualStyleBackColor = true;
            this.btnLoadSql.Click += new System.EventHandler(this.btnLoadSql_Click);
            // 
            // ddlGroup
            // 
            this.ddlGroup.FormattingEnabled = true;
            this.ddlGroup.Location = new System.Drawing.Point(290, 6);
            this.ddlGroup.Name = "ddlGroup";
            this.ddlGroup.Size = new System.Drawing.Size(86, 20);
            this.ddlGroup.TabIndex = 6;
            // 
            // edShortKey
            // 
            this.edShortKey.Location = new System.Drawing.Point(578, 5);
            this.edShortKey.Name = "edShortKey";
            this.edShortKey.Size = new System.Drawing.Size(41, 22);
            this.edShortKey.TabIndex = 5;
            this.edShortKey.TextChanged += new System.EventHandler(this.edFormName_TextChanged);
            // 
            // edTxtSearch
            // 
            this.edTxtSearch.Location = new System.Drawing.Point(743, 4);
            this.edTxtSearch.Name = "edTxtSearch";
            this.edTxtSearch.Size = new System.Drawing.Size(89, 22);
            this.edTxtSearch.TabIndex = 5;
            this.edTxtSearch.Visible = false;
            this.edTxtSearch.TextChanged += new System.EventHandler(this.edTxtSearch_TextChanged);
            this.edTxtSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.edTxtSearch_KeyUp);
            // 
            // btnRangeMark
            // 
            this.btnRangeMark.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRangeMark.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnRangeMark.Location = new System.Drawing.Point(185, 4);
            this.btnRangeMark.Name = "btnRangeMark";
            this.btnRangeMark.Size = new System.Drawing.Size(47, 23);
            this.btnRangeMark.TabIndex = 22;
            this.btnRangeMark.Text = "/**/";
            this.btnRangeMark.UseVisualStyleBackColor = true;
            this.btnRangeMark.Visible = false;
            // 
            // btnMark
            // 
            this.btnMark.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnMark.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnMark.Location = new System.Drawing.Point(137, 5);
            this.btnMark.Name = "btnMark";
            this.btnMark.Size = new System.Drawing.Size(42, 23);
            this.btnMark.TabIndex = 22;
            this.btnMark.Text = "--";
            this.btnMark.UseVisualStyleBackColor = true;
            this.btnMark.Click += new System.EventHandler(this.btnMark_Click);
            // 
            // edFormName
            // 
            this.edFormName.Location = new System.Drawing.Point(417, 5);
            this.edFormName.Name = "edFormName";
            this.edFormName.Size = new System.Drawing.Size(116, 22);
            this.edFormName.TabIndex = 5;
            this.edFormName.Click += new System.EventHandler(this.edFormName_Click);
            this.edFormName.TextChanged += new System.EventHandler(this.edFormName_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(711, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "搜尋";
            this.label5.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(546, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "熱鍵";
            // 
            // lblFormName
            // 
            this.lblFormName.AutoSize = true;
            this.lblFormName.Location = new System.Drawing.Point(383, 9);
            this.lblFormName.Name = "lblFormName";
            this.lblFormName.Size = new System.Drawing.Size(29, 12);
            this.lblFormName.TabIndex = 4;
            this.lblFormName.Text = "名稱";
            // 
            // lblGroup
            // 
            this.lblGroup.AutoSize = true;
            this.lblGroup.Location = new System.Drawing.Point(257, 10);
            this.lblGroup.Name = "lblGroup";
            this.lblGroup.Size = new System.Drawing.Size(29, 12);
            this.lblGroup.TabIndex = 4;
            this.lblGroup.Text = "群組";
            // 
            // btnSaveSql
            // 
            this.btnSaveSql.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveSql.Image")));
            this.btnSaveSql.Location = new System.Drawing.Point(87, 5);
            this.btnSaveSql.Name = "btnSaveSql";
            this.btnSaveSql.Size = new System.Drawing.Size(44, 23);
            this.btnSaveSql.TabIndex = 3;
            this.btnSaveSql.UseVisualStyleBackColor = true;
            this.btnSaveSql.Click += new System.EventHandler(this.btnSaveSql_Click);
            // 
            // btnFormat
            // 
            this.btnFormat.Location = new System.Drawing.Point(7, 5);
            this.btnFormat.Name = "btnFormat";
            this.btnFormat.Size = new System.Drawing.Size(75, 23);
            this.btnFormat.TabIndex = 2;
            this.btnFormat.Text = "格式化";
            this.btnFormat.UseVisualStyleBackColor = true;
            this.btnFormat.Click += new System.EventHandler(this.btnFormat_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.ContextMenuStrip = this.dgResultStrip;
            this.tabPage2.Controls.Add(this.listCandidate);
            this.tabPage2.Controls.Add(this.dgResult);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(977, 392);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "查詢結果";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Click += new System.EventHandler(this.tabPage2_Click);
            // 
            // dgResultStrip
            // 
            this.dgResultStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.更新ToolStripMenuItem,
            this.刪除ToolStripMenuItem});
            this.dgResultStrip.Name = "dgResultStrip";
            this.dgResultStrip.Size = new System.Drawing.Size(101, 48);
            this.dgResultStrip.Opened += new System.EventHandler(this.dgResultStrip_Opened);
            // 
            // 更新ToolStripMenuItem
            // 
            this.更新ToolStripMenuItem.Name = "更新ToolStripMenuItem";
            this.更新ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.更新ToolStripMenuItem.Text = "更新";
            this.更新ToolStripMenuItem.Click += new System.EventHandler(this.dgResultUpdate);
            // 
            // 刪除ToolStripMenuItem
            // 
            this.刪除ToolStripMenuItem.Name = "刪除ToolStripMenuItem";
            this.刪除ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.刪除ToolStripMenuItem.Text = "刪除";
            this.刪除ToolStripMenuItem.Click += new System.EventHandler(this.dgResultDelete);
            // 
            // listCandidate
            // 
            this.listCandidate.FormattingEnabled = true;
            this.listCandidate.ItemHeight = 12;
            this.listCandidate.Location = new System.Drawing.Point(118, 104);
            this.listCandidate.Name = "listCandidate";
            this.listCandidate.Size = new System.Drawing.Size(132, 124);
            this.listCandidate.TabIndex = 1;
            this.listCandidate.Visible = false;
            this.listCandidate.Click += new System.EventHandler(this.listCandidate_Click);
            // 
            // dgResult
            // 
            this.dgResult.AllowUserToAddRows = false;
            this.dgResult.AllowUserToDeleteRows = false;
            this.dgResult.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgResult.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgResult.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.dgResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Courier New", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgResult.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgResult.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.dgResult.Location = new System.Drawing.Point(6, 6);
            this.dgResult.MaximumSize = new System.Drawing.Size(710, 390);
            this.dgResult.MinimumSize = new System.Drawing.Size(0, 300);
            this.dgResult.Name = "dgResult";
            this.dgResult.RowTemplate.Height = 24;
            this.dgResult.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgResult.Size = new System.Drawing.Size(710, 380);
            this.dgResult.TabIndex = 0;
            this.dgResult.VirtualMode = true;
            this.dgResult.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgResult_CellClick);
            this.dgResult.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.dgResult_CellContextMenuStripNeeded);
            this.dgResult.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgResult_CellDoubleClick);
            this.dgResult.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgResult_CellValueChanged);
            this.dgResult.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgResult_CellValueNeeded);
            this.dgResult.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dgResult_ColumnWidthChanged);
            this.dgResult.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgResult_DataBindingComplete);
            this.dgResult.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgResult_RowEnter);
            this.dgResult.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgResult_KeyDown);
            this.dgResult.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgResult_KeyUp);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.edSelectedContent);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(977, 392);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "內容";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // edSelectedContent
            // 
            this.edSelectedContent.Font = new System.Drawing.Font("Courier New", 11F);
            this.edSelectedContent.Location = new System.Drawing.Point(4, 6);
            this.edSelectedContent.Multiline = true;
            this.edSelectedContent.Name = "edSelectedContent";
            this.edSelectedContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.edSelectedContent.Size = new System.Drawing.Size(715, 380);
            this.edSelectedContent.TabIndex = 4;
            this.edSelectedContent.Text = "Selected Content";
            this.edSelectedContent.TextChanged += new System.EventHandler(this.edSelectedContent_TextChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.AutoScroll = true;
            this.tabPage4.Controls.Add(this.dgResultOld);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(977, 392);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "上次結果";
            this.tabPage4.UseVisualStyleBackColor = true;
            this.tabPage4.Click += new System.EventHandler(this.tabPage4_Click);
            // 
            // dgResultOld
            // 
            this.dgResultOld.AllowUserToAddRows = false;
            this.dgResultOld.AllowUserToDeleteRows = false;
            this.dgResultOld.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgResultOld.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgResultOld.Location = new System.Drawing.Point(7, 3);
            this.dgResultOld.Name = "dgResultOld";
            this.dgResultOld.RowTemplate.Height = 24;
            this.dgResultOld.Size = new System.Drawing.Size(715, 386);
            this.dgResultOld.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.edSqlMessage);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(977, 392);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "訊息";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // edSqlMessage
            // 
            this.edSqlMessage.Font = new System.Drawing.Font("新細明體", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.edSqlMessage.Location = new System.Drawing.Point(1, 1);
            this.edSqlMessage.Multiline = true;
            this.edSqlMessage.Name = "edSqlMessage";
            this.edSqlMessage.Size = new System.Drawing.Size(545, 280);
            this.edSqlMessage.TabIndex = 0;
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(826, 43);
            this.progress.MarqueeAnimationSpeed = 20;
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(161, 23);
            this.progress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progress.TabIndex = 7;
            this.progress.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(595, 36);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(74, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(514, 36);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "執行";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 491);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1013, 22);
            this.statusStrip1.TabIndex = 18;
            this.statusStrip1.Text = "statusStrip1";
            this.statusStrip1.Click += new System.EventHandler(this.statusStrip1_Click);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnCloseTab
            // 
            this.btnCloseTab.Location = new System.Drawing.Point(595, 7);
            this.btnCloseTab.Name = "btnCloseTab";
            this.btnCloseTab.Size = new System.Drawing.Size(75, 23);
            this.btnCloseTab.TabIndex = 10;
            this.btnCloseTab.Text = "關閉";
            this.btnCloseTab.UseVisualStyleBackColor = true;
            this.btnCloseTab.Click += new System.EventHandler(this.btnCloseTab_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(612, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 12);
            this.label2.TabIndex = 20;
            // 
            // lblConnCnt
            // 
            this.lblConnCnt.AutoSize = true;
            this.lblConnCnt.Location = new System.Drawing.Point(770, 41);
            this.lblConnCnt.Name = "lblConnCnt";
            this.lblConnCnt.Size = new System.Drawing.Size(33, 12);
            this.lblConnCnt.TabIndex = 21;
            this.lblConnCnt.Text = "label5";
            this.lblConnCnt.Visible = false;
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(779, 11);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(24, 12);
            this.lblInfo.TabIndex = 22;
            this.lblInfo.Text = "info";
            // 
            // btnXmlFormate
            // 
            this.btnXmlFormate.Location = new System.Drawing.Point(678, 7);
            this.btnXmlFormate.Name = "btnXmlFormate";
            this.btnXmlFormate.Size = new System.Drawing.Size(75, 23);
            this.btnXmlFormate.TabIndex = 24;
            this.btnXmlFormate.Text = "XML";
            this.btnXmlFormate.UseVisualStyleBackColor = true;
            this.btnXmlFormate.Visible = false;
            this.btnXmlFormate.Click += new System.EventHandler(this.btnXmlFormate_Click);
            this.btnXmlFormate.MouseEnter += new System.EventHandler(this.btnXmlFormate_MouseEnter);
            this.btnXmlFormate.MouseLeave += new System.EventHandler(this.btnXmlFormate_MouseLeave);
            // 
            // btnTranslate
            // 
            this.btnTranslate.Location = new System.Drawing.Point(678, 36);
            this.btnTranslate.Name = "btnTranslate";
            this.btnTranslate.Size = new System.Drawing.Size(75, 23);
            this.btnTranslate.TabIndex = 25;
            this.btnTranslate.Text = "Grid轉置";
            this.btnTranslate.UseVisualStyleBackColor = true;
            this.btnTranslate.Visible = false;
            this.btnTranslate.Click += new System.EventHandler(this.dgResultTranslate);
            // 
            // txtSpName
            // 
            this.txtSpName.Location = new System.Drawing.Point(253, 36);
            this.txtSpName.Name = "txtSpName";
            this.txtSpName.Size = new System.Drawing.Size(136, 22);
            this.txtSpName.TabIndex = 26;
            // 
            // btnSpQuery
            // 
            this.btnSpQuery.Location = new System.Drawing.Point(395, 34);
            this.btnSpQuery.Name = "btnSpQuery";
            this.btnSpQuery.Size = new System.Drawing.Size(75, 23);
            this.btnSpQuery.TabIndex = 27;
            this.btnSpQuery.Text = "SP查詢";
            this.btnSpQuery.UseVisualStyleBackColor = true;
            this.btnSpQuery.Click += new System.EventHandler(this.btnSpQuery_Click);
            // 
            // selectFolder
            // 
            this.selectFolder.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // InqPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSpQuery);
            this.Controls.Add(this.txtSpName);
            this.Controls.Add(this.btnTranslate);
            this.Controls.Add(this.btnXmlFormate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnCloseTab);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblConnCnt);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.tabInquiry);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnNewInquiry);
            this.Controls.Add(this.ddlDB);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ddlConn);
            this.Controls.Add(this.label1);
            this.ImeMode = System.Windows.Forms.ImeMode.On;
            this.Name = "InqPage";
            this.Size = new System.Drawing.Size(1013, 513);
            this.Resize += new System.EventHandler(this.InqPage_Resize);
            this.tabInquiry.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.editMenu.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.dgResultStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgResult)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgResultOld)).EndInit();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ddlConn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox ddlDB;
        private System.Windows.Forms.Button btnNewInquiry;
        private System.Windows.Forms.TabControl tabInquiry;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.ComboBox ddlGroup;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox edFormName;
        private System.Windows.Forms.Label lblFormName;
        private System.Windows.Forms.Label lblGroup;
        private System.Windows.Forms.Button btnSaveSql;
        private System.Windows.Forms.Button btnFormat;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dgResult;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox edSelectedContent;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnLoadSql;
        private System.Windows.Forms.Button btnCloseTab;
        private System.Windows.Forms.ContextMenuStrip editMenu;
        private System.Windows.Forms.ToolStripMenuItem formatItem;
        private System.Windows.Forms.ToolStripMenuItem tocodeItem;
        private System.Windows.Forms.ToolStripMenuItem toSql;
        private System.Windows.Forms.SaveFileDialog fileDialog;
        private System.Windows.Forms.ContextMenuStrip dgResultStrip;
        private System.Windows.Forms.ToolStripMenuItem 更新ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 刪除ToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listCodeAssist;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.DataGridView dgResultOld;
        private System.Windows.Forms.TextBox edShortKey;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblConnCnt;
        private System.Windows.Forms.TextBox edTxtSearch;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolStripMenuItem info;
        private System.Windows.Forms.ToolStripMenuItem toolCopy;
        private System.Windows.Forms.ToolStripMenuItem toolPaste;
        private System.Windows.Forms.ToolStripMenuItem 複製欄位名稱ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem totalCount;
        private System.Windows.Forms.Button btnMark;
        private System.Windows.Forms.Button btnRangeMark;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.ToolStripMenuItem deleteSelected;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.Button btnXmlFormate;
        private System.Windows.Forms.Button btnTranslate;
        private System.Windows.Forms.ListBox listCandidate;
        private ICSharpCode.TextEditor.TextEditorControl icsSqlEditor;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TextBox edSqlMessage;
        private System.Windows.Forms.ToolStripMenuItem 產生SPToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkTransaction;
        private System.Windows.Forms.TextBox txtSpName;
        private System.Windows.Forms.Button btnSpQuery;
        private System.Windows.Forms.FolderBrowserDialog selectFolder;
    }
}
