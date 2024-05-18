using System.ComponentModel;
using TextEditor.Editors.WorkbookEditor;
using TransformService;

namespace TextEditor.TransformParameters
{
    public class HtmlTransformViewParameters : ISupportWorkbookEditorParameters, ICloneable,
        ICustomTypeDescriptor //Json2HtmlTransformParameters
    {
        private const string ToEditorCategoryName = "Преобразование HTML -> Редактор";
        //private const string FromEditorCategoryName = "Преобразование HTML <- Редактор";

        private readonly Html2HtmlTransformParameters _html2HtmlParameters;
        private bool _isWorkbookEditorVisible;

        [Category(ToEditorCategoryName)]
        [DisplayName("Переносить HTML в редактор через JSON")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoValueTypeConverter))]
        [PropertyOrder(0)]
        public bool TransformViaJson { get; set; } = true;

        [Category(ToEditorCategoryName)]
        [DisplayName("Преобразовывать списки в иерархические")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoValueTypeConverter))]
        [PropertyOrder(1)]
        public bool MakeAllListsHierarchical
        {
            get => _html2HtmlParameters.MakeAllListsHierarchical;
            set => _html2HtmlParameters.MakeAllListsHierarchical = value;
        }

        [Category(ToEditorCategoryName)]
        [DisplayName("Параметры редактора ячеек")]
        [ReadOnly(true)]
        [PropertyOrder(2)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public WorkbookEditorParameters WorkbookEditorParameters { get; set; }

        public Html2HtmlTransformParameters GetHtml2HtmlTransformParameters() =>
            _html2HtmlParameters;

        #region Constructors

        public HtmlTransformViewParameters() : this(new Html2HtmlTransformParameters())
        {
        }

        public HtmlTransformViewParameters(Html2HtmlTransformParameters html2HtmlParameters)
        {
            _html2HtmlParameters = html2HtmlParameters;
            WorkbookEditorParameters = new WorkbookEditorParameters();
        }

        #endregion

        #region ICloneable

        public object Clone() =>
            new HtmlTransformViewParameters(_html2HtmlParameters.Clone() as Html2HtmlTransformParameters)
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
                            typeof(HtmlTransformViewParameters), p,
                            new BrowsableAttribute(_isWorkbookEditorVisible))
                        : p
                );

            return new PropertyDescriptorCollection(props.ToArray());
        }

        public object GetPropertyOwner(PropertyDescriptor pd) => this;

        #endregion
    }
}