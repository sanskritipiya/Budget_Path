using BugdetPath.Services;
using Microsoft.Extensions.Logging;

namespace BugdetPath;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        builder.Services.AddMauiBlazorWebView();
        
        builder.Services.AddSingleton<IUserService, UserInfoService>();
        
        // builder.Services.AddSingleton<IIncomeService, IncomeService>();
     
        builder.Services.AddScoped<AuthenticationStateService>();


#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}