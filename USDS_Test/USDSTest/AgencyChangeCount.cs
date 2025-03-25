using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDSTest
{
    public class AgencyChangeCounts
    {
        public AgencyChangeCounts()
        {
            meta = new AgencyChangeCount();
        }

        public AgencyChangeCount meta { get; set; }

    }
    public class AgencyChangeCount
    {
        public AgencyChangeCount() 
        {
            total_count = 0;
            description = string.Empty;
        }

        public int total_count { get; set; }
        public string description { get; set; }

    }
}
