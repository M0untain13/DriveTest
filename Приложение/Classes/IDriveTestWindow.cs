﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Приложение
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
        /// <param name="questions"> Список вопросов </param>
        public void Commands(ref TextBox textBox, ref ListBox listBox, ref DTest test, MainWindow mainWindow);

        /// <summary>
        /// Функция вызова окна
        /// </summary>
        /// <returns> Возвращает значение успешности какого-либо события, в зависимости от типа окна </returns>
        public bool? ShowDialog();
    }
}
