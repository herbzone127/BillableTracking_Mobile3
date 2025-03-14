using BillableTracking.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace BillableTracking.Shared.Models
{
    public class BillableItemEventDTO : BaseRecord
    {
        [Required(ErrorMessage = "Doctor ID is required")]
        public string DoctorID { get; set; } = string.Empty;
        [Required(ErrorMessage = "User Hospital ID is required")]
        public string UserHospitalID { get; set; } = string.Empty;
        [Required(ErrorMessage = "Patient ID is required")]
        public string PatientID { get; set; } = string.Empty;
        [Required(ErrorMessage = "SelectedItem Item ID is required")]
        public string SelectedItemID { get; set; } = string.Empty;


        [Required(ErrorMessage = "Event Date is required")]
        public DateTime EventDate { get; set; }
        public TimeSpan EventTime { get; set; }

        public decimal EventItemPrice { get; set; }

        public string? Notes { get; set; }
        public string? serviceText { get; set; } = "";

        public bool Invoiced { get; set; } = false;

        public int? InvoiceID { get; set; }

        public string? BatchID { get; set; } = string.Empty;

        public EclipseInvoiceStatusEnum? EclipseInvoiceStatus { get; set; }

        public void MapFromBillableItemEventRecord(BillableItemEventRecord billableItemEvent)
        {
            // Map the properties from the BillableItemEventRecord to the BillableItemEventDTO
            this.DoctorID = billableItemEvent.DoctorID;
            this.UserHospitalID = billableItemEvent.UserHospitalID;
            this.PatientID = billableItemEvent.PatientID;
            this.SelectedItemID = billableItemEvent.SelectedItemID;
            this.EventDate = billableItemEvent.EventDate;
            this.EventTime = billableItemEvent.EventTime;
            this.EventItemPrice = billableItemEvent.EventItemPrice;
            this.Notes = billableItemEvent.Notes;
            this.serviceText = billableItemEvent.serviceText;
            this.Invoiced = billableItemEvent.Invoiced;
            this.InvoiceID = billableItemEvent.InvoiceID;
            this.BatchID = billableItemEvent.BatchID;
            this.EclipseInvoiceStatus = billableItemEvent.EclipseInvoiceStatus;
        }
    }
}
