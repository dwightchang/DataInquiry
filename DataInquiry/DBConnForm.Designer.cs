namespace DataInquiry.Assistant
{
    partial class DBConnForm
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

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.dgConn = new System.Windows.Forms.DataGridView();
            this.dbcDB = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dbcName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dbcHost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dbcAccount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dbcPassword = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgConn)).BeginInit();
            this.SuspendLayout();
            // 
            // dgConn
            // 
            this.dgConn.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgConn.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dbcDB,
            this.dbcName,
            this.dbcHost,
            this.dbcAccount,
            this.dbcPassword});
            this.dgConn.Location = new System.Drawing.Point(12, 12);
            this.dgConn.Name = "dgConn";
            this.dgConn.RowTemplate.Height = 24;
            this.dgConn.Size = new System.Drawing.Size(555, 242);
            this.dgConn.TabIndex = 8;
            // 
            // dbcDB
            // 
            this.dbcDB.DataPropertyName = "dbcDB";
            this.dbcDB.HeaderText = "資料庫類型";
            this.dbcDB.Items.AddRange(new object[] {
            "SQL",
            "Oracle"});
            this.dbcDB.Name = "dbcDB";
            // 
            // dbcName
            // 
            this.dbcName.DataPropertyName = "dbcName";
            this.dbcName.HeaderText = "連線名稱";
            this.dbcName.Name = "dbcName";
            // 
            // dbcHost
            // 
            this.dbcHost.DataPropertyName = "dbcHost";
            this.dbcHost.HeaderText = "Host";
            this.dbcHost.Name = "dbcHost";
            // 
            // dbcAccount
            // 
            this.dbcAccount.DataPropertyName = "dbcAccount";
            this.dbcAccount.HeaderText = "帳號";
            this.dbcAccount.Name = "dbcAccount";
            // 
            // dbcPassword
            // 
            this.dbcPassword.DataPropertyName = "dbcPassword";
            this.dbcPassword.HeaderText = "密碼";
            this.dbcPassword.Name = "dbcPassword";
            // 
            // DBConnForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 266);
            this.Controls.Add(this.dgConn);
            this.Name = "DBConnForm";
            this.Text = "DBConn";
            this.Load += new System.EventHandler(this.DBConnForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DBConnForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgConn)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgConn;
        private System.Windows.Forms.DataGridViewComboBoxColumn dbcDB;
        private System.Windows.Forms.DataGridViewTextBoxColumn dbcName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dbcHost;
        private System.Windows.Forms.DataGridViewTextBoxColumn dbcAccount;
        private System.Windows.Forms.DataGridViewTextBoxColumn dbcPassword;
    }
}