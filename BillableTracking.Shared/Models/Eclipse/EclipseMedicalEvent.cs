using System.ComponentModel.DataAnnotations;

namespace BillableTracking.Shared.Models.Eclipse
{
    public class EclipseMedicalEvent
    {
        [Required]
        public string ID { get; set; } = string.Empty; // ID for database table

        [Required, RegularExpression(@"^\d{1,2}$", ErrorMessage = "MedicalEventId must be 1 or 2 digits.")]
        public string MedicalEventId { get; set; } = string.Empty; // Sequential number for each medical event must be 1 or 2 digits
        [Required, RegularExpression(@"^\d{1,5}$", ErrorMessage = "ItemCode must be from 1 to 5 digits.")]
        public string ItemCode { get; set; } = ""; // From 1 to 5 digits, valid Medicare supported MBS code
        [Required, RegularExpression(@"^\d{1,5}\.\d{2}$", ErrorMessage = "Price must be in the format XXXXX.YY.")]
        public decimal Price { get; set; } // Currency notation, up to 5 numeric and 2 decimal digits
        [Required, RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "ServiceDate must be in the format YYYY-MM-DD.")]
        public string ServiceDate { get; set; } = string.Empty;   // Format: YYYY-MM-DD
        [MaxLength(50, ErrorMessage = "ServiceText Max Length 50")]
        public string ServiceText { get; set; } = string.Empty; // Up to 50 characters
        [Required, MaxLength(8, ErrorMessage = "Max Length 8")]
        public string ServiceProviderNumber { get; set; } = ""; // 8 alphanumeric characters

        public EclipseMedicalEvent(BillableItemEventRecord eventRecord, int count)
        {
            ID = eventRecord.ID;
            MedicalEventId = count.ToString();                                    // Column 17 - Medical Event Id Incrmenting Number
            ItemCode = eventRecord.SelectedItem.ItemCode;                         // Column 27 - Item Code (MBS Code)
            Price = eventRecord.EventItemPrice;                                   // Column 28 - Price
            ServiceDate = eventRecord.EventDate.ToString("yyyy-MM-dd");           // Column 29 - Service Date (YYYY-MM-DD)
            ServiceText = eventRecord.serviceText ?? "";                          // Column 30 - Service Text            
            ServiceProviderNumber = eventRecord.UserHospital.UserProviderNumber;  // Column 31 - Service Provider Number
        }
    }
}
