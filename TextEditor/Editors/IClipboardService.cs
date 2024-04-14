namespace TextEditor.Editors
{
    interface IClipboardService
    {
        public ClipboardFormat GetClipboardFormat();
    }

    public enum ClipboardFormat
    {
        Html,
        All
    }
}
