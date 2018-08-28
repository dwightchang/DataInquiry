using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using DataInquiry.Assistant.Data;

namespace DataInquiry.Assistant
{
    public partial class Main : Form
    {
        public string[] _selected = new string[5];

        public Main()
        {
            InitializeComponent();
        }

        private void newInquiry_Click(object sender, EventArgs e)
        {
            try
            {
                InqForm inq = new InqForm();
                inq.setData("", "", "", "", "");
                inq.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ArrayList groups = GlobalClass.getGroups();

            ddlGroup.Items.Clear();
            ddlGroup.Items.Add("");

            for (int i = 0; i < groups.Count; i++)
            {
                ddlGroup.Items.Add(groups[i].ToString());
            }           

            loadList();

            this.toolStripStatusLabel2.Text = "shortKey: Ctrl + Alt + [shortKey] 立即執行該指令";
        }

        private void loadList()
        {
            SqliteConn acc = new SqliteConn();

            try
            {
                Reader r = acc.getDataReader(
                    "select id, groupName, inqName, content, shortKey from Inquiry where parentId = '' and (groupName='" + ddlGroup.Text + "')  "); //  or groupName='' or groupName is null
                DataTable dt = new DataTable();
                dt.Columns.Add("id");
                dt.Columns.Add("Group");
                dt.Columns.Add("Name");
                dt.Columns.Add("SQL");
                dt.Columns.Add("shortKey");

                while (r.Read())
                {
                    DataRow row = dt.NewRow();
                    row["id"] = r[0].ToString();
                    row["Group"] = r[1].ToString();
                    row["Name"] = r[2].ToString();
                    row["SQL"] = r[3].ToString();
                    row["shortKey"] = r[4].ToString();

                    dt.Rows.Add(row);
                }

                this.dgSavedInq.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw ex;
            }
            finally
            {
                //acc.close();
            }
        }


        private void dgSavedInq_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataTable dt = this.dgSavedInq.DataSource as DataTable;

            if (dt == null)
            {
                return;
            }

            if (e.RowIndex > dt.Rows.Count - 1)
            {
                return;
            }

            int idx = e.RowIndex;

            _selected[0] = dt.Rows[idx][0].ToString();
            _selected[1] = dt.Rows[idx][1].ToString();
            _selected[2] = dt.Rows[idx][2].ToString();
            _selected[3] = dt.Rows[idx][3].ToString();
            _selected[4] = dt.Rows[idx][4].ToString();

            this.Close();
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            this.dgSavedInq.Width = this.Width - 40;
            this.dgSavedInq.Height = this.Height - 150;
        }

        public void listChanged()
        {
            loadList();
        }

        private void ddlGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadList();
        }

        private int selectedRows;
        private void dgSavedInq_RowContextMenuStripNeeded(object sender, DataGridViewRowContextMenuStripNeededEventArgs e)
        {
            selectedRows = e.RowIndex;
            
            this.menu.Show(Control.MousePosition.X, Control.MousePosition.Y);
        }

        private int _recursiveDepth;
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //DataTable dt = this.dgSavedInq.DataSource as DataTable;

            string name = this.dgSavedInq.Rows[selectedRows].Cells["Name"].Value.ToString();
            string id = this.dgSavedInq.Rows[selectedRows].Cells["id"].Value.ToString();


            DialogResult r = MessageBox.Show("Delete \"" + name + "\" ?", "Confirm", MessageBoxButtons.YesNo);

            SqliteConn db = null;

            try
            {
                if (r == DialogResult.Yes)
                {
                    db = new SqliteConn();

                    _recursiveDepth = 0;
                    deleteInq(db, id);                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw ex;
            }
            finally
            {
                //if (db != null)
                //{
                //    db.close();
                //}
            }

            loadList();
        }

        private void deleteInq(SqliteConn db, string id)
        {
            ++_recursiveDepth;
            if (_recursiveDepth > 100)
            {
                throw new Exception("recursive error");
            }

            Reader r = db.getDataReader("select id from Inquiry where parentId = '" + id + "'");

            while(r.Read())
            {
                deleteInq(db, r[0].ToString());
            }

            db.executeSQL("delete from Inquiry where id = "+id);
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            
        }

        private void ddlDBConn_SelectedIndexChanged(object sender, EventArgs e)
        {            
        }       

        private void chkSpecifiedConn_CheckedChanged(object sender, EventArgs e)
        {
            //if (chkSpecifiedConn.Checked)
            //{
            //    this.toolStripStatusLabel1.Text = "勾選[自訂]選項時，需在[使用連線]欄位輸入連線字串";
            //}
            //else
            //{
            //    this.toolStripStatusLabel1.Text = "";
            //}
        }

        private void dgSavedInq_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            this.toolStripStatusLabel1.Text = "按右鍵可刪除資料";
        }

        private void dgSavedInq_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            this.toolStripStatusLabel1.Text = "";
        }
    }
}