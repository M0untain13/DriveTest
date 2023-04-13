using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.Windows.Media;
using Приложение.Classes.FactoryMethods;

namespace Приложение.Classes.Models
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
        [DataMember] private readonly string _password;

        /// <summary>
        /// Коллекция известных типов для сериализации
        /// </summary>
        public static IEnumerable<Type> listOfTypes = 
            new List<Type> { typeof(DQuest), typeof(MatrixTransform) }
            .Concat(AbstractFactoryMethod.listOfTypesFactory);

        #endregion

        public DTest(string password)
        {
            _password = Encryption(password);
        }


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
        /// Метод для сверки пароля
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckPass(string password)
        {
            return Encryption(password) == _password;
        }

        public DResult CreateResult(string namePeople)
        {
            return new DResult(name, namePeople, _password);
        }
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
        [DataMember] private AbstractFactoryMethod _factoryMethod;
        private ListBox _listBox;

        #endregion Поля

        #region Свойства

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Type
        {
            get => _type;
            set => _type = value;
        }

        public ObservableCollection<AbstractAnswer> Answers
        {
            get => _answers;
            set => _answers = value;
        }

        public int Number
        {
            get => _number;
            set => _number = value;
        }

        public bool AnswerRequired
        {
            get => _answerRequired; 
            set => _answerRequired = value;
        }

        public string AnswerRequiredText => _answerRequired ? "Ответ обязателен" : string.Empty;

        public double Price
        {
            get => _price;
            set => _price = value;
        }

        public ListBox ListBox
        {
            get => _listBox;
            set => _listBox = value;
        }

        public AbstractFactoryMethod FactoryMethod => _factoryMethod;

        /// <summary>
        /// Это для формы ввода данных
        /// </summary>
        public string InputData
        {
            get => _answers[0].Answer1; 
            set => _answers[0].Answer1 = value;
        }
        /// <summary>
        /// Это для открытого ответа
        /// </summary>
        public string OpenAnswer { get => _answers[0].Answer2; set => _answers[0].Answer2 = value; }

        #endregion Свойства

        public DQuest(AbstractFactoryMethod factoryMethod)
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
        /// <summary>
        /// Коллекция известных типов для сериализации
        /// </summary>
        public static List<Type> listOfTypesAnswer = new List<Type> { typeof(DAnswerPair), typeof(DAnswerOne), typeof(DAnswer) };
        public virtual string Answer1 { get; set;}
        public virtual string Answer2 { get; set; }
        // public virtual string Index1 { get; set; }
        // public virtual string Index2 { get; set; }
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
        public override string Answer2 { get; set; }

        public override bool IsCorrect
        {
            get => _isCorrect; 
            set => _isCorrect = value;
        }
        public override bool IsMarkedByUser
        {
            get => _isMarkedByUser;
            set => _isMarkedByUser = value;
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
            get => _isCorrect;
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
            get => _isMarkedByUser;
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
            get => _answer1;
            set => _answer1 = value;
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
        public override bool IsCorrect 
        { 
            get => false;
            set { }
        }
    }
}
