using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Приложение.Classes
{
    /// <summary>
    /// Описание структуры теста
    /// TODO: нужно сделать фабричный метод для объектов ответа
    /// </summary>
    [DataContract]
    public class DTest
    {
        #region Поля
        [DataMember]
        public string name = "";
        [DataMember]
        public ObservableCollection<DQuest> quests = new();
        [DataMember]
        private readonly string _password = "";
        [DataMember]
        private readonly bool _isOpened = false; //Индикатор того, что тест создан правильно или открыт из файла
        #endregion
        public bool IsOpened { get { return _isOpened; } }
        #region Конструкторы
        /// <summary>
        /// Конструктор для создания нового теста
        /// </summary>
        /// <param name="password"></param>
        public DTest(string password) 
        {
            _password = Encryption(password);
            _isOpened = true;
        }

        /// <summary>
        /// Конструктор для открытия существующего теста
        /// </summary>
        /// <param name="password"></param>
        /// <param name="path"></param>
        public DTest(string password, string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            DataContractSerializer ser = new DataContractSerializer(typeof(DTest), new List<Type> { typeof(DQuest), typeof(DAnswerPair), typeof(DAnswerOne), typeof(DAnswer) });
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
        #endregion
        #region Методы
        /// <summary>
        /// Метод для шифровки пароля
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private string Encryption(string password)
        {
            string newPassword = "";
            for (int i = 0; i < password.Length; i++)
            {
                newPassword += password[i] + i;
            }
            return newPassword;
        }

        /// <summary>
        /// Метод для сохранения теста
        /// </summary>
        /// <param name="pathToMainDirectory"></param>
        public void Save(string pathToMainDirectory)
        {
            string path = pathToMainDirectory + "\\" + name;
            Directory.CreateDirectory(path);
            path += "\\Test.XML";
            FileStream fs = new FileStream(path, FileMode.Create);
            DataContractSerializer ser = new DataContractSerializer(typeof(DTest), new List<Type> { typeof(DQuest), typeof(DAnswerPair), typeof(DAnswerOne), typeof(DAnswer) });
            XmlWriter reader = XmlWriter.Create(fs);
            ser.WriteObject(reader, this); //TODO: сериализация пошла по одному месту из-за наличия ссылки у ответов на вопрос
            reader.Close();
            fs.Close();
        }
        #endregion
    }
    [DataContract]
    public class DQuest
    {
        #region Поля
        [DataMember]
        public string name = "";
        [DataMember]
        private string _type = "";
        [DataMember]
        private ObservableCollection<IAnswer> _answers = null;
        [DataMember]
        private int _number = -1;
        [DataMember]
        private bool _answerRequired = true;
        [DataMember]
        private double _price = 0;
        #endregion Поля

        #region Свойства
        public string Name { get { return name; } set { name = value; } }
        public string Type { get {  return _type; } set { _type = value; } }
        public ObservableCollection<IAnswer> Answers { get { return _answers; } set { _answers = value; } }
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

        public void ReserCorrect()
        {
            foreach(IAnswer answer in _answers)
            {
                answer.IsCorrect = false;
            }
        }
    }
    public interface IAnswer
    {
        public string Answer1 { get; set;}
        public string Answer2 { get; set; }
        public bool IsCorrect { get; set; }
        public IAnswer Init(DQuest quest);
    }
    [DataContract]
    public class DAnswer : IAnswer
    {
        [DataMember]
        private string _answer = "";
        [DataMember]
        private bool _isCorrect = false;
        public string Answer1 { get { return _answer; } set { _answer = value; } }
        public string Answer2 { get { return null; } set { } }
        public bool IsCorrect { get { return _isCorrect; } set { _isCorrect = value; } }
        public IAnswer Init(DQuest quest)
        {
            return new DAnswer();
        }
    }
    [DataContract]
    public class DAnswerOne : IAnswer
    {
        [DataMember]
        private string _answer = "";
        [DataMember]
        private bool _isCorrect = false;
        [DataMember]
        private readonly DQuest _quest = null;
        public DAnswerOne(DQuest quest)
        {
            _quest = quest;
        }
        public string Answer1 { get { return _answer; } set { _answer = value; } }
        public string Answer2 { get { return null; } set { } }
        public bool IsCorrect 
        { 
            get 
            { return _isCorrect; } 
            set 
            {
                if (value)
                {
                    _quest.ReserCorrect();
                }
                _isCorrect = value; 
            } 
        }
        public IAnswer Init(DQuest quest)
        {
            return new DAnswerOne(quest);
        }
    }
    [DataContract]
    public class DAnswerPair : IAnswer
    {
        [DataMember]
        private string _answer1 = "";
        [DataMember]
        private string _answer2 = "";
        public string Answer1 { get { return _answer1; } set { _answer1 = value; } }
        public string Answer2 { get { return _answer2; } set { _answer2 = value; } }
        public bool IsCorrect { get { return false; } set { } }
        public IAnswer Init(DQuest quest)
        {
            return new DAnswerPair();
        }
    }
}
