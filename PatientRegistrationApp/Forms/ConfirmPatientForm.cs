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

namespace PatientRegistrationApp.Forms
{
    public partial class ConfirmPatientForm : Form
    {
        Patient ValidatedPatient;
        public ConfirmPatientForm(Patient validatedPatient)
        {
            InitializeComponent();
            ValidatedPatient = validatedPatient;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ConfirmPatientForm_Load(object sender, EventArgs e)
        {
            txtFirstName.Text = ValidatedPatient.FirstName;
            txtLastName.Text = ValidatedPatient.LastName;
            txtPESEL.Text = ValidatedPatient.PESEL;
            txtPhone.Text = ValidatedPatient.Phone;
            txtEmail.Text = ValidatedPatient.Email;
            txtCity.Text = ValidatedPatient.City;
            txtStreet.Text = ValidatedPatient.Street;
            txtBuildingNumber.Text = ValidatedPatient.BuildingNumber;
            txtApartmentNumber.Text = ValidatedPatient.ApartmentNumber;
            txtPostalCode.Text = ValidatedPatient.PostalCode;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
