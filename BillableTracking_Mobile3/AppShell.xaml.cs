using System.Diagnostics;
using Microsoft.Maui.Storage;
using MonkeyCache.FileStore;

namespace BillableTracking_Mobile3
{
    public partial class AppShell : Shell
    {
        private readonly IServiceProvider _serviceProvider;

        public AppShell(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();

            // Explicitly register routes with lowercase names
            //Routing.RegisterRoute("loginpage", typeof(BillableTracking_Mobile3.Pages.LoginPage));
            //Routing.RegisterRoute("mainpage", typeof(MainPage));
            //Routing.RegisterRoute("accessstatuspage", typeof(BillableTracking_Mobile3.Pages.AccessStatusPage));

            //// Debug registered routes
            //var registeredRoutes = Routing.GetRegisteredRoutes();
            //Debug.WriteLine("Registered Routes: " + string.Join(", ", registeredRoutes));

            // Navigate to the initial route synchronously
            GoToAsync("//LoginPage").GetAwaiter().GetResult();
            Debug.WriteLine("AppShell initialized with routes: loginpage, mainpage, accessstatuspage");
        }

        public async Task NavigateBasedOnToken()
        {
            Debug.WriteLine("Starting NavigateBasedOnToken...");
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                Debug.WriteLine($"Token retrieval completed. Token value: '{token}' (Length: {token?.Length ?? 0})");

                if (!string.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("Token is valid, navigating to AccessStatusPage...");
                    // Attempt Shell navigation
                    await Shell.Current.Navigation.PopToRootAsync();
                   var currentUser= Barrel.Current.Get<Models.UserRecord>("CachedUser");
                    await GoToAsync("//AccessStatusPage", new Dictionary<string, object>
                        {
                            { "CurrentUser", currentUser }
                        });
                    Debug.WriteLine("Navigated to AccessStatusPage due to existing token.");
                }
                else
                {
                    Debug.WriteLine("No valid token found, navigating to LoginPage...");
                    await Shell.Current.Navigation.PopToRootAsync();
                    await GoToAsync("//LoginPage");
                    Debug.WriteLine("Navigated to LoginPage as no token was found.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during token-based navigation: {ex.Message}\nStackTrace: {ex.StackTrace}");
                // Fallback to manual navigation if Shell fails
                var token = await SecureStorage.GetAsync("auth_token");
                if (!string.IsNullOrEmpty(token))
                {
                    var accessStatusPage = _serviceProvider.GetService<BillableTracking_Mobile3.Pages.AccessStatusPage>();
                    if (accessStatusPage != null)
                    {
                        await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage = accessStatusPage);
                        Debug.WriteLine("Fallback navigation to AccessStatusPage succeeded.");
                    }
                }
                else
                {
                    await GoToAsync("LoginPage");
                }
            }
            Debug.WriteLine("NavigateBasedOnToken completed.");
        }
    }
}