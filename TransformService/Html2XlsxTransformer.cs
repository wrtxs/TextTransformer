using System.Collections.Generic;
using System.IO;
using System.Text;
using Aspose.Cells;
using DevExpress.XtraRichEdit.Import.Doc;
using HtmlAgilityPack;

namespace TransformService
{
    public static class Html2XlsxTransformer
    {
        private static readonly Workbook Workbook;
        private static readonly Style CellStyle;
        private static readonly Style HeaderStyle;

        static Html2XlsxTransformer()
        {
            Workbook = new Workbook(FileFormatType.Xlsx);
            CellStyle = Workbook.CreateStyle();
            CellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            CellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            CellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellStyle.IsTextWrapped = true;
            CellStyle.VerticalAlignment = TextAlignmentType.Top;

            HeaderStyle = Workbook.CreateStyle();
            HeaderStyle.Copy(CellStyle);
            HeaderStyle.ForegroundColor = HtmlUtils.CommonTableHeaderColor.Value;
            HeaderStyle.Pattern = BackgroundType.Solid;
        }

        public static MemoryStream Transform(string htmlData, bool autoFitCellsWidth)
        {
            var docNode = HtmlUtils.GetHtmlNodeFromText(htmlData);

            //var doc = new HtmlDocument();
            //doc.LoadHtml(htmlData);
            Workbook.Worksheets.Clear();

            var tableNode = docNode.SelectSingleNode("//table");

            //var worksheet = CreateTextInTempWorksheet(htmlData);
            var worksheet = (tableNode != null)
                ? CreateTableInTempWorksheet(tableNode)
                : CreateTextInTempWorksheet(htmlData);

            if (worksheet != null)
            {
                if (autoFitCellsWidth)
                {
                    worksheet.AutoFitColumns();
                    worksheet.AutoFitRows();
                }
                else
                {
                    worksheet.AutoFitRows();
                    worksheet.AutoFitColumns();
                }
            }

            var ms = new MemoryStream();
            Workbook.Save(ms, SaveFormat.Xlsx);
            //Workbook.Save("C:\\Users\\admin\\Desktop\\1\\12.xlsx", SaveFormat.Xlsx);
            Workbook.Worksheets.Clear();
            ms.Position = 0;

            return ms;
        }

        private static Worksheet CreateTableInTempWorksheet(HtmlNode tableNode)
        {
            var worksheetIndex = Workbook.Worksheets.Add();
            var worksheet = Workbook.Worksheets[worksheetIndex];

            var colSpanMap = new Dictionary<int, int>();
            var y = 0;

            var rows = tableNode.SelectNodes("//tr");

            if (rows == null)
                return null;

            foreach (var row in rows)
            {
                var x = 0;
                foreach (var cell in row.SelectNodes("th|td"))
                {
                    var rowSpan = int.Parse(cell.GetAttributeValue("rowspan", "1"));
                    var colSpan = int.Parse(cell.GetAttributeValue("colspan", "1"));

                    // Вычисляем значение x с учетом объединенных ячеек
                    x = HtmlUtils.GetXWithColSpan(y, x, colSpanMap);

                    var cellRange = worksheet.Cells.CreateRange(y, x, rowSpan, colSpan);
                    cellRange.Merge();
                    cellRange[0, 0].HtmlString = cell.InnerHtml;

                    var isHeaderCell = HtmlUtils.IsCellHeader(cell, true, null);
                    cellRange.ApplyStyle(isHeaderCell ? HeaderStyle : CellStyle, new StyleFlag { All = true });

                    // Обновляем карту объединенных ячеек для текущей строки
                    for (var i = x; i < x + colSpan; i++)
                    {
                        colSpanMap[i] = y + rowSpan;
                    }

                    x += colSpan;
                }

                y++;
            }

            return worksheet;
        }

        private static Worksheet CreateTextInTempWorksheet(string htmlText,
            TextInsertMode mode = TextInsertMode.HtmlString)
        {
            var worksheetIndex = Workbook.Worksheets.Add();
            var worksheet = Workbook.Worksheets[worksheetIndex];

            if (mode == TextInsertMode.HtmlBinary)
            {
                // Конвертация HTML текста в байты
                var htmlBytes = Encoding.UTF8.GetBytes(htmlText);

                using var stream = new MemoryStream(htmlBytes);
                var loadOptions = new HtmlLoadOptions()
                {
                    AutoFitColsAndRows = true,
                    ConvertNumericData = false,
                    ConvertDateTimeData = false,
                    CheckDataValid = false,
                    ParsingFormulaOnOpen = false
                };

                var tmpWorkbook = new Workbook(stream, loadOptions);
                var tmpWorksheet = tmpWorkbook.Worksheets[0];

                var range = tmpWorksheet.Cells.MaxDisplayRange;
                var copiedRange = worksheet.Cells.CreateRange(0, 0, range.RowCount, range.ColumnCount);
                copiedRange.CopyData(range);
            }
            else
            {
                worksheet.Cells["A1"].HtmlString = htmlText;
            }

            return worksheet;
        }

        private enum TextInsertMode
        {
            HtmlString,
            HtmlBinary
        }
    }
}