using System.ComponentModel;
using TransfromService;

namespace TableEditor.TransformParameters
{
    public class JsonTransformViewParameters : Html2JsonTransformParameters
    {
        /// <summary>
        /// To JSON
        /// </summary>
        [Category("Преобразование в JSON")]
        [DisplayName("Выходной формат текста")]
        [DefaultValue(ValueFormat.Html)]
        [PropertyOrder(0)]
        public override ValueFormat TargetFormat
        {
            get => base.TargetFormat;
            set => base.TargetFormat = value;
        }

        [Category("Преобразование в JSON")]
        [DisplayName("Обрабатывать цвет текста")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(1)]
        public override bool ProcessTextColor
        {
            get => base.ProcessTextColor;
            set => base.ProcessTextColor = value;
        }

        [Category("Преобразование в JSON")]
        [DisplayName("Заменять табы пробелами")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(2)]
        public override bool ReplaceTabsBySpaces
        {
            get => base.ReplaceTabsBySpaces;
            set => base.ReplaceTabsBySpaces = value;
        }

        [Category("Преобразование в JSON")]
        [DisplayName("Убирать форматирование из HTML")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(3)]
        public override bool RemoveFormatting
        {
            get => base.RemoveFormatting;
            set => base.RemoveFormatting = value;
        }

        [Category("Преобразование в JSON")]
        [DisplayName("Убирать Bold стиль для заголовков таблиц")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(4)]
        public override bool RemoveBoldStyleForHeaderCells
        {
            get => base.RemoveBoldStyleForHeaderCells;
            set => base.RemoveBoldStyleForHeaderCells = value;
        }

        [Category("Преобразование в JSON")]
        [DisplayName("Преобразовывать списки в плоские")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(5)]
        public override bool MakeAllListsFlatten
        {
            get => base.MakeAllListsFlatten;
            set => base.MakeAllListsFlatten = value;
        }

        [Category("Преобразование в JSON")]
        [DisplayName("Многоуровневая нумерация для плоского списка")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(6)]
        public override bool MultiLevelNumerationForFlattenList
        {
            get => base.MultiLevelNumerationForFlattenList;
            set => base.MultiLevelNumerationForFlattenList = value;
        }

        /// <summary>
        /// Признак необходимости копирования результрующего JSON в буфер обмена после трансформации 
        /// </summary>
        [Category("Преобразование в JSON")]
        [DisplayName("Копировать JSON в буфер обмена после преобразования")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(7)]
        public bool CopyJsonToClipboardAfterTransformation { get; set; } = true;

        [Category("Преобразование в JSON")]
        [DisplayName("Форматировать результирующий JSON")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(8)]
        public override bool NeedFormatJsonResult
        {
            get => base.NeedFormatJsonResult;
            set => base.NeedFormatJsonResult = value;
        }

        [Category("Преобразование в JSON")]
        [DisplayName("Двойная трансформация при преобразовании данных")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [Browsable(false)]
        [PropertyOrder(25)]
        public override bool NeedDoubleTransformation
        {
            get => base.NeedDoubleTransformation;
            set => base.NeedDoubleTransformation = value;
        }

        /// <summary>
        /// From JSON
        /// </summary>
        [Category("Преобразование из JSON")]
        [DisplayName("Преобразовывать списки в иерархические")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(9)]
        public bool MakeAllListsHierarchical { get; set; }

        public JsonTransformViewParameters()
        {
        }

        public JsonTransformViewParameters(Html2JsonTransformParameters source)
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