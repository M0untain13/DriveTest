using System.Windows.Controls;
using Приложение.Classes;
using Приложение.Classes.Models;

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
        /// <param name="textBox"> Текстовое поле </param>
        /// <param name="listBox"> Поле для вопросов вроде бы </param>
        /// <param name="test"> Объект теста </param>
        /// <param name="mainWindow"> Объект главного окна </param>
        public void Commands(ref TextBox textBoxText, ref TextBox textBoxTime, ref ListBox listBox, ref DTest test, MainWindow mainWindow);

        /// <summary>
        /// Функция вызова окна
        /// </summary>
        /// <returns> Возвращает значение успешности какого-либо события, в зависимости от типа окна </returns>
        public bool? ShowDialog();
    }
}
