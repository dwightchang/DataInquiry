using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace DataInquiry.Assistant
{
    class MatchComparer : IComparer
    {
        public int Compare(object obja, object objb)
        {
            Match a, b;
            a = (Match)obja;
            b = (Match)objb;

            if (a.Index < b.Index)
            {
                return -1;
            }

            if (a.Index > b.Index)
            {
                return 1;
            }

            if (a.Length > b.Length)
            {
                return -1;
            }

            if (a.Length < b.Length)
            {
                return 1;
            }

            return 0;
        }
    }
}
