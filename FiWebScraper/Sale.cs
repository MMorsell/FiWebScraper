using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiWebScraper
{
    public class Sale
    {
        public DateTime Publishdate { get; set; }
        public string Utgivare { get; set; }
        public string PersonName { get; set; }
        public string Position { get; set; }
        public string RelevantPosition { get; set; }
        public string TypeOfBusiness { get; set; }
        public string CompanyName { get; set; }
        public string ISIN { get; set; }
        public DateTime TransactionDate { get; set; }
        public int Volume { get; set; }
        public int Price { get; set; }
        public int TotalPriceOfBusiness { get; set; }
        public string LocationOfBusiness { get; set; }
    }
}
