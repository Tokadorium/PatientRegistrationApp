using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PatientRegistrationApp.DAL;
using PatientRegistrationApp.Models;

namespace PatientRegistrationApp.Forms
{
    public partial class SearchPatientForm : Form
    {
        private DataGridView dgvPatients;

        PatientDAL patientDAL = new PatientDAL();

        public SearchPatientForm(DataGridView dgvPatients)
        {
            InitializeComponent();
            this.dgvPatients = dgvPatients;
        }

        public List<Patient> SearchResults { get; private set; } = null;

        private void btnSearch_Click(object sender, EventArgs e)
        {
            List<Patient> patients = patientDAL.SearchPatients(
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

            dgvPatients.DataSource = patients;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
