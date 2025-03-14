using System.ComponentModel.DataAnnotations;

namespace BillableTracking.Shared.Dto
{
    public class UserHospitalVM
    {
        [Required(ErrorMessage = "Create ID String")]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        [Required(ErrorMessage = "Please Select User Name")]
        public string UserID { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please Enter User Speciality")]
        public string UserSpecialty { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please Select Hospital Name")]
        public string HospitalID { get; set; } = string.Empty;
        public string HospitalName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please Enter User Provider Number for Selected Hospital")]
        public string UserProviderNumber { get; set; } = string.Empty;
    }
}
