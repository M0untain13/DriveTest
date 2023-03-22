using System.Collections.Generic;
using System.Linq;

namespace Приложение.Classes
{
    //TODO: нужно организовать проведения тестирования, для того чтобы здесь определиться с заполнением результатов
    public class DResults
    {
        private readonly IEnumerable<AbstractDResultsAnswer> _answers = Enumerable.Empty<AbstractDResultsAnswer>();
        public IEnumerable<AbstractDResultsAnswer> Answers => _answers;
    }
    /// <summary>
    /// На вывод: Имя, баллы * процент_корректности, варианты ответов при необходимости, данный ответ
    /// </summary>
    public abstract class AbstractDResultsAnswer
    {
        private readonly string _name;
        private readonly double _score;
        /// <summary>
        /// Название вопроса
        /// </summary>
        public string Name => _name;
        /// <summary>
        /// Кол-во баллов за ответ
        /// </summary>
        public double Score { get { return _score * Сorrectness; } }
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
    }
    /// <summary>
    /// Несколько вариантов и все верные
    /// В ответе может быть дано несколько ответов, могут быть неверными и верными
    /// На вывод: только данный ответ
    /// </summary>
    public class DResultsAnswerOpen : AbstractDResultsAnswer
    {
        private readonly HashSet<string> _correctAnswers = new();
        private readonly string _answerFromTesting = "";
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
                return countOfCorrectAnswers/ _correctAnswers.Count * (1 - countOfErrors / _correctAnswers.Count); 
            } 
        }
        /// <summary>
        /// Возвращает пустой список, т.к. все ответы из списка являются корректными, а значит их палить нельзя
        /// </summary>
        public override IEnumerable<string> ArrayOfString01 => base.ArrayOfString01;
        public override IEnumerable<string> ArrayOfString02 => _answerFromTesting.Split();
    }
    /// <summary>
    /// Несколько вариантов и один верный
    /// В ответе может быть дан только один ответ
    /// На вывод: Варианты и данный ответ
    /// </summary>
    public class DResultsAnswerSelectiveOne : AbstractDResultsAnswer
    {
        private readonly List<string> _answers = new();
        private readonly string _receivedAnswer = "";
        private readonly bool _correct;
        protected override double Сorrectness
        {
            get
            {
                if (_correct) return 1;
                return 0;
            }
        }
        public override IEnumerable<string> ArrayOfString01 => _answers;
        public override IEnumerable<string> ArrayOfString02 { get { yield return _receivedAnswer; }
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
                return countOfCorrectAnswers / _correctAnswers.Count * (1 - countOfErrors / _answers.Count);
            }
        }
        public override IEnumerable<string> ArrayOfString01 => _answers;
        public override IEnumerable<string> ArrayOfString02 => _receivedAnswers;
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
                return countOfCorrectAnswers / _correctAnswers.Count * (1 - countOfErrors / _correctAnswers.Count);
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
    }
    /// <summary>
    /// Просто строка
    /// На вывод: Данная строка
    /// </summary>
    public class DResultsAnswerDataInput : AbstractDResultsAnswer
    {
        private readonly string _info = "";
        protected override double Сorrectness => base.Сorrectness;
        public override IEnumerable<string> ArrayOfString01 { get { yield return _info; } }
        public override IEnumerable<string> ArrayOfString02 => base.ArrayOfString02;
    }
}
