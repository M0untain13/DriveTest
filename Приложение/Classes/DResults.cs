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

    }
    public class DResultsAnswerOpen : AbstractDResultsAnswer
    {
        public List<string> correctAnswer = new();
        public string answerFromTesting = "";
    }
    public class DResultsAnswerSelectiveOne : AbstractDResultsAnswer
    {

    }
    public class DResultsAnswerSelectiveMultiple : AbstractDResultsAnswer
    {

    }
    public class DResultsAnswerMatchingSearch : AbstractDResultsAnswer
    {

    }
    public class DResultsAnswerDataInput : AbstractDResultsAnswer
    {

    }
}
