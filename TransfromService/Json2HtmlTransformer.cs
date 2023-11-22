using System.Linq;
using Newtonsoft.Json;
using System.Text;
using TransfromService.JsonData;

namespace TransfromService
{
    public class Json2HtmlTransformer
    {
        public string Transform(string jsonData)
        {
            string htmlResult = string.Empty;

            if (string.IsNullOrEmpty(jsonData))
                return htmlResult;

            Root root = JsonConvert.DeserializeObject<Root>(jsonData);

            if (root != null && root.Content != null && root.Content.Table != null && root.Content.Table.Cells != null)
            {
                var table = root.Content.Table;
                var cells = table.Cells.OrderBy(c => c.Y).ThenBy(c => c.X).ToList();

                if (cells.Any())
                {
                    int numRows = cells.Max(c => c.Y) + 1;
                    int numCols = cells.Max(c => c.X) + 1;

                    var tableBuilder = new StringBuilder();
                    var headerStyleClassName = "cs162A16FE1";

                    tableBuilder.Append(
                        "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
                    tableBuilder.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
                    tableBuilder.Append("<style type=\"text/css\">");
                    tableBuilder.Append(
                        $".{headerStyleClassName}{{background-color:{Utils.CommonTableHeaderColor.HexValue};}}");
                    tableBuilder.Append("</style>");
                    tableBuilder.Append("<body>");
                    tableBuilder.Append("<table>");

                    for (var row = 0; row < numRows; row++)
                    {
                        tableBuilder.Append("<tr>");

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

                                tableBuilder.AppendFormat("<{0}{1}{2}{3}>{4}</{0}>", cellTag, styleClass, rowSpan,
                                    colSpan, cellValue);
                            }
                        }

                        tableBuilder.Append("</tr>");
                    }

                    tableBuilder.Append("</table>");
                    tableBuilder.Append("</body>");
                    tableBuilder.Append("</html>");
                    htmlResult = tableBuilder.ToString();
                }
            }


            return htmlResult;
        }
    }
}
