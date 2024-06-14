namespace TextEditor.Editors.RichTextEditor
{
    public class RichTextEditorParameters : ICloneable, IEditorParameters
    {
        public bool ApplyTableStyle { get; set; }
        public bool AutoFitTable { get; set; }

        #region Constructors

        public RichTextEditorParameters()
        {
        }

        public RichTextEditorParameters(bool applyTableStyle, bool autoFitTable)
        {
            ApplyTableStyle = applyTableStyle;
            AutoFitTable = autoFitTable;
        }

        #endregion

        #region ICloneable

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }
}