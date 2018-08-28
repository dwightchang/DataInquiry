using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DataInquiry.Assistant
{
    public delegate void connChangedHandler(DataTable pConnections);

    class EventCenter
    {
        public event connChangedHandler connChangedEvent;

        /// <summary>
        /// �q���s�u��T�w����
        /// </summary>
        public void connChanged()
        {
            connChangedEvent(GlobalClass.connectionsData());
        }
    }
}
