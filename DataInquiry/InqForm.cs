using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DataInquiry.Assistant.Data;
using DataInquiry.Assistant.Assistant;

namespace DataInquiry.Assistant
{
    public partial class InqForm : Form
    {
        public InqForm()
        {
            InitializeComponent();

            this.Text = "Data Inquiry " + 
            System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion.Substring(0,7);
        }

        
        public void setData(string id, string group, string name, string content, string shortKey)
        {
            TabPage tp = new TabPage();
            InqPage newInq = new InqPage();
            newInq.setData(id, group, name, content, shortKey, this, tp);

            tp.Controls.Add(newInq);            

            this.tabInqs.TabPages.Add(tp);
        }

        public TabPage newInqPage(InqPage inq)
        {
            TabPage tp = new TabPage();
            tp.Controls.Add(inq);

            this.tabInqs.TabPages.Add(tp);
            this.tabInqs.SelectTab(tp);

            return tp;
        }

        private void InqForm_Resize(object sender, EventArgs e)
        {
            this.tabInqs.Width = this.Width - 40;
            this.tabInqs.Height = this.Height - 60;
        }

        public void tabInqs_Resize(object sender, EventArgs e)
        {
            for (int i = 0; i < this.tabInqs.TabPages.Count; i++)
            {
                TabPage tp = this.tabInqs.TabPages[i];
                tp.Width = this.Width - 20;
                tp.Height = this.Height - 70;

                tp.Controls[0].Width = this.Width - 55;
                tp.Controls[0].Height = this.Height - 90;
            }
        }

        private void tabInqs_ContextMenuStripChanged(object sender, EventArgs e)
        {
            
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            CodeRefresh.close();

            if (GlobalClass.dblist == null)
            {
                return;
            }          

            GlobalClass.closeAllConnection();
        }

        public void closeTab(TabPage value)
        {
            this.tabInqs.TabPages.Remove(value);
        }

        public TabControl getTabInq()
        {
            return this.tabInqs;
        }

        private void tabInqs_KeyUp(object sender, KeyEventArgs e)
        {
            InqPage inq = (InqPage)this.tabInqs.SelectedTab.Controls[0];
            inq.InqPageKeyUp(sender, e);
        }
    }
}