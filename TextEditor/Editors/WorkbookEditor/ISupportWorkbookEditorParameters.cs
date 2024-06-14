namespace TextEditor.Editors.WorkbookEditor
{
    public interface ISupportWorkbookEditorParameters
    {
        public void SetWorkbookEditorParametersVisibility(bool value);
        public WorkbookEditorParameters GetWorkbookEditorParameters();
    }
}