using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDSTest
{
    public class AgencyChangeCountsByDate
    {
        public AgencyChangeCountsByDate()
        {
            dates = new List<AgencyChangeCountByDate>();
        }

        public List<AgencyChangeCountByDate> dates { get; set; }

    }
    public class AgencyChangeCountByDate
    {
        public AgencyChangeCountByDate() 
        {
            count = 0;
            dateValue = string.Empty;
        }

        public string dateValue { get; set; }
        public int count { get; set; }

    }


}
