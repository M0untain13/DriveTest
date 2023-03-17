using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Приложение.Classes;

namespace Приложение.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly public DirectoryInfo mainDirectory = new DirectoryInfo("Tests");                       //Главный каталог, где будут лежать тесты.
        private DTest test = null;                                                                      //Объект теста
        readonly private ObservableCollection<string> addQuestion = StringTypeQuestion.List;            //Строки для добавления вопросов в тест.
        public MainWindow()
        {
            InitializeComponent();
            listBox1.ItemsSource = new ObservableCollection<DQuest>();
            comboBox1.ItemsSource = addQuestion;
        }
        
        /// <summary>
        /// Сменить поверхности\сетки\grid.
        /// </summary>
        /// <param name="current"> Данная поверхность </param>
        /// <param name="next"> Новая поверхность </param>
        private static void Switching(UIElement current, UIElement next)
        {
            current.Visibility = Visibility.Hidden;
            current.IsEnabled = false;
            next.Visibility = Visibility.Visible;
            next.IsEnabled = true;
        }

        /// <summary>
        /// Нажатие какой-либо кнопки, приводящее к смене поверхностей.
        /// </summary>
        /// <param name="sender"> Объект кнопки </param>
        /// <param name="e"></param>
        private static void Button_Click_Switch(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Switching(button.Parent as Grid, button.Tag as Grid);
        }

        /// <summary>
        /// Нажатие какой-либо кнопки, приводящее к открытию промежуточного окна.
        /// </summary>
        /// <param name="sender"> Объект кнопки </param>
        /// <param name="e"></param>
        private void Button_Click_IntermediateWindow(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            IFactoryMethod factoryMethod = button.DataContext as IFactoryMethod;
            factoryMethod.SetDirectory = mainDirectory; //Устанавливаем путь к главной директории, из которой при необходимости будут браться тесты.
            IDriveTestWindow window = factoryMethod.Window();
            bool isButtonClick = window.ShowDialog() ?? false;
            if (isButtonClick)
            {
                window.Commands(ref textBox1, ref listBox1, ref test, this); //Комманды, которое должны выполниться над элементами главного окна
                Button_Click_Switch(sender, e);
            }
        }

        /// <summary>
        /// Добавить вопрос в редакторе
        /// </summary>
        private void AddQuestion(string type)
        {
            //questions.Add(DTest.GetTest().quests); //TODO: это надо будет потом поменять, когда будут готовы конструкции разных вопросов.
            //testList.ItemsSource = questions;
            DQuest quest = new()
            {
                Type = type,
                Answers = new ObservableCollection<DAnswer> { new DAnswer() },
                Number = test.quests.Count + 1
            };
            test.quests.Add(quest);
            listBox1.ItemsSource = test.quests;
        }

        /// <summary>
        /// Прокрутка колёсиком мышки.
        /// Без этого события, прокрутка не происходит при наводке на lixtBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = sender as ScrollViewer;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        /// <summary>
        /// Выбор типа вопроса в выпадающем списке, приводящее к созданию нового вопроса в тесте
        /// </summary>
        /// <param name="sender"> Объект выпадающего списка </param>
        /// <param name="e"></param>
        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            if(comboBox1.Text != string.Empty)
            {
                AddQuestion(comboBox1.Text);
                comboBox1.Text = string.Empty;
            }
        }

        /// <summary>
        /// Заглушка, от которой необходимо избавиться
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Заглушка(object sender, RoutedEventArgs e)
        {
            //TODO: везде, где используется заглушка, нужно разработать необходимый функционал
            MessageBox.Show("Эта кнопка пока не работает :(");
        }

        /// <summary>
        /// Нажатие кнопки "Сохранить" сохраняет тест
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveTest(object sender, RoutedEventArgs e)
        {
            test.name = textBox1.Text;
            test.Save(mainDirectory.ToString());
            MessageBox.Show("Тест сохранён!");
        }

        /// <summary>
        /// Нажатие кнопки, приводящее к добавлению поля для ответа в вопросе в редакторе
        /// </summary>
        /// <param name="sender"> Объект кнопки </param>
        /// <param name="e"></param>
        private void Button_Click_AnswerAdd(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int number = (int)button.Tag;
            ObservableCollection<DAnswer> answers = test.quests[number - 1].Answers;
            if (answers.Count < 10)
            {
                answers.Add(new DAnswer());
                listBox1.ItemsSource = test.quests;
            }
        }

        /// <summary>
        /// Нажатие кнопки, приводящее к удалению поля ответа в вопросе в редакторе
        /// </summary>
        /// <param name="sender"> Объект кнопки </param>
        /// <param name="e"></param>
        private void Button_Click_AnswerDelete(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int number = (int)button.Tag;
            ObservableCollection<DAnswer> answers = test.quests[number - 1].Answers;
            if(answers.Count > 1)
            {
                answers.RemoveAt(answers.Count - 1);
                listBox1.ItemsSource = test.quests;
            }
        }

        /// <summary>
        /// Нажатие кнопки, приводящее к удалению вопроса в редакторе
        /// </summary>
        /// <param name="sender"> Объект кнопки </param>
        /// <param name="e"></param>
        private void Button_Click_QuestionDelete(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int number = (int)button.Tag;
            test.quests.RemoveAt(number - 1);
            for(int i = 0; i < test.quests.Count; i++)
            {
                test.quests[i].Number = i + 1;
            }
            listBox1.ItemsSource = test.quests;
            listBox1.Items.Refresh();
        }

        /// <summary>
        /// Проверка ввода баллов на содержание нечисленных символов в вводе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            foreach (char c in textBox.Text)
            {
                if (c is < '0' or > '9')
                {
                    MessageBox.Show("Ошибка: Ввод нечисленных символов недопустим!");
                    textBox.Text = "0";
                }
            }
        }
    }
}
