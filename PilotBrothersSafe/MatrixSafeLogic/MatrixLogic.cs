using System;
using System.Linq;
using System.Windows.Controls;

namespace PilotBrothersSafe.MatrixSafeLogic
{
    /// <summary>
    ///     Логика работы сейфа
    /// </summary>
    public static class MatrixSafeLogic
    {
        /// <summary>
        ///     Меняем рукоятки в данном столбце и данной строке исходной матрицы
        /// </summary>
        /// <param name="buttonMatrix"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static Button[,] ChangeMatrixSafe(Button[,] buttonMatrix, int row, int column)
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
        ///     Проверяем открылся ли сейф
        /// </summary>
        /// <param name="buttonMatrix"></param>
        /// <returns></returns>
        public static bool MatrixWin(Button[,] buttonMatrix)
        {
            var correctMatrix = buttonMatrix
                .Cast<Button>()
                .Select(x => Convert.ToBoolean(x.Tag))
                .ToList();

            return correctMatrix.All(x => x) || correctMatrix.All(x => !x);
        }

        /// <summary>
        ///     Запутываем рукоятки случайным образом
        /// </summary>
        /// <param name="buttonMatrix"></param>
        /// <returns></returns>
        public static Button[,] RandomRotating(Button[,] buttonMatrix)
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
        ///     Позиция рукоятки
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static string SetButtonContent(bool position)
        {
            return position ? "|" : "—";
        }
    }
}