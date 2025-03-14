using BillableTracking_Mobile3.Services;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3
{
    public partial class App : Application
    {
        public App(AppShell appShell, IDatabaseService databaseService)
        {
            InitializeComponent();
          // SecureStorage.RemoveAll();
            MainPage = appShell;
            // Initialize the database with all tables
            Task.Run(async () => await databaseService.InitializeDatabaseAsync()).GetAwaiter().GetResult();
        }

        protected override async void OnStart()
        {
            base.OnStart();
            // Ensure AppShell's navigation is triggered after the app starts
            if (MainPage is AppShell appShell)
            {
                appShell.NavigateBasedOnToken();
            }
        }
    }
}