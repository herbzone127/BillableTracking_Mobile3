namespace BillableTracking.Shared.Models
{
    public class HospitalRecord : BaseRecord
    {
        public string Name { get; set; } = string.Empty;
        public string ProviderNumber { get; set; } = string.Empty;
        public string LSPN { get; set; } = string.Empty;
        public bool IsVerified { get; set; }

        public HospitalRecord MapUpdate(HospitalRecord record)
        {
            Name = record.Name;
            ProviderNumber = record.ProviderNumber;
            LSPN = record.LSPN;
            IsVerified = record.IsVerified;
            return this;
        }
    }
}
