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
        static int textData = 0;
        public decimal secondsDelay { get; set; } = 5000;
        public int valueToSendRespond { get; set; } = 300000;

        public Form1()
        {
            InitializeComponent();
            scraper = new Scraper();
            Text = "Insynshandelsavläsare";

        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            
            textData++;
            if (textData%2 != 0)
            {
                button1.Text = "Pause";
                Text = "Programmet Körs";
            }
            else
            {
                Text = "Insynshandelsavläsare";
                button1.Text = "Start";
            }


            BindingSource source = new BindingSource();
            source.DataSource = scraper.Sales;
            dataGridView1.DataSource = source;

            while (textData%2 != 0)
            {

                //scraper.ScrapeData(@"https://marknadssok.fi.se/publiceringsklient");
                scraper.ScrapeData(@"http://localhost/dashboard/");


                source.ResetBindings(false);
                int.TryParse(secondsDelay.ToString(), out int timeout);
                await Task.Delay(timeout);



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

        private void TextBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            decimal input = numericUpDown1.Value;
            secondsDelay = 1000 * input;
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            UpdateCellColors();
            
        }

        private void UpdateCellColors()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                double.TryParse(dataGridView1.Rows[i].Cells[13].Value.ToString(), out double totalt);

                if (totalt > valueToSendRespond)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.White;
                }
                else
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }

        private void TextBox2_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void TextBox2_TextChanged_2(object sender, EventArgs e)
        {

        }

        private void NumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            int.TryParse(numericUpDown2.Value.ToString(), out int input);
            valueToSendRespond = input;
            UpdateCellColors();
        }
    }
}
