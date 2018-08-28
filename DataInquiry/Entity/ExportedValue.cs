using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInquiry.Assistant.Entity
{
    public class ExportedValue
    {
        public bool needQuote { get; set; }
        public bool isNull { get; set; }
        public string rawValue { get; set; }
        public string value
        {
            get
            {
                string v = this.isNull ? "null" : rawValue;

                if(needQuote)
                {
                    return "'" + v + "'";
                }

                return v;
            }
        }
    }
}
