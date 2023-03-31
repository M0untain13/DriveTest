using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;

namespace Приложение.Classes
{
    public class DResult
    {
        private List<AbstractDResultsAnswer> _answers = new();
        public List<AbstractDResultsAnswer> Answers { get => _answers; set => _answers = value; }
    }
    /// <summary>
    /// На вывод: Имя, баллы * процент_корректности, варианты ответов при необходимости, данный ответ
    /// </summary>
    public abstract class AbstractDResultsAnswer
    {
        private string _name = ""; //Вопрос
        private double _score = 0; //Баллы
        /// <summary>
        /// Название вопроса
        /// </summary>
        public string Name { get => _name; set => _name = value; }
        /// <summary>
        /// Кол-во баллов за ответ
        /// </summary>
        public double Score { get => _score * Сorrectness; set => _score = value; }
        /// <summary>
        /// Процент корректности данного ответа от 0 до 1
        /// </summary>
        protected virtual double Сorrectness => 0;
        /// <summary>
        /// Предполагается список вариантов ответа
        /// </summary>
        public virtual IEnumerable<string> ArrayOfString01 { get { return Enumerable.Empty<string>();} }
        /// <summary>
        /// Предполагается список ответов данных пользователем
        /// </summary>
        public virtual IEnumerable<string> ArrayOfString02 { get { return Enumerable.Empty<string>(); } }
        public void SetObject(DQuest quest)
        {
            _name = quest.Name;
            _score = quest.Price;
            SetCorrectAnswers(quest.Answers);
        }
        public virtual void SetCorrectAnswers(IEnumerable<AbstractAnswer> answers) { }
        public virtual void SetTestingAnswers(IEnumerable<AbstractAnswer> answers) { }
    }
    /// <summary>
    /// Несколько вариантов и все верные
    /// В ответе может быть дано несколько ответов, могут быть неверными и верными
    /// На вывод: только данный ответ
    /// </summary>
    public class DResultsAnswerOpen : AbstractDResultsAnswer
    {
        private readonly HashSet<string> _correctAnswers = new();
        private string _answerFromTesting = "";
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
        /// <summary>
        /// Возвращает пустой список, т.к. все ответы из списка являются корректными, а значит их палить нельзя
        /// </summary>
        public override IEnumerable<string> ArrayOfString01 => base.ArrayOfString01;
        public override IEnumerable<string> ArrayOfString02 => _answerFromTesting.Split();
        public override void SetCorrectAnswers(IEnumerable<AbstractAnswer> answers)
        {
            foreach(var answer in answers)
            {
                _correctAnswers.Add(answer.Answer1);
            }
        }
        public override void SetTestingAnswers(IEnumerable<AbstractAnswer> answers)
        {
            _answerFromTesting = answers.ToArray()[0].Answer2;
        }
    }
    /// <summary>
    /// Несколько вариантов и один верный
    /// В ответе может быть дан только один ответ
    /// На вывод: Варианты и данный ответ
    /// </summary>
    public class DResultsAnswerSelectiveOne : AbstractDResultsAnswer
    {
        private readonly List<string> _answers = new();
        private string _correctAnswer = "";
        private string _receivedAnswer = "";
        protected override double Сorrectness => _correctAnswer == _receivedAnswer ? 1 : 0;
        public override IEnumerable<string> ArrayOfString01 => _answers;
        public override IEnumerable<string> ArrayOfString02 { get { yield return _receivedAnswer; }
        }
        public override void SetCorrectAnswers(IEnumerable<AbstractAnswer> answers)
        {
            foreach (var answer in answers)
            {
                _answers.Add(answer.Answer1);
                if(answer.IsCorrect)
                {
                    _correctAnswer = answer.Answer1;
                }
            }
        }
        public override void SetTestingAnswers(IEnumerable<AbstractAnswer> answers)
        {
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
    public class DResultsAnswerSelectiveMultiple : AbstractDResultsAnswer
    {
        private readonly HashSet<string> _answers = new();
        private readonly HashSet<string> _correctAnswers = new();
        private readonly HashSet<string> _receivedAnswers = new();
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
        public override void SetCorrectAnswers(IEnumerable<AbstractAnswer> answers)
        {
            foreach (var answer in answers)
            {
                _answers.Add(answer.Answer1);
                if (answer.IsCorrect)
                {
                    _correctAnswers.Add(answer.Answer1);
                }
            }
        }
        public override void SetTestingAnswers(IEnumerable<AbstractAnswer> answers)
        {
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
    public class DResultsAnswerMatchingSearch : AbstractDResultsAnswer
    {
        private readonly Dictionary<string, string> _correctAnswers = new();
        private readonly Dictionary<string, string> _receivedAnswers = new();
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
        public override void SetCorrectAnswers(IEnumerable<AbstractAnswer> answers)
        {
            foreach (var answer in answers)
            {
                _correctAnswers.Add(answer.Answer1, answer.Answer2);
            }
        }

        public override void SetTestingAnswers(IEnumerable<AbstractAnswer> answers)
        {
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
    public class DResultsAnswerDataInput : AbstractDResultsAnswer
    {
        private string _info = "";
        public override IEnumerable<string> ArrayOfString01 { get { yield return _info; } }
        public override void SetTestingAnswers(IEnumerable<AbstractAnswer> answers)
        {
            _info = answers.ToArray()[0].Answer1;
        }
    }
}
