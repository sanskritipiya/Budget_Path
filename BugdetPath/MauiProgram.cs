
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
        
        builder.Services.AddSingleton<IInflowService, InflowService>();
        
        builder.Services.AddSingleton<IOutflowService, OutflowService>();
        
        builder.Services.AddSingleton<IDebtService, DebtService>();
        
        builder.Services.AddSingleton<ITransactionService, TransactionService>();
     
        builder.Services.AddScoped<AuthenticationService>();


#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}