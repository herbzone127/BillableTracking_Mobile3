using System;
using System.Threading.Tasks;
using System.Web;
using BillableTracking_Mobile3.Pages;
using BillableTracking_Mobile3.ViewModels;
using Microsoft.Maui.Controls;

namespace BillableTracking_Mobile3.Services
{
    public interface IParameterizedViewModel
    {
        void SetParameter(object parameter);
    }
    public interface INavigationService
    {
        Task NavigateToAsync(string route);
        Task NavigateToAsync(string route, IDictionary<string, object> parameters);
        Task NavigateToAsync<TViewModel>(object parameter = null) where TViewModel : class;
    }

    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task NavigateToAsync(string route)
        {
            try
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.GoToAsync(route);
                }
                else
                {
                    throw new InvalidOperationException("Shell.Current is null. Falling back to direct navigation.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Shell navigation failed: {ex.Message}. Attempting direct navigation.");
                await FallbackToDirectNavigation(route);
            }
        }
        public async Task NavigateToAsync(string route, IDictionary<string, object> parameters)
        {
            if (Shell.Current != null)
            {
               
                await Shell.Current.GoToAsync(route,parameters);
            }
        }
        private async Task FallbackToDirectNavigation(string route)
        {
            Page targetPage = route switch
            {
                "loginpage" => ResolvePage<BillableTracking_Mobile3.Pages.LoginPage>(),
                "mainpage" => ResolvePage<MainPage>(),
                "accessstatuspage" => ResolvePage<BillableTracking_Mobile3.Pages.AccessStatusPage>(),
                _ => throw new ArgumentException($"No direct navigation mapping for route: {route}")
            };

            if (targetPage == null)
            {
                throw new InvalidOperationException($"Could not resolve page for route: {route}");
            }

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.Navigation.PushAsync(targetPage);
                }
                else
                {
                    Application.Current.MainPage = targetPage;
                }
            });
        }

        private TPage ResolvePage<TPage>() where TPage : Page
        {
            var viewModel = _serviceProvider.GetService(GetViewModelType<TPage>());
            if (viewModel == null)
            {
                throw new InvalidOperationException($"Could not resolve view model for {typeof(TPage).Name}");
            }

            return (TPage)Activator.CreateInstance(typeof(TPage), viewModel);
        }
        public async Task NavigateToAsync<TViewModel>(object parameter = null) where TViewModel : class
        {
            var pageType = MapViewModelToPage(typeof(TViewModel));
            var page = (Page)_serviceProvider.GetService(pageType);
            var viewModel = _serviceProvider.GetService<TViewModel>();

            // Pass the parameter to the view model if it supports it
            if (viewModel is IParameterizedViewModel parameterizedViewModel && parameter != null)
            {
                parameterizedViewModel.SetParameter(parameter);
            }

            page.BindingContext = viewModel;

            // Use AppShell navigation to push the page
            await AppShell.Current.Navigation.PushAsync(page);
        }

        private Type MapViewModelToPage(Type viewModelType)
        {
            if (viewModelType == typeof(MainPageViewModel))
                return typeof(MainPage);
            if (viewModelType == typeof(PatientBillingViewModel))
                return typeof(PatientBillingPage);
            if (viewModelType == typeof(NewPatientRegistrationViewModel))
                return typeof(NewPatientRegistrationPage);

            throw new InvalidOperationException($"No page found for view model {viewModelType.Name}");
        }
        private Type GetViewModelType<TPage>() where TPage : Page
        {
            if (typeof(TPage) == typeof(BillableTracking_Mobile3.Pages.LoginPage))
                return typeof(LoginViewModel);
            if (typeof(TPage) == typeof(BillableTracking_Mobile3.Pages.AccessStatusPage))
                return typeof(AccessStatusViewModel);
            if (typeof(TPage) == typeof(MainPage))
                return typeof(object); // Adjust if MainPage has a ViewModel
            throw new ArgumentException($"No view model mapping for {typeof(TPage).Name}");
        }
    }
}