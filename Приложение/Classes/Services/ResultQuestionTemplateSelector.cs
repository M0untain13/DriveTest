using System;
using System.CodeDom;
using Приложение.Classes.Enums;
using Приложение.Classes.Models;

namespace Приложение.Classes.Services
{
    /// <summary>
    /// Селектор для шаблонов результата в окне просмотра результатов
    /// </summary>
    public class ResultQuestionTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        public System.Windows.DataTemplate ResultOpenAnswerTemplate { get; set; }
        public System.Windows.DataTemplate ResultSelectiveAnswerOneTemplate { get; set; }
        public System.Windows.DataTemplate ResultSelectiveAnswerMultipleTemplate { get; set; }
        public System.Windows.DataTemplate ResultMatchingSearchTemplate { get; set; }
        public System.Windows.DataTemplate ResultDataInputTemplate { get; set; }
        public ResultQuestionTemplateSelector()
        {
            ResultOpenAnswerTemplate = new System.Windows.DataTemplate();
            ResultSelectiveAnswerOneTemplate = new System.Windows.DataTemplate();
            ResultSelectiveAnswerMultipleTemplate = new System.Windows.DataTemplate();
            ResultMatchingSearchTemplate = new System.Windows.DataTemplate();
            ResultDataInputTemplate = new System.Windows.DataTemplate();
        }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (item is not AbstractDResultsAnswer result) throw new Exception();
            return result.Type switch
            {
                EnumTypeQuestion.OPEN_ANSWER => ResultOpenAnswerTemplate,
                EnumTypeQuestion.SELECTIVE_ANSWER_ONE => ResultSelectiveAnswerOneTemplate,
                EnumTypeQuestion.SELECTIVE_ANSWER_MULTIPLE => ResultSelectiveAnswerMultipleTemplate,
                EnumTypeQuestion.MATCHING_SEARCH => ResultMatchingSearchTemplate,
                EnumTypeQuestion.DATA_INPUT => ResultDataInputTemplate,
                _ => throw new Exception()
            };
        }
    }
}