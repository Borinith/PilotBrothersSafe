using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace PilotBrothersSafe
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
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

                var buttons = CreateMatrixSafe(safeSize);

                MatrixDraw(buttons);
            }
        }

        /// <summary>
        ///     Создаем матрицу NxN и случайно заполняем ее ― и |
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private Button[,] CreateMatrixSafe(int n)
        {
            var buttonMatrix = new Button[n, n];
            const bool defaultPosition = true;

            for (var i = 0; i < n; i++) //Row
            {
                for (var j = 0; j < n; j++) //Column
                {
                    buttonMatrix[i, j] = new Button
                    {
                        Content = SetButtonContent(defaultPosition),
                        Tag = defaultPosition,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Margin = new Thickness(0.5)
                    };

                    buttonMatrix[i, j].Click += ChangeMatrixSafe;
                }
            }

            return buttonMatrix;
        }

        private static string SetButtonContent(bool position)
        {
            return position ? "|" : "—";
        }

        private void ChangeMatrixSafe(object sender, RoutedEventArgs e)
        {
        }

        private void MatrixDraw(Button[,] buttons)
        {
            CommonWindow.Children.Add(Game);

            for (var i = 0; i < buttons.GetLength(0); i++) //Row
            {
                Game.RowDefinitions.Add(new RowDefinition());
                Game.ColumnDefinitions.Add(new ColumnDefinition());

                for (var j = 0; j < buttons.GetLength(1); j++) //Column    
                {
                    var buttonWithPosition = buttons[i, j];

                    Grid.SetRow(buttonWithPosition, i);
                    Grid.SetColumn(buttonWithPosition, j);

                    Game.Children.Add(buttonWithPosition);
                }
            }
        }
    }
}