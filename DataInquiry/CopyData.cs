using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DataInquiry.Assistant
{
    public partial class CopyData : Form
    {
        public string _tablename = "";
        public bool OK = false;

        public CopyData()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.Left = Control.MousePosition.X;
            this.Top = Control.MousePosition.Y;
        }

        public void setTableName(string name)
        {
            this.edTableName.Text = name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _tablename = this.edTableName.Text;
            OK = true;
            this.Close();
        }

        public string Desc
        {
            set
            {
                this.lblDesc.Text = value;
            }
        }
    }
}