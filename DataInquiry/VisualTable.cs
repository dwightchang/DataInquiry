using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Collections;

namespace DataInquiry.Assistant
{
    class VisualTable: GroupBox
    {
        private ListBox _tablenameAssist = new ListBox();
        private DataSet _tableViewList = null;
        private IMaster _masterUIControl = null;
        private TextBox _txtName = null;
        private Label _lblName = null;

        public string ObjectName
        {
            set
            {
                _lblName.Text = value;
                _txtName.Text = value;
            }
            get
            {
                return _lblName.Text;
            }
        }

        public VisualTable(string name, IMaster master)
        {            
            this.Text = "";
            this.Click += new EventHandler(VisualTable_Click);
            this.MouseDown += new MouseEventHandler(VisualTable_MouseDown);
            this.MouseUp += new MouseEventHandler(VisualTable_MouseUp);
            this.MouseMove += new MouseEventHandler(VisualTable_MouseMove);

            _masterUIControl = master;

            _lblName = new Label();            
            _lblName.Location = new Point(5, 10);
            _lblName.Click += new EventHandler(objName_Click);

            _txtName = new TextBox();
            _txtName.Location = new Point(5, 15);
            _txtName.Width = this.Width - 20;
            _txtName.Visible = false;
            _txtName.KeyUp += new KeyEventHandler(txtName_KeyUp);
            _txtName.TextChanged += new EventHandler(_txtName_TextChanged);

            this.ObjectName = name;

            _tablenameAssist.KeyUp += new KeyEventHandler(tablenameAssist_KeyUp);
            _tablenameAssist.Click += new EventHandler(_tablenameAssist_Click);
            _tablenameAssist.Hide();

            this.Controls.Add(_lblName);
            this.Controls.Add(_txtName);
            this._masterUIControl.getMasterUIControl().Controls.Add(_tablenameAssist);
        }        


        #region mouse event
        private bool _mouseDown = false;
        private int _oMousex, _oMousey;
        private int _oTablex, _oTabley;
        void VisualTable_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;
            _oMousex = e.X;
            _oMousey = e.Y;

            _oTablex = this.Left;
            _oTabley = this.Top;
        }
        void VisualTable_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }

        void VisualTable_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown == false)
            {
                return;
            }
           
            int deltax = e.X - _oMousex;
            int deltay = e.Y - _oMousey;            

            this.Left = _oTablex + deltax;
            this.Top = _oTabley + deltay;
          
            _oTablex = this.Left;
            _oTabley = this.Top;

            this.Update();
        }
        #endregion

        void VisualTable_Click(object sender, EventArgs e)
        {
            toLabel();
        }

        void _tablenameAssist_Click(object sender, EventArgs e)
        {
            codeCompleting();
        }        

        void tablenameAssist_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                codeCompleting();
            }

            if (e.KeyCode == Keys.Escape)
            {
                _tablenameAssist.Hide();
            }            
        }

        private void codeCompleting()
        {
            if (_tablenameAssist.SelectedItem == null)
            {
                return;
            }

            this.ObjectName = _tablenameAssist.SelectedItem.ToString();

            toLabel();            
        }

        private void toLabel()
        {            
            _txtName.Hide();
            _lblName.Show();

            if (_tablenameAssist.Visible)
            {
                _tablenameAssist.Hide();
            }
        }

        public DataSet TableViewList
        {
            set
            {
                _tableViewList = value;
            }
        }

        void txtName_KeyUp(object sender, KeyEventArgs e)
        {            

            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                if (_tablenameAssist.Visible)
                {
                    _tablenameAssist.Focus();
                    _tablenameAssist.SelectedIndex = 1;
                }
            }

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                if (_tablenameAssist.Visible)
                {
                    codeCompleting();
                }               
            }
        }

        void _txtName_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            _tablenameAssist.Items.Clear();
            tableViewAssistant(txt.Text, _tablenameAssist);

            if (_tablenameAssist.Items.Count > 0)
            {
                _tablenameAssist.Top = txt.Top + 50;
                _tablenameAssist.Left = _masterUIControl.getMasterUIControl().Left + this.Left + txt.Left;
                _tablenameAssist.SelectedIndex = 0;
                _tablenameAssist.Show();
            }
            else
            {
                _tablenameAssist.Hide();
            }
        }

        void objName_Click(object sender, EventArgs e)
        {
            //Label lbl = (Label)sender;
            //TextBox txt = (TextBox)lbl.Tag;

            _lblName.Visible = false;            
            _txtName.Visible = true;

            if (_txtName.Text.Equals("unknown") == false)
            {
                _txtName.Select(_txtName.Text.Length, 0);
            }

            _txtName.Focus();
        }

        private void tableViewAssistant(string name, ListBox box)
        {
            if (this._masterUIControl == null)
            {
                return;
            }

            _tableViewList = this._masterUIControl.getDataSet();

            if (_tableViewList == null)
            {                
                return;
            }

            name = name.ToLower();

            DataSet ds = _tableViewList;


            // for compute width
            int width = 100, itemWidth;
            Graphics g = box.CreateGraphics();

            #region tables and views
            ArrayList tablelist2 = new ArrayList();
            int count = 0;
            for (int i = 0; i < ds.Tables[0].Rows.Count && count <= 10; i++)
            {
                bool found = false;
                string table = ds.Tables[0].Rows[i]["TABLE_NAME"].ToString();

                if (table.ToLower().StartsWith(name))
                {
                    box.Items.Add(table);
                    found = true;
                }
                else if (table.ToLower().Contains(name))
                {
                    found = true;
                    tablelist2.Add(table);
                }


                if (found)
                {
                    ++count;

                    // computing width
                    itemWidth = Convert.ToInt32(g.MeasureString(table, box.Font).Width) + 30;
                    width = Math.Max(width, itemWidth);
                    box.Width = width;
                }
            }

            for (int i = 0; i < tablelist2.Count; i++)
            {
                box.Items.Add((string)tablelist2[i]);
            }

            #endregion            
        }
    }
}
