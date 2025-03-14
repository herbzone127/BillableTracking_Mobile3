using BillableTracking_Mobile3.Models;
using BillableTracking_Mobile3.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyCache.FileStore;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3.ViewModels
{
    /// <summary>
    /// ViewModel for handling patient billing operations.
    /// </summary>
    public partial class PatientBillingViewModel : ObservableObject, IParameterizedViewModel
    {
        [ObservableProperty]
        private string visitDate;

        [ObservableProperty]
        private string patientName;

        [ObservableProperty]
        private string patientId;

        [ObservableProperty]
        private string insurance;

        [ObservableProperty]
        private ObservableCollection<BillableItem> billableItems;

        [ObservableProperty]
        private BillableItem selectedBillableItem;

        [ObservableProperty]
        private string notes;

        [ObservableProperty]
        private string selectedService;

        [ObservableProperty]
        private string serviceCode;

        [ObservableProperty]
        private string serviceAmount;

        [ObservableProperty]
        private string syncStatus = "Last synced: 5 minutes ago\n2 pending";

        private readonly IApiService _apiService;
        private PatientNewRecord _patientRecord; // Store the patient record

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientBillingViewModel"/> class.
        /// </summary>
        /// <param name="apiService">The API service for making network requests.</param>
        public PatientBillingViewModel(IApiService apiService)
        {
            _apiService = apiService;
            BillableItems = new ObservableCollection<BillableItem>();
            LoadBillableItemsAsync();
            UpdatePendingCount(); // Initialize pending count
        }

        /// <summary>
        /// Sets the parameter for the ViewModel.
        /// </summary>
        /// <param name="parameter">The parameter to set, expected to be of type <see cref="PatientNewRecord"/>.</param>
        public void SetParameter(object parameter)
        {
            if (parameter is PatientNewRecord patientRecord)
            {
                _patientRecord = patientRecord;
                UpdatePatientInformation();
            }
        }

        /// <summary>
        /// Updates the patient information based on the provided patient record.
        /// </summary>
        private void UpdatePatientInformation()
        {
            if (_patientRecord != null)
            {
                PatientName = $"{_patientRecord.lastName}";
                PatientId = _patientRecord.id ?? $"UR-{_patientRecord.unitRecord}";
                Insurance = _patientRecord.insuranceCompanyRecord?.name ?? "Unknown Insurance";
            }
        }

        /// <summary>
        /// Loads the billable items asynchronously from the API.
        /// </summary>
        private async Task LoadBillableItemsAsync()
        {
            try
            {
                var items = await _apiService.GetAsync<BillableItem>("api/BillableItems");
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        BillableItems.Add(item);
                    }
                    if (BillableItems.Any())
                    {
                        SelectedBillableItem = BillableItems[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading billable items: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles changes to the selected billable item.
        /// </summary>
        /// <param name="value">The newly selected billable item.</param>
        partial void OnSelectedBillableItemChanged(BillableItem value)
        {
            if (value != null)
            {
                SelectedService = value.itemName;
                ServiceCode = value.itemCode;
                ServiceAmount = $"${value.itemPrice:F2}";
            }
        }

        /// <summary>
        /// Saves the billing record asynchronously.
        /// </summary>
        [RelayCommand]
        private async Task SaveRecord()
        {
            try
            {
                var user = Barrel.Current.Get<UserRecord>("CachedUser");
                var billableItemEvent = new BillableItemEvent
                {
                    doctorID = user?.Id ?? "Unknown",
                    userHospitalID = "DefaultHospital",
                    patientID = _patientRecord?.id ?? $"UR-{_patientRecord.unitRecord}",
                    selectedItemID = SelectedBillableItem?.id,
                    eventDate = DateTime.Now,
                    eventTime = DateTime.Now.ToString("HH:mm:ss"),
                    eventItemPrice = double.TryParse(ServiceAmount.Replace("$", ""), out double price) ? price : 0.0,
                    notes = Notes,
                    serviceText = SelectedService,
                    invoiced = false,
                    batchID = null,
                    eclipseInvoiceStatus = "Pending",
                    id = Guid.NewGuid().ToString(),
                    createdByUserID = user?.Id ?? "System",
                    createdDate = DateTime.Now,
                    updatedByUserID = user?.Id ?? "System",
                    updatedDate = DateTime.Now,
                    isDeleted = false
                };

                if (_apiService.IsOnline)
                {
                    var response = await _apiService.PostAsync<BillableItemEvent>("api/BillableItemEvents", billableItemEvent);
                    if (response != null)
                    {
                        await Application.Current.MainPage.DisplayAlert("Success", "Billing record saved", "OK");
                        Notes = string.Empty;
                        SyncStatus = "Last synced: Just now\n0 pending";
                    }
                }
                else
                {
                    // Offline: Save to Barrel with a unique key
                    var key = $"pending_{billableItemEvent.id}";
                    var jsonData = JsonConvert.SerializeObject(billableItemEvent);
                    Barrel.Current.Add(key, jsonData, TimeSpan.FromDays(30)); // Cache for 30 days
                    await Application.Current.MainPage.DisplayAlert("Offline", "Record saved locally. Sync when online.", "OK");
                    Notes = string.Empty;
                    UpdatePendingCount(); // Update the pending count in SyncStatus
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save billing record: {ex.Message}", "OK");
                Console.WriteLine($"Error saving billable item event: {ex.Message}");
            }
        }

        /// <summary>
        /// Synchronizes the pending records with the server asynchronously.
        /// </summary>
        [RelayCommand]
        private async Task SyncNow()
        {
            if (!_apiService.IsOnline)
            {
                await Application.Current.MainPage.DisplayAlert("Offline", "No internet connection. Cannot sync.", "OK");
                return;
            }

            try
            {
                // Get all pending records from Barrel
                var pendingKeys = Barrel.Current.GetKeys().Where(k => k.StartsWith("pending_")).ToList();
                if (!pendingKeys.Any())
                {
                    SyncStatus = "Last synced: Just now\n0 pending";
                    return;
                }

                foreach (var key in pendingKeys)
                {
                    var jsonData = Barrel.Current.Get<string>(key);
                    if (!string.IsNullOrEmpty(jsonData))
                    {
                        var billableItemEvent = JsonConvert.DeserializeObject<BillableItemEvent>(jsonData);
                        var response = await _apiService.PostAsync<BillableItemEvent>("api/BillableItemEvents", billableItemEvent);
                        if (response != null)
                        {
                            // Successfully synced, remove from cache
                            Barrel.Current.Empty(key);
                            Console.WriteLine($"Synced and removed pending record: {key}");
                        }
                    }
                }

                SyncStatus = "Last synced: Just now\n0 pending";
                await Application.Current.MainPage.DisplayAlert("Success", "All pending records synced.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Sync failed: {ex.Message}", "OK");
                Console.WriteLine($"Sync error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the pending count in the sync status.
        /// </summary>
        private void UpdatePendingCount()
        {
            var pendingCount = Barrel.Current.GetKeys().Count(k => k.StartsWith("pending_"));
            SyncStatus = $"Last synced: {DateTime.Now.ToString("HH:mm:ss")}\n{pendingCount} pending";
        }
    }
}
