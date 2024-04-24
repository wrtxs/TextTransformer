using System.ComponentModel;
using TransformService;

namespace TextEditor.TransformParameters
{
    public class HtmlTransformViewParameters : ICloneable //Json2HtmlTransformParameters
    {
        private const string ToEditorCategoryName = "Преобразование HTML -> Редактор";
        //private const string FromEditorCategoryName = "Преобразование HTML <- Редактор";

        private readonly Html2HtmlTransformParameters _html2HtmlParameters;

        [Category(ToEditorCategoryName)]
        [DisplayName("Переносить HTML в редактор через JSON")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(0)]
        public bool TransformViaJson { get; set; } = true;

        [Category(ToEditorCategoryName)]
        [DisplayName("Преобразовывать списки в иерархические")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(1)]
        public bool MakeAllListsHierarchical
        {
            get => _html2HtmlParameters.MakeAllListsHierarchical;
            set => _html2HtmlParameters.MakeAllListsHierarchical = value;
        }

        public Html2HtmlTransformParameters GetHtml2HtmlTransformParameters() =>
            _html2HtmlParameters;

        public HtmlTransformViewParameters() : this(new Html2HtmlTransformParameters())
        {
        }

        public HtmlTransformViewParameters(Html2HtmlTransformParameters html2HtmlParameters)
        {
            _html2HtmlParameters = html2HtmlParameters;
        }

        public object Clone()
        {
            return new HtmlTransformViewParameters(_html2HtmlParameters.Clone() as Html2HtmlTransformParameters);
        }
    }
}