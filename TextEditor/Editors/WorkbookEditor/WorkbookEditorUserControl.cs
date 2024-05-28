using DevExpress.LookAndFeel;
using DevExpress.Spreadsheet;
using DevExpress.XtraEditors;
using System.IO;
using System.Text;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Export.Html;
using DevExpress.XtraSpreadsheet.Menu;
using TransformService;
using TransformService.TableMetadata;

namespace TextEditor.Editors.WorkbookEditor
{
    public sealed partial class WorkbookEditorUserControl : XtraUserControl, IEditorService, ITableMetadataManager//, IClipboardService
    {
        private RichTextEditForm _richTextEditForm;    // Форма редактирования форматированного текста
        private SpreadsheetMenuItem _setRichTextItem;  // Команда контестного меню для вызова редактора форматированного текста

        public WorkbookEditorUserControl()
        {
            InitializeComponent();

            spreadsheetControl.LookAndFeel.UseDefaultLookAndFeel = false;
            spreadsheetControl.LookAndFeel.SetSkinStyle(SkinSvgPalette.Office2019Colorful.Forest);

            spreadsheetControl.CellBeginEdit += spreadsheetControl_CellBeginEdit;
            spreadsheetControl.PopupMenuShowing += spreadsheetControl_PopupMenuShowing;

            ribbonControl.SelectedPage = homeRibbonPage1;
            //ribbonControl.ForceInitialize();

            spreadsheetControl.ContentChanged += SpreadsheetControl_ContentChanged;
            spreadsheetControl.ActiveSheetChanged += SpreadsheetControl_ContentChanged;
        }

        private void SpreadsheetControl_ContentChanged(object sender, EventArgs e) => ContentChanged?.Invoke(sender, e);

        #region IEditorService

        public void SetHtmlContent(string htmlData, IEditorParameters editorParameters)
        {
            var workbookEditorParameters =
                editorParameters as WorkbookEditorParameters ?? new WorkbookEditorParameters();

            using var wbStream = Html2XlsxTransformer.Transform(htmlData, workbookEditorParameters.AutoFitCellsWidth);
            LoadExcelIntoSpreadsheet(wbStream, spreadsheetControl.Document, workbookEditorParameters);

            // Записываем метаданные таблицы
            EditorUtils.SetTableMetadata(this, htmlData, true);
        }

        private void LoadExcelIntoSpreadsheet(Stream wbStream, IWorkbook workbook, WorkbookEditorParameters workbookEditorParameters)
        {
            // Приостанавливаем обновление UI контрола
            spreadsheetControl.BeginUpdate();
            spreadsheetControl.Document.BeginUpdate();

            try
            {
                // Создание временной книги для загрузки данных из потока
                var tempWorkbook = new Workbook();
                tempWorkbook.LoadDocument(wbStream, DocumentFormat.Xlsx);
                var tempSheet = tempWorkbook.Worksheets[0];

                // Очистка активного листа перед вставкой новых данных
                var activeSheet = workbook.Worksheets.ActiveWorksheet;

                // Копирование содержимого из первого листа временной книги в активный лист
                if (workbookEditorParameters.FastInsertHtmlData) // Копирование без сохранения истории Undo}
                {
                    activeSheet.CopyFrom(tempSheet); 
                }
                else // Копирование с сохранением истории Undo
                {
                    CopySheetContentWithStyles(tempSheet, activeSheet);

                    //activeSheet.Cells.AutoFitColumns();
                    //activeSheet.Cells.AutoFitRows();
                }

                // Установка курсора на начальную позицию
                spreadsheetControl.ActiveWorksheet.SelectedCell = spreadsheetControl.ActiveWorksheet.Cells["A2"];
                spreadsheetControl.ActiveWorksheet.SelectedCell = spreadsheetControl.ActiveWorksheet.Cells["A1"];
                spreadsheetControl.ActiveWorksheet.ScrollTo(0, 0);
            }
            finally
            {
                // Возобновляем обновление UI контрола и обновляем его содержимое
                spreadsheetControl.Document.EndUpdate();
                spreadsheetControl.EndUpdate();
            }

            //// Создание временной книги для загрузки данных из потока
            //var tempWorkbook = new Workbook();
            //tempWorkbook.LoadDocument(wbStream, DocumentFormat.Xlsx);
            //var tempSheet = tempWorkbook.Worksheets[0];

            //// Очистка активного листа перед вставкой новых данных
            //var activeSheet = workbook.Worksheets.ActiveWorksheet;
            //activeSheet.Clear(activeSheet.GetUsedRange());

            //// Копирование содержимого из первого листа временной книги в активный лист
            //activeSheet.CopyFrom(tempSheet);
            //spreadsheetControl.ActiveWorksheet.SelectedCell = spreadsheetControl.ActiveWorksheet.Cells["A2"];
            //spreadsheetControl.ActiveWorksheet.SelectedCell = spreadsheetControl.ActiveWorksheet.Cells["A1"];

            ////activeSheet.Rows.AutoFit(1,1000);
            ////activeSheet.Columns.AutoFit(1, 1000);
        }
        private void CopySheetContentWithStyles(Worksheet sourceSheet, Worksheet destinationSheet)
        {
            ClearWorksheetContent(destinationSheet);
            var usedRange = sourceSheet.GetUsedRange();

            // Копирование данных, высоты строк и ширины столбцов
            for (var rowIndex = 0; rowIndex < usedRange.RowCount; rowIndex++)
            {
                var sourceRow = sourceSheet.Rows[rowIndex];
                var destinationRow = destinationSheet.Rows[rowIndex];

                // Копирование высоты строк
                if (sourceRow.Height != 0)
                    destinationRow.Height = sourceRow.Height;

                for (var columnIndex = 0; columnIndex < usedRange.ColumnCount; columnIndex++)
                {
                    var sourceCell = sourceSheet.Cells[rowIndex, columnIndex];
                    var destinationCell = destinationSheet.Cells[rowIndex, columnIndex];
                    destinationCell.CopyFrom(sourceCell);

                    // Копирование ширины столбцов только для первой строки
                    if (rowIndex == 0)
                    {
                        var sourceColumn = sourceSheet.Columns[columnIndex];
                        var destinationColumn = destinationSheet.Columns[columnIndex];

                        if (sourceColumn.Width != 0)
                            destinationColumn.Width = sourceColumn.Width;
                    }
                }
            }

            // Копирование объединенных ячеек
            var mergedRanges = sourceSheet.Cells.GetMergedRanges();
            foreach (var range in mergedRanges)
            {
                destinationSheet.Range.FromLTRB(range.LeftColumnIndex, range.TopRowIndex,
                    range.RightColumnIndex, range.BottomRowIndex).Merge();
            }
        }

        private void ClearWorksheetContent(Worksheet worksheet)
        {
            var usedRange = worksheet.GetUsedRange();
            worksheet.Clear(usedRange);

            // Сбросить ширину всех столбцов до значения по умолчанию
            for (var i = 0; i < worksheet.Columns.LastUsedIndex; i++)
            {
                worksheet.Columns[i].Width = worksheet.DefaultColumnWidth;
            }

            // Сбросить высоту всех строк до значения по умолчанию
            for (var i = 0; i < worksheet.Rows.LastUsedIndex; i++)
            {
                worksheet.Rows[i].Height = worksheet.DefaultRowHeight;
            }
        }

        public string GetHtmlContent(bool needActualColumnWidths = false)
        {
            var workbook = spreadsheetControl.Document;

            workbook.Calculate();
            var worksheet = workbook.Worksheets.ActiveWorksheet;

            using var ms = new MemoryStream();

            workbook.BeforeExport += WorkbookOnBeforeExport;
            workbook.ExportToHtml(ms, worksheet);
            workbook.BeforeExport -= WorkbookOnBeforeExport;
            var htmlContent = Encoding.UTF8.GetString(ms.ToArray());

            // Записываем метаданные таблицы
            htmlContent = TableMetadataUtils.SetFirstTableMetadata(htmlContent, GetTableMetadata(needActualColumnWidths));

            return htmlContent;
        }

        private static void WorkbookOnBeforeExport(object sender, SpreadsheetBeforeExportEventArgs e)
        {
            if (e.Options is HtmlDocumentExporterOptions options)
            {
                options.Encoding = Encoding.UTF8;
                options.ExportRootTag = ExportRootTag.Html;
                options.CssPropertiesExportType = CssPropertiesExportType.Inline;
                //options.EmbedImages = false;
                //options.UseSpanTagForIndentation = true;
                
                //exportHtml.TabMarker = "&nbsp;&nbsp;&nbsp;&nbsp;";
                //exportHtml.HtmlNumberingListExportFormat = HtmlNumberingListExportFormat.HtmlFormat;
            }
        }

        public bool HasContent() =>
            spreadsheetControl.ActiveWorksheet.GetExistingCells().Any(cell => !cell.Value.IsEmpty);

        public event EventHandler ContentChanged;

        #endregion

        #region ITableMetadataManager   
        private TableMetadata _tableMetadata = new();

        public TableMetadata GetTableMetadata(bool needActualColumnWidths = false) =>
            new(barEditItemTableTitle.EditValue as string, _tableMetadata.OriginalColumnWidths,
                needActualColumnWidths ? GetActualColumnWidths() : null);

        private IEnumerable<int> GetActualColumnWidths()
        {
            var activeSheet = spreadsheetControl.Document.Worksheets.ActiveWorksheet;
            var columnWidths = new List<int>();

            for (var colIndex = 0; colIndex <= activeSheet.Columns.LastUsedIndex; colIndex++)
            {
                columnWidths.Add((int)activeSheet.Columns[colIndex].Width);
            }

            return columnWidths;
        }

        public void SetTableMetadata(TableMetadata tableMetadata)
        {
            _tableMetadata = tableMetadata != null ? tableMetadata.Clone() : new TableMetadata();
            barEditItemTableTitle.EditValue = tableMetadata?.Title;
        }
        #endregion

        #region Event handlers
        private void spreadsheetControl_CellBeginEdit(object sender,
            DevExpress.XtraSpreadsheet.SpreadsheetCellCancelEventArgs e)
        {
            if (e.Cell.HasRichText)
            {
                e.Cancel = true;
                ShowRichTextEditForm(e.Cell);
            }
        }

        private void ShowRichTextEditForm(Cell cell)
        {
            _richTextEditForm ??= new RichTextEditForm();
            _richTextEditForm.SetCell(cell);
            _richTextEditForm.ShowDialog();
        }

        private void spreadsheetControl_PopupMenuShowing(object sender,
            DevExpress.XtraSpreadsheet.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == DevExpress.XtraSpreadsheet.SpreadsheetMenuType.Cell)
            {
                Cell activeCell = spreadsheetControl.ActiveCell;
                if (activeCell.Value.IsEmpty || (!activeCell.HasFormula && activeCell.Value.IsText))
                {
                    if (_setRichTextItem == null)
                    {
                        _setRichTextItem = new SpreadsheetMenuItem("Форматированный текст...", SetRichTextItemClick);
                        _setRichTextItem.Image =
                            DevExpress.Images.ImageResourceCache.Default.GetImage(
                                Utils.GetNormalizedString("Office2013/Reports/ConvertToParagraphs_16x16.png"));
                    }

                    e.Menu.Items.Add(_setRichTextItem);
                }
            }
        }

        private void SetRichTextItemClick(object sender, EventArgs e)
        {
            ShowRichTextEditForm(spreadsheetControl.ActiveCell);
        }
        #endregion
    }
}