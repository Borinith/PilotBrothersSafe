namespace PilotBrothersSafe.LanguageService
{
    public interface ILanguageService
    {
        string CreateSafeText { get; }

        string MainMenuText { get; }

        string MinimumSafeSizeText { get; }

        string SafeSizeText { get; }

        string TitleText { get; }

        string WinText { get; }
    }
}