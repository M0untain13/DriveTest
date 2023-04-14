using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xaml;
using Приложение.Classes.Enums;
using Приложение.Classes.FactoryMethods;
using Приложение.Classes.Models;
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
        public readonly DirectoryInfo mainDirectory = new("Tests");
        private readonly ObservableCollection<string> _addQuestion = EnumTypeQuestion.strings;
        private DTest _test;
        private DResult _result;

        private DispatcherTimer _timer;
        private TimeSpan _time;

        public MainWindow()
        {
            InitializeComponent();
            Directory.CreateDirectory(mainDirectory.Name);
            EditListBox1.ItemsSource = new ObservableCollection<DQuest>();
            EditComboBox1.ItemsSource = _addQuestion;
        }


        #region Редактор
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
        /// Нажатие кнопки, приводящее к добавлению поля для ответа в вопросе в редакторе
        /// </summary>
        /// <param name="sender"> Объект кнопки </param>
        /// <param name="e"></param>
        private void Button_Click_AnswerAdd(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { DataContext: DQuest quest, Tag: int number }) return;
            var factoryMethod = quest.FactoryMethod;
            var abstractAnswer = factoryMethod.Answer();
            if (quest.Type is EnumTypeQuestion.DATA_INPUT or EnumTypeQuestion.OPEN_ANSWER) abstractAnswer.IsCorrect = true;
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
            if (sender is not Button { Tag: int number }) return;
            var answers = _test.quests[number - 1].Answers;
            if (answers.Count > 1)
            {
                answers.RemoveAt(answers.Count - 1);
                EditListBox1.ItemsSource = _test.quests;
            }
        }
        /// <summary>
        /// Добавить вопрос в редакторе
        /// </summary>
        private void AddQuestion(string type)
        {
            DQuest quest = type switch
            {
                EnumTypeQuestion.OPEN_ANSWER => new DQuest(new FactoryOpen()),
                EnumTypeQuestion.SELECTIVE_ANSWER_ONE => new DQuest(new FactorySelectiveOne()),
                EnumTypeQuestion.SELECTIVE_ANSWER_MULTIPLE => new DQuest(new FactorySelectiveMultiple()),
                EnumTypeQuestion.MATCHING_SEARCH => new DQuest(new FactoryMatchingSearch()),
                EnumTypeQuestion.DATA_INPUT => new DQuest(new FactoryDataInput()),
                _ => throw new Exception("Неизвестный тип")
            };
            quest.Type = type;
            quest.Answers = new ObservableCollection<AbstractAnswer> { quest.FactoryMethod.Answer() };
            if (quest.Type is EnumTypeQuestion.DATA_INPUT)
                quest.Answers[0].IsMarkedByUser = true;
            if (quest.Type is EnumTypeQuestion.DATA_INPUT or EnumTypeQuestion.OPEN_ANSWER)
                quest.Answers[0].IsCorrect = true;
            quest.Answers[0].Parrent = quest;
            quest.Number = _test.quests.Count + 1;
            quest.ListBox = EditListBox1;
            _test.quests.Add(quest);
            EditListBox1.ItemsSource = _test.quests;
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
            for (var i = 0; i < _test.quests.Count; i++)
            {
                _test.quests[i].Number = i + 1;
            }
            EditListBox1.ItemsSource = _test.quests;
            EditListBox1.Items.Refresh();
        }
        /// <summary>
        /// Прокрутка колёсиком мышки.
        /// Без этого события, прокрутка не происходит при наводке на lixtBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is not ScrollViewer scv) return;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        private void Button_Click_SaveTest(object sender, RoutedEventArgs e)
        {
            SaveTest();
        }
        public bool SaveTest()
        {
            if (EditTextBox1.Text.Length < 5)
            {
                MessageBox.Show("Название должно содержать 5 или более символов!");
                return false;
            }
            while (EditTextBox1.Text[EditTextBox1.Text.Length - 1] == ' ')
            {
                EditTextBox1.Text = EditTextBox1.Text.Substring(0, EditTextBox1.Text.Length - 1);
            }
            
            if ((from char sym in EditTextBox1.Text
                 where incorrectChars.Contains(sym)
                 select sym).Count() != 0)
            {
                MessageBox.Show("Имя файла не должно содержать специальные знаки! " + string.Join(" ", incorrectChars.ToArray()));
                return false;
            }
            else
            {
                var tempName = _test.name;
                _test.name = EditTextBox1.Text;
                if ((from char sym in EditTextBoxTime.Text
                     where !char.IsDigit(sym)
                     select sym).Count() != 0)
                {
                    MessageBox.Show("Время выполнения содержит нечисленные символы!");
                    return false;
                }
                else if (!Loader.CheckTest(_test, out var message))
                {
                    MessageBox.Show($"{message}!");
                    return false;
                }
                else
                {
                    if (tempName != EditTextBox1.Text)
                    {
                        if (Directory.Exists($"{mainDirectory}\\{tempName}\\Results"))
                        {
                            //Переносим результаты в новую папку
                            Directory.CreateDirectory($"{mainDirectory}\\{EditTextBox1.Text}");
                            Directory.Move($"{mainDirectory}\\{tempName}\\Results", $"{mainDirectory}\\{EditTextBox1.Text}\\Results");
                        }
                        Directory.Delete($"{mainDirectory}\\{tempName}", true);
                    }
                    _test.time = Convert.ToInt32(EditTextBoxTime.Text);
                    Loader.SaveTest(_test, mainDirectory + "\\" + _test.name);
                    MessageBox.Show("Тест сохранён!");
                    return true;
                }
            }
        }
        private void SaveTestAsNew(object sender, RoutedEventArgs e)
        {
            if (EditTextBox1.Text.Length < 5)
            {
                MessageBox.Show("Название должно содержать 5 или более символов!");
                return;
            }
            while (EditTextBox1.Text[EditTextBox1.Text.Length - 1] == ' ')
            {
                EditTextBox1.Text = EditTextBox1.Text.Substring(0, EditTextBox1.Text.Length - 1);
            }
            if (Directory.Exists($"Tests\\{EditTextBox1.Text}"))
            {
                MessageBox.Show("Вы пытались сохранить тест как новый, но название уже занято!");
            }
            else if ((from char sym in EditTextBox1.Text
                 where incorrectChars.Contains(sym)
                 select sym).Count() != 0)
            {
                MessageBox.Show("Имя теста не должно содержать специальные знаки! " + string.Join(" ", incorrectChars.ToArray()));
            }
            else
            {
                _test.name = EditTextBox1.Text;
                if ((from char sym in EditTextBoxTime.Text
                     where !char.IsDigit(sym)
                     select sym).Count() != 0)
                {
                    MessageBox.Show("Время выполнения содержит нечисленные символы!");
                }
                else if (!Loader.CheckTest(_test, out var message))
                {
                    MessageBox.Show($"{message}!");
                }
                else
                {
                    _test.time = Convert.ToInt32(EditTextBoxTime.Text);
                    Loader.SaveTest(_test, mainDirectory + "\\" + _test.name);
                    MessageBox.Show("Тест сохранён!");
                }
            }
        }
        /// <summary>
        /// Проверка ввода баллов на содержание нечисленных символов в вводе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox textBox) return;
            if (!textBox.Text.Any(c => c is < '0' or > '9')) return;
            MessageBox.Show("Ошибка: Ввод нечисленных символов недопустим!");
            textBox.Text = "0";
        }
        private void EditTextBoxTime_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is not TextBox textBox) throw new Exception();
            if (textBox.Text == string.Empty) textBox.Text = "0";
        }
        #endregion



        #region Тестирование
        /// <summary>
        /// Метод для перемешивания элементов коллекции
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<string> MixList(List<string> list)
        {
            if (list.Count < 2) throw new Exception("Элементов в коллекции должно быть хотя бы 2");

            var randizer = new Random();
            var isMixed = false;
            var result = new List<string>();

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
            }
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && _textBlock == null) listBox.SelectedIndex = -1;
        }
        #endregion



        #region Просмотр результатов
        private void Button_Click_ShowCorrectAnswers(object sender, RoutedEventArgs e)
        {
            if (VisCheck.IsChecked ?? false) return;
            if (_result.CheckPass(resPassBox.Password))
            {
                VisCheck.IsChecked = true;
                ButtonShowCorAns.IsEnabled = false;
                resPassBox.IsEnabled = false;
                resPassBox.Clear();
                ResultListBox1.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Неверный пароль!");
            }
        }
        #endregion



        #region Общее
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
        private void Button_Click_FromResToMain(object sender, RoutedEventArgs e)
        {
            VisCheck.IsChecked = false;
            ButtonShowCorAns.IsEnabled = true;
            resPassBox.IsEnabled = true;
            resPassBox.Clear();
            _result = null;
            ResultListBox1.ItemsSource = null;
            Button_Click_Switch(sender, e);
        }
        private void Button_Click_FromTestToRes(object sender, RoutedEventArgs e)
        {
            foreach (var quest in _test.quests)
            {
                if (!quest.AnswerRequired) //Если ответ не требуется, то пропускаем такой вопрос
                    continue;

                var isMarked = false; //Если ответ требуется, то ожидаем ответа
                foreach (var answer in quest.Answers)
                {
                    if (answer.IsMarkedByUser
                        || (quest.Type == EnumTypeQuestion.OPEN_ANSWER && quest.OpenAnswer != string.Empty)
                        || quest.Type == EnumTypeQuestion.DATA_INPUT) //Если был дан ответ, то делаем пометку, что ответ был дан
                    {
                        isMarked = true;
                        break;
                    }
                }

                if (!isMarked) //Если ответ не был дан в вопросе с обязательным ответом, то нужно указать на это
                {
                    MessageBox.Show("На какой-то вопрос с обязательным ответом не был дан ответ!");
                    return;
                }
            }

            _timer?.Stop();
            Button_Click_IntermediateWindow(sender, e);
        }
        /// <summary>
        /// Нажатие какой-либо кнопки, приводящее к открытию промежуточного окна.
        /// </summary>
        /// <param name="sender"> Объект кнопки </param>
        /// <param name="e"></param>
        private void Button_Click_IntermediateWindow(object sender, RoutedEventArgs e)
        {
            //Если вдруг sender не является кнопкой или в данных нет фабричного метода, то нас просто посылают подальше
            if (sender is not Button { DataContext: IWindowFactoryMethod windowFactoryMethod })
                return;
            //Устанавливаем путь к главной директории, из которой при необходимости будут браться тесты.
            windowFactoryMethod.SetDirectory = mainDirectory;
            var window = windowFactoryMethod.Window();
            if (window is ExitFromEdit w)
            {
                w.MainWin = this;
                window = w;
            }
            var isButtonClick = window.ShowDialog() ?? false;

            if (!isButtonClick)
                return; //Если не была нажата кнопка, то ничего не должно произойти

            var isSaveTest = false;
            window.Transfer(ref _test, ref _result, ref isSaveTest); //Передача экземпляров теста и резултата между окнами

            //Большая часть логики вынесена ниже
            Dictionary<Type, Action> actions = new()
            { //Почему не использовал switch? А потому что там в кейсах требуются константные значения, а typeof(%Class%) не является константным
                //Почему не использовал if else? А потому что захотелось реализовать словарь анонимных методов
                {typeof(CreateTest),
                    delegate{
                        EditTextBox1.Text = _test.name;
                        EditTextBoxTime.Text = _test.time.ToString();
                        EditListBox1.ItemsSource = _test.quests;
                    }
                },
                {typeof(ExitFromEdit),
                    delegate{
                        if (isSaveTest)
                            Button_Click_SaveTest(sender, e);
                        _test.quests.Clear();
                        EditTextBox1.Clear();
                        EditTextBoxTime.Clear();
                    }
                },
                {typeof(ExitFromTest),
                    delegate{
                        var correctAnswers = _result.Answers;
                        for (var i = 0; i < _test.quests.Count; i++)
                            correctAnswers[i].SetTestingAnswers(_test.quests[i]);
                        _test.quests.Clear();
                        ResultTextBox1.Text = $"{_result.NameOfTest} - {_result.NameOfPeople} - {_result.Score} из {_result.MaxScore}";
                        ResultListBox1.ItemsSource = _result.Answers;
                        Loader.SaveResult(_result, $"{mainDirectory}\\{TestTextBox1.Text}\\Results", _result.NameOfPeople);
                        TestTextBox1.Text = string.Empty;
                        TestTextBoxTime.Text = string.Empty;
                    }
                },
                {typeof(OpenResults),
                    delegate{
                        ResultTextBox1.Text = $"{_result.NameOfTest} - {_result.NameOfPeople} - {_result.Score} из {_result.MaxScore}";
                        ResultListBox1.ItemsSource = _result.Answers;
                    }
                },
                {typeof(OpenTest),
                    delegate{
                        TestTextBox1.Text = _test.name;
                        TestListBox1.ItemsSource = _test.quests;

                        var correctAnswers = _result.Answers;
                        foreach (var quest in _test.quests) {
                            correctAnswers.Add(quest.FactoryMethod.Result());
                            correctAnswers[correctAnswers.Count - 1].SetObject(quest);
                            if (quest.Answers[0].GetType() != typeof(DAnswerPair))

                                continue; //Если вопрос является соответствием, то необходимо перемешать варианты, иначе пропускаем

                            var answers = quest.Answers;
                            List<string> list = answers.Select(answer => answer.Answer2).ToList();
                            list = MixList(list);
                            for (var i = 0; i < answers.Count; i++){
                                answers[i].IsMarkedByUser = true; //Пометка, что ответы перемешаны
                                answers[i].Answer2 = list[i];
                            }
                            quest.Answers = answers;
                        }
                        _result.Answers = correctAnswers;
                        if (_test.time == 0) {
                            TestTextBoxTime.Text = "Время выполнения неограничено";
                        }
                        else {
                            //Установка таймера
                            _time = TimeSpan.FromMinutes(_test.time);
                            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate{
                                TestTextBoxTime.Text = _time.ToString("c");
                                if (_time == TimeSpan.Zero){
                                    _timer.Stop();
                                    correctAnswers = _result.Answers;
                                    for (var i = 0; i < _test.quests.Count; i++)
                                        correctAnswers[i].SetTestingAnswers(_test.quests[i]);
                                    _test.quests.Clear();
                                    ResultTextBox1.Text = $"{_result.NameOfTest} - {_result.NameOfPeople} - {_result.Score} из {_result.MaxScore}";
                                    ResultListBox1.ItemsSource = _result.Answers;
                                    Loader.SaveResult(_result, $"{mainDirectory}\\{TestTextBox1.Text}\\Results", _result.NameOfPeople);
                                    TestTextBox1.Text = string.Empty;
                                    TestTextBoxTime.Text = string.Empty;
                                    Test.Visibility = Visibility.Hidden;
                                    Test.IsEnabled = false;
                                    Result.Visibility = Visibility.Visible;
                                    Result.IsEnabled = true;
                                    MessageBox.Show("Время закончилось!");
                                }
                                _time = _time.Add(TimeSpan.FromSeconds(-1));
                            }, Application.Current.Dispatcher);
                            _timer.Start();
                        }

                        foreach (var quest in _test.quests)
                            quest.ListBox = TestListBox1;
                    }
                },
                {typeof(OpenTestForEdit),
                    delegate{
                        EditTextBox1.Text = _test.name;
                        EditTextBoxTime.Text = _test.time.ToString();
                        EditListBox1.ItemsSource = _test.quests;
                        foreach (var quest in _test.quests)
                            quest.ListBox = EditListBox1;
                    }
                }
            };
            actions[window.GetType()].Invoke();
            Button_Click_Switch(sender, e);
        }
        private void Button_Click_close_app(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Несохранённые данные будут утеряны.",
                "Выйти из приложения?",
                MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }

        #endregion
    }
}
