using BillableTracking_Mobile3.ViewModels;

namespace BillableTracking_Mobile3.Pages;

public partial class PatientBillingPage : ContentPage
{
	PatientBillingViewModel _viewModel;
    public PatientBillingPage(ViewModels.PatientBillingViewModel patientBillingViewModel)
	{
		InitializeComponent();
		BindingContext= _viewModel = patientBillingViewModel;
	}
}