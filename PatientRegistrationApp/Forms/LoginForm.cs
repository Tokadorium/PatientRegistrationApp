using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PatientRegistrationApp.Models;
using PatientRegistrationApp.BLL;
using PatientRegistrationApp.DAL;

namespace PatientRegistrationApp.Forms
{
    public partial class LoginForm : Form
    {
        private const int MAX_FAILED = 5;
        private const int SHOW_RESET_LINK_AFTER = 2;

        private int failedAttempts = 0;
        private DateTime lockoutUntil = DateTime.MinValue;

        public User LoggedUser { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void lblPassword_Click(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                lnkForgotPassword.Visible = false;

                // no spaces in username and password
                txtUsername.Text = txtUsername.Text.Trim();
                txtPassword.Text = txtPassword.Text.Trim();

                string username = txtUsername.Text;
                string password = txtPassword.Text;

                if (DateTime.UtcNow < lockoutUntil)
                {
                    MessageBox.Show($"Too many failed attempts. Try again after {lockoutUntil.ToLocalTime():T}.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Invalid credentials.");
                    return;
                }

                var user = AuthService.TryLogin(username, password);

                if (user == null)
                {
                    failedAttempts++;

                    if (failedAttempts >= MAX_FAILED)
                    {
                        // TODO THIS IS IMPORTANT!!!
                        // god i just realized that this is a terrible way to handle lockouts
                        // i definately WILL remember to handle it via database later
                        lockoutUntil = DateTime.UtcNow.AddMinutes(1);
                        failedAttempts = 0;
                    }

                    MessageBox.Show("Invalid credentials.");

                    if (failedAttempts >= SHOW_RESET_LINK_AFTER)
                        lnkForgotPassword.Visible = true;

                    return;
                }

                LoggedUser = user;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error occurred.");

                // potentially log the exception to a file later
            }
            finally
            {
                lnkForgotPassword.Visible = false;

                txtUsername.Clear();
                txtPassword.Clear();
                txtUsername.Focus();
            }
        }
    }
}
