using System;
using System.Globalization;
using System.Linq;
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
        private static Button[,] CreateMatrixSafe(int n)
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

                    var row = i;
                    var column = j;

                    buttonMatrix[i, j].Click += (_, _) =>
                    {
                        ChangeMatrixSafe(buttonMatrix, row, column);

                        if (MatrixWin(buttonMatrix))
                        {
                            //FormIfWin();
                        }
                    };
                }
            }

            var buttonMatrixRotated = RandomRotating(buttonMatrix);

            return buttonMatrixRotated;
        }

        private static Button[,] RandomRotating(Button[,] buttonMatrix)
        {
            var rnd = new Random();
            var count = 0;
            var n = buttonMatrix.GetLength(0);

            while (count < 10 * n * n)
            {
                buttonMatrix = ChangeMatrixSafe(buttonMatrix, rnd.Next(n), rnd.Next(n));
                count++;
            }

            return buttonMatrix;
        }

        private static string SetButtonContent(bool position)
        {
            return position ? "|" : "—";
        }

        /// <summary>
        ///     Меняем рукоятки в данном столбце и данной строке исходной матрицы
        /// </summary>
        /// <param name="buttonMatrix"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private static Button[,] ChangeMatrixSafe(Button[,] buttonMatrix, int row, int column)
        {
            for (var i = 0; i < buttonMatrix.GetLength(0); i++) //Row
            {
                buttonMatrix = RotateButton(buttonMatrix, i, column);
            }

            for (var j = 0; j < buttonMatrix.GetLength(1); j++) //Column
            {
                buttonMatrix = RotateButton(buttonMatrix, row, j);
            }

            buttonMatrix = RotateButton(buttonMatrix, row, column);

            return buttonMatrix;
        }

        /// <summary>
        ///     Поворот одной рукоятки
        /// </summary>
        /// <param name="buttonMatrix"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private static Button[,] RotateButton(Button[,] buttonMatrix, int row, int column)
        {
            if (Convert.ToBoolean(buttonMatrix[row, column].Tag))
            {
                buttonMatrix[row, column].Tag = false;
                buttonMatrix[row, column].Content = SetButtonContent(false);
            }
            else
            {
                buttonMatrix[row, column].Tag = true;
                buttonMatrix[row, column].Content = SetButtonContent(true);
            }

            return buttonMatrix;
        }

        /// <summary>
        ///     Проверяем открылся ли сейф
        /// </summary>
        /// <param name="buttonMatrix"></param>
        /// <returns></returns>
        private static bool MatrixWin(Button[,] buttonMatrix)
        {
            var correctMatrix = buttonMatrix
                .Cast<Button>()
                .Select(x => Convert.ToBoolean(x.Tag))
                .ToList();

            return correctMatrix.All(x => x) || correctMatrix.All(x => !x);
        }

        private void MatrixDraw(Button[,] buttonMatrix)
        {
            // Если в сейфе всего одна рукоятка
            if (buttonMatrix.Length == 1)
            {
                //FormIfWin();
            }

            // Если сейф изначально открыт, поворачиваем первую рукоятку
            if (MatrixWin(buttonMatrix))
            {
                buttonMatrix = ChangeMatrixSafe(buttonMatrix, 0, 0);
            }

            CommonWindow.Children.Add(Game);

            for (var i = 0; i < buttonMatrix.GetLength(0); i++) //Row
            {
                Game.RowDefinitions.Add(new RowDefinition());
                Game.ColumnDefinitions.Add(new ColumnDefinition());

                for (var j = 0; j < buttonMatrix.GetLength(1); j++) //Column    
                {
                    var buttonWithPosition = buttonMatrix[i, j];

                    Grid.SetRow(buttonWithPosition, i);
                    Grid.SetColumn(buttonWithPosition, j);

                    Game.Children.Add(buttonWithPosition);
                }
            }
        }
    }
}