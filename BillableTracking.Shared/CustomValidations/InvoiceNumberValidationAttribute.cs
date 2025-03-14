using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BillableTracking.Shared
{
    public partial class InvoiceNumberValidationAttribute : ValidationAttribute
    {
        // Define the regex pattern: prefix (letters) followed by numeric (digits)
        [GeneratedRegex(@"^[^\d]+\d+$")]
        private static partial Regex InvoiceNumberRegex();

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success; // Allow null (use [Required] to enforce presence)

            string? invoiceNumber = value.ToString();

            if (invoiceNumber != null && InvoiceNumberRegex().IsMatch(invoiceNumber))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Invoice number must contain a text prefix followed by a numeric suffix.");
            }
        }
    }
}
