using System;
using System.Linq;
using System.Text;
using TransfromService.JsonData;

namespace TransfromService
{
    public class Json2HtmlTransformer
    {
        public string Transform(string jsonData)
        {
            var htmlResult = string.Empty;

            if (string.IsNullOrEmpty(jsonData))
                return htmlResult;

            var jsonObject = JsonUtils.DeserializeObject(jsonData);

            if (jsonObject is TableJsonRoot { Content: { Table.Cells: not null } tableRoot })
            {
                htmlResult = CreateHtmlTable(tableRoot);
            }
            else if (jsonObject is TextJsonRoot textRoot)
            {
                htmlResult = CreateHtmlText(textRoot);
            }

            return htmlResult;
        }

        private string CreateHtmlText(TextJsonRoot textRoot)
        {
            var htmlBuilder = new StringBuilder();

            htmlBuilder.Append(GetHtmlDocBegin());
            htmlBuilder.Append(GetHtmlBodyBegin());
            htmlBuilder.Append(textRoot.Content.Value);
            htmlBuilder.Append(GetHtmlDocEnd());

            return htmlBuilder.ToString();
        }

        private string CreateHtmlTable(TableRootContent tableRoot)
        {
            var result = string.Empty;
            var cells = tableRoot.Table.Cells.OrderBy(c => c.Y).ThenBy(c => c.X).ToList();

            if (cells.Any())
            {
                var numRows = cells.Max(c => c.Y) + 1;
                var numCols = cells.Max(c => c.X) + 1;

                var headerStyleClassName = "cs162A16FE1";

                var htmlBuilder = new StringBuilder();
                
                htmlBuilder.Append(GetHtmlDocBegin());
                htmlBuilder.Append("<style type=\"text/css\">");
                htmlBuilder.Append(
                    $".{headerStyleClassName}{{background-color:{HtmlUtils.CommonTableHeaderColor.HexValue};}}");
                htmlBuilder.Append("</style>");
                htmlBuilder.Append(GetHtmlBodyBegin());
                htmlBuilder.Append("<table" + (string.IsNullOrEmpty(tableRoot.Title)
                    ? ">"
                    : $" title='{tableRoot.Title}'>"));

                for (var row = 0; row < numRows; row++)
                {
                    htmlBuilder.Append("<tr>");

                    for (var col = 0; col < numCols; col++)
                    {
                        var cell = cells.FirstOrDefault(c => c.X == col && c.Y == row);
                        if (cell != null)
                        {
                            var cellTag = cell.IsHeader == true ? "th" : "td";
                            var styleClass = cell.IsHeader == true
                                ? $" class=\"{headerStyleClassName}\""
                                : string.Empty;

                            var cellValue = cell.Items.FirstOrDefault()?.Content?.Value ?? string.Empty;

                            var colSpan = cell.W > 1 ? $" colspan=\"{cell.W}\"" : string.Empty;
                            var rowSpan = cell.H > 1 ? $" rowspan=\"{cell.H}\"" : string.Empty;

                            htmlBuilder.AppendFormat("<{0}{1}{2}{3}>{4}</{0}>", cellTag, styleClass, rowSpan,
                                colSpan, cellValue);
                        }
                    }

                    htmlBuilder.Append("</tr>");
                }

                htmlBuilder.Append("</table>");
                htmlBuilder.Append(GetHtmlDocEnd());
                result = htmlBuilder.ToString();
            }

            return result;
        }

        private string GetHtmlDocBegin() =>
            "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">" +
            Environment.NewLine + "<html xmlns=\"http://www.w3.org/1999/xhtml\">";

        private string GetHtmlBodyBegin() => "<body>";

        private string GetHtmlDocEnd() => "</body>" + Environment.NewLine + "</html>";
    }
}