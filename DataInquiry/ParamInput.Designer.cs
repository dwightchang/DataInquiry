namespace DataInquiry.Assistant
{
    partial class ParamInput
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
            this.lblParam = new System.Windows.Forms.Label();
            this.edParam = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblParam
            // 
            this.lblParam.AutoSize = true;
            this.lblParam.Location = new System.Drawing.Point(21, 20);
            this.lblParam.Name = "lblParam";
            this.lblParam.Size = new System.Drawing.Size(0, 12);
            this.lblParam.TabIndex = 0;
            // 
            // edParam
            // 
            this.edParam.Location = new System.Drawing.Point(23, 45);
            this.edParam.Name = "edParam";
            this.edParam.Size = new System.Drawing.Size(241, 22);
            this.edParam.TabIndex = 1;
            this.edParam.KeyUp += new System.Windows.Forms.KeyEventHandler(this.edParam_KeyUp);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(23, 77);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "確定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ParamInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 112);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.edParam);
            this.Controls.Add(this.lblParam);
            this.Name = "ParamInput";
            this.Text = "ParamInput";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ParamInput_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblParam;
        private System.Windows.Forms.TextBox edParam;
        private System.Windows.Forms.Button btnOK;
    }
}