using System.ComponentModel;
using TransformService;

namespace TextEditor.TransformParameters
{
    public class HtmlTransformViewParameters : Json2HtmlTransformParameters
    {
        [Category("Преобразование в HTML")]
        [DisplayName("Переносить HTML в редактор через JSON")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(0)]
        public bool TransformViaJson { get; set; } = true;
        
        [Category("Преобразование в HTML")]
        [DisplayName("Преобразовывать списки в иерархические")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(1)]
        public override bool MakeAllListsHierarchical
        {
            get => base.MakeAllListsHierarchical;
            set => base.MakeAllListsHierarchical = value;
        }
    }
}