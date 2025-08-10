using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PatientRegistrationApp.Forms;
using PatientRegistrationApp.Tests;

namespace PatientRegistrationApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Tests.PatientSeeder.SeedPatients(3000);

            using (var loginForm = new LoginForm())
            {
                var result = loginForm.ShowDialog();
                if (result == DialogResult.OK && loginForm.LoggedUser != null)
                {
                    Application.Run(new MainForm(loginForm.LoggedUser));
                }
            }
        }
    }
}
