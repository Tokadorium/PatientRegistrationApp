using System;
using System.Windows.Forms;
using PatientRegistrationApp.BLL;
using PatientRegistrationApp.DAL;
using PatientRegistrationApp.Models;

namespace PatientRegistrationApp.Forms
{
    public partial class AddUserForm : Form
    {
        public AddUserForm()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            var dal = new UserDAL();
            if (dal.GetByUsername(username) != null)
            {
                MessageBox.Show("Username already exists.");
                return;
            }

            var user = new User
            {
                Username = username,
                PasswordHash = PasswordHasher.Hash(password),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                FailedAttempts = 0,
                LockedUntil = null,
                MetaData = null
            };

            bool success = dal.CreateUser(user);
            if (success)
            {
                MessageBox.Show("User added successfully.");
            }
            else
            {
                MessageBox.Show("Failed to add user.");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}