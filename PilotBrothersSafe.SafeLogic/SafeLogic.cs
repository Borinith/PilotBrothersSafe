using System;
using System.Collections.Generic;
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
        ///     Создаём сейф размера n*n
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public Task<bool[,]> CreateSafe(int n)
        {
            return Task.Run(() =>
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

                return matrix;
            });
        }

        /// <summary>
        ///     Возможно ли открыть сейф?
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="iterationCount"></param>
        /// <returns></returns>
        public async Task<bool> IsPossibleToSolveSafe(bool[,] matrix, int iterationCount = 0)
        {
            const bool searchingPosition = true;

            var placesWithSearchingPosition = new List<(int row, int column)>();

            for (var i = 0; i < matrix.GetLength(0); i++) //Row
            {
                for (var j = 0; j < matrix.GetLength(1); j++) //Column
                {
                    if (matrix[i, j] == searchingPosition)
                    {
                        placesWithSearchingPosition.Add((i, j));
                    }
                }
            }

            foreach (var (placeRow, placeColumn) in placesWithSearchingPosition)
            {
                matrix = await ChangeSafe(matrix, placeRow, placeColumn);

                if (await MatrixWin(matrix))
                {
                    return true;
                }
            }

            if (iterationCount < 10)
            {
                return await IsPossibleToSolveSafe(matrix, iterationCount + 1);
            }

            return false;
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
            var randomRotatingMatrix = (bool[,])matrix.Clone();

            while (count < 10 * n)
            {
                randomRotatingMatrix = await ChangeSafe(randomRotatingMatrix, rnd.Next(n), rnd.Next(n));
                count++;
            }

            // Если сейф всё же открыт, поворачиваем первую рукоятку
            if (await MatrixWin(randomRotatingMatrix))
            {
                randomRotatingMatrix = await ChangeSafe(randomRotatingMatrix, 0, 0);
            }

            return randomRotatingMatrix;
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