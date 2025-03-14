using BillableTracking_Mobile3.Services;
using BillableTracking_Mobile3.ViewModels;
using Newtonsoft.Json;

namespace BillableTracking_Mobile3.Pages;

public partial class AccessStatusPage : ContentPage
{
    AccessStatusViewModel _viewModel;
    public AccessStatusPage(AccessStatusViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel= viewModel;
    }
 
    
}