using System.ComponentModel.DataAnnotations;

namespace BillableTracking.Shared.Models
{
    public class ReferralSourceRecord : BaseRecord
    {
        public string FullName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Referral Type is required"), MaxLength(1, ErrorMessage = "Referral Type Max Length is 1"), RegularExpression("^[DPS]$", ErrorMessage = "Referral Type must be D, P, or S")]
        public string ServicelType { get; set; } = string.Empty;   // D = Diagnostic Imaging, P = Pathology, S = Specialist  
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = "New South Wales";
        public string Country { get; set; } = "Australia";
        public string Postcode { get; set; } = string.Empty;
        public string ProviderNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string ABN { get; set; } = string.Empty;

        public ReferralSourceRecord MapUpdate(ReferralSourceRecord record)
        {
            FullName = record.FullName;
            FirstName = record.FirstName;
            LastName = record.LastName;
            Address1 = record.Address1;
            Address2 = record.Address2;
            City = record.City;
            State = record.State;
            Country = record.Country;
            Postcode = record.Postcode;
            ProviderNumber = record.ProviderNumber;
            PhoneNumber = record.PhoneNumber;
            MobileNumber = record.MobileNumber;
            CompanyName = record.CompanyName;
            ABN = record.ABN;
            return this;
        }
    }
}
