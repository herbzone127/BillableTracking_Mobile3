namespace BillableTracking.Shared.Dto
{
    public class PatientToInvoiceSummary
    {
        public string PatientId { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string InsuranceCoID { get; set; } = string.Empty;
        public string InsuranceCoName { get; set; } = string.Empty;
        public int NumberOfEvents { get; set; } = 0;
        public decimal TotalAmount { get; set; } = 0;
    }
}
