using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AFPI_Beejees_db
{
    public partial class qm : MetroFramework.Forms.MetroForm
    {
        public qm()
        {
            InitializeComponent();
        }

        private void qm_Load(object sender, EventArgs e)
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
    }
}
