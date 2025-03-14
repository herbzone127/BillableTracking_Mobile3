using BillableTracking_Mobile3.ViewModels;

namespace BillableTracking_Mobile3
{
    public partial class MainPage : ContentPage
    {

        MainPageViewModel _viewModel;
        public MainPage(MainPageViewModel mainPageViewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = mainPageViewModel;
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            lblPlaceHolder.IsVisible = false;
            img.IsVisible = true;
            _viewModel.ScanCommand.Execute(null);
        }
    }

}
