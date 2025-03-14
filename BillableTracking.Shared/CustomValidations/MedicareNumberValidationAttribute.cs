using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BillableTracking.Shared
{


    public class MedicareNumberValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string medicareNo = value.ToString() ?? string.Empty;

                if (String.IsNullOrEmpty(medicareNo))
                {
                    return new ValidationResult("Medicare Card Number Required");
                }
                else if (ValidateMedicareNumber(medicareNo))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Invalid Medicare Number");
                }
            }
            else
            {
                return new ValidationResult("Medicare Card Number Required");
            }
        }
        public bool ValidateMedicareNumber(string medicareNo)
        {
            // Remove non-digit characters
            var sanitizedMedicareNo = Regex.Replace(medicareNo, @"[^\d]", "");

            // Check for 11 digits
            //     Checks medicare card number for validity
            //     using the published checksum algorithm.
            //     Returns: true if the number is valid, false otherwise.
            //     Note - this expects 11 digits including the IRN.  
            //     To validate numbers without IRNs, change the length
            //     check to 10 digits.
            if (sanitizedMedicareNo.Length == 10)
            {
                // Test leading digit and checksum
                var match = Regex.Match(sanitizedMedicareNo, @"^([2-6]\d{7})(\d)");
                if (match.Success)
                {
                    var baseNumber = match.Groups[1].Value;
                    var checkDigit = int.Parse(match.Groups[2].Value);
                    int sum = 0;
                    int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9 };

                    for (int i = 0; i < weights.Length; i++)
                    {
                        sum += (baseNumber[i] - '0') * weights[i];
                    }

                    return (sum % 10) == checkDigit;
                }
            }

            return false;
        }
    }
}
