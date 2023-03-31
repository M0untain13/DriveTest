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
using Приложение.Classes.Enums;
using Приложение.Classes.FactoryMethods;
using Приложение.Classes.Services;
using Приложение.Windows.InterWindows;

namespace Приложение.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HashSet<char> incorrectChars = EnumIncorrectCharacters.characters.ToHashSet();
        public readonly DirectoryInfo mainDirectory = new("Tests");                         //Главный каталог, где будут лежать тесты.
        private readonly ObservableCollection<string> _addQuestion = EnumTypeQuestion.strings;    //Строки для добавления вопросов в тест.
        private DTest _test;
        private DResult _result;

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
            if (sender is not Button { Tag: Grid nextGrid, Parent: Grid currentGrid }) return;
            Title = nextGrid.Name switch
            {
                "Main" => "Главное меню",
                "Editor" => "Редактор тестов",
                "Test" => "Тестирование",
                "Result" => "Результаты",
                _ => throw new Exception("Неизвестное окно")
            };
            Switching(currentGrid, nextGrid);
        }

        /// <summary>
        /// Нажатие какой-либо кнопки, приводящее к открытию промежуточного окна.
        /// </summary>
        /// <param name="sender"> Объект кнопки </param>
        /// <param name="e"></param>
        private void Button_Click_IntermediateWindow(object sender, RoutedEventArgs e)
        {
            //Внимание! В этом методе некоторые блоки условия выполняют выход из метода или из блока цикла, это сделано для уменьшения уровня вложенности

            //Если вдруг sender не является кнопкой или в данных нет фабричного метода, то нас просто посылают подальше
            if (sender is not Button { DataContext: IWindowFactoryMethod windowFactoryMethod }) return;
            //Устанавливаем путь к главной директории, из которой при необходимости будут браться тесты.
            windowFactoryMethod.SetDirectory = mainDirectory; 
            var window = windowFactoryMethod.Window();
            var isButtonClick = window.ShowDialog() ?? false;

            if (!isButtonClick) return; //Если не была нажата кнопка, то ничего не должно произойти
            window.Commands(ref EditTextBox1, ref EditTextBoxTime, ref EditListBox1, ref _test, this); //Операции, которое должны выполниться над элементами главного окна
            //Некоторая часть операций вынесена ниже
            
            if (window.GetType() == typeof(OpenTest))
            { 
                //Здесь происходит подготовка к проведению тестирования
                _result = new DResult();
                var correctAnswers = _result.Answers;

                foreach (var quest in _test.quests)
                {
                    switch (quest.Type)
                    {
                        case EnumTypeQuestion.OPEN_ANSWER:
                            correctAnswers.Add(new DResultsAnswerOpen());
                            break;
                        case EnumTypeQuestion.SELECTIVE_ANSWER_ONE:
                            correctAnswers.Add(new DResultsAnswerSelectiveOne());
                            break;
                        case EnumTypeQuestion.SELECTIVE_ANSWER_MULTIPLE:
                            correctAnswers.Add(new DResultsAnswerSelectiveMultiple());
                            break;
                        case EnumTypeQuestion.MATCHING_SEARCH:
                            correctAnswers.Add(new DResultsAnswerMatchingSearch());
                            break;
                        case EnumTypeQuestion.DATA_INPUT:
                            correctAnswers.Add(new DResultsAnswerDataInput());
                            break;
                        default:
                            throw new Exception("Что-то пошло не так...");
                    }
                    correctAnswers[correctAnswers.Count - 1].SetObject(quest);

                    if (quest.Answers[0].GetType() != typeof(DAnswerPair)) continue; //Если вопрос является соответствием, то необходимо перемешать варианты, иначе пропускаем
                    var answers = quest.Answers;
                    List<string> list = answers.Select(answer => answer.Answer2).ToList();
                    list = MixList(list);
                    for(var i = 0; i < answers.Count; i++)
                    {
                        answers[i].IsMarkedByUser = true; //Пометка, что ответы перемешаны
                        answers[i].Answer2 = list[i];
                    }
                    quest.Answers = answers;
                }
                _result.Answers = correctAnswers;

                TestTextBox1.Text = EditTextBox1.Text;
                TestListBox1.ItemsSource = EditListBox1.ItemsSource;

                if (Convert.ToInt32(EditTextBoxTime.Text) == 0)
                {
                    TestTextBoxTime.Text = "Время выполнения неограничено";
                }
                else
                {
                    //Установка таймера
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
            else if (window.GetType() == typeof(ExitFromTest))
            {
                //TODO: Прежде чем очищать, нужно ответы занести в _result
                var correctAnswers = _result.Answers;
                for (var i = 0; i < _test.quests.Count; i++)
                {
                    correctAnswers[i].SetTestingAnswers(_test.quests[i].Answers);
                }
                _test.quests.Clear();
                TestTextBox1.Text = string.Empty;
                TestTextBoxTime.Text = string.Empty;
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
            quest.ListBox = EditListBox1;
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
            if(sender is not ScrollViewer scv) return;
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
                Loader.SaveTest(_test, mainDirectory + "\\" + _test.name);
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
            if(sender is not Button {DataContext: DQuest quest, Tag: int number}) return;
            var factoryMethod = quest.FactoryMethod;
            var abstractAnswer = factoryMethod.Answer();
            abstractAnswer.Parrent = quest;
            var answers = _test.quests[number - 1].Answers;
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
            if(sender is not Button {Tag: int number})return;
            var answers = _test.quests[number - 1].Answers;
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
            if (sender is not Button { Tag: int number }) return;
            _test.quests.RemoveAt(number - 1);
            for(var i = 0; i < _test.quests.Count; i++)
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
            if(sender is not TextBox textBox) return;
            if (!textBox.Text.Any(c => c is < '0' or > '9')) return;
            MessageBox.Show("Ошибка: Ввод нечисленных символов недопустим!");
            textBox.Text = "0";
        }

        /// <summary>
        /// Метод для перемешивания элементов коллекции
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<string> MixList(List<string> list)
        {
            var randizer = new Random();
            var isMixed = false;
            var result = new List<string>();

            //TODO: если содержиться только один элемент, то происходит зацикливание, так что нужно будет обязательно сделать проверку, чтобы в тесте не было вопросов с одним вариантом ответа (кроме открытого)
            while (!isMixed)
            {
                result = list.OrderBy(k => randizer.Next()).ToList();
                for (var i = 0; i < result.Count; i++)
                {
                    //Проверяем, перемешалась ли коллекция
                    if (result[i] != list[i])
                    {
                        isMixed = true;
                        break;
                    }
                }
            }

            return result;
        }

        #region Перемещение элементов соответствия
        private TextBlock _textBlock;
        private void Label_MouseClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TextBlock textBlock) return;
            if (_textBlock == null)
            {
                _textBlock = textBlock;
            }
            else
            {
                (textBlock.Text, _textBlock.Text) = (_textBlock.Text, textBlock.Text);
                _textBlock = null;
                    //TestListBox1.Items.Refresh();
            }
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && _textBlock == null) listBox.SelectedIndex = -1;
        }
        #endregion

    }
}
