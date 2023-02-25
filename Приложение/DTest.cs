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
        static public DTest GetTest()
        {
            DTest test = new();
            test.name = "Name";
            for(int i = 0; i < 3; i++)
            {
                DQuest quest = new();
                quest.quest = i.ToString();
                quest.number = i + 1;
                for (int j = 0; j < 3; j++)
                {
                    DAnswer answer = new();
                    answer.answer = i.ToString();
                    quest.answers.Add(answer);
                }
                test.quests.Add(quest);
            }
            return test;
        }
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
