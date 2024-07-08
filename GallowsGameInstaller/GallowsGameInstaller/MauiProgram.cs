using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;

namespace GallowsGameInstaller
{
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
                    fonts.AddFont("Maki-Sans.ttf", "Maki-Sans");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // Зарегистрируйте FolderPicker как singleton
            builder.Services.AddSingleton<IFolderPicker>(FolderPicker.Default);
            // Зарегистрируйте MainPage как transient, чтобы разрешить зависимость от IFolderPicker
            builder.Services.AddTransient<MainPage>();
            return builder.Build();
        }
    }
}
