namespace DataInquiry.Assistant
{
    partial class CopyData
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
            this.edTableName = new System.Windows.Forms.TextBox();
            this.lblDesc = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // edTableName
            // 
            this.edTableName.Location = new System.Drawing.Point(15, 28);
            this.edTableName.Name = "edTableName";
            this.edTableName.Size = new System.Drawing.Size(257, 22);
            this.edTableName.TabIndex = 0;
            // 
            // lblDesc
            // 
            this.lblDesc.AutoSize = true;
            this.lblDesc.Location = new System.Drawing.Point(13, 13);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(59, 12);
            this.lblDesc.TabIndex = 1;
            this.lblDesc.Text = "Table name";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 56);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // CopyData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 95);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblDesc);
            this.Controls.Add(this.edTableName);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CopyData";
            this.Text = "CopyData";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox edTableName;
        private System.Windows.Forms.Label lblDesc;
        private System.Windows.Forms.Button button1;
    }
}