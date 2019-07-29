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
        public bool ShowOnlySalesRows { get; set; } = false;
        public bool HideUHandelsplatsRows { get; set; } = false;
        public bool DisableColor { get; set; } = false;
        StringBuilder reportErrorMessages = new StringBuilder();
        public int reportErrorMessagesNumber { get; set; }
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
                while (textData % 2 != 0)
                {
                    //tries to download the new version
                    try
                    {
                        //scraper.ScrapeData(@"https://marknadssok.fi.se/publiceringsklient");
                        scraper.ScrapeData(@"http://192.168.1.35/dashboard/");

                    }
                    catch
                    {
                        if (reportErrorMessagesNumber != 5)
                        {
                            reportErrorMessages.AppendLine($"Misslyckad uppdatering {DateTime.Now.ToString("HH:mm:ss")}");
                            textBox3.Text = reportErrorMessages.ToString();
                            reportErrorMessagesNumber++;
                        }
                        else
                        {
                            reportErrorMessages.Clear();
                            reportErrorMessagesNumber = 0;
                        }
                    }
                    //Updates the data
                    if (dataGridView1.Enabled == true)
                        {
                            source.ResetBindings(false);
                        }

                    ControlAllCheckStates();

                    dataGridView1.ClearSelection();
                    
                
                    //Delay until next update
                    int.TryParse(SecondsDelay.ToString(), out int timeout);
                    await Task.Delay(timeout);
                }

                if (dataGridView1.Enabled == true)
                {
                    ControlAllCheckStates();
                }
            
            

        }

        private void SetupDataGrid()
        {
            //Settings for the datagrid
            source = new BindingSource
            {
                DataSource = scraper.Sales
            };
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
                //dataGridView1.Enabled = false;
                dataGridView1.Enabled = true;
                dataGridView1.ClearSelection();
            }
            else
            {
                Text = "Insynshandelsavläsare";
                button1.Text = "Start";
                dataGridView1.Enabled = true;
                dataGridView1.ClearSelection();
            }
        }

        private void ControlAllCheckStates()
        {

            //Display options
            DisplayOnlySelectedData();


            //Notification options below
            //Warn only about purchases
            if (checkBox2.Checked)
            {
                ReportOnlyPurchases = true;
            }
            else
            {
                ReportOnlyPurchases = false;
            }


            UpdateCellColors();
            PushNotice();
        }

        private void DisplayOnlySelectedData()
        {
            source.SuspendBinding();
            if (ShowOnlySalesRows)
            {
                if (HideUHandelsplatsRows)
                {
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        if (dataGridView1.Rows[i].Cells[6].Value.ToString().Equals("Förvärv", StringComparison.CurrentCultureIgnoreCase) && !dataGridView1.Rows[i].Cells[16].Value.ToString().Equals("Utanför handelsplats", StringComparison.CurrentCultureIgnoreCase))
                        {
                            dataGridView1.Rows[i].Visible = true;
                        }
                        else
                        {
                            dataGridView1.Rows[i].Visible = false;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        if (dataGridView1.Rows[i].Cells[6].Value.ToString().Equals("Förvärv", StringComparison.CurrentCultureIgnoreCase))
                        {
                            dataGridView1.Rows[i].Visible = true;
                        }
                        else
                        {
                            dataGridView1.Rows[i].Visible = false;
                        } 
                    }
                }
            }
            else
            {
                if (HideUHandelsplatsRows)
                {
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        if (!dataGridView1.Rows[i].Cells[16].Value.ToString().Equals("Utanför handelsplats", StringComparison.CurrentCultureIgnoreCase))
                        {
                            dataGridView1.Rows[i].Visible = true;
                        }
                        else
                        {
                            dataGridView1.Rows[i].Visible = false;
                        }
                    }
                }
                else
                {
                    for(int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                            dataGridView1.Rows[i].Visible = true;
                    }
                }
            }
            source.ResumeBinding();
        }

        private void PushNotice()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                double.TryParse(dataGridView1.Rows[i].Cells[14].Value.ToString(), out double totalt);

                if (totalt > MaxValueBeforeAResponse)
                {
                    //Add numberformat here
                    //string formattedTotal = $"{0:N}";
                    //string msg = string.Format(formattedTotal, totalt);
                    string message = $"{dataGridView1.Rows[i].Cells[3].Value} har {dataGridView1.Rows[i].Cells[6].Value} till ett värde av {totalt} på {dataGridView1.Rows[i].Cells[7].Value}";
                    NotifyIcon notifyIcon = new NotifyIcon
                    {
                        Visible = true,
                        BalloonTipTitle = $"Ny Affär av {dataGridView1.Rows[i].Cells[3].Value}",
                        BalloonTipText = message,
                        Icon = SystemIcons.Application
                    };


                    bool alreadySentAlert = CheckIfMessageIsAlreadySent(message);

                    if (!alreadySentAlert && SendPushNotice)
                    {


                        if (ReportOnlyPurchases)
                        {
                            if (dataGridView1.Rows[i].Cells[6].Value.ToString().Equals("Förvärv", StringComparison.CurrentCultureIgnoreCase))
                            {
                                notifyIcon.ShowBalloonTip(30000);
                                ListOfAlertMessagesSent.Add(message);
                            }

                        }
                        else
                        {
                            notifyIcon.ShowBalloonTip(30000);
                            ListOfAlertMessagesSent.Add(message);
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

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            decimal input = numericUpDown1.Value;
            SecondsDelay = 1000 * input;
        }

        private void UpdateCellColors()
        {
            source.SuspendBinding();
            if (!DisableColor)
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
            else
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
            }
            source.ResumeBinding();
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
                ShowOnlySalesRows = true;
            }
            else
            {
                ShowOnlySalesRows = false;
            }
            DisplayOnlySelectedData();
        }

        private void CheckBox2_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                HideUHandelsplatsRows = true;
            }
            else
            {
                HideUHandelsplatsRows = false;
            }
            DisplayOnlySelectedData();
        }

        private void CheckBox3_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                ReportOnlyPurchases = true;
                
            }
            else
            {
                ReportOnlyPurchases = true;
                
            }

            PushNotice();
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

        private void CheckBox5_CheckStateChanged_1(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                DisableColor = true;
            }
            else
            {
                DisableColor = false;
            }

            ControlAllCheckStates();
        }

        private void DataGridView1_Click(object sender, EventArgs e)
        {
            ControlAllCheckStates();
        }


    }
}
