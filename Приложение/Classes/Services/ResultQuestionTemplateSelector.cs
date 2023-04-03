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
            if (result.GetType() == typeof(DResultsAnswerOpen)) return ResultOpenAnswerTemplate;
            if (result.GetType() == typeof(DResultsAnswerSelectiveOne)) return ResultSelectiveAnswerOneTemplate;
            if (result.GetType() == typeof(DResultsAnswerSelectiveMultiple)) return ResultSelectiveAnswerMultipleTemplate;
            if (result.GetType() == typeof(DResultsAnswerMatchingSearch)) return ResultMatchingSearchTemplate;
            if (result.GetType() == typeof(DResultsAnswerDataInput)) return ResultDataInputTemplate;
            throw new Exception();
        }
    }
}