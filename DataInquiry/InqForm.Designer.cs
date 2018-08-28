namespace DataInquiry.Assistant
{
    partial class InqForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InqForm));
            this.tabInqs = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // tabInqs
            // 
            this.tabInqs.Location = new System.Drawing.Point(13, 13);
            this.tabInqs.Name = "tabInqs";
            this.tabInqs.SelectedIndex = 0;
            this.tabInqs.Size = new System.Drawing.Size(658, 386);
            this.tabInqs.TabIndex = 0;
            this.tabInqs.ContextMenuStripChanged += new System.EventHandler(this.tabInqs_ContextMenuStripChanged);
            this.tabInqs.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tabInqs_KeyUp);
            this.tabInqs.Resize += new System.EventHandler(this.tabInqs_Resize);
            // 
            // InqForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 411);
            this.Controls.Add(this.tabInqs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InqForm";
            this.Text = "InqForm 1.21";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Resize += new System.EventHandler(this.InqForm_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabInqs;
    }
}