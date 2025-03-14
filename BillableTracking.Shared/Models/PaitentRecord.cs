using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace BillableTracking.Shared.Models
{

    public class PatientRecord : BaseRecord
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty; // Sensitive, should be encrypted

        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty; // Sensitive, should be encrypted

        [MaxLength(50)]
        public string? Middle { get; set; } // Sensitive, should be encrypted

        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty; // Sensitive, should be encrypted

        [ForeignKey("InsuranceCompanyRecord")]
        public string? InsuranceCompanyID { get; set; }
        public virtual InsuranceCompanyRecord? InsuranceCompanyRecord { get; set; }

        [Required(ErrorMessage = "Insurance Member Number is required")]
        [MaxLength(20)]
        public string InsuranceMemberNumber { get; set; } = string.Empty; // Sensitive, should be encrypted

        [MedicareNumberValidationAttribute]
        [MaxLength(20)]
        public string MedicareNumber { get; set; } = string.Empty;// Sensitive, should be encrypted

        // Medicare Individual Reference is required and should be between 1 and 9.
        [Required(ErrorMessage = "Medicare Individual Reference is required")]
        public int MedicareIndRef { get; set; }

        [Required(ErrorMessage = "Address Line 1 is required")]
        [MaxLength(100)]
        public string Address1 { get; set; } = string.Empty;// Sensitive, should be encrypted

        [MaxLength(100)]
        public string? Address2 { get; set; } = string.Empty;// Sensitive, should be encrypted

        [Required(ErrorMessage = "City is required")]
        [MaxLength(50)]
        public string City { get; set; } = string.Empty;// Sensitive, should be encrypted

        [Required(ErrorMessage = "New South Wales")]
        [MaxLength(50)]
        public string State { get; set; } = string.Empty;// Sensitive, should be encrypted

        [Required(ErrorMessage = "Country is required")]
        [MaxLength(50)]
        public string Country { get; set; } = "Australia";// Sensitive, should be encrypted

        [Required(ErrorMessage = "Postcode is required")]
        [MaxLength(10)]
        public string PostCode { get; set; } = string.Empty;// Sensitive, should be encrypted

        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime DateOfBirth { get; set; } = DateTime.UtcNow.AddYears(-60); // Sensitive, should be encrypted

        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddDays(-1);

        [Required(ErrorMessage = "Admission Date is required")]
        public DateTime AdmissionDate { get; set; } = DateTime.UtcNow.AddDays(-1);

        [Required(ErrorMessage = "Unit Record is required")]
        public string UnitRecord { get; set; } = string.Empty;

        [Required(ErrorMessage = "Admission Number is required")]
        public string AdmissionNumber { get; set; } = string.Empty;

        public bool AccidentIndicator { get; set; } = false; // Y or N

        [IgnoreDataMember]
        public string AccidentIndicatorStr
        {
            get
            {
                if (AccidentIndicator) return "Y";
                return "N";
            }
        }

        [Required(ErrorMessage = "The value is required.")]
        [AllowedValues("S", "O", "P", ErrorMessage = "The value must be one of 'S', 'O', or 'P'.")]
        public string ServiceTypeCode { get; set; } = "S"; // S, O, P

        [Required(ErrorMessage = "Claim Type is required")]
        [AllowedValues("AG", "SC", "MB", "MO", "PC", ErrorMessage = "Claim Type Must be AG, SC, MB, MO, PC")]
        public string ClaimTypeCode { get; set; } = "SC"; // AG = Agreement, SC = Scheme , MB = Billing Agent, MO = Medicare Only, PC = Patient Claim

        public bool CompensationIndicator { get; set; } = false; // Y or N

        [IgnoreDataMember]
        public string CompensationIndicatorStr
        {
            get
            {
                if (CompensationIndicator) return "Y";
                return "N";
            }
        }

        public bool FinancialInsterestDisclosureIndicator { get; set; } = false; // Y or N

        [IgnoreDataMember]
        public string FinancialInsterestDisclosureIndicatorStr
        {
            get
            {
                if (FinancialInsterestDisclosureIndicator) return "Y";
                return "N";
            }
        }

        [Required(ErrorMessage = "IFC Issue Code is required"), RegularExpression(@"^[VWNX]$", ErrorMessage = "IFC Issue Code Must be V, W, N, X")]
        public string IfcIssueCode { get; set; } = "W";   // V = Verbal , W = Written, N = Not Issued, X = Not Obtained

        [ForeignKey("ReferralSourceRecord")]
        public string? ReferralID { get; set; }
        public virtual ReferralSourceRecord? ReferralSourceRecord { get; set; }

        [Required(ErrorMessage = "Referral Date is required")]
        public DateTime ReferralDate { get; set; } = DateTime.UtcNow.AddDays(-1);
        [Required(ErrorMessage = "Referral Period is required")]
        public int ReferralPeriod { get; set; } = 3; // 3 Months by default. Period is in months
        [Required(ErrorMessage = "Referral Period Code is required"), RegularExpression(@"^[DSNI]$", ErrorMessage = "Referral Period Code Must be D, S, N, I")]
        public string ReferralPeriodCode { get; set; } = "S"; // S = Standard, N = Non-Standard, I = Indefinite

        //[Required(ErrorMessage = "Referral OverRide Code is required")]
        [AllowedValues("", "H", "E", "L", "N", "O", ErrorMessage = "Referral OverRide Code Must be Blank or H, E, L, N, O")]
        [MaxLength(1, ErrorMessage = "Referral Override Code Max Length 1")]
        public string ReferralOverRideCode { get; set; } = string.Empty; // Can be Blank, H = Hospital, E = Emergency, L = Lost, N = Not Required, O: Omitted - Self Deemed Service.  

        [ForeignKey("HospitalRecord")]
        public string? HospitalID { get; set; }
        public virtual HospitalRecord? HospitalRecord { get; set; }

        public ICollection<BillableItemEventRecord>? BillableItemEvents { get; set; } = [];

        public string? PatientStickerImageFile { get; set; }


        //TOTO: Add Mapping for additional properties
        public void MapUpdate(PatientRecord record)
        {
            Name = record.Name;
            FirstName = record.FirstName;
            Middle = record.Middle;
            LastName = record.LastName;
            InsuranceCompanyID = record.InsuranceCompanyID;
            InsuranceMemberNumber = record.InsuranceMemberNumber;
            MedicareNumber = record.MedicareNumber;
            Address1 = record.Address1;
            Address2 = record.Address2;
            Country = record.Country;
            State = record.State;
            City = record.City;
            PostCode = record.PostCode;
            DateOfBirth = record.DateOfBirth;
            StartDate = record.StartDate;
            AdmissionDate = record.AdmissionDate;
            UnitRecord = record.UnitRecord;
            AdmissionNumber = record.AdmissionNumber;
            ReferralID = record.ReferralID;
            ReferralDate = record.ReferralDate;
            HospitalID = record.HospitalID;
            AccidentIndicator = record.AccidentIndicator;
            ServiceTypeCode = record.ServiceTypeCode;
            ClaimTypeCode = record.ClaimTypeCode;
            CompensationIndicator = record.CompensationIndicator;
            FinancialInsterestDisclosureIndicator = record.FinancialInsterestDisclosureIndicator;
            IfcIssueCode = record.IfcIssueCode;
            ReferralPeriod = record.ReferralPeriod;
            ReferralPeriodCode = record.ReferralPeriodCode;
            ReferralOverRideCode = record.ReferralOverRideCode;
            PatientStickerImageFile = record.PatientStickerImageFile;
        }

        public void MapFromDTO(PatientRestrictedDTO record)
        {
            ID = record.ID;
            LastName = record.LastName;
            InsuranceCompanyID = record.InsuranceCompanyID;
            UnitRecord = record.UnitRecord;
            AdmissionNumber = record.AdmissionNumber;
        }
        public void MapFromNewPatientRecord(PatientNewRecord record)
        {
            ID = record.ID;
            CreatedByUserID = record.CreatedByUserID;
            CreatedDate = record.CreatedDate;
            LastName = record.LastName;
            InsuranceCompanyID = record.InsuranceCompanyID;
            UnitRecord = record.UnitRecord;
            AdmissionNumber = record.AdmissionNumber;
            HospitalID = record.HospitalID;
            PatientStickerImageFile = record.PatientStickerImageFile;
        }
    }

}
