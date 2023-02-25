using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Приложение
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly public DirectoryInfo mainDirectory = new DirectoryInfo("Tests"); //Главный каталог, где будут лежать тесты
        private ObservableCollection<DQuest> questions = new(); //Коллекция вопросов в тесте
        readonly private ObservableCollection<string> addQuestion = new ObservableCollection<string> //Строки для добавления вопросов в тест
        {
            StringTypeQuestion.OPEN_ANSWER,
            StringTypeQuestion.SELECTIVE_ANSWER,
            StringTypeQuestion.MATCHING_SEARCH,
            StringTypeQuestion.DATA_INPUT
        };
        public MainWindow() //TODO: потом надо будет убрать заранее подготовленный список
        {
            InitializeComponent();
            comboBox1.ItemsSource = addQuestion;
            //DTest.GetTest().quests.ToList().ForEach(quest => { questions.Add(quest); }); //Добавляем вопросы из заранее подготовленного списка
            listBox1.ItemsSource = questions;
        }
        private void Switching(Grid current, Grid next)
        {
            //Функция смены поверхности
            current.Visibility = Visibility.Hidden;
            current.IsEnabled = false;
            next.Visibility = Visibility.Visible;
            next.IsEnabled = true;
        }
        private void Button_Click_Switch(object sender, RoutedEventArgs e)
        {
            //Событие нажатия кнопки для смены поверхности
            Button button = sender as Button;
            Switching(button.Parent as Grid, button.Tag as Grid);
        }
        private void Button_Click_IntermediateWindow(object sender, RoutedEventArgs e)
        {
            //Событие нажатия кнопки, вызывающего промежуточное окно
            Button button = sender as Button;
            dynamic window = null;
            switch (button.DataContext)
            {
                case CreateTest:
                    window = new CreateTest();
                    break;
                case ExitFromEdit:
                    window = new ExitFromEdit();
                    break;
                case OpenTestForEdit:
                    window = new OpenTestForEdit();
                    mainDirectory.GetDirectories().ToList().ForEach(directory => { window.testsDir.Add(directory); });
                    break;
                case OpenTest:
                    window = new OpenTest();
                    mainDirectory.GetDirectories().ToList().ForEach(directory => { window.testsDir.Add(directory); });
                    break;
            }
            if(window == null)
            {
                throw new Exception("Ошибка: Попытка открыть несуществующее окно!");
            }
            bool isButtonClick = window.ShowDialog() ?? false; //Если метод возвращает null, то в переменную запишется false.
            if (isButtonClick)
            {
                if(window is OpenTestForEdit)
                {
                    questions = window.testList;
                    listBox1.ItemsSource = questions;
                }
                else if(window is ExitFromEdit)
                {
                    if (!window.saveTest)
                    {
                        questions.Clear();
                    }
                }
                Button_Click_Switch(sender, e); //Если не были учтены условия для перехода в другое окно, то перехода не будет
            }
        }
        private void AddTest()
        {
            //questions.Add(DTest.GetTest().quests); //TODO: это надо будет потом поменять, когда буду готовы конструкции разных вопросов
            //testList.ItemsSource = questions;
            DQuest quest = new();
            quest.answers = new ObservableCollection<DAnswer> { new DAnswer() };
            quest.Number = questions.Count + 1;
            questions.Add(quest);
            listBox1.ItemsSource = questions;
        }
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Событие для прокрутки вопросов
            //При наводке курсора на listbox, прокрутка не происходила
            //Этот метод решает проблему данную проблему
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            //Событие, с которого начинается добавление вопроса в тест
            switch (comboBox1.Text) //TODO: потом это переделать, конструкции должны быть разными
            {
                case StringTypeQuestion.OPEN_ANSWER:
                    AddTest();
                    break;
                case StringTypeQuestion.SELECTIVE_ANSWER:
                    AddTest();
                    break;
                case StringTypeQuestion.MATCHING_SEARCH:
                    AddTest();
                    break;
                case StringTypeQuestion.DATA_INPUT:
                    AddTest();
                    break;
            }
            comboBox1.Text = string.Empty;
        }
        private void Заглушка(object sender, RoutedEventArgs e)
        {
            //TODO: везде, где используется заглушка, нужно разработать необходимый функционал
            MessageBox.Show("Эта кнопка пока не работает :(");
        }

        private void Button_Click_AnswerAdd(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int number = (int)button.Tag;
            questions[number - 1].answers.Add(new DAnswer());
            listBox1.ItemsSource = questions;
        }
    }
}
