using BillableTracking_Mobile3.ViewModels;

namespace BillableTracking_Mobile3.Pages;

public partial class NewPatientRegistrationPage : ContentPage
{
    public NewPatientRegistrationViewModel ViewModel { get; set; }
    public NewPatientRegistrationPage(NewPatientRegistrationViewModel viewModel)
	{
		InitializeComponent();
        ViewModel = viewModel;
        BindingContext = ViewModel;
    }
}