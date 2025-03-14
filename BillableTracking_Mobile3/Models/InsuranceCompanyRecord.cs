using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3.Models
{
 public   class InsuranceCompanyRecord
    {
        public string id { get; set; }
        public string createdByUserID { get; set; }
       
        public DateTime createdDate { get; set; }
        public string updatedByUserID { get; set; }
        
        public DateTime updatedDate { get; set; }
        public string deletedByUserID { get; set; }
        
        public DateTime deletedDate { get; set; }
        public bool isDeleted { get; set; }
        public string name { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string postCode { get; set; }
        public string abn { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);



  



}
