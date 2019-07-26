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
            var webInterface = new HtmlWeb();
            var htmlDocument = webInterface.Load(page);

            var outerDiv = htmlDocument.DocumentNode.SelectSingleNode("//*[@class = 'table table-bordered table-hover table-striped zero-margin-top']");

            var outerDivText = outerDiv.InnerText;


            List<string> listOfText = outerDivText.Split('\n').ToList();
            listOfText.RemoveRange(0, 24);
            listOfText.RemoveRange(listOfText.Count - 1,1);

            for (int i = 0; i < listOfText.Count; i++)
            {
                listOfText[i] = listOfText[i].Replace('"', '!');
                listOfText[i] = listOfText[i].Trim();
            }

            List<int> intPositionsOfDetaljer = new List<int>();
            for (int i = 0; i < listOfText.Count; i++)
            {
                if (listOfText[i].Equals("Detaljer"))
                {
                    intPositionsOfDetaljer.Add(i);
                }
            }


            intPositionsOfDetaljer.Reverse();

            foreach (var intPosition in intPositionsOfDetaljer)
            {
                    listOfText.RemoveRange(intPosition + 1, 4);
                    listOfText.RemoveRange(intPosition - 4, 4);

            }



            int counterTo10 = 0;


            for (int i = 0; i < 9; i++)
            {
                //Later Remove PublishDate and implement time as timeNow in sql if entry is not present
                DateTime.TryParse(listOfText[0 + counterTo10], out DateTime publishDateParsed);
                DateTime.TryParse(listOfText[8 + counterTo10], out DateTime transactionDateParsed);
                double.TryParse(listOfText[9 + counterTo10], out double volymParsed);
                double.TryParse(listOfText[11 + counterTo10], out double prisParsed);

                _sales.Add(new Sale { Publiceringsdatum = publishDateParsed, Utgivare = listOfText[1 + counterTo10], Namn = listOfText[2 + counterTo10], Befattning = listOfText[3 + counterTo10], Närstående = listOfText[4 + counterTo10], Karaktär = listOfText[5 + counterTo10], Instrumentnamn = listOfText[6 + counterTo10], ISIN = listOfText[7 + counterTo10], Transaktionsdatum = transactionDateParsed, Volym = volymParsed, Volymsenhet = listOfText[10 + counterTo10], Pris = prisParsed, Valuta = listOfText[12 + counterTo10], Handelsplats = listOfText[13 + counterTo10], Status = listOfText[14 + counterTo10], Detaljer = listOfText[15 + counterTo10], TotalPriceOfBusiness = volymParsed * prisParsed });
                counterTo10 = counterTo10 + 16;
            }



                //if (counterTo10 == 0)
                //{
                //_sales.Add(new Sale { Publiceringsdatum = publishDateParsed, Utgivare = listOfText[2 + counterTo10], Namn = listOfText[3 + counterTo10], Befattning = listOfText[4 + counterTo10], Närstående = listOfText[5 + counterTo10], Karaktär = listOfText[6 + counterTo10], Instrumentnamn = listOfText[7 + counterTo10] });

                //counterTo10 = counterTo10 + 16;
                //}
                //else
                //{
                
                //}


            //Lennart Sigvard Olof Sj&#246;lund
            //F &#246;rv&#228;rv
            //Aff &#228;rsomr&#229;deschef
            Console.ReadLine();
        }

    }
}
