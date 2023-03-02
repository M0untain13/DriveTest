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
        readonly public DirectoryInfo mainDirectory = new DirectoryInfo("Tests");                       //Главный каталог, где будут лежать тесты.
        private ObservableCollection<DQuest> questions = new();                                         //Коллекция вопросов в тесте.
        readonly private ObservableCollection<string> addQuestion = new ObservableCollection<string>    //Строки для добавления вопросов в тест.
        {
            StringTypeQuestion.OPEN_ANSWER,
            StringTypeQuestion.SELECTIVE_ANSWER,
            StringTypeQuestion.MATCHING_SEARCH,
            StringTypeQuestion.DATA_INPUT
        };
        public MainWindow()
        {
            InitializeComponent();
            listBox1.ItemsSource = questions;
            comboBox1.ItemsSource = addQuestion;
        }

        /// <summary>
        /// Сменить поверхности\сетки\grid.
        /// </summary>
        /// <param name="current"> Данная поверхность </param>
        /// <param name="next"> Новая поверхность </param>
        private void Switching(Grid current, Grid next)
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
            Switching(button.Parent as Grid, button.Tag as Grid);
        }

        /// <summary>
        /// Нажатие какой-либо кнопки, приводящее к открытию промежуточного окна.
        /// </summary>
        /// <param name="sender"> Объект кнопки </param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void Button_Click_IntermediateWindow(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            IDriveTestWindow window = button.DataContext as IDriveTestWindow; //Из DataContext мы получаем тип окна.
            window = window.Init(mainDirectory); //Инициируем окно согласно его типу.
            bool isButtonClick = window.ShowDialog() ?? false; //Если метод возвращает null, то в переменную запишется false.
            if (isButtonClick)
            {
                window.Commands(ref textBox1, ref listBox1, ref questions); //Комманды, которое должны выполниться над элементами главного окна
                Button_Click_Switch(sender, e);
            }
        }

        /// <summary>
        /// Добавить вопрос в редакторе
        /// </summary>
        private void AddQuestion()
        {
            //questions.Add(DTest.GetTest().quests); //TODO: это надо будет потом поменять, когда будут готовы конструкции разных вопросов.
            //testList.ItemsSource = questions;
            DQuest quest = new();
            quest.answers = new ObservableCollection<DAnswer> { new DAnswer() };
            quest.Number = questions.Count + 1;
            questions.Add(quest);
            listBox1.ItemsSource = questions;
        }

        /// <summary>
        /// Прокрутка колёсиком мышки.
        /// Без этого события, прокрутка не происходит при наводке на lixtBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
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
            switch (comboBox1.Text) //TODO: потом это переделать, конструкции должны быть разными
            {
                case StringTypeQuestion.OPEN_ANSWER:
                    AddQuestion();
                    break;
                case StringTypeQuestion.SELECTIVE_ANSWER:
                    AddQuestion();
                    break;
                case StringTypeQuestion.MATCHING_SEARCH:
                    AddQuestion();
                    break;
                case StringTypeQuestion.DATA_INPUT:
                    AddQuestion();
                    break;
            }
            comboBox1.Text = string.Empty;
        }
        private void Заглушка(object sender, RoutedEventArgs e)
        {
            //TODO: везде, где используется заглушка, нужно разработать необходимый функционал
            MessageBox.Show("Эта кнопка пока не работает :(");
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
            ObservableCollection<DAnswer> answers = questions[number - 1].answers;
            if (answers.Count < 10)
            {
                answers.Add(new DAnswer());
                listBox1.ItemsSource = questions;
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
            ObservableCollection<DAnswer> answers = questions[number - 1].answers;
            if(answers.Count > 1)
            {
                answers.RemoveAt(answers.Count - 1);
                listBox1.ItemsSource = questions;
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
            questions.RemoveAt(number - 1);
            for(int i = 0; i < questions.Count; i++)
            {
                questions[i].number = i + 1;
            }
            listBox1.ItemsSource = questions;
            listBox1.Items.Refresh();
        }
    }
}
