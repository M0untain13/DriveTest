﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;
using Приложение.Classes.Enums;
using Приложение.Classes.FactoryMethods;

namespace Приложение.Classes.Models
{
    public class Kostil
    {
        public static Kostil instant;
        private Kostil()
        {
            instant = new Kostil();
        }
        public static Kostil Instant => instant ??= new Kostil();

        public bool isVisible = false;
    }

    [DataContract]
    public class DResult
    {
        public static IEnumerable<Type> listOfTypes = AbstractDResultsAnswer.listOfTypes;
        public Kostil kostil = Kostil.instant;

        [DataMember] private List<AbstractDResultsAnswer> _answers = new();
        [DataMember] private string _nameOfTest = "";
        [DataMember] private string _nameOfPeople = "";

        public void GetVisible()
        {
            foreach (var answer in Answers)
            {
                answer.IsVisible = true;
            }
        }
        public List<AbstractDResultsAnswer> Answers { get => _answers; set => _answers = value; }
        public string NameOfTest { get => _nameOfTest; set => _nameOfTest = value; }
        public string NameOfPeople { get => _nameOfPeople; set => _nameOfPeople = value; }
    }
    [DataContract]
    public abstract class AbstractDResultsAnswer
    {
        public static IEnumerable<Type> listOfTypes =
            new List<Type> 
            { 
                typeof(DResultsAnswerOpen), 
                typeof(DResultsAnswerSelectiveOne), 
                typeof(DResultsAnswerSelectiveMultiple), 
                typeof(DResultsAnswerMatchingSearch), 
                typeof(DResultsAnswerDataInput) 
            };
        [DataMember] private string _name = ""; //Вопрос
        [DataMember] private double _score = 0; //Баллы
        [DataMember] private string _type = "";
        public bool isVisible = false;
        public bool IsVisible
        {
            get => isVisible; 
            set => isVisible = value;
        }
        /// <summary>
        /// Название вопроса
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        /// <summary>
        /// Кол-во баллов за ответ
        /// </summary>
        public string Score
        {
            get => (_score * Сorrectness).ToString("f2");
            set {}
        }

        public string MaxScore
        {
            get => _score.ToString("f2"); 
            set {}
        }

        public string Type => _type;

        /// <summary>
        /// Процент корректности данного ответа от 0 до 1
        /// </summary>
        protected virtual double Сorrectness => 0;
        /// <summary>
        /// Предполагается список вариантов ответа
        /// </summary>
        public virtual IEnumerable<string> ArrayOfString01 { get { return Enumerable.Empty<string>();} set {} }
        /// <summary>
        /// Предполагается список ответов данных пользователем
        /// </summary>
        public virtual IEnumerable<string> ArrayOfString02 { get { return Enumerable.Empty<string>(); } set {} }
        /// <summary>
        /// Предполагается список верных ответов
        /// </summary>
        public virtual IEnumerable<string> ArrayOfString03 { get { return Enumerable.Empty<string>(); } set {} }
        public void SetObject(DQuest quest)
        {
            _name = quest.Name;
            _score = quest.Price;
            _type = quest.Type;
            SetCorrectAnswers(quest);
        }
        public virtual void SetCorrectAnswers(DQuest quest) { }
        public virtual void SetTestingAnswers(DQuest quest) { }
    }
    /// <summary>
    /// Несколько вариантов и все верные
    /// В ответе может быть дано несколько ответов, могут быть неверными и верными
    /// На вывод: только данный ответ
    /// </summary>
    [DataContract]
    public class DResultsAnswerOpen : AbstractDResultsAnswer
    {
        [DataMember] private readonly HashSet<string> _correctAnswers = new();
        [DataMember] private string _answerFromTesting = "";
        protected override double Сorrectness 
        { 
            get 
            {
                var arrayOfAnswerFromTesting = _answerFromTesting.Split();
                var countOfErrors = arrayOfAnswerFromTesting.Length - (from answer in arrayOfAnswerFromTesting
                                                                      where _correctAnswers.Contains(answer)
                                                                      select answer).Count();
                var countOfCorrectAnswers = arrayOfAnswerFromTesting.Length-countOfErrors;
                //Объяснение данной формулы есть на гитхабе в roadmap в выполненной задаче "Классы внутренней логики"
                return 1.0 * countOfCorrectAnswers/ _correctAnswers.Count * (1 - countOfErrors / _correctAnswers.Count); 
            } 
        }
        public override IEnumerable<string> ArrayOfString02
        {
            get
            {
                return _answerFromTesting.Split();
            }
        }
        public override IEnumerable<string> ArrayOfString03 => _correctAnswers;
        public override void SetCorrectAnswers(DQuest quest)
        {
            var answers = quest.Answers;
            foreach(var answer in answers)
            {
                _correctAnswers.Add(answer.Answer1);
            }
        }
        public override void SetTestingAnswers(DQuest quest)
        {
            var answers = quest.Answers;
            _answerFromTesting = answers.ToArray()[0].Answer2;
        }
    }

    /// <summary>
    /// Несколько вариантов и один верный
    /// В ответе может быть дан только один ответ
    /// На вывод: Варианты и данный ответ
    /// </summary>
    [DataContract]
    public class DResultsAnswerSelectiveOne : AbstractDResultsAnswer
    {
        [DataMember] private readonly List<string> _answers = new();
        [DataMember] private string _correctAnswer = "";
        [DataMember] private string _receivedAnswer = "";
        protected override double Сorrectness => _correctAnswer == _receivedAnswer ? 1 : 0;
        public override IEnumerable<string> ArrayOfString01 => _answers;
        public override IEnumerable<string> ArrayOfString02 { get { yield return _receivedAnswer; }
        }
        public override IEnumerable<string> ArrayOfString03 => new List<string>() { _correctAnswer };
        public override void SetCorrectAnswers(DQuest quest)
        {
            var answers = quest.Answers;
            foreach (var answer in answers)
            {
                _answers.Add(answer.Answer1);
                if(answer.IsCorrect)
                {
                    _correctAnswer = answer.Answer1;
                }
            }
        }
        public override void SetTestingAnswers(DQuest quest)
        {
            var answers = quest.Answers;
            foreach (var answer in answers.Where(answer => answer.IsMarkedByUser))
            {
                _receivedAnswer = answer.Answer1;
            }
        }
    }

    /// <summary>
    /// Несколько вариантов и несколько верных
    /// В ответе может быть дано несколько ответов
    /// На вывод: Варианты и данные ответы
    /// </summary>
    [DataContract]
    public class DResultsAnswerSelectiveMultiple : AbstractDResultsAnswer
    {
        [DataMember] private readonly HashSet<string> _answers = new();
        [DataMember] private readonly HashSet<string> _correctAnswers = new();
        [DataMember] private readonly HashSet<string> _receivedAnswers = new();
        protected override double Сorrectness 
        {
            get
            {
                var countOfCorrectAnswers = (from answer in _receivedAnswers
                                             where _correctAnswers.Contains(answer)
                                             select answer).Count();
                var countOfErrors = _receivedAnswers.Count - countOfCorrectAnswers;

                //Объяснение данной формулы есть на гитхабе в roadmap в выполненной задаче "Классы внутренней логики"
                return 1.0 * countOfCorrectAnswers / _correctAnswers.Count * (1 - countOfErrors / _answers.Count);
            }
        }
        public override IEnumerable<string> ArrayOfString01 => _answers;
        public override IEnumerable<string> ArrayOfString02 => _receivedAnswers;
        public override IEnumerable<string> ArrayOfString03 => _correctAnswers;
        public override void SetCorrectAnswers(DQuest quest)
        {
            var answers = quest.Answers;
            foreach (var answer in answers)
            {
                _answers.Add(answer.Answer1);
                if (answer.IsCorrect)
                {
                    _correctAnswers.Add(answer.Answer1);
                }
            }
        }
        public override void SetTestingAnswers(DQuest quest)
        {
            var answers = quest.Answers;
            foreach (var answer in answers.Where(answer => answer.IsMarkedByUser))
            {
                _receivedAnswers.Add(answer.Answer1);
            }
        }
    }
    /// <summary>
    /// Пары ответов
    /// На вывод: Данная пара ответов
    /// </summary>
    [DataContract]
    public class DResultsAnswerMatchingSearch : AbstractDResultsAnswer
    {
        [DataMember] private readonly Dictionary<string, string> _correctAnswers = new();
        [DataMember] private readonly Dictionary<string, string> _receivedAnswers = new();
        protected override double Сorrectness
        {
            get
            {
                var countOfCorrectAnswers = (from answer in _receivedAnswers
                                             where _correctAnswers[answer.Key] == _receivedAnswers[answer.Key]
                                             select answer).Count();
                var countOfErrors = _receivedAnswers.Count - countOfCorrectAnswers;

                //Объяснение данной формулы есть на гитхабе в roadmap в выполненной задаче "Классы внутренней логики"
                return 1.0 * countOfCorrectAnswers / _correctAnswers.Count * (1 - countOfErrors / _correctAnswers.Count);
            }
        }
        /// <summary>
        /// Возвращаем первый список данных ответов, которые являются парами ответам из второго списка
        /// </summary>
        public override IEnumerable<string> ArrayOfString01 => _receivedAnswers.Keys;
        /// <summary>
        /// Возвращаем второй список данных ответов, которые являются парами ответам из первого списка
        /// </summary>
        public override IEnumerable<string> ArrayOfString02 => _receivedAnswers.Values;
        public override IEnumerable<string> ArrayOfString03 => _correctAnswers.Values;
        public override void SetCorrectAnswers(DQuest quest)
        {
            var answers = quest.Answers;
            foreach (var answer in answers)
            {
                _correctAnswers.Add(answer.Answer1, answer.Answer2);
            }
        }

        public override void SetTestingAnswers(DQuest quest)
        {
            var answers = quest.Answers;
            foreach (var answer in answers)
            {
                _receivedAnswers.Add(answer.Answer1, answer.Answer2);
            }
        }
    }
    /// <summary>
    /// Просто строка
    /// На вывод: Данная строка
    /// </summary>
    [DataContract]
    public class DResultsAnswerDataInput : AbstractDResultsAnswer
    {
        [DataMember] private string _info = "";
        public override IEnumerable<string> ArrayOfString01 { get { yield return _info; } }
        public override void SetTestingAnswers(DQuest quest)
        {
            var answers = quest.Answers;
            _info = answers.ToArray()[0].Answer1;
        }
    }
}
