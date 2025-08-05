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
        public class ValidationMessages
        {
            public bool IsValid => Errors.Count == 0;
            public List<string> Errors { get; } = new List<string>();

            // TODO rewrite AddError method if later using something other than list to store errors
            public void AddError(string errorMessage)
            {
                Errors.Add(errorMessage);
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
        public static Patient TryParsePatient(Patient patient, out ValidationMessages errorMessages)
        {
            var result = new ValidationMessages();
            Patient normalized = new Patient();

            // name
            if (string.IsNullOrWhiteSpace(patient.FirstName))
                result.AddError("Name field is empty");
            else
                normalized.FirstName = NormalizeName(patient.FirstName);

            // last name
            if (string.IsNullOrWhiteSpace(patient.LastName))
                result.AddError("Last name field is empty");
            else
                normalized.LastName = NormalizeName(patient.LastName);

            // PESEL
            if (!TryParsePesel(patient.PESEL, out string normalizedPesel, out string peselError))
                result.AddError(peselError);
            else
                normalized.PESEL = normalizedPesel;

            // phone
            if (!TryParsePhone(patient.Phone, out string normalizedPhone, out string phoneError))
                result.AddError(phoneError);
            else
                normalized.Phone = normalizedPhone;

            // email
            if (!TryParseEmail(patient.Email, out string normalizedEmail, out string emailError))
                result.AddError(emailError);
            else
                normalized.Email = normalizedEmail;

            // street
            if (string.IsNullOrWhiteSpace(patient.Street))
                result.AddError("Street field is empty");
            else
                normalized.Street = NormalizeName(patient.Street);

            // building number
            if (string.IsNullOrWhiteSpace(patient.BuildingNumber))
                result.AddError("Building field is empty");
            else
                normalized.BuildingNumber = patient.BuildingNumber.Trim();

            // apartment number
            if (string.IsNullOrWhiteSpace(patient.ApartmentNumber))
                result.AddError("Apartment field is empty");
            else
                normalized.ApartmentNumber = patient.ApartmentNumber.Trim();

            // postal code
            if (!TryParsePostalCode(patient.PostalCode, out string  normalizedPostalCode, out string postalCodeError))
                result.AddError(postalCodeError);
            else
                normalized.PostalCode = normalizedPostalCode;

            // city
            if (string.IsNullOrWhiteSpace(patient.City))
                result.AddError("City field is empty");
            else
                normalized.City = NormalizeName(patient.City);

            errorMessages = result;
            return normalized;
        }
        public static bool ValidatePatient(Patient patient, out ValidationMessages errorMessages)
        {
            Patient parsed = TryParsePatient(patient, out ValidationMessages patientErrors);
            // idk if i will use messages but why not have them
            errorMessages = patientErrors;

            // always required fields
            if (parsed.FirstName == null ||
                parsed.LastName == null ||
                parsed.PESEL == null)
                return false;

            // require either phone or email
            if (parsed.Phone == null && parsed.Email == null)
                return false;

            // IF address is given then require it to be filled properly,
            if (parsed.City != null)
            {
                if (parsed.PostalCode == null || parsed.BuildingNumber == null)
                    return false;
            }

            return true;
        }
    }
}
