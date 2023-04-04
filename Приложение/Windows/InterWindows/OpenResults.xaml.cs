using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Приложение.Classes;
using Приложение.Classes.Models;
using Приложение.Classes.Services;

namespace Приложение.Windows.InterWindows
{
    /// <summary>
    /// Логика взаимодействия для OpenResults.xaml
    /// </summary>
    public partial class OpenResults : Window, IDriveTestWindow
    {
        public DResult result;
        public List<DirectoryInfo> testsDir = new List<DirectoryInfo>();
        public List<FileInfo> students = new List<FileInfo>();
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
            if (choseTest.Items.IsEmpty)
                testsDir.ToList().ForEach(dir => { choseTest.Items.Add(dir); });
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (choseTest.Text == string.Empty)
            {
                warning.Text = "Тест не выбран!";
                return;
            }
            else if (choseStudent.Text == string.Empty)
            {
                warning.Text = "Результат не выбран!";
                return;
            }
            else
            {
                result = Loader.LoadResult($"{mainDirectory}\\{choseTest.Text}\\Results", choseStudent.Text);
                DialogResult = true;
            }
        }

        private void choseTest_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (sender is not ComboBox comboBox)
                    throw new Exception("Неверный тип объекта");
                if (comboBox.Text == "")
                    return;
                if (comboBox.SelectionBoxItem is not DirectoryInfo directoryInfo)
                    throw new Exception("Неверный тип объекта");
                if (directoryInfo
                    .GetDirectories()
                    .FirstOrDefault(d => d.Name == "Results") is not {} resDir)
                    throw new Exception("Результатов нет");
                if (resDir.GetFiles().Length == 0)
                    throw new Exception("Результатов нет");
                resDir.GetFiles().ToList().ForEach(d => { students.Add(d); });
                students.ToList().ForEach(d => { choseStudent.Items.Add($"{Path.GetFileNameWithoutExtension(d.Name)}"); });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                choseTest.Text = string.Empty;
            }
        }

        void IDriveTestWindow.Commands(ref DTest test, ref DResult result, ref bool isSaveTest)
        {
            result = this.result;
        }
    }
}
