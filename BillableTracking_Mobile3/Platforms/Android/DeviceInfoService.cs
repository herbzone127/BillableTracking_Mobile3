using System;

namespace BillableTracking_Mobile3.Services
{
    public partial class DeviceInfoService : IDeviceInfoService
    {
        public string GetDeviceName()
        {
            try
            {
#if ANDROID
                return Android.OS.Build.Model; // Retrieves the device model (e.g., "Pixel 6")
#else
                return DeviceInfo.Model; // Fallback for other platforms (MAUI's DeviceInfo)
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving device name: {ex.Message}");
                return "Unknown Device";
            }
        }

        public string GetDeviceId()
        {
            try
            {
#if ANDROID
                var context = Android.App.Application.Context;
                var deviceId = Android.Provider.Settings.Secure.GetString(context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                return deviceId ?? "Unknown Device ID";
#else
                return DeviceInfo.Idiom.ToString(); // Fallback for other platforms (MAUI's DeviceInfo)
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving device ID: {ex.Message}");
                return "Unknown Device ID";
            }
        }

        public async Task<bool> IsConnectedToInternetAsync()
        {
            return Connectivity.NetworkAccess == NetworkAccess.Internet;
        }
    }
}