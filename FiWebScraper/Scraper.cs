using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace FiWebScraper
{
    public class Scraper
    {
        private ObservableCollection<Sale> _sales = new ObservableCollection<Sale>();

        public ObservableCollection<Sale> Sales
        {
            get { return _sales; }
            set { _sales = value; }
        }


        public void ScrapeData(string page)
        {
            var web = new HtmlWeb();
            web.
        }

    }
}
