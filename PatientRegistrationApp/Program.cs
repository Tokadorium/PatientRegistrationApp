using System;
using System.Windows.Forms;
using PatientRegistrationApp.Forms;

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

            using (var devForm = new DevelopmentForm())
            {
                var result = devForm.ShowDialog();
                if (result != DialogResult.OK) return;
            }    

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
