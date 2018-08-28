using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace DataInquiry.Assistant
{
    class VisualTableManager
    {
        private Control _uiControl;
        private IMaster _master;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseUiControl">visualTable 執行的環境</param>
        /// <param name="master">資料提供者</param>
        public VisualTableManager(Control baseUiControl, IMaster master)
        {
            _uiControl = baseUiControl;
            _master = master;

            VisualTable box = makeVisualTable(master);
            box.Location = new Point(30, 30);            
        }

        private VisualTable makeVisualTable(IMaster master)
        {
            Control baseUiControl = master.getMasterUIControl();
            VisualTable box = new VisualTable("unknown", master);
            baseUiControl.Controls.Add(box);

            baseUiControl.MouseUp += new MouseEventHandler(baseUiControl_MouseUp);

            return box;
        }

        void baseUiControl_MouseUp(object sender, MouseEventArgs e)
        {
            VisualTable box = makeVisualTable(_master);
            box.Location = new Point(e.X, e.Y); 
        }       
    }
}
