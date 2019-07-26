using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private void Button1_Click(object sender, EventArgs e)
        {

        }
    }
}
