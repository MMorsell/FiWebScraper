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

            //NOT DONE
            for (int i = 0; i < listOfText.Count; i++)
            {
               
                    if (listOfText[i].Contains("&#246;"))
                    {
                        listOfText[i] = listOfText[i].Replace('"', ' ');
                        listOfText[i] = listOfText[i].Replace('&', ' ');
                        listOfText[i] = listOfText[i].Replace('#', ' ');
                        listOfText[i] = listOfText[i].Replace('2', ' ');
                        listOfText[i] = listOfText[i].Replace('4', ' ');
                        listOfText[i] = listOfText[i].Replace('6', ' ');
                        string result = listOfText[i].Trim();
                        listOfText[i] = listOfText[i].Replace(';', ' ');
                    }
            }




            int nextPost = 0;
            for (int i = 0; i < 9; i++)
            {
                //Later Remove PublishDate and implement time as timeNow in sql if entry is not present
                DateTime.TryParse(listOfText[0 + nextPost], out DateTime publishDateParsed);
                DateTime.TryParse(listOfText[8 + nextPost], out DateTime transactionDateParsed);
                double.TryParse(listOfText[9 + nextPost], out double volymParsed);
                double.TryParse(listOfText[11 + nextPost], out double prisParsed);

                var sale = new Sale { Publiceringsdatum = publishDateParsed, Utgivare = listOfText[1 + nextPost], Namn = listOfText[2 + nextPost], Befattning = listOfText[3 + nextPost], Närstående = listOfText[4 + nextPost], Karaktär = listOfText[5 + nextPost], Instrumentnamn = listOfText[6 + nextPost], ISIN = listOfText[7 + nextPost], Transaktionsdatum = transactionDateParsed, Volym = volymParsed, Volymsenhet = listOfText[10 + nextPost], Pris = prisParsed, Valuta = listOfText[12 + nextPost], Handelsplats = listOfText[13 + nextPost], Status = listOfText[14 + nextPost], Detaljer = listOfText[15 + nextPost], TotalPriceOfBusiness = volymParsed * prisParsed };

                //checks if record already exists with person and total cost
                bool recordExist = false;
                foreach (var record in _sales)
                {
                    if (sale.TotalPriceOfBusiness == record.TotalPriceOfBusiness && sale.Namn == record.Namn)
                    {
                        recordExist = true;
                    }
                }


                if (!recordExist)
                {
                    _sales.Insert(0, sale);
                }


            nextPost = nextPost + 16;
            }



            //Lennart Sigvard Olof Sj&#246;lund
            //F &#246;rv&#228;rv
            //Aff &#228;rsomr&#229;deschef
        }

    }
}
