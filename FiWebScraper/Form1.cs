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
        BindingSource source;
        public decimal secondsDelay { get; set; } = 5000;
        public int maxValueBeforeAResponse { get; set; } = 300000;
        public List<string> listOfAlertMessagesSent { get; set; }

        public Form1()
        {
            InitializeComponent();
            scraper = new Scraper();
            Text = "Insynshandelsavläsare";
            listOfAlertMessagesSent = new List<string>();

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
            
            //Settings for the datagrid
            source = new BindingSource();
            source.DataSource = scraper.Sales;
            dataGridView1.DataSource = source;
            dataGridView1.Columns[14].DefaultCellStyle.Format = $"{0:N}";


            //Primary loop
            while (textData%2 != 0)
            {

                //scraper.ScrapeData(@"https://marknadssok.fi.se/publiceringsklient");
                scraper.ScrapeData(@"http://localhost/dashboard/");

                //Updates the data
                source.ResetBindings(false);

                CheckIfNotice();
                CheckIfHideRows();

                //Delay until next update
                int.TryParse(secondsDelay.ToString(), out int timeout);
                await Task.Delay(timeout);
            }

        }

        private void CheckIfHideRows()
        {
            if (checkedListBox1.GetItemCheckState(0) == CheckState.Checked)
            {
                source.SuspendBinding();
                HideSaleColumns();
                source.ResumeBinding();
            }


            if (checkedListBox1.GetItemCheckState(2) == CheckState.Checked)
            {
                source.SuspendBinding();
                HideUHandelsplatsColumns();
                source.ResumeBinding();
            }
        }

        private void CheckIfNotice()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                double.TryParse(dataGridView1.Rows[i].Cells[14].Value.ToString(), out double totalt);

                if (totalt > maxValueBeforeAResponse)
                {
                    //Add notification here
                    NotifyIcon notifyIcon = new NotifyIcon();
                    notifyIcon.Visible = true;
                    notifyIcon.BalloonTipTitle = $"Ny Affär av {dataGridView1.Rows[i].Cells[3].Value}";
                    notifyIcon.BalloonTipText = $"{dataGridView1.Rows[i].Cells[3].Value} har {dataGridView1.Rows[i].Cells[6].Value} till ett värde av {dataGridView1.Rows[i].Cells[14].Value} på {dataGridView1.Rows[i].Cells[7].Value}";
                    notifyIcon.Icon = SystemIcons.Application;


                    bool alreadySentAlert = CheckIfMessageIsAlreadySent($"{dataGridView1.Rows[i].Cells[3].Value} har {dataGridView1.Rows[i].Cells[6].Value} till ett värde av {dataGridView1.Rows[i].Cells[14].Value} på {dataGridView1.Rows[i].Cells[7].Value}");

                    if (!alreadySentAlert && checkedListBox1.GetItemCheckState(4) != CheckState.Checked)
                    {

                        if (checkedListBox1.GetItemCheckState(1) == CheckState.Checked)
                        {
                            if (dataGridView1.Rows[i].Cells[6].Value.ToString() == "Förvärv")
                            {
                                notifyIcon.ShowBalloonTip(30000);
                                listOfAlertMessagesSent.Add($"{dataGridView1.Rows[i].Cells[3].Value} har {dataGridView1.Rows[i].Cells[6].Value} till ett värde av {dataGridView1.Rows[i].Cells[14].Value} på {dataGridView1.Rows[i].Cells[7].Value}");
                            }

                        }
                        else
                        {
                            notifyIcon.ShowBalloonTip(30000);
                            listOfAlertMessagesSent.Add($"{dataGridView1.Rows[i].Cells[3].Value} har {dataGridView1.Rows[i].Cells[6].Value} till ett värde av {dataGridView1.Rows[i].Cells[14].Value} på {dataGridView1.Rows[i].Cells[7].Value}");
                        }



                    }

                    notifyIcon.Dispose();
                }
            }

        }

        private bool CheckIfMessageIsAlreadySent(string v)
        {
            bool result = false;
            foreach (var message in listOfAlertMessagesSent)
            {
                if (message == v)
                {
                    result = true;
                }
            }

            return result;
        }

        private void HideSaleColumns()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[6].Value.ToString().Equals("Avyttring"))
                {
                    dataGridView1.Rows[i].Visible = false;
                }
            }
        }

        private void HideUHandelsplatsColumns()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[15].Value.ToString().Equals("Utanför handelsplats"))
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
                double.TryParse(dataGridView1.Rows[i].Cells[14].Value.ToString(), out double totalt);

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
            CheckIfNotice();
        }

        private void CheckedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void CheckedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
        }
    }
}
