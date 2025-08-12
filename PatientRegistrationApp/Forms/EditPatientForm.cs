using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PatientRegistrationApp.BLL;
using PatientRegistrationApp.DAL;
using PatientRegistrationApp.Models;

namespace PatientRegistrationApp.Forms
{
    public partial class EditPatientForm : Form
    {
        private User LoggedUser;
        private Patient Patient;
        private TextBox[] textBoxes;
        private Dictionary<string, TextBox> fieldMapping;
        private ErrorProvider errorProvider = new ErrorProvider();
        private DataGridView DataGridView;

        public EditPatientForm(User loggedUser, DataGridView dataGridView, Patient patient)
        {
            InitializeComponent();
            Patient = patient;
            LoggedUser = loggedUser;
            DataGridView = dataGridView;
            mapTextBoxes();
            PopulateFields();
        }

        private void mapTextBoxes()
        {
            textBoxes = new TextBox[] {
                txtFirstName, txtLastName, txtPESEL, txtPhone, txtEmail,
                txtStreet, txtBuildingNumber, txtApartmentNumber, txtPostalCode, txtCity
            };

            fieldMapping = new Dictionary<string, TextBox>
            {
                { "FirstName", txtFirstName },
                { "LastName", txtLastName },
                { "PESEL", txtPESEL },
                { "Phone", txtPhone },
                { "Email", txtEmail },
                { "Street", txtStreet },
                { "BuildingNumber", txtBuildingNumber },
                { "ApartmentNumber", txtApartmentNumber },
                { "PostalCode", txtPostalCode },
                { "City", txtCity }
            };
        }
        private void PopulateFields()
        {
            txtFirstName.Text = Patient.FirstName;
            txtLastName.Text = Patient.LastName;
            txtPESEL.Text = Patient.PESEL;
            txtPhone.Text = Patient.Phone;
            txtEmail.Text = Patient.Email;
            txtStreet.Text = Patient.Street;
            txtBuildingNumber.Text = Patient.BuildingNumber;
            txtApartmentNumber.Text = Patient.ApartmentNumber;
            txtPostalCode.Text = Patient.PostalCode;
            txtCity.Text = Patient.City;
        }
        private void highlightFieldsInError(Dictionary<string, string> generalMessages, Dictionary<string, string> detailedMessages)
        {
            bool IsInGroup1(string fieldKey)
            {
                return fieldKey == "FirstName" || fieldKey == "LastName" || fieldKey == "PESEL";
            }

            bool IsInGroup2(string fieldKey)
            {
                return fieldKey == "Phone" || fieldKey == "Email";
            }

            bool IsInGroup3(string fieldKey)
            {
                return fieldKey == "Street" || fieldKey == "BuildingNumber" ||
                       fieldKey == "ApartmentNumber" || fieldKey == "PostalCode" || fieldKey == "City";
            }

            errorProvider.Clear();

            foreach (var detailedMessage in detailedMessages)
            {
                string fieldKey = detailedMessage.Key;
                string errorMessage = detailedMessage.Value;

                if (string.IsNullOrEmpty(errorMessage))
                    continue;

                if (fieldMapping.ContainsKey(fieldKey))
                {
                    TextBox targetTextBox = fieldMapping[fieldKey];

                    bool shouldShowError = false;

                    if (IsInGroup1(fieldKey) && generalMessages.ContainsKey("group1"))
                        shouldShowError = true;
                    else if (IsInGroup2(fieldKey) && generalMessages.ContainsKey("group2"))
                        shouldShowError = true;
                    else if (IsInGroup3(fieldKey) && generalMessages.ContainsKey("group3"))
                        shouldShowError = true;

                    if (shouldShowError)
                    {
                        errorProvider.SetError(targetTextBox, errorMessage);
                    }
                }
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var updatedPatient = new Patient
            {
                Id = Patient.Id,
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
                PESEL = txtPESEL.Text,
                Phone = txtPhone.Text,
                Email = txtEmail.Text,
                Street = txtStreet.Text,
                BuildingNumber = txtBuildingNumber.Text,
                ApartmentNumber = txtApartmentNumber.Text,
                PostalCode = txtPostalCode.Text,
                City = txtCity.Text,
                MetaData = Patient.MetaData
            };

            // validate and normalize
            Patient parsedPatient = PatientValidator.TryParsePatient(updatedPatient, out PatientValidator.ParsingMessages parsingMessages);

            if (!PatientValidator.ValidatePatient(parsedPatient, out Patient validatedPatient, out PatientValidator.ValidationMessages validationMessages))
            {
                highlightFieldsInError(validationMessages.groupedErrors, parsingMessages.Errors);
                MessageBox.Show(string.Join(Environment.NewLine, validationMessages.groupedErrors.Values), "Validation Error");
                return;
            }

            var userDAL = new UserDAL();
            var patientDAL = new PatientDAL();

            if (userDAL.UserHasPermission(LoggedUser.Id, "User") && patientDAL.UpdatePatient(validatedPatient))
            {
                this.DialogResult = DialogResult.OK;
                if (DataGridView.CurrentRow != null)
                {
                    DataGridView.CurrentRow.Cells["FirstName"].Value = validatedPatient.FirstName;
                    DataGridView.CurrentRow.Cells["LastName"].Value = validatedPatient.LastName;
                    DataGridView.CurrentRow.Cells["PESEL"].Value = validatedPatient.PESEL;
                    DataGridView.CurrentRow.Cells["Phone"].Value = validatedPatient.Phone;
                    DataGridView.CurrentRow.Cells["Email"].Value = validatedPatient.Email;
                    DataGridView.CurrentRow.Cells["Street"].Value = validatedPatient.Street;
                    DataGridView.CurrentRow.Cells["BuildingNumber"].Value = validatedPatient.BuildingNumber;
                    DataGridView.CurrentRow.Cells["ApartmentNumber"].Value = validatedPatient.ApartmentNumber;
                    DataGridView.CurrentRow.Cells["PostalCode"].Value = validatedPatient.PostalCode;
                    DataGridView.CurrentRow.Cells["City"].Value = validatedPatient.City;
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to update patient.");
            }
        }
    }
}
