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
using PatientRegistrationApp.DAL;

namespace PatientRegistrationApp.Forms
{
    public partial class MainForm : Form
    {
        private User LoggedUser { get; set; }
        private int CurrentOffset = 0;
        private const int PageSize = 200;
        private BindingList<Patient> CurrentPatientsPage;
        private HashSet<int> LoadedPages = new HashSet<int>();

        public MainForm(User loggedUser)
        {
            InitializeComponent();
            LoggedUser = loggedUser;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = $"Welcome, {LoggedUser.FirstName}!";
            
            var patientDAL = new PatientDAL();
            CurrentPatientsPage = new BindingList<Patient>(patientDAL.GetPatientsPage(CurrentOffset, PageSize));
            dgvPatients.DataSource = CurrentPatientsPage;
        }
        private void dgvPatients_Scroll(object sender, ScrollEventArgs e)
        {

        }
        private void refreshViewToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void addPatientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddPatientForm addPatientForm = new AddPatientForm(LoggedUser);
            addPatientForm.Show();
        }
        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
    }
}
