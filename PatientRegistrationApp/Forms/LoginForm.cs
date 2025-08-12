using System;
using System.Windows.Forms;
using PatientRegistrationApp.BLL;
using PatientRegistrationApp.Models;

namespace PatientRegistrationApp.Forms
{
    public partial class LoginForm : Form
    {
        public User LoggedUser { get; private set; }
        private int _timesTried = 0;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
        
        private void lblPassword_Click(object sender, EventArgs e)
        {
            // logic to reset a password
        }
        
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                lnkForgotPassword.Visible = false;

                string username = txtUsername.Text;
                string password = txtPassword.Text;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Username and password cannot be empty.");
                    return;
                }

                LoggedUser = AuthService.TryLogin(username, password, out string loginErrorMessage);

                if (LoggedUser == null)
                {
                    _timesTried++;
                    if (_timesTried >= 3) lnkForgotPassword.Visible = true;
                    MessageBox.Show(loginErrorMessage);
                    return;
                }

                // successful login
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch
            {
                MessageBox.Show("Unexpected error occurred.");
            }
            finally
            {
                txtUsername.Clear();
                txtPassword.Clear();
                txtUsername.Focus();
            }
        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
