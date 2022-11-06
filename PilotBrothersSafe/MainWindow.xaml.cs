using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace PilotBrothersSafe
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CommonWindow.Children.Clear();
            CreateStartMenu();
        }

        private void CreateStartMenu()
        {
            CommonWindow.Children.Add(StartMenu);

            if (StartMenu.FindName("CreateSafe") is Button createSafeButton)
            {
                createSafeButton.Click += CreateSafeButtonClick;
            }
        }

        private void CreateSafeButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                int.TryParse((StartMenu.FindName("SafeSize") as IntegerUpDown)?.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var safeSize);

                StartMenu.Children.Clear();
            }
        }
    }
}