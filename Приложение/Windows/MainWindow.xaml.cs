using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Приложение.Classes;

namespace Приложение.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// Напишу тут про известные баги
    /// TODO: баг, кнопочки RadioButtons не хотят нормально группироваться
    /// TODO: баг, инфа в форме данных не сохраняется
    /// TODO: при выходе из редактора и нажатии кнопки "Сохранить и выйти" сохранение время для прохождения теста не сохраняется
    /// </summary>
    public partial class MainWindow : Window
    {
        public HashSet<char> incorrectChars = EnumIncorrectCharacters.characters.ToHashSet();
        public readonly DirectoryInfo mainDirectory = new("Tests");                         //Главный каталог, где будут лежать тесты.
        private readonly ObservableCollection<string> _addQuestion = EnumTypeQuestion.strings;    //Строки для добавления вопросов в тест.
        private DTest _test = null;
        private DResult _result = null;

        private DispatcherTimer _timer;
        private TimeSpan _time;

        public MainWindow()
        {
            InitializeComponent();
            EditListBox1.ItemsSource = new ObservableCollection<DQuest>();
            EditComboBox1.ItemsSource = _addQuestion;
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
        private void Button_Click_Switch(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Grid nextGrid = button.Tag as Grid;
            switch (nextGrid.Name)
            {
                case "Main":
                    Title = "Главное меню";
                    break;
                case "Editor":
                    Title = "Редактор тестов";
                    break;
                case "Test":
                    Title = "Тестирование";
                    break;
                case "Result":
                    Title = "Результаты";
                    break;
                default:
                    Title = "А что это за окно?";
                    break;
            }
            Switching(button.Parent as Grid, nextGrid);
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
                window.Commands(ref EditTextBox1, ref EditTextBoxTime, ref EditListBox1, ref _test, this); //Комманды, которое должны выполниться над элементами главного окна
                if (window.GetType() == typeof(OpenTest))
                {
                    TestTextBox1.Text = EditTextBox1.Text;
                    TestListBox1.ItemsSource = EditListBox1.ItemsSource;

                    if (Convert.ToInt32(EditTextBoxTime.Text) == 0)
                    {
                        TestTextBoxTime.Text = "Время выполнения неограничено";
                    }
                    else
                    {
                        _time = TimeSpan.FromMinutes(Convert.ToInt32(EditTextBoxTime.Text));
                        _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                        {
                            //TODO: при завершении обратного отсчёта тест должен завершиться принудительно
                            TestTextBoxTime.Text = _time.ToString("c");
                            if (_time == TimeSpan.Zero) _timer.Stop();
                            _time = _time.Add(TimeSpan.FromSeconds(-1));
                        }, Application.Current.Dispatcher);
                        _timer.Start();
                    }
                    foreach (var quest in _test.quests)
                    {
                        quest.ListBox = TestListBox1;
                    }
                }
                else
                {
                    foreach (var quest in _test.quests)
                    {
                        quest.ListBox = EditListBox1;
                    }
                }
                Button_Click_Switch(sender, e);
            }
        }

        /// <summary>
        /// Выбор типа вопроса в выпадающем списке, приводящее к созданию нового вопроса в тесте
        /// </summary>
        /// <param name="sender"> Объект выпадающего списка </param>
        /// <param name="e"></param>
        private void СomboBox1_DropDownClosed(object sender, EventArgs e)
        {
            if (EditComboBox1.Text != string.Empty)
            {
                AddQuestion(EditComboBox1.Text);
                EditComboBox1.Text = string.Empty;
            }
        }

        /// <summary>
        /// Добавить вопрос в редакторе
        /// </summary>
        private void AddQuestion(string type)
        {
            DQuest quest = type switch
            {
                EnumTypeQuestion.MATCHING_SEARCH => new DQuest(new FactoryAnswerPair()),
                EnumTypeQuestion.SELECTIVE_ANSWER_ONE => new DQuest(new FactoryAnswerOne()),
                _ => new DQuest(new FactoryAnswer()),
            };
            quest.Type = type;
            quest.Answers = new ObservableCollection<AbstractAnswer> { quest.FactoryMethod.Answer() };
            quest.Answers[0].Parrent = quest;
            quest.Number = _test.quests.Count + 1;
            _test.quests.Add(quest);
            EditListBox1.ItemsSource = _test.quests;
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
            if ((from char sym in EditTextBox1.Text
                 where incorrectChars.Contains(sym)
                 select sym).Count() != 0)
            {
                MessageBox.Show("Имя файла не должно содержать специальные знаки! " + string.Join(" ", incorrectChars.ToArray()));
            }
            else if((from char sym in EditTextBoxTime.Text
                    where !char.IsDigit(sym)
                    select sym).Count() != 0)
            {
                MessageBox.Show("Время выполнения содержит нечисленные символы!");
            }
            else
            {
                _test.time = Convert.ToInt32(EditTextBoxTime.Text);
                _test.name = EditTextBox1.Text; //TODO: Тут нужно добавить время сохранения наверное в название? А может и нет, а может уже стоит наконец решить проблему с коллизией названий
                _test.Save(mainDirectory.ToString());
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
            abstractAnswer.Parrent = quest;
            int number = (int)button.Tag;
            ObservableCollection<AbstractAnswer> answers = _test.quests[number - 1].Answers;
            if (answers.Count < 10)
            {
                answers.Add(abstractAnswer);
                EditListBox1.ItemsSource = _test.quests;
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
            ObservableCollection<AbstractAnswer> answers = _test.quests[number - 1].Answers;
            if(answers.Count > 1)
            {
                answers.RemoveAt(answers.Count - 1);
                EditListBox1.ItemsSource = _test.quests;
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
            _test.quests.RemoveAt(number - 1);
            for(int i = 0; i < _test.quests.Count; i++)
            {
                _test.quests[i].Number = i + 1;
            }
            EditListBox1.ItemsSource = _test.quests;
            EditListBox1.Items.Refresh();
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
