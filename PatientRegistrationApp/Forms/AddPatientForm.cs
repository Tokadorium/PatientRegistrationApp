using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PatientRegistrationApp.BLL;
using PatientRegistrationApp.DAL;
using PatientRegistrationApp.Models;

namespace PatientRegistrationApp.Forms
{
    public partial class AddPatientForm : Form
    {
        public AddPatientForm(User loggedUser)
        {
            InitializeComponent();

            mapTextBoxes();

            LoggedUser = loggedUser;
        }
        private User LoggedUser { get; set; }
        private TextBox[] textBoxes;
        private Dictionary<string, TextBox> fieldMapping;
        private ErrorProvider errorProvider = new ErrorProvider();

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
        private void btnAdd_Click(object sender, EventArgs e)
        {
            var patient = new Patient
            {
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
                MetaData = null
            };

            // validate and normalize
            Patient parsedPatient = PatientValidator.TryParsePatient(patient, out PatientValidator.ParsingMessages parsingMessages);

            // if patient is not valid show a box containing general errors and mark the form fields
            if (!PatientValidator.ValidatePatient(parsedPatient, out Patient validatedPatient, out PatientValidator.ValidationMessages validationMessages))
            {
                highlightFieldsInError(validationMessages.groupedErrors, parsingMessages.Errors);
                MessageBox.Show(string.Join(Environment.NewLine, validationMessages.groupedErrors.Values), "Validation Error");
                return;
            }


            using (var confirmForm = new ConfirmPatientForm(validatedPatient))
            {
                if (confirmForm.ShowDialog() != DialogResult.OK)
                {
                    // user cancelled
                    return;
                }
            }

            // try to add to DB
            var patientDAL = new PatientDAL();
            var userDAL = new UserDAL();

            if (patientDAL.GetPatientByPESEL(validatedPatient.PESEL) != null)
            {
                MessageBox.Show("Patient with this PESEL already exists.");
                return;
            }

            // do not add if the user does not have permission
            if (userDAL.UserHasPermission(LoggedUser.Id, "User") && patientDAL.AddPatient(validatedPatient))
            {
                MessageBox.Show("Patient added successfully.");
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to add patient.");
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
