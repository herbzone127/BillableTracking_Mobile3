using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BillableTracking_Mobile3.Models;
using BillableTracking_Mobile3.Services;
using Newtonsoft.Json;
using System.Web;

namespace BillableTracking_Mobile3.ViewModels
{
   
    public partial class AccessStatusViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private string _statusMessage = "Checking access status...";

        [ObservableProperty]
        private Color _statusBackgroundColor = Colors.Gray;

        [ObservableProperty]
        private string _statusIcon = "checkmark.png"; // Default icon

        [ObservableProperty]
        private string _statusDescription = "Checking your access status...";

        [ObservableProperty]
        private bool _isBusy = false;

        [ObservableProperty]
        private bool _isVerified = false; // Tracks verification status for enabling/disabling Continue button

        private readonly IApiService _apiService;
        private readonly INavigationService _navigationService;
        //private readonly IUserSessionService _userSessionService;
        private Models.UserRecord _currentUser;

        public Models.UserRecord CurrentUser
        {
            get { return _currentUser; }
            set { _currentUser = value; }
        }


        public AccessStatusViewModel(IApiService apiService, INavigationService navigationService)
        {
            _apiService = apiService;
            _navigationService = navigationService;
            //_userSessionService = userSessionService;
            
        }

        //public void SetUserEmail(UserRecord user)
        //{
        //    user = userEmail;
        //    //_currentUser = _userSessionService.GetCurrentUser();
        //    if (_currentUser == null || _currentUser.Email != _userEmail)
        //    {
        //        _currentUser = null; // Invalidate if email doesn't match
        //    }
        //    LoadAccessStatusAsync();
        //}

        private async void LoadAccessStatusAsync()
        {
            IsBusy = true;
            try
            {
                if (CurrentUser != null)
                {
                    
                    IsVerified = CurrentUser.IsVerified;
                    StatusMessage = IsVerified ? "Approved" : "Unapproved";
                    StatusBackgroundColor = IsVerified ? Color.FromArgb("#E6F3E6") : Color.FromArgb("#F3E6E6");
                    StatusIcon = IsVerified ? "checkmark.png" : "cross.png";
                    StatusDescription = IsVerified
                        ? "You have been granted access"
                        : "Access denied. Please contact support.";
                }
               IsBusy = false;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error checking access: {ex.Message}";
                StatusBackgroundColor = Color.FromArgb("#F3E6E6");
                StatusIcon = "cross.png";
                StatusDescription = "An error occurred. Please try again.";
                IsVerified = false;
                Debug.WriteLine($"Access status error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task ContinueAsync()
        {
            if (!IsVerified)
            {
                StatusMessage = "Access denied.";
                StatusDescription = "Cannot continue without verification.";
                return;
            }

            try
            {
                await _navigationService.NavigateToAsync("//MainPage");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Navigation failed: {ex.Message}";
                StatusBackgroundColor = Color.FromArgb("#F3E6E6");
                StatusDescription = "Please try again.";
                Debug.WriteLine($"Navigation error: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task LogoutAsync()
        {
            IsBusy = true;
            try
            {
                 SecureStorage.RemoveAll();
                //await SecureStorage.Remove("auth_token");
                //await SecureStorage.Remove("user_email");
                // _userSessionService.ClearCurrentUser(); // Correct service
                await _navigationService.NavigateToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Logout failed: {ex.Message}";
                StatusBackgroundColor = Color.FromArgb("#F3E6E6");
                StatusDescription = "Please try again.";
                Debug.WriteLine($"Logout error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            CurrentUser = query["CurrentUser"]as Models.UserRecord;
            LoadAccessStatusAsync();
        }
    }
}