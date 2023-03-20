using System;
using System.Collections.Generic;
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
    /// Напишу тут про известные баги
    /// TODO: баг, кнопочки RadioButtons не хотят нормально группироваться
    /// </summary>
    public partial class MainWindow : Window
    {
        public HashSet<char> incorrectChars = new HashSet<char>()
        {
            '\\', '/', ':', '*', '?', '"', '<', '>', '|'
        };
        readonly public DirectoryInfo mainDirectory = new DirectoryInfo("Tests");                  //Главный каталог, где будут лежать тесты.
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
            IWindowFactoryMethod windowFactoryMethod = button.DataContext as IWindowFactoryMethod;
            windowFactoryMethod.SetDirectory = mainDirectory; //Устанавливаем путь к главной директории, из которой при необходимости будут браться тесты.
            IDriveTestWindow window = windowFactoryMethod.Window();
            bool isButtonClick = window.ShowDialog() ?? false;
            if (isButtonClick)
            {
                window.Commands(ref textBox1, ref listBox1, ref test, this); //Комманды, которое должны выполниться над элементами главного окна
                Button_Click_Switch(sender, e);
            }
        }

        /// <summary>
        /// Выбор типа вопроса в выпадающем списке, приводящее к созданию нового вопроса в тесте
        /// </summary>
        /// <param name="sender"> Объект выпадающего списка </param>
        /// <param name="e"></param>
        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBox1.Text != string.Empty)
            {
                AddQuestion(comboBox1.Text);
                comboBox1.Text = string.Empty;
            }
        }

        /// <summary>
        /// Добавить вопрос в редакторе
        /// </summary>
        private void AddQuestion(string type)
        {
            DQuest quest = type switch
            {
                StringTypeQuestion.MATCHING_SEARCH => new DQuest(new FactoryAnswerPair()),
                StringTypeQuestion.SELECTIVE_ANSWER_ONE => new DQuest(new FactoryAnswerOne()),
                _ => new DQuest(new FactoryAnswer()),
            };
            quest.Type = type;
            quest.Answers = new ObservableCollection<AbstractAnswer> { quest.FactoryMethod.Answer() };
            quest.Number = test.quests.Count + 1;
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
            //TODO: проверка названия работает некорректно со слэшами, причем в отладке вроде все в порядке
            if ((from char sym in test.name
                 where incorrectChars.Contains(sym)
                 select sym).Count() != 0)
            {
                MessageBox.Show("Имя файла не должно содержать специальные знаки! " + string.Join(" ", incorrectChars.ToArray()));
            }
            else
            {
                test.name = textBox1.Text; //TODO: Тут нужно добавить время сохранения наверное в название? А может и нет, а может уже стоит наконец решить проблему с коллизией названий
                test.Save(mainDirectory.ToString());
                MessageBox.Show("Тест сохранён!");
            }
        }

        /// <summary>
        /// Нажатие кнопки, приводящее к добавлению поля для ответа в вопросе в редакторе
        /// </summary>
        /// <param name="sender"> Объект кнопки </param>
        /// <param name="e"></param>
        private void Button_Click_AnswerAdd(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            DQuest quest = button.DataContext as DQuest;
            AbstractAnswerFactoryMethod factoryMethod = quest.FactoryMethod;
            AbstractAnswer abstractAnswer = factoryMethod.Answer();
            int number = (int)button.Tag;
            ObservableCollection<AbstractAnswer> answers = test.quests[number - 1].Answers;
            if (answers.Count < 10)
            {
                answers.Add(abstractAnswer);
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
            ObservableCollection<AbstractAnswer> answers = test.quests[number - 1].Answers;
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
