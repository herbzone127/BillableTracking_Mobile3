using System.ComponentModel.DataAnnotations;

namespace BillableTracking.Shared.Enums
{
    public enum XeroInvoiceStatusEnum
    {
        // Enum for Xero Invoice Status to be stored in the database as a string value representing the status of the Xero invoice
        // The status of the Xero invoice can be one of the following:
        // - Draft: The invoice is in draft status
        // - Submitted: The invoice has been submitted
        // - Authorised: The invoice has been authorised
        // - Paid: The invoice has been paid
        // - Voided: The invoice has been voided
        [Display(Name = "NotInvoiced")]
        NotInvoiced,
        [Display(Name = "Draft")]
        Draft,
        [Display(Name = "Submitted")]
        Submitted,
        [Display(Name = "Authorised")]
        Authorised,
        [Display(Name = "Paid")]
        Paid,
        [Display(Name = "Voided")]
        Voided
    }
}
