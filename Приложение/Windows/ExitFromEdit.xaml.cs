using System.Windows;
using System.Windows.Controls;
using Приложение.Classes;

namespace Приложение.Windows
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
        /// Выход с сохранием теста
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitWithSave(object sender, RoutedEventArgs e)
        {
            saveTest = true;
            DialogResult = true;
        }

        void IDriveTestWindow.Commands(ref TextBox textBox, ref ListBox listBox, ref DTest test, MainWindow mainWindow)
        {
            if(saveTest)
            {
                test.Save(mainWindow.mainDirectory.ToString());
            }
            test.quests.Clear();
            textBox.Clear();
        }
    }
}
