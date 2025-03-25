using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDSTest
{
    public class AgencyChangeCountsByTitle
    {
        public AgencyChangeCountsByTitle()
        {
            titles = new List<AgencyChangeCountByTitle>();
        }

        public List<AgencyChangeCountByTitle> titles { get; set; }

    }
    public class AgencyChangeCountByTitle
    {
        public AgencyChangeCountByTitle() 
        {
            count = 0;
            titleValue = string.Empty;
        }

        public string titleValue { get; set; }
        public int count { get; set; }

    }


}
