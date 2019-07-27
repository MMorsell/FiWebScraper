using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FiWebScraper
{
    public class Notice
    {


        public List<string> ListOfMessagesSent { get; set; }
        public Notice()
        {
            ListOfMessagesSent = new List<string>();
        }




        public void ShowPopup(string title, string message)
        {
            

        }
    }
}
