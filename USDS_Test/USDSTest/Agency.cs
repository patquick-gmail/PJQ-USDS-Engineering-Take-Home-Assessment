using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace USDSTest
{
    public class Agencies
    {
        public Agencies()
        {
            //name = string.Empty;
            agencies= new List<Agency>();
        }

        //public string name { get; set; }
        public List<Agency> agencies { get; set; }
    }


    public class Agency
    {
        public Agency()
        {
            name = string.Empty;
            short_name = string.Empty;
            display_name = string.Empty;
            sortable_name = string.Empty;
            slug = string.Empty;
            children = new List<Agency> { };
            cfr_references = new List<cfr_reference> { };

            word_count = string.Empty;
            total_count = string.Empty; 
            description = string.Empty;
            agencyChangeCountsByDate = new AgencyChangeCountsByDate();

        }

        public string name { get; set; }
        public string short_name { get; set; }
        public string display_name { get; set; }
        public string sortable_name { get; set; }
        public string slug { get; set; }
        public List<Agency>? children { get; set; }
        public List<cfr_reference>? cfr_references { get; set; }
                

        //word count
        public string word_count { get; set; }

        //Historical Changes
        public string total_count { get; set; } 
        public string description { get; set; } 
        
        public AgencyChangeCountsByDate? agencyChangeCountsByDate { get; set; }
        public AgencyChangeCountsByTitle? agencyChangeCountsByTitle { get; set; }


    }
}
