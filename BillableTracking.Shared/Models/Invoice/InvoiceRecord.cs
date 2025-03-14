using BillableTracking.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace BillableTracking.Shared.Models
{
    public class InvoiceRecord : BaseRecord
    {
        // Properties for invoice number is to be unique in the database

        [Required]
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GST { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }

        public ICollection<InvoiceEventLog> InvoiceEventLogItems { get; set; } = []; // Tracking the events that are part of this invoice

        // Tracking the billable items that are part of this invoice
        public ICollection<string> BillableItemsEventIDs { get; set; } = []; // List of Foreign Keys to BillableItemEventRecord
        public virtual ICollection<BillableItemEventRecord> BillableItemEvents { get; set; } = []; // List of BillableItemEventRecord

        //Tyro Specific Information
        public string Funder { get; set; } = "eclipse";

        // Track the Tyro lodgement status of the invoice
        public bool TyroLodged { get; set; } = false;
        public DateTime? TyroLodgedDate { get; set; }
        public string? TyroLodgedBy { get; set; }
        public EclipseInvoiceStatusEnum EclipseLodgementStatus { get; set; } = EclipseInvoiceStatusEnum.NotInvoiced;

        // Track the Xero invoice status of the invoice
        public bool XeroInvoiced { get; set; } = false;
        public XeroInvoiceStatusEnum XeroInvoiceStatus { get; set; } = XeroInvoiceStatusEnum.NotInvoiced;
        public string XeroInvoiceID { get; set; } = string.Empty;
    }
}
