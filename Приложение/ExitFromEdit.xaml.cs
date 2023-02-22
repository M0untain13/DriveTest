using System;
using System.Collections.Generic;
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
    public partial class ExitFromEdit : Window
    {
        public bool isTestClosed = false;
        public ExitFromEdit()
        {
            InitializeComponent();
        }
        private void ExitWithoutSave(object sender, RoutedEventArgs e)
        {
            //TODO: сделать проверки ввода
            isTestClosed = true;
            Close();
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Заглушка(object sender, RoutedEventArgs e)
        {
            //TODO: везде, где используется заглушка, нужно разработать необходимый функционал
            MessageBox.Show("Функция кнопки пока не реализована...");
        }
    }
}
