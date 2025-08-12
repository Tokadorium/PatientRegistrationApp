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
        private readonly User _loggedUser;
        private readonly Patient _patient;
        private readonly DataGridView _dataGridView;
        
        private readonly TextBox[] _textBoxes;
        private readonly Dictionary<string, TextBox> _fieldMapping;
        private readonly ErrorProvider _errorProvider = new ErrorProvider();

        public EditPatientForm(User loggedUser, DataGridView dataGridView, Patient patient)
        {
            InitializeComponent();
            _patient = patient;
            _loggedUser = loggedUser;
            _dataGridView = dataGridView;
            
            // Initialize readonly fields in constructor
            _textBoxes = new TextBox[] {
                txtFirstName, txtLastName, txtPESEL, txtPhone, txtEmail,
                txtStreet, txtBuildingNumber, txtApartmentNumber, txtPostalCode, txtCity
            };

            _fieldMapping = new Dictionary<string, TextBox>
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
            
            PopulateFields();
        }

        private void PopulateFields()
        {
            txtFirstName.Text = _patient.FirstName;
            txtLastName.Text = _patient.LastName;
            txtPESEL.Text = _patient.PESEL;
            txtPhone.Text = _patient.Phone;
            txtEmail.Text = _patient.Email;
            txtStreet.Text = _patient.Street;
            txtBuildingNumber.Text = _patient.BuildingNumber;
            txtApartmentNumber.Text = _patient.ApartmentNumber;
            txtPostalCode.Text = _patient.PostalCode;
            txtCity.Text = _patient.City;
        }

        private void HighlightFieldsInError(Dictionary<string, string> generalMessages, Dictionary<string, string> detailedMessages)
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

            _errorProvider.Clear();

            foreach (var detailedMessage in detailedMessages)
            {
                string fieldKey = detailedMessage.Key;
                string errorMessage = detailedMessage.Value;

                if (string.IsNullOrEmpty(errorMessage))
                {
                    continue;
                }

                if (_fieldMapping.ContainsKey(fieldKey))
                {
                    TextBox targetTextBox = _fieldMapping[fieldKey];
                    bool shouldShowError = false;

                    if (IsInGroup1(fieldKey) && generalMessages.ContainsKey("group1"))
                    {
                        shouldShowError = true;
                    }
                    else if (IsInGroup2(fieldKey) && generalMessages.ContainsKey("group2"))
                    {
                        shouldShowError = true;
                    }
                    else if (IsInGroup3(fieldKey) && generalMessages.ContainsKey("group3"))
                    {
                        shouldShowError = true;
                    }

                    if (shouldShowError)
                    {
                        _errorProvider.SetError(targetTextBox, errorMessage);
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
                Id = _patient.Id,
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
                MetaData = _patient.MetaData
            };

            // validate and normalize
            Patient parsedPatient = PatientValidator.TryParsePatient(updatedPatient, out PatientValidator.ParsingMessages parsingMessages);

            if (!PatientValidator.ValidatePatient(parsedPatient, out Patient validatedPatient, out PatientValidator.ValidationMessages validationMessages))
            {
                HighlightFieldsInError(validationMessages.GroupedErrors, parsingMessages.Errors);
                MessageBox.Show(string.Join(Environment.NewLine, validationMessages.GroupedErrors.Values), "Validation Error");
                return;
            }

            var userDAL = new UserDAL();
            var patientDAL = new PatientDAL();

            if (userDAL.UserHasPermission(_loggedUser.Id, "User") && patientDAL.UpdatePatient(validatedPatient))
            {
                this.DialogResult = DialogResult.OK;

                if (_dataGridView.CurrentRow != null)
                {
                    _dataGridView.CurrentRow.Cells["FirstName"].Value = validatedPatient.FirstName;
                    _dataGridView.CurrentRow.Cells["LastName"].Value = validatedPatient.LastName;
                    _dataGridView.CurrentRow.Cells["PESEL"].Value = validatedPatient.PESEL;
                    _dataGridView.CurrentRow.Cells["Phone"].Value = validatedPatient.Phone;
                    _dataGridView.CurrentRow.Cells["Email"].Value = validatedPatient.Email;
                    _dataGridView.CurrentRow.Cells["Street"].Value = validatedPatient.Street;
                    _dataGridView.CurrentRow.Cells["BuildingNumber"].Value = validatedPatient.BuildingNumber;
                    _dataGridView.CurrentRow.Cells["ApartmentNumber"].Value = validatedPatient.ApartmentNumber;
                    _dataGridView.CurrentRow.Cells["PostalCode"].Value = validatedPatient.PostalCode;
                    _dataGridView.CurrentRow.Cells["City"].Value = validatedPatient.City;
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
