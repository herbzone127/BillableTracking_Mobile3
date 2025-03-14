namespace BillableTracking.Shared.Models
{
    public class InvoiceEventLog
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string InvoiceID { get; set; }
        public DateTime EventLogDate { get; set; } = DateTime.Now;
        public string EventLogUser { get; set; }
        public string EventLogDescription { get; set; }
    }

}
