using System;
using System.Windows.Forms;
using PatientRegistrationApp.Tests;

namespace PatientRegistrationApp.Forms
{
    public partial class DevelopmentForm : Form
    {
        public DevelopmentForm()
        {
            InitializeComponent();
        }

        private void btnGenerateUsers_Click(object sender, EventArgs e)
        {
            using (var countForm = new CountInputForm())
            {
                if (countForm.ShowDialog() == DialogResult.OK && countForm.Count.HasValue)
                {
                    PatientSeeder.SeedPatients(countForm.Count.Value);
                    MessageBox.Show($"{countForm.Count.Value} patients generated.");
                }
            }
        }

        private void btnGeneratePatients_Click(object sender, EventArgs e)
        {
            UserSeeder.SeedDefaultUsers();
            MessageBox.Show("Users generated.");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }
    }
}
