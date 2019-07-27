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


        private ObservableCollection<Sale> _addedSales = new ObservableCollection<Sale>();

        public ObservableCollection<Sale> AddedSales
        {
            get { return _sales; }
            set { _sales = value; }
        }

        public void ScrapeData(string page)
        {

            List<string> listOfText = DownloadNewVersion(page);





            int nextPost = 0;
            for (int i = 0; i < 9; i++)
            {
                //
                //Later Remove PublishDate and implement time as timeNow in sql if entry is not present
                //
                //


                //Creates a new sale
                DateTime.TryParse(listOfText[0 + nextPost], out DateTime publishDateParsed);
                DateTime.TryParse(listOfText[8 + nextPost], out DateTime transactionDateParsed);
                double.TryParse(listOfText[9 + nextPost], out double volymParsed);
                double.TryParse(listOfText[11 + nextPost], out double prisParsed);

                var sale = new Sale { Publiceringsdatum = publishDateParsed, Utgivare = listOfText[1 + nextPost], Namn = listOfText[2 + nextPost], Befattning = listOfText[3 + nextPost], Närstående = listOfText[4 + nextPost], Karaktär = listOfText[5 + nextPost], Instrumentnamn = listOfText[6 + nextPost], ISIN = listOfText[7 + nextPost], Transaktionsdatum = transactionDateParsed, Volym = volymParsed, Volymsenhet = listOfText[10 + nextPost], Pris = prisParsed, Valuta = listOfText[12 + nextPost], Handelsplats = listOfText[13 + nextPost], Status = listOfText[14 + nextPost], Detaljer = listOfText[15 + nextPost], Totalt = volymParsed * prisParsed };



                //checks if record already exists with person and total cost
                bool recordExistInSaleList = EntryAlreadyExistsInSaleList(sale);





                //Checks if entry is already combined to one row
                bool  entryAlreadyExistsInAddedList = EntryAlreadyExistsInAlreadyAddedList(sale);




                //checks if person has bought many and combines the ammount to one row. statusrow updates with number of sales, total volume and total cost is correct

                bool isSecondPurchaseOfSameStock = EntryHasBeenAddedToOneRow(sale, recordExistInSaleList, entryAlreadyExistsInAddedList);
               


                //if it doesnt exist, add it to the main interface
                //if (!recordExist && !secondPurchase && !alreadyAddedinList)
                if (!recordExistInSaleList && !isSecondPurchaseOfSameStock && !entryAlreadyExistsInAddedList)
                {
                    _sales.Insert(0, sale);
                    _addedSales.Add(sale);
                }


            nextPost = nextPost + 16;
            }
        }

        private List<string> DownloadNewVersion(string page)
        {
            var webInterface = new HtmlWeb();
            var htmlDocument = webInterface.Load(page);

            var outerDiv = htmlDocument.DocumentNode.SelectSingleNode("//*[@class = 'table table-bordered table-hover table-striped zero-margin-top']");

            var outerDivText = outerDiv.InnerText;


            List<string> listOfText = outerDivText.Split('\n').ToList();

            listOfText.RemoveRange(0, 24);
            listOfText.RemoveRange(listOfText.Count - 1, 1);

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

            //NOT DONE NEED TO FIX SWEDISH CHARSET
            //NEED TO FIX FIRST 10 ITEMS TO BE ADDED AS USUAL; NOT INSERTED
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

            return listOfText;
        }

        private bool EntryHasBeenAddedToOneRow(Sale sale, bool recordExistInSaleList, bool entryAlreadyExistsInAddedList)
        {
            bool result = false;



            if (!recordExistInSaleList && !entryAlreadyExistsInAddedList)
            {
                foreach (var record in _sales)
                {
                    if (sale.Utgivare == record.Utgivare && sale.Namn == record.Namn && sale.Befattning == record.Befattning && sale.Karaktär == record.Karaktär && sale.Instrumentnamn == record.Instrumentnamn)
                    {
                        _addedSales.Add(sale);
                        _addedSales.Add(record);






                        //int.TryParse(sale.Status.ToString(), out int salestatus);
                        //salestatus++;
                        //record.Status = salestatus.ToString();


                        double extravalue = sale.Pris * sale.Volym;


                        record.Totalt = record.Totalt + extravalue;
                        record.Volym = record.Volym + sale.Volym;
                        result = true;
                        _addedSales.Add(record);

                    }
                }
            }

            return result;
        }

        private bool EntryAlreadyExistsInSaleList(Sale sale)
        {
            bool result = false;
            foreach (var record in _sales)
            {
                if (sale.Totalt == record.Totalt && sale.Namn == record.Namn)
                {
                    result = true;
                }
            }
            return result;
        }

        private bool EntryAlreadyExistsInAlreadyAddedList(Sale newEntry)
        {
            bool result = false;

            foreach (var entry in _addedSales)
            {
                if (newEntry.Utgivare == entry.Utgivare && newEntry.Namn == entry.Namn && newEntry.Befattning == entry.Befattning && newEntry.Karaktär == entry.Karaktär && newEntry.Instrumentnamn == entry.Instrumentnamn && newEntry.Pris == entry.Pris && newEntry.Volym == entry.Volym && newEntry.Totalt == entry.Totalt)
                {
                    result = true;
                }
            }

            return result;
        }
    }
}
