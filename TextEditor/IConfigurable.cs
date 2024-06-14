namespace TextEditor
{
    public interface IConfigurable
    {
        public void LoadParameters(bool enableWorkbookEditor);
        public void SaveParameters();
    }
}