using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BillableTracking.Shared.Models
{


    public class BillableItemRecord : BaseRecord
    {
        [ForeignKey("InsuranceCompanyRecord")]
        public string InsuranceCompanyID { get; set; } = string.Empty;
        public virtual InsuranceCompanyRecord? InsuranceCompanyRecord { get; set; }

        [Required(ErrorMessage = "Item Code is required")]
        [MaxLength(20)]
        public string ItemCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Item Name is required")]
        [MaxLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Item Price is required")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ItemPrice { get; set; }

        public BillableItemRecord UpdateRecord(BillableItemRecord record)
        {
            this.InsuranceCompanyID = record.InsuranceCompanyID;
            this.ItemCode = record.ItemCode;
            this.ItemName = record.ItemName;
            this.ItemPrice = record.ItemPrice;
            return this;
        }
    }

}
