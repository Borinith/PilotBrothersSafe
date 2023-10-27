using PilotBrothersSafe.LanguageService;
using PilotBrothersSafe.SafeConvertExtensions;
using PilotBrothersSafe.SafeLogic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Toolkit;

namespace PilotBrothersSafe
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const string CANNOT_LOAD_LANGUAGE = "Cannot load language";
        private const string LANGUAGE_IMAGES_FOLDER = "LanguageImages";
        private const int MINIMUM_SAFE_SIZE_VALUE = 2;
        private readonly HashSet<string> _constants = new(4);
        private readonly Dictionary<LanguageEnum, string> _imagePaths = new();
        private readonly ProxyLanguage.ProxyLanguageResolver _resolver;
        private readonly ISafeLogic _safeLogic;
        private LanguageEnum _currentLanguage = LanguageEnum.English;
        private ILanguageService _languageService = null!;
        private int _safeSizeSelectedValue = 4;

        /// <summary>
        ///     Main window
        /// </summary>
        public MainWindow(ProxyLanguage.ProxyLanguageResolver resolver, ISafeLogic safeLogic)
        {
            _resolver = resolver;
            _safeLogic = safeLogic;

            foreach (var language in Enum.GetValues<LanguageEnum>())
            {
                _imagePaths.Add(language, $"{language}.png");
            }

            UpdateLanguage();

            InitializeComponent();
            StartMenuDraw();
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
        private async Task<Button[,]> CreateMatrixSafe(int n)
        {
            var matrix = new bool[n, n];
            const bool defaultPosition = true;

            for (var i = 0; i < n; i++) //Row
            {
                for (var j = 0; j < n; j++) //Column
                {
                    matrix[i, j] = defaultPosition;
                }
            }

            matrix = await _safeLogic.RandomRotating(matrix);

            // Если сейф изначально открыт, поворачиваем первую рукоятку
            if (await _safeLogic.MatrixWin(matrix))
            {
                matrix = await _safeLogic.ChangeSafe(matrix, 0, 0);
            }

            var buttonMatrixRotated = matrix.ToButtonArray();

            for (var i = 0; i < n; i++) //Row
            {
                for (var j = 0; j < n; j++) //Column
                {
                    var row = i;
                    var column = j;

                    buttonMatrixRotated[i, j].Click += async (_, _) =>
                    {
                        matrix = await _safeLogic.ChangeSafe(matrix, row, column);

                        if (await _safeLogic.MatrixWin(matrix))
                        {
                            FormIfWin();
                        }
                        else
                        {
                            buttonMatrixRotated = buttonMatrixRotated.UpdateButtonMatrix(matrix);
                        }
                    };
                }
            }

            return buttonMatrixRotated;
        }

        /// <summary>
        ///     Создаём сейф размера NxN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CreateSafeButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button createSafeButton)
            {
                if (StartMenu.FindName(RegisterNames.UPDATE_LANGUAGE) is Button updateLanguageButton)
                {
                    createSafeButton.IsEnabled = false;
                    updateLanguageButton.IsEnabled = false;
                }

                var isParsedSafeSize = int.TryParse((StartMenu.FindName(RegisterNames.SAFE_SIZE) as IntegerUpDown)?.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var safeSize);

                if (isParsedSafeSize == false || safeSize < MINIMUM_SAFE_SIZE_VALUE)
                {
                    throw new Exception(_languageService.MinimumSafeSizeText + MINIMUM_SAFE_SIZE_VALUE);
                }

                _safeSizeSelectedValue = safeSize;

                var buttons = await CreateMatrixSafe(safeSize);

                MatrixDraw(buttons);
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
                StartMenuDraw();
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

            createStartMenuButton.Click += CreateStartMenuClick;

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
                Value = _safeSizeSelectedValue,
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

            createSafeButton.Click += CreateSafeButtonClick;

            Grid.SetRow(createSafeButton, 4);
            Grid.SetColumn(createSafeButton, 0);

            StartMenu.Children.Add(createSafeButton);

            RegisterNameCustom(RegisterNames.CREATE_SAFE, createSafeButton);

            #endregion Create safe button

            #region Create update language button

            var img = new Image
            {
                Source = new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LANGUAGE_IMAGES_FOLDER, _imagePaths[_currentLanguage]), UriKind.RelativeOrAbsolute))
            };

            var stackPnl = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            stackPnl.Children.Add(img);

            var createUpdateLanguageButton = new Button
            {
                Content = stackPnl,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                Name = RegisterNames.UPDATE_LANGUAGE,
                Tag = _currentLanguage,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 30
            };

            createUpdateLanguageButton.Click += UpdateLanguageButtonClick;

            Grid.SetRow(createUpdateLanguageButton, 6);
            Grid.SetColumn(createUpdateLanguageButton, 0);

            StartMenu.Children.Add(createUpdateLanguageButton);

            RegisterNameCustom(RegisterNames.UPDATE_LANGUAGE, createUpdateLanguageButton);

            #endregion Create update language button

            CommonWindow.Children.Add(StartMenu);
        }

        private void UpdateLanguage()
        {
            var languageService = _resolver(_currentLanguage);

            _languageService = languageService ?? throw new Exception(CANNOT_LOAD_LANGUAGE);

            Title = _languageService.TitleText;
        }

        private void UpdateLanguageButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var isParsedSafeSize = int.TryParse((StartMenu.FindName(RegisterNames.SAFE_SIZE) as IntegerUpDown)?.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var safeSize);

                if (isParsedSafeSize == false || safeSize < MINIMUM_SAFE_SIZE_VALUE)
                {
                    throw new Exception(_languageService.MinimumSafeSizeText + MINIMUM_SAFE_SIZE_VALUE);
                }

                _safeSizeSelectedValue = safeSize;

                var isParsedCurrentLanguage = Enum.TryParse<LanguageEnum>(button.Tag.ToString(), out var currentLanguage);

                if (isParsedCurrentLanguage == false)
                {
                    throw new Exception(CANNOT_LOAD_LANGUAGE);
                }

                var allLanguages = Enum.GetValues<LanguageEnum>();
                var currentLanguageIndex = Array.IndexOf(allLanguages, currentLanguage);
                var nextLanguageIndex = (currentLanguageIndex + 1) % allLanguages.Length;

                _currentLanguage = (LanguageEnum)nextLanguageIndex;

                UpdateLanguage();
                StartMenuDraw();
            }
        }
    }
}