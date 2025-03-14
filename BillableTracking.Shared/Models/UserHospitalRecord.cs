using BillableTracking.Shared.Dto;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillableTracking.Shared.Models
{
    public class UserHospitalRecord : BaseRecord
    {


        public string UserProviderNumber { get; set; } = string.Empty;
        [ForeignKey("Hospital")]
        public string HospitalID { get; set; } = string.Empty;
        public virtual HospitalRecord? Hospital { get; set; }
        [ForeignKey("UserRecord")]
        public string UserID { get; set; } = string.Empty;
        public virtual User? UserRecord { get; set; }
        public string UserSpeciality { get; set; } = string.Empty;
        public string UserTypeCode { get; set; } = "S";

        public UserHospitalRecord MapDTO(UserHospitalDTO record)
        {
            UserProviderNumber = record.UserProviderNumber;
            HospitalID = record.HospitalID;
            UserID = record.UserID;
            UserSpeciality = record.UserSpeciality;
            UserTypeCode = record.UserTypeCode;
            return this;
        }
    }
}
