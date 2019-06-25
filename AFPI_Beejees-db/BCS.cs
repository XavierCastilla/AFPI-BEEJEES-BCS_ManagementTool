using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AFPI_Beejees_db
{
    public partial class BCS : MetroFramework.Forms.MetroForm
    { //////////////////////////////////////////
        //RENDER PROPERTIES
        //////////////////////////////////////////
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );
      
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        SqlConnection conn;
        public BCS()
        {
            conn = new SqlConnection(@"Data Source=XAVIER\SQLEXPRESS;Initial Catalog=AFPI.BCS.DB;Integrated Security=True");
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            DoubleBuffered = true;
        }

        public BCS(string con)
        {
            conn = new SqlConnection(con);
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            DoubleBuffered = true;
        }

        public void UpdateALLCOMBOS()
        {
            metroComboBoxCANT.Items.Clear();
            CloseConn();
            using (SqlCommand cmd = new SqlCommand("SELECT CAN from Cards", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        metroComboBoxCANT.Items.Add(reader.GetValue(0).ToString().ToUpper());
                    }
                }
                conn.Close();
            }
        }

        private void BCS_Load(object sender, EventArgs e)
        {
            try
            {
                timePortionDateTimePicker.Format = DateTimePickerFormat.Time;
                timePortionDateTimePicker.ShowUpDown = true;
                dateTimePickerTT.Format = DateTimePickerFormat.Time;
                dateTimePickerTT.ShowUpDown = true;
                titleLB.Text = "Card Requests";
                dgvDELETE.ItemSize = new Size(0, 2);
                dgvDELETE.SizeMode = TabSizeMode.Normal;
                dgvDELETE.SelectedIndex = 0;
                UpdateALLGRIDS();
                UpdateALLCOMBOS();
                this.BringToFront();
                buttonCMENU.BackColor = ColorTranslator.FromHtml("#00AEDB");
                panelMenu.BackColor = ColorTranslator.FromHtml("#00AEDB");
                panelMenu2.BackColor = ColorTranslator.FromHtml("#00AEDB");
                panelMenu2.Visible = false;
            }
            catch
            {
                MessageBox.Show("Database miscommunication");
            }

        }

        private void logo_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void buttonCMENU_Click(object sender, EventArgs e)
        {
            panelMenu.Visible = true;
            panelMenu2.Visible = false;
            buttonCMENU.BackColor = ColorTranslator.FromHtml("#00AEDB");
            buttonAMENU.BackColor = Color.FromArgb(64, 64, 64);
            buttonUMENU.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void buttonAMENU_Click(object sender, EventArgs e)
        {
            panelMenu.Visible = false;
            panelMenu2.Visible = true;
            buttonCMENU.BackColor = Color.FromArgb(64, 64, 64);
            buttonAMENU.BackColor = ColorTranslator.FromHtml("#00AEDB");
            buttonUMENU.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void buttonUMENU_Click(object sender, EventArgs e)
        {
            dgvDELETE.SelectedIndex = 5;
            titleLB.Text = dgvDELETE.TabPages[5].Text;
        }

        private void buttonEXIT_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure?", "Close Application", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
                this.Close();
            AFPI_Beejees_db.LoginForm ap = new LoginForm();
            this.Hide();
            ap.ShowDialog();
        }

        private void buttonm_Click(object sender, EventArgs e)
        {
            dgvDELETE.SelectedIndex = 0;
            titleLB.Text = dgvDELETE.TabPages[0].Text;
        }

        private void buttonBP_Click(object sender, EventArgs e)
        {
            dgvDELETE.SelectedIndex = 1;
            titleLB.Text = dgvDELETE.TabPages[1].Text;
        }

        private void buttonRBF_Click(object sender, EventArgs e)
        {
            dgvDELETE.SelectedIndex = 2;
            titleLB.Text = dgvDELETE.TabPages[2].Text;
        }

        private void buttonRDF_Click(object sender, EventArgs e)
        {
            dgvDELETE.SelectedIndex = 3;
            titleLB.Text = dgvDELETE.TabPages[3].Text;
            
        }

        private void buttonPD_Click(object sender, EventArgs e)
        {
            dgvDELETE.SelectedIndex = 4;
            titleLB.Text = dgvDELETE.TabPages[4].Text;
        }

        private void textBoxPNBP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) &&
           (e.KeyChar != '.') && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        public void UpdateGrid(string cmd, DataGridView dataGrid)
        {
            var select = cmd;
            var dataAdapter = new SqlDataAdapter(select, conn);
            var commandBuilder = new SqlCommandBuilder(dataAdapter);
            var ds = new DataSet();
            dataAdapter.Fill(ds);
            dataGrid.ReadOnly = true;
            dataGrid.DataSource = ds.Tables[0];
        }

        public void UpdateALLGRIDS()
        {
            UpdateGrid("SELECT id, CAN FROM CardRequests", dgvCR);
            UpdateGrid("SELECT * FROM Cards", dgvC);
            UpdateGrid("SELECT * FROM BlacklistRequests", dgvBR);
            UpdateGrid("SELECT * FROM BlacklistedCards", dgvBC);
            UpdateGrid("SELECT * FROM BlacklistedCardRanges", dgvBCR);
            UpdateGrid("SELECT * FROM Transactions", dgvT);
            UpdateGrid("SELECT Transactions.ID, Cards.CAN, Transactions.TransactionAmount, Transactions.TransactionDateTime, Transactions.ParticipantName FROM Transactions LEFT JOIN Cards ON Cards.ID = Transactions.CardRefId", dgvT);
        }
        public void CloseConn()
        {
            if (conn == null || conn.State == ConnectionState.Open)
                conn.Close();
        }
        private void buttonCreateCR_Click(object sender, EventArgs e)
        {
            CloseConn();
            string ftid = textBoxCANcr.Text;
            if (textBoxCANcr.Text == "" || textBoxCANcr.TextLength != 16)
            {
                MessageBox.Show("Invalid Card format (16 digits)");
            }
            else
            {


                object obj2;
                using (SqlCommand cmd3 = new SqlCommand("select COUNT(*) FROM CardRequests WHERE CAN=@pn", conn)) //check if PID is existing
                {
                    conn.Open();
                    cmd3.CommandType = CommandType.Text;
                    cmd3.Parameters.AddWithValue("pn", ftid);
                    obj2 = cmd3.ExecuteScalar();
                    conn.Close();
                }

                if (Convert.ToInt32(obj2) == 0)
                {

                    using (SqlCommand cmd2 = new SqlCommand("INSERT INTO CardRequests VALUES (NEWID(), @fname, @pto);", conn))
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("fname", ftid);
                        cmd2.Parameters.AddWithValue("pto", "1");
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                    }
                    MessageBox.Show("Creation Successful!");
                    textBoxCANcr.Text = "";
                    UpdateALLGRIDS();
                }
                else
                {
                    MessageBox.Show("Card already requested");
                    textBoxCANcr.Text = "";
                }

            }
        }

        private void buttonUP_UA_Click(object sender, EventArgs e)
        {
            CloseConn();
            string ftid = textBoxCANC.Text;
            if (textBoxCANC.Text == "" || textBoxCANC.TextLength != 16)
            {
                MessageBox.Show("Invalid Card format (16 digits)");
            }
            else if (textBoxPBC.Text == "" || CTC.Text == "" || CSC.Text == "")
            {
                MessageBox.Show("Please fill out all fields.");
            }
            else
            {

                object obj2;
                using (SqlCommand cmd3 = new SqlCommand("select COUNT(*) FROM Cards WHERE CAN=@pn", conn)) //check if PID is existing
                {
                    conn.Open();
                    cmd3.CommandType = CommandType.Text;
                    cmd3.Parameters.AddWithValue("pn", ftid);
                    obj2 = cmd3.ExecuteScalar();
                    conn.Close();
                }

                if (Convert.ToInt32(obj2) == 0)
                {
                    DateTime myDate = datePortionDateTimePicker.Value.Date +
                    timePortionDateTimePicker.Value.TimeOfDay;
                    using (SqlCommand cmd2 = new SqlCommand("INSERT INTO Cards VALUES (NEWID(), @fname, @pto, @csdt, @cs, @ct);", conn))
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("fname", ftid);
                        cmd2.Parameters.AddWithValue("pto", textBoxPBC.Text);
                        cmd2.Parameters.AddWithValue("csdt", myDate.ToString());
                        cmd2.Parameters.AddWithValue("cs", CSC.Text);
                        cmd2.Parameters.AddWithValue("ct", CTC.Text);
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                    }
                    MessageBox.Show("Creation Successful!");
                    textBoxCANC.Text = "";
                    textBoxPBC.Text = "";
                    datePortionDateTimePicker.Value = DateTime.Now;
                    timePortionDateTimePicker.Value = DateTime.Now;
                    CTC.SelectedIndex = -1;
                    CSC.SelectedIndex = -1;
                    UpdateALLGRIDS();
                }
                else
                {
                    MessageBox.Show("Card already existing");
                    textBoxCANC.Text = "";
                }

            }
        }

        private void buttonADDBR_Click(object sender, EventArgs e)
        {
            CloseConn();
            string ftid = textBoxCANBR.Text;
            if (textBoxCANBR.Text == "" || textBoxCANBR.TextLength != 16 && textBoxMN.Text == "" || textBoxMN.TextLength != 11 && textBoxMN.TextLength != 13)
            {
                MessageBox.Show("Invalid Card format (16 digits) or Mobile Number (13 digits)");
            }
            else if (textBoxBR.Text == "")
            {
                MessageBox.Show("Please fill out all fields.");
            }
            else
            {
                object obj2;
                using (SqlCommand cmd3 = new SqlCommand("select COUNT(*) FROM BlacklistRequests WHERE CAN=@pn", conn)) //check if PID is existing
                {
                    conn.Open();
                    cmd3.CommandType = CommandType.Text;
                    cmd3.Parameters.AddWithValue("pn", ftid);
                    obj2 = cmd3.ExecuteScalar();
                    conn.Close();
                }

                if (Convert.ToInt32(obj2) == 0)
                {
                    using (SqlCommand cmd2 = new SqlCommand("INSERT INTO BlacklistRequests VALUES (NEWID(), @fname, @pto, @cs);", conn))
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("fname", ftid);
                        cmd2.Parameters.AddWithValue("pto", textBoxBR.Text);
                        cmd2.Parameters.AddWithValue("cs", textBoxMN.Text);
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                    }
                    MessageBox.Show("Creation Successful!");
                    textBoxCANBR.Text = "";
                    textBoxMN.Text = "";
                    textBoxBR.Text = "";
                    UpdateALLGRIDS();
                }
                else
                {
                    MessageBox.Show("Card already existing");
                    textBoxCANBR.Text = "";
                }

            }
        }

        private void buttonCREATEBC_Click(object sender, EventArgs e)
        {
            CloseConn();
            string ftid = textBoxCANBC.Text;
            if (textBoxCANBC.Text == "" || textBoxCANBC.TextLength != 16)
            {
                MessageBox.Show("Invalid Card format (16 digits)");
            }
            else if (metroComboBoxBRCBC.Text == "" || metroComboBoxBDSN.Text == "")
            {
                MessageBox.Show("Please fill out all fields.");
            }
            else
            {
                object obj2;
                using (SqlCommand cmd3 = new SqlCommand("select COUNT(*) FROM BlacklistedCards WHERE CardApplicationNumber=@pn", conn)) //check if PID is existing
                {
                    conn.Open();
                    cmd3.CommandType = CommandType.Text;
                    cmd3.Parameters.AddWithValue("pn", ftid);
                    obj2 = cmd3.ExecuteScalar();
                    conn.Close();
                }

                if (Convert.ToInt32(obj2) == 0)
                {
                    using (SqlCommand cmd2 = new SqlCommand("INSERT INTO BlacklistedCards VALUES (NEWID(), @fname, @pto, @cs);", conn))
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("fname", ftid);
                        cmd2.Parameters.AddWithValue("pto", metroComboBoxBDSN.Text);
                        cmd2.Parameters.AddWithValue("cs", metroComboBoxBRCBC.Text);
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                    }
                    MessageBox.Show("Creation Successful!");
                    textBoxCANBC.Text = "";
                    metroComboBoxBRCBC.SelectedIndex = -1;
                    metroComboBoxBDSN.SelectedIndex = -1;
                    panel3.Refresh();
                    UpdateALLGRIDS();
                }
                else
                {
                    MessageBox.Show("Card already existing");
                    textBoxCANBC.Text = "";
                }
            }
        }

        private void buttonADDBRC_Click(object sender, EventArgs e)
        {
            CloseConn();
            if (textBoxSCANBR.Text == "" || textBoxSCANBR.TextLength != 16 && textBoxECANBR.Text == "" || textBoxECANBR.TextLength != 16)
            {
                MessageBox.Show("Invalid Card format (16 digits)");
            }
            else if (metroComboBoxBRC.Text == "")
            {
                MessageBox.Show("Please fill out all fields.");
            }
            else
            {

                using (SqlCommand cmd2 = new SqlCommand("INSERT INTO BlacklistedCardRanges VALUES (NEWID(), @fname, @pto, @cs);", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("fname", textBoxSCANBR.Text);
                    cmd2.Parameters.AddWithValue("pto", textBoxECANBR.Text);
                    cmd2.Parameters.AddWithValue("cs", metroComboBoxBRC.Text);
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                }
                MessageBox.Show("Creation Successful!");
                textBoxSCANBR.Text = "";
                textBoxECANBR.Text = "";
                metroComboBoxBRC.SelectedIndex = -1;
                UpdateALLGRIDS();

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string act = "";
            CloseConn();
            if (metroComboBoxCANT.Text == "" || metroComboBoxPNT.Text == "" || textBoxTAT.Text == "")
            {
                MessageBox.Show("Please fill out all fields.");
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ID FROM Cards WHERE CAN=@pid", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("pid", metroComboBoxCANT.Text);
                        using (var reader2 = cmd.ExecuteReader())
                        {
                            reader2.Read();
                            act = (reader2[0].ToString());
                        }
                        CloseConn();

                    }
                    catch { }
                }
                DateTime myDate = dateTimePickerT.Value.Date +
                   dateTimePickerTT.Value.TimeOfDay;
                using (SqlCommand cmd2 = new SqlCommand("INSERT INTO Transactions VALUES (NEWID(), @fname, @pto, @cs, @pn);", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("fname", act);
                    cmd2.Parameters.AddWithValue("pto", textBoxTAT.Text);
                    cmd2.Parameters.AddWithValue("cs", myDate.ToString());
                    cmd2.Parameters.AddWithValue("pn", metroComboBoxPNT.Text);
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                }
                MessageBox.Show("Creation Successful!");
                textBoxTAT.Text = "";
                dateTimePickerT.Value = DateTime.Now;
                dateTimePickerTT.Value = DateTime.Now;
                metroComboBoxCANT.SelectedIndex = -1;
                metroComboBoxPNT.SelectedIndex = -1;
                UpdateALLGRIDS();
                UpdateALLCOMBOS();
            }
        }

        private void togDEL_CheckedChanged(object sender, EventArgs e)
        {
            if (togDEL.Checked == true)
            {
                string response;
                using (Dialog formOptions = new Dialog())
                {
                    formOptions.ShowDialog();

                    response = formOptions.GetResult();
                }
                //string response = Microsoft.VisualBasic.Interaction.InputBox("Enter Password:", "Allow delete", "");
                if (response == "maamabby")
                {//SHOW

                    delBC.Visible = true;
                    delBCR.Visible = true;
                    delBR.Visible = true;
                    delC.Visible = true;
                    delCR.Visible = true;
                    delT.Visible = true;

                    MessageBox.Show("Admin Mode:ON");
                }
                else
                {
                    MessageBox.Show("Wrong Password");
                    togDEL.Checked = false;
                }
            }
            else
            {//HIDE

                delBC.Visible = false;
                delBCR.Visible = false;
                delBR.Visible = false;
                delC.Visible = false;
                delCR.Visible = false;
                delT.Visible = false;
                MessageBox.Show("Admin Mode:OFF");
            }
        }

        private void delT_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";
            string c = "";
            string d = "";
            if (dgvT.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvT.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvT.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
                c = Convert.ToString(selectedRow.Cells[2].Value);
                d = Convert.ToString(selectedRow.Cells[3].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + " - " + c + " - " + d + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM Transactions WHERE ID=@param", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("param", b);
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("Deleted!");
                    }
                    catch { MessageBox.Show("Delete Constrait first."); }
                }

                UpdateALLGRIDS();
                UpdateALLCOMBOS();
            }
        }

        private void delBCR_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";

            if (dgvBCR.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvBCR.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvBCR.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);

            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM BlacklistedCardRanges WHERE ID=@param", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("param", b);
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("Deleted!");
                    }
                    catch { MessageBox.Show("Delete Constrait first."); }
                }

                UpdateALLGRIDS();
                UpdateALLCOMBOS();
            }
        }

        private void delBC_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";

            if (dgvBC.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvBC.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvBC.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);

            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM BlacklistedCards WHERE ID=@param", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("param", b);
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("Deleted!");
                    }
                    catch { MessageBox.Show("Delete Constrait first."); }
                }

                UpdateALLGRIDS();
                UpdateALLCOMBOS();
            }
        }

        private void delBR_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";

            if (dgvBR.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvBR.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvBR.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);

            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM BlacklistRequests WHERE ID=@param", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("param", b);
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("Deleted!");
                    }
                    catch { MessageBox.Show("Delete Constrait first."); }
                }

                UpdateALLGRIDS();
                UpdateALLCOMBOS();
            }
        }

        private void delC_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";

            if (dgvC.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvC.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvC.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);

            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM Cards WHERE ID=@param", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("param", b);
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("Deleted!");
                    }
                    catch { MessageBox.Show("Delete Constrait first."); }
                }

                UpdateALLGRIDS();
                UpdateALLCOMBOS();
            }
        }

        private void delCR_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";

            if (dgvCR.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvCR.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvCR.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);

            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM CardRequests WHERE ID=@param", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("param", b);
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("Deleted!");
                    }
                    catch { MessageBox.Show("Delete Constrait first."); }
                }

                UpdateALLGRIDS();
                UpdateALLCOMBOS();
            }
        }

        public void IsMenuDisabled(bool t, Button b)
        {
            if (t == true)
            {
                panelMenu.Enabled = false;
                panelMenu2.Enabled = false;
                b.Enabled = false;
                buttonUMENU.Enabled = false;
            }
            else
            {
                panelMenu.Enabled = true;
                panelMenu2.Enabled = true;
                buttonUMENU.Enabled = true;
                b.Enabled = true;
            }

        }
        private void buttonSaveT_Click(object sender, EventArgs e)
        { string s = "";
            CloseConn();
            if (metroComboBoxCANT.Text == "" || metroComboBoxPNT.Text == "" || textBoxTAT.Text == "")
            {
                MessageBox.Show("Please fill out all fields.");
            }
            else
            {
                CloseConn();
                using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM Cards WHERE CAN=@pid", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", metroComboBoxCANT.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        try
                        {
                            reader2.Read();
                           s = (reader2[0].ToString());
                        }
                        catch
                        {
                            conn.Close();
                        }
                    }
                    conn.Close();
                }
                DateTime myDate = dateTimePickerT.Value.Date +
                   dateTimePickerTT.Value.TimeOfDay;
                using (SqlCommand cmd2 = new SqlCommand("UPDATE Transactions SET CardRefID=@s, TransactionAmount=@pto, Transactiondatetime=@cs, ParticipantName=@pn WHERE ID=@fname;", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("fname", holder1);
                    cmd2.Parameters.AddWithValue("s", s);
                    cmd2.Parameters.AddWithValue("pto", textBoxTAT.Text);
                    cmd2.Parameters.AddWithValue("cs", myDate.ToString());
                    cmd2.Parameters.AddWithValue("pn", metroComboBoxPNT.Text);
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                }
                MessageBox.Show("Update Successful!");
                textBoxTAT.Text = "";
                dateTimePickerT.Value = DateTime.Now;
                dateTimePickerTT.Value = DateTime.Now;
                metroComboBoxCANT.SelectedIndex = -1;
                metroComboBoxPNT.SelectedIndex = -1;
                UpdateALLGRIDS();
                UpdateALLCOMBOS();
                IsMenuDisabled(false, delT);
                buttonSaveT.Visible = false;
                buttonADDT.Enabled = true;
            }
        }
        string holder1;
        private void dgvT_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            DateTime dateTime;
            CloseConn();
            if (togDEL.Checked)
            {
                buttonADDT.Enabled = false;
                buttonSaveT.Visible = true;
                if (dgvT.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {

                    holder1 = dgvT.SelectedRows[0].Cells[0].Value.ToString();
                    metroComboBoxCANT.Text = dgvT.SelectedRows[0].Cells[1].Value.ToString();
                    textBoxTAT.Text = dgvT.SelectedRows[0].Cells[2].Value.ToString();
                    dateTime = Convert.ToDateTime(dgvT.SelectedRows[0].Cells[3].Value);
                    dateTimePickerT.Value = dateTime.Date;
                    dateTimePickerTT.Value = dateTime;
                    metroComboBoxPNT.Text = dgvT.SelectedRows[0].Cells[4].Value.ToString();
                    IsMenuDisabled(true, delT);

                }
            }
        }

        private void buttonSaveBRC_Click(object sender, EventArgs e)
        {
            CloseConn();
            if (textBoxSCANBR.Text == "" || textBoxSCANBR.TextLength != 16 && textBoxECANBR.Text == "" || textBoxECANBR.TextLength != 16)
            {
                MessageBox.Show("Invalid Card format (16 digits)");
            }
            else if (metroComboBoxBRC.Text == "")
            {
                MessageBox.Show("Please fill out all fields.");
            }
            else
            {

                using (SqlCommand cmd2 = new SqlCommand("UPDATE BlacklistedCardRanges SET StartCardApplicationNumber=@fname, EndCardApplicationNumber=@pto, BlacklistReasonCode=@cs WHERE ID=@id;", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("id", holder1);
                    cmd2.Parameters.AddWithValue("fname", textBoxSCANBR.Text);
                    cmd2.Parameters.AddWithValue("pto", textBoxECANBR.Text);
                    cmd2.Parameters.AddWithValue("cs", metroComboBoxBRC.Text);
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                }
                MessageBox.Show("Update Successful!");
                textBoxSCANBR.Text = "";
                textBoxECANBR.Text = "";
                metroComboBoxBRC.SelectedIndex = -1;
                UpdateALLGRIDS();
                IsMenuDisabled(false, delBCR);
                buttonSaveBRC.Visible = false;
                buttonADDBRC.Enabled = true;
            }
        }

        private void dgvBCR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                buttonADDBRC.Enabled = false;
                buttonSaveBRC.Visible = true;
                if (dgvBCR.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    holder1 = dgvBCR.SelectedRows[0].Cells[0].Value.ToString();
                    textBoxSCANBR.Text = dgvBCR.SelectedRows[0].Cells[1].Value.ToString();
                    textBoxECANBR.Text = dgvBCR.SelectedRows[0].Cells[2].Value.ToString();
                    metroComboBoxBRC.Text = dgvBCR.SelectedRows[0].Cells[3].Value.ToString();
                    IsMenuDisabled(true, delBCR);
                }
            }
        }

        private void dgvCR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                buttonCreateCR.Enabled = false;
                buttonSaveCR.Visible = true;
                if (dgvCR.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    holder1 = dgvCR.SelectedRows[0].Cells[0].Value.ToString();
                    textBoxCANcr.Text = dgvCR.SelectedRows[0].Cells[1].Value.ToString();
                    IsMenuDisabled(true, delCR);
                }
            }
        }

        private void buttonSaveCR_Click(object sender, EventArgs e)
        {
            CloseConn();
            string ftid = textBoxCANcr.Text;
            if (textBoxCANcr.Text == "" || textBoxCANcr.TextLength != 16)
            {
                MessageBox.Show("Invalid Card format (16 digits)");
            }
            else
            {
               
                    using (SqlCommand cmd2 = new SqlCommand("UPDATE CardRequests SET CAN=@fname WHERE ID=@pto", conn))
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("fname", ftid);
                        cmd2.Parameters.AddWithValue("pto", holder1);
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                    }
                    MessageBox.Show("Creation Successful!");
                    textBoxCANcr.Text = "";
                UpdateALLGRIDS();
                IsMenuDisabled(false, delCR);
                buttonSaveCR.Visible = false;
                buttonCreateCR.Enabled = true;
            }
        }

        private void dgvC_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                DateTime dateTime;
                buttonCREATEC.Enabled = false;
                buttonSaveC.Visible = true;
                if (dgvC.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    holder1 = dgvC.SelectedRows[0].Cells[0].Value.ToString();

                    textBoxCANC.Text = dgvC.SelectedRows[0].Cells[1].Value.ToString();
                    textBoxPBC.Text = dgvC.SelectedRows[0].Cells[2].Value.ToString();
                    dateTime = Convert.ToDateTime(dgvC.SelectedRows[0].Cells[3].Value);
                    datePortionDateTimePicker.Value = dateTime.Date;
                    timePortionDateTimePicker.Value = dateTime;
                    CSC.Text = dgvC.SelectedRows[0].Cells[4].Value.ToString();
                    CTC.Text = dgvC.SelectedRows[0].Cells[5].Value.ToString();
                    IsMenuDisabled(true, delC);
                }
            }
        }

        private void buttonSaveC_Click(object sender, EventArgs e)
        {
            CloseConn();
            string ftid = textBoxCANC.Text;
            if (textBoxCANC.Text == "" || textBoxCANC.TextLength != 16)
            {
                MessageBox.Show("Invalid Card format (16 digits)");
            }
            else if (textBoxPBC.Text == "" || CTC.Text == "" || CSC.Text == "")
            {
                MessageBox.Show("Please fill out all fields.");
            }
            else
            {

                

                    DateTime myDate = datePortionDateTimePicker.Value.Date +
                    timePortionDateTimePicker.Value.TimeOfDay;
                    using (SqlCommand cmd2 = new SqlCommand("UPDATE Cards SET CAN=@fname, PurseBalance=@pto, CardSyncDateTime=@csdt, CardStatus=@cs, CardProfileType=@ct WHERE ID=@holder;", conn))
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("fname", ftid);
                        cmd2.Parameters.AddWithValue("pto", textBoxPBC.Text);
                        cmd2.Parameters.AddWithValue("csdt", myDate.ToString());
                        cmd2.Parameters.AddWithValue("cs", CSC.Text);
                    cmd2.Parameters.AddWithValue("holder", holder1);
                    cmd2.Parameters.AddWithValue("ct", CTC.Text);
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                    }
                    MessageBox.Show("Update Successful!");
                    textBoxCANC.Text = "";
                    textBoxPBC.Text = "";
                    datePortionDateTimePicker.Value = DateTime.Now;
                    timePortionDateTimePicker.Value = DateTime.Now;
                    CTC.SelectedIndex = -1;
                    CSC.SelectedIndex = -1;
                    UpdateALLGRIDS();
                UpdateALLCOMBOS();
                IsMenuDisabled(false, delC);
                buttonSaveC.Visible = false;
                buttonCREATEC.Enabled = true;

            }
        }

        private void dgvBR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            CloseConn();
            if (togDEL.Checked)
            {
               
                buttonADDBR.Enabled = false;
                buttonSaveBR.Visible = true;
                if (dgvBR.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    holder1 = dgvBR.SelectedRows[0].Cells[0].Value.ToString();

                    textBoxCANBR.Text = dgvBR.SelectedRows[0].Cells[1].Value.ToString();
                    textBoxBR.Text = dgvBR.SelectedRows[0].Cells[2].Value.ToString();
                    textBoxMN.Text = dgvBR.SelectedRows[0].Cells[3].Value.ToString();
                    IsMenuDisabled(true, delBR);
                }
            }
        }

        private void buttonSaveBR_Click(object sender, EventArgs e)
        {
            CloseConn();
            string ftid = textBoxCANBR.Text;
            if (textBoxCANBR.Text == "" || textBoxCANBR.TextLength != 16 && textBoxMN.Text == "" || textBoxMN.TextLength != 11 && textBoxMN.TextLength != 13)
            {
                MessageBox.Show("Invalid Card format (16 digits) or Mobile Number (13 digits)");
            }
            else if (textBoxBR.Text == "")
            {
                MessageBox.Show("Please fill out all fields.");
            }
            else
            {
               
                    using (SqlCommand cmd2 = new SqlCommand("UPDATE BlacklistRequests SET CAN=@fname, BlacklistReason=@pto, MobileNumber=@cs WHERE ID=@hold", conn))
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("fname", ftid);
                        cmd2.Parameters.AddWithValue("pto", textBoxBR.Text);
                        cmd2.Parameters.AddWithValue("cs", textBoxMN.Text);
                    cmd2.Parameters.AddWithValue("hold", holder1);
                    cmd2.ExecuteNonQuery();
                        conn.Close();
                    }
                    MessageBox.Show("Update Successful!");
                    textBoxCANBR.Text = "";
                    textBoxMN.Text = "";
                    textBoxBR.Text = "";
                    UpdateALLGRIDS();
                IsMenuDisabled(false, delBR);
                buttonSaveBR.Visible = false;
                buttonADDBR.Enabled = true;

            }
        }

        private void buttonSaveBC_Click(object sender, EventArgs e)
        {
            CloseConn();
            string ftid = textBoxCANBC.Text;
            if (textBoxCANBC.Text == "" || textBoxCANBC.TextLength != 16)
            {
                MessageBox.Show("Invalid Card format (16 digits)");
            }
            else if (metroComboBoxBRCBC.Text == "" || metroComboBoxBDSN.Text == "")
            {
                MessageBox.Show("Please fill out all fields.");
            }
            else
            {
               

     
                    using (SqlCommand cmd2 = new SqlCommand("UPDATE BlacklistedCards SET CardApplicationNumber=@fname, BadDebtSequenceNumber=@pto, BlacklistReasonCode=@cs WHERE ID=@hold", conn))
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("fname", ftid);
                        cmd2.Parameters.AddWithValue("pto", metroComboBoxBDSN.Text);
                        cmd2.Parameters.AddWithValue("cs", metroComboBoxBRCBC.Text);
                    cmd2.Parameters.AddWithValue("hold", holder1);
                    cmd2.ExecuteNonQuery();
                        conn.Close();
                    }
                    MessageBox.Show("Update Successful!");
                    textBoxCANBC.Text = "";
                    metroComboBoxBRCBC.SelectedIndex = -1;
                    metroComboBoxBDSN.SelectedIndex = -1;
                    panel3.Refresh();
                    UpdateALLGRIDS();
                UpdateALLCOMBOS();
                IsMenuDisabled(false, delBC);
                buttonSaveBC.Visible = false;
                buttonADDBC.Enabled = true;

            }
        }

        private void dgvBC_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            CloseConn();
            if (togDEL.Checked)
            {

                buttonADDBC.Enabled = false;
                buttonSaveBC.Visible = true;
                if (dgvBC.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    holder1 = dgvBC.SelectedRows[0].Cells[0].Value.ToString();

                    textBoxCANBC.Text = dgvBC.SelectedRows[0].Cells[1].Value.ToString();
                    metroComboBoxBRCBC.Text = dgvBC.SelectedRows[0].Cells[3].Value.ToString();
                    metroComboBoxBDSN.Text = dgvBC.SelectedRows[0].Cells[2].Value.ToString();
                    IsMenuDisabled(true, delBC);
                }
            }
        }

        private void qm_Click(object sender, EventArgs e)
        {
            using (qm qm = new qm())
            {
                qm.ShowDialog();
            }
        }

        private void dgvT_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvT);
        }
        private void CelltoClip(object sender, DataGridViewCellEventArgs e, DataGridView dgv)
        {
            try { 
            if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                Clipboard.SetText(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                using (cellclip cp = new cellclip(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                {
                    cp.ShowDialog();
                }

            }
            }
            catch { }
        }

        private void dgvBCR_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvBCR);
        }

        private void dgvBC_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvBC);
        }

        private void dgvBR_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvBR);
        }

        private void dgvC_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvC);
        }

        private void dgvCR_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvCR);
        }
    }
}