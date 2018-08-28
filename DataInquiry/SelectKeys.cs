using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DataInquiry.Assistant
{
    public partial class SelectKeys : Form
    {
        public DataTable datasource;
        public string[] selectedCols;

        public SelectKeys()
        {
            InitializeComponent();
        }
        

        protected override void OnLoad(EventArgs e)
        {
            this.dgColumns.DataSource = datasource;
            this.dgColumns.Update();

            
            base.OnLoad(e);
        }

        protected override void OnShown(EventArgs e)
        {
            //this.dgColumns.DataSource = datasource; 

            base.OnShown(e);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rows = this.dgColumns.SelectedRows;

            if (rows.Count == 0)
            {
                MessageBox.Show("½Ð¿ï¾ÜkeyÄæ¦ì");
                return;
            }

            selectedCols = new string[rows.Count];

            for (int i = 0; i < rows.Count; i++)
            {
                selectedCols[i] = datasource.Rows[rows[i].Index][0].ToString();
            }

            this.Close();
        }
    }
}