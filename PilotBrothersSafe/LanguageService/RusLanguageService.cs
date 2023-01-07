namespace PilotBrothersSafe.LanguageService
{
    public class RusLanguageService : ILanguageService
    {
        public string CreateSafeText => "Создать сейф";

        public string MainMenuText => "Главное меню";

        public string MinimumSafeSizeText => "Размер сейфа должен быть больше или равен ";

        public string SafeSizeText => "Введите размер сейфа";

        public string TitleText => "Сейф Братьев Пилотов";

        public string WinText => "Вы выиграли!";
    }
}