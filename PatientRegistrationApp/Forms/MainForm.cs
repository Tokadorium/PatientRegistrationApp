using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PatientRegistrationApp.DAL;
using PatientRegistrationApp.Models;

namespace PatientRegistrationApp.Forms
{
    public partial class MainForm : Form
    {
        private const int PageSize = 200;
        private const int ScrollTimerInterval = 200;

        private readonly User _loggedUser;
        private readonly PatientDAL _dal = new PatientDAL();
        private readonly HashSet<int> _loadedPages = new HashSet<int>();

        private BindingList<Patient> _currentPatientsPage;
        private bool _isFiltered = false;
        private DateTime _lastScrollTime = DateTime.MinValue;
        private SemaphoreSlim _semaphoreLoading = new SemaphoreSlim(1, 1);

        public MainForm(User loggedUser)
        {
            InitializeComponent();
            _loggedUser = loggedUser;


        }

        private void LoadPatients()
        {
            //_currentPatientsPage = new BindingList<Patient>(_dal.GetAllPatients());
            //dgvPatients.DataSource = _currentPatientsPage;
            _currentPatientsPage = new BindingList<Patient>(_dal.GetPatientsPage(0, PageSize));
            dgvPatients.DataSource = _currentPatientsPage;
            _loadedPages.Add(0);
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

        private bool IsPageVisible(int pageNumber, int pageSize = PageSize)
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
            var now = DateTime.UtcNow;
            if ((now - _lastScrollTime).TotalMilliseconds >= ScrollTimerInterval)
            {
                _lastScrollTime = now;
                HandleScroll();
            }
        }

        private async void LoadPage(int pageNumber, int pageSize = PageSize)
        {
            if (_loadedPages.Contains(pageNumber)) return;

            _semaphoreLoading.Wait();

            var nextPatients = await Task.Run(() =>
                new BindingList<Patient>(_dal.GetPatientsPage(pageNumber * pageSize, pageSize))
            );

            foreach (var patient in nextPatients)
            {
                _currentPatientsPage.Add(patient);
            }

            _loadedPages.Add(pageNumber);

            _semaphoreLoading.Release();
        }

        private async Task UnloadUnusedPages(int pageSize = PageSize)
        {
            _semaphoreLoading.Wait();

            foreach (var page in _loadedPages)
            {
                if (IsPageVisible(page)) continue;

                for (var i = page; i < page + pageSize; i++)
                {
                    _currentPatientsPage.RemoveAt(i);
                }

                _loadedPages.Remove(page);
            }

            _semaphoreLoading.Release();
        }

        private async void HandleScroll()
        {
            if (GetScrollPositionPercent() > 70)
            {
                LoadPage(GetCurrentPageNumber() + 1);   // load next page
                //UnloadUnusedPages();
            }
            if (GetScrollPositionPercent() < 30 && GetCurrentPageNumber() > 0)
            {
                LoadPage(GetCurrentPageNumber() - 1);   // load prev page
                //UnloadUnusedPages();
            }

#if DEBUG
            Console.WriteLine($"Scroll: {GetScrollPositionPercent()}");
            Console.WriteLine($"Page: {GetCurrentPageNumber()}");
            Console.WriteLine($"Loaded pages: {string.Join(", ", _loadedPages)}");
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
