using System;
using System.Windows.Forms;
using PatientRegistrationApp.DAL;
using PatientRegistrationApp.Models;


namespace PatientRegistrationApp.Forms
{
    public partial class DeletePatientForm : Form
    {
        private readonly Patient _patient;
        private readonly User _loggedUser;

        public DeletePatientForm(Patient patient, User loggedUser)
        {
            InitializeComponent();
            _patient = patient;
            _loggedUser = loggedUser;
            PopulateFields();
        }

        private void PopulateFields()
        {
            txtFirstName.Text = _patient.FirstName;
            txtLastName.Text = _patient.LastName;
            txtPESEL.Text = _patient.PESEL;
            txtPhone.Text = _patient.Phone;
            txtEmail.Text = _patient.Email;
            txtStreet.Text = _patient.Street;
            txtBuildingNumber.Text = _patient.BuildingNumber;
            txtApartmentNumber.Text = _patient.ApartmentNumber;
            txtPostalCode.Text = _patient.PostalCode;
            txtCity.Text = _patient.City;
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

            if (!userDAL.UserHasPermission(_loggedUser.Id, "Manager"))
            {
                MessageBox.Show("Insufficient permission level. You need Manager role for that.");
                this.DialogResult = DialogResult.No;
                return;
            }

            if (patientDAL.DeletePatient(_patient.Id))
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.No;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
