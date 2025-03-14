using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillableTracking.Shared.Models
{
    public class SiteConfiguration
    {
        [Key]
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        [Required(ErrorMessage = "Company Name is required")]
        [StringLength(100, ErrorMessage = "Company Name can't be longer than 100 characters")]
        public string Company { get; set; } = string.Empty;

        [Required(ErrorMessage = "ABN is required")]
        [StringLength(50, ErrorMessage = "ABN can't be longer than 50 characters")]
        public string ABN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address1 is required")]
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Post Code is required")]
        public string PostCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account Name is required")]
        public string AccountName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account Number is required")]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account BSB is required")]
        public string AccountBSB { get; set; } = string.Empty;

        [Required(ErrorMessage = "Website Address is required")]
        public string WebsiteAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Website IP Address is required")]
        [RegularExpression(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b", ErrorMessage = "Invalid IP Address format")]
        public string WebsiteIPAddress { get; set; }

        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
        public string? Message { get; set; }
        public string? UserId { get; set; }

        public string? LogoFileReference { get; set; }
        [ForeignKey("UserId")]
        public virtual User? UserRecord { get; set; }

        [Required(ErrorMessage = "Principal Provider Number is required"), MaxLength(8, ErrorMessage = "Error - Principal Provider Number is Required")]
        public string PrincipalProviderNumber { get; set; } = string.Empty;
        // Next Invoice Number is required and should be unique and should only be characters as a prefix and the incrementing number at the end.        
        [InvoiceNumberValidation]
        [MaxLength(20)] // Optional: Limit the length of the invoice number
        public string? NextInvoiceNumber { get; set; }




    }
}
