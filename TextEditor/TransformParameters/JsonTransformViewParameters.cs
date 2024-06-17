using System.ComponentModel;
using TextEditor.Editors.WorkbookEditor;
using TransformService;

namespace TextEditor.TransformParameters
{
    public class
        JsonTransformViewParameters : ISupportWorkbookEditorParameters, ICloneable,
        ICustomTypeDescriptor // : Html2JsonTransformParameters
    {
        protected const string ToEditorCategoryName = "Преобразование JSON -> Редактор";
        protected const string FromEditorCategoryName = "Преобразование JSON <- Редактор";

        private readonly Json2HtmlTransformParameters _json2HtmlParameters;
        private readonly Html2JsonTransformParameters _html2JsonParameters;

        private bool _isWorkbookEditorVisible;

        /// <summary>
        /// From JSON
        /// </summary>
        [Category(ToEditorCategoryName)]
        [DisplayName("Преобразовывать списки в иерархические")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoValueTypeConverter))]
        [PropertyOrder(0)]
        public bool MakeAllListsHierarchical
        {
            get => _json2HtmlParameters.MakeAllListsHierarchical;
            set => _json2HtmlParameters.MakeAllListsHierarchical = value;
        }

        [Category(ToEditorCategoryName)]
        [DisplayName("Параметры редактора ячеек")]
        [ReadOnly(true)]
        [PropertyOrder(1)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public WorkbookEditorParameters WorkbookEditorParameters { get; set; }

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
        [TypeConverter(typeof(YesNoValueTypeConverter))]
        [PropertyOrder(1)]
        public bool ProcessTextColor
        {
            get => _html2JsonParameters.ProcessTextColor;
            set => _html2JsonParameters.ProcessTextColor = value;
        }

        [Category(FromEditorCategoryName)]
        [DisplayName("Заменять табы пробелами")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoValueTypeConverter))]
        [PropertyOrder(2)]
        public bool ReplaceTabsBySpaces
        {
            get => _html2JsonParameters.ReplaceTabsBySpaces;
            set => _html2JsonParameters.ReplaceTabsBySpaces = value;
        }

        [Category(FromEditorCategoryName)]
        [DisplayName("Убирать форматирование из HTML")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoValueTypeConverter))]
        [PropertyOrder(3)]
        public bool RemoveFormatting
        {
            get => _html2JsonParameters.RemoveFormatting;
            set => _html2JsonParameters.RemoveFormatting = value;
        }

        [Category(FromEditorCategoryName)]
        [DisplayName("Убирать Bold стиль для заголовков таблиц")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoValueTypeConverter))]
        [PropertyOrder(4)]
        public bool RemoveBoldStyleForHeaderCells
        {
            get => _html2JsonParameters.RemoveBoldStyleForHeaderCells;
            set => _html2JsonParameters.RemoveBoldStyleForHeaderCells = value;
        }

        [Category(FromEditorCategoryName)]
        [DisplayName("Обрабатывать автонумерацию строк таблиц")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoValueTypeConverter))]
        [PropertyOrder(5)]
        public bool ProcessAutoNumberedRows
        {
            get => _html2JsonParameters.ProcessAutoNumberedRows;
            set => _html2JsonParameters.ProcessAutoNumberedRows = value;
        }


        [Category(FromEditorCategoryName)]
        [DisplayName("Обрабатывать заливку серым цветом ячеек таблиц")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoValueTypeConverter))]
        [PropertyOrder(6)]
        public bool ProcessGreyBackgroundColorForCells
        {
            get => _html2JsonParameters.ProcessGreyBackgroundColorForCells;
            set => _html2JsonParameters.ProcessGreyBackgroundColorForCells = value;
        }

        [Category(FromEditorCategoryName)]
        [DisplayName("Сохранять исходные размеры колонок таблиц")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoValueTypeConverter))]
        [PropertyOrder(7)]
        public bool KeepOriginalColumnWidths
        {
            get => _html2JsonParameters.KeepOriginalColumnWidths;
            set => _html2JsonParameters.KeepOriginalColumnWidths = value;
        }

        [Category(FromEditorCategoryName)]
        [DisplayName("Преобразовывать списки в плоские")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoValueTypeConverter))]
        [PropertyOrder(8)]
        public bool MakeAllListsFlatten
        {
            get => _html2JsonParameters.MakeAllListsFlatten;
            set => _html2JsonParameters.MakeAllListsFlatten = value;
        }

        [Category(FromEditorCategoryName)]
        [DisplayName("Многоуровневая нумерация для плоского списка")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoValueTypeConverter))]
        [PropertyOrder(9)]
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
        [TypeConverter(typeof(YesNoValueTypeConverter))]
        [PropertyOrder(10)]
        public bool CopyJsonToClipboardAfterTransformation { get; set; } = true;

        [Category(FromEditorCategoryName)]
        [DisplayName("Форматировать результирующий JSON")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoValueTypeConverter))]
        [PropertyOrder(11)]
        public bool NeedFormatJsonResult
        {
            get => _html2JsonParameters.NeedFormatJsonResult;
            set => _html2JsonParameters.NeedFormatJsonResult = value;
        }

        [Category(FromEditorCategoryName)]
        [DisplayName("Двойная трансформация при преобразовании данных (HTML -> JSON, JSON -> HTML, HTML -> JSON)")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoValueTypeConverter))]
        [Browsable(false)]
        [PropertyOrder(25)]
        public bool NeedDoubleTransformation
        {
            get => _html2JsonParameters.NeedDoubleTransformation;
            set => _html2JsonParameters.NeedDoubleTransformation = value;
        }

        #region Constructors

        public JsonTransformViewParameters() : this(new Json2HtmlTransformParameters(),
            new Html2JsonTransformParameters())
        {
        }

        public JsonTransformViewParameters(Json2HtmlTransformParameters json2HtmlParameters,
            Html2JsonTransformParameters html2JsonParameters)
        {
            _json2HtmlParameters = json2HtmlParameters;
            _html2JsonParameters = html2JsonParameters;
            WorkbookEditorParameters = new WorkbookEditorParameters();
        }

        #endregion

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

        #region ICloneable

        public object Clone() =>
            new JsonTransformViewParameters(_json2HtmlParameters.Clone() as Json2HtmlTransformParameters,
                _html2JsonParameters.Clone() as Html2JsonTransformParameters)
            {
                WorkbookEditorParameters = WorkbookEditorParameters.Clone() as WorkbookEditorParameters
            };

        #endregion

        #region ISupportWorkbookEditorParameters

        public void SetWorkbookEditorParametersVisibility(bool value) => _isWorkbookEditorVisible = value;
        public WorkbookEditorParameters GetWorkbookEditorParameters() => WorkbookEditorParameters;

        #endregion

        #region ICustomTypeDescriptor

        // Реализация ICustomTypeDescriptor
        public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes(this, true);
        public string GetClassName() => TypeDescriptor.GetClassName(this, true);
        public string GetComponentName() => TypeDescriptor.GetComponentName(this, true);
        public TypeConverter GetConverter() => TypeDescriptor.GetConverter(this, true);
        public EventDescriptor GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(this, true);
        public PropertyDescriptor GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(this, true);
        public object GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(this, editorBaseType, true);

        public EventDescriptorCollection GetEvents(Attribute[] attributes) =>
            TypeDescriptor.GetEvents(this, attributes, true);

        public EventDescriptorCollection GetEvents() => TypeDescriptor.GetEvents(this, true);
        public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes) => GetProperties();

        public virtual PropertyDescriptorCollection GetProperties()
        {
            var props = TypeDescriptor.GetProperties(this, true)
                .Cast<PropertyDescriptor>()
                .Select(p =>
                    p.Name == "WorkbookEditorParameters"
                        ? TypeDescriptor.CreateProperty(
                            typeof(JsonTransformViewParameters), p,
                            new BrowsableAttribute(_isWorkbookEditorVisible))
                        : p
                );

            return new PropertyDescriptorCollection(props.ToArray());
        }

        public object GetPropertyOwner(PropertyDescriptor pd) => this;

        #endregion
    }
}