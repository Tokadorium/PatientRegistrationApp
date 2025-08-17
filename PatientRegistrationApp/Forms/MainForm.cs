using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PatientRegistrationApp.DAL;
using PatientRegistrationApp.Models;

namespace PatientRegistrationApp.Forms
{
    public partial class MainForm : Form
    {
        private const int PageSize = 500;
        private const int ScrollTimerInterval = 200;

        private readonly User _loggedUser;
        private readonly PatientDAL _dal = new PatientDAL();
        private readonly HashSet<int> _loadedPages = new HashSet<int>();

        private BindingList<Patient> _currentPatientsPage;
        private bool _isFiltered = false;
        private DateTime _lastScrollTime = DateTime.MinValue;
        private SemaphoreSlim _semaphoreLoading = new SemaphoreSlim(1, 1);
        private int _firstLoadedPage = 0;

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
            _firstLoadedPage = 0;
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

            return _firstLoadedPage + firstIndex / PageSize;
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

        private async Task LoadPageAsync(int pageNumber, int pageSize = PageSize)
        {
            if (_loadedPages.Contains(pageNumber)) return;

            var nextPatients = await Task.Run(() =>
                new BindingList<Patient>(_dal.GetPatientsPage(pageNumber * pageSize, pageSize))
            );

            await _semaphoreLoading.WaitAsync();
            try
            {
                if (_loadedPages.Contains(pageNumber)) return;

                if (pageNumber > _firstLoadedPage) // append at bottom
                {
                    foreach (var p in nextPatients)
                        _currentPatientsPage.Add(p);
                }
                else // prepend at top
                {
                    for (int i = nextPatients.Count - 1; i >= 0; i--)
                        _currentPatientsPage.Insert(0, nextPatients[i]);

                    // keep the loaded range correct
                    _firstLoadedPage = pageNumber;
                }

                _loadedPages.Add(pageNumber);
            }
            finally
            {
                _semaphoreLoading.Release();
            }
        }

        private List<int> GetUnusedPagesList()
        {
            List<int> unusedPages = new List<int>();

            foreach (var page in _loadedPages)
            {
                if (!IsPageVisible(page))
                {
                    unusedPages.Add(page);
                }
            }

            return unusedPages;
        }

        private async Task UnloadPrevPagesAsync(int pageSize = PageSize)
        {
            await _semaphoreLoading.WaitAsync();
            try
            {
                int currentPage = GetCurrentPageNumber();

                var toUnload = GetUnusedPagesList()
                    .Where(p => p < currentPage)
                    .OrderByDescending(p => p) // avoid index shifts
                    .ToList();

#if DEBUG
                if (toUnload.Count > 0)
                    Console.WriteLine($"Unloading pages: {string.Join(", ", toUnload)}");
#endif

                foreach (var page in toUnload)
                {
                    int startIndex = page * pageSize;
                    if (startIndex < 0 || startIndex >= _currentPatientsPage.Count)
                        continue;

                    int countToRemove = Math.Min(pageSize, _currentPatientsPage.Count - startIndex);
                    for (int i = 0; i < countToRemove; i++)
                        _currentPatientsPage.RemoveAt(startIndex);

                    _loadedPages.Remove(page);
                }

                if (_loadedPages.Count > 0)
                    _firstLoadedPage = _loadedPages.Min();
            }
            finally
            {
                _semaphoreLoading.Release();
            }
        }

        private async Task UnloadNextPagesAsync(int pageSize = PageSize)
        {
            await _semaphoreLoading.WaitAsync();
            try
            {
                int currentPage = GetCurrentPageNumber();

                var toUnload = GetUnusedPagesList()
                    .Where(p => p > currentPage)
                    .OrderByDescending(p => p)
                    .ToList();

#if DEBUG
                if (toUnload.Count > 0)
                    Console.WriteLine($"Unloading pages: {string.Join(", ", toUnload)}");
#endif

                foreach (var page in toUnload)
                {
                    int startIndex = page * PageSize;
                    if (startIndex < 0 || startIndex >= _currentPatientsPage.Count)
                        continue;

                    int countToRemove = Math.Min(PageSize, _currentPatientsPage.Count - startIndex);
                    for (int i = 0; i < countToRemove; i++)
                        _currentPatientsPage.RemoveAt(startIndex);

                    _loadedPages.Remove(page);
                }
            }
            finally
            {
                _semaphoreLoading.Release();
            }
        }

        private async void HandleScroll()
        {
            // skip if another scroll operation is in progress
            if (!_semaphoreLoading.Wait(0))
                return;
            _semaphoreLoading.Release();

            if (GetScrollPositionPercent() > 70)
            {
                await LoadPageAsync(GetCurrentPageNumber() + 1);
                //await UnloadPrevPagesAsync();
            }
            else if (GetScrollPositionPercent() < 30 && GetCurrentPageNumber() > 0)
            {
                await LoadPageAsync(GetCurrentPageNumber() - 1);
                //await UnloadNextPagesAsync();
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
