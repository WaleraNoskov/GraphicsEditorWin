using GraphicsEditor.Models;
using GraphicsEditor.Services;
using GraphicsEditor.ViewModels;
using GraphicsEditor.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GraphicsEditor;

public class Program
{
    [STAThread]
    public static void Main()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services
                    .AddTransient<IFiltersService, FiltersService>()
                    .AddTransient<ISavingService, SavingService>();
                
                services
                    .AddSingleton<MainModel>()
                    .AddSingleton<MainViewModel>();
                
                services.AddSingleton<App>();
                services.AddSingleton<MainWindow>();
            })
            .Build();
        
        var app = host.Services.GetService<App>();
        app?.Run();
    }
}