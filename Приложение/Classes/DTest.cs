using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Приложение
{
    [Serializable]
    public class DTest //TODO: надо будет заменить этот класс
    {
        public string name = "";
        public ObservableCollection<DQuest> quests = new();
        private readonly string password = "";
        public DTest() { }
        public DTest(string password, string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            DataContractSerializer ser = new DataContractSerializer(typeof(DTest));
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            // Deserialize the data and read it from the instance.
            DTest deserialized = (DTest)ser.ReadObject(reader, true);
            if (Encryption(password) == deserialized.password)
            {
                this.password = deserialized.password;
                this.name = deserialized.name;
                this.quests = deserialized.quests;
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

        // Приложение\Tests(который pathToMainDirectory)\%название_теста%(которое this.name)\Test.XML
        public void Save(string pathToMainDirectory)
        {
            
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
        public string Type { get {  return _type; } set { _type = value; } }
        public ObservableCollection<DAnswer> Answers { get { return _answers; } set { _answers = value; } }
        public int Number { get { return _number; } set { _number = value; } }
        public bool AnswerRequired { get { return _answerRequired; } set { _answerRequired = value; } }
        public double Price { get { return _price; } set { _price = value; } }
        #endregion Свойства
    }
    public class DAnswer
    {
        private string _answer = "";
        public string Answer { get { return _answer; } set { _answer = value; } }
    }
    
}
