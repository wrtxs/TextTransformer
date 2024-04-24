using System.ComponentModel;
using TransformService;

namespace TextEditor.TransformParameters
{
    public class JsonTransformViewParameters : ICloneable // : Html2JsonTransformParameters
    {
        private const string ToEditorCategoryName = "Преобразование JSON -> Редактор";
        private const string FromEditorCategoryName = "Преобразование JSON <- Редактор";

        private readonly Json2HtmlTransformParameters _json2HtmlParameters;
        private readonly Html2JsonTransformParameters _html2JsonParameters;

        /// <summary>
        /// From JSON
        /// </summary>
        [Category(ToEditorCategoryName)]
        [DisplayName("Преобразовывать списки в иерархические")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(0)]
        public bool MakeAllListsHierarchical
        {
            get => _json2HtmlParameters.MakeAllListsHierarchical;
            set => _json2HtmlParameters.MakeAllListsHierarchical = value;
        }

        /// <summary>
        /// To JSON
        /// </summary>
        [Category(FromEditorCategoryName)]
        [DisplayName("Выходной формат текста")]
        [DefaultValue(Html2JsonTransformParameters.ValueFormat.Html)]
        [PropertyOrder(0)]
        public Html2JsonTransformParameters.ValueFormat TargetFormat
        {
            get => _html2JsonParameters.TargetFormat;
            set => _html2JsonParameters.TargetFormat = value;
        }

        [Category(FromEditorCategoryName)]
        [DisplayName("Обрабатывать цвет текста")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(1)]
        public bool ProcessTextColor
        {
            get => _html2JsonParameters.ProcessTextColor;
            set => _html2JsonParameters.ProcessTextColor = value;
        }

        [Category(FromEditorCategoryName)]
        [DisplayName("Заменять табы пробелами")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(2)]
        public bool ReplaceTabsBySpaces
        {
            get => _html2JsonParameters.ReplaceTabsBySpaces;
            set => _html2JsonParameters.ReplaceTabsBySpaces = value;
        }

        [Category(FromEditorCategoryName)]
        [DisplayName("Убирать форматирование из HTML")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(3)]
        public bool RemoveFormatting
        {
            get => _html2JsonParameters.RemoveFormatting;
            set => _html2JsonParameters.RemoveFormatting = value;
        }

        [Category(FromEditorCategoryName)]
        [DisplayName("Убирать Bold стиль для заголовков таблиц")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(4)]
        public bool RemoveBoldStyleForHeaderCells
        {
            get => _html2JsonParameters.RemoveBoldStyleForHeaderCells;
            set => _html2JsonParameters.RemoveBoldStyleForHeaderCells = value;
        }

        [Category(FromEditorCategoryName)]
        [DisplayName("Обрабатывать автонумерацию строк таблиц")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(5)]
        public bool ProcessAutoNumberedRows
        {
            get => _html2JsonParameters.ProcessAutoNumberedRows;
            set => _html2JsonParameters.ProcessAutoNumberedRows = value;
        }


        [Category(FromEditorCategoryName)]
        [DisplayName("Обрабатывать заливку серым цветом ячеек таблиц")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(6)]
        public bool ProcessGreyBackgroundColorForCells
        {
            get => _html2JsonParameters.ProcessGreyBackgroundColorForCells;
            set => _html2JsonParameters.ProcessGreyBackgroundColorForCells = value;
        }

        [Category(FromEditorCategoryName)]
        [DisplayName("Преобразовывать списки в плоские")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(7)]
        public bool MakeAllListsFlatten
        {
            get => _html2JsonParameters.MakeAllListsFlatten;
            set => _html2JsonParameters.MakeAllListsFlatten = value;
        }

        [Category(FromEditorCategoryName)]
        [DisplayName("Многоуровневая нумерация для плоского списка")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(8)]
        public bool MultiLevelNumerationForFlattenList
        {
            get => _html2JsonParameters.MultiLevelNumerationForFlattenList;
            set => _html2JsonParameters.MultiLevelNumerationForFlattenList = value;
        }

        /// <summary>
        /// Признак необходимости копирования результрующего JSON в буфер обмена после трансформации 
        /// </summary>
        [Category(FromEditorCategoryName)]
        [DisplayName("Копировать JSON в буфер обмена после преобразования")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(9)]
        public bool CopyJsonToClipboardAfterTransformation { get; set; } = true;

        [Category(FromEditorCategoryName)]
        [DisplayName("Форматировать результирующий JSON")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [PropertyOrder(0)]
        public bool NeedFormatJsonResult
        {
            get => _html2JsonParameters.NeedFormatJsonResult;
            set => _html2JsonParameters.NeedFormatJsonResult = value;
        }

        [Category(FromEditorCategoryName)]
        [DisplayName("Двойная трансформация при преобразовании данных (HTML -> JSON, JSON -> HTML, HTML -> JSON)")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [Browsable(false)]
        [PropertyOrder(25)]
        public bool NeedDoubleTransformation
        {
            get => _html2JsonParameters.NeedDoubleTransformation;
            set => _html2JsonParameters.NeedDoubleTransformation = value;
        }

        public JsonTransformViewParameters() : this(new Json2HtmlTransformParameters(),
            new Html2JsonTransformParameters())
        {
        }

        public JsonTransformViewParameters(Json2HtmlTransformParameters json2HtmlParameters,
            Html2JsonTransformParameters html2JsonParameters)
        {
            _json2HtmlParameters = json2HtmlParameters;
            _html2JsonParameters = html2JsonParameters;
        }

        public Json2HtmlTransformParameters GetJson2HtmlTransformParameters() => _json2HtmlParameters;
        public Html2JsonTransformParameters GetHtml2JsonTransformParameters() => _html2JsonParameters;

        //public JsonTransformViewParameters(Html2JsonTransformParameters source)
        //{
        //    CommonUtils.CopyValues(source, this);
        //}

        //public void SetValues(Html2JsonTransformParameters source)
        //{
        //    CommonUtils.CopyValues(source, this);
        //}
        public object Clone()
        {
            return new JsonTransformViewParameters(_json2HtmlParameters.Clone() as Json2HtmlTransformParameters,
                _html2JsonParameters.Clone() as Html2JsonTransformParameters);
        }
    }

    internal class PropertyOrderAttribute : Attribute
    {
        public int Value { get; set; }
        public PropertyOrderAttribute(int value) => Value = value;
    }
}