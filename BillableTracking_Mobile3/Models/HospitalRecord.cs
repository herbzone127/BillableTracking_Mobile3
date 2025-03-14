using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3.Models
{
    public class HospitalRecord
    {
        public string name { get; set; }
        public string providerNumber { get; set; }
        public string lspn { get; set; }
        public bool isVerified { get; set; }
        public string id { get; set; }
        public string createdByUserID { get; set; }
       
        public DateTime createdDate { get; set; }
        public string updatedByUserID { get; set; }
       
        public DateTime updatedDate { get; set; }
        public bool isDeleted { get; set; }
    }
}
