using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiAuth;
using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using BillableTracking_Mobile3.Models;
using BillableTracking_Mobile3.Services;
using AutoMapper;
using BillableTracking_Mobile3.Tables;
using Newtonsoft.Json;

namespace BillableTracking_Mobile3.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        // Status and Loading Properties
        [ObservableProperty]
        private string _statusMessage = "Please select a site and log in.";

        [ObservableProperty]
        private Color _statusColor = Colors.Gray;

        [ObservableProperty]
        private bool _isBusy = false;

        [ObservableProperty]
        private bool _isLoading = false;

        // Site Selection Properties
        [ObservableProperty]
        private ObservableCollection<string> _siteOptions = new ObservableCollection<string>();

        [ObservableProperty]
        private string _selectedSite;

        [ObservableProperty]
        private ObservableCollection<Models.SiteConfiguration> _siteConfigurations = new ObservableCollection<Models.SiteConfiguration>();

        // User Authentication Properties
        [ObservableProperty]
        private ExternalLoginResponse _currentUser;

        // Device Information Properties
        [ObservableProperty]
        private string _deviceName;

        [ObservableProperty]
        private string _deviceId;

        private TaskCompletionSource<object> _tcs;
        private readonly IUserService _userService;
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private readonly IDeviceInfoService _deviceInfoService;
        private readonly IDatabaseService _databaseService;
        private readonly IMapper _mapper;
        public LoginViewModel(IUserService userService, INavigationService navigationService,
            IApiService apiService, IDeviceInfoService deviceInfoService,
            IDatabaseService databaseService,IMapper mapper)
        {
            _userService = userService;
            _navigationService = navigationService;
            _apiService = apiService;
            _deviceInfoService = deviceInfoService;
            _mapper = mapper;
            // Initialize device information
            DeviceName = _deviceInfoService.GetDeviceName();
            DeviceId = _deviceInfoService.GetDeviceId();
            _databaseService = databaseService;
            LoadSiteConfigurationsAsync();
        }

        private async void LoadSiteConfigurationsAsync()
        {
            IsLoading = true;
            try
            {
                // Use GetUnauthenticatedAsync to fetch site configurations without a token
                var configurations = await _apiService.GetUnauthenticatedAsync<Models.SiteConfiguration>("api/SiteConfigurations");
                SiteConfigurations.Clear();
                SiteOptions.Clear();
                foreach (var config in configurations)
                {
                    SiteConfigurations.Add(config);
                    SiteOptions.Add(config.WebsiteAddress);
                }
                if (SiteOptions.Count > 0)
                {
                    SelectedSite = SiteOptions[0]; // Default to the first site
                }
                StatusMessage = SiteOptions.Count > 0 ? "Site configurations loaded successfully." : "No sites available.";
                StatusColor = Colors.Green;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading site configurations: {ex.Message}";
                StatusColor = Colors.Red;
            }
            finally
            {
                IsLoading = false;
                IsBusy = false;
            }
        }

        private async Task<object> SocialLogin(string provider)
        {
            if (string.IsNullOrEmpty(SelectedSite))
            {
                StatusMessage = "Please select a site before logging in.";
                StatusColor = Colors.Red;
                return null;
            }

            try
            {
                IsBusy = true;
                _tcs = new TaskCompletionSource<object>();

                // Find the selected site configuration to get the LinkedSiteConfiguration (SiteConfiguration.Id)
                var selectedConfig = SiteConfigurations.FirstOrDefault(c => c.WebsiteAddress == SelectedSite);
                if (selectedConfig == null)
                {
                    StatusMessage = "Selected site configuration not found.";
                    StatusColor = Colors.Red;
                    return null;
                }
                string linkedSiteConfiguration = selectedConfig.Id;

                OAuth2Authenticator authenticator;
                if (provider == "Google")
                {
                    authenticator = new OAuth2Authenticator(
                        clientId: "499023355628-6cr2dtg7u8e9qmdjtu9mtmcuiusj5upg.apps.googleusercontent.com",
                        clientSecret: null,
                        scope: "email profile",
                        authorizeUrl: new Uri("https://accounts.google.com/o/oauth2/v2/auth"),
                        redirectUrl: new Uri("com.googleusercontent.apps.499023355628-6cr2dtg7u8e9qmdjtu9mtmcuiusj5upg:/oauth2redirect"),
                        accessTokenUrl: new Uri("https://oauth2.googleapis.com/token")
                    );
                }
                else if (provider == "Microsoft")
                {
                    authenticator = new OAuth2Authenticator(
                        clientId: "790a53ff-db12-40ee-a7dd-c54f49da106f",
                        clientSecret: null,
                        scope: "openid profile User.Read",
                        authorizeUrl: new Uri("https://login.microsoftonline.com/common/oauth2/v2.0/authorize"),
                        redirectUrl: new Uri("msauth://com.billabletracking.mobile/RDRZClQyYa5m1aE9OsIqGQsK32A%3D"),
                        accessTokenUrl: new Uri("https://login.microsoftonline.com/common/oauth2/v2.0/token")
                    );
                }
                else
                {
                    throw new ArgumentException("Unsupported provider");
                }

                // Inside the SocialLogin method, update the navigation call
                authenticator.Authenticated += async (sender, e) =>
                {
                    try
                    {
                        Debug.WriteLine($"OAuth Authenticated for {provider} and Id_Token {e.IdToken}");
                        ExternalLoginResponse loginResponse = null;
                        if (provider == "Google")
                        {
                            loginResponse = await _userService.LoginWithGoogleAsync(e.IdToken, DeviceName, DeviceId, linkedSiteConfiguration);
                        }
                        else if (provider == "Microsoft")
                        {
                            loginResponse = await _userService.LoginWithMicrosoftAsync(e.IdToken, DeviceName, DeviceId, linkedSiteConfiguration);
                        }

                        CurrentUser = loginResponse;
                        Debug.WriteLine($"Login Response: {loginResponse?.Name}");

                        // Check if the user is verified
                        if (selectedConfig != null)
                        {
                            StatusMessage = $"Login successful with {provider}! Welcome, {loginResponse.Name}";
                            StatusColor = Colors.Green;
                            if (_navigationService == null)
                            {
                                StatusMessage = "Error: Navigation service not available.";
                                StatusColor = Colors.Red;
                                Debug.WriteLine("Error: NavigationService is null during navigation.");
                            }
                            else
                            {
                                try
                                {
                                    // Sync all relevant data types
                                    //await _apiService.SyncDataAsync<Models.UserRecord>("api/users");
                                    await _apiService.SyncDataAsync<Models.SiteConfiguration>("api/siteconfigurations");

                                    Debug.WriteLine("Attempting navigation to AccessStatusPage...");
                                    //await _navigationService.NavigateToAsync("AccessStatusPage");
                                   // await DebugCachedData();
                                    //await Shell.Current.Navigation.PopToRootAsync();
                                    Debug.WriteLine("Attempting navigation to accessstatuspage...");
                                    await Shell.Current.Navigation.PopToRootAsync();
                                    await _navigationService.NavigateToAsync("//AccessStatusPage", new Dictionary<string, object>
                        {
                            { "CurrentUser", loginResponse.UserRecord }
                        });
                                    Debug.WriteLine("Navigation to accessstatuspage completed successfully.");
                                }
                                catch (Exception navEx)
                                {
                                    StatusMessage = $"Navigation failed: {navEx.Message}";
                                    StatusColor = Colors.Red;
                                    Debug.WriteLine($"Navigation error: {navEx.Message}\nStackTrace: {navEx.StackTrace}");
                                }
                            }
                        }
                        else
                        {
                            StatusMessage = "Access denied. User is not verified or site not found.";
                            StatusColor = Colors.Red;
                            Debug.WriteLine("Access Denied: User not verified");
                        }
                        _tcs.TrySetResult(loginResponse);
                    }
                    catch (Exception ex)
                    {
                        StatusMessage = $"Error: {ex.Message}";
                        StatusColor = Colors.Red;
                        Debug.WriteLine($"Error in OAuth Authenticated: {ex.Message}");
                        _tcs.TrySetResult(null);
                    }
                };

                authenticator.Error += (sender, error) =>
                {
                    Debug.WriteLine($"{provider} authentication error: {error}");
                    StatusMessage = $"Error: {error}";
                    StatusColor = Colors.Red;
                    _tcs.TrySetResult(null);
                };

                var initialUrl = await authenticator.GetInitialUrlAsync();
                Debug.WriteLine($"Opening {provider} auth URL: {initialUrl}");
                await Launcher.OpenAsync(initialUrl);

                return await _tcs.Task;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{provider} login failed: {ex}");
                StatusMessage = $"Login failed: {ex.Message}";
                StatusColor = Colors.Red;
                _tcs?.TrySetResult(null);
                return null;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SignInGoogleAsync()
        {
            await SocialLogin("Google");
        }

        [RelayCommand]
        public async Task SignInMicrosoftAsync()
        {
            await SocialLogin("Microsoft");
        }

        private bool CanSignIn()
        {
            var canExecute = !IsBusy && !IsLoading && !string.IsNullOrEmpty(SelectedSite);
            Debug.WriteLine($"CanSignIn: IsBusy={IsBusy}, IsLoading={IsLoading}, SelectedSite={SelectedSite}, CanExecute={canExecute}");
            return canExecute;
        }
        private async Task DebugCachedData()
        {
            //var userRecords = await _apiService.GetAsync<Models.UserRecord>("api/users");
            //Debug.WriteLine($"Total UserRecords in Cache: {userRecords.Count}");
            //foreach (var user in userRecords)
            //{
            //    Debug.WriteLine($"UserRecord - Id: {user.Id}, Email: {user.Email}, IsVerified: {user.IsVerified}");
            //}

            var siteConfigs = await _apiService.GetAsync<Models.SiteConfiguration>("api/siteconfigurations");
            Debug.WriteLine($"Total SiteConfigurations in Cache: {siteConfigs.Count}");
            foreach (var site in siteConfigs)
            {
                Debug.WriteLine($"SiteConfiguration - Id: {site.Id}, WebsiteAddress: {site.WebsiteAddress}, UserRecordId: {site.UserId}, UserRecord: {site.UserRecord?.Id ?? "null"}");
            }
        }
    }
}