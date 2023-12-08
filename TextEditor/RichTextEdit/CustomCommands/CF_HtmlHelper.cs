using System.Text;

namespace TextEditor.RichTextEdit.CustomCommands
{
    internal class CF_HtmlHelper
    {
        //HTML Clipboard Format http://msdn.microsoft.com/en-us/library/aa767917(v=vs.85).aspx
        private const string StartFragmentTag = "<!--StartFragment-->";
        private const string EndFragmentTag = "<!--EndFragment-->";

        private const string BodyTag = "<body>\r\n";
        private const string BodyTagClose = "</body>";
        private const string EmptyDescription = "Version:0.9\r\nStartHTML:{0:D10}\r\nEndHTML:{1:D10}\r\nStartFragment:{2:D10}\r\nEndFragment:{3:D10}\r\n";

        public static string GetHtmlClipboardFormat(string html)
        {
            var startBodyTagPos = html.IndexOf(BodyTag, StringComparison.Ordinal);
            var bodyEndTagPos = html.LastIndexOf(BodyTagClose, StringComparison.Ordinal);

            var contentBeforeFramentLength = startBodyTagPos + BodyTag.Length;
            var contentBeforeFragment = html.Substring(0, contentBeforeFramentLength);

            var fragment = html.Substring(contentBeforeFramentLength, bodyEndTagPos - contentBeforeFramentLength);

            var contentAfterFragment = html.Substring(bodyEndTagPos, html.Length - bodyEndTagPos);

            var result = Get_CF_HTML(contentBeforeFragment + StartFragmentTag, fragment,
                EndFragmentTag + contentAfterFragment);

            return result;
        }

        static string Get_CF_HTML(string contentBeforeFragment, string fragment, string contentAfterFragment)
        {
            var contentBeforeFragmentCount = Encoding.UTF8.GetByteCount(contentBeforeFragment);
            var fragmentCount = Encoding.UTF8.GetByteCount(fragment);
            var contentAfterFragmentCount = Encoding.UTF8.GetByteCount(contentAfterFragment);

            var descriptionOffset = Encoding.UTF8.GetByteCount(String.Format(EmptyDescription, 0, 0, 0, 0));
            var endHtmlOffset = descriptionOffset + contentBeforeFragmentCount + fragmentCount +
                                contentAfterFragmentCount;
            var startFragmentOffset = descriptionOffset + contentBeforeFragmentCount;
            var endFragmentOffset = descriptionOffset + contentBeforeFragmentCount + fragmentCount;

            var description = String.Format(EmptyDescription, descriptionOffset, endHtmlOffset, startFragmentOffset,
                endFragmentOffset);

            var content = new StringBuilder();
            content.Append(description);
            content.Append(contentBeforeFragment);
            content.Append(fragment);
            content.Append(contentAfterFragment);

            return content.ToString();
        }
    }
}
