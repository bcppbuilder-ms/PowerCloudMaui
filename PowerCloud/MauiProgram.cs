using PowerCloud.ViewModels;
using PowerCloud.Views;
using PowerCloud.Platforms;

using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace PowerCloud;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<IAccountFiler>((e) => new AccountFiler());
        builder.Services.AddSingleton<IIte2DeviceInfo>((e) => new Ite2DeviceInfoService());
        builder.Services.AddSingleton<MainViewModel>();

        builder.Services.AddTransient<TestPage>();
        
		return builder.Build();
	}
}
