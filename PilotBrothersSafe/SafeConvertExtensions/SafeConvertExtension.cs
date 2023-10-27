using System.Windows;
using System.Windows.Controls;

namespace PilotBrothersSafe.SafeConvertExtensions
{
    /// <summary>
    ///     Логика работы сейфа
    /// </summary>
    public static class SafeConvertExtension
    {
        /// <summary>
        ///     Конвертация булевой матрицы в матрицу кнопок
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Button[,] ToButtonArray(this bool[,] matrix)
        {
            var buttonMatrix = new Button[matrix.GetLength(0), matrix.GetLength(0)];

            for (var i = 0; i < matrix.GetLength(0); i++) //Row
            {
                for (var j = 0; j < matrix.GetLength(1); j++) //Column
                {
                    buttonMatrix[i, j] = new Button
                    {
                        Content = SetButtonContent(matrix[i, j]),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Margin = new Thickness(0.5)
                    };
                }
            }

            return buttonMatrix;
        }

        /// <summary>
        ///     Обновление матрицы кнопок по новой булевой матрице
        /// </summary>
        /// <param name="buttonMatrix"></param>
        /// <param name="newMatrix"></param>
        /// <returns></returns>
        public static Button[,] UpdateButtonMatrix(this Button[,] buttonMatrix, bool[,] newMatrix)
        {
            for (var i = 0; i < buttonMatrix.GetLength(0); i++) //Row
            {
                for (var j = 0; j < buttonMatrix.GetLength(1); j++) //Column
                {
                    buttonMatrix[i, j].Content = SetButtonContent(newMatrix[i, j]);
                }
            }

            return buttonMatrix;
        }

        /// <summary>
        ///     Позиция рукоятки
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private static string SetButtonContent(bool position)
        {
            return position ? "|" : "—";
        }
    }
}