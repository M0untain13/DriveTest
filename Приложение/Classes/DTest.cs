using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml;

namespace Приложение
{
    [Serializable]
    public class DTest //TODO: надо будет заменить этот класс
    {
        public string name = "";
        public ObservableCollection<DQuest> quests = new();
        private readonly string _password = "";
        private readonly bool _isOpened = false;
        public bool IsOpened { get { return _isOpened; } }
        public DTest(string password) 
        {
            _password = Encryption(password);
            _isOpened = true;
        }
        public DTest(string password, string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            DataContractSerializer ser = new DataContractSerializer(typeof(DTest));
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            DTest deserialized = ser.ReadObject(reader, true) as DTest;
            if (Encryption(password) == deserialized._password)
            {
                _password = deserialized._password;
                name = deserialized.name;
                quests = deserialized.quests;
                _isOpened = true;
            }
            reader.Close();
            fs.Close();
        }

        private string Encryption(string password)
        {
            string newPassword = "";
            for (int i = 0; i < password.Length; i++)
            {
                newPassword += password[i] + i;
            }
            return newPassword;
        }

        public void Save(string pathToMainDirectory)
        {
            string path = pathToMainDirectory + "\\" + name;
            Directory.CreateDirectory(path);
            path += "\\Test.XML";
            FileStream fs = new FileStream(path, FileMode.Create);
            DataContractSerializer ser = new DataContractSerializer(typeof(DTest));
            XmlWriter reader = XmlWriter.Create(fs);
            ser.WriteObject(reader, this);
            reader.Close();
            fs.Close();
        }
    }
    public class DQuest
    {
        #region Поля
        public string name = "";
        private string _type = "";
        private ObservableCollection<DAnswer> _answers = new();
        private int _number = -1;
        private bool _answerRequired = true;
        private double _price = 0;
        #endregion Поля

        #region Свойства
        public string Name { get { return name; } set { name = value; } }
        public string Type { get {  return _type; } set { _type = value; } }
        public ObservableCollection<DAnswer> Answers { get { return _answers; } set { _answers = value; } }
        public int Number { get { return _number; } set { _number = value; } }
        public bool AnswerRequired { 
            get 
            { 
                if(_type == StringTypeQuestion.OPEN_ANSWER)
                    return true;
                return _answerRequired; 
            } 
            set { _answerRequired = value; } }
        public double Price { get { return _price; } set { _price = value; } }
        #endregion Свойства
    }
    public class DAnswer
    {
        private string _answer = "";
        private bool _isCorrect = false;
        public string Answer { get { return _answer; } set { _answer = value; } }
        public bool IsCorrect { get { return _isCorrect; } set { _isCorrect = value; } }
    }
    
}
