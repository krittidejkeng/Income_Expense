using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace Table_Accounting
{   
    public partial class Form1 : Form
    {
        string[] month = {"มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน", 
            "กรกฎาคม","สิงหาคม","กันยายน","ตุลาคม","พฤศจิกายน","ธันวาคม" };
        private string month_in, month_out;
        Form2 form2 = new Form2();
        Form3 form3 = new Form3();
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "...", //add database
            BasePath = "..."
        };
        IFirebaseClient client;
        public Form1()
        {
            InitializeComponent();
        }
        private async void btn_finish_Click(object sender, EventArgs e)
        {           
            CheckMonth();
        }

        public async void CheckMonth()
        {
            if (radi_myincome.Checked == true)
            {

                this.Hide();
                for (int i = 0; i <= month.Length; i++)
                {
                    if (dateTimePicker1.Value.Month == i)
                    {
                        form2.labelmonth.Text = month[i - 1];
                        month_in = month[i - 1];
                    }
                }

                form2.Show();

                var node_in2 = new start_Monthin
                {
                    Number = Convert.ToString(0)
                };
                SetResponse node2 = await client.SetTaskAsync($"Start_in:{month_in}", node_in2);
                

            }
            else if (radi_myout.Checked == true)
            {

                this.Hide();
                for (int i = 0; i <= month.Length; i++)
                {
                    if (dateTimePicker1.Value.Month == i)
                    {
                        form3.labelmonth.Text = month[i - 1];
                        month_out = month[i - 1];
                    }

                }
                form3.Show();             
                var node_out1 = new start_MonthOutAll
                {
                    Number = Convert.ToString(0),
                };
                SetResponse node1 = await client.SetTaskAsync($"Start_OutAll:{month_out}", node_out1);
                var node_out2 = new start_MonthOutEat
                {
                    Number = Convert.ToString(0),
                };
                SetResponse node2 = await client.SetTaskAsync($"Start_OutEat:{month_out}", node_out2);
                var node_out3 = new start_MonthOutEdu
                {
                    Number = Convert.ToString(0),
                };
                SetResponse node3 = await client.SetTaskAsync($"Start_OutEdu:{month_out}", node_out3);
                var node_out4 = new start_MonthOutEtc
                {
                    Number = Convert.ToString(0),
                };
                SetResponse node4 = await client.SetTaskAsync($"Start_OutEtc:{month_out}", node_out4);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form4 form4_1 = new Form4();
            this.Hide();
            form4_1.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new FireSharp.FirebaseClient(config);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
    class start_Monthin
    {
        public string Number { get; set; }
    }

    class start_MonthOutAll
    {
        public string Number { get; set; }
    }
    class start_MonthOutEat
    {
        public string Number { get; set; }
    }
    class start_MonthOutEdu
    {
        public string Number { get; set; }
    }
    class start_MonthOutEtc
    {
        public string Number { get; set; }
    }
}