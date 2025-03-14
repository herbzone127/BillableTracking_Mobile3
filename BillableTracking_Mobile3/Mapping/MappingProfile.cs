using AutoMapper;
using BillableTracking_Mobile3.Models;
using BillableTracking_Mobile3.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map ExternalLoginResponse to UserRecord
            CreateMap<Tables.UserRecord, Models.UserRecord>();
            CreateMap<Models.UserRecord, Tables.UserRecord>();

            // Map ExternalLoginResponse to SiteConfiguration
            CreateMap<Tables.SiteConfiguration, Models.SiteConfiguration>(); 
            CreateMap<Models.SiteConfiguration, Tables.SiteConfiguration>();
        }
    }
}
