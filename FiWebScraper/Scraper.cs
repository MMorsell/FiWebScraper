using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Web;

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
            var doc = web.Load(page);



            var sales = doc.DocumentNode.SelectSingleNode("//*[@class = 'table table-bordered table-hover table-striped zero-margin-top']");

            var items1 = sales.InnerText;


            List<string> listItems = items1.Split('\n').ToList();
            listItems.RemoveRange(0, 24);
            listItems.RemoveRange(listItems.Count - 1,1);

            for (int i = 0; i < listItems.Count; i++)
            {
                //Lennart Sigvard Olof Sj&#246;lund
                //F &#246;rv&#228;rv
                //Aff &#228;rsomr&#229;deschef
                listItems[i] = listItems[i].Replace('"', '!');
                listItems[i] = listItems[i].Trim();


                

            }

            List<int> positionsDescription = new List<int>();
            for (int i = 0; i < listItems.Count; i++)
            {
                if (listItems[i].Equals("Detaljer"))
                {
                    positionsDescription.Add(i);
                }
            }


            positionsDescription.Reverse();

            foreach (var removeBlankSpaces in positionsDescription)
            {
                    listItems.RemoveRange(removeBlankSpaces + 1, 4);
                    listItems.RemoveRange(removeBlankSpaces - 4, 4);

            }

            
            

            Console.ReadLine();
        }

    }
}
