using BillableTracking.Shared.Models;

namespace BillableTracking.Shared.Dto
{
    public class UserHospitalDTO
    {
        public string ID { get; set; } = string.Empty;
        public string UserID { get; set; } = string.Empty;
        public string UserSpeciality { get; set; } = string.Empty;
        public string HospitalID { get; set; } = string.Empty;
        public string UserProviderNumber { get; set; } = string.Empty;
        public string UserTypeCode { get; set; } = "S";

        public UserHospitalRecord MapDTO()
        {
            UserHospitalRecord returnRecord = new();
            // Map DTO to new record
            returnRecord.ID = this.ID;
            returnRecord.UserID = this.UserID;
            returnRecord.UserSpeciality = this.UserSpeciality;
            returnRecord.HospitalID = this.HospitalID;
            returnRecord.UserProviderNumber = this.UserProviderNumber;
            returnRecord.UserTypeCode = this.UserTypeCode;
            return returnRecord;
        }
    }
}
