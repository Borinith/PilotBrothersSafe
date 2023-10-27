using System.Threading.Tasks;

namespace PilotBrothersSafe.SafeLogic
{
    public interface ISafeLogic
    {
        /// <summary>
        ///     Меняем рукоятки в данном столбце и данной строке исходной матрицы
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        Task<bool[,]> ChangeSafe(bool[,] matrix, int row, int column);

        /// <summary>
        ///     Проверяем открылся ли сейф
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        Task<bool> MatrixWin(bool[,] matrix);

        /// <summary>
        ///     Запутываем рукоятки случайным образом
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        Task<bool[,]> RandomRotating(bool[,] matrix);
    }
}