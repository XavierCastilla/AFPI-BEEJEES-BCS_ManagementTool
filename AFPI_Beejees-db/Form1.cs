using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

namespace AFPI_Beejees_db
{
    public partial class LoginForm : MetroFramework.Forms.MetroForm
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2; 
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")] 
        public static extern bool ReleaseCapture(); 


        string connString = "";
        public LoginForm()
        {
            this.TopMost = true;
            this.Focus();
            this.BringToFront();
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None; 
            DoubleBuffered = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }


        public List<string> GetDatabaseList()
        {
            List<string> list = new List<string>();
                                                                                
            try
            {
                string conString = @"Data Source=" + txtSN.Text + ";Initial Catalog=master;User ID=" + txtUN.Text + ";Password=" + txtPW.Text;

                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                     
                    using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases", con))
                    {
                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                list.Add(dr[0].ToString());
                            }
                        }
                    }

                    con.Close();
                }

                return list;

            }
            catch
            {
                try
                {
                    string conString = @"Data Source=" + txtSN.Text + ";Initial Catalog=master;Integrated Security=True";

                    using (SqlConnection con = new SqlConnection(conString))
                    {
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases", con))
                        {
                            using (IDataReader dr = cmd.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    list.Add(dr[0].ToString());
                                }
                            }
                        }

                        con.Close();
                    }

                    return list;

                }
                catch
                {
                    return null;
                }
            }
        }
  
        private void buttonLogin_Click(object sender, EventArgs e)
        //{
        {
            try
            {
                if (checkBox.Checked == true) {
                connString = @"Data Source=" + txtSN.Text + ";Initial Catalog=" + txtDB.Text + ";Integrated Security=true;";
                
                }
                else
                connString = @"Data Source=" + txtSN.Text + ";Initial Catalog=" + txtDB.Text + ";User ID=" + txtUN.Text + ";Password=" + txtPW.Text;

            if (radioButton1.Checked)
                {
                    SqlConnection cnn = new SqlConnection(connString);
                    cnn.Open();
                    var l = new MainProg(connString);
                    l.Closed += (s, args) => this.Close();
                    l.Show();
                    this.Hide();
                    cnn.Close();
                }else
                {
                    SqlConnection cnn = new SqlConnection(connString);
                    cnn.Open();
                    var l = new BCS(connString);
                    l.Closed += (s, args) => this.Close();
                    l.Show();
                    this.Hide();
                    cnn.Close();
                }
           
            }
            catch
            {
               //
                MessageBox.Show("Connecting unsuccessful");
            }
            //    bootWait.Start();
        }
    
        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked) {
            txtDB.Text = "AFPI-BEEJEES-DB";
            txtSN.Text = @"XAVIER\SQLEXPRESS";
            checkBox.Checked = true;
            }
            else
            {
                txtDB.Text = "AFPI.BCS.DB";
                txtSN.Text = @"XAVIER\SQLEXPRESS";
                checkBox.Checked = true;
            }
        }

        private void txtPW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonLogin_Click(this, new EventArgs());
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            txtSN.Text = "172.18.9.69";
            txtDB.Text = "AFPI-BEEJEES-DB";
            txtUN.Text = "NTASA";
            txtPW.Text = "P@ssword1";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            txtSN.Text = "172.17.1.6";
            txtDB.Text = "AFPI.BCS.DB";
            txtUN.Text = "sql_admin";
            txtPW.Text = "P@ssw0rd";
        }
    }
}
