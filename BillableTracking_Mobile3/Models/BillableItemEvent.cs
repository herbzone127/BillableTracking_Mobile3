using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3.Models
{
    public class BillableItemEvent
    {
        public string doctorID { get; set; }
        public string userHospitalID { get; set; }
        public UserHospital userHospital { get; set; }
        public string patientID { get; set; }
        public string selectedItemID { get; set; }
        public SelectedItem selectedItem { get; set; }
        public DateTime eventDate { get; set; }
        public string eventTime { get; set; }
        public double eventItemPrice { get; set; }
        public string notes { get; set; }
        public string serviceText { get; set; }
        public bool invoiced { get; set; }
        public string batchID { get; set; }
        public string eclipseInvoiceStatus { get; set; }
        public string id { get; set; }
        public string createdByUserID { get; set; }
       
        public DateTime createdDate { get; set; }
        public string updatedByUserID { get; set; }
       
        public DateTime updatedDate { get; set; }
        public bool isDeleted { get; set; }
    }
}
