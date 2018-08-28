namespace DataInquiry.Assistant
{
    partial class Main
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
            this.components = new System.ComponentModel.Container();
            this.dgSavedInq = new System.Windows.Forms.DataGridView();
            this.btnNewInquiry = new System.Windows.Forms.Button();
            this.ddlGroup = new System.Windows.Forms.ComboBox();
            this.menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dgSavedInq)).BeginInit();
            this.menu.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgSavedInq
            // 
            this.dgSavedInq.AllowUserToAddRows = false;
            this.dgSavedInq.AllowUserToDeleteRows = false;
            this.dgSavedInq.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgSavedInq.Location = new System.Drawing.Point(12, 41);
            this.dgSavedInq.Name = "dgSavedInq";
            this.dgSavedInq.RowTemplate.Height = 24;
            this.dgSavedInq.Size = new System.Drawing.Size(577, 281);
            this.dgSavedInq.TabIndex = 1;
            this.dgSavedInq.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgSavedInq_CellLeave);
            this.dgSavedInq.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgSavedInq_CellDoubleClick);
            this.dgSavedInq.RowContextMenuStripNeeded += new System.Windows.Forms.DataGridViewRowContextMenuStripNeededEventHandler(this.dgSavedInq_RowContextMenuStripNeeded);
            this.dgSavedInq.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgSavedInq_CellLeave);
            // 
            // btnNewInquiry
            // 
            this.btnNewInquiry.Location = new System.Drawing.Point(190, 10);
            this.btnNewInquiry.Name = "btnNewInquiry";
            this.btnNewInquiry.Size = new System.Drawing.Size(75, 23);
            this.btnNewInquiry.TabIndex = 0;
            this.btnNewInquiry.Text = "新查詢";
            this.btnNewInquiry.UseVisualStyleBackColor = true;
            this.btnNewInquiry.Visible = false;
            this.btnNewInquiry.Click += new System.EventHandler(this.newInquiry_Click);
            // 
            // ddlGroup
            // 
            this.ddlGroup.FormattingEnabled = true;
            this.ddlGroup.Location = new System.Drawing.Point(45, 12);
            this.ddlGroup.Name = "ddlGroup";
            this.ddlGroup.Size = new System.Drawing.Size(121, 20);
            this.ddlGroup.TabIndex = 2;
            this.ddlGroup.SelectedIndexChanged += new System.EventHandler(this.ddlGroup_SelectedIndexChanged);
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(114, 26);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "群組";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 337);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(601, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(129, 17);
            this.toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(601, 359);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnNewInquiry);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.ddlGroup);
            this.Controls.Add(this.dgSavedInq);
            this.HelpButton = true;
            this.Name = "Main";
            this.Text = "DataInquiry";
            this.Resize += new System.EventHandler(this.Main_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dgSavedInq)).EndInit();
            this.menu.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnNewInquiry;
        private System.Windows.Forms.DataGridView dgSavedInq;
        private System.Windows.Forms.ComboBox ddlGroup;
        private System.Windows.Forms.ContextMenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
    }
}

