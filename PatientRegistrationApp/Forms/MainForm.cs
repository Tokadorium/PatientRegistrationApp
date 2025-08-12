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
        PatientDAL Dal = new PatientDAL();
        private const int PageSize = 200;
        private BindingList<Patient> CurrentPatientsPage;
        private HashSet<int> LoadedPages = new HashSet<int>();

        private bool IsFiltered = false;

        public MainForm(User loggedUser)
        {
            InitializeComponent();
            LoggedUser = loggedUser;
        }

        private double GetScrollPositionPercent()
        {
            int firstItemIndex = dgvPatients.FirstDisplayedScrollingRowIndex;
            int totalRowCount = dgvPatients.RowCount;

            return (double)firstItemIndex / totalRowCount * 100;
        }
        private int GetCurrentPageNumber()
        {
            int firstIndex = dgvPatients.FirstDisplayedScrollingRowIndex;
            if (firstIndex < 0)
                return 0;

            return firstIndex / PageSize;
        }
        private bool IsPageVisible(int pageNumber, int pageSize)
        {
            if (dgvPatients.FirstDisplayedScrollingRowIndex < 0)
                return false; // nothing is displayed yet

            int firstItemIndex = dgvPatients.FirstDisplayedScrollingRowIndex;
            int lastItemIndex = firstItemIndex + dgvPatients.DisplayedRowCount(true) - 1;

            int pageStartIndex = pageNumber * pageSize;
            int pageEndIndex = pageStartIndex + pageSize - 1;

            return !(pageEndIndex < firstItemIndex || pageStartIndex > lastItemIndex);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = $"Welcome, {LoggedUser.FirstName}!";

            CurrentPatientsPage = new BindingList<Patient>(Dal.GetAllPatients());
            dgvPatients.DataSource = CurrentPatientsPage;
            clearSearchResultsToolStripMenuItem.Enabled = false;
        }
        private void dgvPatients_Scroll(object sender, ScrollEventArgs e)
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
        private void searchPatientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SearchPatientForm searchPatientForm = new SearchPatientForm(dgvPatients))
            {
                searchPatientForm.ShowDialog();

                var allPatients = Dal.GetAllPatients();
                if (dgvPatients.Rows.Count < allPatients.Count)
                {
                    IsFiltered = true;
                    clearSearchResultsToolStripMenuItem.Enabled = true;
                }
                else
                {
                    IsFiltered = false;
                    clearSearchResultsToolStripMenuItem.Enabled = false;
                }
            }
        }
        private void editPatientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvPatients.CurrentRow != null && dgvPatients.CurrentRow.DataBoundItem is Patient selectedPatient)
            {
                using (var editForm = new EditPatientForm(LoggedUser, dgvPatients, selectedPatient))
                {
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show("Patient updated successfully.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a patient to edit.");
            }
        }
        private void clearSearchResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentPatientsPage = new BindingList<Patient>(Dal.GetAllPatients());
            dgvPatients.DataSource = CurrentPatientsPage;
            IsFiltered = false;
            clearSearchResultsToolStripMenuItem.Enabled = false;
        }
        private void deletePatientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvPatients.CurrentRow != null && dgvPatients.CurrentRow.DataBoundItem is Patient selectedPatient)
            {
                using (var deleteForm = new DeleteUser(selectedPatient, dgvPatients, LoggedUser))
                {
                    if (deleteForm.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show("Patient deleted successfully.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a patient to delete.");
            }
        }
    }
}
