using System.Windows;
using System.Windows.Controls;
using Приложение.Classes;

namespace Приложение.Windows.InterWindows
{
    /// <summary>
    /// Логика взаимодействия для ExitFromTest.xaml
    /// </summary>
    public partial class ExitFromTest : Window, IDriveTestWindow
    {
        public bool saveTest = false;
        public ExitFromTest()
        {
            InitializeComponent();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Выход с сохранием теста
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit(object sender, RoutedEventArgs e)
        {
            saveTest = true;
            DialogResult = true;
        }

        void IDriveTestWindow.Commands(ref TextBox textBoxText, ref TextBox textBoxTime, ref ListBox listBox, ref DTest test, MainWindow mainWindow)
        {
            //Nothing
        }
    }
}
