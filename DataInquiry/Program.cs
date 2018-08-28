using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DataInquiry.Assistant
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            InqForm form = new InqForm();
            form.setData("", "", "", "", "");
            Application.Run(form);
        }
    }
}