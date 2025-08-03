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

namespace PatientRegistrationApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnTestAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var dal = new PatientDAL();
                var patient = new Patient
                {
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    PESEL = "12345678902",
                    Phone = "123456789",
                    Email = "jan@test.pl"
                };

                bool result = dal.AddPatient(patient);
                MessageBox.Show(result ? "Patient added successfully." : "Failed to add patient.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void btnTestGet_Click(object sender, EventArgs e)
        {
            try
            {
                var dal = new PatientDAL();
                var patients = dal.GetAllPatients();

                string message = $"Found {patients.Count} patients:\n";
                foreach (var p in patients.Take(3))
                {
                    message += $"ID: {p.Id}, {p.FirstName} {p.LastName}, PESEL: {p.PESEL}\n";
                }

                MessageBox.Show(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured: {ex.Message}");
            }
        }

        private void btnTestUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtPatientId.Text, out int id))
                {
                    MessageBox.Show("Wpisz prawidłowe ID pacjenta!");
                    return;
                }

                var dal = new PatientDAL();
                var patient = dal.GetPatientById(id);

                if (patient == null)
                {
                    MessageBox.Show($"Nie znaleziono pacjenta o ID: {id}");
                    return;
                }

                // Zmień dane
                patient.FirstName = "ZAKTUALIZOWANY";
                patient.LastName = "PACJENT";

                bool success = dal.UpdatePatient(patient);
                MessageBox.Show($"Aktualizacja: {(success ? "Sukces!" : "Błąd!")}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}");
            }
        }

        private void btnTestDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtPatientId.Text, out int id))
                {
                    MessageBox.Show("Wpisz prawidłowe ID pacjenta!");
                    return;
                }

                var dal = new PatientDAL();

                // Sprawdź czy pacjent istnieje
                var patient = dal.GetPatientById(id);
                if (patient == null)
                {
                    MessageBox.Show($"Nie znaleziono pacjenta o ID: {id}");
                    return;
                }

                // Potwierdź usunięcie
                var result = MessageBox.Show(
                    $"Czy na pewno usunąć pacjenta: {patient.FirstName} {patient.LastName}?",
                    "Potwierdzenie",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    bool success = dal.DeletePatient(id);
                    MessageBox.Show($"Usuwanie: {(success ? "Sukces!" : "Błąd!")}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}");
            }
        }
    }
}
