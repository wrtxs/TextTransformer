using System.IO;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Layout;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraSpellChecker.Native;
using TextEditor.Editors.RichTextEditor.CustomCommands;
using TransformService.RichText;
using TransformService.TableMetadata;
//using Table = DevExpress.XtraRichEdit.API.Native.Table;
//using TableLayoutType = DevExpress.XtraRichEdit.API.Native.TableLayoutType;

namespace TextEditor.Editors.RichTextEditor
{
    public sealed partial class RichTextEditorUserControl : XtraUserControl, IEditorService, ITableMetadataManager,
        IClipboardService
    {
        private DevExpress.XtraBars.BarCheckItem _copyFormatItem;
        private DevExpress.XtraBars.PopupMenu _popupMenu;

        private bool _isZoomChanging;
        private int _pageCount = 1;
        private int _currentPage = 1;

        public RichTextEditorUserControl()
        {
            //OfficeCharts.Instance.ActivateWinFormsCharts();
            InitializeComponent();

            //LookAndFeel.UseDefaultLookAndFeel = false;
            //LookAndFeel.SetSkinStyle(SkinSvgPalette.DefaultSkin.VioletDark);

            Load += RichTextUserControl_Load;

            // Инициализируем RichEditControl
            InitializeRichEditControl();

            //RichEditControlCompatibility.EnableMSWordCompatibleFieldParser = true;
            //RichEditControlCompatibility.UseThemeFonts = false;
            //richEditControl.Options.DocumentCapabilities.InlinePictures = DocumentCapability.Enabled;
            //clipboardRibbonPageGroup1.ItemLinks.Remove(pasteSpecialItem1);
        }

        private void InitializeRichEditControl()
        {
            // Устанавливаем обработчик ошибок
            new RichEditControlExceptionHandler(richEditControl).Install();

            //richEditControl.Initialize(this);

            richEditControl.Text = string.Empty;

            richEditControl.ZoomChanged += richEditControl_ZoomChanged;
            richEditControl.ZoomChanged += richEditControl_ZoomChanged;
            richEditControl.SelectionChanged += richEditControl_SelectionChanged;
            richEditControl.InvalidFormatException += richEditControl_InvalidFormatException;
            richEditControl.VisiblePagesChanged += richEditControl_VisiblePagesChanged;
            richEditControl.DocumentClosing += richEditControl_DocumentClosing;

            richEditControl.ActiveViewType = RichEditViewType.Simple;
            //richEditControl.LayoutUnit = DocumentLayoutUnit.Pixel;
            //richEditControl.Location = new Point(0, 0);
            //richEditControl.MenuManager = ribbonControl;
            //richEditControl.Name = "richEditControl";
            richEditControl.Options.DocumentSaveOptions.CurrentFormat = DocumentFormat.OpenXml;
            //richEditControl.Size = new Size(1014, 295);
            //richEditControl.TabIndex = 17;
            richEditControl.MouseUp += richEditControl_MouseUp;

            // Устанавливаем альбомную ориентацию
            SetLandscapeOrientation();

            // Устанавливаем параметры работы с буфером обмена и параметры экспорта
            var clipboardFormats = richEditControl.Options.ClipboardFormats;
            clipboardFormats.PlainText = RichEditClipboardMode.Enabled;
            clipboardFormats.Rtf = RichEditClipboardMode.Enabled;
            clipboardFormats.Html = RichEditClipboardMode.Enabled;

            richEditControl.Options.Export.Html.SetCommonExportOptions();

            // Добавляем дополнительные команды ("Формат по образцу") и переопределяем панель команд
            AdjustCommandsBar();

            // Создаем строку состояния
            AdjustStatusBar();

            // Переопределяем стандартные команды (копирование в буфер обмена)
            RedefineStandardCommands();

            ribbonControl.SelectedPage = homeRibbonPage1;

            LoadDictionaries();

            ribbonControl.ForceInitialize();

            richEditControl.ContentChanged += RichEditControl_ContentChanged;
        }

        private void RichEditControl_ContentChanged(object sender, EventArgs e) => ContentChanged?.Invoke(sender, e);

        private void LoadDictionaries()
        {
            spellChecker.Dictionaries.Clear();

            var ruAffStream = new MemoryStream(Properties.Resources.ru_RU_aff);
            var ruDicStream = new MemoryStream(Properties.Resources.ru_RU_dic);

            var enAffStream = new MemoryStream(Properties.Resources.en_US_aff);
            var enDicStream = new MemoryStream(Properties.Resources.en_US_dic);

            var ruDict = new HunspellDictionary();
            ruDict.LoadFromStream(ruDicStream, ruAffStream);

            var enDict = new HunspellDictionary();
            enDict.LoadFromStream(enDicStream, enAffStream);

            //dictionary.Encoding = 

            spellChecker.Dictionaries.Add(ruDict);
            spellChecker.Dictionaries.Add(enDict);

            SpellCheckTextControllersManager.Default.RegisterClass(typeof(RichEditControlEx),
                typeof(DevExpress.XtraRichEdit.SpellChecker.RichEditSpellCheckController));
        }

        private void AdjustCommandsBar()
        {
            clipboardRibbonPageGroup1.Text = @"Буфер обмена";

            // Добавляем команду Формат по образцу
            _copyFormatItem = new DevExpress.XtraBars.BarCheckItem();
            ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[] { _copyFormatItem });

            _copyFormatItem.Caption = @"Формат по образцу";
            _copyFormatItem.Id = 302;
            _copyFormatItem.ImageOptions.Image = Properties.Resources.applyStyleIco;
            _copyFormatItem.Name = "copyFormatItem";
            _copyFormatItem.RibbonStyle = RibbonItemStyles.SmallWithText |
                                          RibbonItemStyles.SmallWithoutText;
            _copyFormatItem.CheckedChanged += copyFormatItem_CheckedChanged;
            clipboardRibbonPageGroup1.ItemLinks.Add(_copyFormatItem);

            // Добавляем всплывающее меню для команды Вставка (с элементом Специальная вставка)
            _popupMenu = new DevExpress.XtraBars.PopupMenu(components);
            ((System.ComponentModel.ISupportInitialize)_popupMenu).BeginInit();
            pasteItem1.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
            pasteItem1.CloseRadialMenuOnItemClick = true;
            pasteItem1.CloseSubMenuOnClickMode = DevExpress.Utils.DefaultBoolean.True;
            pasteItem1.DropDownControl = _popupMenu;
            clipboardRibbonPageGroup1.ItemLinks.Remove(pasteSpecialItem1);
            _popupMenu.ItemLinks.Add(pasteSpecialItem1);
            _popupMenu.Name = "popupMenu";
            _popupMenu.Ribbon = ribbonControl;
            ((System.ComponentModel.ISupportInitialize)_popupMenu).EndInit();
        }

        private void SetLandscapeOrientation()
        {
            richEditControl.Options.DocumentCapabilities.Undo = DocumentCapability.Disabled;
            richEditControl.Document.Sections[0].Page.Landscape = true;
            richEditControl.Options.DocumentCapabilities.Undo = DocumentCapability.Enabled;
        }

        private void RedefineStandardCommands()
        {
            var commandFactory = new CustomRichEditCommandFactoryService(richEditControl, this, this,
                richEditControl.GetService<IRichEditCommandFactoryService>());

            richEditControl.RemoveService(typeof(IRichEditCommandFactoryService));
            richEditControl.AddService(typeof(IRichEditCommandFactoryService), commandFactory);
        }

        //public void InsertEmptyTable()
        //{
        //    //var richEdit = new RichEditDocumentServer();

        //    var doc = richEditControl.Document;
        //    var table = doc.Tables.Create(doc.Range.End, 6, 6);

        //    table.BeginUpdate();

        //    table.ForEachCell(((cell, rowIndex, cellIndex) =>
        //    {
        //        if (rowIndex == 0)
        //        {
        //            cell.BackgroundColor = Color.FromArgb(191, 191, 191);
        //            doc.InsertSingleLineText(cell.Range.Start, "Столбец " + (cellIndex + 1));
        //        }
        //    }));

        //    table.EndUpdate();
        //}

        private Document GetDocument() => richEditControl.Document;
        //public HtmlDocumentExporterOptions GetHtmlExporterOptions() => richEditControl.Options.Export.Html;

        private Table GetFirstTable() => GetDocument().Tables.First;

        public void AutoFitTable()
        {
            //var doc = GetDocument();
            var table = GetFirstTable();

            if (table == null)
                return;

            table.BeginUpdate();

            table.TableLayout = TableLayoutType.Autofit;
            table.ForEachRow((row, _) => row.HeightType = HeightType.Auto);
            table.ForEachCell(((cell, _, _) => cell.PreferredWidthType = WidthType.Auto));

            //table.TableLayout = TableLayoutType.Autofit;

            //table.ForEachCell(((cell, rowIndex, cellIndex) =>
            //{
            //    cell.PreferredWidthType = WidthType.FiftiethsOfPercent;

            //    var props = doc.BeginUpdateParagraphs(cell.Range);
            //    props.Alignment = ParagraphAlignment.Center;
            //    doc.EndUpdateParagraphs(props);
            //}));

            //table.PreferredWidthType = WidthType.FiftiethsOfPercent;
            //table.PreferredWidth = 5000;

            table.EndUpdate();
        }

        //public void SetSimpleViewType()
        //{
        //    richEditControl.ActiveViewType = RichEditViewType.Simple;
        //}

        //public RichEditControl RichEditControl => richEditControl;

        private int PageCount
        {
            get => _pageCount;
            set
            {
                if (_pageCount == value)
                    return;
                _pageCount = value;
                OnPagesInfoChanged();
            }
        }

        private int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage == value)
                    return;
                _currentPage = value;
                OnPagesInfoChanged();
            }
        }

        #region Status bar

        //private RibbonStatusBar _ribbonStatusBar;

        //private DevExpress.XtraBars.BarEditItem _barEditItemTableTitle;

        //private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit _txtTableTitle;
        private DevExpress.XtraBars.BarStaticItem _pagesBarItem;
        private DevExpress.XtraBars.BarEditItem _zoomBarEditItem;
        private DevExpress.XtraEditors.Repository.RepositoryItemZoomTrackBar _repositoryItemZoomTrackBar;

        private void AdjustStatusBar()
        {
            // Создаем панель статуса
            //_ribbonStatusBar = new RibbonStatusBar();
            //ribbonControl.StatusBar = _ribbonStatusBar;

            //_ribbonStatusBar.Location = new Point(0, 501);
            //_ribbonStatusBar.Name = "ribbonStatusBar";
            //_ribbonStatusBar.Ribbon = ribbonControl;
            //_ribbonStatusBar.Size = new Size(1008, 27);
            //Controls.Add(_ribbonStatusBar);

            // Добавляем дополнительные элементы в строку состояния

            //// txtTableTitle
            //_txtTableTitle = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            //((System.ComponentModel.ISupportInitialize)_txtTableTitle).BeginInit();
            //ribbonControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[]
            //{
            //    repositoryItemFontEditRichEdit1, repositoryItemRichEditFontSizeEdit1, displayForReviewModeComboBox1,
            //    repositoryItemBorderLineStyle1, repositoryItemBorderLineWeight1,
            //    repositoryItemFloatingObjectOutlineWeight1, _txtTableTitle
            //});
            //_txtTableTitle.AutoHeight = false;
            //_txtTableTitle.Name = "txtTableTitle";
            //((System.ComponentModel.ISupportInitialize)_txtTableTitle).EndInit();

            //// barEditItemTableTitle
            //_barEditItemTableTitle = new DevExpress.XtraBars.BarEditItem();
            //ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[] { _barEditItemTableTitle });

            //_barEditItemTableTitle.Caption = @"Заголовок таблицы";
            //_barEditItemTableTitle.Edit = _txtTableTitle;
            //_barEditItemTableTitle.EditWidth = 360;
            //_barEditItemTableTitle.Name = "barEditItemTableTitle";
            //_ribbonStatusBar.ItemLinks.Add(_barEditItemTableTitle, true);

            _pagesBarItem = new DevExpress.XtraBars.BarStaticItem();
            _zoomBarEditItem = new DevExpress.XtraBars.BarEditItem();
            _repositoryItemZoomTrackBar = new DevExpress.XtraEditors.Repository.RepositoryItemZoomTrackBar();

            ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[]
            {
                _pagesBarItem,
                _zoomBarEditItem,
            });

            ribbonControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[]
            {
                _repositoryItemZoomTrackBar,
            });

            // 
            // pagesBarItem
            // 
            var superToolTip1 = new DevExpress.Utils.SuperToolTip();
            var toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            _pagesBarItem.Id = 246;
            _pagesBarItem.Name = "pagesBarItem";
            toolTipItem1.Text = @"Номер страницы в документе.";
            superToolTip1.Items.Add(toolTipItem1);
            _pagesBarItem.SuperTip = superToolTip1;
            // 
            // zoomBarEditItem
            // 
            _zoomBarEditItem.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            _zoomBarEditItem.Caption = @"100%";
            _zoomBarEditItem.Edit = _repositoryItemZoomTrackBar;
            _zoomBarEditItem.EditValue = 100;
            _zoomBarEditItem.EditWidth = 150;
            _zoomBarEditItem.Id = 245;
            _zoomBarEditItem.Name = "zoomBarEditItem";
            _zoomBarEditItem.EditValueChanged += this.zoomBarEditItem_EditValueChanged;

            // 
            // repositoryItemZoomTrackBar1
            // 
            ((System.ComponentModel.ISupportInitialize)(_repositoryItemZoomTrackBar)).BeginInit();

            _repositoryItemZoomTrackBar.AllowUseMiddleValue = true;
            _repositoryItemZoomTrackBar.LargeChange = 50;
            _repositoryItemZoomTrackBar.Maximum = 500;
            _repositoryItemZoomTrackBar.Middle = 100;
            _repositoryItemZoomTrackBar.Minimum = 10;
            _repositoryItemZoomTrackBar.Name = "repositoryItemZoomTrackBar1";
            _repositoryItemZoomTrackBar.SmallChange = 10;
            _repositoryItemZoomTrackBar.SnapToMiddle = 2;

            ((System.ComponentModel.ISupportInitialize)(_repositoryItemZoomTrackBar)).EndInit();

            ribbonStatusBar.ItemLinks.Add(_pagesBarItem, true);
            ribbonStatusBar.ItemLinks.Add(_zoomBarEditItem);

            //this.ribbonStatusBar.Ribbon = ribbonControl;
        }

        #endregion

        private void RichTextUserControl_Load(object sender, EventArgs e)
        {
            richEditControl.DocumentLayout.DocumentFormatted += DocumentLayout_DocumentFormatted;
            OnPagesInfoChanged();
            //RichEdit.HyphenationDictionaries.Add(new OpenOfficeHyphenationDictionary(DemoUtils.GetRelativePath("hyph_en_US.dic"), new System.Globalization.CultureInfo("en-US")));
            //LoadDocument("FirstLook.docx");
        }

        private void DocumentLayout_DocumentFormatted(object sender, EventArgs e)
        {
            BeginInvoke(() => { PageCount = richEditControl.DocumentLayout.GetPageCount(); });
        }

        private void zoomBarEditItem_EditValueChanged(object sender, EventArgs e)
        {
            if (this._isZoomChanging)
                return;
            int value = Convert.ToInt32(_zoomBarEditItem.EditValue);
            this._isZoomChanging = true;
            try
            {
                richEditControl.ActiveView.ZoomFactor = value / 100f;
                _zoomBarEditItem.Caption = $@"{value}%";
            }
            finally
            {
                this._isZoomChanging = false;
            }
        }

        private void richEditControl_ZoomChanged(object sender, EventArgs e)
        {
            if (this._isZoomChanging)
                return;
            int value = (int)Math.Round(richEditControl.ActiveView.ZoomFactor * 100);
            this._isZoomChanging = true;
            try
            {
                _zoomBarEditItem.EditValue = value;
                _zoomBarEditItem.Caption = $@"{value}%";
            }
            finally
            {
                this._isZoomChanging = false;
            }
        }

        private void OnPagesInfoChanged()
        {
            _pagesBarItem.Caption = $@"Страница {CurrentPage} из {PageCount}";
        }

        private void richEditControl_VisiblePagesChanged(object sender, EventArgs e)
        {
            CurrentPage = richEditControl.ActiveView.GetVisiblePageLayoutInfos()[0].PageIndex + 1;
        }

        private void richEditControl_SelectionChanged(object sender, EventArgs e)
        {
            var element =
                richEditControl.DocumentLayout.GetElement<RangedLayoutElement>(richEditControl.Document.CaretPosition);
            if (element != null)
                CurrentPage = richEditControl.DocumentLayout.GetPageIndex(element) + 1;
        }

        private void richEditControl_InvalidFormatException(object sender, RichEditInvalidFormatExceptionEventArgs e)
        {
            XtraMessageBox.Show(
                $"Невозможно открыть файл '{richEditControl.Options.DocumentSaveOptions.CurrentFileName}' поскольку файл имеет недоступимый формат или расширение.\n" +
                "Удостоверьтесь, что файл не поврежден и расширение файла соответсвует его формату.",
                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void richEditControl_DocumentClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (richEditControl.Modified)
            {
                var currentFileName = richEditControl.Options.DocumentSaveOptions.CurrentFileName;
                var message = !string.IsNullOrEmpty(currentFileName)
                    ? $"Вы хотите сохранить изменения, сделанные для '{currentFileName}'?"
                    : "Вы хотите сохранить изменения?";
                var result = XtraMessageBox.Show(message, "Сохранение", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                    e.Cancel = !richEditControl.SaveDocument();
                else
                    e.Cancel = result == DialogResult.Cancel;
            }
        }

        #region Обработка команды применения стиля

        private void copyFormatItem_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (_copyFormatItem.Checked)
            {
                SaveSelectedRange();
                richEditControl.FormatCalculatorEnabled = true;
            }
            else
            {
                richEditControl.FormatCalculatorEnabled = false;
            }
        }

        private DocumentRange _sourceSelectedRange;

        private void SaveSelectedRange()
        {
            var selection = richEditControl.Document.Selection;
            var subDocument = selection.BeginUpdateDocument();
            _sourceSelectedRange = subDocument.CreateRange(selection.Start, richEditControl.Document.Selection.Length);
            selection.EndUpdateDocument(subDocument);
        }

        private void richEditControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (_copyFormatItem.Checked)
            {
                ApplyFormatToSelectedText();
                _copyFormatItem.Checked = false;
            }
        }

        private void ApplyFormatToSelectedText()
        {
            var targetSelectedRange = richEditControl.Document.Selection;

            richEditControl.BeginUpdate();
            var targetSubDocument = targetSelectedRange.BeginUpdateDocument();
            var subDocument = _sourceSelectedRange.BeginUpdateDocument();

            var targetCharactersProperties = targetSubDocument.BeginUpdateCharacters(targetSelectedRange);
            var sourceCharactersProperties = subDocument.BeginUpdateCharacters(_sourceSelectedRange);
            targetCharactersProperties.Assign(sourceCharactersProperties);
            subDocument.EndUpdateCharacters(sourceCharactersProperties);
            targetSubDocument.EndUpdateCharacters(targetCharactersProperties);

            var targetParagraphProperties = targetSubDocument.BeginUpdateParagraphs(targetSelectedRange);
            var sourceParagraphProperties = subDocument.BeginUpdateParagraphs(_sourceSelectedRange);
            targetParagraphProperties.Assign(sourceParagraphProperties);
            subDocument.EndUpdateParagraphs(sourceParagraphProperties);
            targetSubDocument.EndUpdateParagraphs(targetParagraphProperties);

            _sourceSelectedRange.EndUpdateDocument(subDocument);
            targetSelectedRange.EndUpdateDocument(targetSubDocument);
            richEditControl.EndUpdate();
        }

        #endregion

        #region IEditorService

        /// <summary>
        /// Формирование содержимого редактора на основе HTML
        /// </summary>
        /// <param name="htmlData"></param>
        /// <param name="parameters"></param>
        public void SetHtmlContent(string htmlData, IEditorParameters parameters)
        {
            //var richEditControl = richTextEditorUserControl.RichEditControl;
            var richTextEditorParameters = parameters as RichTextEditorParameters ?? new RichTextEditorParameters();

            richEditControl.Document.BeginUpdate();
            richEditControl.Document.Delete(richEditControl.Document.Range);

            using (var server = new RichEditDocumentServer())
            {
                server.HtmlText = htmlData;

                if (richTextEditorParameters.ApplyTableStyle)
                {
                    const string styleName = "Table Simple 1";

                    if (server.Document.Tables.Count > 0)
                    {
                        var tableStyle =
                            server.Document.TableStyles.FirstOrDefault(style =>
                                style.Name.Equals(styleName, StringComparison.OrdinalIgnoreCase));

                        if (tableStyle != null)
                        {
                            foreach (var documentTable in server.Document.Tables)
                            {
                                documentTable.Style = tableStyle;
                            }
                        }

                        //if (server.Document.TableStyles.Any(style => style.Name.Equals(styleName)))
                        //    server.Document.Tables.First.Style = server.Document.TableStyles[styleName];
                    }
                }

                var insertedRange = richEditControl.Document.InsertDocumentContent(
                    richEditControl.Document.Range.Start, server.Document.Range);

                // Удаляем последнюю пустую строку, которая появляется в редакторе после вставки контента
                richEditControl.Document.Delete(richEditControl.Document.CreateRange(insertedRange.End.ToInt() - 1, 1));

                richEditControl.Document.Sections[0].Page.Landscape = true;
                richEditControl.Document.CaretPosition =
                    richEditControl.Document.Range.Start;

                //richEditControl.ActiveViewType = RichEditViewType.Simple;

                var firstTable = richEditControl.Document.Tables.First;

                if (firstTable != null)
                {
                    firstTable.PreferredWidthType = WidthType.Auto;
                    firstTable.ForEachCell(
                        CellProcessor); // Устанавливаем серую заливку для соответствующих ячеек таблицы

                    // Устанавливаем ширину столбцов
                    //for (var i = 0; i < firstTable.FirstRow.Cells.Count && i < tableMetadata.OriginalColumnWidths.Count(); i++)
                    //{
                    //    foreach (var row in firstTable.Rows)
                    //    {
                    //        row.Cells[i].PreferredWidth = tableMetadata.OriginalColumnWidths.ElementAt(i);
                    //        row.Cells[i].PreferredWidthType = WidthType.Fixed;
                    //    }
                    //}

                    //firstTable.TableLayout = TableLayoutType.Fixed;

                    //var lt = richEditControl.DocumentLayout.GetElement<LayoutTable>(table.Range.Start);

                    //table.PreferredWidth = Units.TwipsToCentimetersF(lt.Bounds.Width);

                    //for (var rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                    //{
                    //    for (var cellIndex = 0; cellIndex < table.Rows[rowIndex].Cells.Count; cellIndex++)
                    //    {
                    //        var cell = table[rowIndex, cellIndex];
                    //        cell.PreferredWidthType = WidthType.Auto;
                    //    }
                    //}

                    //if (firstTable.Rows.Count > 0 && firstTable.Rows[0].Cells.Count > 0)
                    //{
                    //    firstTable.Rows[0].Cells[0].PreferredWidth += DevExpress.Office.Utils.Units.DocumentsToMillimetersF(1);

                    //}
                }

                //richTextEditorUserControl.AutoFitTable();

                if (richTextEditorParameters.AutoFitTable)
                {
                    AutoFitTable();
                }
            }

            richEditControl.Document.EndUpdate();

            // Записываем метаданные таблицы
            EditorUtils.SetTableMetadata(this, htmlData, true);

            //if (firstTable != null)
            //    firstTable.PreferredWidthType = WidthType.Fixed;

            //richEditControl.Update();
            //richEditControl.Refresh();
        }

        private void CellProcessor(TableCell cell, int rowIndex, int cellIndex)
        {
            //if (rowindex == 0)
            //{
            //    cell.HeightType = HeightType.Exact;
            //    cell.Height = 500;
            //    cell.BackgroundColor = Color.BlanchedAlmond;
            //}

            // Заливка ячеек серым цветом
            var cellParagraph = richEditControl.Document.BeginUpdateParagraphs(cell.Range);

            if (cellParagraph.BackColor.GetValueOrDefault().ToArgb()
                .Equals(TransformService.HtmlUtils.CommonTableHeaderColor.ArgbValue))
            {
                //cell.HeightType = HeightType.Auto;
                //cell.PreferredWidthType = WidthType.Auto;
                //cell.Height = 500;
                cell.BackgroundColor = TransformService.HtmlUtils.CommonTableHeaderColor.Value;
                //cell.PreferredWidthType = WidthType.Fixed;
            }

            richEditControl.Document.EndUpdateParagraphs(cellParagraph);
        }

        public string GetHtmlContent(bool needActualColumnWidths = false) =>
            richEditControl.Document.GetHtmlContent(RichTextUtils.TextRangeType.All, GetTableMetadata(needActualColumnWidths),
                richEditControl.Options.Export.Html);

        public bool HasContent() => !richEditControl.Document.IsEmpty;

        public event EventHandler ContentChanged;

        #endregion

        #region ITableMetadataManager

        private TableMetadata _tableMetadata = new();

        public TableMetadata GetTableMetadata(bool needActualColumnWidths = false) =>
            new(barEditItemTableTitle.EditValue as string, _tableMetadata.OriginalColumnWidths,
                needActualColumnWidths ? GetActualColumnWidths() : null);

        private IEnumerable<int> GetActualColumnWidths()
        {
            var firstTable = richEditControl.Document.Tables.First;
            var dictColumnWidths = new SortedDictionary<int, int>();

            firstTable?.ForEachCell((cell, _, colIndex) =>
                {
                    var cellLayout = richEditControl.DocumentLayout.GetElement<LayoutTableCell>(cell.Range.Start);
                    if (cellLayout != null)
                    {
                        var cellWidth = cellLayout.Bounds.Width;

                        if (dictColumnWidths.TryGetValue(colIndex, out var dictWidth))
                        {
                            if (cellWidth < dictWidth)
                                dictColumnWidths[colIndex] = cellWidth;
                        }
                        else dictColumnWidths[colIndex] = cellWidth;
                    }
                }
            );

            //if (firstTable != null)
            //{
            //    foreach (var row in firstTable.Rows)
            //    {
            //        for (var colIndex = 0; colIndex < row.Cells.Count; colIndex++)
            //        {
            //            var cell = row.Cells[colIndex];
            //            var cellLayout = richEditControl.DocumentLayout.GetElement<LayoutTableCell>(cell.Range.Start);

            //            if (cellLayout != null)
            //            {
            //                var cellWidth = cellLayout.Bounds.Width;

            //                if (dictColumnWidths.TryGetValue(colIndex, out var dictWidth))
            //                {
            //                    if (cellWidth < dictWidth)
            //                        dictColumnWidths[colIndex] = cellWidth;
            //                }
            //                else dictColumnWidths[colIndex] = cellWidth;
            //            }
            //        }
            //    }
            //}

            return dictColumnWidths.Values.ToArray();
        }

        public void SetTableMetadata(TableMetadata tableMetadata)
        {
            _tableMetadata = tableMetadata != null ? tableMetadata.Clone() : new TableMetadata();
            barEditItemTableTitle.EditValue = tableMetadata?.Title;
        }

        #endregion

        #region IClipboardService

        public ClipboardFormat GetClipboardFormat()
        {
            return ClipboardFormat.All;
            //return copyToClipboardInHtmlFormatItem.Down ? ClipboardFormat.Html : ClipboardFormat.All;
        }

        #endregion
    }
}