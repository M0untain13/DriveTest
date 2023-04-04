using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Приложение.Classes;
using Приложение.Classes.Enums;
using Приложение.Classes.Models;

namespace Приложение.Windows.InterWindows
{
    /// <summary>
    /// Логика взаимодействия для CreateTest.xaml
    /// </summary>
    public partial class CreateTest : Window, IDriveTestWindow
    {
        public HashSet<char> incorrectChars = EnumIncorrectCharacters.characters.ToHashSet();
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
            if(textBox1.Text.Length < 5)
            {
                warningBlock.Text = "Название должно содержать 5 или более символов!"; //TODO: сделать проверку на занятость названия, или не делать, если мы в название будем добавлять время создания?
            }
            else if(passBox1.Password.Length < 3)
            {
                warningBlock.Text = "Пароль должен содержать 3 или более символов!";
            }
            //TODO: проверка названия работает некорректно со слэшами, причем в отладке вроде все в порядке, странная херня
            else if ((from char sym in textBox1.Text
                     where incorrectChars.Contains(sym)
                     select sym).Count() != 0)
            {
                warningBlock.Text = "Имя файла не должно содержать специальные знаки! " + string.Join(" ", incorrectChars.ToArray());
            }
            else
            {
                test = new DTest(passBox1.Password)
                {
                    name = textBox1.Text
                };
                DialogResult = true; //Окно закрывается
            }
            ClearWarning();
        }

        private async void ClearWarning()
        {
            await Task.Delay(3000);
            warningBlock.Text = "Прежде чем создать тест, введите название и пароль!";
        }

        void IDriveTestWindow.Commands(ref TextBox textBoxText, ref TextBox textBoxTime, ref ListBox listBox, ref DTest test, ref DResult result, MainWindow w)
        {
            test = this.test;
            textBoxText.Text = test.name;
            textBoxTime.Text = test.time.ToString();
            listBox.ItemsSource = test.quests;
        }
    }
}
