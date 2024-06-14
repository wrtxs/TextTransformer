using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;
using TextEditor.TransformParameters;

namespace TextEditor.Editors.WorkbookEditor
{
    /// <summary>
    /// Параметры представления, специфичные для табличного редактора
    /// </summary>
    [JsonObject]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WorkbookEditorParameters : ICloneable, IEditorParameters
    {
        /// <summary>
        /// Признак необходимости быстрой вставки HTML содержимого в редактор, без возможности Undo
        /// </summary>
        [DisplayName("Быстрая вставка HTML без сохранения истории")]
        //[Description("Быстрая вставка значения без сохранения истории")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoValueTypeConverter))]
        [Browsable(true)]
        public bool FastInsertHtmlData { get; set; } = true;

        /// <summary>
        /// Признак необходимости автоподбора ширины ячеек по их содержимому
        /// </summary>
        [DisplayName("Автоподбор ширины ячеек по их содержимому")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoValueTypeConverter))]
        [Browsable(true)]
        public bool AutoFitCellsWidth { get; set; } = false;

        #region ICloneable

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion

        public override string ToString() => string.Empty;
    }
}