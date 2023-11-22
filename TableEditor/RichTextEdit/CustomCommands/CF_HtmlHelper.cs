using System.Text;

namespace TableEditor.RichTextEdit.CustomCommands
{
    internal class CF_HtmlHelper
    {
        //HTML Clipboard Format http://msdn.microsoft.com/en-us/library/aa767917(v=vs.85).aspx
        const string StartFragmentTag = "<!--StartFragment-->";
        const string EndFragmentTag = "<!--EndFragment-->";

        const string bodyTag = "<body>\r\n";
        const string bodyTagClose = "</body>";
        const string EmptyDescription = "Version:0.9\r\nStartHTML:{0:D10}\r\nEndHTML:{1:D10}\r\nStartFragment:{2:D10}\r\nEndFragment:{3:D10}\r\n";

        public static string GetHtmlClipboardFormat(string html)
        {
            int startBodyTagPos = html.IndexOf(bodyTag);
            int bodyEndTagPos = html.LastIndexOf(bodyTagClose);

            int contentBeforeFramentLength = startBodyTagPos + bodyTag.Length;
            string contentBeforeFragment = html.Substring(0, contentBeforeFramentLength);

            string fragment = html.Substring(contentBeforeFramentLength, bodyEndTagPos - contentBeforeFramentLength);

            string contentAfterFragment = html.Substring(bodyEndTagPos, html.Length - bodyEndTagPos);

            string result = Get_CF_HTML(contentBeforeFragment + StartFragmentTag, fragment, EndFragmentTag + contentAfterFragment);

            return result;
        }

        static string Get_CF_HTML(string contentBeforeFragment, string fragment, string contentAfterFragment)
        {
            var contentBeforeFragmentCount = Encoding.UTF8.GetByteCount(contentBeforeFragment);
            var fragmentCount = Encoding.UTF8.GetByteCount(fragment);
            var contentAfterFragmentCount = Encoding.UTF8.GetByteCount(contentAfterFragment);

            var descriptionOffset = Encoding.UTF8.GetByteCount(String.Format(EmptyDescription, 0, 0, 0, 0));
            var endHTMLOffset = descriptionOffset + contentBeforeFragmentCount + fragmentCount + contentAfterFragmentCount;
            var startFragmentOffset = descriptionOffset + contentBeforeFragmentCount;
            var endFragmentOffset = descriptionOffset + contentBeforeFragmentCount + fragmentCount;

            var description = String.Format(EmptyDescription, descriptionOffset, endHTMLOffset, startFragmentOffset, endFragmentOffset);

            StringBuilder content = new StringBuilder();
            content.Append(description);
            content.Append(contentBeforeFragment);
            content.Append(fragment);
            content.Append(contentAfterFragment);
            return content.ToString();
        }
    }
}
