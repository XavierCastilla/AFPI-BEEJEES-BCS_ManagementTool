using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AFPI_Beejees_db
{
    public partial class MainProg : MetroFramework.Forms.MetroForm
    {
        //////////////////////////////////////////
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

        //////////////////////////////////////////
        //STARTING VARIABLES
        //////////////////////////////////////////        
        SqlConnection conn;
        string log;
        string uni;
        string idMerc;
        string PT0;
        string orig;
        string RBF_RID;
        string RBF_BFI;
        string RDF_RID;
        string RDF_DID;
        string PR_PID;
        string PR_RID;
        string PDB_PID;
        string PDB_BID;
        string holder1;
        string holder2;
        string holder3;
        afpiLog.Logger logger = new afpiLog.Logger("log.txt");

        //////////////////////////////////////////
        //METHODS
        //////////////////////////////////////////
        int DropDownWidth(ComboBox myCombo)
        {
            int maxWidth = 0, temp = 0;
            foreach (var obj in myCombo.Items)
            {
                temp = TextRenderer.MeasureText(obj.ToString(), myCombo.Font).Width;
                if (temp > maxWidth)
                {
                    maxWidth = temp;
                }
            }
            return maxWidth;
        }
        private void radioButtonDFT_U_CheckedChanged(object sender, EventArgs e)
        {
            textBoxDFT_RA.Minimum = 50;
        }

        private void radioButtonDFT_D_CheckedChanged(object sender, EventArgs e)
        {
            textBoxDFT_RA.Minimum = 50;
        }

        private void radioButtonDFT_E_CheckedChanged(object sender, EventArgs e)
        {
            textBoxDFT_RA.Minimum = 0;
        }
        public void SysParamsCombo()
        {
            CloseConn();
            metroComboBoxSP_P.Items.Clear();
            metroComboBoxSP_P.Items.Add("AcquirerId");
            metroComboBoxSP_P.Items.Add("FacilityCode");
            metroComboBoxSP_P.Items.Add("FacilityName");
            metroComboBoxSP_P.Items.Add("HeartBeatInterval");
            metroComboBoxSP_P.Items.Add("HideCashTotal");
            metroComboBoxSP_P.Items.Add("LogLevel");
            metroComboBoxSP_P.Items.Add("NumberOfIncrements");
            metroComboBoxSP_P.Items.Add("ParticipantId");
            metroComboBoxSP_P.Items.Add("ParticipantName");
            metroComboBoxSP_P.Items.Add("ParticipantShortName");
            metroComboBoxSP_P.Items.Add("PrinterConfig");
            metroComboBoxSP_P.Items.Add("ScreenTimeout");
            metroComboBoxSP_P.Items.Add("TransactionUploadCount");
            metroComboBoxSP_P.Items.Add("TransactionUploadInterval");
            metroComboBoxSP_P2.Items.Clear();
            metroComboBoxSP_P2.Items.Add("AcquirerId");
            metroComboBoxSP_P2.Items.Add("FacilityCode");
            metroComboBoxSP_P2.Items.Add("FacilityName");
            metroComboBoxSP_P2.Items.Add("HeartBeatInterval");
            metroComboBoxSP_P2.Items.Add("HideCashTotal");
            metroComboBoxSP_P2.Items.Add("LogLevel");
            metroComboBoxSP_P2.Items.Add("NumberOfIncrements");
            metroComboBoxSP_P2.Items.Add("ParticipantId");
            metroComboBoxSP_P2.Items.Add("ParticipantName");
            metroComboBoxSP_P2.Items.Add("ParticipantShortName");
            metroComboBoxSP_P2.Items.Add("PrinterConfig");
            metroComboBoxSP_P2.Items.Add("ScreenTimeout");
            metroComboBoxSP_P2.Items.Add("TransactionUploadCount");
            metroComboBoxSP_P2.Items.Add("TransactionUploadInterval");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure?", "Close Application", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
                this.Close();
            AFPI_Beejees_db.LoginForm ap = new LoginForm();
            this.Hide();
            ap.ShowDialog();
        }

        public void IsMenuDisabled(bool t, Button b)
        {
            if (t == true)
            {
                panelMenu.Enabled = false;
                panelMenu2.Enabled = false;
                panelMenu3.Enabled = false;
                panelMenu4.Enabled = false;
                b.Enabled = false;
            }
            else
            {
                panelMenu.Enabled = true;
                panelMenu2.Enabled = true;
                panelMenu3.Enabled = true;
                panelMenu4.Enabled = true;
                b.Enabled = true;
            }

        }

        private void Mouse_Down(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        /// <summary>
        /// UPDATES ALL COMBOBOX
        /// </summary>
        private void UpdateAllCombos()
        {
            try
            {
                comboBoxMerchantList.Items.Clear();
                comboBoxMercBP.Items.Clear();
                comboBoxBF.Items.Clear();
                textBoxFname.Items.Clear();
                metroComboBoxRBF_RID.Items.Clear();
                metroComboBoxRBF_BFI.Items.Clear();
                metroComboBoxRBF_RID2.Items.Clear();
                metroComboBoxRBF_BFI2.Items.Clear();
                metroComboBoxRDBF_RID.Items.Clear();
                metroComboBoxRDBF_DID.Items.Clear();
                metroComboBoxRDBF_RID2.Items.Clear();
                metroComboBoxRDBF_DID2.Items.Clear();
                metroComboBoxPR_RID.Items.Clear();
                metroComboBoxPR_RID2.Items.Clear();
                metroComboBoxPR_PID.Items.Clear();
                metroComboBoxPR_PID2.Items.Clear();
                metroComboBoxPDB_PID.Items.Clear();
                metroComboBoxPDB_BID.Items.Clear();
                metroComboBoxPDB_PID2.Items.Clear();
                metroComboBoxPDB_BID2.Items.Clear();
                metroComboBoxPP_PID.Items.Clear();
                metroComboBoxPP_SPI.Items.Clear();
                metroComboBoxPP_PID2.Items.Clear();
                metroComboBoxPP_SPI2.Items.Clear();
                metroComboBoxUA_MN.Items.Clear();
                metroComboBoxUA_MN2.Items.Clear();
                metroComboBoxUC_UN.Items.Clear();
                metroComboBoxUC_UN2.Items.Clear();
                mcbxProfiles.Items.Clear();
                mcbxProfiles2.Items.Clear();
                mcbxFTID.Items.Clear();
                mcbxFTID2.Items.Clear();
                DBIF_ftid.Items.Clear();
                cbxNewFTIDDBIF.Items.Clear();
                metroComboBoxFleet.Items.Clear();
                textBoxINS.Items.Clear();
                mcbxTB.Items.Clear();
                mcbxTBDEL.Items.Clear();
                SysParamsCombo();
                TIDBS.Items.Clear();
                TIDBS2.Items.Clear();
                comboCHECK.Items.Clear();
                metroComboBoxDBCP_DFTID.Items.Clear();
                metroComboBoxDBCP_DFTID2.Items.Clear();
                CloseConn();
                using (SqlCommand cmd = new SqlCommand("SELECT ParticipantID from Merchants", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBoxMerchantList.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            comboBoxMercBP.Items.Add(reader.GetValue(0).ToString().ToUpper());
                        }
                    }
                    conn.Close();
                }

                using (SqlCommand cmd = new SqlCommand("SELECT NAME FROM sys.Tables", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            textBoxINS.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            textBoxINS.Items.Remove("SYSDIAGRAMS");
                            mcbxTB.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            mcbxTBDEL.Items.Add(reader.GetValue(0).ToString().ToUpper());
                        }
                    }
                    conn.Close();
                }

                using (SqlCommand cmd = new SqlCommand("SELECT FareTableID FROM DiscountFareTables", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            metroComboBoxDBCP_DFTID.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            metroComboBoxDBCP_DFTID2.Items.Add(reader.GetValue(0).ToString().ToUpper());
                        }
                    }
                    conn.Close();
                }


                using (SqlCommand cmd = new SqlCommand("select FareTableID from DistanceBasedFareTables WHERE NOT EXISTS (SELECT * FROM DistanceBasedIncrementFares WHERE DistanceBasedIncrementFares.FareTable_ID = DistanceBasedFareTables.ID) ORDER BY FareTableId ASC", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DBIF_ftid.Items.Add(reader.GetValue(0).ToString().ToUpper());
                        }
                    }
                    conn.Close();
                }

                using (SqlCommand cmd = new SqlCommand("select FareTableID from DistanceBasedFareTables", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbxNewFTIDDBIF.Items.Add(reader.GetValue(0).ToString().ToUpper());
                        }
                    }
                    conn.Close();
                }

                using (SqlCommand cmd = new SqlCommand("SELECT ProfileID from BeejeesProfiles", conn)) //WHERE NOT EXISTS (SELECT * FROM BeejeesFleets WHERE BeejeesProfiles.ID = BeejeesFleets.ProfileID) ORDER BY ProfileID ASC"
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBoxBF.Items.Add(reader.GetValue(0).ToString().ToUpper());
                        }
                    }
                    conn.Close();
                }

                using (SqlCommand cmd = new SqlCommand("SELECT ParticipantName from Merchants", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            metroComboBoxUA_MN.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            metroComboBoxUA_MN2.Items.Add(reader.GetValue(0).ToString().ToUpper());
                        }
                    }
                    conn.Close();
                }

                using (SqlCommand cmd = new SqlCommand("SELECT ID from UserAccounts", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            metroComboBoxUC_UN.Items.Add(reader.GetValue(0).ToString().ToUpper());
                           
                        }
                    }
                    conn.Close();
                }

                using (SqlCommand cmd = new SqlCommand("select FleetsName from BeejeesFleets", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            textBoxFname.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            metroComboBoxFleet.Items.Add(reader.GetValue(0).ToString().ToUpper());
                        }
                    }
                    conn.Close();
                }


                using (SqlCommand cmd = new SqlCommand("select Id from UserAccounts", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            metroComboBoxUC_UN2.Items.Add(reader.GetValue(0).ToString().ToUpper());
                        }
                    }
                    conn.Close();
                }
            
                using (SqlCommand cmd = new SqlCommand("select RouteLongName from DistanceBasedRoutes", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            metroComboBoxRBF_RID.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            metroComboBoxRBF_RID2.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            metroComboBoxRDBF_RID.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            metroComboBoxRDBF_RID2.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            metroComboBoxPR_RID.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            metroComboBoxPR_RID2.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            comboCHECK.Items.Add(reader.GetValue(0).ToString().ToUpper());
                        }
                    }
                    conn.Close();
                }
                using (SqlCommand cmd = new SqlCommand("select ProfileName from BeejeesProfiles", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            metroComboBoxPR_PID.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            metroComboBoxPR_PID2.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            metroComboBoxPDB_PID.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            metroComboBoxPDB_PID2.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            mcbxProfiles.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            mcbxProfiles2.Items.Add(reader.GetValue(0).ToString().ToUpper());
                        }
                    }
                    conn.Close();
                }

                using (SqlCommand cmd = new SqlCommand("select FareTableID from DistanceBasedFareTables", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            metroComboBoxRBF_BFI.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            metroComboBoxRBF_BFI2.Items.Add(reader.GetValue(0).ToString().ToUpper());
                        }
                    }
                    conn.Close();
                }

                using (SqlCommand cmd = new SqlCommand("select FareTableID from DiscountFareTables", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            metroComboBoxRDBF_DID.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            metroComboBoxRDBF_DID2.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            mcbxFTID.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            mcbxFTID2.Items.Add(reader.GetValue(0).ToString().ToUpper());
                        }
                    }
                    conn.Close();
                }

                using (SqlCommand cmd = new SqlCommand("SELECT ProfileID from BeejeesProfiles", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            metroComboBoxPP_PID.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            metroComboBoxPP_PID2.Items.Add(reader.GetValue(0).ToString().ToUpper());
                        }
                    }
                    conn.Close();
                }

                using (SqlCommand cmd = new SqlCommand("SELECT FareTableID from DistanceBasedFareTables", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            metroComboBoxPDB_BID.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            metroComboBoxPDB_BID2.Items.Add(reader.GetValue(0).ToString().ToUpper());
                        }
                    }
                    conn.Close();
                }


                using (SqlCommand cmd = new SqlCommand("SELECT ID from SystemParameters", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            metroComboBoxPP_SPI.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            metroComboBoxPP_SPI2.Items.Add(reader.GetValue(0).ToString().ToUpper());
                        }
                    }
                    conn.Close();
                }

                using (SqlCommand cmd = new SqlCommand("SELECT TerminalID from Terminals", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TIDBS.Items.Add(reader.GetValue(0).ToString().ToUpper());
                            TIDBS2.Items.Add(reader.GetValue(0).ToString().ToUpper());
                        }
                    }
                    conn.Close();
                }

            }
            catch
            {
                MessageBox.Show("Not Connected to database!");
            }


        }
        private void buttonexpmerc_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dgvMERC);
        }

        private void buttonexpbp_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dataGridViewBP);
        }

        private void buttonexpbf_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dataGridViewFleets);
        }

        private void buttonterminalexp_Click(object sender, EventArgs e)
        {
            ExportTOCSV(metroGridFleets);
        }

        private void buttonexpdbr_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dgvDBR);
        }

        private void buttonexpdft_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dgvDFT);
        }

        private void buttonexpdbft_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dgvDBFT);

        }

        private void buttonexprbf_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dgvRBF);
        }

        private void buttonexprdf_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dgvRDF);
        }

        private void buttonexppr_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dgvPR);
        }

        private void buttonexppdb_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dgvPDB);
        }

        private void buttonexppp_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dgvPP);
        }

        private void buttonexpsp_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dgvSP);
        }

        private void buttonexpdbif_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dgvDBIF);
        }

        private void buttonexpua_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dgvUA);
        }

        private void buttonexpbjs_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dgvBS);
        }

        private void buttonexpsff_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dgvSFF);
        }

        private void buttonexpdbcp_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dgvDBCP);
        }

        private void buttonexppd_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dgvPD);
        }

        private void buttonexpuc_Click(object sender, EventArgs e)
        {
            ExportTOCSV(dgvUC);
        }

        private void UpdateTxts(string comd)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(comd, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        textBoxEdit_Pname.Text = (reader["ParticipantName"].ToString());
                        textBox_PSname.Text = (reader["ParticipantShortname"].ToString());
                    }
                    conn.Close();
                }
            }
            catch
            {
                MessageBox.Show("Not Connected to database!");
            }
        }

        private void cbxNewFTIDDBIF_SelectedIndexChanged(object sender, EventArgs e)
        {
            CloseConn();
            try
            {
                ShowDetails(cbxNewFTIDDBIF, "DistanceBasedFareTables", "FareTableID");
            }
            catch
            {
                CloseConn();
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

                    panelMerc.Location = new Point(40, 25);
                    buttonDELPD.Visible = true;
                    buttonDELMERC.Visible = true;
                    buttonBPdel.Visible = true;
                    buttonBFdel.Visible = true;
                    buttonDelT.Visible = true;
                    buttonDELDBR.Visible = true;
                    buttonDELDFT.Visible = true;
                    buttonDELDBFT.Visible = true;
                    buttonDELRBF.Visible = true;
                    buttonDELRDF.Visible = true;
                    buttonDELPR.Visible = true;
                    buttonDELSP.Visible = true;
                    buttonDELPDB.Visible = true;
                    buttonDELDBIF.Visible = true;
                    buttonDELPP.Visible = true;
                    buttonDELUA.Visible = true;
                    buttonDELBS.Visible = true;
                    buttonUC_DEL.Visible = true;
                    panelPlaceHolderMerc.Visible = true;
                    buttonSFFDEL.Visible = true;
                    buttonDBCPDEL.Visible = true;
                    buttonRemoveSP.Visible = true;
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
                panelMerc.Location = new Point(246, 25);
                buttonUC_DEL.Visible = false;
                buttonDELMERC.Visible = false;
                buttonBPdel.Visible = false;
                buttonBFdel.Visible = false;
                buttonDelT.Visible = false;
                buttonDELDBR.Visible = false;
                buttonDELDFT.Visible = false;
                buttonDELDBFT.Visible = false;
                buttonDELRBF.Visible = false;
                buttonDELRDF.Visible = false;
                buttonDELPR.Visible = false;
                buttonDELSP.Visible = false;
                buttonDELPDB.Visible = false;
                buttonDELDBIF.Visible = false;
                buttonDELPP.Visible = false;
                buttonDELUA.Visible = false;
                buttonDELBS.Visible = false;
                buttonSFFDEL.Visible = false;
                buttonDBCPDEL.Visible = false;
                panelPlaceHolderMerc.Visible = false;
                buttonRemoveSP.Visible = false;
                buttonDELPD.Visible = false;
                MessageBox.Show("Admin Mode:OFF");
            }
        }
        public void UpdateALLGRIDS()
        {
            try
            {
                UpdateGrid("Select * from Merchants", dgvMERC);
                UpdateGrid("select BeejeesFleets.ID, BeejeesFleets.FleetsName, BeejeesFleets.PTO, BeejeesProfiles.ProfileID from  BeejeesFleets LEFT JOIN BeejeesProfiles ON BeejeesProfiles.ID = BeejeesFLeets.ProfileID  ", dataGridViewFleets);
                UpdateGrid("SELECT Terminals.ID, BeejeesFleets.FleetsName, Terminals.TerminalId, Terminals.TerminalType FROM Terminals INNER JOIN BeejeesFleets ON Terminals.FleetID = BeejeesFleets.ID; ", metroGridFleets);
                UpdateGrid("select * from BeejeesProfiles ORDER BY ProfileID ASC ", dataGridViewBP);
                UpdateGrid("select * from DistanceBasedRoutes", dgvDBR);
                UpdateGrid("select * from DistanceBasedFareTables", dgvDBFT);
                UpdateGrid("select * from DiscountFareTables", dgvDFT);
                UpdateGrid("SELECT RouteBasedFares.ID, DistanceBasedRoutes.RouteLongName, DistanceBasedFareTables.FareTableId  FROM RouteBasedFares LEFT JOIN DistanceBasedFareTables ON DistanceBasedFareTables.ID = RouteBasedFares.BasedFareID  LEFT JOIN DistanceBasedRoutes ON DistanceBasedRoutes.ID = RouteBasedFares.RouteID", dgvRBF);
                UpdateGrid("SELECT RouteDiscountedFares.ID, DistanceBasedRoutes.RouteLongName, DiscountFareTables.FareTableId  FROM RouteDiscountedFares LEFT JOIN DiscountFareTables ON DiscountFareTables.ID = RouteDiscountedFares.DiscountedID  LEFT JOIN DistanceBasedRoutes ON DistanceBasedRoutes.ID = RouteDiscountedFares.RouteID", dgvRDF);
                UpdateGrid("SELECT ProfileRoutes.ID, DistanceBasedRoutes.RouteLongName, BeejeesProfiles.ProfileName  FROM ProfileRoutes Left JOIN DistanceBasedRoutes ON ProfileRoutes.RoutesID = DistanceBasedRoutes.ID   LEFT JOIN BeejeesProfiles ON  ProfileRoutes.ProfileID = BeejeesProfiles.ID", dgvPR);
                UpdateGrid("SELECT ProfileDistanceBaseds.ID, BeejeesProfiles.ProfileName, DistanceBasedFareTables.FareTableId  FROM ProfileDistanceBaseds LEft JOIN DistanceBasedFareTables ON ProfileDistanceBaseds.BasedID = DistanceBasedFareTables.ID   LEFT JOIN BeejeesProfiles ON  ProfileDistanceBaseds.ProfileID = BeejeesProfiles.ID", dgvPDB);
                UpdateGrid("select ID, Parameter,Type,Value from SystemParameters order by Value", dgvSP);
                UpdateGrid("SELECT ProfileParameters.ID, BeejeesProfiles.ProfileID, SystemParameters.Id  AS SystemsParamsID FROM ProfileParameters LEFT JOIN SystemParameters ON ProfileParameters.SystemParametersID = SystemParameters.ID   LEFT JOIN BeejeesProfiles ON  ProfileParameters.ProfileID = BeejeesProfiles.ID", dgvPP);
                UpdateGrid("SELECT DistanceBasedIncrementFares.ID, DistanceBasedFareTables.FareTableId ,DistanceBasedIncrementFares.IncrementalFareAmount, DistanceBasedIncrementFares.IncrementalFareDistance, DistanceBasedIncrementFares.StartDistance from DistanceBasedIncrementFares INNER JOIN DistanceBasedFareTables ON DistanceBasedIncrementFares.FareTable_ID = DistanceBasedFareTables.ID;", dgvDBIF);
                UpdateGrid("select UserAccounts.UserAccountId, UserAccounts.Id, Merchants.ParticipantName AS MerchantName, UserAccounts.CompanyID ,UserAccounts.ShortName,UserAccounts.LongName, UserAccounts.UserEffectiveDateFrom, UserAccounts.UserEffectiveDateTo FROM UserAccounts LEFT JOIN Merchants ON Merchants.ID = UserAccounts.ParticipantId", dgvUA);
                UpdateGrid("select UserCards.ID, UserAccounts.ID AS UserID, UserCards.UID, UserCards.Role, UserCards.CardEffectiveDateFrom, UserCards.CardEffectiveDateTo FROM UserCards LEFT JOIN UserAccounts ON UserAccounts.UserAccountId = UserCards.UserID", dgvUC);
                UpdateGrid("select BusJeepneySettings.ID, Merchants.ParticipantName, Terminals.TerminalId, VehicleID, ShortIdentifier, LongName, VehicleEffectiveDateFrom, VehicleEffectiveDateTo from BusJeepneySettings LEFT JOIN Merchants ON Merchants.ID = BusJeepneySettings.ParticipantId LEFT JOIN Terminals ON Terminals.ID = BusJeepneySettings.TerminalID", dgvBS);
                UpdateGrid("SELECT * FROM SingleFixedFareTables", dgvSFF);
                UpdateGrid("SELECT ProfileDiscounts.ID, BeejeesProfiles.ProfileName, DiscountFareTables.FareTableId  FROM ProfileDiscounts LEft JOIN DiscountFareTables ON ProfileDiscounts.DiscountID = DiscountFareTables.ID   LEFT JOIN BeejeesProfiles ON  ProfileDiscounts.ProfileID = BeejeesProfiles.ID", dgvPD);
                UpdateGrid("SELECT DistanceBasedCardProfiles.ID, CardProfileId, CardProfileName, DiscountFareTables.FareTableId FROM DistanceBasedCardProfiles LEFT JOIN DiscountFareTables ON DistanceBasedCardProfiles.DiscountedFare = DiscountFareTables.Id", dgvDBCP);
            }
            catch
            {
                MessageBox.Show("Not Connected to database!");
            }
        }

        private void Keyboard_KeyPress(object sender, KeyPressEventArgs e)
        {
            //e.Handled = !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '-' && e.KeyChar != '_' && e.KeyChar != (char)Keys.Back && e.KeyChar != '.' && !char.IsSymbol(e.KeyChar);
        }

        private void textBoxCreate_PID_KeyPress(object sender, KeyPressEventArgs e)
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

        //////////////////////////////////////////
        //PROGRAM INITIALIZATION
        //////////////////////////////////////////

        public MainProg(string con)
        {
            conn = new SqlConnection(con);
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            DoubleBuffered = true;
        }

        public MainProg()
        {
            conn = new SqlConnection(@"Data Source=XAVIER\SQLEXPRESS;Initial Catalog=AFPI-BEEJEES-DB;Integrated Security=True");
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            DoubleBuffered = true;
        }

        private void AFPI_Form_Load(object sender, EventArgs e)
        {
            try
            {
                this.BringToFront();
                button1_Click(sender, e);
                panelMenu2.Visible = false;
                panelMenu3.Visible = false;
                titleLB.Text = "Merchants";
                tabMASTER.ItemSize = new Size(0, 2);
                tabMASTER.SizeMode = TabSizeMode.Normal;
                UpdateAllCombos();
                UpdateALLGRIDS();
                tabMASTER.SelectedIndex = 0;
                buttonCMENU.BackColor = ColorTranslator.FromHtml("#00AEDB");
                panelMenu.BackColor = ColorTranslator.FromHtml("#00AEDB");
                buttonBPdel.Visible = false;
                buttonBFdel.Visible = false;
                buttonDelT.Visible = false;
                buttonDELDBR.Visible = false;
                buttonDELDFT.Visible = false;
                buttonDELDBFT.Visible = false;
                buttonDELRBF.Visible = false;
                buttonDELRDF.Visible = false;
                buttonDELPR.Visible = false;
                buttonDELSP.Visible = false;
                buttonDELPDB.Visible = false;
                buttonDELPP.Visible = false;
                buttonDELDBIF.Visible = false;
                buttonDELMERC.Visible = false;
                panelMenu2.BackColor = ColorTranslator.FromHtml("#00AEDB");
                panelMenu3.BackColor = ColorTranslator.FromHtml("#00AEDB");
                panelMenu4.BackColor = ColorTranslator.FromHtml("#00AEDB");
                metroComboBoxSearchParam.SelectedIndex = 0;
            }
            catch
            {
                MessageBox.Show("Database miscommunication");
            }
        }


        //////////////////////////////////////////
        //PROGRAM BODY
        /////////////////////////////////////////
        private void comboBoxMerchantList_SelectedIndexChanged(object sender, EventArgs e)
        {
            CloseConn();
            UpdateTxts("SELECT * from Merchants WHERE ParticipantID='" + comboBoxMerchantList.Text + @"'");
            orig = textBoxEdit_Pname.Text;
        }


        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            object obj;
            CloseConn();
            try
            { //
                if (comboBoxMerchantList.Text == "" || textBoxEdit_Pname.Text == "" || textBox_PSname.Text == "")
                {
                    MessageBox.Show("Please fill out all fields.");
                }
                else
                {
                    using (SqlCommand cmd = new SqlCommand("select COUNT(*) FROM Merchants WHERE ParticipantName = @pid", conn))
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("pid", textBoxEdit_Pname.Text);

                        obj = cmd.ExecuteScalar();
                    }

                    if (Convert.ToInt32(obj) == 0)
                    {
                        using (SqlCommand cmd = new SqlCommand("UPDATE Merchants SET ParticipantName=@pname, ParticipantShortname=@psname WHERE ParticipantID=@pid", conn))
                        {

                            cmd.Parameters.AddWithValue("pname", textBoxEdit_Pname.Text);
                            cmd.Parameters.AddWithValue("psname", textBox_PSname.Text);
                            cmd.Parameters.AddWithValue("pid", comboBoxMerchantList.Text);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }

                        using (SqlCommand cmd = new SqlCommand("UPDATE BeejeesFleets SET FleetsName=@pname WHERE FleetsName=@pname2", conn)) //PROFILES
                        {
                            conn.Open();
                            cmd.Parameters.AddWithValue("pname", textBoxEdit_Pname.Text);
                            cmd.Parameters.AddWithValue("pname2", orig);
                            cmd.ExecuteNonQuery();
                            conn.Close();

                        }

                        using (SqlCommand cmd = new SqlCommand("UPDATE BeejeesProfiles SET ProfileName=@pname WHERE ProfileName=@pname2", conn)) //PROFILES
                        {
                            conn.Open();
                            cmd.Parameters.AddWithValue("pname", textBoxEdit_Pname.Text);
                            cmd.Parameters.AddWithValue("pname2", orig);
                            cmd.ExecuteNonQuery();
                            conn.Close();

                        }

                        MessageBox.Show("Update Successful!");
                        log = DateTime.Now.ToString() + ": Merchant updated - PNAME: " + textBoxEdit_Pname.Text + " - PSNAME: " + textBox_PSname.Text + " - PID: " + comboBoxMerchantList.Text;
                        logger.Write(log);
                        log = string.Empty;
                        UpdateAllCombos();
                        UpdateALLGRIDS();
                        panelPlaceHolderMerc.Refresh();
                        PB_PIDCHECKER.Visible = false;
                        PB_PNCHECKER.Visible = false;
                        textBoxEdit_Pname.Text = "";
                        textBox_PSname.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Participant Name already taken");
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            CloseConn();
            try
            {
                if (textBoxCreate_PID.Text != "" && textBoxCreatePname.Text != "" && textBox_CreatePsname.Text != "")
                {
                    using (SqlCommand cmd = new SqlCommand("select COUNT(*) FROM Merchants WHERE ParticipantID = @pid", conn))
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("pid", textBoxCreate_PID.Text);
                        object obj;
                        obj = cmd.ExecuteScalar();

                        if (Convert.ToInt32(obj) == 0)
                        {
                            using (SqlCommand cmd1 = new SqlCommand("select COUNT(*) FROM Merchants WHERE ParticipantName = @pid", conn))
                            {
                                cmd1.CommandType = CommandType.Text;
                                cmd1.Parameters.AddWithValue("pid", textBoxCreatePname.Text);
                                object obj1;
                                obj1 = cmd1.ExecuteScalar();
                                if (Convert.ToInt32(obj1) == 0)
                                {
                                    using (SqlCommand cmd2 = new SqlCommand("INSERT INTO Merchants VALUES (NEWID(), @pid, @pname, @psname, GETDATE(), 1);", conn))
                                    {
                                        cmd2.Parameters.AddWithValue("pname", textBoxCreatePname.Text);
                                        cmd2.Parameters.AddWithValue("psname", textBox_CreatePsname.Text);
                                        cmd2.Parameters.AddWithValue("pid", textBoxCreate_PID.Text);
                                        cmd2.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                    MessageBox.Show("Creation Successful!");
                                    log = DateTime.Now.ToString() + ": Merchant created - PNAME: " + textBoxCreatePname.Text + " - PSNAME: " + textBox_CreatePsname.Text + " - PID: " + textBoxCreate_PID.Text;
                                    logger.Write(log);
                                    log = string.Empty;
                                    textBoxCreate_PID.Text = "";
                                    textBoxCreatePname.Text = "";
                                    textBox_CreatePsname.Text = "";
                                }
                                else
                                {
                                    MessageBox.Show("Participant Name already exists");
                                    conn.Close();
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("PID already exists");
                        }
                        UpdateAllCombos();
                        UpdateALLGRIDS();
                        panelMerc.Refresh();
                    }
                }
                else
                {
                    MessageBox.Show("Please fill out all fields.");
                }
            }
            catch
            {
                CloseConn();
                MessageBox.Show("Participant ID is too big ");
            }

        }

        public void SetMinDate(MetroFramework.Controls.MetroDateTime mt, MetroFramework.Controls.MetroDateTime mf)
        {
            mf.MinDate = mt.Value.Date;
        }
        private void metroDateTimeDFT_F_ValueChanged(object sender, EventArgs e)
        {
            SetMinDate(metroDateTimeDFT_F, metroDateTimeDFT_T);
        }

        private void metroDateTimeDBFT_F_ValueChanged(object sender, EventArgs e)
        {
            SetMinDate(metroDateTimeDBFT_F, metroDateTimeDBFT_T);
        }

        private void metroDateTimeEDUA_ValueChanged(object sender, EventArgs e)
        {
            SetMinDate(metroDateTimeEDUA, metroDateTimeEDUA2);
        }

        private void metroDateTimeUCED_ValueChanged(object sender, EventArgs e)
        {
            SetMinDate(metroDateTimeUCED, metroDateTimeUCED2);
        }

        private void DTBSFROM_ValueChanged(object sender, EventArgs e)
        {
            SetMinDate(DTBSFROM, DTBSTO);
        }

        private void DTBSFROM2_ValueChanged(object sender, EventArgs e)
        {
            SetMinDate(DTBSFROM2, DTBSTO2);
        }

        private void metroDateTimeSFF_FROM_ValueChanged(object sender, EventArgs e)
        {
            SetMinDate(metroDateTimeSFF_FROM, metroDateTimeSFF_TO);
        }
        private void NumZeroTextChanged(object sender, EventArgs e)
        {
            NumZeroTrim(textBoxPNBP);
            CloseConn();
        }

        public void NumZeroTrim(TextBox rem)
        {
            rem.Text = rem.Text.TrimStart(new Char[] { '0' });
        }

        private void textBoxNewPIDBF_TextChanged(object sender, EventArgs e)
        {
            NumZeroTrim(textBoxNewPIDBF);
        }

        private void textBoxNewTID_TextChanged(object sender, EventArgs e)
        {
            NumZeroTrim(textBoxNewTID);
        }

        private void textBox_TermID_TextChanged(object sender, EventArgs e)
        {
            NumZeroTrim(textBox_TermID);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            NumZeroTrim(textBoxNRID_DBR);
        }

        private void textBoxBDR_RID_TextChanged(object sender, EventArgs e)
        {
            NumZeroTrim(textBoxBDR_RID);
        }

        private void textBoxDFT_FTB_TextChanged(object sender, EventArgs e)
        {
            NumZeroTrim(textBoxDFT_FTB);
        }

        private void textBoxDFT_DISC_TextChanged(object sender, EventArgs e)
        {
            NumZeroTrim(textBoxDFT_DISC);
        }

        private void textBoxDBFT_FTI_TextChanged(object sender, EventArgs e)
        {
            NumZeroTrim(textBoxDBFT_FTI);
        }

        private void textBoxDBFT_FA_TextChanged(object sender, EventArgs e)
        {
            NumZeroTrim(textBoxDBFT_FA);
        }

        private void textBoxDBFT_BFD_TextChanged(object sender, EventArgs e)
        {
            NumZeroTrim(textBoxDBFT_BFD);
        }

        private void DBIF_ifa_TextChanged(object sender, EventArgs e)
        {
            NumZeroTrim(DBIF_ifa);
        }

        private void DBIF_sd_TextChanged(object sender, EventArgs e)
        {
            NumZeroTrim(DBIF_sd);
        }

        private void DBIF_ifd_TextChanged(object sender, EventArgs e)
        {
            NumZeroTrim(DBIF_ifd);
        }

        private void textBox_UA_UID_TextChanged(object sender, EventArgs e)
        {
            NumZeroTrim(textBox_UA_UID);
        }

        private void textBoxUC_UID_TextChanged(object sender, EventArgs e)
        {
            NumZeroTrim(textBoxUC_UID);
        }

        private void VIDBS_TextChanged(object sender, EventArgs e)
        {
            NumZeroTrim(VIDBS);
        }

        private void radioButtonExactNDFT_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownNRA_DFT.Minimum = 0;
        }

        private void radioButtonNUP_DFT_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownNRA_DFT.Minimum = 50;
        }

        private void radioButtonNDOWN_DFT_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownNRA_DFT.Minimum = 50;
        }
        private void SearchQuery_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 23;
            titleLB.Text = "Search";

        }
        private void buttonUpdateBP_Click(object sender, EventArgs e)
        {
            CloseConn();
            if (textBoxPNBP2.Text != "" && textBoxPNBP.Text != "")
            {
                try
                {
                    using (SqlCommand cmd1 = new SqlCommand("select COUNT(*) FROM BeejeesProfiles WHERE ProfileName=@pn", conn))
                    {
                        conn.Open();
                        cmd1.CommandType = CommandType.Text;
                        cmd1.Parameters.AddWithValue("pn", textBoxPNBP2.Text);
                        object obj;
                        obj = cmd1.ExecuteScalar();

                        if (Convert.ToInt32(obj) != 0 || Convert.ToInt32(obj) == 0) //create
                        {
                            using (SqlCommand cmd3 = new SqlCommand("select COUNT(*) FROM BeejeesProfiles WHERE ProfileID=@pn", conn)) //check if PID is existing
                            {
                                cmd3.CommandType = CommandType.Text;
                                cmd3.Parameters.AddWithValue("pn", textBoxPNBP.Text);
                                object obj2;
                                obj2 = cmd3.ExecuteScalar();

                                if (Convert.ToInt32(obj2) == 0)
                                {

                                    if (holder3 == textBoxPNBP2.Text)
                                        MessageBox.Show("Please add characters to the Profile Name");
                                    else
                                    {
                                        using (SqlCommand cmd2 = new SqlCommand("INSERT INTO BeejeesProfiles VALUES (NEWID(), @pname, @pid);", conn))
                                        {
                                            cmd2.Parameters.AddWithValue("pname", textBoxPNBP.Text);
                                            cmd2.Parameters.AddWithValue("pid", textBoxPNBP2.Text);
                                            cmd2.ExecuteNonQuery();
                                            conn.Close();
                                        }
                                        MessageBox.Show("Creation Successful!");
                                        log = DateTime.Now.ToString() + ": BeejeesProfile updated - PNAME: " + textBoxPNBP2.Text + " - PID: " + textBoxPNBP.Text;
                                        logger.Write(log);
                                        log = string.Empty;
                                        UpdateAllCombos();
                                        textBoxPNBP.Text = "";
                                        textBoxPNBP2.Text = "";
                                        UpdateALLGRIDS();                                    
                                    }

                                }
                                else
                                {
                                    MessageBox.Show("Profile ID already exists!");
                                }

                            }


                        }
                        else //update
                        {
                            using (SqlCommand cmd = new SqlCommand("SELECT * FROM BeejeesProfiles WHERE ProfileID=@pid", conn))
                            {
                                string str;
                                cmd.Parameters.AddWithValue("pid", textBoxPNBP.Text);
                                try
                                {
                                    using (var reader = cmd.ExecuteReader())
                                    {
                                        reader.Read();
                                        str = (reader["ProfileName"].ToString());
                                    }
                                }
                                catch
                                {
                                    str = textBoxPNBP2.Text;
                                }
                                if (str != textBoxPNBP2.Text)
                                {
                                    MessageBox.Show("Profile ID already taken");
                                }
                                else
                                {
                                    if (togDEL.Checked == true)
                                    {
                                        using (SqlCommand cmd2 = new SqlCommand("UPDATE BeejeesProfiles SET ProfileID=@pid WHERE ProfileName=@pn;", conn))
                                        {
                                            cmd2.Parameters.AddWithValue("pn", textBoxPNBP2.Text);
                                            cmd2.Parameters.AddWithValue("pid", textBoxPNBP.Text);
                                            cmd2.ExecuteNonQuery();
                                            conn.Close();
                                        }
                                        MessageBox.Show("Update Successful!");
                                        UpdateAllCombos();
                                        textBoxPNBP.Text = "";
                                        textBoxPNBP2.Text = "";
                                        UpdateALLGRIDS();
                                        log = DateTime.Now.ToString() + ": BeejeesProfile updated - PNAME: " + textBoxPNBP2.Text + " - PID: " + textBoxPNBP.Text;
                                        logger.Write(log);
                                        log = string.Empty;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Enable admin mode to update");
                                    }

                                }
                                conn.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (conn == null || conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                        MessageBox.Show("Error: " + ex.ToString());
                    }
                    else
                        MessageBox.Show("Error: " + ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Please select Merchant and input Profile ID");
            }
        }
        private void comboBoxMercBP_SelectedIndexChanged(object sender, EventArgs e)
        {
            CloseConn();
            try
            {
                using (SqlCommand cmd2 = new SqlCommand("select ID from Merchants WHERE ParticipantID = @pid", conn))
                {

                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", comboBoxMercBP.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        reader2.Read();
                        idMerc = (reader2[0].ToString());
                    }
                    conn.Close();

                    ShowDetails(comboBoxMercBP, "Merchants", "ParticipantID");
                }
            }
            catch
            {
                idMerc = "";
                conn.Close();
            }
        }
        private void comboBoxBF_SelectedIndexChanged(object sender, EventArgs e)
        {
            CloseConn();
            string str;
            string pname;
            object obj;
            try
            {
                using (SqlCommand cmd2 = new SqlCommand("select ProfileName from BeejeesProfiles WHERE ProfileID = @pid", conn))
                {

                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", comboBoxBF.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        reader2.Read();
                        str = (reader2["ProfileName"].ToString());
                    }
                    conn.Close();
                    ShowDetails(comboBoxBF, "BeejeesProfiles", "ProfileID");
                }

                textBoxFleetsName.Text = str;
                CloseConn();
                using (SqlCommand cmd2 = new SqlCommand("select * from BeejeesProfiles WHERE ProfileID = @pid", conn))
                {

                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", comboBoxBF.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        reader2.Read();
                        str = (reader2["ID"].ToString());
                        pname = (reader2["ProfileName"].ToString());
                    }
                    conn.Close();
                    uni = str;
                }

                //
                CloseConn();

                using (SqlCommand cmd1 = new SqlCommand("select COUNT(*) FROM BeejeesFleets WHERE ProfileID=@pn", conn))
                {
                    conn.Open();
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("pn", uni);
                    obj = cmd1.ExecuteScalar();
                }


            }
            catch { MessageBox.Show("Multiple Profiles with the given ProfileID"); }
        }

        private void buttonBF_Click(object sender, EventArgs e)
        {
            CloseConn();

            if (comboBoxBF.Text == "" || textBoxFleetsName.Text == "" || textBoxAppend.Text == "" || comboBoxMercBP.Text == "")
            {
                MessageBox.Show("Please fill out all fields.");
            }
            else
            {
                //CHECK IF BEEJEESPROFILES EXIST
                using (SqlCommand cmd1 = new SqlCommand("select COUNT(*) FROM BeejeesProfiles WHERE ProfileID = @pid", conn))
                {
                    conn.Open();
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("pid", comboBoxBF.Text);
                    object obj1;
                    obj1 = cmd1.ExecuteScalar();
                    if (Convert.ToInt32(obj1) != 0)
                    {
                        try
                        {

                            using (SqlCommand cmd2 = new SqlCommand("INSERT INTO BeejeesFleets VALUES (NEWID(), @fname, @pto, @pid);", conn))
                            {

                                cmd2.Parameters.AddWithValue("fname", textBoxFleetsName.Text + textBoxAppend.Text);
                                cmd2.Parameters.AddWithValue("pto", idMerc);
                                cmd2.Parameters.AddWithValue("pid", uni);
                                cmd2.ExecuteNonQuery();
                                conn.Close();
                            }
                            MessageBox.Show("Creation Successful!");
                            log = DateTime.Now.ToString() + ": BeejeesFleets generated - FleetsName: " + textBoxFleetsName.Text + " - PID: " + uni;
                            logger.Write(log);
                            log = string.Empty;
                            UpdateAllCombos();
                            textBoxFleetsName.Text = "";
                            textBoxAppend.Text = "";
                            UpdateALLGRIDS();
                            panel.Refresh();
                        }
                        catch (Exception ex) { MessageBox.Show(ex.ToString()); }

                    }
                    else { MessageBox.Show("Proflile ID does not exist"); }
                }

            }
        }

        private void buttonTerminalUP_Click(object sender, EventArgs e)
        {
            object obj2;
            CloseConn();
            conn.Open();
            if (textBox_TermID.Text != "" && comboBoxTermT.Text != "")
            {
                using (SqlCommand cmd3 = new SqlCommand("select COUNT(*) FROM Terminals WHERE TerminalId=@pn", conn)) //check if PID is existing
                {
                    cmd3.CommandType = CommandType.Text;
                    cmd3.Parameters.AddWithValue("pn", textBox_TermID.Text);
                    obj2 = cmd3.ExecuteScalar();
                }

                if (Convert.ToInt32(obj2) == 0)
                {
                    try
                    {
                        if (conn == null || conn.State == ConnectionState.Open)
                            conn.Close();


                        using (SqlCommand cmd2 = new SqlCommand("select * from BeejeesFleets WHERE FleetsName = @pid", conn))
                        {

                            conn.Open();
                            cmd2.Parameters.AddWithValue("pid", textBoxFname.Text);
                            using (var reader2 = cmd2.ExecuteReader())
                            {

                                reader2.Read();
                                PT0 = (reader2["PTO"].ToString());
                                comboBoxBJFID.Text = (reader2["ID"].ToString().ToUpper());
                            }
                        }
                        using (SqlCommand cmd2 = new SqlCommand("INSERT INTO Terminals VALUES (NEWID(), @tid, @pid, @ttype, GETDATE(), 1, @fid);", conn))
                        {
                            cmd2.Parameters.AddWithValue("tid", textBox_TermID.Text);
                            cmd2.Parameters.AddWithValue("pid", PT0); //GET FROM PTO
                            cmd2.Parameters.AddWithValue("ttype", comboBoxTermT.Text);
                            cmd2.Parameters.AddWithValue("fid", comboBoxBJFID.Text);
                            cmd2.ExecuteNonQuery();
                            conn.Close();
                        }
                        MessageBox.Show("Creation Successful!");
                        log = DateTime.Now.ToString() + ": Terminal created - TerminalID: " + textBox_TermID.Text + " - Terminal Type: " + comboBoxTermT.Text + " - Participant ID: " + idMerc + " - FleetsID: " + textBoxFname.Text;
                        logger.Write(log);
                        log = string.Empty;
                        UpdateAllCombos();
                        UpdateALLGRIDS();
                        textBox_TermID.Text = "";
                        comboBoxBJFID.Text = "";
                        comboBoxTermT.Text = "";
                        panel4.Refresh();
                    }
                    catch
                    {
                        MessageBox.Show("Please fill out all fields");
                    }
                }
                else { MessageBox.Show("Terminal ID already existing"); }
            }
            else { MessageBox.Show("Please fill out all fields"); }
        }

        private void comboBoxBJFID_SelectedIndexChanged(object sender, EventArgs e)
        {
            CloseConn();
            using (SqlCommand cmd2 = new SqlCommand("select * from BeejeesFleets WHERE FleetsName = @pid", conn))
            {

                conn.Open();
                cmd2.Parameters.AddWithValue("pid", textBoxFname.Text);
                using (var reader2 = cmd2.ExecuteReader())
                {
                    reader2.Read();
                    PT0 = (reader2["PTO"].ToString());
                    comboBoxBJFID.Text = (reader2["ID"].ToString().ToUpper());
                }
                conn.Close();
                ShowDetails(textBoxFname, "BeejeesFleets", "FleetsName");
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            panelMenu.Visible = false;
            panelMenu2.Visible = true;
            panelMenu3.Visible = false;
            panelMenu4.Visible = false;
            buttonCMENU.BackColor = Color.FromArgb(64, 64, 64);
            buttonAMENU.BackColor = ColorTranslator.FromHtml("#00AEDB");
            buttonUMENU.BackColor = Color.FromArgb(64, 64, 64);
            buttonqsl.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void Button11_Click(object sender, EventArgs e)
        {
            panelMenu.Visible = true;
            panelMenu2.Visible = false;
            panelMenu3.Visible = false;
            panelMenu4.Visible = false;
            buttonCMENU.BackColor = ColorTranslator.FromHtml("#00AEDB");
            buttonAMENU.BackColor = Color.FromArgb(64, 64, 64);
            buttonUMENU.BackColor = Color.FromArgb(64, 64, 64);
            buttonqsl.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void buttonUMENU_Click(object sender, EventArgs e)
        {
            panelMenu.Visible = false;
            panelMenu2.Visible = false;
            panelMenu3.Visible = true;
            panelMenu4.Visible = false;
            buttonCMENU.BackColor = Color.FromArgb(64, 64, 64);
            buttonAMENU.BackColor = Color.FromArgb(64, 64, 64);
            buttonUMENU.BackColor = ColorTranslator.FromHtml("#00AEDB");
            buttonqsl.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void buttonqsl_Click(object sender, EventArgs e)
        {
            if (togDEL.Checked == true)
            {
                panelMenu.Visible = false;
                panelMenu2.Visible = false;
                panelMenu3.Visible = false;
                panelMenu4.Visible = true;
                buttonCMENU.BackColor = Color.FromArgb(64, 64, 64);
                buttonAMENU.BackColor = Color.FromArgb(64, 64, 64);
                buttonUMENU.BackColor = Color.FromArgb(64, 64, 64);
                buttonqsl.BackColor = ColorTranslator.FromHtml("#00AEDB");
            }
            else
            {
                MessageBox.Show("Enable Admin mode first");
                Button11_Click(sender, e);
            }

        }

        private void buttonm_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 0;
            titleLB.Text = "Merchants";
        }

        private void buttonBP_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 1;
            titleLB.Text = "BeejeesProfiles";
        }

        private void buttonF_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 2;
            titleLB.Text = "BeejeesFleets";
        }

        private void buttonT_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 3;
            titleLB.Text = "Terminals";
        }

        private void buttonDBRm_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 4;
            titleLB.Text = "Distance Based Routes";
        }

        private void buttonDFRT_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 5;
            titleLB.Text = "Discount Fare Tables";
        }

        private void buttonDBFRT_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 6;
            titleLB.Text = "Distance Based Fares Table";
        }

        private void buttonRBF_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 7;
            titleLB.Text = "Route Based Fares";
        }

        private void buttonRDF_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 8;
            titleLB.Text = "Route Discounted Fares";
        }

        private void buttonPR_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 9;
            titleLB.Text = "Profile Routes";
        }

        private void buttonpdb_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 10;
            titleLB.Text = "Profile Distance Based";
        }

        private void buttonpp_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 11;
            titleLB.Text = "Profile Parameters";
        }
        private void buttonSP_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 12;
            titleLB.Text = "System Parameters";
        }

        private void buttonDBIF_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 13;
            titleLB.Text = "Distance Based Increment Fares";
        }

        private void button_UA_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 14;
            titleLB.Text = "User Accounts";
        }

        private void buttonUC_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 15;
            titleLB.Text = "User Cards";
        }

        private void button14_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 16;
            titleLB.Text = "Insert SQL";
        }

        private void button15_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 17;
            titleLB.Text = "Update SQL";
        }

        private void button16_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 18;
            titleLB.Text = "Delete SQL";
        }

        private void button16_Click_1(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 19;
            titleLB.Text = "BusJeepneySettings";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 20;
            titleLB.Text = "Query Runner";
        }

 


        private void button4_Click_1(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 21;
            titleLB.Text = "SingleFixFare";
        }
        private void button5_Click_1(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 22;
            titleLB.Text = "Distance Based Card Profiles";
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 23;
            titleLB.Text = "Route Segments";
        }

        private void button51(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 24;
            titleLB.Text = "Profile Discounts";
        }
        private void buttonDBR_Create_Click(object sender, EventArgs e)
        {
            CloseConn();
            string fs;
            string act;

            if (textBoxBDR_RID.Text != "" && textBoxDBR_RSN.Text != "" && textBoxDBR_RLN.Text != "")
            {
                if (radioButton_debit.Checked == true || radioButton_cred.Checked == true)
                {
                    using (SqlCommand cmd1 = new SqlCommand("select COUNT(*) FROM DistanceBasedRoutes WHERE RouteID = @pid", conn))
                    {
                        conn.Open();
                        cmd1.CommandType = CommandType.Text;
                        cmd1.Parameters.AddWithValue("pid", textBoxBDR_RID.Text);
                        object obj1;
                        obj1 = cmd1.ExecuteScalar();
                        if (Convert.ToInt32(obj1) == 0)
                        {
                            //START CREATE
                            using (SqlCommand cmd2 = new SqlCommand("INSERT INTO DistanceBasedRoutes VALUES (NEWID(), @rid, @rsn, @rln, @fst, @act);", conn))
                            {

                                cmd2.Parameters.AddWithValue("rid", textBoxBDR_RID.Text);
                                cmd2.Parameters.AddWithValue("rsn", textBoxDBR_RSN.Text); //GET FROM PTO
                                cmd2.Parameters.AddWithValue("rln", textBoxDBR_RLN.Text);
                                if (radioButton_debit.Checked) { fs = "debit_debit"; } else { fs = "debit_credit"; }
                                cmd2.Parameters.AddWithValue("fst", fs);
                                if (checkBoxDBR.Checked) { act = "1"; } else { act = "0"; }
                                cmd2.Parameters.AddWithValue("act", act);
                                cmd2.ExecuteNonQuery();


                                conn.Close();
                            }
                            MessageBox.Show("Route created!");
                            textBoxBDR_RID.Text = "";
                            textBoxDBR_RSN.Text = "";
                            textBoxDBR_RLN.Text = "";
                            radioButton_debit.Checked = false;
                            radioButton_cred.Checked = false;
                            checkBoxDBR.Checked = false;
                            UpdateALLGRIDS();
                            UpdateAllCombos();
                        }
                        else { MessageBox.Show("Route ID already taken"); }
                    }

                }
                else { MessageBox.Show("Please choose Fare Strategy"); }
            }
            else
            { MessageBox.Show("Please fill out all fields!"); }
        }

        private void buttonDFT_Create_Click(object sender, EventArgs e)
        {
            CloseConn();
            string fs = "";
            string act;

            if (textBoxDFT_FTB.Text != "" && textBoxDFT_DISC.Text != "" && textBoxDFT_RA.Text != "")
            {
                if (radioButtonDFT_E.Checked == true || radioButtonDFT_U.Checked == true)
                {
                    using (SqlCommand cmd1 = new SqlCommand("select COUNT(*) FROM  DiscountFareTables WHERE FareTableID = @pid", conn))
                    {
                        conn.Open();
                        cmd1.CommandType = CommandType.Text;
                        cmd1.Parameters.AddWithValue("pid", textBoxDFT_FTB.Text);
                        object obj1;
                        obj1 = cmd1.ExecuteScalar();
                        if (Convert.ToInt32(obj1) == 0)
                        {
                            //START CREATE
                            using (SqlCommand cmd2 = new SqlCommand(@"INSERT INTO DiscountFareTables VALUES (NEWID(), @ftid, @dis, @rm, @ef, @et, 'DISCOUNT',@act,@ra);", conn))
                            {

                                cmd2.Parameters.AddWithValue("ftid", textBoxDFT_FTB.Text);
                                cmd2.Parameters.AddWithValue("dis", textBoxDFT_DISC.Text); //GET FROM PTO
                                cmd2.Parameters.AddWithValue("ef", metroDateTimeDFT_F.Value.Date.ToString());
                                cmd2.Parameters.AddWithValue("et", metroDateTimeDFT_T.Value.Date.ToString());
                                if (radioButtonDFT_E.Checked) { fs = "EXACT"; } else if (radioButtonDFT_U.Checked) { fs = "UP"; } else { fs = "DOWN"; }
                                cmd2.Parameters.AddWithValue("rm", fs);
                                if (checkBoxDFT_ACT.Checked) { act = "1"; } else { act = "0"; }
                                cmd2.Parameters.AddWithValue("act", act);
                                cmd2.Parameters.AddWithValue("ra", textBoxDFT_RA.Text);
                                cmd2.ExecuteNonQuery();
                                conn.Close();
                            }
                            MessageBox.Show("Discount Fare created!");
                            textBoxDFT_FTB.Text = "";
                            textBoxDFT_DISC.Text = "";
                            textBoxDFT_RA.Text = "";
                            radioButtonDFT_E.Checked = false;
                            radioButtonDFT_U.Checked = false;
                            checkBoxDFT_ACT.Checked = false;
                            metroDateTimeDFT_F.Value = DateTime.Now;
                            metroDateTimeDFT_T.Value = DateTime.Now;
                            UpdateALLGRIDS();
                            UpdateAllCombos();
                        }
                        else { MessageBox.Show("Fare Table ID already taken"); }
                    }

                }
                else { MessageBox.Show("Please choose Rounding Method"); }
            }
            else
            { MessageBox.Show("Please fill out all fields!"); }
        }

        private void buttonDBFT_Create_Click(object sender, EventArgs e)
        {
            CloseConn();
            string act, act2, act3;

            if (textBoxDBFT_FTI.Text != "" && textBoxDBFT_FA.Text != "" && textBoxDBFT_BFD.Text != "")
            {

                using (SqlCommand cmd1 = new SqlCommand("select COUNT(*) FROM DistanceBasedFareTables WHERE FareTableID = @pid", conn))
                {
                    conn.Open();
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("pid", textBoxDBFT_FTI.Text);
                    object obj1;
                    obj1 = cmd1.ExecuteScalar();
                    if (Convert.ToInt32(obj1) == 0)
                    {
                        //START CREATE
                        using (SqlCommand cmd2 = new SqlCommand(@"INSERT INTO DistanceBasedFareTables VALUES (NEWID(), @ftid, @fa, @bfd, @ef, @et, 'DISTANCE-BASED',@act, @act2, @act3);", conn))
                        {

                            cmd2.Parameters.AddWithValue("ftid", textBoxDBFT_FTI.Text);
                            cmd2.Parameters.AddWithValue("fa", textBoxDBFT_FA.Text);
                            cmd2.Parameters.AddWithValue("bfd", textBoxDBFT_BFD.Text);
                            cmd2.Parameters.AddWithValue("ef", metroDateTimeDBFT_F.Value.Date.ToString());
                            cmd2.Parameters.AddWithValue("et", metroDateTimeDBFT_T.Value.Date.ToString());
                            if (checkBoxDBFT_A.Checked) { act = "1"; } else { act = "0"; }
                            cmd2.Parameters.AddWithValue("act", act);
                            if (checkBoxAcc_DBFT.Checked) { act2 = "1"; } else { act2 = ""; }
                            cmd2.Parameters.AddWithValue("act2", act2);
                            if (checkBoxPro_DBFT.Checked) { act3 = "1"; } else { act3 = ""; }
                            cmd2.Parameters.AddWithValue("act3", act3);
                            cmd2.ExecuteNonQuery();

                            string FTID;
                            using (SqlCommand cmd3 = new SqlCommand("SELECT ID FROM DistanceBasedFareTables WHERE FareTableID=@pid", conn))
                            {
                                cmd3.Parameters.AddWithValue("pid", textBoxDBFT_FTI.Text);
                                using (var reader2 = cmd3.ExecuteReader())
                                {
                                    reader2.Read();
                                    FTID = (reader2[0].ToString());
                                }

                            }


                            conn.Close();
                        }
                        MessageBox.Show("Distance Based Fare Table created!");
                        textBoxDBFT_FTI.Text = "";
                        textBoxDBFT_FA.Text = "";
                        textBoxDBFT_BFD.Text = "";
                        checkBoxDBFT_A.Checked = false;
                        checkBoxAcc_DBFT.Checked = false;
                        checkBoxPro_DBFT.Checked = false;
                        metroDateTimeDBFT_F.Value = DateTime.Now;
                        metroDateTimeDBFT_T.Value = DateTime.Now;
                        UpdateALLGRIDS();
                        UpdateAllCombos();
                    }
                    else { MessageBox.Show("Fare Table ID already taken"); }
                }

            }
            else { MessageBox.Show("Please fill out all fields!"); }
        }

        private void metroComboBoxRBF_RID_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CloseConn();
                using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM DistanceBasedRoutes WHERE RouteLongName=@pid", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", metroComboBoxRBF_RID.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        reader2.Read();
                        RBF_RID = (reader2[0].ToString());
                    }
                    CloseConn();
                    ShowDetails(metroComboBoxRBF_RID, "DistanceBasedRoutes", "RouteLongName");
                }
            }
            catch { conn.Close(); }
        }



        /// <summary>
        /// SHOWDETAILS
        /// </summary>
        /// <param name="cbx"></param>
        /// <param name="tableName"></param>
        /// <param name="valClause"></param>
        public void ShowDetails(MetroFramework.Controls.MetroComboBox cbx, string tableName, string valClause)
        {
            try
            {
                int colCount = 0;
                if (cbx.Text != "")
                {

                    using (SqlCommand cmd = new SqlCommand("select count(*) from INFORMATION_SCHEMA.columns where TABLE_NAME = @tn", conn))
                    {
                        cmd.Parameters.AddWithValue("tn", tableName);
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                colCount = Convert.ToInt32(reader.GetValue(0));
                            }
                        }
                        conn.Close();
                    }

                    string str = "";
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM " + tableName + " WHERE " + valClause + "=@param", conn))
                    {
                        cmd.Parameters.AddWithValue("param", cbx.Text);
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var columns = new List<string>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    columns.Add(reader.GetName(i) + ": " + reader.GetValue(i));
                                }

                                str = string.Join(Environment.NewLine, columns);
                            }
                        }
                        conn.Close();
                    }

                    AutoClosingMessageBox.Show(str, "Table: " + tableName, 5000);
                }
                else { /*MessageBox.Show("Select Route first");*/ }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        public class AutoClosingMessageBox
        {
            System.Threading.Timer _timeoutTimer;
            string _caption;
            AutoClosingMessageBox(string text, string caption, int timeout)
            {
                _caption = caption;
                _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                    null, timeout, System.Threading.Timeout.Infinite);
                using (_timeoutTimer)
                    MessageBox.Show(text, caption);
            }
            public static void Show(string text, string caption, int timeout)
            {
                new AutoClosingMessageBox(text, caption, timeout);
            }
            void OnTimerElapsed(object state)
            {
                IntPtr mbWnd = FindWindow("#32770", _caption); // lpClassName is #32770 for MessageBox
                if (mbWnd != IntPtr.Zero)
                    SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                _timeoutTimer.Dispose();
            }
            const int WM_CLOSE = 0x0010;
            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
            [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        }


        private void metroComboBoxRBF_BFI_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CloseConn();
                using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM DistanceBasedFareTables WHERE FareTableID=@pid", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", metroComboBoxRBF_BFI.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        reader2.Read();
                        RBF_BFI = (reader2[0].ToString());
                    }
                    conn.Close();

                    ShowDetails(metroComboBoxRBF_BFI, "DistanceBasedFareTables", "FareTableID");
                }
            }
            catch { }
        }

        private void buttonRBF_Gen_Click(object sender, EventArgs e)
        {
            CloseConn();
            if (metroComboBoxRBF_BFI.Text == "" || metroComboBoxRBF_RID.Text == "")
            {
                MessageBox.Show("Please fill out all fields");
            }
            else
            {
                using (SqlCommand cmd2 = new SqlCommand("INSERT INTO RouteBasedFares VALUES (NEWID(), @pname, @pid);", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pname", RBF_RID);
                    cmd2.Parameters.AddWithValue("pid", RBF_BFI);
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                }
                metroComboBoxRBF_BFI.SelectedIndex = -1;
                metroComboBoxRBF_RID.SelectedIndex = -1;
                UpdateALLGRIDS();
                MessageBox.Show("Route Based Fare ID generated!");
            }
        }

        private void metroComboBoxRDBF_RID_SelectedIndexChanged(object sender, EventArgs e)
        {
            CloseConn();
            using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM DistanceBasedRoutes WHERE RouteLongName=@pid", conn))
            {
                conn.Open();
                cmd2.Parameters.AddWithValue("pid", metroComboBoxRDBF_RID.Text);
                using (var reader2 = cmd2.ExecuteReader())
                {
                    try
                    {
                        reader2.Read();
                        RDF_RID = (reader2[0].ToString());
                    }
                    catch
                    {
                        conn.Close();
                    }
                }
                conn.Close();
                ShowDetails(metroComboBoxRDBF_RID, "DistanceBasedRoutes", "RouteLongName");
            }
        }

        private void metroComboBoxRDBF_DID_SelectedIndexChanged(object sender, EventArgs e)
        {
            CloseConn();
            using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM DiscountFareTables WHERE FareTableID=@pid", conn))
            {
                conn.Open();
                cmd2.Parameters.AddWithValue("pid", metroComboBoxRDBF_DID.Text);
                using (var reader2 = cmd2.ExecuteReader())
                {
                    try
                    {
                        reader2.Read();
                        RDF_DID = (reader2[0].ToString());
                    }
                    catch
                    {
                        conn.Close();
                    }
                }
                conn.Close();
                ShowDetails(metroComboBoxRDBF_DID, "DiscountFareTables", "FareTableID");
            }
        }

        private void buttonRDBF_Gen_Click(object sender, EventArgs e)
        {
            CloseConn();
            if (metroComboBoxRDBF_DID.Text == "" || metroComboBoxRDBF_RID.Text == "")
            {
                MessageBox.Show("Please fill out all fields");
            }
            else
            {
                using (SqlCommand cmd2 = new SqlCommand("INSERT INTO RouteDiscountedFares VALUES (NEWID(), @pname, @pid);", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pname", RDF_RID);
                    cmd2.Parameters.AddWithValue("pid", RDF_DID);
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                }
                metroComboBoxRDBF_DID.SelectedIndex = -1;
                metroComboBoxRDBF_RID.SelectedIndex = -1;
                UpdateALLGRIDS();
                MessageBox.Show("Route Discounted Fare ID generated!");
            }
        }

        private void metroComboBoxPR_PID_SelectedIndexChanged(object sender, EventArgs e)
        {
            CloseConn();
            using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM BeejeesProfiles WHERE ProfileName=@pid", conn))
            {
                conn.Open();
                cmd2.Parameters.AddWithValue("pid", metroComboBoxPR_PID.Text);
                using (var reader2 = cmd2.ExecuteReader())
                {
                    try
                    {
                        reader2.Read();
                        PR_PID = (reader2[0].ToString());
                    }
                    catch
                    {
                        conn.Close();
                    }
                }
                conn.Close();
                ShowDetails(metroComboBoxPR_PID, "BeejeesProfiles", "ProfileName");
            }
        }

        private void metroComboBoxPR_RID_SelectedIndexChanged(object sender, EventArgs e)
        {
            CloseConn();
            using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM DistanceBasedRoutes WHERE RouteLongName=@pid", conn))
            {
                conn.Open();
                cmd2.Parameters.AddWithValue("pid", metroComboBoxPR_RID.Text);
                using (var reader2 = cmd2.ExecuteReader())
                {
                    try
                    {
                        reader2.Read();
                        PR_RID = (reader2[0].ToString());
                    }
                    catch
                    {
                        conn.Close();
                    }
                }
                conn.Close();
                ShowDetails(metroComboBoxPR_RID, "DistanceBasedRoutes", "RouteLongName");
            }
        }

        private void buttonPR_Gen_Click(object sender, EventArgs e)
        {
            CloseConn();
            if (metroComboBoxPR_PID.Text == "" || metroComboBoxPR_RID.Text == "")
            {
                MessageBox.Show("Please fill out all fields");
            }
            else
            {
                using (SqlCommand cmd2 = new SqlCommand("INSERT INTO ProfileRoutes VALUES (NEWID(), @pname, @pid);", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pname", PR_PID);
                    cmd2.Parameters.AddWithValue("pid", PR_RID);
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                }
                metroComboBoxPR_PID.SelectedIndex = -1;
                metroComboBoxPR_RID.SelectedIndex = -1;
                UpdateALLGRIDS();
                MessageBox.Show("Profile Routes ID generated!");
            }
        }

        private void RBF_MH(object sender, EventArgs e)
        {
            MessageBox.Show("- Route ID from DistanceBasedRoutes Table" + Environment.NewLine + "- Base Fare ID from DistanceBasedFareTables");
        }

        private void metroComboBoxPDB_PID_SelectedIndexChanged(object sender, EventArgs e)
        {
            CloseConn();
            using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM BeejeesProfiles WHERE ProfileName=@pid", conn))
            {
                try
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", metroComboBoxPDB_PID.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        reader2.Read();
                        PDB_PID = (reader2[0].ToString());
                    }
                    conn.Close();
                    ShowDetails(metroComboBoxPDB_PID, "BeejeesProfiles", "ProfileName");
                }
                catch { }
            }
        }

        private void metroComboBoxPDB_BID_SelectedIndexChanged(object sender, EventArgs e)
        {
            CloseConn();
            using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM DistanceBasedFareTables WHERE FareTableID=@pid", conn))
            {
                conn.Open();
                cmd2.Parameters.AddWithValue("pid", metroComboBoxPDB_BID.Text);
                using (var reader2 = cmd2.ExecuteReader())
                {
                    try
                    {
                        reader2.Read();
                        PDB_BID = (reader2[0].ToString());
                    }
                    catch
                    {
                        conn.Close();
                    }
                }
                conn.Close();
                ShowDetails(metroComboBoxPDB_BID, "DistanceBasedFareTables", "FaretableID");
            }
        }

        private void buttonPDB_Gen_Click(object sender, EventArgs e)
        {
            CloseConn();
            if (metroComboBoxPDB_PID.Text == "" || metroComboBoxPDB_BID.Text == "")
            {
                MessageBox.Show("Please fill out all fields");
            }
            else
            {
                using (SqlCommand cmd2 = new SqlCommand("INSERT INTO ProfileDistanceBaseds VALUES (NEWID(), @pname, @pid);", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pname", PDB_PID);
                    cmd2.Parameters.AddWithValue("pid", PDB_BID);
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                }
                metroComboBoxPDB_PID.SelectedIndex = -1;
                metroComboBoxPDB_BID.SelectedIndex = -1;
                UpdateALLGRIDS();
                MessageBox.Show("Profile Distance Based ID generated!");
            }
        }

        private void textBoxSP_V_TextChanged(object sender, EventArgs e)
        {
            SysParamsCombo();
            if (textBoxSP_V.Text != string.Empty)
            {
                CloseConn();
                int i;
                if (!int.TryParse(textBoxSP_V.Text, out i))
                { if (metroComboBoxSP_T.Text == "Int64") { MessageBox.Show("Failed to Parse Int."); textBoxSP_V.Text = ""; } }
            }


            //conn.Open();

            //string selectSQLR = "SELECT Parameter, COUNT(VALUE) AS CountVal,Value FROM SystemParameters Where Parameter='AcquirerID' OR Parameter='ParticipantID' OR Parameter='ParticipantName' GROUP BY Value,Parameter HAVING COUNT(VALUE) > 0 ";

            //SqlCommand cmdR = new SqlCommand(selectSQLR, conn);
            //SqlDataReader rdR;
            //rdR = cmdR.ExecuteReader();

            //while (rdR.Read())
            //{
            //    string rdrr = rdR.GetString(0);
            //    string val = rdR.GetString(2).ToString();
            //    if (textBoxSP_V.Text == val)
            //    {

            //        metroComboBoxSP_P.Items.Remove(rdrr);
            //    }
            
            //}

            //rdR.Close();
            //conn.Close();

        }

        private void InsSysParams()
        {
            CloseConn();
            using (SqlCommand cmd2 = new SqlCommand("INSERT INTO SystemParameters VALUES (NEWID(), @param, @type, @val, null, null);", conn))
            {
                conn.Open();
                cmd2.Parameters.AddWithValue("param", metroComboBoxSP_P.Text);
                cmd2.Parameters.AddWithValue("type", metroComboBoxSP_T.Text);
                cmd2.Parameters.AddWithValue("val", textBoxSP_V.Text);
                cmd2.ExecuteNonQuery();
                conn.Close();
            }
            //metroComboBoxPDB_PID.SelectedIndex = -1;
            //metroComboBoxPDB_BID.SelectedIndex = -1;
            textBoxSP_V.Text = "";
            UpdateALLGRIDS();
            UpdateAllCombos();
            MessageBox.Show("System Parameters Based ID generated!");
        }
        private void buttonSP_Gen_Click(object sender, EventArgs e)
        {
            CloseConn();
            if (metroComboBoxSP_P.Text == "" || metroComboBoxSP_T.Text == "" || textBoxSP_V.Text == "")
            {
                MessageBox.Show("Please fill out all fields");
            }
            else
            {

                if (metroComboBoxSP_T.Text == "string")
                {
                    InsSysParams();
                }
                else
                {
                    int i;
                    if (!int.TryParse(textBoxSP_V.Text, out i))
                    { MessageBox.Show("Value cannot be coverted to Int64"); }
                    else
                    {
                        InsSysParams();
                    }
                }
            }
        }

        private void buttonGenPP_Click(object sender, EventArgs e)
        {
            string PP_PID;
            CloseConn();

            if (metroComboBoxPP_PID.Text == "" || metroComboBoxPP_SPI.Text == "")
            {
                MessageBox.Show("Please fill out all fields");
            }
            else
            {
                using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM BeejeesProfiles WHERE ProfileID=@param;", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("param", metroComboBoxPP_PID.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        reader2.Read();
                        PP_PID = (reader2[0].ToString());
                    }
                    conn.Close();
                }

                using (SqlCommand cmd2 = new SqlCommand("INSERT INTO ProfileParameters VALUES (NEWID(), @param, @type);", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("param", PP_PID);
                    cmd2.Parameters.AddWithValue("type", metroComboBoxPP_SPI.Text);
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                }
                metroComboBoxPP_PID.SelectedIndex = -1;
                metroComboBoxPP_SPI.SelectedIndex = -1;
                UpdateALLGRIDS();
                MessageBox.Show("Profile Parameters ID generated!");
            }
        }

        private void buttonBPdel_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";
            if (dataGridViewBP.SelectedCells.Count > 0)
            {
                int selectedrowindex = dataGridViewBP.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dataGridViewBP.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM BeejeesProfiles WHERE ID=@param", conn))
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
                UpdateAllCombos();
            }

        }

        private void textBoxCreate_PID_TextChanged(object sender, EventArgs e)
        {
            CloseConn();
            //textBoxCreate_PID.Text = textBoxCreate_PID.Text.TrimStart(new Char[] { '0' });
            try
            {
                using (SqlCommand cmd1 = new SqlCommand("select COUNT(*) FROM Merchants WHERE ParticipantID = @pid", conn))
                {
                    conn.Open();
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("pid", textBoxCreate_PID.Text);
                    object obj1;
                    obj1 = cmd1.ExecuteScalar();
                    if (textBoxCreate_PID.Text != "")
                    {
                        PB_PIDCHECKER.Visible = true;
                    }
                    if (Convert.ToInt32(obj1) == 0 && textBoxCreate_PID.Text != "")
                    {
                        PB_PIDCHECKER.BackgroundImage = Properties.Resources.check;

                    }
                    else
                    {
                        PB_PIDCHECKER.BackgroundImage = Properties.Resources.ex;
                    }
                    conn.Close();
                }
            }
            catch { }
        }

        private void DBIF_ftid_SelectedIndexChanged(object sender, EventArgs e)
        {
            CloseConn();
            try
            {
                string ftid;
                using (SqlCommand cmd2 = new SqlCommand("select ID, BaseFareDistance from DistanceBasedFareTables WHERE FareTableId = @pid", conn))
                {

                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", DBIF_ftid.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        reader2.Read();
                        ftid = (reader2[0].ToString());
                        DBIF_sd.Text = (reader2[1].ToString());
                    }
                    conn.Close();
                    ShowDetails(DBIF_ftid, "DistanceBasedFareTables", "FareTableID");
                }
            }
            catch
            {
                CloseConn();
                DBIF_ftid.Text = "";
                DBIF_ifa.Text = "";
                DBIF_sd.Text = "";
                DBIF_ifd.Text = "";
            }
        }

        private void buttonCreate_DBIF_Click(object sender, EventArgs e)
        {
            CloseConn();
            string ftid = "";
            if (DBIF_ifa.Text == "" || DBIF_sd.Text == "" || DBIF_ifd.Text == "" || DBIF_ftid.Text == "")
            {
                MessageBox.Show("Please fill out all fields.");

            }
            else
            {

                using (SqlCommand cmd2 = new SqlCommand("select ID from DistanceBasedFareTables WHERE FareTableId = @pid", conn))
                {

                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", DBIF_ftid.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        reader2.Read();
                        ftid = (reader2[0].ToString());
                    }
                    conn.Close();

                }
                object obj2;
                using (SqlCommand cmd3 = new SqlCommand("select COUNT(*) FROM DistanceBasedIncrementFares WHERE FareTable_Id=@pn", conn)) //check if PID is existing
                {
                    conn.Open();
                    cmd3.CommandType = CommandType.Text;
                    cmd3.Parameters.AddWithValue("pn", ftid);
                    obj2 = cmd3.ExecuteScalar();
                    conn.Close();
                }

                if (Convert.ToInt32(obj2) == 0)
                {

                    using (SqlCommand cmd2 = new SqlCommand("INSERT INTO DistanceBasedIncrementFares VALUES (NEWID(), @fname, @pto, @pid, @sid);", conn))
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("fname", ftid);
                        cmd2.Parameters.AddWithValue("pto", DBIF_ifa.Text);
                        cmd2.Parameters.AddWithValue("pid", DBIF_ifd.Text);
                        cmd2.Parameters.AddWithValue("sid", DBIF_sd.Text);
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                    }
                    MessageBox.Show("Creation Successful!");
                    DBIF_ftid.Text = "";
                    DBIF_ifa.Text = "";
                    DBIF_sd.Text = "";
                    DBIF_ifd.Text = "";
                    UpdateAllCombos();
                    UpdateALLGRIDS();
                }
                else
                {
                    MessageBox.Show("Increment Fares already exists");
                    DBIF_ftid.Text = "";
                    DBIF_ifa.Text = "";
                    DBIF_sd.Text = "";
                    DBIF_ifd.Text = "";
                }

            }
        }

        private void textBoxCreatePname_TextChanged(object sender, EventArgs e)
        {
            CloseConn();
            using (SqlCommand cmd1 = new SqlCommand("select COUNT(*) FROM Merchants WHERE ParticipantName = @pid", conn))
            {
                conn.Open();
                cmd1.CommandType = CommandType.Text;
                cmd1.Parameters.AddWithValue("pid", textBoxCreatePname.Text);
                object obj1;
                obj1 = cmd1.ExecuteScalar();
                if (textBoxCreatePname.Text != "")
                {
                    PB_PNCHECKER.Visible = true;
                }
                if (Convert.ToInt32(obj1) == 0 && textBoxCreatePname.Text != "")
                {
                    PB_PNCHECKER.BackgroundImage = Properties.Resources.check;

                }
                else
                {
                    PB_PNCHECKER.BackgroundImage = Properties.Resources.ex;
                }
                conn.Close();
            }
        }

        private void buttonBFdel_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";
            if (dataGridViewFleets.SelectedCells.Count > 0)
            {
                int selectedrowindex = dataGridViewFleets.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dataGridViewFleets.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM BeejeesFleets WHERE ID=@param", conn))
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
                UpdateAllCombos();
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";
            if (metroGridFleets.SelectedCells.Count > 0)
            {
                int selectedrowindex = metroGridFleets.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = metroGridFleets.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[2].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM Terminals WHERE ID=@param", conn))
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
                UpdateAllCombos();
                panel4.Refresh();
                panelTerminal2.Visible = false;
                buttonST.Visible = false;
                buttonTerminalUP.Enabled = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";
            if (dgvDBR.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvDBR.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvDBR.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[3].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM DistanceBasedRoutes WHERE ID=@param", conn))
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
                UpdateAllCombos();
            }
        }



        private void buttonDELDFT_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";
            if (dgvDFT.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvDFT.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvDFT.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM DiscountFareTables WHERE ID=@param", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("param", b);
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("Deleted!");
                    }
                    catch { MessageBox.Show("Delete Constrait first.);"); }
                }

                UpdateALLGRIDS();
                UpdateAllCombos();
            }
        }

        private void buttonDELDBFT_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";
            if (dgvDBFT.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvDBFT.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvDBFT.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM DistanceBasedFareTables WHERE ID=@param", conn))
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
                UpdateAllCombos();
            }
        }

        private void buttonDELDBIF_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";
            if (dgvDBIF.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvDBIF.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvDBIF.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM DistanceBasedIncrementFares WHERE ID=@param", conn))
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
                UpdateAllCombos();
            }
        }

        private void buttonDELSP_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";
            if (dgvSP.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvSP.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvSP.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[3].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM SystemParameters WHERE ID=@param", conn))
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
                UpdateAllCombos();
            }
        }

        private void buttonDELRBF_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";
            if (dgvRBF.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvRBF.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvRBF.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM RouteBasedFares WHERE ID=@param", conn))
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
                UpdateAllCombos();
            }
        }
   
        private void buttonDELRDF_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";
            if (dgvRDF.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvRDF.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvRDF.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM RouteDiscountedFares WHERE ID=@param", conn))
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
                UpdateAllCombos();
            }
        }

        private void buttonDELPR_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";
            if (dgvPR.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvPR.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvPR.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM ProfileRoutes WHERE ID=@param", conn))
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
                UpdateAllCombos();
            }
        }

        private void buttonDELPDB_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";
            if (dgvPDB.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvPDB.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvPDB.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM ProfileDistanceBaseds WHERE ID=@param", conn))
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
                UpdateAllCombos();
            }
        }

        private void buttonDELPP_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";
            if (dgvPP.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvPP.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvPP.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM ProfileParameters WHERE ID=@param", conn))
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
                UpdateAllCombos();
            }

        }

        private void buttonDELMERC_Click(object sender, EventArgs e)
        {
            CloseConn();
            if (comboBoxMerchantList.Text != "") //&& textBoxEdit_Pname.Text != "" && textBox_PSname.Text != ""
            {
                DialogResult dialogResult = MessageBox.Show("Delete " + textBoxEdit_Pname.Text + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (dialogResult == DialogResult.Yes)
                {
                    using (SqlCommand cmd2 = new SqlCommand("DELETE FROM Merchants WHERE ParticipantID=@param", conn))
                    {
                        try
                        {
                            conn.Open();
                            cmd2.Parameters.AddWithValue("param", comboBoxMerchantList.Text);
                            cmd2.ExecuteNonQuery();
                            conn.Close();
                            MessageBox.Show("Deleted!");
                        }
                        catch
                        {
                            MessageBox.Show("Delete Constrait first.");
                        }
                    }

                    UpdateALLGRIDS();
                    UpdateAllCombos();
                    textBoxEdit_Pname.Text = "";
                    textBox_PSname.Text = "";
                    comboBoxMerchantList.SelectedIndex = -1;
                }
            }
            else
            {
                MessageBox.Show("Please select Merchant.");
            }

        }

        private void buttonUP_UA_Click(object sender, EventArgs e)
        {
            CloseConn();
            string act = "";

            if (textBox_UA_UID.Text != "" && textBox_UA_LN.Text != "" && textBox_UA_SN.Text != "" && metroComboBoxUA_MN.Text != "" && textBoxUA_CID.Text != "")
            {

                using (SqlCommand cmd1 = new SqlCommand("select COUNT(*) FROM UserAccounts WHERE ID = @pid", conn))
                {
                    conn.Open();
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("pid", textBox_UA_UID.Text);
                    object obj1;
                    obj1 = cmd1.ExecuteScalar();
                    if (Convert.ToInt32(obj1) == 0)
                    {
                        using (SqlCommand cmd2 = new SqlCommand("select COUNT(*) FROM UserAccounts WHERE LongName = @pid OR ShortName= @pids", conn))
                        {

                            cmd2.CommandType = CommandType.Text;
                            cmd2.Parameters.AddWithValue("pid", textBox_UA_LN.Text);
                            cmd2.Parameters.AddWithValue("pids", textBox_UA_SN.Text);
                            object obj2;
                            obj2 = cmd1.ExecuteScalar();
                            if (Convert.ToInt32(obj2) == 0)
                            {

                                if (conn == null || conn.State == ConnectionState.Open)
                                    conn.Close();
                                using (SqlCommand cmd = new SqlCommand("SELECT ID FROM Merchants WHERE ParticipantName=@pid", conn))
                                {
                                    try
                                    {
                                        conn.Open();
                                        cmd.Parameters.AddWithValue("pid", metroComboBoxUA_MN.Text);
                                        using (var reader2 = cmd.ExecuteReader())
                                        {
                                            reader2.Read();
                                            act = (reader2[0].ToString());
                                        }

                                    }
                                    catch { }
                                }

                                //START CREATE
                                using (SqlCommand cmd3 = new SqlCommand(@"INSERT INTO UserAccounts VALUES (NEWID(), @ftid, @fa, @bfd,@bfd1,@bfd2, @ef, @et, GETDATE() , 1);", conn))
                                {

                                    cmd3.Parameters.AddWithValue("ftid", textBox_UA_UID.Text);
                                    cmd3.Parameters.AddWithValue("fa", act);
                                    cmd3.Parameters.AddWithValue("bfd", textBoxUA_CID.Text);
                                    cmd3.Parameters.AddWithValue("bfd1", textBox_UA_SN.Text);
                                    cmd3.Parameters.AddWithValue("bfd2", textBox_UA_LN.Text);
                                    cmd3.Parameters.AddWithValue("ef", metroDateTimeEDUA.Value.Date.ToString());
                                    cmd3.Parameters.AddWithValue("et", metroDateTimeEDUA2.Value.Date.ToString());
                                    cmd3.ExecuteNonQuery();

                                    conn.Close();
                                }

                                textBox_UA_UID.Text = "";
                                textBox_UA_LN.Text = "";
                                textBox_UA_SN.Text = "";
                                metroComboBoxUA_MN.SelectedIndex = -1;
                                metroDateTimeEDUA.Value = DateTime.Now;
                                metroDateTimeEDUA2.Value = DateTime.Now;
                                UpdateALLGRIDS();
                                UpdateAllCombos();
                                MessageBox.Show("User Account created!");
                            }
                            else
                            {
                                MessageBox.Show("User Name already taken");
                            }
                        }
                    }
                    else { MessageBox.Show("UID already taken"); }
                }

            }
            else { MessageBox.Show("Please fill out all fields!"); }
        }

        private void buttonDELUA_Click(object sender, EventArgs e)
        {
            string a = "";
            string b = "";
            if (dgvUA.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvUA.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvUA.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[4].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM UserAccounts WHERE UserAccountID=@param", conn))
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
                UpdateAllCombos();
            }
        }
        private void metroComboBoxSP_P2_TextChanged(object sender, EventArgs e)
        {
            if (metroComboBoxSP_P2.Text == "TransactionUploadInterval") metroComboBoxSP_T2.Text = "Int64";
            else { metroComboBoxSP_T2.Text = "string"; }
            if (metroComboBoxSP_P2.Text == "AcquirerID") metroComboBoxSP_T2.Text = "Int64";
            if (metroComboBoxSP_P2.Text == "FacilityCode") metroComboBoxSP_T2.Text = "Int64";
            if (metroComboBoxSP_P2.Text == "HeartBeatInterval") metroComboBoxSP_T2.Text = "Int64";
            if (metroComboBoxSP_P2.Text == "NumberOfIncrements") metroComboBoxSP_T2.Text = "Int64";
            if (metroComboBoxSP_P2.Text == "ParticipantId") metroComboBoxSP_T2.Text = "Int64";
            if (metroComboBoxSP_P2.Text == "ScreenTimeout") metroComboBoxSP_T2.Text = "Int64";
            if (metroComboBoxSP_P2.Text == "TransactionUploadCount") metroComboBoxSP_T2.Text = "Int64";
        }

        private void metroComboBoxSP_P_TextChanged(object sender, EventArgs e)
        {
            {
                if (metroComboBoxSP_P.Text == "TransactionUploadInterval") metroComboBoxSP_T.Text = "Int64";
                else { metroComboBoxSP_T.Text = "string"; }
                if (metroComboBoxSP_P.Text == "AcquirerID") metroComboBoxSP_T.Text = "Int64";
                if (metroComboBoxSP_P.Text == "FacilityCode") metroComboBoxSP_T.Text = "Int64";
                if (metroComboBoxSP_P.Text == "HeartBeatInterval") metroComboBoxSP_T.Text = "Int64";
                if (metroComboBoxSP_P.Text == "NumberOfIncrements") metroComboBoxSP_T.Text = "Int64";
                if (metroComboBoxSP_P.Text == "ParticipantId") metroComboBoxSP_T.Text = "Int64";
                if (metroComboBoxSP_P.Text == "ScreenTimeout") metroComboBoxSP_T.Text = "Int64";
                if (metroComboBoxSP_P.Text == "TransactionUploadCount") metroComboBoxSP_T.Text = "Int64";
            }
        }
        private void buttonUCADD_Click(object sender, EventArgs e)
        {
            string PP_PID = "";
            CloseConn();

            if (textBoxUC_UID.Text == "" || metroComboBoxUC_UN.Text == "" || radioButtonDRIVER.Checked == false && radioButtonDISPATCHER.Checked == false)
            {
                MessageBox.Show("Please fill out all fields");
            }
            else
            {
                using (SqlCommand cmd2 = new SqlCommand("SELECT UserAccountID FROM UserAccounts WHERE ID=@param;", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("param", metroComboBoxUC_UN.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        reader2.Read();
                        PP_PID = (reader2[0].ToString());
                    }
                    conn.Close();
                }

                using (SqlCommand cmd2 = new SqlCommand("INSERT INTO UserCards VALUES (NEWID(), @param, @type, @bfd2, @ef, @et, NULL);", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("param", PP_PID);
                    cmd2.Parameters.AddWithValue("type", textBoxUC_UID.Text);
                    if (radioButtonDRIVER.Checked)
                        cmd2.Parameters.AddWithValue("bfd2", radioButtonDRIVER.Text);
                    else
                        cmd2.Parameters.AddWithValue("bfd2", radioButtonDISPATCHER.Text);
                    cmd2.Parameters.AddWithValue("ef", metroDateTimeUCED.Value.Date.ToString());
                    cmd2.Parameters.AddWithValue("et", metroDateTimeUCED2.Value.Date.ToString());
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                }
                metroComboBoxUC_UN.SelectedIndex = -1;
                radioButtonDRIVER.Checked = false;
                radioButtonDISPATCHER.Checked = false;
                textBoxUC_UID.Text = "";
                UpdateALLGRIDS();
                MessageBox.Show("UserCard Created!");
            }
        }

        private void buttonUC_DEL_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";
            string c = "";
            string d = "";
            if (dgvUC.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvUC.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvUC.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
                c = Convert.ToString(selectedRow.Cells[2].Value);
                d = Convert.ToString(selectedRow.Cells[3].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + " - " + c + " (" + d + ")?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM UserCards WHERE ID=@param", conn))
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
                UpdateAllCombos();
            }
        }

        private void dataGridViewFleets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            CloseConn();
            if (togDEL.Checked)
            {
                buttonBF.Enabled = false;
                if (dataGridViewFleets.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    holder2 = dataGridViewFleets.SelectedRows[0].Cells[3].Value.ToString();
                    string userId = dataGridViewFleets.SelectedRows[0].Cells[1].Value.ToString();
                    holder1 = dataGridViewFleets.SelectedRows[0].Cells[0].Value.ToString();
                    textBoxNewPIDBF.Text = holder2;
                    textBoxFleetsName.Text = userId;
                    buttonSAVE_Fleets.Visible = true;
                    label136.Visible = true;
                    textBoxNewPIDBF.Visible = true;
                    label135.Visible = true;
                    label20d.Text = "New Fleets Name:";
                    panelBF.Visible = true;
                    textBoxFleetsName.Size = new Size(188, 41);
                    textBoxFleetsName.ReadOnly = false;
                    textBoxAppend.Visible = false;
                    label23.Visible = false;
                    IsMenuDisabled(true, buttonBFdel);
                    using (SqlCommand cmd2 = new SqlCommand("select ParticipantID from Merchants WHERE ID = @pid", conn))
                    {

                        conn.Open();
                        cmd2.Parameters.AddWithValue("pid", dataGridViewFleets.SelectedRows[0].Cells[2].Value.ToString());
                        using (var reader2 = cmd2.ExecuteReader())
                        {
                            reader2.Read();
                            comboBoxMercBP.Text = (reader2[0].ToString());
                        }
                        conn.Close();
                    }
                }
            }
        }

        private void buttonSAVE_Fleets_Click(object sender, EventArgs e)
        {
            try
            {
                CloseConn();

                if (textBoxNewPIDBF.Text == "" || textBoxFleetsName.Text == "")
                {
                    MessageBox.Show("Please fill out all fields.");

                }
                else
                {
                    if (conn == null || conn.State == ConnectionState.Open)
                        conn.Close();

                    try
                    {
                        using (SqlCommand cmd2 = new SqlCommand("select ID from Merchants WHERE ParticipantID = @pid", conn))
                        {

                            conn.Open();
                            cmd2.Parameters.AddWithValue("pid", comboBoxMercBP.Text);
                            using (var reader2 = cmd2.ExecuteReader())
                            {
                                reader2.Read();
                                idMerc = (reader2[0].ToString());
                            }
                            conn.Close();
                        }
                        using (SqlCommand cmd2 = new SqlCommand("select ID from BeejeesProfiles WHERE ProfileID = @pid", conn))
                        {

                            conn.Open();
                            cmd2.Parameters.AddWithValue("pid", textBoxNewPIDBF.Text);
                            using (var reader2 = cmd2.ExecuteReader())
                            {
                                reader2.Read();
                                holder2 = (reader2[0].ToString());
                            }
                            conn.Close();
                        }

                    }
                    catch
                    {
                        idMerc = "";
                        conn.Close();
                    }

                    using (SqlCommand cmd2 = new SqlCommand("UPDATE BeejeesFleets SET ProfileID=@pid, FleetsName=@pto, PTO=@ptos WHERE ID=@fname", conn))
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("fname", holder1);
                        cmd2.Parameters.AddWithValue("pto", textBoxFleetsName.Text);
                        cmd2.Parameters.AddWithValue("ptos", idMerc);
                        cmd2.Parameters.AddWithValue("pid", holder2);
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                    }
                    label136.Visible = false;
                    textBoxNewPIDBF.Visible = false;
                    label135.Visible = false;
                    MessageBox.Show("Update Successful!");
                    log = DateTime.Now.ToString() + ": BeejeesFleets generated - FleetsName: " + textBoxFleetsName.Text + " - PID: " + uni;
                    logger.Write(log);
                    label20d.Text = "Fleets Name:";
                    log = string.Empty;
                    UpdateAllCombos();
                    textBoxFleetsName.Text = "";
                    buttonSAVE_Fleets.Visible = false;
                    buttonBF.Enabled = true;
                    UpdateAllCombos();
                    textBoxNewPIDBF.Text = "";
                    UpdateALLGRIDS();
                    holder2 = "";
                    holder1 = "";
                    textBoxFleetsName.Size = new Size(140, 34);
                    textBoxFleetsName.ReadOnly = true;
                    panelBF.Visible = false;
                    IsMenuDisabled(false, buttonBFdel);
                    textBoxAppend.Visible = true;
                    label23.Visible = true;
                }
            }
            catch
            {
                MessageBox.Show("Profile ID does not exist");
                if (conn == null || conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }



        private void comboBoxBF_TextChanged(object sender, EventArgs e)
        {
            try
            {
                CloseConn();
                string str;
                string pname;
                object obj;

                using (SqlCommand cmd2 = new SqlCommand("select ProfileName from BeejeesProfiles WHERE ProfileID = @pid", conn))
                {

                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", comboBoxBF.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        reader2.Read();
                        str = (reader2["ProfileName"].ToString());
                    }
                    conn.Close();
                }
                textBoxFleetsName.Text = str;

                using (SqlCommand cmd2 = new SqlCommand("select * from BeejeesProfiles WHERE ProfileID = @pid", conn))
                {

                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", comboBoxBF.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        reader2.Read();
                        str = (reader2["ID"].ToString());
                        pname = (reader2["ProfileName"].ToString());
                    }
                    conn.Close();
                    uni = str;
                }

                using (SqlCommand cmd2 = new SqlCommand("select * from Merchants WHERE ParticipantName = @pid", conn))
                {

                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", pname);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        reader2.Read();
                        idMerc = (reader2["ID"].ToString());
                    }
                    conn.Close();

                }

                using (SqlCommand cmd1 = new SqlCommand("select COUNT(*) FROM BeejeesFleets WHERE ProfileID=@pn", conn))
                {
                    conn.Open();
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("pn", uni);
                    obj = cmd1.ExecuteScalar();
                }
            }
            catch
            {
                textBoxFleetsName.Text = "";
            }
        }

        private void metroGridFleets_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            CloseConn();
            holder1 = "";
            if (metroGridFleets.SelectedRows.Count > 0 && togDEL.Checked) // make sure user select at least 1 row 
            {
                panelTerminal2.Visible = true;
                buttonST.Visible = true;
                buttonTerminalUP.Enabled = false;
                holder1 = metroGridFleets.SelectedRows[0].Cells[0].Value.ToString();
                metroComboBoxFleet.Text = metroGridFleets.SelectedRows[0].Cells[1].Value.ToString();
                textBoxNewTID.Text = metroGridFleets.SelectedRows[0].Cells[2].Value.ToString();
                metroComboBoxTType.Text = metroGridFleets.SelectedRows[0].Cells[3].Value.ToString();
                IsMenuDisabled(true, buttonDelT);
            }
        }

        private void metroComboBoxFleet_SelectedIndexChanged(object sender, EventArgs e)
        {
            CloseConn();

            PT0 = "";
            using (SqlCommand cmd2 = new SqlCommand("select PTO from BeejeesFleets WHERE FleetsName = @pid", conn))
            {

                conn.Open();
                cmd2.Parameters.AddWithValue("pid", metroComboBoxFleet.Text);
                using (var reader2 = cmd2.ExecuteReader())
                {
                    reader2.Read();
                    PT0 = (reader2[0].ToString());

                }
                conn.Close();
                ShowDetails(metroComboBoxFleet, "BeejeesFleets", "FleetsName");
            }
        }

        private void buttonST_Click(object sender, EventArgs e)
        {
            holder2 = "";
            {
                try
                {
                    CloseConn();

                    if (metroComboBoxFleet.Text == "" || textBoxNewTID.Text == "" || metroComboBoxTType.Text == "")
                    {
                        MessageBox.Show("Please fill out all fields.");
                    }
                    else
                    {
                        using (SqlCommand cmd2 = new SqlCommand("Select ID from BeejeesFleets WHERE FleetsName=@fname", conn))
                        {
                            conn.Open();
                            cmd2.Parameters.AddWithValue("fname", metroComboBoxFleet.Text);
                            using (var reader2 = cmd2.ExecuteReader())
                            {
                                try
                                {
                                    reader2.Read();
                                    holder2 = (reader2[0].ToString());
                                }
                                catch
                                {
                                    conn.Close();
                                }
                            }

                            conn.Close();
                        }


                        using (SqlCommand cmd2 = new SqlCommand("UPDATE Terminals SET TerminalID=@pid, ParticipantID=@pto, TerminalType=@tt, FleetID=@fid WHERE ID=@fname", conn))
                        {
                            conn.Open();
                            cmd2.Parameters.AddWithValue("fname", holder1);
                            cmd2.Parameters.AddWithValue("pto", PT0);
                            cmd2.Parameters.AddWithValue("pid", textBoxNewTID.Text);
                            cmd2.Parameters.AddWithValue("tt", metroComboBoxTType.Text);
                            cmd2.Parameters.AddWithValue("fid", holder2);
                            cmd2.ExecuteNonQuery();
                            conn.Close();
                        }
                        panelTerminal2.Visible = false;
                        MessageBox.Show("Update Successful!");
                        UpdateAllCombos();
                        textBoxNewTID.Text = "";
                        buttonST.Visible = false;
                        buttonTerminalUP.Enabled = true;
                        UpdateAllCombos();

                        UpdateALLGRIDS();
                        holder2 = "";
                        holder1 = "";
                        IsMenuDisabled(false, buttonDelT);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    CloseConn();
                }
            }
        }

        private void dgvDBR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            CloseConn();
            holder1 = "";
            if (dgvDBR.SelectedRows.Count > 0 && togDEL.Checked) // make sure user select at least 1 row 
            {
                buttonSAVEDBR.Visible = true;
                panelDBR.Visible = true;
                buttonDBR_Create.Enabled = false;
                holder1 = dgvDBR.SelectedRows[0].Cells[0].Value.ToString();
                textBoxNRID_DBR.Text = dgvDBR.SelectedRows[0].Cells[1].Value.ToString();
                textBoxNRSN_DBR.Text = dgvDBR.SelectedRows[0].Cells[2].Value.ToString();
                textBoxRLN_DBR.Text = dgvDBR.SelectedRows[0].Cells[3].Value.ToString();
                if (dgvDBR.SelectedRows[0].Cells[3].Value.ToString() == "debit_debit") { radioButtonNddDBR.Checked = true; }
                else { radioButtonNdcDBR.Checked = true; }
                if (dgvDBR.SelectedRows[0].Cells[5].Value.ToString() == "True") { checkBoxNDBR.Checked = true; }
                else { checkBoxNDBR.Checked = true; }
                IsMenuDisabled(true, buttonDELDBR);
            }
        }

        private void buttonRUNins_Click(object sender, EventArgs e)
        {
     RunInsert(labelINSfile.Text, textBoxINS.Text);

        }

        private void RunInsert(string label, string text)
        {
            CloseConn();
            string tableName;
            string query = "";

            if (label== "No file selected." || text == "")
            {
                MessageBox.Show("Please select CSV file and input table name first");
            }
            else
            {
                tableName = text;
                try
                {
                    using (var reader = new StreamReader(label))
                    {
                        List<string> listA = new List<string>();
                        List<string> listB = new List<string>();
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(';');

                            query = "INSERT INTO " + tableName + " VALUES(" + values[0] + ");";

                            using (SqlCommand cmd2 = new SqlCommand(query, conn))
                            {
                                conn.Open();
                                cmd2.ExecuteNonQuery();
                                conn.Close();
                            }
                        }
                        UpdateGrid("select * from " + tableName + @";", dgvINSERT);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + Environment.NewLine + ex.GetType().FullName);
                }

         
            }
        }
        private void buttonOpenINS_Click(object sender, EventArgs e)
        {
            CloseConn();
            using (var selectFileDialog = new OpenFileDialog())
            {
                selectFileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";

                selectFileDialog.Title = "Select CSV file";
                if (selectFileDialog.ShowDialog() == DialogResult.OK)
                {
                    labelINSfile.Text = selectFileDialog.FileName;
                }
            }
        }

        private void mcbxTB_SelectedIndexChanged(object sender, EventArgs e)
        {
            CloseConn();
            mcbxColUP.Items.Clear();
            using (SqlCommand cmd = new SqlCommand("SELECT NAME FROM sys.columns WHERE object_id = OBJECT_ID('" + mcbxTB.Text + "')", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mcbxColUP.Items.Add(reader.GetValue(0).ToString().ToUpper());
                    }
                }
                conn.Close();
            }
            UpdateGrid("select * from " + mcbxTB.Text, dgvUPDATE);
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            CloseConn();
            string tableName;
            string query = "";

            if (mcbxTBDEL.Text == "" || mcbxCOLDEL.Text == "" || mcbxTBDEL.Text == "")
            {
                MessageBox.Show("Complete all fields.");
            }
            else
            {
                tableName = mcbxTBDEL.Text;
                try
                {
                    query = "DELETE FROM " + tableName + " WHERE " + mcbxCOLDEL.Text + "=@pid";

                    using (SqlCommand cmd2 = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("pid", mcbxVALDEL.Text);
                        cmd2.ExecuteNonQuery();

                        conn.Close();
                        UpdateGrid("select * from " + tableName + @";", dgvDEL);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Invalid Value for column or delete constraint first" + Environment.NewLine + ex.GetType().FullName);
                }
            }
        }

        private void mcbxTBDEL_SelectedIndexChanged(object sender, EventArgs e)
        {
            mcbxCOLDEL.Items.Clear();
            CloseConn();
            using (SqlCommand cmd = new SqlCommand("SELECT NAME FROM sys.columns WHERE object_id = OBJECT_ID('" + mcbxTBDEL.Text + "')", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mcbxCOLDEL.Items.Add(reader.GetValue(0).ToString().ToUpper());
                    }
                }
                conn.Close();
            }
            UpdateGrid("select * from " + mcbxTBDEL.Text, dgvDEL);
        }
        public void CloseConn()
        {
            if (conn == null || conn.State == ConnectionState.Open)
                conn.Close();
        }
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUp_Click_1(object sender, EventArgs e)
        {
            CloseConn();
            string tableName;
            string query = "";

            if (mcbxTB.Text == "" || mcbxColUP.Text == "" || textBoxVALUP.Text == "" || textBoxSETUP.Text == "")
            {
                MessageBox.Show("Please complete all fields.");
            }
            else
            {
                tableName = mcbxTB.Text;
                try
                {
                    query = "UPDATE " + tableName + " SET " + textBoxSETUP.Text + " WHERE " + mcbxColUP.Text + "=" + @"'" + textBoxVALUP.Text + @"'";

                    using (SqlCommand cmd2 = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("pid", mcbxVALDEL.Text);
                        cmd2.ExecuteNonQuery();

                        conn.Close();
                        UpdateGrid("select * from " + tableName + @";", dgvUPDATE);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Invalid Value for column" + Environment.NewLine + ex.GetType().FullName);
                }
            }
        }

        private void metroComboBoxUC_UN_SelectedIndexChanged(object sender, EventArgs e)
        {
            string pid = "";
            CloseConn();
            using (SqlCommand cmd = new SqlCommand("select ParticipantId from UserAccounts WHERE LongName=@pid", conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("pid", metroComboBoxUC_UN.Text);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pid = reader.GetValue(0).ToString().ToUpper();
                    }
                }
                conn.Close();
                ShowDetails(metroComboBoxUC_UN, "UserAccounts", "ID");
            }

            try { 
            using (SqlCommand cmd = new SqlCommand("select ParticipantName from merchants WHERE ID=@pid", conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("pid", pid);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        labelPID.Text = "Merchant: " + reader.GetValue(0).ToString().ToUpper();
                    }
                }
                conn.Close();
            }
            }
            catch { }
        }

        private void buttonUPBS_Click(object sender, EventArgs e)
        {
            CloseConn();
            string act = "";
            string act1 = "";

            if (TIDBS.Text != "" && VIDBS.Text != "" && SIBS.Text != "" && LNBS.Text != "")
            {

                using (SqlCommand cmd1 = new SqlCommand("select COUNT(*) FROM BusJeepneySettings WHERE VehicleID = @pid", conn))
                {
                    conn.Open();
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("pid", VIDBS.Text);
                    object obj1;
                    obj1 = cmd1.ExecuteScalar();
                    if (Convert.ToInt32(obj1) == 0)
                    {
                        using (SqlCommand cmd2 = new SqlCommand("select COUNT(*) FROM BusJeepneySettings WHERE LongName = @pid OR ShortIdentifier= @pids", conn))
                        {

                            cmd2.CommandType = CommandType.Text;
                            cmd2.Parameters.AddWithValue("pid", LNBS.Text);
                            cmd2.Parameters.AddWithValue("pids", SIBS.Text);
                            object obj2;
                            obj2 = cmd1.ExecuteScalar();
                            if (Convert.ToInt32(obj2) == 0)
                            {

                                if (conn == null || conn.State == ConnectionState.Open)
                                    conn.Close();

                                using (SqlCommand cmd = new SqlCommand("SELECT ID, ParticipantID FROM Terminals WHERE TerminalID=@pid", conn))
                                {
                                    try
                                    {
                                        conn.Open();
                                        cmd.Parameters.AddWithValue("pid", TIDBS.Text);
                                        using (var reader2 = cmd.ExecuteReader())
                                        {
                                            reader2.Read();
                                            act1 = (reader2[0].ToString());
                                            act = (reader2[1].ToString());
                                        }
                                    }
                                    catch { }
                                }
                                //START CREATE
                                using (SqlCommand cmd3 = new SqlCommand(@"INSERT INTO BusJeepneySettings VALUES (NEWID(), @ftid, @fa, @bfd,@bfd1,@bfd2, @ef, @et, GETDATE() , 1);", conn))
                                {

                                    cmd3.Parameters.AddWithValue("ftid", act);
                                    cmd3.Parameters.AddWithValue("fa", act1);
                                    cmd3.Parameters.AddWithValue("bfd", VIDBS.Text);
                                    cmd3.Parameters.AddWithValue("bfd1", SIBS.Text);
                                    cmd3.Parameters.AddWithValue("bfd2", LNBS.Text);
                                    cmd3.Parameters.AddWithValue("ef", DTBSFROM.Value.Date.ToString());
                                    cmd3.Parameters.AddWithValue("et", DTBSTO.Value.Date.ToString());
                                    cmd3.ExecuteNonQuery();

                                    conn.Close();
                                }

                                SIBS.Text = "";
                                TIDBS.SelectedIndex = -1;
                                VIDBS.Text = "";
                                LNBS.Text = "";
                                DTBSFROM.Value = DateTime.Now;
                                DTBSTO.Value = DateTime.Now;
                                UpdateALLGRIDS();
                                UpdateAllCombos();
                                MessageBox.Show("BusJeepneySettings created!");
                            }
                            else
                            {
                                MessageBox.Show("Name already taken");
                            }
                        }
                    }
                    else { MessageBox.Show("Vehicle ID already taken"); }
                }

            }
            else { MessageBox.Show("Please fill out all fields!"); }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            CloseConn();
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(textBoxQuery.SelectedText, conn);
                SqlDataReader sdr = cmd.ExecuteReader();
                sdr.Close();
                conn.Close();
                labelAlert.Text = "Execution successful";
                labelAlert.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                labelAlert.Text = "Sql Error: " + ex.GetType().Name;
                labelAlert.ForeColor = Color.Red;
            }
            try { UpdateGrid(textBoxQuery.SelectedText, dgvQUERY); } catch { conn.Close(); }
        }

        private void buttonQuery_Click(object sender, EventArgs e)
        {
            CloseConn();
            using (var selectFileDialog = new OpenFileDialog())
            {
                selectFileDialog.Filter = "sql files (*.sql)|*.sql|All files (*.*)|*.*";

                selectFileDialog.Title = "Select SQL file";
                if (selectFileDialog.ShowDialog() == DialogResult.OK)
                {
                    labelQuery.Text = selectFileDialog.FileName;
                    textBoxQuery.Text = File.ReadAllText(selectFileDialog.FileName);
                }
            }
        }

        private void buttonDELBS_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";
            if (dgvBS.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvBS.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvBS.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[4].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM BusJeepneySettings WHERE ID=@param", conn))
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
                UpdateAllCombos();
            }
        }


        private void metroComboBoxPP_PID_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetails(metroComboBoxPP_PID, "BeejeesProfiles", "ProfileID");
        }

        private void metroComboBoxPP_SPI_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetails(metroComboBoxPP_SPI, "SystemParameters", "ID");
            if (metroComboBoxPP_SPI.Text != "") { 
            using (SqlCommand cmd2 = new SqlCommand("SELECT Value FROM SystemParameters WHERE ID=@param;", conn))
            {
                CloseConn();
                conn.Open();
                cmd2.Parameters.AddWithValue("param", metroComboBoxPP_SPI.Text);
                using (var reader2 = cmd2.ExecuteReader())
                {
                    reader2.Read();
                    labelSSVAL.Text = "System Parameter Value: "+ (reader2[0].ToString());
                }
                conn.Close();
            }
            }
            else
            {
                labelSSVAL.Text = "";
            }

        }

        private void metroComboBoxUA_MN_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetails(metroComboBoxUA_MN, "Merchants", "ParticipantName");
        }

        private void TIDBS_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetails(TIDBS, "Terminals", "TerminalID");
        }
        private void TIDBS2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetails(TIDBS2, "Terminals", "TerminalID");
        }


        private void textBoxPNBP2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBoxPNBP.Text == "" && !togDEL.Checked)
                {
                    CloseConn();
                    using (SqlCommand cmd2 = new SqlCommand("SELECT MIN(ProfileID) + 1 FROM BeejeesProfiles WHERE ProfileID + 1 NOT IN(SELECT ProfileID FROM BeejeesProfiles)", conn))
                    {
                        conn.Open();
                        using (var reader2 = cmd2.ExecuteReader())
                        {
                            reader2.Read();
                            textBoxPNBP.Text = (reader2[0].ToString());
                        }
                        CloseConn();
                    }
                }
            }
            catch
            {
                CloseConn();
            }
        }

        private void buttonSFFADD_Click(object sender, EventArgs e)
        {
            CloseConn();
            string act;

            if (textBoxSFF_FTID.Text != "" && textBoxSFF_FA.Text != "")
            {

                using (SqlCommand cmd1 = new SqlCommand("select COUNT(*) FROM SingleFixedFareTables WHERE FareTableID = @pid", conn))
                {
                    conn.Open();
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("pid", textBoxSFF_FTID.Text);
                    object obj1;
                    obj1 = cmd1.ExecuteScalar();
                    if (Convert.ToInt32(obj1) == 0)
                    {
                        //START CREATE
                        using (SqlCommand cmd2 = new SqlCommand("INSERT INTO SingleFixedFareTables VALUES (NEWID(), @rid, @rsn, @rln, @fst, @act);", conn))
                        {

                            cmd2.Parameters.AddWithValue("rid", textBoxSFF_FTID.Text);
                            cmd2.Parameters.AddWithValue("rsn", textBoxSFF_FA.Text); //GET FROM PTO
                            cmd2.Parameters.AddWithValue("rln", metroDateTimeSFF_FROM.Value.Date);
                            cmd2.Parameters.AddWithValue("fst", metroDateTimeSFF_TO.Value.Date);
                            if (checkBoxSFF_A.Checked) { act = "1"; } else { act = "0"; }
                            cmd2.Parameters.AddWithValue("act", act);
                            cmd2.ExecuteNonQuery();


                            conn.Close();
                        }
                        MessageBox.Show("Fare created!");
                        textBoxSFF_FTID.Text = "";
                        textBoxSFF_FA.Text = "";
                        metroDateTimeSFF_FROM.Value = DateTime.Now;
                        metroDateTimeSFF_TO.Value = DateTime.Now;
                        checkBoxSFF_A.Checked = false;
                        UpdateALLGRIDS();
                        UpdateAllCombos();
                    }
                    else { MessageBox.Show("FareTableID already taken"); }
                }
            }
            else
            { MessageBox.Show("Please fill out all fields!"); }
        }

        private void buttonSFFDEL_Click(object sender, EventArgs e)
        {
            string a = "";
            string b = "";
            if (dgvSFF.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvSFF.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvSFF.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM SingleFixedFareTables WHERE ID=@param", conn))
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
                UpdateAllCombos();
            }
        }

        private void buttonDBCPADD_Click(object sender, EventArgs e)
        {
            CloseConn();
            string get = "";
            if (textBoxDBCP_CPN.Text != "" && textBoxDBCP_CPID.Text != "" && metroComboBoxDBCP_DFTID.Text != "")
            {

                using (SqlCommand cmd1 = new SqlCommand("select COUNT(*) FROM DistanceBasedCardProfiles WHERE CardProfileID = @pid OR DistanceBasedCardProfiles.CardProfileName = @cpn", conn))
                {
                    conn.Open();
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("pid", textBoxDBCP_CPID.Text);
                    cmd1.Parameters.AddWithValue("cpn", textBoxDBCP_CPN.Text);
                    object obj1;
                    obj1 = cmd1.ExecuteScalar();
                    if (Convert.ToInt32(obj1) == 0 || Convert.ToInt32(obj1) != 0)
                    {


                        using (SqlCommand cmd2 = new SqlCommand("SELECT ID from DiscountFareTables WHERE FareTableID=@param", conn))
                        {
                   
                            cmd2.Parameters.AddWithValue("param", metroComboBoxDBCP_DFTID.Text);
                            using (var reader2 = cmd2.ExecuteReader())
                            {
                                reader2.Read();
                                get = (reader2[0].ToString());
                            }
                            conn.Close();
                        }


                        using (SqlCommand cmd = new SqlCommand("select COUNT(*) FROM DistanceBasedCardProfiles WHERE DiscountedFare=@pid", conn))
                        {
                            conn.Open();
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("pid", get);
                            object obj;
                            obj = cmd.ExecuteScalar();
                            if (Convert.ToInt32(obj) == 0 || Convert.ToInt32(obj) != 0)
                            {
                                //START CREATE
                                using (SqlCommand cmd2 = new SqlCommand("INSERT INTO DistanceBasedCardProfiles VALUES (NEWID(), NULL, @rsn, @rln, NULL, NULL, @DF,NULL);", conn))
                                {

                                    cmd2.Parameters.AddWithValue("rsn", textBoxDBCP_CPID.Text); //GET FROM PTO
                                    cmd2.Parameters.AddWithValue("rln", textBoxDBCP_CPN.Text);
                                    cmd2.Parameters.AddWithValue("DF", holder3);
                                    cmd2.ExecuteNonQuery();
                                    conn.Close();
                                }
                                MessageBox.Show("Card Profile created!");
                                textBoxDBCP_CPN.Text = "";
                                textBoxDBCP_CPID.Text = "";
                                metroComboBoxDBCP_DFTID.Text = "";
                                holder3 = "";
                                UpdateALLGRIDS();
                                panel23.Refresh();
                                UpdateAllCombos();
                            }
                            else { MessageBox.Show("Discount Fare Table ID already taken."); }
                        }
                     
                    }
                    else { MessageBox.Show("Card Profile already taken"); }
                }
            }
            else
            { MessageBox.Show("Please fill out all fields!"); }
        }

        private void metroComboBoxDBCP_DFTID_SelectedIndexChanged(object sender, EventArgs e)
        {
            CloseConn();
            holder3 = "";
            using (SqlCommand cmd = new SqlCommand("select ID from DiscountFareTables where FareTableID=@fid", conn))
            {
                cmd.Parameters.AddWithValue("fid", metroComboBoxDBCP_DFTID.Text);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        holder3 = reader.GetValue(0).ToString().ToUpper();
                    }
                }
                CloseConn();
                ShowDetails(metroComboBoxDBCP_DFTID, "DiscountFareTables", "FareTableID");
            }
        }

        private void buttonDBCPDEL_Click(object sender, EventArgs e)
        {
            string a = "";
            string b = "";
            if (dgvDBCP.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvDBCP.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvDBCP.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[2].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM DistanceBasedCardProfiles WHERE ID=@param", conn))
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
                UpdateAllCombos();

            }
        }

        private void dataGridViewBP_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            holder2 = "";
            holder3 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                buttonUpdateBP.Enabled = false;
                if (dataGridViewBP.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    holder2 = dataGridViewBP.SelectedRows[0].Cells[1].Value.ToString();
                    holder3 = dataGridViewBP.SelectedRows[0].Cells[2].Value.ToString();
                    holder1 = dataGridViewBP.SelectedRows[0].Cells[0].Value.ToString();

                    buttonSaveBP.Visible = true;
                    label13.Text = "New Profile ID:";
                    label12.Text = "New Profile Name:";
                    textBoxPNBP.Text = holder2;
                    textBoxPNBP2.Text = holder3;
                    IsMenuDisabled(true, buttonBPdel);
                }
            }
        }

        private void buttonSaveBP_Click(object sender, EventArgs e)
        {
            holder2 = "";
            holder3 = "";
            {
                try
                {
                    CloseConn();

                    if (textBoxPNBP.Text == "" || textBoxPNBP2.Text == "")
                    {
                        MessageBox.Show("Please fill out all fields.");
                    }
                    else
                    {
                        using (SqlCommand cmd2 = new SqlCommand("UPDATE BeejeesProfiles SET ProfileID=@pid, ProfileName=@pto WHERE ID=@fname", conn))
                        {
                            conn.Open();
                            cmd2.Parameters.AddWithValue("fname", holder1);
                            cmd2.Parameters.AddWithValue("pto", textBoxPNBP2.Text);
                            cmd2.Parameters.AddWithValue("pid", textBoxPNBP.Text);
                            cmd2.ExecuteNonQuery();
                            conn.Close();
                        }
                        MessageBox.Show("Update Successful!");
                        UpdateAllCombos();
                        textBoxNewTID.Text = "";
                        buttonSaveBP.Visible = false;
                        buttonUpdateBP.Enabled = true;
                        UpdateAllCombos();
                        UpdateALLGRIDS();
                        textBoxPNBP.Text = "";
                        textBoxPNBP2.Text = "";
                        holder2 = "";
                        holder1 = "";
                        IsMenuDisabled(false, buttonBPdel);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    CloseConn();
                }
            }
        }
        private void buttonSAVEDBR_Click(object sender, EventArgs e)
        {
            holder2 = "";
            string fs = "";
            string act = "";
            {
                try
                {
                    CloseConn();
                    if (textBoxNRID_DBR.Text != "" && textBoxNRSN_DBR.Text != "" && textBoxRLN_DBR.Text != "")
                    {
                        if (radioButtonNddDBR.Checked == true || radioButtonNdcDBR.Checked == true)
                        {
                            object obj;
                            using (SqlCommand cmd = new SqlCommand("select COUNT(*) FROM DistanceBasedRoutes WHERE RouteID = @pid", conn))
                            {
                                conn.Open();
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("pid", textBoxNRID_DBR.Text);

                                obj = cmd.ExecuteScalar();
                            }
                            CloseConn();
                            if (Convert.ToInt32(obj) == 0 || dgvDBR.SelectedRows[0].Cells[1].Value.ToString() == textBoxNRID_DBR.Text)
                            {
                                using (SqlCommand cmd2 = new SqlCommand("UPDATE DistanceBasedRoutes SET RouteID=@rid, RouteShortName=@rsn,RouteLongName=@rln,FareStrategy=@fst,Active=@act Where ID=@hold", conn))
                                {
                                    conn.Open();
                                    cmd2.Parameters.AddWithValue("rid", textBoxNRID_DBR.Text);
                                    cmd2.Parameters.AddWithValue("rsn", textBoxNRSN_DBR.Text); //GET FROM PTO
                                    cmd2.Parameters.AddWithValue("rln", textBoxRLN_DBR.Text);
                                    if (radioButtonNddDBR.Checked) { fs = "debit_debit"; } else { fs = "debit_credit"; }
                                    cmd2.Parameters.AddWithValue("fst", fs);
                                    if (checkBoxNDBR.Checked) { act = "1"; } else { act = "0"; }
                                    cmd2.Parameters.AddWithValue("act", act);
                                    cmd2.Parameters.AddWithValue("hold", holder1);
                                    cmd2.ExecuteNonQuery();


                                    conn.Close();
                                }
                                MessageBox.Show("Route updated!");
                                textBoxNRID_DBR.Text = "";
                                textBoxNRSN_DBR.Text = "";
                                textBoxRLN_DBR.Text = "";
                                radioButtonNddDBR.Checked = false;
                                radioButtonNdcDBR.Checked = false;
                                checkBoxNDBR.Checked = false;
                                UpdateALLGRIDS();
                                UpdateAllCombos();
                                panelDBR.Visible = false;
                                buttonSAVEDBR.Visible = false;
                                buttonDBR_Create.Enabled = true;
                                IsMenuDisabled(false, buttonDELDBR);
                            }
                            else { MessageBox.Show("RouteID already taken"); }
                        }
                        else { MessageBox.Show("Please choose Fare Strategy"); }
                    }
                    else
                    { MessageBox.Show("Please fill out all fields!"); }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    CloseConn();
                }
            }
        }

        private void dgvDFT_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            holder2 = "";
            holder3 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                buttonDFT_Create.Enabled = false;
                panelDFT.Visible = true;
                buttonSaveDFT.Visible = true;
                if (dgvDFT.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    try
                    {
                        numericUpDownNRA_DFT.Value = Convert.ToDecimal(dgvDFT.SelectedRows[0].Cells[8].Value.ToString());
                    }
                    catch { numericUpDownNRA_DFT.Value = numericUpDownNRA_DFT.Minimum; }
                    holder3 = dgvDFT.SelectedRows[0].Cells[7].Value.ToString();
                    metroDateTimeNTO_DFT.Value = Convert.ToDateTime(dgvDFT.SelectedRows[0].Cells[5].Value.ToString());
                    metroDateTimeNED_DFT.Value = Convert.ToDateTime(dgvDFT.SelectedRows[0].Cells[4].Value.ToString());
                    holder2 = dgvDFT.SelectedRows[0].Cells[3].Value.ToString();
                    textBoxND_DFT.Text = dgvDFT.SelectedRows[0].Cells[2].Value.ToString();
                    textBoxNFTID_DFT.Text = dgvDFT.SelectedRows[0].Cells[1].Value.ToString();
                    holder1 = dgvDFT.SelectedRows[0].Cells[0].Value.ToString();
                    if (holder3 == "True") checkBoxA_DFT.Checked = true;
                    else checkBoxA_DFT.Checked = false;
                    if (holder2 == "EXACT") radioButtonExactNDFT.Checked = true;
                    else if (holder2 == "UP") radioButtonNUP_DFT.Checked = true;
                    else radioButtonNDOWN_DFT.Checked = true;
                    IsMenuDisabled(true, buttonDELDFT);
                }
            }
        }

        private void buttonSaveDFT_Click(object sender, EventArgs e)
        {
            holder2 = "";
            holder3 = "";
            string fs = "";
            string act = "";
            {
                try
                {
                    CloseConn();
                    if (textBoxNFTID_DFT.Text != "" && textBoxND_DFT.Text != "")
                    {
                        if (radioButtonExactNDFT.Checked == true || radioButtonNUP_DFT.Checked == true || radioButtonNDOWN_DFT.Checked == true)
                        {
                            object obj;
                            using (SqlCommand cmd = new SqlCommand("select COUNT(*) FROM DiscountFareTables WHERE FareTableID = @pid", conn))
                            {
                                conn.Open();
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("pid", textBoxNFTID_DFT.Text);

                                obj = cmd.ExecuteScalar();
                            }
                            CloseConn();
                            if (Convert.ToInt32(obj) == 0 || dgvDFT.SelectedRows[0].Cells[1].Value.ToString() == textBoxNFTID_DFT.Text)
                            {
                                using (SqlCommand cmd2 = new SqlCommand("UPDATE DiscountFareTables SET FareTableID=@rid, Discount=@rsn,RoundingMethod=@rln,EffectiveFrom=@fst,EffectiveTo=@fst2,Active=@act,RoundingAccuracy=@ra Where ID=@hold", conn))
                                {
                                    conn.Open();
                                    cmd2.Parameters.AddWithValue("rid", textBoxNFTID_DFT.Text);
                                    cmd2.Parameters.AddWithValue("rsn", textBoxND_DFT.Text); //GET FROM PTO
                                    if (radioButtonExactNDFT.Checked) { fs = "EXACT"; } else if (radioButtonNUP_DFT.Checked) { fs = "UP"; } else { fs = "DOWN"; }
                                    cmd2.Parameters.AddWithValue("rln", fs);
                                    cmd2.Parameters.AddWithValue("fst", metroDateTimeNED_DFT.Value.Date);
                                    if (checkBoxA_DFT.Checked) { act = "1"; } else { act = "0"; }
                                    cmd2.Parameters.AddWithValue("fst2", metroDateTimeNTO_DFT.Value.Date);
                                    cmd2.Parameters.AddWithValue("act", act);
                                    cmd2.Parameters.AddWithValue("ra", numericUpDownNRA_DFT.Value.ToString());
                                    cmd2.Parameters.AddWithValue("hold", holder1);
                                    cmd2.ExecuteNonQuery();


                                    conn.Close();
                                }
                                MessageBox.Show("Fare Table updated!");
                                metroDateTimeNED_DFT.Value = DateTime.Now;
                                metroDateTimeNTO_DFT.Value = DateTime.Now;
                                numericUpDownNRA_DFT.Value = numericUpDownNRA_DFT.Minimum;
                                textBoxND_DFT.Text = "";
                                textBoxNFTID_DFT.Text = "";
                                checkBoxA_DFT.Checked = false;
                                radioButtonNDOWN_DFT.Checked = false;
                                radioButtonNUP_DFT.Checked = false;
                                radioButtonExactNDFT.Checked = false;
                                UpdateALLGRIDS();
                                UpdateAllCombos();
                                panelDFT.Visible = false;
                                buttonSaveDFT.Visible = false;
                                buttonDFT_Create.Enabled = true;
                                IsMenuDisabled(false, buttonDELDFT);
                            }
                            else { MessageBox.Show("FareTable ID already taken"); }
                        }
                        else { MessageBox.Show("Please choose Rounding Method"); }
                    }
                    else
                    { MessageBox.Show("Please fill out all fields!"); }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    CloseConn();
                }
            }
        }

        private void dgvDBFT_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            holder2 = "";
            holder3 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                buttonDBFT_Create.Enabled = false;
                panelDBFT.Visible = true;
                buttonSAVE_DBFT.Visible = true;
                if (dgvDBFT.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    holder3 = dgvDBFT.SelectedRows[0].Cells[7].Value.ToString();
                    if (holder3 == "True") checkBoxA_DBFT.Checked = true;
                    else checkBoxA_DBFT.Checked = false;
                    holder3 = dgvDBFT.SelectedRows[0].Cells[8].Value.ToString();
                    if (holder3 == "True") checkBoxAcc_DBFT2.Checked = true;
                    else checkBoxAcc_DBFT2.Checked = false;
                    holder3 = dgvDBFT.SelectedRows[0].Cells[9].Value.ToString();
                    if (holder3 == "True") checkBoxPro_DBFT2.Checked = true;
                    else checkBoxPro_DBFT2.Checked = false;

                    metroDateTimeNTO_DBFT.Value = Convert.ToDateTime(dgvDBFT.SelectedRows[0].Cells[5].Value.ToString());
                    metroDateTimeNED_DBFT.Value = Convert.ToDateTime(dgvDBFT.SelectedRows[0].Cells[4].Value.ToString());
                    textBoxNBFD_DBFT.Text = dgvDBFT.SelectedRows[0].Cells[3].Value.ToString();
                    textBoxNFA_DBFT.Text = dgvDBFT.SelectedRows[0].Cells[2].Value.ToString();
                    textBoxNFTID_DBFT.Text = dgvDBFT.SelectedRows[0].Cells[1].Value.ToString();
                    holder1 = dgvDBFT.SelectedRows[0].Cells[0].Value.ToString();

                    IsMenuDisabled(true, buttonDELDBFT);
                }
            }
        }

        private void buttonSAVE_DBFT_Click(object sender, EventArgs e)
        {
            holder2 = "";
            holder3 = "";
            string act, act2, act3;
            {
                try
                {
                    CloseConn();
                    if (textBoxNFTID_DBFT.Text != "" && textBoxNFA_DBFT.Text != "" || textBoxNBFD_DBFT.Text != "")
                    {

                        object obj;
                        using (SqlCommand cmd = new SqlCommand("select COUNT(*) FROM DistanceBasedFareTables WHERE FareTableID = @pid", conn))
                        {
                            conn.Open();
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("pid", textBoxNFTID_DFT.Text);

                            obj = cmd.ExecuteScalar();
                        }
                        CloseConn();
                        if (Convert.ToInt32(obj) == 0 || dgvDFT.SelectedRows[0].Cells[1].Value.ToString() == textBoxNFTID_DBFT.Text)
                        {
                            using (SqlCommand cmd2 = new SqlCommand("UPDATE DistanceBasedFareTables SET FareTableID=@ftid, FareAmount=@fa, BaseFareDistance=@bfd, EffectiveFrom=@ef,EffectiveTo=@et, Active=@act, Accumulative=@act2, Prorated=@act3 Where ID=@hold", conn))
                            {
                                conn.Open();
                                cmd2.Parameters.AddWithValue("ftid", textBoxNFTID_DBFT.Text);
                                cmd2.Parameters.AddWithValue("fa", textBoxNFA_DBFT.Text);
                                cmd2.Parameters.AddWithValue("bfd", textBoxNBFD_DBFT.Text);
                                cmd2.Parameters.AddWithValue("ef", metroDateTimeNED_DBFT.Value.Date);
                                cmd2.Parameters.AddWithValue("et", metroDateTimeNTO_DBFT.Value.Date);
                                if (checkBoxA_DBFT.Checked == true) { act = "1"; } else act = "0";
                                cmd2.Parameters.AddWithValue("act", act);
                                if (checkBoxAcc_DBFT2.Checked) { act2 = "1"; } else { act2 = ""; }
                                cmd2.Parameters.AddWithValue("act2", act2);
                                if (checkBoxPro_DBFT2.Checked) { act3 = "1"; } else { act3 = ""; }
                                cmd2.Parameters.AddWithValue("act3", act3);
                                cmd2.Parameters.AddWithValue("hold", holder1);
                                cmd2.ExecuteNonQuery();
                                conn.Close();
                            }
                            MessageBox.Show("Fare Table updated!");
                            metroDateTimeNED_DBFT.Value = DateTime.Now;
                            metroDateTimeNTO_DBFT.Value = DateTime.Now;

                            textBoxNFTID_DBFT.Text = "";
                            textBoxNFA_DBFT.Text = "";
                            textBoxNBFD_DBFT.Text = "";
                            checkBoxA_DBFT.Checked = false;
                            checkBoxAcc_DBFT2.Checked = false;
                            checkBoxPro_DBFT2.Checked = false;
                            UpdateALLGRIDS();
                            UpdateAllCombos();
                            panelDBFT.Visible = false;
                            buttonSAVE_DBFT.Visible = false;
                            buttonDBFT_Create.Enabled = true;
                            IsMenuDisabled(false, buttonDELDBFT);
                        }
                        else { MessageBox.Show("FareTable ID already taken"); }

                    }
                    else
                    { MessageBox.Show("Please fill out all fields!"); }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    CloseConn();
                }
            }
        }

        private void dgvDBIF_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            holder2 = "";
            holder3 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                buttonCreate_DBIF.Enabled = false;
                panelDBIF.Visible = true;
                buttonSAVE_DBIF.Visible = true;
                if (dgvDBIF.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {

                    holder1 = dgvDBIF.SelectedRows[0].Cells[0].Value.ToString();
                    cbxNewFTIDDBIF.Text = dgvDBIF.SelectedRows[0].Cells[1].Value.ToString();
                    holder2 = cbxNewFTIDDBIF.Text;
                    textBoxNIF_DBIF.Text = dgvDBIF.SelectedRows[0].Cells[2].Value.ToString();
                    textBoxNIFD_DBIF.Text = dgvDBIF.SelectedRows[0].Cells[3].Value.ToString();
                    textBoxNSDDBIF.Text = dgvDBIF.SelectedRows[0].Cells[4].Value.ToString();
                    IsMenuDisabled(true, buttonDELDBIF);
                }
            }
        }

        private void buttonSAVE_DBIF_Click(object sender, EventArgs e)
        {
            CloseConn();
            string ftid = "";
            if (cbxNewFTIDDBIF.Text == "" || textBoxNIF_DBIF.Text == "" || textBoxNIFD_DBIF.Text == "" || textBoxNSDDBIF.Text == "")
            {
                MessageBox.Show("Please fill out all fields.");
            }
            else
            {

                using (SqlCommand cmd2 = new SqlCommand("select ID from DistanceBasedFareTables WHERE FareTableId = @pid", conn))
                {

                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", cbxNewFTIDDBIF.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        reader2.Read();
                        ftid = (reader2[0].ToString());
                    }
                    conn.Close();

                }
                object obj2;
                using (SqlCommand cmd3 = new SqlCommand("select COUNT(*) FROM DistanceBasedIncrementFares WHERE FareTable_Id=@pn", conn)) //check if PID is existing
                {
                    conn.Open();
                    cmd3.CommandType = CommandType.Text;
                    cmd3.Parameters.AddWithValue("pn", ftid);
                    obj2 = cmd3.ExecuteScalar();
                    conn.Close();
                }
                if (!checkBoxOneDBIF.Checked)
                {
                    obj2 = 0;
                }
                if (Convert.ToInt32(obj2) == 0 || holder2 == cbxNewFTIDDBIF.Text)
                {

                    using (SqlCommand cmd2 = new SqlCommand("UPDATE DistanceBasedIncrementFares set FareTable_ID=@fname, IncrementalFareAmount=@pto, IncrementalFareDistance=@pid, StartDistance=@sid WHERE ID=@hold;", conn))
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("fname", ftid);
                        cmd2.Parameters.AddWithValue("pto", textBoxNIF_DBIF.Text);
                        cmd2.Parameters.AddWithValue("pid", textBoxNIFD_DBIF.Text);
                        cmd2.Parameters.AddWithValue("sid", textBoxNSDDBIF.Text);
                        cmd2.Parameters.AddWithValue("hold", holder1);
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                    }
                    MessageBox.Show("Update Successful!");
                    cbxNewFTIDDBIF.Text = "";
                    textBoxNIF_DBIF.Text = "";
                    textBoxNIFD_DBIF.Text = "";
                    textBoxNSDDBIF.Text = "";
                    UpdateAllCombos();
                    UpdateALLGRIDS();
                    panelDBIF.Visible = false;
                    buttonSAVE_DBIF.Visible = false;
                    buttonCreate_DBIF.Enabled = true;
                    IsMenuDisabled(false, buttonDELDBIF);
                }
                else
                {
                    MessageBox.Show("FareTable ID already associated to a an Increment Fare");
                }

            }
        }


        private void dgvSP_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            holder2 = "";
            holder3 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                buttonSP_Gen.Enabled = false;
                panelSP.Visible = true;
                buttonSaveSP.Visible = true;
                if (dgvSP.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {

                    holder1 = dgvSP.SelectedRows[0].Cells[0].Value.ToString();
                    textBoxSP_V2.Text = dgvSP.SelectedRows[0].Cells[3].Value.ToString();
                    metroComboBoxSP_P2.Text = dgvSP.SelectedRows[0].Cells[1].Value.ToString();
                    IsMenuDisabled(true, buttonDELSP);
                }
            }
        }
        private void InsSysParams2()
        {
            CloseConn();
            using (SqlCommand cmd2 = new SqlCommand("UPDATE SystemParameters SET Parameter=@param, Type=@type, Value=@val WHERE ID=@hold;", conn))
            {
                conn.Open();
                cmd2.Parameters.AddWithValue("param", metroComboBoxSP_P2.Text);
                cmd2.Parameters.AddWithValue("type", metroComboBoxSP_T2.Text);
                cmd2.Parameters.AddWithValue("val", textBoxSP_V2.Text);
                cmd2.Parameters.AddWithValue("hold", holder1);
                cmd2.ExecuteNonQuery();
                conn.Close();
            }
            textBoxSP_V2.Text = "";
            panelSP.Visible = false;
            buttonSaveSP.Visible = false;
            buttonSP_Gen.Enabled = true;
            IsMenuDisabled(false, buttonDELSP);
            UpdateALLGRIDS();
            UpdateAllCombos();
            MessageBox.Show("System Parameters Based Saved!");
        }
        private void buttonSaveSP_Click(object sender, EventArgs e)
        {
            CloseConn();
            if (metroComboBoxSP_P2.Text == "" || metroComboBoxSP_T2.Text == "" || textBoxSP_V2.Text == "")
            {
                MessageBox.Show("Please fill out all fields");
            }
            else
            {

                if (metroComboBoxSP_T2.Text == "string")
                {
                    InsSysParams2();
                }
                else
                {
                    int i;
                    if (!int.TryParse(textBoxSP_V2.Text, out i))
                    { MessageBox.Show("Value cannot be coverted to Int64"); }
                    else
                    {
                        InsSysParams2();
                    }
                }
            }
        }


        private void dgvBS_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            holder2 = "";
            holder3 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                buttonUPBS.Enabled = false;
                panelBJS.Visible = true;
                buttonSaveBJS.Visible = true;
                if (dgvBS.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {

                    holder1 = dgvBS.SelectedRows[0].Cells[0].Value.ToString();
                    TIDBS2.Text = dgvBS.SelectedRows[0].Cells[2].Value.ToString();
                    VIDBS2.Text = dgvBS.SelectedRows[0].Cells[3].Value.ToString();
                    SIBS2.Text = dgvBS.SelectedRows[0].Cells[4].Value.ToString();
                    LNBS2.Text = dgvBS.SelectedRows[0].Cells[5].Value.ToString();
                    DTBSFROM2.Value = Convert.ToDateTime(dgvBS.SelectedRows[0].Cells[6].Value.ToString());
                    DTBSTO2.Value = Convert.ToDateTime(dgvBS.SelectedRows[0].Cells[7].Value.ToString());
                    IsMenuDisabled(true, buttonDELBS);
                }
            }
        }

        private void buttonSaveBJS_Click(object sender, EventArgs e)
        {
            CloseConn();
            string act = "";
            string act1 = "";

            if (TIDBS2.Text != "" && VIDBS2.Text != "" && SIBS2.Text != "" && LNBS2.Text != "")
            {

                using (SqlCommand cmd1 = new SqlCommand("select COUNT(*) FROM BusJeepneySettings WHERE VehicleID = @pid", conn))
                {
                    conn.Open();
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("pid", VIDBS2.Text);
                    object obj1;
                    obj1 = cmd1.ExecuteScalar();
                    if (Convert.ToInt32(obj1) != 0 || Convert.ToInt32(obj1) == 0)
                    {
                        using (SqlCommand cmd2 = new SqlCommand("select COUNT(*) FROM BusJeepneySettings WHERE LongName = @pid OR ShortIdentifier= @pids", conn))
                        {
                            cmd2.CommandType = CommandType.Text;
                            cmd2.Parameters.AddWithValue("pid", LNBS2.Text);
                            cmd2.Parameters.AddWithValue("pids", SIBS2.Text);
                            object obj2;
                            obj2 = cmd1.ExecuteScalar();
                            if (Convert.ToInt32(obj2) != 0 || Convert.ToInt32(obj2) == 0)
                            {

                                if (conn == null || conn.State == ConnectionState.Open)
                                    conn.Close();

                                using (SqlCommand cmd = new SqlCommand("SELECT ID, ParticipantID FROM Terminals WHERE TerminalID=@pid", conn))
                                {
                                    try
                                    {
                                        conn.Open();
                                        cmd.Parameters.AddWithValue("pid", TIDBS2.Text);
                                        using (var reader2 = cmd.ExecuteReader())
                                        {
                                            reader2.Read();
                                            act1 = (reader2[0].ToString());
                                            act = (reader2[1].ToString());
                                        }

                                    }
                                    catch { }
                                }

                                //START CREATE
                                using (SqlCommand cmd3 = new SqlCommand(@"UPDATE BusJeepneySettings SET ParticipantID=@ftid, TerminalID=@fa, VehicleID=@bfd, ShortIdentifier=@bfd1, LongName=@bfd2, VehicleEffectiveDateFrom=@ef, VehicleEffectiveDateTo=@et WHERE ID=@hold;", conn))
                                {

                                    cmd3.Parameters.AddWithValue("ftid", act);
                                    cmd3.Parameters.AddWithValue("hold", holder1);
                                    cmd3.Parameters.AddWithValue("fa", act1);
                                    cmd3.Parameters.AddWithValue("bfd", VIDBS2.Text);
                                    cmd3.Parameters.AddWithValue("bfd1", SIBS2.Text);
                                    cmd3.Parameters.AddWithValue("bfd2", LNBS2.Text);
                                    cmd3.Parameters.AddWithValue("ef", DTBSFROM2.Value.Date.ToString());
                                    cmd3.Parameters.AddWithValue("et", DTBSTO2.Value.Date.ToString());
                                    cmd3.ExecuteNonQuery();

                                    conn.Close();
                                }

                                SIBS2.Text = "";
                                TIDBS2.SelectedIndex = -1;
                                VIDBS2.Text = "";
                                LNBS2.Text = "";
                                DTBSFROM2.Value = DateTime.Now;
                                DTBSTO2.Value = DateTime.Now;
                                UpdateALLGRIDS();
                                panelBJS.Visible = false;
                                buttonUPBS.Enabled = true;
                                buttonSaveBJS.Visible = false;
                                IsMenuDisabled(false, buttonDELBS);
                                UpdateAllCombos();
                                MessageBox.Show("BusJeepneySettings Updated!");
                            }
                            else
                            {
                                MessageBox.Show("Name already taken");
                            }
                        }
                    }
                    else { MessageBox.Show("Vehicle ID already taken"); }
                }

            }
            else { MessageBox.Show("Please fill out all fields!"); }
        }

        private void metroDateTimeSFF_FROM2_ValueChanged(object sender, EventArgs e)
        {
            SetMinDate(metroDateTimeSFF_FROM2, metroDateTimeSFF_TO2);
        }

        private void dgvSFF_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            holder2 = "";
            holder3 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                
                if (dgvSFF.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    holder3 = dgvSFF.SelectedRows[0].Cells[5].Value.ToString();
                    if (holder3 == "True") checkBoxSFF_A2.Checked = true;
                    else checkBoxSFF_A2.Checked = false;
                    holder1 = dgvSFF.SelectedRows[0].Cells[0].Value.ToString();
                    textBoxSFF_FTID2.Text = dgvSFF.SelectedRows[0].Cells[1].Value.ToString();
                    textBoxSFF_FA2.Text = dgvSFF.SelectedRows[0].Cells[2].Value.ToString();
                    metroDateTimeSFF_FROM2.Value = Convert.ToDateTime(dgvSFF.SelectedRows[0].Cells[3].Value.ToString());
                    metroDateTimeSFF_TO2.Value = Convert.ToDateTime(dgvSFF.SelectedRows[0].Cells[4].Value.ToString());
                    IsMenuDisabled(true, buttonSFFDEL);
                    buttonSFFADD.Enabled = false;
                    panelSFF.Visible = true;
                    buttonSaveSFF.Visible = true;
                }
            }
        }

        private void buttonSaveSFF_Click_1(object sender, EventArgs e)
        {
            CloseConn();
            string act;

            if (textBoxSFF_FTID2.Text != "" && textBoxSFF_FA2.Text != "")
            {

                using (SqlCommand cmd1 = new SqlCommand("select COUNT(*) FROM SingleFixedFareTables WHERE FareTableID = @pid", conn))
                {
                    conn.Open();
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("pid", textBoxSFF_FTID2.Text);
                    object obj1;
                    obj1 = cmd1.ExecuteScalar();
                    if (Convert.ToInt32(obj1) == 0 || Convert.ToInt32(obj1) != 0)
                    {
                        //START CREATE
                        using (SqlCommand cmd2 = new SqlCommand("UPDATE SingleFixedFareTables SET FareTableID=@rid, FareAmount=@rsn, EffectiveFrom=@rln, EffectiveTo=@fst, Active=@act WHERE ID=@hold;", conn))
                        {

                            cmd2.Parameters.AddWithValue("rid", textBoxSFF_FTID2.Text);
                            cmd2.Parameters.AddWithValue("rsn", textBoxSFF_FA2.Text); //GET FROM PTO
                            cmd2.Parameters.AddWithValue("rln", metroDateTimeSFF_FROM2.Value.Date);
                            cmd2.Parameters.AddWithValue("fst", metroDateTimeSFF_TO2.Value.Date);
                            cmd2.Parameters.AddWithValue("hold", holder1);
                            if (checkBoxSFF_A2.Checked) { act = "1"; } else { act = "0"; }
                            cmd2.Parameters.AddWithValue("act", act);
                            cmd2.ExecuteNonQuery();


                            conn.Close();
                        }
                        MessageBox.Show("Fare updated!");
                        textBoxSFF_FTID2.Text = "";
                        textBoxSFF_FA2.Text = "";
                        metroDateTimeSFF_FROM2.Value = DateTime.Now;
                        metroDateTimeSFF_TO2.Value = DateTime.Now;
                        checkBoxSFF_A2.Checked = false;
                        UpdateALLGRIDS();
                        UpdateAllCombos();
                        IsMenuDisabled(false, buttonSFFDEL);
                        buttonSFFADD.Enabled = true;
                        panelSFF.Visible = false;
                        buttonSaveSFF.Visible = false;
                    }
                    else { MessageBox.Show("FareTableID already taken"); }
                }
            }
            else
            { MessageBox.Show("Please fill out all fields!"); }
        }

        private void dgvDBCP_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            holder2 = "";
            holder3 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                if (dgvDBCP.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    holder1 = dgvDBCP.SelectedRows[0].Cells[0].Value.ToString();
                    textBoxDBCP_CPN2.Text = dgvDBCP.SelectedRows[0].Cells[2].Value.ToString();
                    textBoxDBCP_CPID2.Text = dgvDBCP.SelectedRows[0].Cells[1].Value.ToString();
                    metroComboBoxDBCP_DFTID2.Text = dgvDBCP.SelectedRows[0].Cells[3].Value.ToString();

                    IsMenuDisabled(true, buttonDBCPDEL);
                    buttonDBCPADD.Enabled = false;
                    panelDBCP.Visible = true;
                    buttonSaveDBCP.Visible = true;
                }
            }
        }

        private void buttonSaveDBCP_Click(object sender, EventArgs e)
        {
            CloseConn();

            if (textBoxDBCP_CPN2.Text != "" && textBoxDBCP_CPID2.Text != "" && metroComboBoxDBCP_DFTID2.Text != "")
            {

                using (SqlCommand cmd1 = new SqlCommand("select COUNT(*) FROM DistanceBasedCardProfiles WHERE CardProfileID = @pid OR DistanceBasedCardProfiles.CardProfileName = @cpn", conn))
                {
                    conn.Open();
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("pid", textBoxDBCP_CPID2.Text);
                    cmd1.Parameters.AddWithValue("cpn", textBoxDBCP_CPN2.Text);
                    object obj1;
                    obj1 = cmd1.ExecuteScalar();
                    if (Convert.ToInt32(obj1) == 0 || Convert.ToInt32(obj1) != 0)
                    {
                        //START CREATE
                        using (SqlCommand cmd2 = new SqlCommand("UPDATE DistanceBasedCardProfiles SET  CardProfileID=@rsn, CardProfileName=@rln, DiscountedFare=@DF WHERE ID=@hold;", conn))
                        {

                            cmd2.Parameters.AddWithValue("rsn", textBoxDBCP_CPID2.Text); //GET FROM PTO
                            cmd2.Parameters.AddWithValue("rln", textBoxDBCP_CPN2.Text);
                            cmd2.Parameters.AddWithValue("DF", holder3);
                            cmd2.Parameters.AddWithValue("hold", holder1);
                            cmd2.ExecuteNonQuery();
                            conn.Close();
                        }
                        MessageBox.Show("Card Profile updated!");
                        textBoxDBCP_CPN.Text = "";
                        textBoxDBCP_CPID.Text = "";
                        metroComboBoxDBCP_DFTID.Text = "";
                        holder3 = "";
                        UpdateALLGRIDS();
                        panel23.Refresh();
                        UpdateAllCombos();

                        IsMenuDisabled(false, buttonDBCPDEL);
                        buttonDBCPADD.Enabled = true;
                        panelDBCP.Visible = false;
                        buttonSaveDBCP.Visible = false;
                    }
                    else { MessageBox.Show("Card Profile already taken"); }
                }
            }
            else
            { MessageBox.Show("Please fill out all fields!"); }
        }

        private void metroComboBoxDBCP_DFTID2_TextChanged(object sender, EventArgs e)
        {
            CloseConn();
            holder3 = "";
            using (SqlCommand cmd = new SqlCommand("select ID from DiscountFareTables where FareTableID=@fid", conn))
            {
                cmd.Parameters.AddWithValue("fid", metroComboBoxDBCP_DFTID2.Text);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        holder3 = reader.GetValue(0).ToString().ToUpper();
                    }
                }
                CloseConn();
            }
        }

        private void dgvUC_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            holder2 = "";
            holder3 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                if (dgvUC.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    holder1 = dgvUC.SelectedRows[0].Cells[0].Value.ToString();
                    metroComboBoxUC_UN2.Text = dgvUC.SelectedRows[0].Cells[1].Value.ToString();
                    textBoxUC_UID2.Text = dgvUC.SelectedRows[0].Cells[2].Value.ToString();
                    holder2 = dgvUC.SelectedRows[0].Cells[3].Value.ToString();
                    if (holder2 == "Driver") radioButtonDRIVER2.Checked = true; else radioButtonDISPATCHER2.Checked = true;
                    metroDateTimeUCEDsecond.Value = Convert.ToDateTime(dgvUC.SelectedRows[0].Cells[4].Value.ToString());
                    metroDateTimeUCED2second.Value = Convert.ToDateTime(dgvUC.SelectedRows[0].Cells[5].Value.ToString());
                    IsMenuDisabled(true, buttonUC_DEL);
                    buttonUCADD.Enabled = false;
                    panelUC.Visible = true;
                    buttonSaveUC.Visible = true;
                }
            }
        }

        private void metroComboBoxUC_UN2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetails(metroComboBoxUC_UN2, "UserAccounts", "Id");
        }

        private void buttonSaveUC_Click(object sender, EventArgs e)
        {
            string PP_PID = "";
            CloseConn();

            if (textBoxUC_UID2.Text == "" || metroComboBoxUC_UN2.Text == "" || radioButtonDRIVER2.Checked == false && radioButtonDISPATCHER2.Checked == false)
            {
                MessageBox.Show("Please fill out all fields");
            }
            else
            {
                using (SqlCommand cmd2 = new SqlCommand("SELECT UserAccountID FROM UserAccounts WHERE Id=@param;", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("param", metroComboBoxUC_UN2.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        reader2.Read();
                        PP_PID = (reader2[0].ToString());
                    }
                    conn.Close();
                }

                using (SqlCommand cmd2 = new SqlCommand("UPDATE UserCards set UserID=@param, UID=@type, Role=@bfd2, CardEffectiveDateFrom=@ef, CardEffectiveDateTo=@et WHERE ID=@holder;", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("param", PP_PID);
                    cmd2.Parameters.AddWithValue("type", textBoxUC_UID2.Text);
                    if (radioButtonDRIVER2.Checked)
                        cmd2.Parameters.AddWithValue("bfd2", radioButtonDRIVER2.Text);
                    else
                        cmd2.Parameters.AddWithValue("bfd2", radioButtonDISPATCHER2.Text);
                    cmd2.Parameters.AddWithValue("ef", metroDateTimeUCEDsecond.Value.Date.ToString());
                    cmd2.Parameters.AddWithValue("et", metroDateTimeUCED2second.Value.Date.ToString());
                    cmd2.Parameters.AddWithValue("holder", holder1);
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                }
                metroComboBoxUC_UN.SelectedIndex = -1;
                radioButtonDRIVER.Checked = false;
                radioButtonDISPATCHER.Checked = false;
                textBoxUC_UID.Text = "";
                UpdateALLGRIDS();
                MessageBox.Show("UserCard Updated!");
                IsMenuDisabled(false, buttonUC_DEL);
                buttonUCADD.Enabled = true;
                panelUC.Visible = false;
                buttonSaveUC.Visible = false;
            }
        }

        private void dgvUA_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            holder2 = "";
            holder3 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                if (dgvUA.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    holder1 = dgvUA.SelectedRows[0].Cells[0].Value.ToString();
                    textBox_UA_UID2.Text = dgvUA.SelectedRows[0].Cells[1].Value.ToString();
                    metroComboBoxUA_MN2.Text = dgvUA.SelectedRows[0].Cells[2].Value.ToString();
                    textBoxUA_CID2.Text = dgvUA.SelectedRows[0].Cells[3].Value.ToString();
                    textBox_UA_SN2.Text = dgvUA.SelectedRows[0].Cells[4].Value.ToString();
                    textBox_UA_LN2.Text = dgvUA.SelectedRows[0].Cells[5].Value.ToString();
                    metroDateTimeEDUAsecond.Value = Convert.ToDateTime(dgvUA.SelectedRows[0].Cells[6].Value.ToString());
                    metroDateTimeEDUA2second.Value = Convert.ToDateTime(dgvUA.SelectedRows[0].Cells[7].Value.ToString());
                    IsMenuDisabled(true, buttonDELUA);
                    buttonUP_UA.Enabled = false;
                    panelUA.Visible = true;
                    buttonSaveUA.Visible = true;
                }
            }
        }

        private void metroComboBoxUA_MN2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetails(metroComboBoxUA_MN2, "Merchants", "ParticipantName");
        }

        private void buttonSaveUA_Click(object sender, EventArgs e)
        {
            CloseConn();
            string act = "";

            if (textBox_UA_UID2.Text != "" && textBox_UA_LN2.Text != "" && textBox_UA_SN2.Text != "" && metroComboBoxUA_MN2.Text != "" && textBoxUA_CID2.Text != "")
            {

                using (SqlCommand cmd1 = new SqlCommand("select COUNT(*) FROM UserAccounts WHERE ID = @pid", conn))
                {
                    conn.Open();
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("pid", textBox_UA_UID2.Text);
                    object obj1;
                    obj1 = cmd1.ExecuteScalar();
                    if (Convert.ToInt32(obj1) == 0 || Convert.ToInt32(obj1) != 0)
                    {
                        using (SqlCommand cmd2 = new SqlCommand("select COUNT(*) FROM UserAccounts WHERE LongName = @pid OR ShortName= @pids", conn))
                        {

                            cmd2.CommandType = CommandType.Text;
                            cmd2.Parameters.AddWithValue("pid", textBox_UA_LN2.Text);
                            cmd2.Parameters.AddWithValue("pids", textBox_UA_SN2.Text);
                            object obj2;
                            obj2 = cmd1.ExecuteScalar();
                            if (Convert.ToInt32(obj2) == 0 || Convert.ToInt32(obj2) != 0)
                            {

                                if (conn == null || conn.State == ConnectionState.Open)
                                    conn.Close();
                                using (SqlCommand cmd = new SqlCommand("SELECT ID FROM Merchants WHERE ParticipantName=@pid", conn))
                                {
                                    try
                                    {
                                        conn.Open();
                                        cmd.Parameters.AddWithValue("pid", metroComboBoxUA_MN2.Text);
                                        using (var reader2 = cmd.ExecuteReader())
                                        {
                                            reader2.Read();
                                            act = (reader2[0].ToString());
                                        }

                                    }
                                    catch { }
                                }

                                //START CREATE
                                using (SqlCommand cmd3 = new SqlCommand(@"UPDATE UserAccounts SET ID=@ftid, ParticipantID=@fa, CompanyID=@bfd, ShortName=@bfd1, LongName=@bfd2, UserEffectiveDateFrom=@ef, UserEffectiveDateTo=@et WHERE UserAccountID=@hold;", conn))
                                {

                                    cmd3.Parameters.AddWithValue("ftid", textBox_UA_UID2.Text);
                                    cmd3.Parameters.AddWithValue("fa", act);
                                    cmd3.Parameters.AddWithValue("bfd", textBoxUA_CID2.Text);
                                    cmd3.Parameters.AddWithValue("bfd1", textBox_UA_SN2.Text);
                                    cmd3.Parameters.AddWithValue("bfd2", textBox_UA_LN2.Text);
                                    cmd3.Parameters.AddWithValue("ef", metroDateTimeEDUAsecond.Value.Date.ToString());
                                    cmd3.Parameters.AddWithValue("et", metroDateTimeEDUA2second.Value.Date.ToString());
                                    cmd3.Parameters.AddWithValue("hold", holder1);
                                    cmd3.ExecuteNonQuery();

                                    conn.Close();
                                }

                                textBox_UA_UID2.Text = "";
                                textBox_UA_LN2.Text = "";
                                textBox_UA_SN2.Text = "";
                                metroComboBoxUA_MN2.SelectedIndex = -1;
                                metroDateTimeEDUAsecond.Value = DateTime.Now;
                                metroDateTimeEDUA2second.Value = DateTime.Now;
                                UpdateALLGRIDS();
                                UpdateAllCombos();

                                MessageBox.Show("User Account updated!");
                                IsMenuDisabled(false, buttonDELUA);
                                buttonUP_UA.Enabled = true;
                                panelUA.Visible = false;
                                buttonSaveUA.Visible = false;
                            }
                            else
                            {
                                MessageBox.Show("User Name already taken");
                            }
                        }
                    }
                    else { MessageBox.Show("UID already taken"); }
                }

            }
            else { MessageBox.Show("Please fill out all fields!"); }
        }

        private void textBoxINS_SelectedIndexChanged(object sender, EventArgs e)
        {
            try { 
            UpdateGrid("select * from " + textBoxINS.Text, dgvINSERT);
            }
            catch { }
        }

        private void dgvRBF_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            holder2 = "";
            holder3 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                if (dgvRBF.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    holder1 = dgvRBF.SelectedRows[0].Cells[0].Value.ToString();
                    metroComboBoxRBF_RID2.Text = dgvRBF.SelectedRows[0].Cells[1].Value.ToString();
                    metroComboBoxRBF_BFI2.Text = dgvRBF.SelectedRows[0].Cells[2].Value.ToString();
                    IsMenuDisabled(true, buttonDELRBF);
                    buttonRBF_Gen.Enabled = false;
                    panelRBF.Visible = true;
                    buttonSAVERBF.Visible = true;
                }
            }
        }

        private void buttonSAVERBF_Click(object sender, EventArgs e)
        {
            CloseConn();
            if (metroComboBoxRBF_BFI2.Text == "" || metroComboBoxRBF_RID2.Text == "")
            {
                MessageBox.Show("Please fill out all fields");
            }
            else
            {
                try
                {
                    CloseConn();
                    using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM DistanceBasedRoutes WHERE RouteLongName=@pid", conn))
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("pid", metroComboBoxRBF_RID2.Text);
                        using (var reader2 = cmd2.ExecuteReader())
                        {
                            reader2.Read();
                            RBF_RID = (reader2[0].ToString());
                        }
                        CloseConn();
                        ShowDetails(metroComboBoxRBF_RID2, "DistanceBasedRoutes", "RouteLongName");
                    }
                }
                catch { conn.Close(); }

                try
                {
                    CloseConn();
                    using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM DistanceBasedFareTables WHERE FareTableID=@pid", conn))
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("pid", metroComboBoxRBF_BFI2.Text);
                        using (var reader2 = cmd2.ExecuteReader())
                        {
                            reader2.Read();
                            RBF_BFI = (reader2[0].ToString());
                        }
                        conn.Close();

                        ShowDetails(metroComboBoxRBF_BFI2, "DistanceBasedFareTables", "FareTableID");
                    }
                }
                catch { }

                using (SqlCommand cmd2 = new SqlCommand("UPDATE RouteBasedFares SET RouteID=@pname, BasedFareID=@pid WHERE ID=@hold", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pname", RBF_RID);
                    cmd2.Parameters.AddWithValue("pid", RBF_BFI);
                    cmd2.Parameters.AddWithValue("hold", holder1);
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                }
                metroComboBoxRBF_BFI.SelectedIndex = -1;
                metroComboBoxRBF_RID.SelectedIndex = -1;
                UpdateALLGRIDS();
                IsMenuDisabled(false, buttonDELRBF);
                buttonRBF_Gen.Enabled = true;
                panelRBF.Visible = false;
                buttonSAVERBF.Visible = false;
                holder1 = "";
                MessageBox.Show("Route Based Fare ID updated!");
            }
        }

        private void metroComboBoxRBF_RID2_SelectedIndexChanged(object sender, EventArgs e)
        {
         ShowDetails(metroComboBoxRBF_RID2, "DistanceBasedRoutes", "RouteLongName");
        }

        private void metroComboBoxRBF_BFI2_SelectedIndexChanged(object sender, EventArgs e)
        {
          ShowDetails(metroComboBoxRBF_BFI2, "DistanceBasedFareTables", "FareTableID");
        }

        private void metroComboBoxRDBF_RID2_SelectedIndexChanged(object sender, EventArgs e)
        {    
          ShowDetails(metroComboBoxRDBF_RID2, "DistanceBasedRoutes", "RouteLongName");
        }

        private void metroComboBoxRDBF_DID2_SelectedIndexChanged(object sender, EventArgs e)
        {

                ShowDetails(metroComboBoxRDBF_DID2, "DiscountFareTables", "FareTableID");
        }

        private void buttonSAVERDF_Click(object sender, EventArgs e)
        {
            CloseConn();
            if (metroComboBoxRDBF_DID2.Text == "" || metroComboBoxRDBF_RID2.Text == "")
            {
                MessageBox.Show("Please fill out all fields");
            }
            else
            {
                CloseConn();
                using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM DistanceBasedRoutes WHERE RouteLongName=@pid", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", metroComboBoxRDBF_RID2.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        try
                        {
                            reader2.Read();
                            RDF_RID = (reader2[0].ToString());
                        }
                        catch
                        {
                            conn.Close();
                        }
                    }
                    conn.Close();
                
                }
                CloseConn();
                using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM DiscountFareTables WHERE FareTableID=@pid", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", metroComboBoxRDBF_DID2.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        try
                        {
                            reader2.Read();
                            RDF_DID = (reader2[0].ToString());
                        }
                        catch
                        {
                            conn.Close();
                        }
                    }
                    conn.Close();
                }
                using (SqlCommand cmd2 = new SqlCommand("UPDATE RouteDiscountedFares SET RouteID=@pname, DiscountedID=@pid WHERE ID=@hold", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pname", RDF_RID);
                    cmd2.Parameters.AddWithValue("pid", RDF_DID);
                    cmd2.Parameters.AddWithValue("hold", holder1);
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                }
                metroComboBoxRDBF_DID.SelectedIndex = -1;
                metroComboBoxRDBF_RID.SelectedIndex = -1;
                UpdateALLGRIDS();
                MessageBox.Show("Route Discounted Fare ID Updated!");
                IsMenuDisabled(false, buttonDELRDF);
                buttonRDBF_Gen.Enabled = true;
                panelRDF.Visible = false;
                buttonSAVERDF.Visible = false;
            }
        }

        private void dgvRDF_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            holder2 = "";
            holder3 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                if (dgvRDF.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    holder1 = dgvRDF.SelectedRows[0].Cells[0].Value.ToString();
                    metroComboBoxRDBF_RID2.Text = dgvRDF.SelectedRows[0].Cells[1].Value.ToString();
                    metroComboBoxRDBF_DID2.Text = dgvRDF.SelectedRows[0].Cells[2].Value.ToString();
                    IsMenuDisabled(true, buttonDELRDF);
                    buttonRDBF_Gen.Enabled = false;
                    panelRDF.Visible = true;
                    buttonSAVERDF.Visible = true;
                }
            }
        }

        private void dgvPR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            holder1 = "";
            holder2 = "";
            holder3 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                if (dgvPR.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    holder1 = dgvPR.SelectedRows[0].Cells[0].Value.ToString();
                    metroComboBoxPR_RID2.Text = dgvPR.SelectedRows[0].Cells[1].Value.ToString();
                    metroComboBoxPR_PID2.Text = dgvPR.SelectedRows[0].Cells[2].Value.ToString();
                    IsMenuDisabled(true, buttonDELPR);
                    buttonPR_Gen.Enabled = false;
                    panelPR.Visible = true;
                    buttonSavePR.Visible = true;
                }
            }
        }

        private void dgvPDB_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            holder2 = "";
            holder3 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                if (dgvPDB.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    holder1 = dgvPDB.SelectedRows[0].Cells[0].Value.ToString();
                    metroComboBoxPDB_PID2.Text = dgvPDB.SelectedRows[0].Cells[1].Value.ToString();
                    metroComboBoxPDB_BID2.Text = dgvPDB.SelectedRows[0].Cells[2].Value.ToString();
                    IsMenuDisabled(true, buttonDELPDB);
                    buttonPDB_Gen.Enabled = false;
                    panelPDB.Visible = true;
                    buttonSAVEPDB.Visible = true;
                }
            }
        }

        private void dgvPP_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            holder2 = "";
            holder3 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                if (dgvPP.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    holder1 = dgvPP.SelectedRows[0].Cells[0].Value.ToString();
                    metroComboBoxPP_PID2.Text = dgvPP.SelectedRows[0].Cells[1].Value.ToString();
                    metroComboBoxPP_SPI2.Text = dgvPP.SelectedRows[0].Cells[2].Value.ToString();
                    IsMenuDisabled(true, buttonDELPP);
                    buttonGenPP.Enabled = false;
                    panelPP.Visible = true;
                    buttonsavePP.Visible = true;
                }
            }
        }

        private void metroComboBoxPR_PID2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetails(metroComboBoxPR_PID2, "BeejeesProfiles", "ProfileName");
        }

        private void metroComboBoxDBCP_DFTID2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetails(metroComboBoxDBCP_DFTID2, "DiscountFareTables", "FareTableID");
        }

        private void mcbxProfiles2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetails(mcbxProfiles2, "BeejeesProfiles", "ProfileName");
        }

        private void mcbxProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetails(mcbxProfiles, "BeejeesProfiles", "ProfileName");
        }

        private void mcbxFTID_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetails(mcbxFTID, "DiscountFareTables", "FareTableID");
        }

        private void mcbxFTID2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetails(mcbxFTID2, "DiscountFareTables", "FareTableID");
        }
        private void metroComboBoxPR_RID2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetails(metroComboBoxPR_RID2, "DistanceBasedRoutes", "RouteLongName");
        }

        private void metroComboBoxPDB_PID2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetails(metroComboBoxPDB_PID2, "BeejeesProfiles", "ProfileName");
        }

        private void metroComboBoxPDB_BID2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetails(metroComboBoxPDB_BID2, "DistanceBasedFareTables", "FaretableID");
        }

        private void metroComboBoxPP_PID2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetails(metroComboBoxPP_PID2, "BeejeesProfiles", "ProfileID");
        }

        private void metroComboBoxPP_SPI2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetails(metroComboBoxPP_SPI2, "SystemParameters", "ID");
            if (metroComboBoxPP_SPI2.Text != "")
            {
                using (SqlCommand cmd2 = new SqlCommand("SELECT Value FROM SystemParameters WHERE ID=@param;", conn))
                {
                    CloseConn();
                    conn.Open();
                    cmd2.Parameters.AddWithValue("param", metroComboBoxPP_SPI2.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        reader2.Read();
                        labelSSVAL2.Text = "System Parameter Value: " + (reader2[0].ToString());
                    }
                    conn.Close();
                }
            }
            else
            {
                labelSSVAL2.Text = "";
            }
        }

        private void buttonSavePR_Click(object sender, EventArgs e)
        {
            CloseConn();
            if (metroComboBoxPR_PID2.Text == "" || metroComboBoxPR_RID2.Text == "")
            {
                MessageBox.Show("Please fill out all fields");
            }
            else
            {
                CloseConn();
                using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM BeejeesProfiles WHERE ProfileName=@pid", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", metroComboBoxPR_PID2.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        try
                        {
                            reader2.Read();
                            PR_PID = (reader2[0].ToString());
                        }
                        catch
                        {
                            conn.Close();
                        }
                    }
                    conn.Close();
                   
                }

                CloseConn();
                using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM DistanceBasedRoutes WHERE RouteLongName=@pid", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", metroComboBoxPR_RID2.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        try
                        {
                            reader2.Read();
                            PR_RID = (reader2[0].ToString());
                        }
                        catch
                        {
                            conn.Close();
                        }
                    }
                    conn.Close();
                    
                }

                using (SqlCommand cmd2 = new SqlCommand("UPDATE ProfileRoutes SET ProfileID=@pname, RoutesID=@pid WHERE ID=@hold", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pname", PR_PID);
                    cmd2.Parameters.AddWithValue("pid", PR_RID);
                    cmd2.Parameters.AddWithValue("hold", holder1);
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                }
                metroComboBoxPR_PID2.SelectedIndex = -1;
                metroComboBoxPR_RID2.SelectedIndex = -1;
                UpdateALLGRIDS();
                IsMenuDisabled(false, buttonDELPR);
                buttonPR_Gen.Enabled = true;
                panelPR.Visible = false;
                buttonSavePR.Visible = false;
                MessageBox.Show("Profile Routes ID updated!");
            }
        }

        private void buttonSAVEPDB_Click(object sender, EventArgs e)
        {
            CloseConn();
            if (metroComboBoxPDB_PID2.Text == "" || metroComboBoxPDB_BID2.Text == "")
            {
                MessageBox.Show("Please fill out all fields");
            }
            else
            {
                CloseConn();
                using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM BeejeesProfiles WHERE ProfileName=@pid", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("pid", metroComboBoxPDB_PID2.Text);
                        using (var reader2 = cmd2.ExecuteReader())
                        {
                            reader2.Read();
                            PDB_PID = (reader2[0].ToString());
                        }
                        conn.Close();
                      
                    }
                    catch { }
                }

                CloseConn();
                using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM DistanceBasedFareTables WHERE FareTableID=@pid", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", metroComboBoxPDB_BID2.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        try
                        {
                            reader2.Read();
                            PDB_BID = (reader2[0].ToString());
                        }
                        catch
                        {
                            conn.Close();
                        }
                    }
                    conn.Close();
                   
                }
                using (SqlCommand cmd2 = new SqlCommand("UPDATE ProfileDistanceBaseds SET ProfileID=@pname, BasedID=@pid WHERE ID=@hold", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pname", PDB_PID);
                    cmd2.Parameters.AddWithValue("pid", PDB_BID);
                    cmd2.Parameters.AddWithValue("hold", holder1);
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                }
                metroComboBoxPDB_PID2.SelectedIndex = -1;
                metroComboBoxPDB_BID2.SelectedIndex = -1;
                UpdateALLGRIDS();
                MessageBox.Show("Profile Distance Based ID updated!");
                IsMenuDisabled(false, buttonDELPDB);
                buttonPDB_Gen.Enabled = true;
                panelPDB.Visible = false;
                buttonSAVEPDB.Visible = false;
            }
        }

        private void buttonsavePP_Click(object sender, EventArgs e)
        {

            string PP_PID;
            CloseConn();

            if (metroComboBoxPP_PID2.Text == "" || metroComboBoxPP_SPI2.Text == "")
            {
                MessageBox.Show("Please fill out all fields");
            }
            else
            {
                using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM BeejeesProfiles WHERE ProfileID=@param;", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("param", metroComboBoxPP_PID2.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        reader2.Read();
                        PP_PID = (reader2[0].ToString());
                    }
                    conn.Close();
                }

                using (SqlCommand cmd2 = new SqlCommand("UPDATE ProfileParameters SET  ProfileID=@param, SystemParametersID=@type WHERE ID=@hold", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("param", PP_PID);
                    cmd2.Parameters.AddWithValue("type", metroComboBoxPP_SPI2.Text);
                    cmd2.Parameters.AddWithValue("hold", holder1);
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                }
                metroComboBoxPP_PID2.SelectedIndex = -1;
                metroComboBoxPP_SPI2.SelectedIndex = -1;
                UpdateALLGRIDS();
                MessageBox.Show("Profile Parameters ID updated!");
                IsMenuDisabled(false, buttonDELPDB);
                buttonPDB_Gen.Enabled = true;
                panelPDB.Visible = false;
                buttonSAVEPDB.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string query;
            CloseConn();
            try
            {


        query = " select a.id DBFT_ID, a.FareTableID DBFT_FareTableID, A.FareAmount DBFT_FareAmount, a.BaseFareDistance DBFT_BaseFareDistance, b.IncrementalFareAmount DBIF_IncrementAmount " +
        ", b.IncrementalFareDistance DBIF_IncrementDistance, b.StartDistance DBIF_StartDistance " + 
        ", c.ID DBR_ID, c.RouteID, c.RouteShortName " +
        ",e.Id DiscountID, e.FareTableID DiscFareTableID, e.Discount Discount, e.ROundingMethod, e.RoundingAccuracy " +
        ",g.ProfileID BJProfileID, g.ProfileName BJProfileName " +
        ", j.id Merchants_ID, j.ParticipantId MerchantsParticipantID, k.FleetsName BJFleetsName , L.TerminalID "+
        "From DistanceBasedFareTables a, DistanceBasedIncrementFares b ,DistanceBasedRoutes c, RouteBasedFares d,DiscountFareTables e , RouteDiscountedFares f ,BeejeesProfiles g "+
        ", ProfileDiscounts h,ProfileRoutes i , Merchants j  ,BeejeesFleets k , Terminals l where   a.id = b.faretable_id " +
        "and d.basedfareid = a.id and d.RouteID = c.id and f.RouteID = c.id and f.Discountedid = e.id and h.ProfileID = g.id and h.DiscountID = e.id " +
        "and i.ProfileId = g.id and i.RoutesID = c.id and k.ProfileId = g.id And K.PTO = j.id and L.Participantid = j.id and L.fleetid = k.id " +
        "and L.TerminalId = " + @"'" + txtBxTerminalID.Text + @"'";

                using (SqlCommand cmd2 = new SqlCommand(query, conn))
                {
                    conn.Open();
                    cmd2.ExecuteNonQuery();

                    conn.Close();
                    UpdateGrid(query, dgvSEARCH);
                }
        }
            catch /*(Exception ex)*/
            {
                //MessageBox.Show("Invalid Value for column" + Environment.NewLine + ex.GetType().FullName);
            }
}

        private void metroComboBoxSearchParam_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (metroComboBoxSearchParam.Text == "Terminal ID") {
                panelTerminalID.Visible = true;
                panelProfileID.Visible = false;
                panelUID.Visible = false;
            }
            if (metroComboBoxSearchParam.Text == "Profile ID")
            {
                panelTerminalID.Visible = false;
                panelProfileID.Visible = true;
                panelUID.Visible = false;
            }
            if (metroComboBoxSearchParam.Text == "UID" || metroComboBoxSearchParam.Text == "UserCards")
            {
                panelTerminalID.Visible = false;
                panelProfileID.Visible = false;
                panelUID.Visible = true;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string query;
            CloseConn();
            if (textBoxProfileID.Text != "" && textBoxVal.Text != "" && metroComboBoxParam.Text != "") {
                try
                {
                 

                    query = "  select a.id, a.ProfileId, a.SystemParametersID " +
                     ", b.Parameter, b.value, b.type " +
                    "from ProfileParameters a" +
                    ", SystemParameters b " +
                    "where   a.SystemParametersID = b.id " +
                    "and a.ProfileID = (select TOP 1 id from beejeesprofiles where profileid=" + textBoxProfileID.Text + ") " +
                    "and b.parameter=" + @"'" + metroComboBoxParam.Text + @"' " +
                    "and b.value =" + @"'" + textBoxVal.Text + @"'";

                    using (SqlCommand cmd2 = new SqlCommand(query, conn))
                {
                    conn.Open();
                    cmd2.ExecuteNonQuery();

                    conn.Close();
                    UpdateGrid(query, dgvSEARCH);
                }
            }
            catch /*(/*Exception ex)*/
            {
              //  MessageBox.Show("Invalid Value for column" + Environment.NewLine + ex.GetType().FullName);
            }
        }
            else { MessageBox.Show("Please fill out all fields"); }
        }

        private void buttonUID_Click(object sender, EventArgs e)
        {
            string query, query2;
            CloseConn();

            query = "select a.id, a.userid, a.role ,b.CompanyID UserAccountCompanyID, b.ShortName,c.ParticipantID , c.ParticipantName " +
                    "from UserCards a, UserAccounts b,Merchants c " +
                    "where a.userid = b.useraccountid  and b.Participantid = c.id and uid =" + @"'" + textBoxUID.Text + @"'";

            query2 = "Select  a.id, a.userid CardsUserID, b.id UserAccountID, a.uid CardsUID, a.role CardsRole, c.participantname Operator from UserCards a, UserAccounts b, Merchants c "+
                     "where   a.userid = b.useraccountid and uid=" + @"'" + textBoxUID.Text + @"'" +
                     "order by CardsUID desc";
            try { 
            if (metroComboBoxSearchParam.Text == "UID")
            {
                using (SqlCommand cmd2 = new SqlCommand(query, conn))
                {
                    conn.Open();
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                    UpdateGrid(query, dgvSEARCH);
                }
            }
            else
            {
                using (SqlCommand cmd2 = new SqlCommand(query2, conn))
                {
                    conn.Open();
                    cmd2.ExecuteNonQuery();

                    conn.Close();
                    UpdateGrid(query2, dgvSEARCH);
                }
            }
            }
            catch
            {

            }

        }

        private void buttonSAVEPD_Click(object sender, EventArgs e)
        {
            CloseConn();
            if (mcbxProfiles2.Text == "" ||mcbxFTID2.Text == "")
            {
                MessageBox.Show("Please fill out all fields");
            }
            else
            {
                CloseConn();
                using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM BeejeesProfiles WHERE ProfileName=@pid", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("pid", mcbxProfiles2.Text);
                        using (var reader2 = cmd2.ExecuteReader())
                        {
                            reader2.Read();
                            PD_PID = (reader2[0].ToString());
                        }
                        conn.Close();

                    }
                    catch { }
                }

                CloseConn();
                using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM DiscountFareTables WHERE FareTableID=@pid", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", mcbxFTID2.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        try
                        {
                            reader2.Read();
                            PD_BID = (reader2[0].ToString());
                        }
                        catch
                        {
                            conn.Close();
                        }
                    }
                    conn.Close();

                }
                using (SqlCommand cmd2 = new SqlCommand("UPDATE ProfileDiscounts SET ProfileID=@pname, DiscountID=@pid WHERE ID=@hold", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pname", PD_PID);
                    cmd2.Parameters.AddWithValue("pid", PD_BID);
                    cmd2.Parameters.AddWithValue("hold", holder1);
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                }
                mcbxProfiles2.SelectedIndex = -1;
                mcbxFTID2.SelectedIndex = -1;
                UpdateALLGRIDS();
                MessageBox.Show("Profile Discounts ID updated!");
                IsMenuDisabled(false, buttonDELPD);
                buttonGENPD.Enabled = true;
                panelPD.Visible = false;
                buttonSAVEPD.Visible = false;
            }
        }
        string PD_PID = "";
        string PD_BID = "";
        private void buttonGENPD_Click(object sender, EventArgs e)
        {

            CloseConn();
            if (mcbxProfiles.Text == "" || mcbxFTID.Text == "")
            {
                MessageBox.Show("Please fill out all fields");
            }
            else
            {
                using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM BeejeesProfiles WHERE ProfileName=@pid", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd2.Parameters.AddWithValue("pid", mcbxProfiles.Text);
                        using (var reader2 = cmd2.ExecuteReader())
                        {
                            reader2.Read();
                            PD_PID = (reader2[0].ToString());
                        }
                        conn.Close();

                    }
                    catch { }
                }

                CloseConn();
                using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM DiscountFareTables WHERE FareTableID=@pid", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pid", mcbxFTID.Text);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        try
                        {
                            reader2.Read();
                            PD_BID = (reader2[0].ToString());
                        }
                        catch
                        {
                            conn.Close();
                        }
                    }
                    conn.Close();

                }
                using (SqlCommand cmd2 = new SqlCommand("INSERT INTO ProfileDiscounts VALUES (NEWID(), @pname, @pid);", conn))
                {
                    conn.Open();
                    cmd2.Parameters.AddWithValue("pname", PD_PID);
                    cmd2.Parameters.AddWithValue("pid", PD_BID);
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                }
                mcbxProfiles.SelectedIndex = -1;
                mcbxFTID.SelectedIndex = -1;
                UpdateALLGRIDS();
                MessageBox.Show("Profile Discount ID generated!");
            }
        }

        private void buttonDELPD_Click(object sender, EventArgs e)
        {
            CloseConn();
            string a = "";
            string b = "";
            if (dgvPD.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgvPD.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgvPD.Rows[selectedrowindex];

                a = Convert.ToString(selectedRow.Cells[1].Value);
                b = Convert.ToString(selectedRow.Cells[0].Value);
            }

            DialogResult dialogResult = MessageBox.Show("Delete " + a + "?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand("DELETE FROM ProfileDiscounts WHERE ID=@param", conn))
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
                UpdateAllCombos();
            }
        }

        private void dgvPD_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            holder1 = "";
            holder2 = "";
            holder3 = "";
            CloseConn();
            if (togDEL.Checked)
            {
                if (dgvPD.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    holder1 = dgvPD.SelectedRows[0].Cells[0].Value.ToString();
                   mcbxProfiles2.Text = dgvPD.SelectedRows[0].Cells[1].Value.ToString();
                   mcbxFTID2.Text = dgvPD.SelectedRows[0].Cells[2].Value.ToString();
                    IsMenuDisabled(true, buttonDELPD);
                    buttonGENPD.Enabled = false;
                    panelPD.Visible = true;
                    buttonSAVEPD.Visible = true;
                }
            }
        }
        public void ExportTOCSV(MetroFramework.Controls.MetroGrid mg)
        {
            string labelCSV="";

            BindingSource bs = new BindingSource();
            bs.DataSource = mg.DataSource;
            DataTable dataTable = (DataTable)(bs.DataSource);

            var lines = new List<string>();
            string[] columnNames = dataTable.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName).
                                              ToArray();
            var header = string.Join(",", columnNames);
            lines.Add(header);

            var valueLines = dataTable.AsEnumerable()
                               .Select(row => string.Join(",", row.ItemArray));
            lines.AddRange(valueLines);
            using (var selectFileDialog = new SaveFileDialog())
            { 
                selectFileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
                selectFileDialog.Title = "Save CSV file";
                if (selectFileDialog.ShowDialog() == DialogResult.OK) {labelCSV = selectFileDialog.FileName;}}
            try { 
            File.WriteAllLines(labelCSV, lines);
            FileInfo fi = new FileInfo(labelCSV);
                if (fi.Exists)
                {System.Diagnostics.Process.Start(labelCSV);}
                else
                {//file doesn't exist 
                } }
            catch { }
        }

        private void buttonRemoveSP_Click(object sender, EventArgs e)
        {
            int num = 0;
            CloseConn();
            string query = "WITH TempEmp(Value, duplicateRecCount) " +
                           "AS  " +
                           "(  " +
                           "SELECT Value + Parameter, ROW_NUMBER() OVER(PARTITION by Value + Parameter ORDER BY Value + Parameter)  " +
                           "AS duplicateRecCount  " +
                           "FROM dbo.SystemParameters  " +
                           ")  " +
                           "DELETE FROM TempEmp  " +
                           "WHERE duplicateRecCount > 1  ";

            DialogResult dialogResult = MessageBox.Show("Delete Redundancies? Once you click Yes, data lost cannot be retrieved", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlCommand cmd2 = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        num = cmd2.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("Deleted " + num.ToString() + " row/s");
                    }
                    catch { MessageBox.Show("Deleted " + num.ToString() + " row/s"); }
                }
                CloseConn();
                UpdateALLGRIDS();
                UpdateAllCombos();
            }


        }

        private void buttonConfigChecker_Click(object sender, EventArgs e)
        {
            tabMASTER.SelectedIndex = 25;
            titleLB.Text = "Config Checker";
        }
        int process = 0;
        string profile = "";
        string faretableid = "";
        string discountid = "";
        string rid = "";
        private void timerCHECK_Tick(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                if (process == 0)
                {
                    labelCHECK.Text = "Checking... Fare Table ID";
                    CloseConn();
                    using (SqlCommand cmd2 = new SqlCommand("SELECT ID FROM DistanceBasedRoutes WHERE RouteLongName=@pid", conn))
                    {
                        try
                        {
                            conn.Open();
                            cmd2.Parameters.AddWithValue("pid", comboCHECK.Text);
                            using (var reader2 = cmd2.ExecuteReader())
                            {
                                reader2.Read();
                                rid = (reader2[0].ToString());
                            }
                            conn.Close();

                        }
                        catch { }
                    }
                    CloseConn();
                    string query = "SELECT BasedFareID FROM Routebasedfares WHERE RouteID ='" + rid + "';";
                    UpdateGrid(query, dgvCC);
                    if (dgvCC.RowCount <= 0) { timerCHECK.Stop(); labelCHECK.Text += Environment.NewLine + "No DistanceBased FareTableID available"; profile = ""; faretableid = ""; discountid = ""; success = false; timerCHECK.Stop(); }
                    else { faretableid = dgvCC.SelectedRows[0].Cells[0].Value.ToString(); labelCHECK.Text += " Checked ✓" + Environment.NewLine; success = true; }

                }
                else if (process == 1)
                {

                    labelCHECK.Text += "Checking Discount Fare Table ID......";
                    CloseConn();
                    string query = "SELECT DiscountedID FROM RouteDiscountedfares WHERE RouteID ='" + rid + "';";
                    UpdateGrid(query, dgvCC);
                    if (dgvCC.RowCount <= 0) { timerCHECK.Stop(); labelCHECK.Text += "No DiscountID available"; profile = ""; faretableid = ""; discountid = ""; success = false; timerCHECK.Stop(); }
                    else { discountid = dgvCC.SelectedRows[0].Cells[0].Value.ToString(); labelCHECK.Text += " Checked ✓" + Environment.NewLine; success = true; }
                }
                else if (process == 2)
                {

                    labelCHECK.Text += "Checking Profile ID......";
                    CloseConn();
                    string query = "SELECT ProfileID FROM ProfileRoutes WHERE RoutesID ='" + rid + "';";
                    UpdateGrid(query, dgvCC);
                    if (dgvCC.RowCount <= 0) { timerCHECK.Stop(); labelCHECK.Text += "No Profile ID available"; profile = ""; faretableid = ""; discountid = ""; success = false; timerCHECK.Stop(); }
                    else { profile = dgvCC.SelectedRows[0].Cells[0].Value.ToString(); labelCHECK.Text += " Checked ✓" + Environment.NewLine; success = true; }
                }
                else if (process == 3)
                {

                    labelCHECK.Text += "Checking Profile Discounts......";
                    CloseConn();
                    string query = "SELECT * FROM ProfileDiscounts WHERE DiscountID ='" + discountid + "' AND ProfileID ='" + profile + "';";
                    UpdateGrid(query, dgvCC);
                    if (dgvCC.RowCount <= 0) { timerCHECK.Stop(); labelCHECK.Text += "No Profile Discount available"; profile = ""; faretableid = ""; discountid = ""; success = false; timerCHECK.Stop(); }
                    else { labelCHECK.Text += " Checked ✓" + Environment.NewLine; success = true; }
                }
                else if (process == 4)
                {

                    labelCHECK.Text += "Checking Profile Distance Based......";
                    CloseConn();
                    string query = "SELECT * FROM ProfileDistanceBaseds WHERE ProfileID ='" + profile + "' AND BasedID ='" + faretableid + "';";
                    UpdateGrid(query, dgvCC);
                    if (dgvCC.RowCount <= 0) { timerCHECK.Stop(); labelCHECK.Text += "No BasedID available (ProfileDistanceBaseds) "; profile = ""; faretableid = ""; discountid = ""; success = false; timerCHECK.Stop(); }
                    else { labelCHECK.Text += " Checked ✓" + Environment.NewLine; success = true; }
                }
                else if (process == 5)
                {

                    labelCHECK.Text += "Checking Profile Parameters......";
                    CloseConn();
                    string query = "SELECT * FROM ProfileParameters WHERE ProfileID ='" + profile + "';";
                    UpdateGrid(query, dgvCC);
                    if (dgvCC.RowCount <= 0) { timerCHECK.Stop(); labelCHECK.Text += "No Profile Parameters available"; profile = ""; faretableid = ""; discountid = ""; success = false; timerCHECK.Stop(); }
                    else { labelCHECK.Text += " Checked ✓" + Environment.NewLine; success = true; }
                }
                else if (process == 6)
                {

                    labelCHECK.Text += "Checking Card Profiles......";
                    CloseConn();
                    string query = "SELECT CardProfileID, CardProfileName FROM DistanceBasedCardProfiles WHERE DiscountedFare ='" + discountid + "';";
                    UpdateGrid(query, dgvCC);
                    if (dgvCC.RowCount <= 0) { timerCHECK.Stop(); labelCHECK.Text += "No Card Profile available"; profile = ""; faretableid = ""; discountid = ""; success = false; timerCHECK.Stop(); }
                    else { labelCHECK.Text += " Checked ✓" + Environment.NewLine + "All parameters complete!"; success = false; }
                }


                if (success) { process = process + 1; } else { process = -1; timerCHECK.Stop(); startInitial.Enabled = true; pb.Visible = false; efx.Visible = false; startInitial.Visible = true; buttonPauseResume.Visible = false; }
            }
            catch { } }

        private void button1_Click_2(object sender, EventArgs e)
        {
            startInitial.Visible = false;
            pb.Visible = true;
            efx.Visible = true;
            labelCHECK.Text = "";
            startInitial.Enabled = false;
            timerCHECK.Start();
            process = 0;
            buttonPauseResume.Visible = true;
        }
        bool isPaused = false;
        private void buttonPauseResume_Click(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                buttonPauseResume.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.RUN1));
                timerCHECK.Stop();
                pb.Visible = false;
                efx.Visible = false;
                isPaused = true;
            }
            else
            {
                buttonPauseResume.BackgroundImage = Properties.Resources.pause;
                timerCHECK.Start();
                pb.Visible = true;
                efx.Visible = true;
                isPaused = false;
            }
        }

        private void qm_Click(object sender, EventArgs e)
        {
            using (qm qm  = new qm())
            {
               qm.ShowDialog();
            }
        }

        private void dgvCC_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvCC);
        }

        private void CelltoClip(object sender, DataGridViewCellEventArgs e, DataGridView dgv)
        {
            try
            {
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

        private void dgvPD_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvPD);
        }

        private void dgvSEARCH_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvSEARCH);
        }

        private void dgvDBCP_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvDBCP);
        }

        private void dgvSFF_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvSFF);
        }

        private void dgvBS_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvBS);
        }

        private void dgvDEL_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvDEL);
        }

        private void dgvUPDATE_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvUPDATE);
        }

        private void dgvINSERT_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvINSERT);
        }

        private void dgvUC_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvUC);
        }

        private void dgvUA_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvUA);
        }

        private void dgvDBIF_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvDBIF);
        }

        private void dgvSP_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvSP);
        }

        private void dgvPP_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvPP);
        }

        private void dgvPDB_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvPDB);
        }

        private void dgvPR_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvPR);

        }

        private void dgvRDF_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvRDF);
        }

        private void dgvRBF_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvRBF);
        }

        private void dgvDBFT_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvDBFT);
        }

        private void dgvDFT_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvDFT);
        }

        private void dgvDBR_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvDBR);
        }

        private void metroGridFleets_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, metroGridFleets);
        }

        private void dataGridViewFleets_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dataGridViewFleets);
        }

        private void dataGridViewBP_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dataGridViewBP);
        }

        private void dgvMERC_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CelltoClip(sender, e, dgvMERC);
        }
    }
}

