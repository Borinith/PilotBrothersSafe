using PilotBrothersSafe.LanguageService;
using System;
using System.Collections.Generic;
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
        private const int MINIMUM_SAFE_SIZE_VALUE = 2;
        private readonly HashSet<string> _constants = new(4);
        private readonly ILanguageService _languageService;

        /// <summary>
        ///     Main window
        /// </summary>
        public MainWindow(ILanguageService languageService)
        {
            _languageService = languageService;
            Title = _languageService.TitleText;

            InitializeComponent();
            CreateStartMenu();
        }

        /// <summary>
        ///     Очищаем поле
        /// </summary>
        private void ChildrenClear()
        {
            CommonWindow.Children.Clear();

            StartMenu.Children.Clear();
            StartMenu.RowDefinitions.Clear();

            Game.Children.Clear();
            Game.RowDefinitions.Clear();
            Game.ColumnDefinitions.Clear();

            FormWin.Children.Clear();
            FormWin.RowDefinitions.Clear();

            var constants = new List<string>(_constants);

            foreach (var constant in constants)
            {
                UnregisterName(constant);
                _constants.Remove(constant);
            }
        }

        /// <summary>
        ///     Создаем матрицу NxN и случайно заполняем ее ― и |
        /// </summary>
        /// <param name="n">Размер сейфа</param>
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
                        Content = MatrixSafeLogic.MatrixSafeLogic.SetButtonContent(defaultPosition),
                        Tag = defaultPosition,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Margin = new Thickness(0.5)
                    };

                    var row = i;
                    var column = j;

                    buttonMatrix[i, j].Click += (_, _) =>
                    {
                        MatrixSafeLogic.MatrixSafeLogic.ChangeMatrixSafe(buttonMatrix, row, column);

                        if (MatrixSafeLogic.MatrixSafeLogic.MatrixWin(buttonMatrix))
                        {
                            FormIfWin();

                            if (FormWin.FindName(RegisterNames.CREATE_START_MENU) is Button createSafeButton)
                            {
                                createSafeButton.Click += CreateStartMenuClick;
                            }
                        }
                    };
                }
            }

            var buttonMatrixRotated = MatrixSafeLogic.MatrixSafeLogic.RandomRotating(buttonMatrix);

            return buttonMatrixRotated;
        }

        /// <summary>
        ///     Создаём сейф размера NxN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateSafeButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var isParsed = int.TryParse((StartMenu.FindName(RegisterNames.SAFE_SIZE) as IntegerUpDown)?.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var safeSize);

                if (isParsed == false || safeSize < MINIMUM_SAFE_SIZE_VALUE)
                {
                    throw new Exception(_languageService.MinimumSafeSizeText + MINIMUM_SAFE_SIZE_VALUE);
                }

                var buttons = CreateMatrixSafe(safeSize);

                MatrixDraw(buttons);
            }
        }

        /// <summary>
        ///     Создаём стартовое меню
        /// </summary>
        private void CreateStartMenu()
        {
            StartMenuDraw();

            if (StartMenu.FindName(RegisterNames.CREATE_SAFE) is Button createSafeButton)
            {
                createSafeButton.Click += CreateSafeButtonClick;
            }
        }

        /// <summary>
        ///     Возвращаемся на стартовое меню
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateStartMenuClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                CreateStartMenu();
            }
        }

        /// <summary>
        ///     Отрисовываем форму выигрыша
        /// </summary>
        private void FormIfWin()
        {
            ChildrenClear();

            const int rowsCount = 6;

            for (var i = 0; i < rowsCount; i++) //Row
            {
                FormWin.RowDefinitions.Add(new RowDefinition());
            }

            #region Win label

            var label = new Label
            {
                Content = _languageService.WinText,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetRow(label, 2);
            Grid.SetColumn(label, 0);

            FormWin.Children.Add(label);

            #endregion Win label

            #region Create start menu button

            var createStartMenuButton = new Button
            {
                Content = _languageService.MainMenuText,
                Height = 25,
                HorizontalAlignment = HorizontalAlignment.Center,
                Name = RegisterNames.CREATE_START_MENU,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 120
            };

            Grid.SetRow(createStartMenuButton, 3);
            Grid.SetColumn(createStartMenuButton, 0);

            FormWin.Children.Add(createStartMenuButton);

            RegisterNameCustom(RegisterNames.CREATE_START_MENU, createStartMenuButton);

            #endregion Create start menu button

            CommonWindow.Children.Add(FormWin);
        }

        /// <summary>
        ///     Отрисовываем кнопки
        /// </summary>
        /// <param name="buttonMatrix"></param>
        private void MatrixDraw(Button[,] buttonMatrix)
        {
            ChildrenClear();

            // Если сейф изначально открыт, поворачиваем первую рукоятку
            if (MatrixSafeLogic.MatrixSafeLogic.MatrixWin(buttonMatrix))
            {
                buttonMatrix = MatrixSafeLogic.MatrixSafeLogic.ChangeMatrixSafe(buttonMatrix, 0, 0);
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

        /// <summary>
        ///     Регистрируем имя и добавляем имя в кэш
        /// </summary>
        /// <param name="name"></param>
        /// <param name="scopedElement"></param>
        private void RegisterNameCustom(string name, object scopedElement)
        {
            RegisterName(name, scopedElement);
            _constants.Add(name);
        }

        /// <summary>
        ///     Отрисовываем стартовое меню
        /// </summary>
        private void StartMenuDraw()
        {
            ChildrenClear();

            const int rowsCount = 8;

            for (var i = 0; i < rowsCount; i++) //Row
            {
                StartMenu.RowDefinitions.Add(new RowDefinition());
            }

            #region Safe size label

            var label = new Label
            {
                Content = _languageService.SafeSizeText,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetRow(label, 2);
            Grid.SetColumn(label, 0);

            StartMenu.Children.Add(label);

            #endregion Safe size label

            #region Safe size value

            var safeSize = new IntegerUpDown
            {
                Height = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                Increment = 1,
                Maximum = 10,
                Minimum = MINIMUM_SAFE_SIZE_VALUE,
                Name = RegisterNames.SAFE_SIZE,
                Value = 4,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 60
            };

            Grid.SetRow(safeSize, 3);
            Grid.SetColumn(safeSize, 0);

            StartMenu.Children.Add(safeSize);

            RegisterNameCustom(RegisterNames.SAFE_SIZE, safeSize);

            #endregion Safe size value

            #region Create safe button

            var createSafeButton = new Button
            {
                Content = _languageService.CreateSafeText,
                Height = 25,
                HorizontalAlignment = HorizontalAlignment.Center,
                Name = RegisterNames.CREATE_SAFE,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 120
            };

            Grid.SetRow(createSafeButton, 4);
            Grid.SetColumn(createSafeButton, 0);

            StartMenu.Children.Add(createSafeButton);

            RegisterNameCustom(RegisterNames.CREATE_SAFE, createSafeButton);

            #endregion Create safe button

            CommonWindow.Children.Add(StartMenu);
        }
    }
}