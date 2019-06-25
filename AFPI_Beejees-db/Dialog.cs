using System;
using System.Windows.Forms;

namespace AFPI_Beejees_db
{
    public partial class Dialog : MetroFramework.Forms.MetroForm
    {
        public Dialog()
        {
            InitializeComponent();
        }

        private void Dialog_Load(object sender, EventArgs e)
        {

        }
        Timer t1 = new Timer();

        private void main_FormClosing(object sender, FormClosingEventArgs e)
        {
            t1.Interval = 5;
            e.Cancel = true;    //cancel the event so the form won't be closed

            t1.Tick += new EventHandler(fadeOut);  //this calls the fade out function
            t1.Start();

            if (Opacity == 0)  //if the form is completly transparent
                e.Cancel = false;   //resume the event - the program can be closed

        }

        void fadeOut(object sender, EventArgs e)
        {
            if (Opacity <= 0)     //check if opacity is 0
            {
                t1.Stop();    //if it is, we stop the timer
                Close();   //and we try to close the form
            }
            else
                Opacity -= 0.05;
        }
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            GetResult();
            this.Close();
        }

        public string GetResult()
        {
            return textBox1.Text;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonLogin_Click(this, new EventArgs());
            }
        }
    }
}
