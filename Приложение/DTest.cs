using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Приложение
{
    public class DTest //TODO: надо будет заменить этот класс
    {
        public string name = "";
        public ObservableCollection<DQuest> quests = new();
    }
    public class DQuest
    {
        public string quest = "";
        public ObservableCollection<DAnswer> answers = new();
        public int number = -1;
        public int Number { get { return number; } set { number = value; } }
        public ObservableCollection<DAnswer> Answers { get { return answers; } }
    }
    public class DAnswer
    {
        public string answer = "";
        public string Answer { get { return answer; } set { answer = value; } }
    }
    
}
