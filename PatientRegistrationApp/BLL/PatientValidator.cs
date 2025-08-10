using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Globalization;
using PatientRegistrationApp.Models;
using System.Net.Http.Headers;

namespace PatientRegistrationApp.BLL
{
    public static class PatientValidator
    {
        public class ParsingMessages
        {
            public Dictionary<string, string> Errors { get; } = new Dictionary<string, string>();

            public bool IsValid => Errors.Count == 0;

            public void AddError(string field, string errorMessage)
            {
                if (!Errors.ContainsKey(field))
                    Errors[field] = errorMessage;
            }
        }
        public class ValidationMessages
        {
            public Dictionary<string, string> groupedErrors { get; } = new Dictionary<string, string>();

            public bool IsValid => groupedErrors.Count == 0;

            public void AddError(string field, string errorMessage)
            {
                if (!groupedErrors.ContainsKey(field))
                    groupedErrors[field] = errorMessage;
            }
        }

        private static string NormalizeName(string name)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.Trim().ToLower());
        }
        private static bool TryParsePesel(string pesel, out string normalized, out string errorMessage)
        {
            errorMessage = null;
            normalized = null;

            if (string.IsNullOrWhiteSpace(pesel))
            {
                errorMessage = "PESEL field is empty";
                return false;
            }

            pesel = pesel.Trim();

            if (pesel.Length != 11 || !pesel.All(char.IsDigit))
            {
                errorMessage = "PESEL must consist of exactly 11 digits";
                return false;
            }

            int year = int.Parse(pesel.Substring(0, 2));
            int month = int.Parse(pesel.Substring(2, 2));
            int day = int.Parse(pesel.Substring(4, 2));

            int century;
            if (month >= 81 && month <= 92)
            {
                century = 1800;
                month -= 80;
            }
            else if (month >= 1 && month <= 12)
            {
                century = 1900;
            }
            else if (month >= 21 && month <= 32)
            {
                century = 2000;
                month -= 20;
            }
            else if (month >= 41 && month <= 52)
            {
                century = 2100;
                month -= 40;
            }
            else if (month >= 61 && month <= 72)
            {
                century = 2200;
                month -= 60;
            }
            else
            {
                errorMessage = "Incorrect month in PESEL. Make sure PESEL starts with YYMMDD, where MM is in range 01 - 92";
                return false;
            }

            try
            {
                DateTime birthDate = new DateTime(century + year, month, day);
            }
            catch
            {
                errorMessage = "Incorrect date of birth in PESEL. Make sure PESEL starts with: YYMMDD";
                return false;
            }

            int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
            int sum = 0;

            for (int i = 0; i < 10; i++)
            {
                sum += (pesel[i] - '0') * weights[i];
            }

            int controlDigit = (10 - (sum % 10)) % 10;

            if (controlDigit != (pesel[10] - '0'))
            {
                errorMessage = "PESEL control sum not correct";
                return false;
            }

            normalized = pesel;
            return true;
        }
        private static bool TryParsePhone(string phone, out string normalized, out string errorMessage)
        {
            errorMessage = null;
            normalized = null;

            if (string.IsNullOrWhiteSpace(phone))
            {
                errorMessage = "Phone field is empty";
                return false;
            }

            // keep only digits
            string cleaned = Regex.Replace(phone, @"[^\d]", "");

            // accept optional 048 or 48 prefix, capture the 9 digits after
            var m = Regex.Match(cleaned, @"^(?:48|048)?(\d{9})$");
            if (!m.Success)
            {
                errorMessage = "Number must be a valid polish number. Make sure the number has exactly 9 digits";
                return false;
            }

            string subscriber = m.Groups[1].Value;
            normalized = "048" + subscriber;

            return true;
        }
        private static bool TryParseEmail(string email, out string normalized, out string errorMessage)
        {
            errorMessage = null;
            normalized = null;

            if (string.IsNullOrWhiteSpace(email))
            {
                errorMessage = "Email field is empty";
                return false;
            }

            email = email.Trim();

            try
            {
                MailAddress m = new MailAddress(email);
                normalized = m.Address;
            }
            catch (FormatException)
            {
                errorMessage = "Email address format not correct";
                return false;
            }

            return true;
        }
        private static bool TryParsePostalCode(string postalCode, out string normalized, out string errorMessage)
        {
            errorMessage = null;
            normalized = null;

            if (string.IsNullOrWhiteSpace(postalCode))
            {
                errorMessage = "Postal code field is empty";
                return false;
            }

            string cleaned = postalCode.Trim();

            // Polish codes only
            var match = Regex.Match(cleaned, @"^(\d{2})-?(\d{3})$");
            if (!match.Success)
            {
                errorMessage = "Postal code must be in format XX-XXX";
                return false;
            }

            normalized = $"{match.Groups[1].Value}-{match.Groups[2].Value}";
            return true;
        }
        public static Patient TryParsePatient(Patient patient, out ParsingMessages errorMessages)
        {
            var result = new ParsingMessages();
            Patient normalized = new Patient();

            // name
            if (string.IsNullOrWhiteSpace(patient.FirstName))
                result.AddError("FirstName", "Name field is empty");
            else
                normalized.FirstName = NormalizeName(patient.FirstName);

            // last name
            if (string.IsNullOrWhiteSpace(patient.LastName))
                result.AddError("LastName", "Last name field is empty");
            else
                normalized.LastName = NormalizeName(patient.LastName);

            // PESEL
            if (!TryParsePesel(patient.PESEL, out string normalizedPesel, out string peselError))
                result.AddError("PESEL", peselError);
            else
                normalized.PESEL = normalizedPesel;

            // phone
            if (!TryParsePhone(patient.Phone, out string normalizedPhone, out string phoneError))
                result.AddError("Phone", phoneError);
            else
                normalized.Phone = normalizedPhone;

            // email
            if (!TryParseEmail(patient.Email, out string normalizedEmail, out string emailError))
                result.AddError("Email", emailError);
            else
                normalized.Email = normalizedEmail;

            // street
            if (string.IsNullOrWhiteSpace(patient.Street))
                result.AddError("Street", "Street field is empty");
            else
                normalized.Street = NormalizeName(patient.Street);

            // building number
            if (string.IsNullOrWhiteSpace(patient.BuildingNumber))
                result.AddError("BuildingNumber", "Building field is empty");
            else
                normalized.BuildingNumber = patient.BuildingNumber.Trim();

            // apartment number
            if (string.IsNullOrWhiteSpace(patient.ApartmentNumber))
                result.AddError("ApartmentNumber", "Apartment field is empty");
            else
                normalized.ApartmentNumber = patient.ApartmentNumber.Trim();

            // postal code
            if (!TryParsePostalCode(patient.PostalCode, out string normalizedPostalCode, out string postalCodeError))
                result.AddError("PostalCode", postalCodeError);
            else
                normalized.PostalCode = normalizedPostalCode;

            // city
            if (string.IsNullOrWhiteSpace(patient.City))
                result.AddError("City", "City field is empty");
            else
                normalized.City = NormalizeName(patient.City);

            errorMessages = result;
            return normalized;
        }
        public static bool ValidatePatient(Patient patient, out Patient validatedPatient, out ValidationMessages errorMessages)
        {
            errorMessages = new ValidationMessages();
            validatedPatient = null;

            // always required fields
            if (patient.FirstName == null ||
                patient.LastName == null ||
                patient.PESEL == null)
            {
                errorMessages.AddError("group1", "First name, last name and PESEL are required fields.");
            }

            // require either phone or email
            if (patient.Phone == null && patient.Email == null)
            {
                errorMessages.AddError("group2", "Either phone or email must be provided.");
            }

            // IF address is given then require it to be filled properly,
            if (patient.City != null)
            {
                if (patient.PostalCode == null || patient.BuildingNumber == null)
                {
                    errorMessages.AddError("group3", "Address should contain at least postal code and building number.");
                }
            }

            if (!errorMessages.IsValid)
                return false;

            validatedPatient = patient;
            return true;
        }
    }
}
