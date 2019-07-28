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
        public decimal SecondsDelay { get; set; } = 5000;
        public int MaxValueBeforeAResponse { get; set; } = 300000;
        public List<string> ListOfAlertMessagesSent { get; set; }

        public bool ReportOnlyPurchases { get; set; } = false;
        public bool SendPushNotice { get; set; } = true;

        public Form1()
        {
            InitializeComponent();
            scraper = new Scraper();
            Text = "Insynshandelsavläsare";
            ListOfAlertMessagesSent = new List<string>();
            SetupDataGrid();

        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            CheckTextData();

            //Primary loop
            while (textData%2 != 0)
            {

                //scraper.ScrapeData(@"https://marknadssok.fi.se/publiceringsklient");
                scraper.ScrapeData(@"http://localhost/dashboard/");

                //Updates the data
                source.ResetBindings(false);

                ControlAllCheckStates();

                //Delay until next update
                int.TryParse(SecondsDelay.ToString(), out int timeout);
                await Task.Delay(timeout);
            }

            ControlAllCheckStates();

        }

        private void SetupDataGrid()
        {
            //Settings for the datagrid
            source = new BindingSource();
            source.DataSource = scraper.Sales;
            dataGridView1.DataSource = source;
            dataGridView1.Columns[14].DefaultCellStyle.Format = $"{0:N}";
        }

        private void CheckTextData()
        {
            textData++;
            if (textData % 2 != 0)
            {
                button1.Text = "Pause";
                Text = "Programmet Körs";
            }
            else
            {
                Text = "Insynshandelsavläsare";
                button1.Text = "Start";
            }
        }

        private void ControlAllCheckStates()
        {
            //Warn only about purchases
            if (checkBox2.Checked)
            {
                ReportOnlyPurchases = true;
            }
            else
            {
                ReportOnlyPurchases = false;
            }


            //Show only purchases
            if (checkBox1.Checked)
            {
                source.SuspendBinding();
                HideSaleColumns();
                source.ResumeBinding();
            }
            else
            {
                source.SuspendBinding();
                UnHideSaleColumns();
                source.ResumeBinding();
            }

            //Show every sale except outside __
            if (checkBox3.Checked)
            {
                source.SuspendBinding();
                HideUHandelsplatsColumns();
                source.ResumeBinding();
            }
            else
            {
                source.SuspendBinding();
                UnHideUHandelsplatsColumns();
                source.ResumeBinding();
            }

            UpdateCellColors();
            PushNotice();



        }

        private void PushNotice()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                double.TryParse(dataGridView1.Rows[i].Cells[14].Value.ToString(), out double totalt);

                if (totalt > MaxValueBeforeAResponse)
                {
                    NotifyIcon notifyIcon = new NotifyIcon();
                    notifyIcon.Visible = true;
                    notifyIcon.BalloonTipTitle = $"Ny Affär av {dataGridView1.Rows[i].Cells[3].Value}";
                    notifyIcon.BalloonTipText = $"{dataGridView1.Rows[i].Cells[3].Value} har {dataGridView1.Rows[i].Cells[6].Value} till ett värde av {dataGridView1.Rows[i].Cells[14].Value} på {dataGridView1.Rows[i].Cells[7].Value}";
                    notifyIcon.Icon = SystemIcons.Application;


                    bool alreadySentAlert = CheckIfMessageIsAlreadySent($"{dataGridView1.Rows[i].Cells[3].Value} har {dataGridView1.Rows[i].Cells[6].Value} till ett värde av {dataGridView1.Rows[i].Cells[14].Value} på {dataGridView1.Rows[i].Cells[7].Value}");

                    if (!alreadySentAlert && SendPushNotice)
                    {


                        if (ReportOnlyPurchases)
                        {
                            if (dataGridView1.Rows[i].Cells[6].Value.ToString() == "Förvärv")
                            {
                                notifyIcon.ShowBalloonTip(30000);
                                ListOfAlertMessagesSent.Add($"{dataGridView1.Rows[i].Cells[3].Value} har {dataGridView1.Rows[i].Cells[6].Value} till ett värde av {dataGridView1.Rows[i].Cells[14].Value} på {dataGridView1.Rows[i].Cells[7].Value}");
                            }

                        }
                        else
                        {
                            notifyIcon.ShowBalloonTip(30000);
                            ListOfAlertMessagesSent.Add($"{dataGridView1.Rows[i].Cells[3].Value} har {dataGridView1.Rows[i].Cells[6].Value} till ett värde av {dataGridView1.Rows[i].Cells[14].Value} på {dataGridView1.Rows[i].Cells[7].Value}");
                        }



                    }

                    notifyIcon.Dispose();
                }
            }
        }

        private bool CheckIfMessageIsAlreadySent(string v)
        {
            bool result = false;
            foreach (var message in ListOfAlertMessagesSent)
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
        private void UnHideSaleColumns()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[6].Value.ToString().Equals("Avyttring"))
                {
                    dataGridView1.Rows[i].Visible = true;
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
        private void UnHideUHandelsplatsColumns()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[15].Value.ToString().Equals("Utanför handelsplats"))
                {
                    if (checkBox1.Checked && dataGridView1.Rows[i].Cells[6].Value.ToString().Equals("Avyttring"))
                    {

                    }
                    else
                    {
                        dataGridView1.Rows[i].Visible = true;
                    }
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
            SecondsDelay = 1000 * input;
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {   
        }

        private void UpdateCellColors()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                double.TryParse(dataGridView1.Rows[i].Cells[14].Value.ToString(), out double totalt);

                if (totalt > MaxValueBeforeAResponse)
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
            MaxValueBeforeAResponse = input;
            UpdateCellColors();
            PushNotice();
        }

        private void CheckBox1_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                source.SuspendBinding();
                HideSaleColumns();
                source.ResumeBinding();
            }
            else
            {
                source.SuspendBinding();
                UnHideSaleColumns();
                source.ResumeBinding();
            }
        }

        private void CheckBox2_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                ReportOnlyPurchases = true;
                PushNotice();
            }
            else
            {
                ReportOnlyPurchases = true;
                PushNotice();
            }
        }

        private void CheckBox3_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                source.SuspendBinding();
                HideUHandelsplatsColumns();
                source.ResumeBinding();
            }
            else
            {
                source.SuspendBinding();
                UnHideUHandelsplatsColumns();
                source.ResumeBinding();
            }
        }

        private void CheckBox4_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                SendPushNotice = false;
            }
            else
            {
                SendPushNotice = true;
            }

            PushNotice();
        }
    }
}
