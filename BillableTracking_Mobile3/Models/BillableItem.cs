using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3.Models
{
    public class BillableItem
    {
        public string insuranceCompanyID { get; set; }
        public InsuranceCompanyRecord insuranceCompanyRecord { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public double itemPrice { get; set; }
        public string id { get; set; }
        public string createdByUserID { get; set; }
      
        public DateTime createdDate { get; set; }
        public string updatedByUserID { get; set; }
        
        public DateTime updatedDate { get; set; }
        public bool isDeleted { get; set; }
    }
}
