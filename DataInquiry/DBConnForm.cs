using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DataInquiry.Assistant
{
    public partial class DBConnForm : Form
    {
        public DBConnForm()
        {
            InitializeComponent();
        }

        private void DBConnForm_Load(object sender, EventArgs e)
        {
            this.dgConn.DataSource = GlobalClass.connectionsData();
        }

        private void DBConnForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalClass.eventCenter.connChanged();
        }
    }
}