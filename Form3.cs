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
    public partial class Form3 : Form
    {
        public int Totalexpense = 0, Total_old = 0;
        public int Total_Eatold = 0, Totaleat = 0;
        public int Total_Eduold = 0, Totaledu = 0;
        public int Total_Etcold = 0, Totaletc = 0;

        public int Newexpense_etc = 0, Oldexpense_etc = 0;
        public int Newexpense_eat = 0, Oldexpense_eat = 0;
        public int Newexpense_edu = 0, Oldexpense_edu = 0;

        public byte count = 5; //สำหรับจับเวลากดปุ่ม update ตาราง
        DataTable tb = new DataTable();     
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "...", //add database
            BasePath = "..."
        };
        IFirebaseClient client;

        private void btnreflesh_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6();
            this.Hide();
            form6.Show();
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

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            app.Visible = true;
            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;
            for (int i = 1; i < gridView1_expense.Columns.Count + 1; i++)
            {
                worksheet.Cells[1, i] = gridView1_expense.Columns[i - 1].HeaderText;
            }
            for (int i = 0; i < gridView1_expense.Rows.Count - 1; i++)
            {
                for (int j = 0; j < gridView1_expense.Columns.Count; j++)
                {
                    if (gridView1_expense.Rows[i].Cells[j].Value != null)
                    {
                        worksheet.Cells[i + 2, j + 1] = gridView1_expense.Rows[i].Cells[j].Value.ToString();
                    }
                    else
                    {
                        worksheet.Cells[i + 2, j + 1] = "";
                    }
                }
            }
        }

        private async void Form3_Load(object sender, EventArgs e)
        {
            radi_eat.Checked = true;
            DelectEat.Checked = true;
            client = new FireSharp.FirebaseClient(config);

            tb.Columns.Add("ลำดับที่");
            tb.Columns.Add("รายการ");
            tb.Columns.Add("ราคา");
            tb.Columns.Add("ประเภท");
            gridView1_expense.DataSource = tb;//รายจ่าย    
            try
            {
                FirebaseResponse value_out = await client.GetTaskAsync($"Count for Totalexpense:{labelmonth.Text}/node/");
                Count_Expense get_out = value_out.ResultAs<Count_Expense>();

                var node_out = new start_MonthOutAll
                {
                    Number = get_out.expense_All,

                };
                SetResponse node1 = await client.SetTaskAsync($"Start_OutAll:{labelmonth.Text}/", node_out);
                Totalexpense = get_out.Total_expense;

            }
            catch
            {

            }

            try
            {
                FirebaseResponse value_out = await client.GetTaskAsync($"Count for eatexpense:{labelmonth.Text}/node/");
                Count_eat get_out = value_out.ResultAs<Count_eat>();

                var node_out = new start_MonthOutEat
                {
                    Number = get_out.eatexpense_All,

                };
                SetResponse node1 = await client.SetTaskAsync($"Start_OutEat:{labelmonth.Text}/", node_out);
                Newexpense_eat = get_out.Total_eat;

            }
            catch
            {

            }

            try
            {
                FirebaseResponse value_out = await client.GetTaskAsync($"Count for eduexpense:{labelmonth.Text}/node/");
                Count_edu get_out = value_out.ResultAs<Count_edu>();

                var node_out = new start_MonthOutEdu
                {
                    Number = get_out.eduexpense_All,

                };
                SetResponse node1 = await client.SetTaskAsync($"Start_OutEdu:{labelmonth.Text}/", node_out);
                Newexpense_edu = get_out.Total_edu;

            }
            catch
            {

            }

            try
            {
                FirebaseResponse value_out = await client.GetTaskAsync($"Count for etcexpense:{labelmonth.Text}/node/");
                Count_etc get_out = value_out.ResultAs<Count_etc>();

                var node_out = new start_MonthOutEtc
                {
                    Number = get_out.etcexpense_All,

                };
                SetResponse node1 = await client.SetTaskAsync($"Start_OutEtc:{labelmonth.Text}/", node_out);
                Newexpense_etc = get_out.Total_etc;

            }
            catch
            {

            }           
        }

        public Form3()
        {
            InitializeComponent();
        }

        private void btncancle_Click(object sender, EventArgs e)
        {
            Form1 form1_3 = new Form1();
            this.Hide();
            form1_3.Show();
        }

        private async void btnfinish_Click(object sender, EventArgs e)
        {
            int parsedValue;
            if (!int.TryParse(expensetxt.Text, out parsedValue))
            {
                MessageBox.Show("กรอกข้อมูลไม่ถูกต้อง กรุณาใส่ตัวเลข", "เกิดข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                //-----------------------สำหรับเลือกของกิน---------------------------------
                if (radi_eat.Checked == true)//สำหรับเลือกเป็นของกิน
                {
                    try
                    {

                        //------------------------------------------------
                        FirebaseResponse value_out1 = await client.GetTaskAsync($"Start_OutAll:{labelmonth.Text}/");
                        start_MonthOutAll get_out1 = value_out1.ResultAs<start_MonthOutAll>();

                        var node_out = new Count_Expense
                        {
                            expense_All = (Convert.ToInt32(get_out1.Number) + 1).ToString(),
                            Total_expense = Totalexpense
                        };
                        SetResponse resincome3 = await client.SetTaskAsync($"Count for Totalexpense:{labelmonth.Text}/node", node_out);

                        FirebaseResponse value_out = await client.GetTaskAsync($"Count for Totalexpense:{labelmonth.Text}/node");
                        Count_Expense get_out = value_out.ResultAs<Count_Expense>();
                        var expense = new ExpenseTotal
                        {
                            //รับค่า income-All มาและเมื่อกดปุ่ม นำไปบวกอีก 1 เสมอ จากนั้นก็แปลงให้อยู่ในรูปสตริง
                            Number = node_out.expense_All,
                            Detail = detailtxt.Text,
                            Expense = expensetxt.Text,  // จำนวนเงินของรายรับ 
                            Type = "อุปโภคบริโภค"
                        };
                        SetResponse resexpense = await client.SetTaskAsync($"Expense:{labelmonth.Text}/order" +
                            expense.Number, expense);
                        ExpenseTotal result = resexpense.ResultAs<ExpenseTotal>(); //เพิ่มเติม

                        Total_old = 0; //เซ็ตค่าให้ Total_old ให้เป็น 0 ทุกครั้งที่กดปุ่มบันทึก
                        Total_old = get_out.Total_expense; //รับค่าจาก firebase เก็บไว้ใน Total_old
                                                           //นำค่่าจาก Total_old บวกกับ เงินที่เราใส่ไป และนำไปเก็บไว้ใน Totalexpense
                        Totalexpense = Total_old + Convert.ToInt32(expense.Expense);

                        var obj_amount = new Count_Expense
                        {
                            expense_All = expense.Number,
                            Total_expense = Totalexpense
                        };
                        SetResponse resincome2 = await client.SetTaskAsync($"Count for Totalexpense:{labelmonth.Text}/node", obj_amount);

                        var obj_num = new start_MonthOutAll
                        {
                            Number = obj_amount.expense_All,
                        };
                        SetResponse node2 = await client.SetTaskAsync($"Start_OutAll:{labelmonth.Text}/", obj_num);
                        //เทียบจาก form2 ถูกถึงตรงนี้


                        //------------------------------------------------------
                        FirebaseResponse value_out2 = await client.GetTaskAsync($"Start_OutEat:{labelmonth.Text}/");
                        start_MonthOutEat get_out2 = value_out2.ResultAs<start_MonthOutEat>();
                        var obj_eat = new Count_eat
                        {
                            eatexpense_All = (Convert.ToInt32(get_out2.Number) + 1).ToString(),
                            Total_eat = Newexpense_eat
                        };
                        SetResponse resexpense_eat = await client.SetTaskAsync($"Count for eatexpense:{labelmonth.Text}/node", obj_eat);

                        FirebaseResponse value_eat = await client.GetTaskAsync($"Count for eatexpense:{labelmonth.Text}/node");
                        Count_eat get_eat = value_eat.ResultAs<Count_eat>();

                        Oldexpense_eat = 0; //เซ็ตค่าให้ Oldexpense_eat ให้เป็น 0 ทุกครั้งที่กดปุ่มบันทึก
                        Oldexpense_eat = get_eat.Total_eat; //รับค่าจาก firebase เก็บไว้ใน Oldexpense_eat
                                                            //นำค่่าจาก Total_old บวกกับ เงินที่เราใส่ไป และนำไปเก็บไว้ใน Newexpense_eat
                        Newexpense_eat = Oldexpense_eat + Convert.ToInt32(expense.Expense);

                        var obj_eat2 = new Count_eat
                        {
                            eatexpense_All = obj_eat.eatexpense_All,
                            //eatexpense_All = (Convert.ToInt32(get_eat.eatexpense_All) + 1).ToString(),
                            Total_eat = Newexpense_eat
                        };
                        SetResponse resexpense_eat2 = await client.SetTaskAsync($"Count for eatexpense:{labelmonth.Text}/node", obj_eat2);

                        var obj_eat3 = new start_MonthOutEat
                        {
                            Number = obj_eat2.eatexpense_All
                        };
                        SetResponse node3 = await client.SetTaskAsync($"Start_OutEat:{labelmonth.Text}/", obj_eat3);
                    }
                    catch { }
                }//-----------------------สำหรับเลือกของกิน---------------------------------


                else if (radi_edu.Checked == true)//สำหรับเลือกใช้เป็นของการศึกษา
                {
                    try
                    {
                        //------------------------------------------------
                        FirebaseResponse value_out1 = await client.GetTaskAsync($"Start_OutAll:{labelmonth.Text}/");
                        start_MonthOutAll get_out1 = value_out1.ResultAs<start_MonthOutAll>();

                        var node_out = new Count_Expense
                        {
                            expense_All = (Convert.ToInt32(get_out1.Number) + 1).ToString(),
                            Total_expense = Totalexpense
                        };
                        SetResponse resincome3 = await client.SetTaskAsync($"Count for Totalexpense:{labelmonth.Text}/node", node_out);

                        FirebaseResponse value_out = await client.GetTaskAsync($"Count for Totalexpense:{labelmonth.Text}/node");
                        Count_Expense get_out = value_out.ResultAs<Count_Expense>();
                        var expense = new ExpenseTotal
                        {
                            //รับค่า income-All มาและเมื่อกดปุ่ม นำไปบวกอีก 1 เสมอ จากนั้นก็แปลงให้อยู่ในรูปสตริง
                            Number = node_out.expense_All,
                            Detail = detailtxt.Text,
                            Expense = expensetxt.Text,  // จำนวนเงินของรายรับ 
                            Type = "การศึกษา"
                        };
                        SetResponse resexpense = await client.SetTaskAsync($"Expense:{labelmonth.Text}/order" +
                            expense.Number, expense);
                        ExpenseTotal result = resexpense.ResultAs<ExpenseTotal>(); //เพิ่มเติม

                        Total_old = 0; //เซ็ตค่าให้ Total_old ให้เป็น 0 ทุกครั้งที่กดปุ่มบันทึก
                        Total_old = get_out.Total_expense; //รับค่าจาก firebase เก็บไว้ใน Total_old
                                                           //นำค่่าจาก Total_old บวกกับ เงินที่เราใส่ไป และนำไปเก็บไว้ใน Totalexpense
                        Totalexpense = Total_old + Convert.ToInt32(expense.Expense);

                        var obj_amount = new Count_Expense
                        {
                            expense_All = expense.Number,
                            Total_expense = Totalexpense
                        };
                        SetResponse resincome2 = await client.SetTaskAsync($"Count for Totalexpense:{labelmonth.Text}/node", obj_amount);

                        var obj_num = new start_MonthOutAll
                        {
                            Number = obj_amount.expense_All,
                        };
                        SetResponse node2 = await client.SetTaskAsync($"Start_OutAll:{labelmonth.Text}/", obj_num);
                        //เทียบจาก form2 ถูกถึงตรงนี้


                        //------------------------------------------------------
                        FirebaseResponse value_out2 = await client.GetTaskAsync($"Start_OutEdu:{labelmonth.Text}/");
                        start_MonthOutEdu get_out2 = value_out2.ResultAs<start_MonthOutEdu>();
                        var obj_edu = new Count_edu
                        {
                            eduexpense_All = (Convert.ToInt32(get_out2.Number) + 1).ToString(),
                            Total_edu = Newexpense_edu
                        };
                        SetResponse resexpense_edu = await client.SetTaskAsync($"Count for eduexpense:{labelmonth.Text}/node", obj_edu);

                        FirebaseResponse value_edu = await client.GetTaskAsync($"Count for eduexpense:{labelmonth.Text}/node");
                        Count_edu get_edu = value_edu.ResultAs<Count_edu>();

                        Oldexpense_edu = 0; //เซ็ตค่าให้ Oldexpense_eat ให้เป็น 0 ทุกครั้งที่กดปุ่มบันทึก
                        Oldexpense_edu = get_edu.Total_edu; //รับค่าจาก firebase เก็บไว้ใน Oldexpense_eat
                                                            //นำค่่าจาก Total_old บวกกับ เงินที่เราใส่ไป และนำไปเก็บไว้ใน Newexpense_eat
                        Newexpense_edu = Oldexpense_edu + Convert.ToInt32(expense.Expense);

                        var obj_edu2 = new Count_edu
                        {
                            eduexpense_All = obj_edu.eduexpense_All,
                            Total_edu = Newexpense_edu
                        };
                        SetResponse resexpense_edu2 = await client.SetTaskAsync($"Count for eduexpense:{labelmonth.Text}/node", obj_edu2);

                        var obj_edu3 = new start_MonthOutEdu
                        {
                            Number = obj_edu2.eduexpense_All
                        };
                        SetResponse node3 = await client.SetTaskAsync($"Start_OutEdu:{labelmonth.Text}/", obj_edu3);
                    }
                    catch { }
                }
                else if (radi_ote.Checked == true)//สำหรับเลือกใช้เป็นของจิปาถะ
                {
                    try
                    {
                        //------------------------------------------------
                        FirebaseResponse value_out1 = await client.GetTaskAsync($"Start_OutAll:{labelmonth.Text}/");
                        start_MonthOutAll get_out1 = value_out1.ResultAs<start_MonthOutAll>();

                        var node_out = new Count_Expense
                        {
                            expense_All = (Convert.ToInt32(get_out1.Number) + 1).ToString(),
                            Total_expense = Totalexpense
                        };
                        SetResponse resincome3 = await client.SetTaskAsync($"Count for Totalexpense:{labelmonth.Text}/node", node_out);

                        FirebaseResponse value_out = await client.GetTaskAsync($"Count for Totalexpense:{labelmonth.Text}/node");
                        Count_Expense get_out = value_out.ResultAs<Count_Expense>();
                        var expense = new ExpenseTotal
                        {
                            //รับค่า income-All มาและเมื่อกดปุ่ม นำไปบวกอีก 1 เสมอ จากนั้นก็แปลงให้อยู่ในรูปสตริง
                            Number = node_out.expense_All,
                            Detail = detailtxt.Text,
                            Expense = expensetxt.Text,  // จำนวนเงินของรายรับ 
                            Type = "จิปาถะ"
                        };
                        SetResponse resexpense = await client.SetTaskAsync($"Expense:{labelmonth.Text}/order" +
                            expense.Number, expense);
                        ExpenseTotal result = resexpense.ResultAs<ExpenseTotal>(); //เพิ่มเติม

                        Total_old = 0; //เซ็ตค่าให้ Total_old ให้เป็น 0 ทุกครั้งที่กดปุ่มบันทึก
                        Total_old = get_out.Total_expense; //รับค่าจาก firebase เก็บไว้ใน Total_old
                                                           //นำค่่าจาก Total_old บวกกับ เงินที่เราใส่ไป และนำไปเก็บไว้ใน Totalexpense
                        Totalexpense = Total_old + Convert.ToInt32(expense.Expense);

                        var obj_amount = new Count_Expense
                        {
                            expense_All = expense.Number,
                            Total_expense = Totalexpense
                        };
                        SetResponse resincome2 = await client.SetTaskAsync($"Count for Totalexpense:{labelmonth.Text}/node", obj_amount);

                        var obj_num = new start_MonthOutAll
                        {
                            Number = obj_amount.expense_All,
                        };
                        SetResponse node2 = await client.SetTaskAsync($"Start_OutAll:{labelmonth.Text}/", obj_num);
                        //เทียบจาก form2 ถูกถึงตรงนี้


                        //------------------------------------------------------
                        FirebaseResponse value_out2 = await client.GetTaskAsync($"Start_OutEtc:{labelmonth.Text}/");
                        start_MonthOutEtc get_out2 = value_out2.ResultAs<start_MonthOutEtc>();
                        var obj_etc = new Count_etc
                        {
                            etcexpense_All = (Convert.ToInt32(get_out2.Number) + 1).ToString(),
                            Total_etc = Newexpense_etc
                        };
                        SetResponse resexpense_etc = await client.SetTaskAsync($"Count for etcexpense:{labelmonth.Text}/node", obj_etc);

                        FirebaseResponse value_etc = await client.GetTaskAsync($"Count for etcexpense:{labelmonth.Text}/node");
                        Count_etc get_etc = value_etc.ResultAs<Count_etc>();

                        Oldexpense_etc = 0; //เซ็ตค่าให้ Oldexpense_eat ให้เป็น 0 ทุกครั้งที่กดปุ่มบันทึก
                        Oldexpense_etc = get_etc.Total_etc; //รับค่าจาก firebase เก็บไว้ใน Oldexpense_eat
                                                            //นำค่่าจาก Total_old บวกกับ เงินที่เราใส่ไป และนำไปเก็บไว้ใน Newexpense_eat
                        Newexpense_etc = Oldexpense_etc + Convert.ToInt32(expense.Expense);

                        var obj_etc2 = new Count_etc
                        {
                            etcexpense_All = obj_etc.etcexpense_All,
                            Total_etc = Newexpense_etc
                        };
                        SetResponse resexpense_eat2 = await client.SetTaskAsync($"Count for etcexpense:{labelmonth.Text}/node", obj_etc2);

                        var obj_etc3 = new start_MonthOutEtc
                        {
                            Number = obj_etc2.etcexpense_All
                        };
                        SetResponse node3 = await client.SetTaskAsync($"Start_OutEtc:{labelmonth.Text}/", obj_etc3);
                    }
                    catch { }
                }
                MessageBox.Show("บันทึกเรียบร้อย");
            }
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
                string Value_type = null;
                FirebaseResponse check_expense = await client.GetTaskAsync($"Expense:{labelmonth.Text}/order" +
                   Numbertxt.Text);
                ExpenseTotal check = check_expense.ResultAs<ExpenseTotal>();
                //------------------------------------
                if (DelectEat.Checked == true)
                    Value_type = "อุปโภคบริโภค";
                else if (DeleteEdu.Checked == true)
                    Value_type = "การศึกษา";
                else if (DeleteEtc.Checked == true)
                    Value_type = "จิปาถะ";
                if (Value_type != check.Type)
                {
                    MessageBox.Show("คุณกรอกลำดับไม่ถูกต้อง กรุณากรอกใหม่", "เกิดข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                else
                {
                    btnDelete.Enabled = false;
                    FirebaseResponse value_out = await client.GetTaskAsync($"Count for Totalexpense:{labelmonth.Text}/node");
                    Count_Expense get_out = value_out.ResultAs<Count_Expense>();


                    var expense = new ExpenseTotal
                    {
                        //รับค่า income-All มาและเมื่อกดปุ่ม นำไปลบอีก 1 เสมอ จากนั้นก็แปลงให้อยู่ในรูปสตริง
                        Number = (Convert.ToInt32(get_out.expense_All) - 1).ToString(),
                        Detail = detailtxt.Text,
                        Expense = expensetxt.Text,  // จำนวนเงินของรายรับ

                    };
                    ////สำหรับเรียกค่ารายจ่ายและลำดับโดยการกำหนด
                    FirebaseResponse resexpense = await client.GetTaskAsync($"Expense:{labelmonth.Text}/order" +
                        Numbertxt.Text);

                    ExpenseTotal result = resexpense.ResultAs<ExpenseTotal>(); //เพิ่มเติม
                    UpdateExpense_delete();

                    Total_old = 0; //เซ็ตค่าให้ Total_old ให้เป็น 0 ทุกครั้งที่กดปุ่มบันทึก
                    Total_old = get_out.Total_expense; //รับค่าจาก firebase เก็บไว้ใน Total_old
                                                       //นำค่่าจาก Total_old บวกกับ เงินที่เราใส่ไป และนำไปเก็บไว้ใน Totalexpense
                    Totalexpense = Total_old - Convert.ToInt32(result.Expense);
                    var obj_amount = new Count_Expense
                    {
                        expense_All = expense.Number,
                        Total_expense = Totalexpense
                    };
                    SetResponse resincome2 = await client.SetTaskAsync($"Count for Totalexpense:{labelmonth.Text}/node", obj_amount);
                    //--------------------สำหรับเรียกรายจ่ายทั้งหมด-----------------------------                          

                    var obj_start1 = new start_MonthOutAll
                    {
                        //รับค่า income-All มาและเมื่อกดปุ่ม นำไปบวกอีก -1 เสมอ จากนั้นก็แปลงให้อยู่ในรูปสตริง
                        Number = obj_amount.expense_All

                    };
                    //สำหรับเรียกค่ารายลับและลำดับโดยการกำหนด$"Start_in:{labelmonth.Text}/"
                    SetResponse start1 = await client.SetTaskAsync($"Start_OutAll:{labelmonth.Text}/", obj_start1);

                    //----------------------------------------------------------
                    if (DelectEat.Checked == true)
                    {
                        //--------------------สำหรับรายจ่ายของกินทั้งหมด-----------------------------   
                        ////สำหรับเรียกค่า expense และ  Number เก่าใน firebase
                        FirebaseResponse value2_out = await client.GetTaskAsync($"Count for eatexpense:{labelmonth.Text}/node");
                        Count_eat get2_out = value2_out.ResultAs<Count_eat>();

                        Total_Eatold = 0; //เซ็ตค่าให้ Total_old ให้เป็น 0 ทุกครั้งที่กดปุ่มบันทึก
                        Total_Eatold = get2_out.Total_eat; //รับค่าจาก firebase เก็บไว้ใน Total_old
                                                           //นำค่่าจาก Total_old บวกกับ เงินที่เราใส่ไป และนำไปเก็บไว้ใน Totalexpense
                        Totaleat = Total_Eatold - Convert.ToInt32(result.Expense);

                        var obj_eat = new Count_eat
                        {
                            eatexpense_All = (Convert.ToInt32(get2_out.eatexpense_All) - 1).ToString(),
                            Total_eat = Totaleat
                        };


                        SetResponse res_eat2 = await client.SetTaskAsync($"Count for eatexpense:{labelmonth.Text}/node", obj_eat);
                        //สำหรับลบค่า โดยที่เรากำหนดลำดับว่าจะลบลำดับที่เท่าไหร่  

                        var obj_start2 = new start_MonthOutEat
                        {
                            //รับค่า income-All มาและเมื่อกดปุ่ม นำไปบวกอีก -1 เสมอ จากนั้นก็แปลงให้อยู่ในรูปสตริง
                            Number = obj_eat.eatexpense_All

                        };
                        //สำหรับเรียกค่ารายลับและลำดับโดยการกำหนด$"Start_in:{labelmonth.Text}/"
                        SetResponse start2 = await client.SetTaskAsync($"Start_OutEat:{labelmonth.Text}/", obj_start2);
                        MessageBox.Show("แก้ไขเรียบร้อย");
                    }
                    else if (DeleteEdu.Checked == true)
                    {
                        //--------------------สำหรับรายจ่ายการศึกษาทั้งหมด-----------------------------
                        ////สำหรับเรียกค่า expense และ  Number เก่าใน firebase
                        FirebaseResponse value3_out = await client.GetTaskAsync($"Count for eduexpense:{labelmonth.Text}/node");
                        Count_edu get3_out = value3_out.ResultAs<Count_edu>();

                        Total_Eduold = 0; //เซ็ตค่าให้ Total_old ให้เป็น 0 ทุกครั้งที่กดปุ่มบันทึก
                        Total_Eduold = get3_out.Total_edu; //รับค่าจาก firebase เก็บไว้ใน Total_old
                                                           //นำค่่าจาก Total_old บวกกับ เงินที่เราใส่ไป และนำไปเก็บไว้ใน Totalexpense
                        Totaledu = Total_Eduold - Convert.ToInt32(result.Expense);

                        var obj_edu = new Count_edu
                        {
                            eduexpense_All = (Convert.ToInt32(get3_out.eduexpense_All) - 1).ToString(),
                            Total_edu = Totaledu
                        };
                        SetResponse res_edu2 = await client.SetTaskAsync($"Count for eduexpense:{labelmonth.Text}/node", obj_edu);

                        //สำหรับลบค่า โดยที่เรากำหนดลำดับว่าจะลบลำดับที่เท่าไหร่
                        MessageBox.Show("แก้ไขเรียบร้อย");
                    }

                    else if (DeleteEtc.Checked == true)
                    {
                        //--------------------สำหรับรายจ่ายจิปาถะทั้งหมด-----------------------------
                        ////สำหรับเรียกค่า expense และ  Number เก่าใน firebase
                        FirebaseResponse value3_out = await client.GetTaskAsync($"Count for etcexpense:{labelmonth.Text}/node");
                        Count_etc get3_out = value3_out.ResultAs<Count_etc>();

                        Total_Etcold = 0; //เซ็ตค่าให้ total_old ให้เป็น 0 ทุกครั้งที่กดปุ่มบันทึก
                        Total_Etcold = get3_out.Total_etc; //รับค่าจาก firebase เก็บไว้ใน total_old
                                                           //นำค่่าจาก total_old บวกกับ เงินที่เราใส่ไป และนำไปเก็บไว้ใน totalexpense
                        Totaletc = Total_Etcold - Convert.ToInt32(result.Expense);

                        var obj_etc = new Count_etc
                        {
                            etcexpense_All = (Convert.ToInt32(get3_out.etcexpense_All) - 1).ToString(),
                            Total_etc = Totaletc
                        };
                        SetResponse res_etc2 = await client.SetTaskAsync($"Count for etcexpense:{labelmonth.Text}/node", obj_etc);

                        //สำหรับลบค่า โดยที่เรากำหนดลำดับว่าจะลบลำดับที่เท่าไหร่    
                        MessageBox.Show("แก้ไขเรียบร้อย");
                    }
                }
            }
        }

        private async void expoer_expentBtn_Click(object sender, EventArgs e)
        {
            try
            {
                timer1.Interval = 1000;
                timer1.Start();
                expoer_expentBtn.Enabled = false;
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
                FirebaseResponse value_income = await client.GetTaskAsync($"Count for Income:{labelmonth.Text}/node");
                Count_Income get_in = value_income.ResultAs<Count_Income>();

                //ส่วนของยอดคงเหลือ
                label_MonthofExportBalance.Text = labelmonth.Text;
                label_monthAllBalance.Text = Convert.ToString(get_in.Total_income - get_out.Total_expense);

                chart1.Titles.Add("สรุปรายรับรายจ่าย");
                chart1.Series["s1"].IsValueShownAsLabel = true;
                chart1.Series["s1"].Points.AddXY("1", get_eat.Total_eat);
                chart1.Series["s1"].Points.AddXY("2", get_edu.Total_edu);
                chart1.Series["s1"].Points.AddXY("3", get_etc.Total_etc);
                chart1.Series["s1"].Points.AddXY("4", get_in.Total_income);

                label_MonthofExport.Text = labelmonth.Text;
                int i = 0;
                FirebaseResponse resex_out1 = await client.GetTaskAsync($"Count for Totalexpense:{labelmonth.Text}/node");
                Count_Expense objc_out1 = resex_out1.ResultAs<Count_Expense>();
                int expense_All = Convert.ToInt32(objc_out1.expense_All);
                label_monthAll.Text = Convert.ToString(objc_out1.Total_expense);

                while (true)
                {
                    i++;
                    try
                    {
                        FirebaseResponse resex_out2 = await client.GetTaskAsync($"Expense:{labelmonth.Text}/order" + i);
                        ExpenseTotal objc_out2 = resex_out2.ResultAs<ExpenseTotal>();

                        DataRow row = tb.NewRow();
                        row["ลำดับที่"] = objc_out2.Number;
                        row["รายการ"] = objc_out2.Detail;
                        row["ราคา"] = objc_out2.Expense;
                        row["ประเภท"] = objc_out2.Type;
                        tb.Rows.Add(row);
                    }
                    catch
                    {

                    }
                }
            }
            catch { MessageBox.Show($"ข้อมูลไม่ถูกต้องหรือไม่มีข้อมูลเดือน{labelmonth.Text}ในฐานระบบ"); }
        }
        
        private async void UpdateExpense_delete()
        {
            FirebaseResponse value_out = await client.GetTaskAsync($"Count for Totalexpense:{labelmonth.Text}/node");
            Count_Expense get_out = value_out.ResultAs<Count_Expense>();
            var expense = new ExpenseTotal
            {
                //รับค่า income-All มาและเมื่อกดปุ่ม นำไปลบอีก 1 เสมอ จากนั้นก็แปลงให้อยู่ในรูปสตริง
                Number = (Convert.ToInt32(get_out.expense_All) - 1).ToString(),
                Detail = detailtxt.Text,
                Expense = expensetxt.Text,  // จำนวนเงินของรายรับ                                           
            };
            var obj_amount = new Count_Expense
            {
                expense_All = expense.Number,
                Total_expense = Totalexpense
            };
            int i = Convert.ToInt32(Numbertxt.Text);
            while (i <= Convert.ToInt32(obj_amount.expense_All) + 1)
            {
                try
                {
                    FirebaseResponse response_loop = await client.GetTaskAsync($"Expense:{labelmonth.Text}/order" + (i + 1));
                    ExpenseTotal obj_loop = response_loop.ResultAs<ExpenseTotal>();
                    var expense_loop = new ExpenseTotal
                    {
                        Number = Convert.ToString(i),
                        Detail = obj_loop.Detail, // รายการที่ได้รับ 
                        Expense = obj_loop.Expense,  // จำนวนเงินของรายรับ   
                        Type = obj_loop.Type
                    };
                    SetResponse resincome3 = await client.SetTaskAsync($"Expense:{labelmonth.Text}/order" + i, expense_loop);
                }
                catch { }                
                i++;
            }
            FirebaseResponse response2 = await client.DeleteTaskAsync($"Expense:{labelmonth.Text}/order" +
               (Convert.ToInt32(obj_amount.expense_All) + 1).ToString());
        }
        private async void btnDeleteAll_expense_Click(object sender, EventArgs e)
        {
            FirebaseResponse delete1 = await client.DeleteTaskAsync($"Count for Totalexpense:{labelmonth.Text}");
            FirebaseResponse delete2 = await client.DeleteTaskAsync($"Count for eatexpense:{labelmonth.Text}");
            FirebaseResponse delete3 = await client.DeleteTaskAsync($"Count for eduexpense:{labelmonth.Text}");
            FirebaseResponse delete4 = await client.DeleteTaskAsync($"Count for etcexpense:{labelmonth.Text}");
            FirebaseResponse delete5 = await client.DeleteTaskAsync($"Expense:{labelmonth.Text}");
            var node_out1 = new start_MonthOutAll
            {
                Number = Convert.ToString(0),
            };
            SetResponse node1 = await client.SetTaskAsync($"Start_OutAll:{labelmonth.Text}", node_out1);
            var node_out2 = new start_MonthOutEat
            {
                Number = Convert.ToString(0),
            };
            SetResponse node2 = await client.SetTaskAsync($"Start_OutEat:{labelmonth.Text}", node_out2);
            var node_out3 = new start_MonthOutEat
            {
                Number = Convert.ToString(0),
            };
            SetResponse node3 = await client.SetTaskAsync($"Start_OutEdu:{labelmonth.Text}", node_out3);
            var node_out4 = new start_MonthOutEat
            {
                Number = Convert.ToString(0),
            };
            SetResponse node4 = await client.SetTaskAsync($"Start_OutEtc:{labelmonth.Text}", node_out4);
            Totalexpense = 0;
            Newexpense_eat = 0;
            Newexpense_edu = 0;
            Newexpense_etc = 0;
        }
    }
    class ExpenseTotal
    {
        public string Number { get; set; }
        public string Detail { get; set; }
        public string Expense { get; set; }
        public string Type { get; set; }
    }   
    class Count_Expense
    {
        public string expense_All { get; set; }
        public int Total_expense { get; set; }
    }
    public class Count_eat
    {
        public string eatexpense_All { get; set; }
        public int Total_eat { get; set; }
    }
    class Count_etc
    {
        public string etcexpense_All { get; set; }
        public int Total_etc { get; set; }
    }
    class Count_edu
    {
        public string eduexpense_All { get; set; }
        public int Total_edu { get; set; }
    }
    
}