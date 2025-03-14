using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3.Services
{
    public interface IDeviceInfoService
    {
        string GetDeviceName();
        string GetDeviceId();
        Task<bool> IsConnectedToInternetAsync();
    }
}