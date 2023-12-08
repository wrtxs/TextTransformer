using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Layout;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Services;
using TextEditor.RichTextEdit.CustomCommands;
using TransfromService.RichText;
using Table = DevExpress.XtraRichEdit.API.Native.Table;
using TableLayoutType = DevExpress.XtraRichEdit.API.Native.TableLayoutType;

namespace TextEditor.RichTextEdit
{
    public partial class RtfDocUserControl : XtraUserControl, ITableTitleService
    {
        private DevExpress.XtraBars.BarCheckItem _copyFormatItem;
        private DevExpress.XtraBars.PopupMenu _popupMenu;

        bool _isZoomChanging;
        int _pageCount = 1;
        int _currentPage = 1;

        public RtfDocUserControl()
        {
            //OfficeCharts.Instance.ActivateWinFormsCharts();
            InitializeComponent();

            Load += RtfDocUserControl_Load;

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

            richEditControl.Initialize(this);

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
            AdjsutCommandsBar();

            // Создаем строку состояния
            CreateStatusBar();

            // Переопределяем стандартные команды (копирование в буфер обмена)
            RedefineStandartCommands();

            ribbonControl.SelectedPage = homeRibbonPage1;

            ribbonControl.ForceInitialize();
        }

        private void AdjsutCommandsBar()
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

        private void RedefineStandartCommands()
        {
            var commandFactory = new CustomRichEditCommandFactoryService(richEditControl,
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

        public void SetSimpleViewType()
        {
            richEditControl.ActiveViewType = RichEditViewType.Simple;
        }

        public RichEditControl RichEditControl => richEditControl;

        int PageCount
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

        int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (_currentPage == value)
                    return;
                _currentPage = value;
                OnPagesInfoChanged();
            }
        }

        #region Status bar

        private RibbonStatusBar _ribbonStatusBar;
        private DevExpress.XtraBars.BarEditItem _barEditItemTableTitle;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit _txtTableTitle;
        private DevExpress.XtraBars.BarStaticItem _pagesBarItem;
        private DevExpress.XtraBars.BarEditItem _zoomBarEditItem;
        private DevExpress.XtraEditors.Repository.RepositoryItemZoomTrackBar _repositoryItemZoomTrackBar1;

        private void CreateStatusBar()
        {
            // Создаем панель статуса
            _ribbonStatusBar = new RibbonStatusBar();
            ribbonControl.StatusBar = _ribbonStatusBar;

            _ribbonStatusBar.Location = new Point(0, 445);
            _ribbonStatusBar.Name = "ribbonStatusBar1";
            _ribbonStatusBar.Ribbon = ribbonControl;
            _ribbonStatusBar.Size = new Size(1014, 27);
            Controls.Add(_ribbonStatusBar);

            // Добавляем дополнительные элементы в строку состояния
            AppendStatusBarItems();
        }

        void AppendStatusBarItems()
        {
            // txtTableTitle
            _txtTableTitle = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            ((System.ComponentModel.ISupportInitialize)_txtTableTitle).BeginInit();
            ribbonControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[]
            {
                repositoryItemFontEditRichEdit1, repositoryItemRichEditFontSizeEdit1, displayForReviewModeComboBox1,
                repositoryItemBorderLineStyle1, repositoryItemBorderLineWeight1,
                repositoryItemFloatingObjectOutlineWeight1, _txtTableTitle
            });

            _txtTableTitle.AutoHeight = false;
            _txtTableTitle.Name = "txtTableTitle";
            ((System.ComponentModel.ISupportInitialize)_txtTableTitle).EndInit();

            // barEditItemTableTitle
            _barEditItemTableTitle = new DevExpress.XtraBars.BarEditItem();
            ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[] { _barEditItemTableTitle });

            _barEditItemTableTitle.Caption = @"Заголовок таблицы";
            _barEditItemTableTitle.Edit = _txtTableTitle;
            _barEditItemTableTitle.EditWidth = 360;
            _barEditItemTableTitle.Id = 304;
            _barEditItemTableTitle.Name = "barEditItemTableTitle";
            _ribbonStatusBar.ItemLinks.Add(_barEditItemTableTitle, true);

            _pagesBarItem = new DevExpress.XtraBars.BarStaticItem();
            _zoomBarEditItem = new DevExpress.XtraBars.BarEditItem();
            _repositoryItemZoomTrackBar1 = new DevExpress.XtraEditors.Repository.RepositoryItemZoomTrackBar();

            ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[]
            {
                _pagesBarItem,
                _zoomBarEditItem,
            });

            ribbonControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[]
            {
                _repositoryItemZoomTrackBar1,
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
            _zoomBarEditItem.Edit = _repositoryItemZoomTrackBar1;
            _zoomBarEditItem.EditValue = 100;
            _zoomBarEditItem.EditWidth = 150;
            _zoomBarEditItem.Id = 245;
            _zoomBarEditItem.Name = "zoomBarEditItem";
            _zoomBarEditItem.EditValueChanged += this.zoomBarEditItem_EditValueChanged;

            // 
            // repositoryItemZoomTrackBar1
            // 
            ((System.ComponentModel.ISupportInitialize)(_repositoryItemZoomTrackBar1)).BeginInit();

            _repositoryItemZoomTrackBar1.AllowUseMiddleValue = true;
            _repositoryItemZoomTrackBar1.LargeChange = 50;
            _repositoryItemZoomTrackBar1.Maximum = 500;
            _repositoryItemZoomTrackBar1.Middle = 100;
            _repositoryItemZoomTrackBar1.Minimum = 10;
            _repositoryItemZoomTrackBar1.Name = "repositoryItemZoomTrackBar1";
            _repositoryItemZoomTrackBar1.SmallChange = 10;
            _repositoryItemZoomTrackBar1.SnapToMiddle = 2;

            ((System.ComponentModel.ISupportInitialize)(_repositoryItemZoomTrackBar1)).EndInit();

            _ribbonStatusBar.ItemLinks.Add(_pagesBarItem, true);
            _ribbonStatusBar.ItemLinks.Add(_zoomBarEditItem);

            //this.ribbonStatusBar1.Ribbon = ribbonControl;
        }

        #endregion

        void RtfDocUserControl_Load(object sender, EventArgs e)
        {
            RichEditControl.DocumentLayout.DocumentFormatted += DocumentLayout_DocumentFormatted;
            OnPagesInfoChanged();
            //RichEdit.HyphenationDictionaries.Add(new OpenOfficeHyphenationDictionary(DemoUtils.GetRelativePath("hyph_en_US.dic"), new System.Globalization.CultureInfo("en-US")));
            //LoadDocument("FirstLook.docx");
        }

        void DocumentLayout_DocumentFormatted(object sender, EventArgs e)
        {
            BeginInvoke(() => { PageCount = RichEditControl.DocumentLayout.GetPageCount(); });
        }

        void zoomBarEditItem_EditValueChanged(object sender, EventArgs e)
        {
            if (this._isZoomChanging)
                return;
            int value = Convert.ToInt32(_zoomBarEditItem.EditValue);
            this._isZoomChanging = true;
            try
            {
                RichEditControl.ActiveView.ZoomFactor = value / 100f;
                _zoomBarEditItem.Caption = $@"{value}%";
            }
            finally
            {
                this._isZoomChanging = false;
            }
        }

        void richEditControl_ZoomChanged(object sender, EventArgs e)
        {
            if (this._isZoomChanging)
                return;
            int value = (int)Math.Round(RichEditControl.ActiveView.ZoomFactor * 100);
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

        void OnPagesInfoChanged()
        {
            _pagesBarItem.Caption = $@"Страница {CurrentPage} из {PageCount}";
        }

        void richEditControl_VisiblePagesChanged(object sender, EventArgs e)
        {
            CurrentPage = RichEditControl.ActiveView.GetVisiblePageLayoutInfos()[0].PageIndex + 1;
        }

        void richEditControl_SelectionChanged(object sender, EventArgs e)
        {
            RangedLayoutElement element =
                RichEditControl.DocumentLayout.GetElement<RangedLayoutElement>(RichEditControl.Document.CaretPosition);
            if (element != null)
                CurrentPage = RichEditControl.DocumentLayout.GetPageIndex(element) + 1;
        }

        void richEditControl_InvalidFormatException(object sender, RichEditInvalidFormatExceptionEventArgs e)
        {
            XtraMessageBox.Show(string.Format(
                    "Невозможно открыть файл '{0}' поскольку файл имеет недоступимый формат или расширение.\n" +
                    "Удостоверьтесь, что файл не поврежден и расширение файла соответсвует его формату.",
                    RichEditControl.Options.DocumentSaveOptions.CurrentFileName),
                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        void richEditControl_DocumentClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (RichEditControl.Modified)
            {
                var currentFileName = RichEditControl.Options.DocumentSaveOptions.CurrentFileName;
                var message = !string.IsNullOrEmpty(currentFileName)
                    ? string.Format("Вы хотите сохранить изменения, сделанные для '{0}'?", currentFileName)
                    : "Вы хотите сохранить изменения?";
                DialogResult result = XtraMessageBox.Show(message, "Сохранение", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                    e.Cancel = !RichEditControl.SaveDocument();
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

        public string GetTableTitle() => (_barEditItemTableTitle.EditValue as string);

        public void SetTableTitle(string tableTitle) =>
            _barEditItemTableTitle.EditValue = tableTitle;
    }
}