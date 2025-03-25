using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDSTest
{
    public class cfr_reference
    {
        public cfr_reference() 
        {
            title = 0;
            chapter = string.Empty;
        }

        public int title { get; set; }

        //public string title { get; set; }
        public string chapter { get; set; }
    }
}
