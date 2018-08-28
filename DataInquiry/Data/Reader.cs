using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInquiry.Assistant.Data
{
    public class Reader
    {
        private ArrayList _datalist;
        private int _idx;

        public Reader()
        {
            _datalist = new ArrayList();
            _idx = 0;
        }

        public int Count
        {
            get
            {
                if(_datalist == null)
                {
                    return 0;
                }

                return _datalist.Count;
            }
        }

        public Reader(SQLiteDataReader r)
        {
            _datalist = new ArrayList();
            _idx = 0;

            object [] val;
            int cnt = r.FieldCount;

            while(r.Read())
            {
                val = new object[cnt];

                for(int i=0;i<cnt;i++)
                {
                    val[i] = r[i];
                }

                _datalist.Add(val);
            }
        }

        public bool Read()
        {
            bool hasRows = false;

            if(_idx < _datalist.Count)
            {
                hasRows = true;
                ++_idx;
            }

            return hasRows;
        }

        public object this[int i]
        {
            get
            {
                object[] val = (object[])_datalist[_idx-1];
                return val[i];
            }
        }
    }
}
