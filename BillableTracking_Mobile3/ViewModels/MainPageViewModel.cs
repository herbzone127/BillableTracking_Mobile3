using BillableTracking_Mobile3.Models;
using BillableTracking_Mobile3.Pages;
using BillableTracking_Mobile3.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.OCR;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string unitRecordNumber;

    [ObservableProperty]
    private string unitRecordNumberDisplay;

    [ObservableProperty]
    private string admissionNumberDisplay;

    [ObservableProperty]
    private bool isPatientFound;

    [ObservableProperty]
    private bool isNewPatientRegistrationEnabled = true;

    private string _admNo;
    private PatientNewRecord _patientRecord; // Store the retrieved patient record

    public MainPageViewModel(INavigationService navigationService, IApiService apiService)
    {
        _navigationService = navigationService;
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task ProceedToPatientRecord()
    {
        // Pass the retrieved PatientRecord to PatientBillingViewModel
        await _navigationService.NavigateToAsync<PatientBillingViewModel>(_patientRecord);
    }

    [RelayCommand]
    private async Task NewPatientRegistration()
    {
        var parameters = new Dictionary<string, string>
        {
            { "UnitRecordNumber", UnitRecordNumber ?? "" },
            { "AdmissionNumber", _admNo ?? "" }
        };
        await _navigationService.NavigateToAsync<NewPatientRegistrationViewModel>(parameters);
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
                await CheckPatientRecord();
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
                UnitRecordNumber = line.Replace("UR:", "").Trim();
                UnitRecordNumberDisplay = $"UNIT RECORD NUMBER\nURN-{UnitRecordNumber}";
            }
            else if (line.StartsWith("Adm No"))
            {
                _admNo = line.Replace("Adm No.", "").Trim();
                AdmissionNumberDisplay = $"ADMISSION NUMBER\nADM-{_admNo}";
            }
        }

        IsPatientFound = true;
    }

    private async Task CheckPatientRecord()
    {
        try
        {
            string endpoint = $"api/PatientsNew/unitrecord?unitrecord=UR-{UnitRecordNumber}";
            _patientRecord = await _apiService.GetSingleAsync<PatientNewRecord>(endpoint);

            if (_patientRecord != null)
            {
                IsNewPatientRegistrationEnabled = false;
                await Application.Current.MainPage.DisplayAlert(
                    "Info",
                    $"Patient with Unit Record UR-{UnitRecordNumber} already exists.",
                    "OK");
            }
            else
            {
                IsNewPatientRegistrationEnabled = true;
            }
        }
        catch (Exception ex)
        {
            IsNewPatientRegistrationEnabled = true;
            System.Diagnostics.Debug.WriteLine($"Error checking patient record: {ex.Message}");
        }
    }
}