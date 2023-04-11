using System.Windows.Controls;
using Приложение.Classes;
using Приложение.Classes.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Приложение.Windows.InterWindows
{
    /// <summary>
    /// Интерфейс взаимодействия с окнами
    /// </summary>
    public interface IDriveTestWindow
    {
        /// <summary>
        /// Набор комманд, которые должны выполниться над элементами главного окна
        /// </summary>
        public void Transfer(ref DTest test, ref DResult result, ref bool isSaveTest);

        /// <summary>
        /// Функция вызова окна
        /// </summary>
        /// <returns> Возвращает значение успешности какого-либо события, в зависимости от типа окна </returns>
        public bool? ShowDialog();
    }
}
