using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Приложение.Classes;

namespace Приложение.Windows
{
    /// <summary>
    /// Логика взаимодействия для OpenResults.xaml
    /// </summary>
    public partial class OpenResults : Window, IDriveTestWindow
    {
        public List<DirectoryInfo> testsDir = new List<DirectoryInfo>();
        public List<object> students = new List<object>();
        public string mainDirectory = "";
        public OpenResults()
        {
            InitializeComponent();
            passwordBox.MaxLength = 20;
        }
        public OpenResults(DirectoryInfo mainDirectory)
        {
            InitializeComponent();
            passwordBox.MaxLength = 20;
            mainDirectory.GetDirectories().ToList().ForEach(directory => { testsDir.Add(directory); });
            this.mainDirectory = mainDirectory.ToString();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            testsDir.ToList().ForEach(dir => { choseTest.Items.Add($"{dir.Name}"); });
        }

        void IDriveTestWindow.Commands(ref TextBox textBoxText, ref TextBox textBoxTime, ref ListBox listBox, ref DTest test, MainWindow w)
        {
            //Пока ничего нет, но тут должно что-то быть
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (choseTest.Text == string.Empty)
            {
                warning.Text = "Тест не выбран!";
            }
        }
    }
}
