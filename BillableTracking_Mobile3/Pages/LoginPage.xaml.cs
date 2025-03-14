using BillableTracking_Mobile3.ViewModels;

namespace BillableTracking_Mobile3.Pages;

public partial class LoginPage : ContentPage
{
    LoginViewModel _viewModel;

    public LoginPage(LoginViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
        _viewModel = viewModel;
    }
  
}