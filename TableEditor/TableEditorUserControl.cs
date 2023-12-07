using ActiproSoftware.Text;
using ActiproSoftware.Text.Languages.JavaScript.Implementation;
using ActiproSoftware.Text.Languages.Xml.Implementation;
using ActiproSoftware.UI.WinForms.Controls.SyntaxEditor;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using TableEditor.TransformParameters;
using TransfromService;
using TransfromService.RichText;

namespace TableEditor
{
    public partial class TableEditorUserControl : XtraUserControl, ISupportParameters
    {
        private const string Html2JsonParamsSectionName = "Html2JsonTableEditorParameters";
        private const string Json2HtmlParamsSectionName = "Json2HtmlTableEditorParameters";

        public TableEditorUserControl()
        {
            InitializeComponent();
            //rtfDocUserControl.InsertEmptyTable();
            txtHtml.Document.Language.RegisterIndentProvider(new XmlIndentProvider());
            txtHtml.Document.Language = new XmlSyntaxLanguage();

            txtJson.Document.Language.RegisterIndentProvider(new JsonIndentProvider());
            txtJson.Document.Language = new JsonSyntaxLanguage();

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

            rtfDocUserControl.RichEditControl.ContentChanged += RichEdit_ContentChanged;

            // Убираем ненужные рамки у контролов
            tcgParameters.CustomDraw += TabbedControlGroupOnCustomDraw;

            tcgEditors.SelectedTabPage = lcgJsonEditor;

            AdjustControlsState();
            AdjustParametersTab();

            //rtfDocUserControl.RichEdit.
        }

        private void TxtJson_PasteDragDrop(object sender, PasteDragDropEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text) &&
                !e.Text.Contains(Environment.NewLine) /* && e.DataStore.TextKind == DataStoreTextKind.Default */ &&
                e.Text.Length > 1000)
                e.Text += Environment.NewLine;
        }

        private void CodeChanged(object sender, EditorSnapshotChangedEventArgs e)
        {
            AdjustControlsState();
        }

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

        private void RichEdit_ContentChanged(object sender, EventArgs e)
        {
            AdjustControlsState();
        }

        /// <summary>
        /// Формирование содержимого редактора на основе HTML
        /// </summary>
        /// <param name="htmlData"></param>
        /// <param name="applyTableStyle"></param>
        /// <param name="autoFitTable"></param>
        private void SetHtmlContentToEditor(string htmlData, bool applyTableStyle, bool autoFitTable)
        {
            var richEditControl = rtfDocUserControl.RichEditControl;

            // Получаем заголовок таблицы
            var tableTitle = RichTextUtils.GetFirstTableTitle(htmlData);

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

                // Удаляем последнюю пустую строку, которая появляется в реадкторе после вставки контента 
                richEditControl.Document.Delete(richEditControl.Document.CreateRange(insertedRange.End.ToInt() - 1, 1));

                richEditControl.Document.Sections[0].Page.Landscape = true;
                richEditControl.Document.CaretPosition =
                    richEditControl.Document.Range.Start;

                //richEditControl.ActiveViewType = RichEditViewType.Simple;

                var firstTable = rtfDocUserControl.RichEditControl.Document.Tables.First;

                if (firstTable != null)
                {
                    firstTable.ForEachCell(CellProcessor);
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

                //rtfDocUserControl.AutoFitTable();

                if (autoFitTable)
                {
                    rtfDocUserControl.AutoFitTable();
                }
            }

            if (!string.IsNullOrEmpty(tableTitle))
                rtfDocUserControl.SetTableTitle(tableTitle);

            richEditControl.Document.EndUpdate();

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
            var cellParagraph = rtfDocUserControl.RichEditControl.Document.BeginUpdateParagraphs(cell.Range);

            if (cellParagraph.BackColor.GetValueOrDefault().ToArgb().Equals(TransfromService.HtmlUtils.CommonTableHeaderColor.ArgbValue))
            {
                //cell.HeightType = HeightType.Auto;
                //cell.PreferredWidthType = WidthType.Auto;
                //cell.Height = 500;
                cell.BackgroundColor = TransfromService.HtmlUtils.CommonTableHeaderColor.Value;
                //cell.PreferredWidthType = WidthType.Fixed;
            }

            rtfDocUserControl.RichEditControl.Document.EndUpdateParagraphs(cellParagraph);
        }

        //private void SetHtmlContentToEditor(string html)
        //{
        //    //string text = "<b>SOME SAMPLE TEXT</b>";
        //    //AddHeaderToDocument(server.Document, text);
        //    //AddFooterToDocument(server.Document, text);
        //    rtfDocUserControl.RichEdit.Document.BeginUpdate();
        //    rtfDocUserControl.RichEdit.Document.Delete(rtfDocUserControl.RichEdit.Document.Range);

        //    using (RichEditDocumentServer server = new RichEditDocumentServer())
        //    {
        //        server.HtmlText = html;

        //        rtfDocUserControl.RichEdit.Document.InsertDocumentContent(
        //            rtfDocUserControl.RichEdit.Document.Range.Start, server.Document.Range);
        //    }

        //    rtfDocUserControl.RichEdit.Document.EndUpdate();
        //}

        private void AdjustControlsState()
        {
            cmdHtml2Editor.Enabled = cmdFormatHtml.Enabled = txtHtml.Document.CurrentSnapshot.HasContent;

            cmdCopyJson.Enabled = cmdJson2Editor.Enabled =
                cmdFormatJson.Enabled = txtJson.Document.CurrentSnapshot.HasContent;

            cmdEditor2Html.Enabled = cmdEditor2Json.Enabled = !rtfDocUserControl.RichEditControl.Document.IsEmpty;
        }

        private void AdjustParametersTab()
        {
            tcgParameters.ShowTabHeader = DefaultBoolean.False;

            if (tcgEditors.SelectedTabPage == lcgJsonEditor)
            {
                lcgHtmlParameters.Visibility = LayoutVisibility.Never;
                lcgJsonParameters.Visibility = LayoutVisibility.OnlyInRuntime;

                tcgParameters.SelectedTabPage = lcgJsonParameters;
            }
            else if (tcgEditors.SelectedTabPage == lcgHtmlEditor)
            {
                lcgJsonParameters.Visibility = LayoutVisibility.Never;
                lcgHtmlParameters.Visibility = LayoutVisibility.OnlyInRuntime;

                tcgParameters.SelectedTabPage = lcgHtmlParameters;
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

        private void CmdCopyJsonClick(object sender, System.EventArgs e)
        {
            Utils.CopyJsonToClipBoard(txtJson.Text);
            //TestHtmlContent(File.ReadAllText("..\\..\\Data\\table.html"));
        }

        private void cmdFormatJson_Click(object sender, System.EventArgs e)
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

        private void CmdJson2EditorClick(object sender, System.EventArgs e)
        {
            try
            {
                Utils.ShowProgressForm();
                CreateTableOnEditorByJsonData(txtJson.Text);
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

        private void CreateTableOnEditorByJsonData(string jsonData, JsonTransformViewParameters transformParams = null)
        {
            if (transformParams == null)
            {
                transformParams =
                    jsonTransformParamsUserControl.GetParameters<JsonTransformViewParameters>();
            }

            var htmlTransformParams = transformParams.ConvertToHtmlTransformParameters();

            var htmlData = Utils.TransformJson2Html(jsonData, htmlTransformParams);
            SetHtmlContentToEditor(htmlData, true, false);
        }

        private void CreateTableOnEditorByHtmlData(string htmlData, HtmlTransformViewParameters transformParams = null)
        {
            if (transformParams == null)
                transformParams = htmlTransformParamsUserControl.GetParameters<HtmlTransformViewParameters>();

            if (transformParams.TransformViaJson)
            {
                var jsonTransformParams = new JsonTransformViewParameters
                {
                    NeedDoubleTransformation = false,
                    CopyJsonToClipboardAfterTransformation = false
                };

                // HTML -> JSON
                var jsonData = Utils.TransformHtml2Json(htmlData, jsonTransformParams);

                // JSON -> HTML
                htmlData = Utils.TransformJson2Html(jsonData, transformParams);
            }
            else
            {
                // HTML -> HTML
                var html2HtmlTransformParams = transformParams.ConvertToHtml2HtmlTransformParameters();
                htmlData = Utils.TransformHtml2Html(htmlData, html2HtmlTransformParams);
            }

            SetHtmlContentToEditor(htmlData, true, false);
        }

        private void CmdEditor2JsonClick(object sender, System.EventArgs e)
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
            var htmlData = rtfDocUserControl.RichEditControl.Document.GetHtmlContent(RichTextUtils.TextRangeType.All,
                rtfDocUserControl.GetTableTitle(), rtfDocUserControl.RichEditControl.Options.Export.Html);

            if (jsonTransformParamsUserControl.GetParameters() is JsonTransformViewParameters transformParams)
            {
                transformParams.NeedDoubleTransformation = false;

                var jsonData = Utils.TransformHtml2Json(htmlData, transformParams);

                InsertNewJsonData(jsonData, false);
            }
        }

        public void InsertNewJsonData(string jsonData, bool updateTable,
            JsonTransformViewParameters transformParams = null)
        {
            txtJson.Text = jsonData;
            txtJson.ActiveView.Selection.CaretPosition = new TextPosition(0, 0);
            //txtJson.Format();

            if (updateTable)
                CreateTableOnEditorByJsonData(jsonData, transformParams);

            tcgEditors.SelectedTabPage = lcgJsonEditor;
        }

        public void InsertNewHtmlData(string htmlData, bool updateTable,
            HtmlTransformViewParameters transformParams = null)
        {
            txtHtml.Text = htmlData;
            txtHtml.ActiveView.Selection.CaretPosition = new TextPosition(0, 0);

            if (updateTable)
                CreateTableOnEditorByHtmlData(htmlData, transformParams);

            tcgEditors.SelectedTabPage = lcgHtmlEditor;
        }

        private void tcgEditors_SelectedPageChanged(object sender, LayoutTabPageChangedEventArgs e)
        {
            AdjustParametersTab();
        }

        private void cmdHtml2Editor_Click(object sender, EventArgs e)
        {
            try
            {
                Utils.ShowProgressForm();
                CreateTableOnEditorByHtmlData(txtHtml.Text);
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
                InsertNewHtmlData(
                    rtfDocUserControl.RichEditControl.Document.GetHtmlContent(RichTextUtils.TextRangeType.All,
                        rtfDocUserControl.GetTableTitle(),
                        rtfDocUserControl.RichEditControl.Options.Export.Html), false);
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

        public void LoadParameters()
        {
            var html2JsonTransformParams =
                Utils.LoadParameters<JsonTransformViewParameters>(Html2JsonParamsSectionName);
            jsonTransformParamsUserControl.SetParameters(html2JsonTransformParams);

            var json2HtmlTransformParams =
                Utils.LoadParameters<HtmlTransformViewParameters>(Json2HtmlParamsSectionName);
            htmlTransformParamsUserControl.SetParameters(json2HtmlTransformParams);
        }

        public void SaveParameters()
        {
            var html2JsonTransformParams =
                jsonTransformParamsUserControl.GetParameters() as JsonTransformViewParameters;
            Utils.SaveParameters(html2JsonTransformParams, Html2JsonParamsSectionName);

            var json2HtmlTransformParams =
                htmlTransformParamsUserControl.GetParameters() as HtmlTransformViewParameters;
            Utils.SaveParameters(json2HtmlTransformParams, Json2HtmlParamsSectionName);
        }
    }
}