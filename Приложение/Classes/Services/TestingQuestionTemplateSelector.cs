using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Приложение.Classes.Enums;
using Приложение.Classes.Models;

namespace Приложение.Classes.Services
{
    /// <summary>
    /// Селектор для шаблонов вопросов в окне тестирования
    /// </summary>
    public class TestingQuestionTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        public System.Windows.DataTemplate TestingOpenAnswerTemplate { get; set; }
        public System.Windows.DataTemplate TestingSelectiveAnswerOneTemplate { get; set; }
        public System.Windows.DataTemplate TestingSelectiveAnswerMultipleTemplate { get; set; }
        public System.Windows.DataTemplate TestingMatchingSearchTemplate { get; set; }
        public System.Windows.DataTemplate TestingDataInputTemplate { get; set; }
        public TestingQuestionTemplateSelector()
        {
            TestingOpenAnswerTemplate = new System.Windows.DataTemplate();
            TestingSelectiveAnswerOneTemplate = new System.Windows.DataTemplate();
            TestingSelectiveAnswerMultipleTemplate = new System.Windows.DataTemplate();
            TestingMatchingSearchTemplate = new System.Windows.DataTemplate();
            TestingDataInputTemplate = new System.Windows.DataTemplate();
        }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (item is not DQuest question) throw new Exception();
            return question.Type switch
            {
                EnumTypeQuestion.OPEN_ANSWER => TestingOpenAnswerTemplate,
                EnumTypeQuestion.SELECTIVE_ANSWER_ONE => TestingSelectiveAnswerOneTemplate,
                EnumTypeQuestion.SELECTIVE_ANSWER_MULTIPLE => TestingSelectiveAnswerMultipleTemplate,
                EnumTypeQuestion.MATCHING_SEARCH => TestingMatchingSearchTemplate,
                EnumTypeQuestion.DATA_INPUT => TestingDataInputTemplate,
                _ => throw new Exception()
            };
        }
    }
}
