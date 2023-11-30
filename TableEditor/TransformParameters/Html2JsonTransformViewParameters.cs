using System.ComponentModel;
using TransfromService;

namespace TableEditor.TransformParameters
{
    internal class Html2JsonTransformViewParameters : Html2JsonTransformParameters
    {
        [Category("")]
        [DisplayName("Выходной формат значений")]
        [DefaultValue(ValueFormat.Html)]
        [PropertyOrder(0)]
        public override ValueFormat TargetFormat
        {
            get => base.TargetFormat;
            set => base.TargetFormat = value;
        }

        [Category("")]
        [DisplayName("Обрабатывать цвет текста")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(1)]
        public override bool ProcessTextColor
        {
            get => base.ProcessTextColor;
            set => base.ProcessTextColor = value;
        }

        [Category("")]
        [DisplayName("Заменять табы пробелами")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(2)]
        public override bool ReplaceTabsBySpaces
        {
            get => base.ReplaceTabsBySpaces;
            set => base.ReplaceTabsBySpaces = value;
        }

        [Category("")]
        [DisplayName("Убирать форматирование из HTML")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(3)]
        public override bool RemoveFormatting
        {
            get => base.RemoveFormatting;
            set => base.RemoveFormatting = value;
        }

        [Category("")]
        [DisplayName("Убирать Bold стиль для заголовков таблиц")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(4)]
        public override bool RemoveBoldStyleForHeaderCells
        {
            get => base.RemoveBoldStyleForHeaderCells;
            set => base.RemoveBoldStyleForHeaderCells = value;
        }

        [Category("")]
        [DisplayName("Переводить все списки в плоский вид")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(5)]
        public override bool MakeAllListsFlatten
        {
            get => base.MakeAllListsFlatten;
            set => base.MakeAllListsFlatten = value;
        }

        [Category("")]
        [DisplayName("Форматировать результирующий JSON")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(6)]
        public override bool NeedFormatJSONResult
        {
            get => base.NeedFormatJSONResult;
            set => base.NeedFormatJSONResult = value;
        }

        [Category("")]
        [DisplayName("Двойная трансформация при преобразовании данных")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [Browsable(false)]
        [PropertyOrder(7)]
        public override bool NeedDoubleTransformation
        {
            get => base.NeedDoubleTransformation;
            set => base.NeedDoubleTransformation = value;
        }

        public Html2JsonTransformViewParameters(Html2JsonTransformParameters source)
        {
            CommonUtils.CopyValues(source, this);
        }

        public void SetValues(Html2JsonTransformParameters source)
        {
            CommonUtils.CopyValues(source, this);
        }
    }
    internal class PropertyOrderAttribute : Attribute
    {
        public int Value { get; set; }
        public PropertyOrderAttribute(int value) => Value = value;
    }
}