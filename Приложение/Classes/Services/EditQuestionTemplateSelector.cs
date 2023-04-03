using System;
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

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if(item is not DQuest question) throw new Exception();
            return question.Type switch
            {
                EnumTypeQuestion.OPEN_ANSWER => EditOpenAnswerTemplate,
                EnumTypeQuestion.SELECTIVE_ANSWER_ONE => EditSelectiveAnswerOneTemplate,
                EnumTypeQuestion.SELECTIVE_ANSWER_MULTIPLE => EditSelectiveAnswerMultipleTemplate,
                EnumTypeQuestion.MATCHING_SEARCH => EditMatchingSearchTemplate,
                EnumTypeQuestion.DATA_INPUT => EditDataInputTemplate,
                _ => throw new Exception()
            };
        }
    }
}
