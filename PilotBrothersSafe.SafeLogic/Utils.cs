using System.Linq;

namespace PilotBrothersSafe.SafeLogic
{
    public static class Utils
    {
        public static bool SequenceEquals<T>(this T[,] a, T[,] b)
        {
            return a.Rank == b.Rank
                   && Enumerable.Range(0, a.Rank).All(d => a.GetLength(d) == b.GetLength(d))
                   && a.Cast<T>().SequenceEqual(b.Cast<T>());
        }
    }
}