using System.ComponentModel.DataAnnotations;

namespace BillableTracking.Shared.Dto
{
    public class UserVM
    {
        [Key]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool? IsVerfied { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string? ApprovedUserDeviceName { get; set; }
        public string? UserFullName { get; set; }

    }
}
