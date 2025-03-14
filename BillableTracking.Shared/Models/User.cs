using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BillableTracking.Shared.Models;

public class User : IdentityUser
{
    public User()
    {
        SiteConfigurations = new HashSet<SiteConfiguration>();
    }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsSuperAdmin { get; set; } = false;
    public string? Provider { get; set; } = null!;
    public string? ApprovedUserDeviceName { get; set; }
    public bool IsVerified { get; set; } = false;
    [JsonIgnore]
    public virtual ICollection<SiteConfiguration> SiteConfigurations { get; set; }
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

}

