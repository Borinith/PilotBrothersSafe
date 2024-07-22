using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PilotBrothersSafe.LanguageService;
using PilotBrothersSafe.SafeLogic;
using System;

namespace PilotBrothersSafe
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            // Создаем билдер
            var builder = Host.CreateApplicationBuilder();

            // Внедряем сервисы
            builder.Services.AddSingleton<App>();
            builder.Services.AddSingleton<MainWindow>();
            builder.Services.AddSingleton<ISafeLogic, SafeLogic.SafeLogic>();

            builder.Services.AddSingleton<EngLanguageService>();
            builder.Services.AddSingleton<RusLanguageService>();
            builder.Services.AddSingleton<HebLanguageService>();

            builder.Services.AddSingleton<ProxyLanguage.ProxyLanguageResolver>(serviceProvider => language =>
            {
                return language switch
                {
                    LanguageEnum.English => serviceProvider.GetService<EngLanguageService>(),
                    LanguageEnum.Russian => serviceProvider.GetService<RusLanguageService>(),
                    LanguageEnum.Hebrew => serviceProvider.GetService<HebLanguageService>(),
                    _ => null
                };
            });

            // Создаем хост приложения
            using var host = builder.Build();

            // Получаем сервис - объект класса App
            var app = host.Services.GetService<App>();

            // Запускаем приложение
            app?.Run();
        }
    }
}