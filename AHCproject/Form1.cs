using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.OleDb;


namespace AHCproject
{
    public partial class Form1 : Form
    {
        private SqlConnection connString = new SqlConnection("Data Source=localhost\\SQLEXPRESS;Initial Catalog=ahcDB;Integrated Security=True");
        string connString1 = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=ahcDB;Integrated Security=True";
        string connString2 = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=ahcDB;Integrated Security=True";



        private SqlDataAdapter da = new SqlDataAdapter();
        private DataTable dt = null;

        private SqlDataAdapter da1 = new SqlDataAdapter();
        private DataTable dt1 = null;

        private SqlDataAdapter da2 = new SqlDataAdapter();
        private DataTable dt2 = null;

        private SqlDataAdapter da3 = new SqlDataAdapter();
        private DataTable dt3 = null;

        private SqlDataAdapter da4 = new SqlDataAdapter();
        private DataTable dt4 = null;

        private SqlCommandBuilder sqlCommandBuilder = null;
        private SqlCommandBuilder sqlCommandBuilder1 = null;
        private SqlCommandBuilder sqlCommandBuilder3 = null;
        private SqlCommandBuilder sqlCommandBuilder2 = null;
        private SqlCommandBuilder sqlCommandBuilder4 = null;


        private BindingSource bindingSource = null;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                SqlConnection conn = new SqlConnection(connString1);
                SqlCommand command1 = conn.CreateCommand();
                SqlDataReader reader1;
                SqlCommand command2 = conn.CreateCommand();
                SqlDataReader reader2;

                ////////////////////////////////////////////////////////
                command1.CommandText = "SELECT Ticker FROM dataE group by Ticker ";
                conn.Open();
                reader1 = command1.ExecuteReader();
                comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                comboBox1.AutoCompleteSource = AutoCompleteSource.ListItems;
                while (reader1.Read())
                { comboBox1.Items.Add(reader1.GetString(0)); }
                conn.Close();

                //////////////////////////////////////////////////////////

                
                //////////////////////////////////////////////////////////

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                //string excelConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\AHC\exp.xls;Extended Properties=""Excel 8.0;HDR=YES;""";

                ////string excelConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\AHC\Details.xls;Extended Properties=""Excel 8.0;HDR=YES;""";

                string provider1 = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
                string brows1 = textBox1.Text;
                string extended1 = @";Extended Properties=""Excel 8.0;HDR=YES;""";

                string excelConnectionString = provider1 + brows1 + extended1;


                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);

                OleDbCommand cmd = new OleDbCommand("Select Ticker,Address,State,Zip,Spaces from [inbase$] ", excelConnection);


                excelConnection.Open();
                OleDbDataReader dReader;
                dReader = cmd.ExecuteReader();

                SqlBulkCopy sqlBulk = new SqlBulkCopy(connString1);
                sqlBulk.DestinationTableName = "Details";

                sqlBulk.WriteToServer(dReader);
                MessageBox.Show("Done");
            }
            catch (Exception)
            {
                MessageBox.Show("Error on Button 1 click...");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filename = openFileDialog1.FileName.ToString();
                textBox1.Text = filename;

            }
            Console.WriteLine(result);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                connString.Open();
                da = new SqlDataAdapter("select Address from Details", connString);
                sqlCommandBuilder = new SqlCommandBuilder(da);
                dt = new DataTable();
                da.Fill(dt);
                connString.Close();

                connString.Open();
                da1 = new SqlDataAdapter("select Address from dataE", connString);
                sqlCommandBuilder1 = new SqlCommandBuilder(da1);
                dt1 = new DataTable();
                da1.Fill(dt1);
                connString.Close();



                foreach (DataRow row1 in dt1.Rows)
                {
                    foreach (var Addr1 in row1.ItemArray)
                    {

                        cal(Addr1.ToString());

                    }

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error on Button 2 click...");
                MessageBox.Show(ex.ToString());
            }



        }

        public void cal(string aa)
        {

            try
            {
                //////////////select from  Details
                string addd = aa;

                connString.Open();
                da2 = new SqlDataAdapter("select Address,Spaces from Details where Address = @add", connString);
                da2.SelectCommand.Parameters.Add("@add", SqlDbType.VarChar).Value = addd;
                sqlCommandBuilder1 = new SqlCommandBuilder(da2);
                dt2 = new DataTable();
                da2.Fill(dt2);
                connString.Close();

                int spaces1 = (from DataRow dr in dt2.Rows
                               where (string)dr["Address"] == addd
                               select (int)dr["Spaces"]).FirstOrDefault();

                //////////////select from  Details
                /////////////////////select from dataE

                connString.Open();
                da4 = new SqlDataAdapter("select Ticker,Address,State,Zip,cars,Spaces from dataE where Address = @add1", connString);
                da4.SelectCommand.Parameters.Add("@add1", SqlDbType.VarChar).Value = addd;
                sqlCommandBuilder4 = new SqlCommandBuilder(da4);
                dt4 = new DataTable();
                da4.Fill(dt4);
                connString.Close();

                string ticker12 = (from DataRow dr1 in dt4.Rows
                                   where (string)dr1["Address"] == addd
                                   select (string)dr1["Ticker"]).FirstOrDefault();


                string state12 = (from DataRow dr1 in dt4.Rows
                                  where (string)dr1["Address"] == addd
                                  select (string)dr1["State"]).FirstOrDefault();

                string zip12 = (from DataRow dr1 in dt4.Rows
                                where (string)dr1["Address"] == addd
                                select (string)dr1["Zip"]).FirstOrDefault();

                int cars12 = (from DataRow dr1 in dt4.Rows
                              where (string)dr1["Address"] == addd
                              select (int)dr1["cars"]).FirstOrDefault();

                int spaces12 = (from DataRow dr1 in dt4.Rows
                                where (string)dr1["Address"] == addd
                                select (int)dr1["Spaces"]).FirstOrDefault();



                ////////////////////select from dataE



                if (spaces1 == 0)
                {

                    //////////inserting to Details when unrecognized add 
                    MessageBox.Show(addd, "This Address is not in the DataBase.. Auto Insert");


                    da4.InsertCommand = new SqlCommand("insert into Details(Ticker,Address,State,Zip,Spaces)values (@Ticker,@Address,@State,@Zip,@Spaces)", connString);

                    da4.InsertCommand.Parameters.Add("@Ticker", SqlDbType.VarChar).Value = ticker12;
                    da4.InsertCommand.Parameters.Add("@Address", SqlDbType.VarChar).Value = addd;
                    da4.InsertCommand.Parameters.Add("@State", SqlDbType.VarChar).Value = state12;
                    da4.InsertCommand.Parameters.Add("@Zip", SqlDbType.VarChar).Value = zip12;
                    da4.InsertCommand.Parameters.Add("@Spaces", SqlDbType.VarChar).Value = spaces12;

                    connString.Open();
                    da4.InsertCommand.ExecuteNonQuery();
                    connString.Close();

                    MessageBox.Show("Done inserting....");

                    //////////inserting to Details when unrecognized add 

                }
                //MessageBox.Show(addd,spaces1.ToString());                
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error on Cal method......");
                MessageBox.Show(ex.ToString());
            }



        }




        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                //string excelConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\AHC\exp.xls;Extended Properties=""Excel 8.0;HDR=YES;""";
                string provider1 = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
                string brows1 = textBox1.Text;
                string extended1 = @";Extended Properties=""Excel 8.0;HDR=YES;""";
                string excelConnectionString = provider1 + brows1 + extended1;


                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);

                OleDbCommand cmd = new OleDbCommand("Select Ticker,Address,State,Zip,Time,Month,Day,Year,cars,Spaces from [outbase$] ", excelConnection);

                excelConnection.Open();
                OleDbDataReader dReader;
                dReader = cmd.ExecuteReader();

                SqlBulkCopy sqlBulk = new SqlBulkCopy(connString1);

                sqlBulk.DestinationTableName = "dataE";

                sqlBulk.WriteToServer(dReader);

                MessageBox.Show("Done");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection conn = new SqlConnection(connString1);
                SqlCommand command1 = conn.CreateCommand();
                SqlDataReader reader1;
                SqlCommand command2 = conn.CreateCommand();
                SqlDataReader reader2;
                SqlCommand command3 = conn.CreateCommand();
                SqlDataReader reader3;

                ////////////////////////////////////////////////
                string tickk = comboBox1.SelectedItem.ToString();
                ////////////////////////////////////////////////
                command1.CommandText = "SELECT MAX(Spaces),MIN(Spaces) FROM dataE where Ticker = '" + tickk + "'  ";
                conn.Open();
                reader1 = command1.ExecuteReader();
                reader1.Read();
                int spaces_max = reader1.GetInt32(0);
                int spaces_min = reader1.GetInt32(1);
                label7.Text = spaces_min.ToString();
                label8.Text = spaces_max.ToString();
                conn.Close();
                ////////////////////////////////////////////////


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {

            string datetime1 = dateTimePicker1.Value.ToShortDateString();
            int subday = dateTimePicker1.Value.Day;
            int submonth = dateTimePicker1.Value.Month;
            int subyear = dateTimePicker1.Value.Year;
            MessageBox.Show(subday.ToString(), subyear.ToString());




        }

        private void button8_Click(object sender, EventArgs e)
        {



            try
            {
                SqlConnection conn = new SqlConnection(connString1);
                SqlCommand command1 = conn.CreateCommand();
                SqlDataReader reader1;
                SqlCommand command2 = conn.CreateCommand();
                SqlDataReader reader2;

                string tick = comboBox1.SelectedItem.ToString();

                string datetime1 = dateTimePicker1.Value.ToShortDateString();
                int subday1 = dateTimePicker1.Value.Day;
                int submonth1 = dateTimePicker1.Value.Month;
                int subyear1 = dateTimePicker1.Value.Year;


                string datetime2 = dateTimePicker2.Value.ToShortDateString();
                int subday2 = dateTimePicker2.Value.Day;
                int submonth2 = dateTimePicker2.Value.Month;
                int subyear2 = dateTimePicker2.Value.Year;

                ///////////////selecting min max///////////////////////////////////

                int min = Int32.Parse(textBox12.Text);
                int max = Int32.Parse(textBox13.Text);


                ////////////////////////selecting min max////////////////////////////

                ////////Percentage of Total Cars
                command2.CommandText = "SELECT SUM(cars)FROM dataE where Month = '" + submonth1.ToString() + "' AND Day >= '" + subday1.ToString() + "' AND Year = '" + subyear1.ToString() + "' AND Month = '" + submonth2.ToString() + "' AND Day <= '" + subday2.ToString() + "' AND Year = '" + subyear2.ToString() + "' AND Spaces >= " + min + " AND Spaces <= " + max + "";
                conn.Open();
                reader2 = command2.ExecuteReader();
                reader2.Read();
                int sumcars1 = reader2.GetInt32(0);
                conn.Close();
                ////////Percentage of Total Cars



                /////command1.CommandText = "SELECT SUM(cars),SUM(Spaces),COUNT(Ticker),SUM(Spaces) FROM dataE where finaldate >= '" + datetime1 + "' AND finaldate <= '" + datetime2 + "' AND Ticker = '" + tick + "' AND Spaces >= " + min + " AND Spaces <= " + max + "  ";

                command1.CommandText = "SELECT SUM(cars),SUM(Spaces),COUNT(Ticker),SUM(Spaces) FROM dataE where Month = '" + submonth1.ToString() + "' AND Day >= '" + subday1.ToString() + "' AND Year = '" + subyear1.ToString() + "' AND Month = '" + submonth2.ToString() + "' AND Day <= '" + subday2.ToString() + "' AND Year = '" + subyear2.ToString() + "' AND Ticker = '" + tick + "' AND Spaces >= " + min + " AND Spaces <= " + max + "  ";
                conn.Open();
                reader1 = command1.ExecuteReader();
                reader1.Read();

                int totalcars = reader1.GetInt32(0);
                int totalspaces = reader1.GetInt32(1);
                int n = reader1.GetInt32(2);
                float sumofspaces = reader1.GetInt32(3);

                int y = dateTimePicker1.Value.Year;

                string year11 = textBox14.Text;
                string year22 = textBox15.Text;

                ////////Percentage of Total Cars
                if (y == Int32.Parse(year11))
                {

                    /////////////////AVG SIZE////////////////////////////////
                    //..........Percentage of Parking Lot Fill Rate.....//////
                    int AvgSize = Convert.ToInt32(sumofspaces / n);
                    textBox6.Text = AvgSize.ToString();
                    //..........Percentage of Parking Lot Fill Rate.....//////
                    /////////////////AVG SIZE///////////////////////////////

                    /////////////////  n ////////////////////////////
                    //..........Percentage of Parking Lot Fill Rate.....//////
                    textBox5.Text = n.ToString();
                    textBox19.Text = n.ToString();
                    //..........Percentage of Parking Lot Fill Rate.....//////
                    /////////////////// n  /////////////////////////


                    ////////////////// Year ///////////////////////////////
                    //..........Percentage of Parking Lot Fill Rate.....//////
                    label10.Text = y.ToString();
                    label22.Text = y.ToString();


                    float totalcars1 = float.Parse(totalcars.ToString());
                    float totalspaces1 = float.Parse(totalspaces.ToString());
                    float year1 = (totalcars1 / totalspaces1) * 100;
                    double year2 = Math.Round(year1, 1);
                    textBox4.Text = year2.ToString();
                    //..........Percentage of Parking Lot Fill Rate.....//////

                    ////////Percentage of Total Cars../////
                    float alltickersumcars = float.Parse(sumcars1.ToString());
                    float totalcarsrate = (totalcars / alltickersumcars) * 100;
                    double totalcarsrate12 = Math.Round(totalcarsrate, 1);
                    textBox17.Text = totalcarsrate12.ToString();
                    ////////Percentage of Total Cars//......

                    //////////////////// Year ////////////////////////////

                    textBox18.Text = totalcars.ToString();

                    ////////////////
                    //..........Percentage of Parking Lot Fill Rate.....//////
                    label9.Text = tick;
                    label21.Text = tick;

                    int da = dateTimePicker1.Value.Day;
                    DateTime xy = dateTimePicker1.Value.AddDays(-da + 1);
                    textBox3.Text = xy.ToShortDateString();
                    //..........Percentage of Parking Lot Fill Rate.....//////
                    ////////Percentage of Total Cars//......
                    textBox16.Text = xy.ToShortDateString();
                    ////////Percentage of Total Cars//......
                    ////////////////      
                    conn.Close();

                    calpercentage();


                }
                else if (y == Int32.Parse(year22))
                {
                    /////////////////AVG SIZE////////////////////////////////
                    //..........Percentage of Parking Lot Fill Rate.....//////
                    int AvgSize = Convert.ToInt32(sumofspaces / n);
                    textBox10.Text = AvgSize.ToString();
                    //..........Percentage of Parking Lot Fill Rate.....//////
                    /////////////////AVG SIZE///////////////////////////////

                    /////////////////  n ////////////////////////////
                    //..........Percentage of Parking Lot Fill Rate.....//////
                    textBox9.Text = n.ToString();
                    textBox20.Text = n.ToString();
                    //..........Percentage of Parking Lot Fill Rate.....//////
                    /////////////////// n  /////////////////////////


                    ////////////////// Year ///////////////////////////////
                    //..........Percentage of Parking Lot Fill Rate.....//////
                    label15.Text = y.ToString();
                    label29.Text = y.ToString();
                    float totalcars1 = float.Parse(totalcars.ToString());
                    float totalspaces1 = float.Parse(totalspaces.ToString());
                    float year1 = (totalcars1 / totalspaces1) * 100;
                    double year2 = Math.Round(year1, 1);
                    textBox8.Text = year2.ToString();
                    //..........Percentage of Parking Lot Fill Rate.....//////

                    ////////Percentage of Total Cars../////
                    float alltickersumcars = float.Parse(sumcars1.ToString());
                    float totalcarsrate = (totalcars / alltickersumcars) * 100;
                    double totalcarsrate12 = Math.Round(totalcarsrate, 1);
                    textBox22.Text = totalcarsrate12.ToString();
                    ////////Percentage of Total Cars//......

                    textBox23.Text = year2.ToString();



                    //////////////////// Year ////////////////////////////
                    ////////////////
                    //..........Percentage of Parking Lot Fill Rate.....//////
                    label6.Text = tick;
                    label30.Text = tick;
                    int da = dateTimePicker1.Value.Day;
                    DateTime xy = dateTimePicker1.Value.AddDays(-da + 1);
                    textBox7.Text = xy.ToShortDateString();

                    ////////Percentage of Total Cars//......
                    textBox23.Text = xy.ToShortDateString();
                    ////////Percentage of Total Cars//......
                    ////////////////      
                    textBox21.Text = totalcars.ToString();

                    conn.Close();

                    //..........Percentage of Parking Lot Fill Rate.....//////
                    float change1 = (((float.Parse(textBox8.Text) - float.Parse(textBox4.Text)) / float.Parse(textBox4.Text)) * 100);

                    double cha2 = Math.Round(change1, 1);
                    textBox11.Text = cha2.ToString();
                    //..........Percentage of Parking Lot Fill Rate.....//////

                    ////////Percentage of Total Cars//......
                    float change12 = (((float.Parse(textBox22.Text) - float.Parse(textBox17.Text)) / float.Parse(textBox17.Text)) * 100);

                    double cha22 = Math.Round(change12, 1);
                    textBox24.Text = cha22.ToString();
                    ////////Percentage of Total Cars//......

                    calpercentage1();


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }





        }


        public void calpercentage()
        {

            ///////////////BIG//////////////////////////
            SqlConnection conn = new SqlConnection(connString1);
            SqlConnection conn1 = new SqlConnection(connString2);
            SqlCommand command1 = conn.CreateCommand();
            SqlDataReader reader1;

            SqlCommand command2 = conn1.CreateCommand();
            SqlDataReader reader2;


            string tick = comboBox1.SelectedItem.ToString();
            /////////////////////
            string datetime1 = dateTimePicker1.Value.ToShortDateString();
            int subday1 = dateTimePicker1.Value.Day;
            int submonth1 = dateTimePicker1.Value.Month;
            int subyear1 = dateTimePicker1.Value.Year;
            //..............
            string datetime2 = dateTimePicker2.Value.ToShortDateString();
            int subday2 = dateTimePicker2.Value.Day;
            int submonth2 = dateTimePicker2.Value.Month;
            int subyear2 = dateTimePicker2.Value.Year;
            ////////////////////////////////////////////////////////
            int min = Int32.Parse(textBox12.Text);
            int max = Int32.Parse(textBox13.Text);
            //...............
            DataTable table = new DataTable();
            table.Columns.Add("State", typeof(string));
            table.Columns.Add("Percentage", typeof(double));
            //..........
            int y = dateTimePicker1.Value.Year;
            string year11 = textBox14.Text;
            string year22 = textBox15.Text;
            /////.................

            int sumcars12;
            int sumspaces12;
            string state22 = "";


            command1.CommandText = "SELECT State FROM dataE where Ticker = '" + tick + "' AND Month = '" + submonth1.ToString() + "' AND Day >= '" + subday1.ToString() + "' AND Year = '" + subyear1.ToString() + "' AND Month = '" + submonth2.ToString() + "' AND Day <= '" + subday2.ToString() + "' AND Year = '" + subyear2.ToString() + "' AND Spaces >= " + min + " AND Spaces <= " + max + " GROUP BY State";
            conn.Open();
            reader1 = command1.ExecuteReader();

            while (reader1.Read())
            {

                state22 = reader1.GetString(0);

                command2.CommandText = "SELECT SUM(cars),SUM(Spaces) FROM dataE where Ticker = '" + tick + "' AND Month = '" + submonth1.ToString() + "' AND Day >= '" + subday1.ToString() + "' AND Year = '" + subyear1.ToString() + "' AND Month = '" + submonth2.ToString() + "' AND Day <= '" + subday2.ToString() + "' AND Year = '" + subyear2.ToString() + "' AND Spaces >= " + min + " AND Spaces <= " + max + " AND State = '" + state22 + "'  ";
                conn1.Open();
                reader2 = command2.ExecuteReader();
                reader2.Read();

                sumcars12 = reader2.GetInt32(0);
                sumspaces12 = reader2.GetInt32(1);
                /////////...............

                float Percentageofstate = (float.Parse(sumcars12.ToString()) / float.Parse(sumspaces12.ToString())) * 100;

                double finalpecentage = Math.Round(Percentageofstate, 1);
                conn1.Close();

                table.Rows.Add(state22, finalpecentage);


                bindingSource = new BindingSource();
                bindingSource.DataSource = table;

               
                    dataGridView1.DataSource = bindingSource;               
                

            }
            label35.Text = tick;

            conn.Close();
            ///////////////BIG////////////////////////// 

        }


        public void calpercentage1()
        {

            ///////////////BIG//////////////////////////
            SqlConnection conn = new SqlConnection(connString1);
            SqlConnection conn1 = new SqlConnection(connString2);
            SqlCommand command1 = conn.CreateCommand();
            SqlDataReader reader1;

            SqlCommand command2 = conn1.CreateCommand();
            SqlDataReader reader2;


            string tick = comboBox1.SelectedItem.ToString();
            /////////////////////
            string datetime1 = dateTimePicker1.Value.ToShortDateString();
            int subday1 = dateTimePicker1.Value.Day;
            int submonth1 = dateTimePicker1.Value.Month;
            int subyear1 = dateTimePicker1.Value.Year;
            //..............
            string datetime2 = dateTimePicker2.Value.ToShortDateString();
            int subday2 = dateTimePicker2.Value.Day;
            int submonth2 = dateTimePicker2.Value.Month;
            int subyear2 = dateTimePicker2.Value.Year;
            ////////////////////////////////////////////////////////
            int min = Int32.Parse(textBox12.Text);
            int max = Int32.Parse(textBox13.Text);
            //...............
            DataTable table = new DataTable();
            table.Columns.Add("State", typeof(string));
            table.Columns.Add("Percentage", typeof(double));
            //..........
            int y = dateTimePicker1.Value.Year;
            string year11 = textBox14.Text;
            string year22 = textBox15.Text;
            /////.................

            int sumcars12;
            int sumspaces12;
            string state22 = "";


            command1.CommandText = "SELECT State FROM dataE where Ticker = '" + tick + "' AND Month = '" + submonth1.ToString() + "' AND Day >= '" + subday1.ToString() + "' AND Year = '" + subyear1.ToString() + "' AND Month = '" + submonth2.ToString() + "' AND Day <= '" + subday2.ToString() + "' AND Year = '" + subyear2.ToString() + "' AND Spaces >= " + min + " AND Spaces <= " + max + " GROUP BY State";
            conn.Open();
            reader1 = command1.ExecuteReader();

            while (reader1.Read())
            {

                state22 = reader1.GetString(0);

                command2.CommandText = "SELECT SUM(cars),SUM(Spaces) FROM dataE where Ticker = '" + tick + "' AND Month = '" + submonth1.ToString() + "' AND Day >= '" + subday1.ToString() + "' AND Year = '" + subyear1.ToString() + "' AND Month = '" + submonth2.ToString() + "' AND Day <= '" + subday2.ToString() + "' AND Year = '" + subyear2.ToString() + "' AND Spaces >= " + min + " AND Spaces <= " + max + " AND State = '" + state22 + "' ";
                conn1.Open();
                reader2 = command2.ExecuteReader();
                reader2.Read();

                sumcars12 = reader2.GetInt32(0);
                sumspaces12 = reader2.GetInt32(1);
                /////////...............

                float Percentageofstate = (float.Parse(sumcars12.ToString()) / float.Parse(sumspaces12.ToString())) * 100;

                double finalpecentage = Math.Round(Percentageofstate, 1);
                conn1.Close();

                table.Rows.Add(state22, finalpecentage);


                bindingSource = new BindingSource();
                bindingSource.DataSource = table;


                dataGridView2.DataSource = bindingSource;


            }

            conn.Close();
            label35.Text = tick;
            ///////////////BIG////////////////////////// 

        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SqlConnection conn = new SqlConnection(connString1);
                SqlCommand command1 = conn.CreateCommand();
                SqlDataReader reader1;
                SqlCommand command2 = conn.CreateCommand();
                SqlDataReader reader2;
                SqlCommand command3 = conn.CreateCommand();
                SqlDataReader reader3;

                ////////////////////////////////////////////////
                string tickk = comboBox1.SelectedItem.ToString();

                textBox2.Text = tickk;
                ////////////////////////////////////////////////
                command1.CommandText = "SELECT MAX(Spaces),MIN(Spaces) FROM dataE where Ticker = '" + tickk + "'  ";
                conn.Open();
                reader1 = command1.ExecuteReader();
                reader1.Read();
                int spaces_max = reader1.GetInt32(0);
                int spaces_min = reader1.GetInt32(1);
                label7.Text = spaces_min.ToString();
                label8.Text = spaces_max.ToString();
                conn.Close();
                ////////////////////////////////////////////////


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox8.Visible = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

            groupBox8.Visible = false;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            textBox14.ReadOnly = false;
            textBox15.ReadOnly = false;
        }



        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox14.ReadOnly = true;
                textBox15.ReadOnly = true;
            }
            else
            {
                textBox14.ReadOnly = false;
                textBox15.ReadOnly = false;
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            textBox9.Clear();
            textBox10.Clear();
            textBox11.Clear();
            textBox12.Clear();
            textBox13.Clear();

            textBox16.Clear();
            textBox17.Clear();
            textBox18.Clear();
            textBox19.Clear();
            textBox20.Clear();
            textBox21.Clear();
            textBox22.Clear();
            textBox23.Clear();
            textBox24.Clear();

            label7.Text = "----";
            label8.Text = "----";




        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            //try
            //{
            //    ///////////////BIG//////////////////////////
            //    SqlConnection conn = new SqlConnection(connString1);
            //    SqlConnection conn1 = new SqlConnection(connString2);
            //    SqlCommand command1 = conn.CreateCommand();
            //    SqlDataReader reader1;

            //    SqlCommand command2 = conn1.CreateCommand();
            //    SqlDataReader reader2;


            //    string tick = comboBox1.SelectedItem.ToString();
            //    /////////////////////
            //    string datetime1 = dateTimePicker1.Value.ToShortDateString();
            //    int subday1 = dateTimePicker1.Value.Day;
            //    int submonth1 = dateTimePicker1.Value.Month;
            //    int subyear1 = dateTimePicker1.Value.Year;
            //    //..............
            //    string datetime2 = dateTimePicker2.Value.ToShortDateString();
            //    int subday2 = dateTimePicker2.Value.Day;
            //    int submonth2 = dateTimePicker2.Value.Month;
            //    int subyear2 = dateTimePicker2.Value.Year;
            //    ////////////////////////////////////////////////////////
            //    int min = Int32.Parse(textBox12.Text);
            //    int max = Int32.Parse(textBox13.Text);
            //    //...............
            //    DataTable table = new DataTable();
            //    table.Columns.Add("State", typeof(string));
            //    table.Columns.Add("Percentage", typeof(double));
            //    //..........
            //    int y = dateTimePicker1.Value.Year;
            //    string year11 = textBox14.Text;
            //    string year22 = textBox15.Text;
            //    /////.................

            //    int sumcars12;
            //    int sumspaces12;
            //    string state22 = "";


            //    command1.CommandText = "select State from dataE group by State ";
            //    conn.Open();
            //    reader1 = command1.ExecuteReader();

            //    while (reader1.Read())
            //    {

            //        state22 = reader1.GetString(0);

            //        command2.CommandText = "SELECT SUM(cars),SUM(Spaces) FROM dataE where Ticker = '" + tick + "' AND Month = '" + submonth1.ToString() + "' AND Day >= '" + subday1.ToString() + "' AND Year = '" + subyear1.ToString() + "' AND Month = '" + submonth2.ToString() + "' AND Day <= '" + subday2.ToString() + "' AND Year = '" + subyear2.ToString() + "' AND Spaces >= " + min + " AND Spaces <= " + max + " AND State = '" + state22 + "' ";
            //        conn1.Open();
            //        reader2 = command2.ExecuteReader();
            //        reader2.Read();

            //        sumcars12 = reader2.GetInt32(0);
            //        sumspaces12 = reader2.GetInt32(1);
            //        /////////...............

            //        float Percentageofstate = (float.Parse(sumcars12.ToString()) / float.Parse(sumspaces12.ToString())) * 100;

            //        double finalpecentage = Math.Round(Percentageofstate, 1);
            //        conn1.Close();

            //        table.Rows.Add(state22, finalpecentage);


            //        bindingSource = new BindingSource();
            //        bindingSource.DataSource = table;

            //        if (subyear1.ToString() == year11)
            //        {
            //            dataGridView1.DataSource = bindingSource;
            //        }
            //        else if (subyear1.ToString() == year22)
            //        {
            //            dataGridView2.DataSource = bindingSource;
            //        }

            //    }

            //    conn.Close();
            //    ///////////////BIG////////////////////////// 
    


            //    ////////////////////////////////////////////////////////
            //}
            //catch (Exception)
            //{

            //}
        }
    }
}
