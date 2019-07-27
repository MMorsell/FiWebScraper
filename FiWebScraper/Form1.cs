using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace FiWebScraper
{
    public partial class Form1 : Form
    {
        Scraper scraper;

        public Form1()
        {
            InitializeComponent();
            scraper = new Scraper();

        }

        private async void Button1_Click(object sender, EventArgs e)
        {


            
            BindingSource source = new BindingSource();
            source.DataSource = scraper.Sales;
            dataGridView1.DataSource = source;

            while (true)
            {

                //scraper.ScrapeData(@"https://marknadssok.fi.se/publiceringsklient");
                scraper.ScrapeData(@"http://localhost/dashboard/");

                await Task.Delay(3000);

                source.ResetBindings(false);


            }

        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private async void waitForTime()
        {
            
        }
    }
}
