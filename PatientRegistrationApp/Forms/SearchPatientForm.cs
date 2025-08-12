using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PatientRegistrationApp.DAL;
using PatientRegistrationApp.Models;

namespace PatientRegistrationApp.Forms
{
    public partial class SearchPatientForm : Form
    {
        private readonly DataGridView _dgvPatients;
        private readonly PatientDAL _patientDAL = new PatientDAL();

        public List<Patient> SearchResults { get; private set; } = null;

        public SearchPatientForm(DataGridView dgvPatients)
        {
            InitializeComponent();
            _dgvPatients = dgvPatients;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            List<Patient> patients = _patientDAL.SearchPatients(
                txtFirstName.Text,
                txtLastName.Text,
                txtPESEL.Text,
                txtPhone.Text,
                txtEmail.Text,
                txtStreet.Text,
                txtBuildingNumber.Text,
                txtApartmentNumber.Text,
                txtPostalCode.Text,
                txtCity.Text
            );

            _dgvPatients.DataSource = patients;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
