using System.Windows;

namespace PilotBrothersSafe
{
    public class App : Application
    {
        private readonly MainWindow _mainWindow;

        // Через систему внедрения зависимостей получаем объект главного окна
        public App(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _mainWindow.Show(); // Отображаем главное окно на экране
            base.OnStartup(e);
        }
    }
}