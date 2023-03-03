using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Приложение
{
    /// <summary>
    /// Логика взаимодействия для ExitFromEdit.xaml
    /// </summary>
    public partial class ExitFromEdit : Window, IDriveTestWindow
    {
        public bool saveTest = false;
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
        /// Заглушка, вместо которой должно происходить сохранение теста и выход из редактора
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Заглушка(object sender, RoutedEventArgs e)
        {
            //TODO: везде, где используется заглушка, нужно разработать необходимый функционал
            MessageBox.Show("Функция кнопки пока не реализована...");
        }

        void IDriveTestWindow.Commands(ref TextBox textBox, ref ListBox listBox, ref ObservableCollection<DQuest> questions)
        {
            if(!saveTest)
            {
                questions.Clear();
                textBox.Clear();
            }
        }

        IDriveTestWindow IDriveTestWindow.Init(DirectoryInfo directoryInfo)
        {
            return new ExitFromEdit();
        }
    }
}
