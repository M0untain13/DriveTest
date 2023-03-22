using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Приложение.Classes
{
    /// <summary>
    /// Описание перечисления форм вопросов
    /// </summary>
    public static class EnumTypeQuestion
    {
        public const string OPEN_ANSWER = "Вопрос с открытым ответом";
        public const string SELECTIVE_ANSWER_ONE = "Вопрос с выборочным ответом (один)";
        public const string SELECTIVE_ANSWER_MULTIPLE = "Вопрос с выборочным ответом (несколько)";
        public const string MATCHING_SEARCH = "Вопрос с поиском соответствия";
        public const string DATA_INPUT = "Поле для ввода данных";

        public static ObservableCollection<string> strings = new()
        {
            OPEN_ANSWER,
            SELECTIVE_ANSWER_ONE,
            SELECTIVE_ANSWER_MULTIPLE,
            MATCHING_SEARCH,
            DATA_INPUT
        };
    }
}