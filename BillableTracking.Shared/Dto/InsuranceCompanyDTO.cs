using BillableTracking.Shared.Models;

namespace BillableTracking.Shared.Dto
{
    public class InsuranceCompanyDTO
    {
        public string ID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = "New South Wales";
        public string Country { get; set; } = "Australia";
        public string PostCode { get; set; } = string.Empty;
        public string ABN { get; set; } = string.Empty;


        public InsuranceCompanyRecord MapFromDTO()
        {
            InsuranceCompanyRecord returnRecord = new();
            // Map DTO to new record
            returnRecord.ID = this.ID;
            returnRecord.Name = this.Name;
            returnRecord.Address1 = this.Address1;
            returnRecord.Address2 = this.Address2;
            returnRecord.City = this.City;
            returnRecord.State = this.State;
            returnRecord.Country = this.Country;
            returnRecord.PostCode = this.PostCode;
            returnRecord.ABN = this.ABN;

            return returnRecord;
        }
        public void MapToDTO(InsuranceCompanyRecord record)
        {
            // Map record to DTO
            this.ID = record.ID;
            this.Name = record.Name;
            this.Address1 = record.Address1;
            this.Address2 = record.Address2;
            this.City = record.City;
            this.State = record.State;
            this.Country = record.Country;
            this.PostCode = record.PostCode;
            this.ABN = record.ABN;
        }
    }
}
