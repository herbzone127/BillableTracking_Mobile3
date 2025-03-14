using System.ComponentModel.DataAnnotations;

namespace BillableTracking.Shared.Models.Eclipse
{
    public class EclipseInvoice
    {
        [Required]
        [Key]
        public string ID { get; set; } = string.Empty; // ID for database table
        [Required, MaxLength(7, ErrorMessage = "Must Be eclipse")]
        public string Funder { get; set; } = "eclipse";// "eclipse" always

        [Required, MaxLength(16, ErrorMessage = "Max Length 16")]
        public string InvoiceReference { get; set; } = string.Empty; // Up to 16 characters, unique for each invoice

        [Required, MaxLength(40, ErrorMessage = "Max Length 40"), RegularExpression(@"^[a-zA-Z0-9\s'-]+$", ErrorMessage = "Can only contain alpha(A-Z and a-z), numeric(0-9), space(), apostrophe(') and hyphen (-) characters")]
        public string PatientFirstName { get; set; } = string.Empty; // 1 to 40 characters • Can only contain alpha(A-Z and a-z),numeric(0-9), space(),apostrophe(') and hyphen (-) characters
        [Required, MaxLength(40, ErrorMessage = "Max Length 40"), RegularExpression(@"^[a-zA-Z0-9\s'-]+$", ErrorMessage = "Can only contain alpha(A-Z and a-z), numeric(0-9), space(), apostrophe(') and hyphen (-) characters")]
        public string PatientLastName { get; set; } = string.Empty; // 1 to 40 characters 
        [Required]
        public string PatientID { get; set; } = string.Empty; // 1 to 128 characters - Id from the patient database
        [Required, RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "ReferralDate must be in the format YYYY-MM-DD.")]
        public string PatientDob { get; set; } = string.Empty; // Format: YYYY-MM-DD

        [Required, Length(10, 10, ErrorMessage = "Fix Length 10"), MedicareNumberValidation]
        public string PatientMedicareMembershipNumber { get; set; } = string.Empty; // 10 digits
        [Required, Length(1, 2, ErrorMessage = "Fix Length 1 or 2")]
        public string PatientMedicareCardRank { get; set; } = string.Empty; // 1 or 2 numbers
        [Required, Length(1, 2, ErrorMessage = "Fund Card Rank 1 or 2 Characters")]
        public string PatientHealthFundCardRank { get; set; } = "0"; // 1 to 2 numbers
        [Required, MaxLength(19, ErrorMessage = "Max Membership Number Length = 19")]
        public string PatientHealthFundMembershipNumber { get; set; } = string.Empty; // 1 to 19 characters
        [Required, MaxLength(1, ErrorMessage = "AccidentIndicator Max Length 1"), RegularExpression(@"^[YN]$", ErrorMessage = "AccidentIndicator Must be Y or N")]
        public string AccidentIndicator { get; set; } = "N"; // Y or N
        [Required, MaxLength(1, ErrorMessage = "Account Paid In Full Max Length 1"), RegularExpression(@"^[YN]$", ErrorMessage = "Account Paid In Full Must be Y or N")]
        public string AccountPaidIndicator { get; set; } = "N"; // Y or N
        [Required, Length(1, 20, ErrorMessage = "AccountRefId Length 1-20 Characters"), RegularExpression(@"^[a-zA-Z0-9]{1,20}$", ErrorMessage = "AccountRefId can only contain alphanumeric characters and must be between 1 and 20 characters long.")]
        public string AccountRefId { get; set; } = string.Empty; // 1 to 20 alphanumeric characters
        [Required, MaxLength(2, ErrorMessage = "Max Length 2"), RegularExpression(@"^(AG|SC|MB|MO|PC)$", ErrorMessage = "Valid values: AG = Agreement, SC = Scheme, MB = Billing Agent, MO = Medicare Only, PC = Patient Claim")]
        public string ClaimTypeCode { get; set; } = "SC"; // Valid values: AG = Agreement, SC = Scheme, MB = Billing Agent, MO = Medicare Only, PC = Patient Claim
        [Required, MaxLength(1, ErrorMessage = "CompensationClaimIndicator Max Length 1"), RegularExpression(@"^[YN]$", ErrorMessage = "CompensationClaimIndicator Must be Y or N")]
        public string CompensationClaimIndicator { get; set; } = "N"; // Y or N Use Regex to validate
        [Required, MaxLength(8, ErrorMessage = "Max Length 8")]
        public string FacilityID { get; set; } = string.Empty; // Up to 8 alphanumeric characters. Hospital Provider Number

        //Items Common to all medical events
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "AdmissionDate must be in the format YYYY-MM-DD.")]
        public string AdmissionDate { get; set; } = string.Empty;  // Format: YYYY-MM-DD, only if applicable        
        [Required, MaxLength(1, ErrorMessage = "Financial Interest Disclosure Must Max Length 1"), RegularExpression(@"^[YN]$", ErrorMessage = "Financial Interest Disclosure Must be Y or N")]
        public string FinancialInterestDisclosure { get; set; } = "Y"; // Y or N
        [Required, MaxLength(1, ErrorMessage = "InformedFinancialConsent Max Length 1"), RegularExpression(@"^[VWNX]$", ErrorMessage = "InformedFinancialConsent Must be V, W, N or X")]
        public string InformedFinancialConsent { get; set; } = "W"; //  V = Verbal, W = In writing, where appropriate, N = Not issued or X = Not obtained
        [Required, RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "ReferralDate must be in the format YYYY-MM-DD.")]
        public string ReferralDate { get; set; } = string.Empty;   // Format: YYYY-MM-DD
        [Required, RegularExpression(@"^\d{1,2}$", ErrorMessage = "Referral Period must be 1 or 2 digits.")]
        public string ReferralPeriod { get; set; } = string.Empty; // Number of days for referral period, must be 1 or 2 digits
        [Required, MaxLength(1, ErrorMessage = "ReferralPeriodCode Max Length 1"), RegularExpression(@"^[SNI]$", ErrorMessage = "Valid values: S = Standard, N = Non Standard, I = Indefinite")]
        public string ReferralPeriodCode { get; set; } = "S"; // Single Character S = Standard, N = Non Standard, I = Indefinite 
        [Required, MaxLength(8, ErrorMessage = "Max Length 8"), RegularExpression(@"^[a-zA-Z0-9]{1,8}$", ErrorMessage = "ReferralProviderNumber can only contain alphanumeric characters and must be between 1 and 8 characters long.")]
        public string ReferralProviderNumber { get; set; } = string.Empty; // Up to 8 alphanumeric characters
        [Required, MaxLength(1, ErrorMessage = "ReferralTypeCode Max Length 1"), RegularExpression(@"^[SDP]$", ErrorMessage = "Valid values: S = Specialist, D = Diagnostic Imaging, P = Pathology")]
        public string ReferralTypeCode { get; set; } = "S";  // Single Character S = Specialist, D = Diagnostic Imaging, P = Pathology
        [Required, MaxLength(1, ErrorMessage = "ReferralOverrideCode Max Length 1"), RegularExpression(@"^[HELNO]?$")]
        public string ReferralOverrideCode { get; set; } = string.Empty; // Blank or Single Character H = Hospital in-patient referral, E = Emergency, L = Lost, N = Not required, non-standard referral, O = Omitted, referral object not set(may be required for self deemed services
        [Required, MaxLength(8, ErrorMessage = "Max Length 8")]
        public string PrincipalProviderNumber { get; set; } = string.Empty;// Up to 8 alphanumeric characters
        [Required, MaxLength(1, ErrorMessage = "ServiceTypeCode Max Length 1"), RegularExpression(@"^[SOP]$", ErrorMessage = "Valid values: S = Specialist, O = General, P = Pathology")]
        public string ServiceTypeCode { get; set; } = "S"; // Single Character S = Specialist, O = General, P = Pathology

        // Sender Contact Information
        [Required, MaxLength(50, ErrorMessage = "SenderContactEmail Max Length 50"), RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "SenderContactEmail must be a valid email address.")]
        public string SenderContactEmail { get; set; } = string.Empty; // Up to 50 characters
        [Required, MaxLength(40, ErrorMessage = "SenderContactName Max Length 40")]
        public string SenderContactName { get; set; } = string.Empty; // Up to 40 characters
        [Required, MaxLength(20, ErrorMessage = "SenderContactPhone Max Length 20"), RegularExpression(@"^(\+61|0)[2-478](\d{8}|\d{9})$", ErrorMessage = "SenderContactPhone must be a valid Australian phone number.")]
        public string SenderContactPhone { get; set; } = string.Empty; // Up to 20 characters

        public List<EclipseMedicalEvent> EclipseMedicalEvents { get; set; } = []; // List of medical events

        public EclipseInvoice(SiteConfiguration config, PatientRecord patient, HospitalRecord hospital, UserHospitalRecord userHospital, User lodgingUser, InvoiceRecord invoice)
        {

            Funder = invoice.Funder;                                            // Column 01  funder
            InvoiceReference = invoice.InvoiceNumber;                           // Column 02  invoiceReference
            PatientDob = patient.DateOfBirth.ToString("yyyy-MM-dd");            // Column 03  patient.identity.dobString Date of birth in YYYY-MM-DD format.
            PatientFirstName = patient.FirstName;                               // Column 04  patient.identity.firstName
            PatientLastName = patient.LastName;                                 // Column 05  patient.identity.lastName
            PatientID = patient.ID;                                             // Column 06  patient.identity.refId
            PatientMedicareCardRank = patient.MedicareIndRef.ToString();        // Column 07  patient.medicareHealthFundAccount.cardRank
            PatientMedicareMembershipNumber = patient.MedicareNumber;           // Column 08  patient.medicareHealthFundAccount.membershipNumber
            PatientHealthFundCardRank = "0";                                    // Column 09  patient.phiHealthFundAccount.cardRank
            PatientHealthFundMembershipNumber = patient.InsuranceMemberNumber;  // Column 10  patient.phiHealthFundAccount.membershipNumber
            AccidentIndicator = patient.AccidentIndicatorStr;                   // Column 11  accident.accidentInd
            AccountPaidIndicator = "N";                                         // Column 12  accountPaidInd
            AccountRefId = invoice.InvoiceNumber;                               // Column 13  accountReferenceId
            ClaimTypeCode = patient.ClaimTypeCode;                              // Column 14  claimTypeCode
            CompensationClaimIndicator = patient.CompensationIndicatorStr;      // Column 15  compensationClaimInd
            FacilityID = hospital.ProviderNumber;                               // Column 16  facilityId                                                                                 
                                                                                // Column 17  Medical Event ID - Set When Writing to File as an incrementing number
            AdmissionDate = patient.AdmissionDate.ToString("yyyy-MM-dd");       // Column 18  medicalEvents.admissionDateString Admission Date in YYYY-MM-DD format.
            FinancialInterestDisclosure = patient.FinancialInsterestDisclosureIndicatorStr;    // Column 19  medicalEvents.financialInterestDisclosureInd
            InformedFinancialConsent = patient.IfcIssueCode;                    // Column 20  medicalEvents.ifcIssueCode
            ReferralDate = patient.ReferralDate.ToString("yyyy-MM-dd");         // Column 21  medicalEvents.referral.issueDateString Referral Date in YYYY-MM-DD format.
            ReferralPeriod = patient.ReferralPeriod.ToString();                 // Column 22  medicalEvents.referral.period
            ReferralPeriodCode = patient.ReferralPeriodCode;                    // Column 23  medicalEvents.referral.periodCode
            ReferralProviderNumber = patient.ReferralSourceRecord != null ? patient.ReferralSourceRecord.ProviderNumber : string.Empty;       // Column 24  medicalEvents.referral.providerNumber
            ReferralTypeCode = patient.ReferralSourceRecord != null ? patient.ReferralSourceRecord.ServicelType : string.Empty;               // Column 25  medicalEvents.referral.referralTypeCode
            ReferralOverrideCode = patient.ReferralOverRideCode.Trim();         // Column 26  medicalEvents.referralOverrideCode

            PrincipalProviderNumber = config.PrincipalProviderNumber;           // Column 32  principalProvider.providerNumber
            SenderContactEmail = lodgingUser.Email ?? "lauren@tarrantrehab.au"; // Column 33  senderContact.emailAddress
            SenderContactName = lodgingUser.FullName;                           // Column 34  senderContact.name
            SenderContactPhone = lodgingUser.PhoneNumber ?? "0437916075";       // Column 35  senderContact.phoneNumber
            ServiceTypeCode = userHospital.UserTypeCode;                        // Column 36  serviceTypeCode

            // Loop through all medical events and add them to the invoice

            int count = 0;
            foreach (BillableItemEventRecord billableItemEventRecord in invoice.BillableItemEvents)
            {
                count++;
                EclipseMedicalEvents.Add(new(billableItemEventRecord, count));
            }
        }
    }
}
