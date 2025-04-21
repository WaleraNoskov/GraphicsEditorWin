using GraphicsEditor.Core.Models;
using GraphicsEditor.Core.Services;
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
        // создаем хост приложения
        var host = Host.CreateDefaultBuilder()
            // внедряем сервисы
            .ConfigureServices(services =>
            {
                services.AddTransient<IFiltersService, FiltersService>();
                
                services
                    .AddSingleton<MainModel>()
                    .AddSingleton<MainViewModel>();
                
                services.AddSingleton<App>();
                services.AddSingleton<MainWindow>();
            })
            .Build();
        // получаем сервис - объект класса App
        var app = host.Services.GetService<App>();
        // запускаем приложения
        app?.Run();
    }
}