using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3.Tables
{
  [Table("SiteConfiguration")]
  public  class SiteConfiguration
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Company { get; set; }
        public string Abn { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PostCode { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountBSB { get; set; }
        public string WebsiteAddress { get; set; }
        public string WebsiteIPAddress { get; set; }
        public string Message { get; set; }
        
        public string UserId { get; set; }
        public string LogoFileReference { get; set; }
       
        public string PrincipalProviderNumber { get; set; }
        public string NextInvoiceNumber { get; set; }
    }
}
