using BillableTracking_Mobile3.Models;
using MonkeyCache.FileStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3.Services
{
    public interface IUserService
    {
        Task<ExternalLoginResponse> LoginWithGoogleAsync(string token, string deviceName, string deviceId, string linkedSiteConfiguration);
        Task<ExternalLoginResponse> LoginWithMicrosoftAsync(string token, string deviceName, string deviceId, string linkedSiteConfiguration);
    }
    public class UserService : IUserService
    {
        private readonly IApiService _apiService;

        public UserService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<ExternalLoginResponse> LoginWithGoogleAsync(string token, string deviceName, string deviceId, string linkedSiteConfiguration)
        {
            var endpoint = "api/ExternalAuth/google-login";
            if (_apiService.IsOnline)
            {
                var requestData = new
                {
                    token,
                    deviceName,
                    deviceId,
                    linkedSiteConfiguration
                };
                var json = System.Text.Json.JsonSerializer.Serialize(requestData);
                var response = await _apiService.PostExternalLoginAsync<ExternalLoginResponse>(endpoint, requestData);

                await StoreUserDataAsync(response);
                Barrel.Current.Add<ExternalLoginResponse>(endpoint, response,TimeSpan.FromDays(365));
                Barrel.Current.Add<Models.UserRecord>("CachedUser", response.UserRecord,TimeSpan.FromDays(365));
                return response;
            }
            else
            {
                return Barrel.Current.Get<ExternalLoginResponse>(endpoint);
            }
          
        }

        public async Task<ExternalLoginResponse> LoginWithMicrosoftAsync(string token, string deviceName, string deviceId, string linkedSiteConfiguration)
        {
            var requestData = new
            {
                token,
                deviceName,
                deviceId,
                linkedSiteConfiguration
            };
            var response = await _apiService.PostExternalLoginAsync<ExternalLoginResponse>("api/ExternalAuth/microsoft-login", requestData);
            await StoreUserDataAsync(response);
            return response;
        }

        private async Task StoreUserDataAsync(ExternalLoginResponse response)
        {
            await SecureStorage.SetAsync("auth_token", response.Token);
            await SecureStorage.SetAsync("user_email", response.Email);
            await SecureStorage.SetAsync("user_name", response.Name);
            await SecureStorage.SetAsync("user_username", response.Username);
        }
    }
    public class ExternalLoginResponse
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public UserRecord? UserRecord { get; set; }
    }
}
