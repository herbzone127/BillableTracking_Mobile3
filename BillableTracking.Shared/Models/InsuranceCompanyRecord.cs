namespace BillableTracking.Shared.Models
{
    public class InsuranceCompanyRecord : BaseRecord
    {
        public string Name { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = "New South Wales";
        public string Country { get; set; } = "Australia";
        public string PostCode { get; set; } = string.Empty;
        public string ABN { get; set; } = string.Empty;

        public InsuranceCompanyRecord UpdateRecord(InsuranceCompanyRecord record)
        {
            this.Name = record.Name;
            this.Address1 = record.Address1;
            this.Address2 = record.Address2;
            this.City = record.City;
            this.State = record.State;
            this.Country = record.Country;
            this.PostCode = record.PostCode;
            this.ABN = record.ABN;
            return this;
        }
    }
}
