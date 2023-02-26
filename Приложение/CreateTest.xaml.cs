﻿using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Приложение
{
    /// <summary>
    /// Логика взаимодействия для CreateTest.xaml
    /// </summary>
    public partial class CreateTest : Window
    {
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
                DialogResult = true; //Окно закрывается
            }
        }
    }
}
