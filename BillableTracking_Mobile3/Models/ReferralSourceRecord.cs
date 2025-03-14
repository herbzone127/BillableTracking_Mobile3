using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3.Models
{
    public class ReferralSourceRecord
    {
        public string fullName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string servicelType { get; set; }
        public string address1 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string postcode { get; set; }
        public string providerNumber { get; set; }
        public string phoneNumber { get; set; }
        public string mobileNumber { get; set; }
        public string companyName { get; set; }
        public string abn { get; set; }
        public string id { get; set; }
        public string createdByUserID { get; set; }
        
        public DateTime createdDate { get; set; }
        public string updatedByUserID { get; set; }
      
        public DateTime updatedDate { get; set; }
        public bool isDeleted { get; set; }
    }

}
