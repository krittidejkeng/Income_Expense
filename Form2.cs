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
using Excel = Microsoft.Office.Interop.Excel;




namespace Table_Accounting
{
    
    public partial class Form2 : Form
    {
        DataTable tb = new DataTable();
        public int Totalincome=0;
        public int Total_old = 0;
        public byte count = 5; //สำหรับจับเวลากดปุ่ม update ตาราง
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "...",//add database
            BasePath = "..."
        };
        IFirebaseClient client;


        private async void Form2_Load(object sender, EventArgs e)
        {           
            client = new FireSharp.FirebaseClient(config);           
            tb.Columns.Add("ลำดับที่");
            tb.Columns.Add("รายการ");
            tb.Columns.Add("ราคา");
            gridView1_income.DataSource = tb;//รายจ่าย
            try
            {
                FirebaseResponse value_in = await client.GetTaskAsync($"Count for Income:{labelmonth.Text}/node/");
                Count_Income get_in = value_in.ResultAs<Count_Income>();
                var node_in2 = new start_Monthin
                {
                    Number = get_in.income_All
                };
                SetResponse node2 = await client.SetTaskAsync($"Start_in:{labelmonth.Text}/",
                    node_in2);
                Totalincome = get_in.Total_income;

            }
            catch { }
        }

        public Form2()
        {
            InitializeComponent();           
        }

        private void btn_cancle_Click(object sender, EventArgs e)
        {
            Form1 form1_2 = new Form1();
            this.Hide();
            form1_2.Show();
            
        }

        private async void btn_finish_Click(object sender, EventArgs e)
        {
            int parsedValue;
            if (!int.TryParse(incometxt.Text, out parsedValue))
            {
                MessageBox.Show("กรอกข้อมูลไม่ถูกต้อง กรุณาใส่ตัวเลข", "เกิดข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                FirebaseResponse value_in3 = await client.GetTaskAsync($"Start_in:{labelmonth.Text}/");
                start_Monthin get_in3 = value_in3.ResultAs<start_Monthin>();


                var node_in = new Count_Income
                {
                    income_All = (Convert.ToInt32(get_in3.Number) + 1).ToString(),
                    Total_income = Totalincome
                };
                SetResponse node = await client.SetTaskAsync($"Count for Income:{labelmonth.Text}/node/",
                    node_in);

                FirebaseResponse value_in = await client.GetTaskAsync($"Count for Income:{labelmonth.Text}/node/");
                Count_Income get_in = value_in.ResultAs<Count_Income>();

                var income = new IncomeTotal
                {
                    //รับค่า income-All มาและเมื่อกดปุ่ม นำไปบวกอีก 1 เสมอ จากนั้นก็แปลงให้อยู่ในรูปสตริง
                    Number = node_in.income_All,
                    Detail = detailtxt.Text, // รายการที่ได้รับ 
                    Income = incometxt.Text,  // จำนวนเงินของรายรับ                                           
                };
                SetResponse resincome = await client.SetTaskAsync($"Income:{labelmonth.Text}/order" +
                    income.Number, income);
                IncomeTotal result = resincome.ResultAs<IncomeTotal>(); //เพิ่มเติม


                Total_old = 0;//เซ็ตค่าให้ Total_old ให้เป็น 0 ทุกครั้งที่กดปุ่มบันทึก
                Total_old = get_in.Total_income;//รับค่าจาก firebase เก็บไว้ใน Total_old
                Totalincome = Total_old + Convert.ToInt32(income.Income);
                //นำค่่าจาก Total_old บวกกับ เงินที่เราใส่ไป และนำไปเก็บไว้ใน Totalincome
                var obj_amount = new Count_Income
                {
                    income_All = income.Number,
                    Total_income = Totalincome
                };
                SetResponse resincome2 = await client.SetTaskAsync($"Count for Income:{labelmonth.Text}/node/", obj_amount);
                MessageBox.Show("บันทึกเรียบร้อย");

                var obj_num = new start_Monthin
                {
                    Number = obj_amount.income_All
                };
                SetResponse node2 = await client.SetTaskAsync($"Start_in:{labelmonth.Text}/", obj_num);
            }
        }


        private async void button2_Click(object sender, EventArgs e) 
        {//ปุุ่มกดดูรายงานรายรับ
            try
            {
                timer1.Interval = 1000;
                timer1.Start();
                button2.Enabled = false;
                //btnreflesh.Enabled = true;

                //สำหรับเรียกค่า Total_expense
                FirebaseResponse value_out = await client.GetTaskAsync($"Count for Totalexpense:{labelmonth.Text}/node");
                Count_Expense get_out = value_out.ResultAs<Count_Expense>();

                //สำหรับเรียกค่า Total_eat
                FirebaseResponse value_eat = await client.GetTaskAsync($"Count for eatexpense:{labelmonth.Text}/node");
                Count_eat get_eat = value_eat.ResultAs<Count_eat>();

                //สำหรับเรียกค่า Total_edu
                FirebaseResponse value_edu = await client.GetTaskAsync($"Count for eduexpense:{labelmonth.Text}/node");
                Count_edu get_edu = value_edu.ResultAs<Count_edu>();

                //สำหรับเรียกค่า Total_etc
                FirebaseResponse value_etc = await client.GetTaskAsync($"Count for etcexpense:{labelmonth.Text}/node");
                Count_etc get_etc = value_etc.ResultAs<Count_etc>();

                //สำหรับเรียกค่า Total_income
                FirebaseResponse value_income = await client.GetTaskAsync($"Count for Income:{labelmonth.Text}/node/");
                Count_Income get_in = value_income.ResultAs<Count_Income>();
                //FirebaseResponse value_income = await client.GetTaskAsync("Count for income/node");
                //Count_Income get_in = value_income.ResultAs<Count_Income>();

                //ส่วนของยอดคงเหลือ
                label_MonthofExportBalance.Text = labelmonth.Text;
                label_monthAllBalance.Text = Convert.ToString(get_in.Total_income - get_out.Total_expense);

                chart1.Titles.Add("สรุปรายรับรายจ่าย");
                chart1.Series["s1"].IsValueShownAsLabel = true;
                chart1.Series["s1"].Points.AddXY("1", get_eat.Total_eat);
                chart1.Series["s1"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
                chart1.Series["s1"].Points.AddXY("2", get_edu.Total_edu);
                chart1.Series["s1"].Points.AddXY("3", get_etc.Total_etc);
                chart1.Series["s1"].Points.AddXY("4", get_in.Total_income);

                label_MonthofExport.Text = labelmonth.Text;
                int i = 0;
                FirebaseResponse resex_in1 = await client.GetTaskAsync($"Count for Income:{labelmonth.Text}/node/");
                //FirebaseResponse resex_in1 = await client.GetTaskAsync("Count for income/node");
                Count_Income objc_in1 = resex_in1.ResultAs<Count_Income>();
                int income_All = Convert.ToInt32(objc_in1.income_All);
                label_monthAll.Text = Convert.ToString(objc_in1.Total_income);

                while (true)
                {
                    i++;
                    try
                    {
                        FirebaseResponse resex_in2 = await client.GetTaskAsync($"Income:{labelmonth.Text}/order" + i);
                        IncomeTotal objc_in2 = resex_in2.ResultAs<IncomeTotal>();

                        DataRow row = tb.NewRow();
                        row["ลำดับที่"] = objc_in2.Number;
                        row["รายการ"] = objc_in2.Detail;
                        row["ราคา"] = objc_in2.Income;

                        tb.Rows.Add(row);
                    }
                    catch
                    {

                    }
                }
            }
            catch { MessageBox.Show($"ข้อมูลไม่ถูกต้องหรือไม่มีข้อมูลเดือน{labelmonth.Text}ในฐานระบบ"); }           
        }


        private async void btnDelete_Click(object sender, EventArgs e)
        {
            int parsedValue;
            if (!int.TryParse(Numbertxt.Text, out parsedValue))
            {
                MessageBox.Show("กรอกข้อมูลไม่ถูกต้อง กรุณาใส่ตัวเลข", "เกิดข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                
                btnDelete.Enabled = false;
                //สำหรับเรียกค่า Income และ  Number เก่าใน firebase
                FirebaseResponse value_income = await client.GetTaskAsync($"Count for Income:{labelmonth.Text}/node/");
                //FirebaseResponse value_income = await client.GetTaskAsync("Count for income/node");
                Count_Income get_in = value_income.ResultAs<Count_Income>();
                
                var income = new IncomeTotal
                {
                    //รับค่า income-All มาและเมื่อกดปุ่ม นำไปบวกอีก -1 เสมอ จากนั้นก็แปลงให้อยู่ในรูปสตริง
                    Number = (Convert.ToInt32(get_in.income_All) - 1).ToString(),
                    Detail = detailtxt.Text, // รายการที่ได้รับ 
                    Income = incometxt.Text,  // จำนวนเงินของรายรับ                                           
                };
                //สำหรับเรียกค่ารายลับและลำดับโดยการกำหนด

                FirebaseResponse response1 = await client.GetTaskAsync($"Income:{labelmonth.Text}/order" +
                    Numbertxt.Text);

                IncomeTotal obj = response1.ResultAs<IncomeTotal>();
                Total_old = 0;//เซ็ตค่าให้ Total_old ให้เป็น 0 ทุกครั้งที่กดปุ่มบันทึก
                Total_old = get_in.Total_income;//รับค่าจาก firebase เก็บไว้ใน Total_old
                Totalincome = Total_old - Convert.ToInt32(obj.Income);//obj.Income คือ เรียกค่ารายจ่ายที่จะลบ
                var obj_amount = new Count_Income
                {
                    income_All = income.Number,
                    Total_income = Totalincome
                };
                SetResponse resincome2 = await client.SetTaskAsync($"Count for Income:{labelmonth.Text}/node/", obj_amount);
                //SetResponse resincome2 = await client.SetTaskAsync("Count for income/node", obj_amount);
                var obj_start = new start_Monthin
                {
                    //รับค่า income-All มาและเมื่อกดปุ่ม นำไปบวกอีก -1 เสมอ จากนั้นก็แปลงให้อยู่ในรูปสตริง
                    Number = obj_amount.income_All

                };
                //สำหรับเรียกค่ารายลับและลำดับโดยการกำหนด$"Start_in:{labelmonth.Text}/"
                SetResponse start = await client.SetTaskAsync($"Start_in:{labelmonth.Text}/", obj_start);

                //----------------------------------------------------------------
                int i = Convert.ToInt32(Numbertxt.Text);
                while (i <= Convert.ToInt32(obj_amount.income_All))
                {
                    //i++;
                    try
                    {
                        FirebaseResponse response_loop = await client.GetTaskAsync($"Income:{labelmonth.Text}/order" + (i + 1));
                        IncomeTotal obj_loop = response_loop.ResultAs<IncomeTotal>();
                        var income_loop = new IncomeTotal
                        {
                            Number = Convert.ToString(i),
                            Detail = obj_loop.Detail, // รายการที่ได้รับ 
                            Income = obj_loop.Income,  // จำนวนเงินของรายรับ                                                     
                        };
                        SetResponse resincome3 = await client.SetTaskAsync($"Income:{labelmonth.Text}/order" + i, income_loop);
                    }
                    catch
                    {

                    }
                    i++;
                }
                //ลบค่าสุดท้าย
                FirebaseResponse response2 = await client.DeleteTaskAsync($"Income:{labelmonth.Text}/order" +
                    (Convert.ToInt32(obj_amount.income_All) + 1).ToString());
                MessageBox.Show("แก้ไขเรียบร้อย");
            }
        }

        private void btnreflesh_Click(object sender, EventArgs e)
        {            
            Form5 form5 = new Form5();
            this.Hide();
            form5.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            count -= 1;
            label_Time.Text = Convert.ToString(count);

            if (count == 0)
            {
                timer1.Stop();
                btnreflesh.Enabled = true;
                
            }          
        }
              
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
     
        private void button1_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            app.Visible = true;
            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;
            for (int i = 1; i < gridView1_income.Columns.Count + 1; i++)
            {
                worksheet.Cells[1, i] = gridView1_income.Columns[i - 1].HeaderText;
            }
            for (int i = 0; i < gridView1_income.Rows.Count - 1; i++)
            {
                for (int j = 0; j < gridView1_income.Columns.Count; j++)
                {
                    if (gridView1_income.Rows[i].Cells[j].Value != null)
                    {
                        worksheet.Cells[i + 2, j + 1] = gridView1_income.Rows[i].Cells[j].Value.ToString();
                    }
                    else
                    {
                        worksheet.Cells[i + 2, j + 1] = "";
                    }
                }
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            app.Visible = true;
            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;
            for (int i = 1; i < gridView1_income.Columns.Count + 1; i++)
            {
                worksheet.Cells[1, i] = gridView1_income.Columns[i - 1].HeaderText;
            }
            for (int i = 0; i < gridView1_income.Rows.Count - 1; i++)
            {
                for (int j = 0; j < gridView1_income.Columns.Count; j++)
                {
                    if (gridView1_income.Rows[i].Cells[j].Value != null)
                    {
                        worksheet.Cells[i + 2, j + 1] = gridView1_income.Rows[i].Cells[j].Value.ToString();
                    }
                    else
                    {
                        worksheet.Cells[i + 2, j + 1] = "";
                    }
                }
            }
        }

        private async void btnDeleteAll_income_Click(object sender, EventArgs e)
        {
            FirebaseResponse delete1 = await client.DeleteTaskAsync($"Count for Income:{labelmonth.Text}");
            FirebaseResponse delete2 = await client.DeleteTaskAsync($"Income:{labelmonth.Text}");
            var node_in2 = new start_Monthin
            {
                Number = Convert.ToString(0)
            };
            SetResponse node2 = await client.SetTaskAsync($"Start_in:{labelmonth.Text}", node_in2);
            Totalincome = 0;
        }
    }
    class IncomeTotal
    {
        public string Number { get; set; }
        public string Detail { get; set; }
        public string Income { get; set; }
    }
    class Count_Income
    {
        public string income_All { get; set; }
        public int Total_income { get; set; }
    }
}