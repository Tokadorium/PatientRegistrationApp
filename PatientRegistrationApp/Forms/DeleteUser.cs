using System;
using System.Linq;
using System.Windows.Forms;
using PatientRegistrationApp.Models;
using PatientRegistrationApp.DAL;
using System.ComponentModel;


namespace PatientRegistrationApp.Forms
{
    public partial class DeleteUser : Form
    {
        private readonly Patient Patient;
        private readonly User LoggedUser;
        private readonly DataGridView DataGridView;


        public DeleteUser(Patient patient, DataGridView dataGridView, User loggedUser)
        {
            InitializeComponent();
            Patient = patient;
            LoggedUser = loggedUser;
            DataGridView = dataGridView;
            PopulateFields();
        }

        private void PopulateFields()
        {
            txtFirstName.Text = Patient.FirstName;
            txtLastName.Text = Patient.LastName;
            txtPESEL.Text = Patient.PESEL;
            txtPhone.Text = Patient.Phone;
            txtEmail.Text = Patient.Email;
            txtStreet.Text = Patient.Street;
            txtBuildingNumber.Text = Patient.BuildingNumber;
            txtApartmentNumber.Text = Patient.ApartmentNumber;
            txtPostalCode.Text = Patient.PostalCode;
            txtCity.Text = Patient.City;
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            var userDAL = new UserDAL();
            var patientDAL = new PatientDAL();

            var confirm = MessageBox.Show(
                "Are you sure you want to delete this patient? This action cannot be undone.",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            if (userDAL.UserHasPermission(LoggedUser.Id, "User") && patientDAL.DeletePatient(Patient.Id))
            {
                this.DialogResult = DialogResult.OK;
                
                if (DataGridView.DataSource is BindingList<Patient> patientList)
                {
                    var patientToRemove = patientList.FirstOrDefault(p => p.Id == Patient.Id);
                    if (patientToRemove != null)
                    {
                        patientList.Remove(patientToRemove);
                    }
                }
                
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to delete patient.", "Error");
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
