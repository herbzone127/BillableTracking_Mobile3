using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3.Models
{
    public class PatientRecord
    {
        public string name { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string insuranceCompanyID { get; set; }
        public InsuranceCompanyRecord insuranceCompanyRecord { get; set; }
        public string insuranceMemberNumber { get; set; }
        public string medicareNumber { get; set; }
        public int medicareIndRef { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string postCode { get; set; }
        public DateTime dateOfBirth { get; set; }
        public DateTime startDate { get; set; }
        public DateTime admissionDate { get; set; }
        public string unitRecord { get; set; }
        public string admissionNumber { get; set; }
        public bool accidentIndicator { get; set; }
        public string accidentIndicatorStr { get; set; }
        public string serviceTypeCode { get; set; }
        public string claimTypeCode { get; set; }
        public bool compensationIndicator { get; set; }
        public string compensationIndicatorStr { get; set; }
        public bool financialInsterestDisclosureIndicator { get; set; }
        public string financialInsterestDisclosureIndicatorStr { get; set; }
        public string ifcIssueCode { get; set; }
        public string referralID { get; set; }
        public ReferralSourceRecord referralSourceRecord { get; set; }
        public DateTime referralDate { get; set; }
        public int referralPeriod { get; set; }
        public string referralPeriodCode { get; set; }
        public string referralOverRideCode { get; set; }
        public string hospitalID { get; set; }
        public HospitalRecord hospitalRecord { get; set; }
        public List<BillableItemEvent> billableItemEvents { get; set; }
        public string id { get; set; }
        public string createdByUserID { get; set; }
       
        public DateTime createdDate { get; set; }
        public string updatedByUserID { get; set; }
        
        public DateTime updatedDate { get; set; }
        public bool isDeleted { get; set; }
    }
}
