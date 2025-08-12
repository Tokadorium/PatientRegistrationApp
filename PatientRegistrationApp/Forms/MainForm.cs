using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using PatientRegistrationApp.DAL;
using PatientRegistrationApp.Models;

namespace PatientRegistrationApp.Forms
{
    public partial class MainForm : Form
    {
        private const int PageSize = 200;

        private readonly User _loggedUser;
        private readonly PatientDAL _dal = new PatientDAL();
        private readonly HashSet<int> _loadedPages = new HashSet<int>();

        private BindingList<Patient> _currentPatientsPage;
        private bool _isFiltered = false;

        public MainForm(User loggedUser)
        {
            InitializeComponent();
            _loggedUser = loggedUser;
        }

        private void LoadPatients()
        {
            _currentPatientsPage = new BindingList<Patient>(_dal.GetAllPatients());
            dgvPatients.DataSource = _currentPatientsPage;
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
                return false;

            int firstItemIndex = dgvPatients.FirstDisplayedScrollingRowIndex;
            int lastItemIndex = firstItemIndex + dgvPatients.DisplayedRowCount(true) - 1;

            int pageStartIndex = pageNumber * pageSize;
            int pageEndIndex = pageStartIndex + pageSize - 1;

            return !(pageEndIndex < firstItemIndex || pageStartIndex > lastItemIndex);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = $"Welcome, {_loggedUser.FirstName}!";

            LoadPatients();
            clearSearchResultsToolStripMenuItem.Enabled = false;
        }

        private void dgvPatients_Scroll(object sender, ScrollEventArgs e)
        {
#if DEBUG
            Console.WriteLine($"Scroll: {GetScrollPositionPercent()}");
            Console.WriteLine($"Page: {GetCurrentPageNumber()}");
            Console.WriteLine($"Loaded pages: {_loadedPages}");
#endif
        }

        private void addPatientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddPatientForm addPatientForm = new AddPatientForm(_loggedUser);
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

                var allPatients = _dal.GetAllPatients();
                if (dgvPatients.Rows.Count < allPatients.Count)
                {
                    _isFiltered = true;
                    clearSearchResultsToolStripMenuItem.Enabled = true;
                }
                else
                {
                    _isFiltered = false;
                    clearSearchResultsToolStripMenuItem.Enabled = false;
                }
            }
        }

        private void editPatientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvPatients.CurrentRow != null && dgvPatients.CurrentRow.DataBoundItem is Patient selectedPatient)
            {
                using (var editForm = new EditPatientForm(_loggedUser, dgvPatients, selectedPatient))
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
            _currentPatientsPage = new BindingList<Patient>(_dal.GetAllPatients());
            dgvPatients.DataSource = _currentPatientsPage;
            _isFiltered = false;
            clearSearchResultsToolStripMenuItem.Enabled = false;
        }

        private void deletePatientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvPatients.CurrentRow != null && dgvPatients.CurrentRow.DataBoundItem is Patient selectedPatient)
            {
                using (var deleteForm = new DeletePatientForm(selectedPatient, _loggedUser))
                {
                    if (deleteForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadPatients();
                        MessageBox.Show("Patient deleted successfully.", "Success");
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete patient.", "Error");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a patient to delete.", "Error");
            }
        }
    }
}
