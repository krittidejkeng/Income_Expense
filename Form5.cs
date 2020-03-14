using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Table_Accounting
{
    public partial class Form5 : Form
    {
        Form2 form2 = new Form2();
        string[] month = {"มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน",
            "กรกฎาคม","สิงหาคม","กันยายน","ตุลาคม","พฤศจิกายน","ธันวาคม" };
        public byte count = 2;
        public Form5()
        {
            InitializeComponent();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < month.Length; i++)
            {
                if (dateTimePicker1.Value.Month == i)
                {
                    form2.labelmonth.Text = month[i - 1];
                }

            }
            timer1.Interval = 1000;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            count -= 1;
            label1.Text = Convert.ToString(count);
            
            if (count == 0)
            {
                timer1.Stop();
                this.Hide();
                form2.Show();
            }
        }

        private void Form5_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}