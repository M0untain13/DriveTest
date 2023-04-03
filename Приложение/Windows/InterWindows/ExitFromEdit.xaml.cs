using System;
using System.Windows;
using System.Windows.Controls;
using Приложение.Classes;
using Приложение.Classes.Models;
using Приложение.Classes.Services;

namespace Приложение.Windows.InterWindows
{
    /// <summary>
    /// Логика взаимодействия для ExitFromEdit.xaml
    /// </summary>
    public partial class ExitFromEdit : Window, IDriveTestWindow
    {
        public bool isSaveTest = false;
        public ExitFromEdit()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Выход без сохранения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitWithoutSave(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Закрытие окна и возврат в редактор
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Выход с сохранием теста
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitWithSave(object sender, RoutedEventArgs e)
        {
            isSaveTest = true;
            DialogResult = true;
        }

        void IDriveTestWindow.Commands(ref TextBox textBoxText, ref TextBox textBoxTime, ref ListBox listBox, ref DTest test, MainWindow mainWindow)
        {
            if(isSaveTest)
            {
                test.time = Convert.ToInt32(textBoxTime.Text);
                Loader.SaveTest(test, mainWindow.mainDirectory + "\\" + test.name);
            }
            test.quests.Clear();
            textBoxText.Clear();
            textBoxTime.Clear();
        }
    }
}
