using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Layout;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Export.Html;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Services;
using TableEditor.RichTextEdit.CustomCommands;
using TransfromService.RichText;
using Table = DevExpress.XtraRichEdit.API.Native.Table;
using TableLayoutType = DevExpress.XtraRichEdit.API.Native.TableLayoutType;

namespace TableEditor.RichTextEdit
{
    public partial class RtfDocUserControl : XtraUserControl
    {
        bool _isZoomChanging;
        int _pageCount = 1;
        int _currentPage = 1;

        public RtfDocUserControl()
        {
            //OfficeCharts.Instance.ActivateWinFormsCharts();

            InitializeComponent();

            new RichEditControlExceptionHandler(richEditControl).Install();

            this.Load += RtfDocUserControl_Load;
            richEditControl.ZoomChanged += richEditControl_ZoomChanged;
            richEditControl.ZoomChanged += richEditControl_ZoomChanged;
            richEditControl.SelectionChanged += richEditControl_SelectionChanged;
            richEditControl.InvalidFormatException += richEditControl_InvalidFormatException;
            richEditControl.VisiblePagesChanged += richEditControl_VisiblePagesChanged;
            richEditControl.DocumentClosing += richEditControl_DocumentClosing;

            //var ribbonControl = richEditControl.CreateRibbon();
            //this.Controls.Add(ribbonControl);
            AppendCustomRibbonItems(ribbonControl);

            SetLandscapePage();

            //RichEditControlCompatibility.EnableMSWordCompatibleFieldParser = true;
            //RichEditControlCompatibility.UseThemeFonts = false;

            var clipboardFormats = richEditControl.Options.ClipboardFormats;

            clipboardFormats.PlainText = RichEditClipboardMode.Enabled;
            clipboardFormats.Rtf = RichEditClipboardMode.Enabled;
            clipboardFormats.Html = RichEditClipboardMode.Enabled;

            richEditControl.Options.Export.Html.SetCommonExportOptions();
            SetCustomCommands();

            //richEditControl.Options.DocumentCapabilities.InlinePictures = DocumentCapability.Enabled;
            //clipboardRibbonPageGroup1.ItemLinks.Remove(pasteSpecialItem1);

            ribbonControl.SelectedPage = homeRibbonPage1;
        }

        private void SetCustomCommands()
        {
            var commandFactory = new CustomRichEditCommandFactoryService(richEditControl,
                richEditControl.GetService<IRichEditCommandFactoryService>());
            richEditControl.RemoveService(typeof(IRichEditCommandFactoryService));
            richEditControl.AddService(typeof(IRichEditCommandFactoryService), commandFactory);
        }

        private void SetLandscapePage()
        {
            richEditControl.Options.DocumentCapabilities.Undo = DocumentCapability.Disabled;
            richEditControl.Document.Sections[0].Page.Landscape = true;
            richEditControl.Options.DocumentCapabilities.Undo = DocumentCapability.Enabled;
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
            table.ForEachRow((row, index) => { row.HeightType = HeightType.Auto; });
            table.ForEachCell(((cell, rowIndex, cellIndex) => { cell.PreferredWidthType = WidthType.Auto; }));

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

        public DevExpress.XtraRichEdit.RichEditControl RichEditControl => richEditControl;

        int PageCount
        {
            get { return _pageCount; }
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

        #region AppendCustomRibbonItems
        private DevExpress.XtraBars.BarStaticItem pagesBarItem;
        private DevExpress.XtraBars.BarEditItem zoomBarEditItem;
        private DevExpress.XtraEditors.Repository.RepositoryItemZoomTrackBar repositoryItemZoomTrackBar1;

        void AppendCustomRibbonItems(RibbonControl ribbonControl1)
        {
            this.pagesBarItem = new DevExpress.XtraBars.BarStaticItem();
            this.zoomBarEditItem = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemZoomTrackBar1 = new DevExpress.XtraEditors.Repository.RepositoryItemZoomTrackBar();

            ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
                this.pagesBarItem,
                this.zoomBarEditItem,
            });

            ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
                this.repositoryItemZoomTrackBar1,
            });

            // 
            // pagesBarItem
            // 
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            this.pagesBarItem.Id = 246;
            this.pagesBarItem.Name = "pagesBarItem";
            toolTipItem1.Text = "Номер страницы в документе.";
            superToolTip1.Items.Add(toolTipItem1);
            this.pagesBarItem.SuperTip = superToolTip1;

            // 
            // zoomBarEditItem
            // 
            this.zoomBarEditItem.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.zoomBarEditItem.Caption = "100%";
            this.zoomBarEditItem.Edit = this.repositoryItemZoomTrackBar1;
            this.zoomBarEditItem.EditValue = 100;
            this.zoomBarEditItem.EditWidth = 150;
            this.zoomBarEditItem.Id = 245;
            this.zoomBarEditItem.Name = "zoomBarEditItem";
            this.zoomBarEditItem.EditValueChanged += this.zoomBarEditItem_EditValueChanged;


            // 
            // repositoryItemZoomTrackBar1
            // 
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemZoomTrackBar1)).BeginInit();

            this.repositoryItemZoomTrackBar1.AllowUseMiddleValue = true;
            this.repositoryItemZoomTrackBar1.LargeChange = 50;
            this.repositoryItemZoomTrackBar1.Maximum = 500;
            this.repositoryItemZoomTrackBar1.Middle = 100;
            this.repositoryItemZoomTrackBar1.Minimum = 10;
            this.repositoryItemZoomTrackBar1.Name = "repositoryItemZoomTrackBar1";
            this.repositoryItemZoomTrackBar1.SmallChange = 10;
            this.repositoryItemZoomTrackBar1.SnapToMiddle = 2;

            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemZoomTrackBar1)).EndInit();

            this.ribbonStatusBar1.ItemLinks.Add(this.pagesBarItem);
            this.ribbonStatusBar1.ItemLinks.Add(this.zoomBarEditItem);

            this.ribbonStatusBar1.Ribbon = ribbonControl1;

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
            BeginInvoke(new Action(() =>
            {
                PageCount = RichEditControl.DocumentLayout.GetPageCount();
            }));
        }
        void zoomBarEditItem_EditValueChanged(object sender, System.EventArgs e)
        {
            if (this._isZoomChanging)
                return;
            int value = Convert.ToInt32(zoomBarEditItem.EditValue);
            this._isZoomChanging = true;
            try
            {
                RichEditControl.ActiveView.ZoomFactor = value / 100f;
                zoomBarEditItem.Caption = String.Format("{0}%", value);
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
                zoomBarEditItem.EditValue = value;
                zoomBarEditItem.Caption = String.Format("{0}%", value);
            }
            finally
            {
                this._isZoomChanging = false;
            }
        }
        void OnPagesInfoChanged()
        {
            pagesBarItem.Caption = String.Format("Страница {0} из {1}", CurrentPage, PageCount);
        }

        void richEditControl_VisiblePagesChanged(object sender, EventArgs e)
        {
            CurrentPage = RichEditControl.ActiveView.GetVisiblePageLayoutInfos()[0].PageIndex + 1;
        }
        void richEditControl_SelectionChanged(object sender, EventArgs e)
        {
            RangedLayoutElement element = RichEditControl.DocumentLayout.GetElement<RangedLayoutElement>(RichEditControl.Document.CaretPosition);
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
            if (copyFormatItem.Checked)
            {
                SaveSelectedRange();
                richEditControl.FormatCalculatorEnabled = true;
            }
            else
            {
                richEditControl.FormatCalculatorEnabled = false;
            }
        }

        DocumentRange sourceSelectedRange;

        private void SaveSelectedRange()
        {
            var selection = richEditControl.Document.Selection;
            var subDocument = selection.BeginUpdateDocument();
            sourceSelectedRange = subDocument.CreateRange(selection.Start, richEditControl.Document.Selection.Length);
            selection.EndUpdateDocument(subDocument);
        }

        private void richEditControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (copyFormatItem.Checked)
            {
                ApplyFormatToSelectedText();
                copyFormatItem.Checked = false;
            }
        }

        private void ApplyFormatToSelectedText()
        {
            var targetSelectedRange = richEditControl.Document.Selection;

            richEditControl.BeginUpdate();
            var targetSubDocument = targetSelectedRange.BeginUpdateDocument();
            var subDocument = sourceSelectedRange.BeginUpdateDocument();

            var targetCharactersProperties = targetSubDocument.BeginUpdateCharacters(targetSelectedRange);
            var sourceCharactersProperties = subDocument.BeginUpdateCharacters(sourceSelectedRange);
            targetCharactersProperties.Assign(sourceCharactersProperties);
            subDocument.EndUpdateCharacters(sourceCharactersProperties);
            targetSubDocument.EndUpdateCharacters(targetCharactersProperties);

            var targetParagraphProperties = targetSubDocument.BeginUpdateParagraphs(targetSelectedRange);
            var sourceParagraphProperties = subDocument.BeginUpdateParagraphs(sourceSelectedRange);
            targetParagraphProperties.Assign(sourceParagraphProperties);
            subDocument.EndUpdateParagraphs(sourceParagraphProperties);
            targetSubDocument.EndUpdateParagraphs(targetParagraphProperties);

            sourceSelectedRange.EndUpdateDocument(subDocument);
            targetSelectedRange.EndUpdateDocument(targetSubDocument);
            richEditControl.EndUpdate();
        }

        #endregion
    }
}