using BillableTracking.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillableTracking.Shared.Models
{
    public class BillableItemEventRecord : BaseRecord
    {
        //[ForeignKey("Doctor")]
        public string DoctorID { get; set; } = string.Empty;
        //public virtual User Doctor { get; set; } = new();
        [ForeignKey("UserHospital")]
        [Required(ErrorMessage = "User Hospital is required")]
        public string UserHospitalID { get; set; } = string.Empty;
        public virtual UserHospitalRecord UserHospital { get; set; } = new();
        [ForeignKey("Patient")]
        public string PatientID { get; set; } = string.Empty;

        public virtual PatientRecord Patient { get; set; } = new();

        [ForeignKey("SelectedItem")]
        public string SelectedItemID { get; set; } = string.Empty;
        public virtual BillableItemRecord SelectedItem { get; set; } = new();

        [Required(ErrorMessage = "Event Date is required")]
        private DateTime eventDate;
        public DateTime EventDate
        {
            get
            {
                return eventDate.ToLocalTime().Date;
            }
            set
            {
                eventDate = value.ToUniversalTime();
            }
        }
        private TimeSpan eventTime;
        public TimeSpan EventTime
        {
            get
            {
                return new TimeSpan(eventTime.Hours, eventTime.Minutes, 0);
            }
            set
            {
                eventTime = value;
            }
        }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal EventItemPrice { get; set; }

        public string? Notes { get; set; }
        public string? serviceText { get; set; } = "";

        public bool Invoiced { get; set; } = false;

        public int? InvoiceID { get; set; }

        public string? BatchID { get; set; } = string.Empty;

        public EclipseInvoiceStatusEnum? EclipseInvoiceStatus { get; set; }

        public void MapUpdate(BillableItemEventRecord updatedRecord)
        {
            this.PatientID = updatedRecord.PatientID;
            this.SelectedItemID = updatedRecord.SelectedItemID;
            this.EventDate = updatedRecord.EventDate;
            this.EventTime = updatedRecord.EventTime; https://localhost:7275/billable-item-events
            this.EventItemPrice = updatedRecord.EventItemPrice;
            this.Notes = updatedRecord.Notes;
            this.Invoiced = updatedRecord.Invoiced;
            this.UpdatedDate = DateTime.UtcNow;
        }

        public bool ValidateEvent(out List<string> ErrorsFound)
        {
            List<string> errors = [];

            BillableItemEventRecord validationModel = this;

            // Validate each property of the invoice model using DataAnnotations
            var context = new ValidationContext(validationModel, serviceProvider: null, items: null);

            // Validate the model
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(validationModel, context, results, true);

            if (!isValid)
            {
                foreach (var validationResult in results)
                {
                    if (validationResult != ValidationResult.Success && !String.IsNullOrEmpty(validationResult.ErrorMessage))
                        errors.Add(validationResult.ErrorMessage);
                }
            }
            ErrorsFound = errors;
            return isValid;
        }
        public BillableItemEventRecord() { } // Optional, but keeps an explicit constructor
        public BillableItemEventRecord(BillableItemEventDTO dto)
        {
            // Create new record from DTO
            this.ID = dto.ID;
            this.DoctorID = dto.DoctorID;
            this.UserHospitalID = dto.UserHospitalID;
            this.PatientID = dto.PatientID;
            this.SelectedItemID = dto.SelectedItemID;
            this.EventDate = dto.EventDate;
            this.EventTime = dto.EventTime;
            this.EventItemPrice = dto.EventItemPrice;
            this.Notes = dto.Notes;
            this.serviceText = dto.serviceText;
            this.Invoiced = dto.Invoiced;
            this.BatchID = dto.BatchID;
            this.EclipseInvoiceStatus = dto.EclipseInvoiceStatus;
            this.CreatedDate = dto.CreatedDate;
            this.CreatedByUserID = dto.CreatedByUserID;
            this.UpdatedDate = dto.UpdatedDate;
            this.UpdatedByUserID = dto.UpdatedByUserID;
            this.IsDeleted = dto.IsDeleted;
            this.DeletedDate = dto.DeletedDate;
            this.DeletedByUserID = dto.DeletedByUserID;
        }
    }
}
