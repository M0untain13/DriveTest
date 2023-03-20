using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Приложение.Classes
{
    //TODO: нужно заполнить классы
    public class DResults
    {
        private ObservableCollection<AbstractDResultsAnswer> _answers = new();
        public ObservableCollection<AbstractDResultsAnswer> Answers { get { return _answers; } }
    }
    public abstract class AbstractDResultsAnswer
    {
        public string name;
        public double score;
    }
    public class DResultsAnswerOpen : AbstractDResultsAnswer
    {
        public List<string> correctAnswer = new();
        public string answerFromTesting = "";
    }
    public class DResultsAnswerSelectiveOne : AbstractDResultsAnswer
    {
        public List<string> answers = new();
        public string studentAnswer;
        bool correct;
    }
    public class DResultsAnswerSelectiveMultiple : AbstractDResultsAnswer
    {
        public List<string> answers = new();
        public List<string> correctAnswer = new();
        bool correct;
    }
    public class DResultsAnswerMatchingSearch : AbstractDResultsAnswer
    {
        Dictionary<string, string> correctAnswers = new Dictionary<string, string>();
        Dictionary<string, string> studentAnswers = new Dictionary<string, string>();
    }
    public class DResultsAnswerDataInput : AbstractDResultsAnswer
    {
        public string info;
    }
}
