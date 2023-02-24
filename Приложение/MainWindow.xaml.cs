﻿using System;
using System.Collections.Generic;
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
        readonly public DirectoryInfo mainDirectory = new DirectoryInfo("Tests");
        public MainWindow()
        {
            InitializeComponent();           
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
                Button_Click_Switch(sender, e); //Если не были учтены условия для перехода в другое окно, то перехода кнш не будет
            }
        }
        private void Заглушка(object sender, RoutedEventArgs e)
        {
            //TODO: везде, где используется заглушка, нужно разработать необходимый функционал
            MessageBox.Show("Эта кнопка пока не работает :(");
        }
    }
}
