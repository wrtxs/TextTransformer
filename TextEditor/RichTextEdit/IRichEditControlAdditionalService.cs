namespace TextEditor.RichTextEdit
{
    internal interface IRichEditControlAdditionalService
    {
        public string GetTableTitle();

        public ClipboardFormat GetClipboardFormat();
    }

    public enum ClipboardFormat
    {
        Html,
        All
    }
}
