using BillableTracking_Mobile3.Pages;
using BillableTracking_Mobile3;
using BillableTracking_Mobile3.Services;
using BillableTracking_Mobile3.ViewModels;

using Microsoft.Extensions.Logging;
using BillableTracking_Mobile3.Mapping;

namespace BillableTracking_Mobile3;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
        // Register AutoMapper
        builder.Services.AddAutoMapper(typeof(MappingProfile));
        // Register Services
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IDeviceInfoService, DeviceInfoService>();
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>(); // Single registration

        // Register ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<AccessStatusViewModel>();
        builder.Services.AddTransient<MainPageViewModel>();
        builder.Services.AddTransient<PatientBillingViewModel>();
        builder.Services.AddTransient<NewPatientRegistrationViewModel>();
        // Register Pages
        builder.Services.AddTransient<BillableTracking_Mobile3.Pages.LoginPage>();
        builder.Services.AddTransient<BillableTracking_Mobile3.Pages.AccessStatusPage>();
        builder.Services.AddTransient<MainPage>();
        //builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<PatientBillingPage>();
        builder.Services.AddTransient<NewPatientRegistrationPage>();

       

        // Register AppShell
        builder.Services.AddSingleton<AppShell>();

        var app = builder.Build();
        var serviceProvider = app.Services;
        return app;
    }
}
