using System.Collections.ObjectModel;

namespace Приложение.Classes
{
    /// <summary>
    /// Описание перечисления форм вопросов
    /// </summary>
    static public class StringTypeQuestion
    {
        public const string OPEN_ANSWER = "Вопрос с открытым ответом";
        public const string SELECTIVE_ANSWER_ONE = "Вопрос с выборочным ответом (один)";
        public const string SELECTIVE_ANSWER_MULTIPLE = "Вопрос с выборочным ответом (несколько)";
        public const string MATCHING_SEARCH = "Вопрос с поиском соответствия";
        public const string DATA_INPUT = "Поле для ввода данных";

        static public ObservableCollection<string> List
        { 
            get 
            {
                return new ObservableCollection<string>
                {
                    OPEN_ANSWER,
                    SELECTIVE_ANSWER_ONE,
                    SELECTIVE_ANSWER_MULTIPLE,
                    MATCHING_SEARCH,
                    DATA_INPUT
                };
            } 
        }
    }
}