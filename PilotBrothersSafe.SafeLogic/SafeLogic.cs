using System;
using System.Linq;
using System.Threading.Tasks;

namespace PilotBrothersSafe.SafeLogic
{
    /// <summary>
    ///     Логика работы сейфа
    /// </summary>
    public class SafeLogic : ISafeLogic
    {
        /// <summary>
        ///     Меняем рукоятки в данном столбце и данной строке исходной матрицы
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public Task<bool[,]> ChangeSafe(bool[,] matrix, int row, int column)
        {
            return Task.Run(() =>
            {
                for (var i = 0; i < matrix.GetLength(0); i++) //Row
                {
                    matrix = Rotate(matrix, i, column);
                }

                for (var j = 0; j < matrix.GetLength(1); j++) //Column
                {
                    matrix = Rotate(matrix, row, j);
                }

                matrix = Rotate(matrix, row, column);

                return matrix;
            });
        }

        /// <summary>
        ///     Проверяем открылся ли сейф
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Task<bool> MatrixWin(bool[,] matrix)
        {
            return Task.Run(() =>
            {
                var correctMatrix = matrix.Cast<bool>().ToList();

                return correctMatrix.All(x => x) || correctMatrix.All(x => !x);
            });
        }

        /// <summary>
        ///     Запутываем рукоятки случайным образом
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public async Task<bool[,]> RandomRotating(bool[,] matrix)
        {
            var rnd = new Random();
            var count = 0;
            var n = matrix.GetLength(0);

            while (count < 10 * n * n)
            {
                matrix = await ChangeSafe(matrix, rnd.Next(n), rnd.Next(n));
                count++;
            }

            return matrix;
        }

        /// <summary>
        ///     Поворот одной рукоятки
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private static bool[,] Rotate(bool[,] matrix, int row, int column)
        {
            matrix[row, column] = !matrix[row, column];

            return matrix;
        }
    }
}