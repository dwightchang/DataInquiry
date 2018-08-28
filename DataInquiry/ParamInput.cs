using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DataInquiry.Assistant
{
    public partial class ParamInput : Form
    {
        public string paramValue = "";
        public string paramText = "";

        public ParamInput()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.lblParam.Text = this.paramText;
            base.OnLoad(e);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            paramValue = this.edParam.Text;
            this.Close();
        }

        private void ParamInput_KeyUp(object sender, KeyEventArgs e)
        {
            //btnOK_Click(null, null);
        }

        private void edParam_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK_Click(null, null);
            }
        }
    }
}