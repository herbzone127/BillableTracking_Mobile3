using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3.Models
{
    public class SelectedItem
    {
        public string insuranceCompanyID { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public int itemPrice { get; set; }
        public string id { get; set; }
        public bool isDeleted { get; set; }
    }
}
