using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace DataInquiry.Assistant
{
    interface IMaster
    {
        DataSet getDataSet();
        Control getMasterUIControl();
        void runSql(string sql);
    }
}
