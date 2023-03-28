using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml;

namespace Приложение.Classes
{
    /// <summary>
    /// Описание структуры теста
    /// </summary>
    [DataContract]
    public class DTest
    {
        #region Поля

        [DataMember] public string name = "";
        [DataMember] public ObservableCollection<DQuest> quests = new();
        [DataMember] public int time = 0; //Время для решения теста, в минутах, если 0 - то время неограничено
        [DataMember] private readonly string _password = "";

        private static readonly DataContractSerializerSettings _settings = new DataContractSerializerSettings
        {
            KnownTypes = new List<Type> { typeof(DQuest), typeof(MatrixTransform)}.Concat(AbstractAnswerFactoryMethod.listOfTypesFactory),
            PreserveObjectReferences = true,
        };
        private readonly DataContractSerializer _serializer =
            new(typeof(DTest), _settings);
        //TODO: надо бы изучить момент, при успешном открытии _isOpened становиться true, остаётся ли он true, при сохранении?
        private readonly bool _isOpened = false; //Индикатор того, что тест создан правильно или открыт из файла

        #endregion

        public bool IsOpened
        {
            get { return _isOpened; }
        }

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
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            DTest deserialized = _serializer.ReadObject(reader, true) as DTest;
            if (Encryption(password) == deserialized._password)
            {
                _password = deserialized._password;
                name = deserialized.name;
                quests = deserialized.quests;
                time = deserialized.time;
                _isOpened = true;
            }

            reader.Close();
            fs.Close();
        }

        public DTest()
        {
        }

        public void OpenTestForTesting(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            DTest deserialized = _serializer.ReadObject(reader, true) as DTest;
            name = deserialized.name;
            quests = deserialized.quests;
            time = deserialized.time;
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
            XmlWriter reader = XmlWriter.Create(fs);
            _serializer.WriteObject(reader,
                this);
            reader.Close();
            fs.Close();
        }

        #endregion
    }

    [DataContract]
    public class DQuest
    {
        #region Поля

        [DataMember] public string name = "";
        [DataMember] private string _type = "";
        [DataMember] private ObservableCollection<AbstractAnswer> _answers = null;
        [DataMember] private int _number = -1;
        [DataMember] private bool _answerRequired = true;
        [DataMember] private double _price = 0;
        [DataMember] private AbstractAnswerFactoryMethod _factoryMethod;
        private ListBox _listBox;

        #endregion Поля

        #region Свойства

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public ObservableCollection<AbstractAnswer> Answers
        {
            get { return _answers; }
            set { _answers = value; }
        }

        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public bool AnswerRequired
        {
            get
            {
                return _answerRequired;
            }
            set { _answerRequired = value; }
        }

        public double Price
        {
            get { return _price; }
            set { _price = value; }
        }

        public ListBox ListBox
        {
            get => _listBox;
            set => _listBox = value;
        }

        public AbstractAnswerFactoryMethod FactoryMethod
        {
            get { return _factoryMethod; }
        }

        /// <summary>
        /// Это для формы ввода данных
        /// </summary>
        public string InputData
        {
            get { return _answers[0].Answer1; } 
            set { _answers[0].Answer1 = value; }
        }

        #endregion Свойства

        public DQuest(AbstractAnswerFactoryMethod factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }

        public void ResetCorrect()
        {
            foreach (var answer in _answers)
            {
                answer.IsCorrect = false;
            }
            _listBox.Items.Refresh();
        }

        public void ResetMarked()
        {
            foreach (var answer in _answers)
            {
                answer.IsMarkedByUser = false;
            }
            _listBox.Items.Refresh();
        }
    }

    [DataContract]
    public abstract class AbstractAnswer
    {
        public static List<Type> ListOfTypesAnswer = new List<Type> { typeof(DAnswerPair), typeof(DAnswerOne), typeof(DAnswer) };
        public virtual string Answer1 { get; set;}
        public virtual string Answer2 { get; set; }
        public virtual bool IsCorrect { get; set; }
        public virtual bool IsMarkedByUser { get; set; }
        public virtual DQuest Parrent { get; set; }
    }

    [DataContract]
    public class DAnswer : AbstractAnswer
    {
        [DataMember] private string _answer = "";
        [DataMember] private bool _isCorrect = false;
        private bool _isMarkedByUser = false;
        public override string Answer1 { get { return _answer; } set { _answer = value; } }
        public override string Answer2 { get { return null; } set { } }

        public override bool IsCorrect
        {
            get { return _isCorrect; } 
            set { _isCorrect = value; }
        }
        public override bool IsMarkedByUser
        {
            get { return _isMarkedByUser; }
            set { _isMarkedByUser = value; }
        }
    }

    [DataContract]
    public class DAnswerOne : AbstractAnswer
    {
        [DataMember] private string _answer = "";
        [DataMember] private bool _isCorrect = false;
        [DataMember] private DQuest _parrent;
        private bool _isMarkedByUser = false;
        public override DQuest Parrent { get => _parrent; set => _parrent = value; }
        public override string Answer1 { get { return _answer; } set { _answer = value; } }
        public override string Answer2 { get { return null; } set { } }
        public override bool IsCorrect 
        { 
            get { return _isCorrect; }
            set
            {
                if (value)
                {
                    _parrent.ResetCorrect();
                }
                _isCorrect = value;
            } 
        }
        public override bool IsMarkedByUser
        {
            get { return _isMarkedByUser; }
            set
            {
                if (value)
                {
                    _parrent.ResetMarked();
                }
                _isMarkedByUser = value;
            }
        }
    }

    [DataContract]
    public class DAnswerPair : AbstractAnswer
    {
        [DataMember] private string _answer1 = "";
        [DataMember] private string _answer2 = "";
        private string _answerUser2 = "";
        private bool _isMixed = false;
        public override bool IsMarkedByUser { get => _isMixed; set => _isMixed = value; }
        public override string Answer1 
        { 
            get 
            { 
                return _answer1; 
            } 
            set 
            {
                _answer1 = value;
            } 
        }
        public override string Answer2 
        { 
            get 
            {
                if (_isMixed)
                {
                    return _answerUser2;
                }
                return _answer2;
            }
            set
            {
                if (_isMixed)
                {
                    _answerUser2 = value;
                }
                else
                {
                    _answer2 = value;
                }
            }
        }
        public override bool IsCorrect { get { return false; } set { } }
    }
}
