using System;
using System.Windows.Forms;

namespace AFPI_Beejees_db
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AFPI_Beejees_db.LoginForm());
        }
    }
}
