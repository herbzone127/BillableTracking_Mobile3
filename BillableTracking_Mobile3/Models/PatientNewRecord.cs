using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3.Models
{
    public class PatientNewRecord
    {
        public string lastName { get; set; }
        public string insuranceCompanyID { get; set; }
        public string unitRecord { get; set; }
        public string admissionNumber { get; set; }
        public string id { get; set; }
        public bool isDeleted { get; set; }
        public InsuranceCompanyRecord? insuranceCompanyRecord { get; set; }
    }
}
