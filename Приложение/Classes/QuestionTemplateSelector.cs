namespace Приложение.Classes
{
    /// <summary>
    /// Описание структуры селектора формы вопроса
    /// </summary>
    public class QuestionTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        public System.Windows.DataTemplate OpenAnswerTemplate { get; set; }
        public System.Windows.DataTemplate SelectiveAnswerOneTemplate { get; set; }
        public System.Windows.DataTemplate SelectiveAnswerMultipleTemplate { get; set; }
        public System.Windows.DataTemplate MatchingSearchTemplate { get; set; }
        public System.Windows.DataTemplate DataInputTemplate { get; set; }
        public QuestionTemplateSelector()
        {
            OpenAnswerTemplate = new System.Windows.DataTemplate();
            SelectiveAnswerOneTemplate = new System.Windows.DataTemplate();
            SelectiveAnswerMultipleTemplate = new System.Windows.DataTemplate();
            MatchingSearchTemplate = new System.Windows.DataTemplate();
            DataInputTemplate = new System.Windows.DataTemplate();
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
                EnumTypeQuestion.OPEN_ANSWER => OpenAnswerTemplate,
                EnumTypeQuestion.SELECTIVE_ANSWER_ONE => SelectiveAnswerOneTemplate,
                EnumTypeQuestion.SELECTIVE_ANSWER_MULTIPLE => SelectiveAnswerMultipleTemplate,
                EnumTypeQuestion.MATCHING_SEARCH => MatchingSearchTemplate,
                EnumTypeQuestion.DATA_INPUT => DataInputTemplate,
                _ => null
            };
        }
    }
}
