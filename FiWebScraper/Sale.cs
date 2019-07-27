using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiWebScraper
{
    public class Sale
    {
        public DateTime Publiceringsdatum { get; set; }
        public string Utgivare { get; set; }
        public string Namn { get; set; }
        public string Befattning { get; set; }
        public string Närstående { get; set; }
        public string Karaktär { get; set; }
        public string Instrumentnamn { get; set; }
        public string ISIN { get; set; }
        public DateTime Transaktionsdatum { get; set; }
        public double Volym { get; set; }
        public string Volymsenhet { get; set; }
        public double Pris { get; set; }
        public string Valuta { get; set; }
        public double Totalt { get; set; }
        public string Handelsplats { get; set; }
        public string Status { get; set; }
        public string Detaljer { get; set; }
    }
}
