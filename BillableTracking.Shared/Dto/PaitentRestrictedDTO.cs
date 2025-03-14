using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillableTracking.Shared.Models
{

    public class PatientRestrictedDTO : BaseRecord
    {
        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty; // Sensitive, should be encrypted

        [ForeignKey("InsuranceCompanyRecord")]
        public string? InsuranceCompanyID { get; set; }

        [Required(ErrorMessage = "Unit Record is required")]
        public string UnitRecord { get; set; } = string.Empty;

        [Required(ErrorMessage = "Admission Number is required")]
        public string AdmissionNumber { get; set; } = string.Empty;

        public string? HospitalID { get; set; }

        public string? PatientStickerImageFile { get; set; }

        public void MapFromPatientRecord(PatientRecord record)
        {
            LastName = record.LastName;
            InsuranceCompanyID = record.InsuranceCompanyID;
            UnitRecord = record.UnitRecord;
            AdmissionNumber = record.AdmissionNumber;
            return;
        }
        public void MapFromPatientNewRecord(PatientNewRecord record)
        {
            LastName = record.LastName;
            InsuranceCompanyID = record.InsuranceCompanyID;
            UnitRecord = record.UnitRecord;
            AdmissionNumber = record.AdmissionNumber;
            return;
        }
    }

}
