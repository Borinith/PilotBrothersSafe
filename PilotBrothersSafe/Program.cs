using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PilotBrothersSafe.LanguageService;
using System;

namespace PilotBrothersSafe
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            // Создаем хост приложения
            using (var host = Host.CreateDefaultBuilder()
                       // Внедряем сервисы
                       .ConfigureServices(services =>
                       {
                           services.AddSingleton<App>();
                           services.AddSingleton<MainWindow>();
                           services.AddSingleton<ILanguageService, EngLanguageService>();
                           //services.AddSingleton<ILanguageService, RusLanguageService>();
                       })
                       .Build())
            {
                // Получаем сервис - объект класса App
                var app = host.Services.GetService<App>();

                // Запускаем приложения
                app?.Run();
            }
        }
    }
}