using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataInquiry.Assistant
{
    public class ExComboBox : System.Windows.Forms.ComboBox
    {
        private void ExComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {            
            int index = e.Index >= 0 ? e.Index : 0;
            var brush = Brushes.Black;
            
            e.DrawBackground();
            e.Graphics.DrawString(this.Items[index].ToString(), e.Font, brush, e.Bounds, StringFormat.GenericDefault);
            e.DrawFocusRectangle();
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            int index = e.Index >= 0 ? e.Index : 0;
            var brush = Brushes.Black;

            e.DrawBackground();
            e.Graphics.DrawString(this.Items[index].ToString(), e.Font, brush, e.Bounds, StringFormat.GenericDefault);
            e.DrawFocusRectangle();
        }
    }
}
