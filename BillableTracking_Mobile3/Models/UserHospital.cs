using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3.Models
{
    public class UserHospital
    {
        public string userProviderNumber { get; set; }
        public string hospitalID { get; set; }
        public string userID { get; set; }
        public string userSpeciality { get; set; }
        public string userTypeCode { get; set; }
        public string id { get; set; }
        public bool isDeleted { get; set; }
    }
}
