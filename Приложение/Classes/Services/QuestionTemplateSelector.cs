using Приложение.Classes.Enums;
using Приложение.Classes.Models;

namespace Приложение.Classes.Services
{
    /// <summary>
    /// Селектор для шаблонов вопросов в окне редактора
    /// </summary>
    public class EditQuestionTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        public System.Windows.DataTemplate EditOpenAnswerTemplate { get; set; }
        public System.Windows.DataTemplate EditSelectiveAnswerOneTemplate { get; set; }
        public System.Windows.DataTemplate EditSelectiveAnswerMultipleTemplate { get; set; }
        public System.Windows.DataTemplate EditMatchingSearchTemplate { get; set; }
        public System.Windows.DataTemplate EditDataInputTemplate { get; set; }
        public EditQuestionTemplateSelector()
        {
            EditOpenAnswerTemplate = new System.Windows.DataTemplate();
            EditSelectiveAnswerOneTemplate = new System.Windows.DataTemplate();
            EditSelectiveAnswerMultipleTemplate = new System.Windows.DataTemplate();
            EditMatchingSearchTemplate = new System.Windows.DataTemplate();
            EditDataInputTemplate = new System.Windows.DataTemplate();
        }

        /// <summary>
        /// Метод выбора формы вопроса
        /// </summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            DQuest question = item as DQuest;
            return question.Type switch
            {
                EnumTypeQuestion.OPEN_ANSWER => EditOpenAnswerTemplate,
                EnumTypeQuestion.SELECTIVE_ANSWER_ONE => EditSelectiveAnswerOneTemplate,
                EnumTypeQuestion.SELECTIVE_ANSWER_MULTIPLE => EditSelectiveAnswerMultipleTemplate,
                EnumTypeQuestion.MATCHING_SEARCH => EditMatchingSearchTemplate,
                EnumTypeQuestion.DATA_INPUT => EditDataInputTemplate,
                _ => null
            };
        }
    }

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

        /// <summary>
        /// Метод выбора формы вопроса
        /// </summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            DQuest question = item as DQuest;
            return question.Type switch
            {
                EnumTypeQuestion.OPEN_ANSWER => TestingOpenAnswerTemplate,
                EnumTypeQuestion.SELECTIVE_ANSWER_ONE => TestingSelectiveAnswerOneTemplate,
                EnumTypeQuestion.SELECTIVE_ANSWER_MULTIPLE => TestingSelectiveAnswerMultipleTemplate,
                EnumTypeQuestion.MATCHING_SEARCH => TestingMatchingSearchTemplate,
                EnumTypeQuestion.DATA_INPUT => TestingDataInputTemplate,
                _ => null
            };
        }
    }
}
