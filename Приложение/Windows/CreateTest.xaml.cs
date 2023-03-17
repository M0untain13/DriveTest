using System.Windows;
using System.Windows.Controls;
using Приложение.Classes;

namespace Приложение.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateTest.xaml
    /// </summary>
    public partial class CreateTest : Window, IDriveTestWindow
    {
        public DTest test;
        public CreateTest()
        {
            InitializeComponent();
            textBox1.MaxLength = 20;
            passBox1.MaxLength = 20;
        }

        /// <summary>
        /// Нажатие на кнопку "Создать"
        /// </summary>
        /// <param name="sender"> Объект кнопки </param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //TODO: стоит ли делать проверку на ввод спец.символов?
            if(textBox1.Text.Length < 5)
            {
                warningBlock.Text = "Название должно содержать 5 или более символов!"; //TODO: сделать проверку на занятость названия
            }
            else if(passBox1.Password.Length < 5)
            {
                warningBlock.Text = "Пароль должен содержать 5 или более символов!";
            }
            else
            {
                test = new DTest(passBox1.Password);
                test.name = textBox1.Text;
                DialogResult = true; //Окно закрывается
            }
        }

        void IDriveTestWindow.Commands(ref TextBox textBox, ref ListBox listBox, ref DTest test, MainWindow w)
        {
            test = this.test;
            test.quests = this.test.quests;
            textBox.Text = this.test.name;
            listBox.ItemsSource = test.quests;
        }
    }
}
