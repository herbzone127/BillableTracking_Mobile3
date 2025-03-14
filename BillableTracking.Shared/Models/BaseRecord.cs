using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BillableTracking.Shared.Models
{
    public class BaseRecord
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("CreatedByUserRecord")]
        public string? CreatedByUserID { get; set; }
        public virtual User? CreatedByUserRecord { get; set; }
        [JsonIgnore]
        private DateTime? createdDate;
        public DateTime? CreatedDate
        {
            get
            {
                if (createdDate != null)
                {
                    return createdDate.Value.ToLocalTime();
                };
                return null;
            }
            set
            {
                if (value != null)
                {
                    createdDate = value.Value.ToUniversalTime();
                }
                else
                {
                    createdDate = value;
                }
            }
        }

        [ForeignKey("UpdatedByUserRecord")]
        public string? UpdatedByUserID { get; set; }
        public virtual User? UpdatedByUserRecord { get; set; }
        [JsonIgnore]
        private DateTime? updatedDate;
        public DateTime? UpdatedDate
        {
            get
            {
                if (updatedDate != null)
                {
                    return updatedDate.Value.ToLocalTime();
                };
                return null;
            }
            set
            {
                if (value != null)
                {
                    updatedDate = value.Value.ToUniversalTime();
                }
                else
                {
                    updatedDate = value;
                }
            }
        }

        // Soft Delete
        [ForeignKey("DeletedBy")]
        public string? DeletedByUserID { get; set; }
        public User? DeletedByUserRecord { get; set; }
        [JsonIgnore]
        private DateTime? deletedDate;
        public DateTime? DeletedDate
        {
            get
            {
                if (deletedDate != null)
                {
                    return deletedDate.Value.ToLocalTime();
                };
                return null;
            }
            set
            {
                if (value != null)
                {
                    deletedDate = value.Value.ToUniversalTime();
                }
                else
                {
                    deletedDate = value;
                }
            }
        }
        public bool IsDeleted { get; set; } = false;

        public void TimeStampCreate(User user)
        {
            CreatedByUserID = user.Id;
            // if (CreatedByUserRecord != null) CreatedByUserRecord = null;
            //CreatedByUserRecord = user;
            CreatedDate = DateTime.UtcNow;

            UpdatedByUserID = user.Id;
            // UpdatedByUserRecord = user;
            UpdatedDate = DateTime.UtcNow;
        }

        public void TimeStampUpdate(User user)
        {
            if (UpdatedByUserID != user.Id)
            {
                UpdatedByUserID = user.Id;
                //UpdatedByUserRecord = user;                
            }
            UpdatedDate = DateTime.UtcNow;
        }

        public void SoftDelete(User user)
        {
            DeletedByUserID = user.Id;
            //DeletedByUserRecord = user;
            DeletedDate = DateTime.UtcNow;
            IsDeleted = true;
        }
    }
}
