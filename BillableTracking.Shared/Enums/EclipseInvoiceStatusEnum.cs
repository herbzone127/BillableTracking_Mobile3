using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BillableTracking.Shared.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EclipseInvoiceStatusEnum
    {
        // Enum for Claim Status to be stored in the database as a string value representing the status of the claim
        // The status of the claim can be one of the following:
        // - Outstanding: The claim is outstanding and has not been processed yet
        // - Declined: The claim has been declined
        // - Approved: The claim has been approved
        // - Completed: The claim has been completed
        // - Cancelled: The claim has been cancelled
        [Display(Name = "NotInvoiced")]
        NotInvoiced,
        [Display(Name = "ReadyToBatch")]
        ReadyToBatch,
        [Display(Name = "Outstanding")]
        Outstanding,
        [Display(Name = "Declined")]
        Declined,
        [Display(Name = "Approved")]
        Approved,
        [Display(Name = "Completed")]
        Completed,
        [Display(Name = "Cancelled")]
        Cancelled
    }
}
