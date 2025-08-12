using System;
using System.Windows.Forms;

using PatientRegistrationApp.Models;

namespace PatientRegistrationApp.Forms
{
    public partial class ConfirmPatientForm : Form
    {
        private Patient _validatedPatient;

        public ConfirmPatientForm(Patient validatedPatient)
        {
            InitializeComponent();
            _validatedPatient = validatedPatient;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ConfirmPatientForm_Load(object sender, EventArgs e)
        {
            txtFirstName.Text = _validatedPatient.FirstName;
            txtLastName.Text = _validatedPatient.LastName;
            txtPESEL.Text = _validatedPatient.PESEL;
            txtPhone.Text = _validatedPatient.Phone;
            txtEmail.Text = _validatedPatient.Email;
            txtCity.Text = _validatedPatient.City;
            txtStreet.Text = _validatedPatient.Street;
            txtBuildingNumber.Text = _validatedPatient.BuildingNumber;
            txtApartmentNumber.Text = _validatedPatient.ApartmentNumber;
            txtPostalCode.Text = _validatedPatient.PostalCode;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
