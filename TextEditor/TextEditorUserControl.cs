using ActiproSoftware.Text;
using ActiproSoftware.Text.Languages.JavaScript.Implementation;
using ActiproSoftware.Text.Languages.Xml.Implementation;
using ActiproSoftware.UI.WinForms.Controls.SyntaxEditor;
using DevExpress.CodeParser;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;
using TextEditor.Editors;
using TextEditor.Editors.RichTextEditor;
using TextEditor.Editors.WorkbookEditor;
using TextEditor.TransformParameters;

namespace TextEditor
{
    public partial class TextEditorUserControl : XtraUserControl, IConfigurable
    {
        private const string JsonTransformParamsSectionName = "JsonTransformParameters";
        private const string HtmlTransformParamsSectionName = "HtmlTransformParameters";

        private WorkbookEditorUserControl _workbookEditor;

        public TextEditorUserControl()
        {
            InitializeComponent();

            // Задаем параметры для вкладок JSON и HTML редакторов
            tabControlData.AllowDrop = true;
            tabControlData.DragOver += TabControlDragOver;
            tabControlData.SelectedTabPage = tabPageJsonData;

            // Задаем пробелы вместо таба
            txtJson.Document.TabSize = 4;
            txtJson.Document.AutoConvertTabsToSpaces = true;

            txtJson.Document.Language.RegisterIndentProvider(new JsonIndentProvider());
            txtJson.Document.Language = new JsonSyntaxLanguage();

            txtHtml.Document.Language.RegisterIndentProvider(new XmlIndentProvider());
            txtHtml.Document.Language = new XmlSyntaxLanguage();

            // Настройка возможности перетаскивания файла в текстовые редакторы
            Utils.SetDragAndDropEventsHandlers(txtJson);
            Utils.SetDragAndDropEventsHandlers(txtHtml);

            // Для смены вкладок при перетаскивании
            layoutControl.DragOver += LayoutControlDragOver;

            //txtHtml.Document.TextChanged += HtmlDocument_TextChanged;
            //txtJson.Document.TextChanged += JsonDocument_TextChanged;

            txtJson.PasteDragDrop += TxtJson_PasteDragDrop;

            txtHtml.DocumentTextChanged += CodeChanged;
            txtJson.DocumentTextChanged += CodeChanged;

            RegisterContentChangedEventHandler(richTextEditorUserControl);

            // Убираем лишние рамки у контролов
            tabControlParameters.CustomDraw += TabbedControlGroupOnCustomDraw;

            AdjustControlsState();
            AdjustParametersTab();
        }

        private void RegisterContentChangedEventHandler(IEditorService editorService) =>
            editorService.ContentChanged += Editor_ContentChanged;

        private void UnRegisterContentChangedEventHandler(IEditorService editorService) =>
            editorService.ContentChanged -= Editor_ContentChanged;

        private void TxtJson_PasteDragDrop(object sender, PasteDragDropEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text) &&
                !e.Text.Contains(Environment.NewLine) /* && e.DataStore.TextKind == DataStoreTextKind.Default */ &&
                e.Text.Length > 1000)
                e.Text += Environment.NewLine;
        }

        /// <summary>
        /// Обработка события изменения содержимого редактора кода (JSON, HTML)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CodeChanged(object sender, EditorSnapshotChangedEventArgs e) => AdjustControlsState();

        /// <summary>
        /// Обработка перетаскивания файла для смены вкладок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutControlDragOver(object sender, DragEventArgs e)
        {
            if (layoutControl.CalcHitInfo(this.layoutControl.PointToClient(new Point(e.X, e.Y))) is TabbedGroupHitInfo
                hitInfo)
            {
                if (hitInfo.Item is TabbedControlGroup tcg && tcg.SelectedTabPageIndex != hitInfo.TabPageIndex)
                    tcg.SelectedTabPageIndex = hitInfo.TabPageIndex;
            }
        }

        #region Remove borders on Tabbed Control Group

        private void TabbedControlGroupOnCustomDraw(object sender, ItemCustomDrawEventArgs e)
        {
            e.DefaultDraw();
            var borderRect = ((DevExpress.XtraLayout.ViewInfo.TabbedGroupViewInfo)e.ViewInfo).BorderInfo.Bounds;
            var tabsRect = ((DevExpress.XtraLayout.ViewInfo.TabbedGroupViewInfo)e.ViewInfo).TabsCaptionArea;
            var topRect = borderRect with { Height = borderRect.Height - tabsRect.Height };
            var pen = new Pen(GetBackColor());
            var brush = pen.Brush;
            e.Cache.FillRectangle(brush, topRect);
            e.Cache.DrawLine(pen, new Point(topRect.X, topRect.Y + topRect.Height),
                new Point(topRect.X + topRect.Width, topRect.Y + topRect.Height));
        }

        private Color GetBackColor() => EditorsSkins.GetSkin(layoutControl.LookAndFeel.ActiveLookAndFeel)
            .TranslateColor(SystemColors.Control);

        #endregion

        private void Editor_ContentChanged(object sender, EventArgs e)
        {
            AdjustControlsState();
        }

        /*
        private void SetHtmlContentToEditor(string htmlData, bool applyTableStyle, bool autoFitTable)
        {
            var richEditControl = richTextEditorUserControl.RichEditControl;

            // Получаем заголовок таблицы
            var tableMetadata = TableMetadataUtils.GetFirstTableMetadata(htmlData);

            richEditControl.Document.BeginUpdate();
            richEditControl.Document.Delete(richEditControl.Document.Range);

            using (var server = new RichEditDocumentServer())
            {
                server.HtmlText = htmlData;

                if (applyTableStyle)
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

                var firstTable = richTextEditorUserControl.RichEditControl.Document.Tables.First;

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

                if (autoFitTable)
                {
                    richTextEditorUserControl.AutoFitTable();
                }
            }

            // Записываем метаданные таблицы
            var curTableMetadata = richTextEditorUserControl.GetTableMetadata();

            if (string.IsNullOrEmpty(tableMetadata.Title))
                tableMetadata.Title =
                    curTableMetadata
                        .Title; // Сохраняем прежнее наименование таблицы при отсутствии наименования в новой таблице

            richTextEditorUserControl.SetTableMetadata(tableMetadata);

            richEditControl.Document.EndUpdate();

            //if (firstTable != null)
            //    firstTable.PreferredWidthType = WidthType.Fixed;

            //richEditControl.Update();
            //richEditControl.Refresh();
        }
        */

        /// <summary>
        /// Формирование содержимого редактора на основе HTML
        /// </summary>
        private void SetHtmlContentToEditor(string htmlData, object parameters)
        {
            var editorService = GetActiveEditorService();
            IEditorParameters editorParameters = null;

            switch (editorService)
            {
                case null:
                    return;
                case RichTextEditorUserControl:
                    editorParameters = GetRichTextEditorParameters();
                    break;
                case WorkbookEditorUserControl:
                    editorParameters = GetWorkbookEditorParameters(parameters);
                    break;
            }

            editorService.SetHtmlContent(htmlData, editorParameters);
        }

        private IEditorService GetActiveEditorService() =>
            tabControlEditors.SelectedTabPage == tabPageRichTextEditor
                ? richTextEditorUserControl
                : _workbookEditor;

        //private void CellProcessor(TableCell cell, int rowIndex, int cellIndex)
        //{
        //    //if (rowindex == 0)
        //    //{
        //    //    cell.HeightType = HeightType.Exact;
        //    //    cell.Height = 500;
        //    //    cell.BackgroundColor = Color.BlanchedAlmond;
        //    //}

        //    // Заливка ячеек серым цветом
        //    var cellParagraph = richTextEditorUserControl.RichEditControl.Document.BeginUpdateParagraphs(cell.Range);

        //    if (cellParagraph.BackColor.GetValueOrDefault().ToArgb()
        //        .Equals(TransformService.HtmlUtils.CommonTableHeaderColor.ArgbValue))
        //    {
        //        //cell.HeightType = HeightType.Auto;
        //        //cell.PreferredWidthType = WidthType.Auto;
        //        //cell.Height = 500;
        //        cell.BackgroundColor = TransformService.HtmlUtils.CommonTableHeaderColor.Value;
        //        //cell.PreferredWidthType = WidthType.Fixed;
        //    }

        //    richTextEditorUserControl.RichEditControl.Document.EndUpdateParagraphs(cellParagraph);
        //}

        //private void SetHtmlContentToEditor(string html)
        //{
        //    //string text = "<b>SOME SAMPLE TEXT</b>";
        //    //AddHeaderToDocument(server.Document, text);
        //    //AddFooterToDocument(server.Document, text);
        //    richTextEditorUserControl.RichEdit.Document.BeginUpdate();
        //    richTextEditorUserControl.RichEdit.Document.Delete(richTextEditorUserControl.RichEdit.Document.Range);

        //    using (RichEditDocumentServer server = new RichEditDocumentServer())
        //    {
        //        server.HtmlText = html;

        //        richTextEditorUserControl.RichEdit.Document.InsertDocumentContent(
        //            richTextEditorUserControl.RichEdit.Document.Range.Start, server.Document.Range);
        //    }

        //    richTextEditorUserControl.RichEdit.Document.EndUpdate();
        //}

        private void AdjustControlsState()
        {
            cmdHtml2Editor.Enabled = cmdFormatHtml.Enabled = txtHtml.Document.CurrentSnapshot.HasContent;

            cmdCopyJson.Enabled = cmdJson2Editor.Enabled =
                cmdFormatJson.Enabled = txtJson.Document.CurrentSnapshot.HasContent;

            cmdEditor2Html.Enabled = cmdEditor2Json.Enabled = GetActiveEditorService()?.HasContent() ?? false;
        }

        private void AdjustParametersTab()
        {
            tabControlParameters.ShowTabHeader = DefaultBoolean.False;

            if (tabControlData.SelectedTabPage == tabPageJsonData)
            {
                tabPageHtmlParameters.Visibility = LayoutVisibility.Never;
                tabPageJsonParameters.Visibility = LayoutVisibility.OnlyInRuntime;

                tabControlParameters.SelectedTabPage = tabPageJsonParameters;
            }
            else if (tabControlData.SelectedTabPage == tabPageHtmlData)
            {
                tabPageJsonParameters.Visibility = LayoutVisibility.Never;
                tabPageHtmlParameters.Visibility = LayoutVisibility.OnlyInRuntime;

                tabControlParameters.SelectedTabPage = tabPageHtmlParameters;
            }
        }


        //void AddHeaderToDocument(DevExpress.XtraRichEdit.API.Native.Document document, string htmlText)
        //{
        //    SubDocument doc = document.Sections[0].BeginUpdateHeader();
        //    doc.AppendHtmlText(htmlText);
        //    document.Sections[0].EndUpdateHeader(doc);
        //}

        //void AddFooterToDocument(DevExpress.XtraRichEdit.API.Native.Document document, string htmlText)
        //{
        //    SubDocument doc = document.Sections[0].BeginUpdateFooter();
        //    doc.AppendHtmlText(htmlText);
        //    document.Sections[0].EndUpdateFooter(doc);
        //}

        private void CmdCopyJsonClick(object sender, EventArgs e)
        {
            Utils.CopyJsonToClipBoard(txtJson.Text);
            //TestHtmlContent(File.ReadAllText("..\\..\\Data\\table.html"));
        }

        private void cmdFormatJson_Click(object sender, EventArgs e)
        {
            try
            {
                Utils.ShowProgressForm();
                txtJson.Format();
            }
            catch (Exception exception)
            {
                Utils.ProcessException(exception);
            }
            finally
            {
                Utils.CloseProgressForm();
            }
        }

        private void cmdFormatHtml_Click(object sender, EventArgs e)
        {
            try
            {
                Utils.ShowProgressForm();
                txtHtml.Format();
            }
            catch (Exception exception)
            {
                Utils.ProcessException(exception);
            }
            finally
            {
                Utils.CloseProgressForm();
            }
        }

        private void CmdJson2EditorClick(object sender, EventArgs e)
        {
            try
            {
                Utils.ShowProgressForm();
                CreateEditorContentByJsonData(txtJson.Text);
            }
            catch (Exception exception)
            {
                Utils.ProcessException(exception);
            }
            finally
            {
                Utils.CloseProgressForm();
            }
        }

        private void CreateEditorContentByJsonData(string jsonData,
            JsonTransformViewParameters jsonTransformViewParams = null)
        {
            jsonTransformViewParams ??= jsonTransformParamsUserControl.GetParameters<JsonTransformViewParameters>();

            //var htmlTransformParams = jsonTransformViewParams.ConvertToHtmlTransformParameters();

            var htmlData =
                Utils.TransformJson2Html(jsonData, jsonTransformViewParams.GetJson2HtmlTransformParameters());

            SetHtmlContentToEditor(htmlData, jsonTransformViewParams);
        }

        private void CreateEditorContentByHtmlData(string htmlData,
            HtmlTransformViewParameters htmlTransformViewParams = null)
        {
            htmlTransformViewParams ??= htmlTransformParamsUserControl.GetParameters<HtmlTransformViewParameters>();

            if (htmlTransformViewParams.TransformViaJson)
            {
                var jsonTransformViewParams =
                    jsonTransformParamsUserControl.GetClonedParameters<JsonTransformViewParameters>();

                //var jsonTransformParams = jsonTransformViewParams.GetHtml2JsonTransformParameters();

                if (jsonTransformViewParams != null)
                {
                    jsonTransformViewParams.NeedDoubleTransformation = false;
                    jsonTransformViewParams.CopyJsonToClipboardAfterTransformation = false;

                    //var jsonTransformParams = new JsonTransformViewParameters
                    //{
                    //    NeedDoubleTransformation = false,
                    //    CopyJsonToClipboardAfterTransformation = false
                    //};

                    // HTML -> JSON
                    var jsonData = Utils.TransformHtml2Json(htmlData, jsonTransformViewParams);

                    // JSON -> HTML
                    htmlData = Utils.TransformJson2Html(jsonData,
                        htmlTransformViewParams.ConvertToJson2HtmlTransformParameters());
                }
            }
            else
            {
                // HTML -> HTML
                var html2HtmlTransformParams = htmlTransformViewParams.ConvertToHtml2HtmlTransformParameters();
                htmlData = Utils.TransformHtml2Html(htmlData, html2HtmlTransformParams);
            }

            SetHtmlContentToEditor(htmlData, htmlTransformViewParams);
        }

        private WorkbookEditorParameters GetWorkbookEditorParameters(object parameters) =>
            parameters is ISupportWorkbookEditorParameters workbookEditorParameters
                ? workbookEditorParameters.GetWorkbookEditorParameters()
                : null;

        private RichTextEditorParameters GetRichTextEditorParameters() => new(true, false);

        private void CmdEditor2JsonClick(object sender, EventArgs e)
        {
            try
            {
                Utils.ShowProgressForm();
                TransformFromEditorToJson();
            }
            catch (Exception exception)
            {
                Utils.ProcessException(exception);
            }
            finally
            {
                Utils.CloseProgressForm();
            }
        }

        private void TransformFromEditorToJson()
        {
            var editorService = GetActiveEditorService();

            var jsonTransformViewParams =
                jsonTransformParamsUserControl.GetClonedParameters<JsonTransformViewParameters>();

            if (jsonTransformViewParams == null)
                return;

            var htmlData = editorService?.GetHtmlContent(!jsonTransformViewParams.KeepOriginalColumnWidths);

            if (htmlData == null)
                return;

            //var htmlData = richTextEditorUserControl.RichEditControl.Document.GetHtmlContent(
            //    RichTextUtils.TextRangeType.All,
            //    richTextEditorUserControl.GetTableMetadata(),
            //    richTextEditorUserControl.RichEditControl.Options.Export.Html);


            jsonTransformViewParams.NeedDoubleTransformation = false;
            var jsonData = Utils.TransformHtml2Json(htmlData, jsonTransformViewParams);

            InsertNewJsonData(jsonData, false);
        }

        public void InsertNewJsonData(string jsonData, bool updateTable,
            JsonTransformViewParameters transformParams = null)
        {
            txtJson.Document.SetText(TextChangeTypes.ReplaceAll, jsonData); // Text = jsonData;

            txtJson.ActiveView.Selection.CaretPosition = new TextPosition(0, 0);
            //txtJson.Format();

            if (updateTable)
                CreateEditorContentByJsonData(jsonData, transformParams);

            tabControlData.SelectedTabPage = tabPageJsonData;
        }

        public void InsertNewHtmlData(string htmlData, bool createEditorContent,
            HtmlTransformViewParameters transformParams = null)
        {
            txtHtml.Document.SetText(TextChangeTypes.ReplaceAll, htmlData); // Text = htmlData;
            txtHtml.ActiveView.Selection.CaretPosition = new TextPosition(0, 0);

            if (createEditorContent)
                CreateEditorContentByHtmlData(htmlData, transformParams);

            tabControlData.SelectedTabPage = tabPageHtmlData;
        }

        //private void tcgEditors_SelectedPageChanged(object sender, LayoutTabPageChangedEventArgs e)
        //{
        //    AdjustParametersTab();
        //}

        private void cmdHtml2Editor_Click(object sender, EventArgs e)
        {
            try
            {
                Utils.ShowProgressForm();
                CreateEditorContentByHtmlData(txtHtml.Text);
            }
            catch (Exception exception)
            {
                Utils.ProcessException(exception);
            }
            finally
            {
                Utils.CloseProgressForm();
            }
        }

        private void CmdEditor2HtmlClick(object sender, EventArgs e)
        {
            try
            {
                Utils.ShowProgressForm();

                var editorService = GetActiveEditorService();

                if (editorService == null)
                    return;

                var htmlData = editorService.GetHtmlContent();
                InsertNewHtmlData(htmlData, false);

                //InsertNewHtmlData(
                //    richTextEditorUserControl.RichEditControl.Document.GetHtmlContent(
                //        RichTextUtils.TextRangeType.All,
                //        richTextEditorUserControl.GetTableMetadata(),
                //        richTextEditorUserControl.RichEditControl.Options.Export.Html),
                //    false);

            }
            catch (Exception exception)
            {
                Utils.ProcessException(exception);
            }
            finally
            {
                Utils.CloseProgressForm();
            }
        }

        #region IConfigurable

        public void LoadParameters(bool isWorkbookEditorEnable)
        {
            var jsonTransformViewParams =
                Utils.LoadParameters<JsonTransformViewParameters>(JsonTransformParamsSectionName);
            SetWorkbookEditorParametersVisibility(jsonTransformViewParams, isWorkbookEditorEnable);
            jsonTransformParamsUserControl.SetParameters(jsonTransformViewParams);

            var htmlTransformViewParams =
                Utils.LoadParameters<HtmlTransformViewParameters>(HtmlTransformParamsSectionName);
            SetWorkbookEditorParametersVisibility(htmlTransformViewParams, isWorkbookEditorEnable);
            htmlTransformParamsUserControl.SetParameters(htmlTransformViewParams);
        }

        private void SetWorkbookEditorParametersVisibility(object parameters, bool isWorkbookEditorEnable)
        {
            if (parameters is ISupportWorkbookEditorParameters workbookEditorParameters)
                workbookEditorParameters.SetWorkbookEditorParametersVisibility(isWorkbookEditorEnable);
        }

        public void SaveParameters()
        {
            var html2JsonTransformViewParams =
                jsonTransformParamsUserControl.GetParameters() as JsonTransformViewParameters;
            Utils.SaveParameters(html2JsonTransformViewParams, JsonTransformParamsSectionName);

            var json2HtmlTransformParams =
                htmlTransformParamsUserControl.GetParameters() as HtmlTransformViewParameters;
            Utils.SaveParameters(json2HtmlTransformParams, HtmlTransformParamsSectionName);
        }

        #endregion

        public void SetWorkbookEditorVisibility(bool visible)
        {
            if (visible) // Необходимо отобразить вкладку редактора ячеек
            {
                _workbookEditor = new WorkbookEditorUserControl();

                tabPageWorkbookEditor.Controls.Add(_workbookEditor);
                _workbookEditor.Dock = DockStyle.Fill;

                RegisterContentChangedEventHandler(_workbookEditor);
            }
            else // Вкладка редактора ячеек не отображается
            {
                tabControlEditors.TabPages.Remove(tabPageWorkbookEditor);

                if (_workbookEditor != null)
                    UnRegisterContentChangedEventHandler(_workbookEditor);
            }

            if (tabControlEditors.TabPages.Count == 0)
                return;

            if (tabControlEditors.TabPages.Count == 1)
            {
                tabControlEditors.ShowTabHeader = DefaultBoolean.False;
                //tabControlEditors.BorderStyle = BorderStyles.NoBorder;
                //tabControlEditors.BorderStylePage = BorderStyles.NoBorder;
                //tabControlEditors.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
                //tabControlEditors.LookAndFeel.UseDefaultLookAndFeel = false;
            }
            else if (tabControlEditors.TabPages.Count > 1)
            {
                tabControlEditors.AllowDrop = true;
                tabControlEditors.DragOver += TabControlDragOver;
                tabControlEditors.SelectedPageChanged += (_, _) => AdjustControlsState();
                tabControlEditors.TabPages[0].Select();
            }
        }

        private void TabControlDragOver(object sender, DragEventArgs e)
        {
            var tabControl = (XtraTabControl)sender;

            var pt = tabControl.PointToClient(new Point(e.X, e.Y));
            var info = tabControl.CalcHitInfo(pt);

            if (info.HitTest == XtraTabHitTest.PageHeader) // Проверяем, что мы находимся над заголовком вкладки
            {
                var tabPage = info.Page;

                if (tabPage != null && tabControl.SelectedTabPage != tabPage && tabPage.Enabled)
                    tabControl.SelectedTabPage = tabPage;
            }
        }

        private void tabControlData_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            AdjustParametersTab();
        }
    }
}