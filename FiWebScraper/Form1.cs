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
        Notice notice;
        static int textData = 0;
        public decimal secondsDelay { get; set; } = 5000;
        public int maxValueBeforeAResponse { get; set; } = 100000;  

        public Form1()
        {
            InitializeComponent();
            scraper = new Scraper();
            notice = new Notice();
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
            dataGridView1.Columns[13].DefaultCellStyle.Format = $"{0:N}";

            while (textData%2 != 0)
            {

                //scraper.ScrapeData(@"https://marknadssok.fi.se/publiceringsklient");
                scraper.ScrapeData(@"http://localhost/dashboard/");

                //Updates the data
                source.ResetBindings(false);

                CheckIfNotice();

                if (checkedListBox1.GetItemCheckState(0) == CheckState.Checked)
                    { 
                    source.SuspendBinding();
                        HideSaleColumns();
                    source.ResumeBinding();
                    }


                //Delay until next update
                int.TryParse(secondsDelay.ToString(), out int timeout);
                await Task.Delay(timeout);
            }

        }

        private void CheckIfNotice()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                double.TryParse(dataGridView1.Rows[i].Cells[13].Value.ToString(), out double totalt);

                if (totalt > maxValueBeforeAResponse)
                {

                   


                    notifyIcon1.BalloonTipTitle = $"Ny Affär!";
                    notifyIcon1.BalloonTipText = $"{dataGridView1.Rows[i].Cells[2].Value} Gjorde ett köp över {maxValueBeforeAResponse}";
                    notifyIcon1.ShowBalloonTip(1000);
                }
            }

        }
     

        private void HideSaleColumns()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[5].Value.ToString().Equals("Avyttring"))
                {
                    dataGridView1.Rows[i].Visible = false;
                }
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

                if (totalt > maxValueBeforeAResponse)
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
            maxValueBeforeAResponse = input;
            UpdateCellColors();
        }

        private void CheckedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
