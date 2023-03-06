using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Приложение
{
    public class QuestionTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        public System.Windows.DataTemplate OpenAnswerTemplate { get; set; }
        public System.Windows.DataTemplate SelectiveAnswerTemplate { get; set; }
        public System.Windows.DataTemplate MatchingSearchTemplate { get; set; }
        public System.Windows.DataTemplate DataInputTemplate { get; set; }
        public QuestionTemplateSelector()
        {
            OpenAnswerTemplate = new System.Windows.DataTemplate();
            SelectiveAnswerTemplate = new System.Windows.DataTemplate();
            MatchingSearchTemplate = new System.Windows.DataTemplate();
            DataInputTemplate = new System.Windows.DataTemplate();
        }
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            DQuest question = item as DQuest;
            return question.Type switch
            {
                StringTypeQuestion.SELECTIVE_ANSWER => SelectiveAnswerTemplate,
                StringTypeQuestion.OPEN_ANSWER => OpenAnswerTemplate,
                StringTypeQuestion.MATCHING_SEARCH => MatchingSearchTemplate,
                StringTypeQuestion.DATA_INPUT => DataInputTemplate,
                _ => null
            };
        }
    }
}
