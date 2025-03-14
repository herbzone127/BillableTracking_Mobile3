using BillableTracking_Mobile3.Models;
using BillableTracking_Mobile3.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyCache.FileStore;
using Newtonsoft.Json;
using Plugin.Maui.OCR;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3.ViewModels;

public partial class NewPatientRegistrationViewModel : ObservableObject, IParameterizedViewModel
{
    [ObservableProperty]
    private ObservableCollection<InsuranceCompanyRecord> insuranceCompanies = new ObservableCollection<InsuranceCompanyRecord>();

    [ObservableProperty]
    private InsuranceCompanyRecord selectedInsuranceCompany;

    [ObservableProperty]
    private string unitRecord;

    [ObservableProperty]
    private string admissionRecord;

    [ObservableProperty]
    private string syncStatus = "Last synced: 5 minutes ago\n0 pending"; // Added sync status

    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;

    public NewPatientRegistrationViewModel(IApiService apiService, INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
        LoadInsuranceCompanyData();
        UpdatePendingCount(); // Initialize pending count
    }

    private async void LoadInsuranceCompanyData()
    {
        try
        {
            var records = await _apiService.GetAsync<InsuranceCompanyRecord>("api/InsuranceCompany");
            InsuranceCompanies.Clear();
            foreach (var record in records)
            {
                InsuranceCompanies.Add(record);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading insurance companies: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task Capture()
    {
        await ScanBarcode();
    }

    [RelayCommand]
    private async Task Scan()
    {
        string action = await Application.Current.MainPage.DisplayActionSheet(
            "Choose an option",
            "Cancel",
            null,
            "Take Photo",
            "Pick Photo");

        byte[] imageBytes = null;
        if (action == "Take Photo")
        {
            imageBytes = await TakePhoto();
        }
        else if (action == "Pick Photo")
        {
            imageBytes = await PickPhoto();
        }

        if (imageBytes != null && imageBytes.Length > 0)
        {
            await ProcessImageForOcr(imageBytes);
        }
    }

    private async Task<byte[]> TakePhoto()
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo != null)
            {
                using var stream = await photo.OpenReadAsync();
                var imageBytes = new byte[stream.Length];
                await stream.ReadAsync(imageBytes, 0, (int)stream.Length);
                return imageBytes;
            }
        }
        return null;
    }

    private async Task<byte[]> PickPhoto()
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images,
            PickerTitle = "Pick an image"
        });
        if (result != null)
        {
            using var stream = await result.OpenReadAsync();
            var imageBytes = new byte[stream.Length];
            await stream.ReadAsync(imageBytes, 0, (int)stream.Length);
            return imageBytes;
        }
        return null;
    }

    private async Task ProcessImageForOcr(byte[] imageBytes)
    {
        try
        {
            using var imageAsStream = new MemoryStream(imageBytes);
            var imageAsBytes = new byte[imageAsStream.Length];
            await imageAsStream.ReadAsync(imageAsBytes);

            var ocrResult = await OcrPlugin.Default.RecognizeTextAsync(imageAsBytes);
            if (!string.IsNullOrEmpty(ocrResult?.AllText))
            {
                ParseOCRResult(ocrResult.AllText);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No text detected in the image.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Failed to process image: {ex.Message}", "OK");
        }
    }

    private void ParseOCRResult(string ocrText)
    {
        if (!ocrText.Contains("Adm No"))
        {
            Application.Current.MainPage.DisplayAlert("Error", "Patient record not found in the image.", "OK");
            return;
        }

        var lines = ocrText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            if (line.StartsWith("UR:"))
            {
                UnitRecord = $"UR-{line.Replace("UR:", "").Trim()}";
            }
            else if (line.StartsWith("Adm No:"))
            {
                AdmissionRecord = $"ADM-{line.Replace("Adm No:", "").Trim()}";
            }
        }
    }

    public async Task ScanBarcode()
    {
        // Placeholder for future implementation
    }

    public void SetParameter(object parameter)
    {
        if (parameter is Dictionary<string, string> paramDict)
        {
            if (paramDict.TryGetValue("UnitRecordNumber", out string unitRecord))
                UnitRecord = $"UR-{unitRecord}";
            if (paramDict.TryGetValue("AdmissionNumber", out string admission))
                AdmissionRecord = $"ADM-{admission}";
        }
    }

    [RelayCommand]
    private async Task SaveRecord()
    {
        try
        {
            if (SelectedInsuranceCompany?.id == null)
            {
                await Application.Current.MainPage.DisplayAlert("ERROR", "Please select insurance company", "OK");
                return;
            }

            var user = Barrel.Current.Get<UserRecord>("CachedUser");
            var patientRecord = new PatientNewRecord
            {
                id = Guid.NewGuid().ToString(),
                unitRecord = UnitRecord,
                admissionNumber = AdmissionRecord,
                insuranceCompanyID = SelectedInsuranceCompany?.id,
                isDeleted = false,
                lastName = UnitRecord, // Assuming lastName is derived from unitRecord for now
            };

            if (_apiService.IsOnline)
            {
                var response = await _apiService.PostAsync<PatientNewRecord>("api/PatientsNew", patientRecord);
                if (response != null)
                {
                    UnitRecord = string.Empty;
                    AdmissionRecord = string.Empty;
                    SelectedInsuranceCompany = null;
                    await Application.Current.MainPage.DisplayAlert("Success", "Patient record saved", "OK");
                    await _navigationService.NavigateToAsync("//MainPage");
                }
            }
            else
            {
                // Offline: Save to Barrel with a unique key
                var key = $"pending_patient_{patientRecord.id}";
                var jsonData = JsonConvert.SerializeObject(patientRecord);
                Barrel.Current.Add(key, jsonData, TimeSpan.FromDays(30)); // Cache for 30 days
                await Application.Current.MainPage.DisplayAlert("Offline", "Record saved locally. Sync when online.", "OK");
                UnitRecord = string.Empty;
                AdmissionRecord = string.Empty;
                SelectedInsuranceCompany = null;
                UpdatePendingCount(); // Update the pending count in SyncStatus
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving patient record: {ex.Message}");
            await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save record: {ex.Message}", "OK");
        }
    }

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
            // Get all pending patient records from Barrel
            var pendingKeys = Barrel.Current.GetKeys().Where(k => k.StartsWith("pending_patient_")).ToList();
            if (!pendingKeys.Any())
            {
                SyncStatus = "Last synced: Just now\n0 pending";
                await Application.Current.MainPage.DisplayAlert("Info", "No pending records to sync.", "OK");
                return;
            }

            foreach (var key in pendingKeys)
            {
                var jsonData = Barrel.Current.Get<string>(key);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    var patientRecord = JsonConvert.DeserializeObject<PatientNewRecord>(jsonData);
                    var response = await _apiService.PostAsync<PatientNewRecord>("api/PatientsNew", patientRecord);
                    if (response != null)
                    {
                        // Successfully synced, remove from cache
                        Barrel.Current.Remove(key);
                        Console.WriteLine($"Synced and removed pending patient record: {key}");
                    }
                }
            }

            SyncStatus = "Last synced: Just now\n0 pending";
            await Application.Current.MainPage.DisplayAlert("Success", "All pending patient records synced.", "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Sync failed: {ex.Message}", "OK");
            Console.WriteLine($"Sync error: {ex.Message}");
        }
    }

    private void UpdatePendingCount()
    {
        var pendingCount = Barrel.Current.GetKeys().Count(k => k.StartsWith("pending_patient_"));
        SyncStatus = $"Last synced: {DateTime.Now.ToString("HH:mm:ss")}\n{pendingCount} pending";
    }
}