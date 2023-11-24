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
using System.Linq;
using TransfromService;
using TransfromService.RichText;

namespace TableEditor
{
    public partial class TableEditorUserControl : XtraUserControl
    {
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
        private void LayoutControlDragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            var hitInfo =
                this.layoutControl.CalcHitInfo(this.layoutControl.PointToClient(new Point(e.X, e.Y))) as
                    TabbedGroupHitInfo;

            if (hitInfo != null)
            {
                var tcg = hitInfo.Item as TabbedControlGroup;
                if (tcg.SelectedTabPageIndex != hitInfo.TabPageIndex)
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

        private void SetHtmlContentToEditor(string html, bool applyTableStyle, bool autoFitTable)
        {
            var richEditControl = rtfDocUserControl.RichEditControl;

            richEditControl.Document.BeginUpdate();
            richEditControl.Document.Delete(richEditControl.Document.Range);

            using (var server = new RichEditDocumentServer())
            {
                server.HtmlText = html;

                if (applyTableStyle)
                {
                    const string styleName = "Table Simple 1";

                    if (server.Document.Tables.Count > 0)
                    {
                        var tableStyle =
                            server.Document.TableStyles.FirstOrDefault(style => style.Name.Equals(styleName, StringComparison.OrdinalIgnoreCase));

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

                richEditControl.Document.InsertDocumentContent(
                    richEditControl.Document.Range.Start, server.Document.Range);

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

            richEditControl.Document.EndUpdate();

            //if (firstTable != null)
            //    firstTable.PreferredWidthType = WidthType.Fixed;

            //richEditControl.Update();
            //richEditControl.Refresh();
        }

        private void CellProcessor(TableCell cell, int rowindex, int cellindex)
        {
            //if (rowindex == 0)
            //{
            //    cell.HeightType = HeightType.Exact;
            //    cell.Height = 500;
            //    cell.BackgroundColor = Color.BlanchedAlmond;
            //}

            var cellPar = rtfDocUserControl.RichEditControl.Document.BeginUpdateParagraphs(cell.Range);

            if (cellPar.BackColor.GetValueOrDefault().ToArgb().Equals(TransfromService.Utils.CommonTableHeaderColor.ArgbValue))
            {
                //cell.HeightType = HeightType.Auto;
                //cell.PreferredWidthType = WidthType.Auto;
                //cell.Height = 500;
                cell.BackgroundColor = TransfromService.Utils.CommonTableHeaderColor.Value;
                //cell.PreferredWidthType = WidthType.Fixed;
            }

            rtfDocUserControl.RichEditControl.Document.EndUpdateParagraphs(cellPar);
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
                CreateTableOnEditorByJsonData();
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

        private void CreateTableOnEditorByJsonData()
        {
            var htmlData = Utils.TransformJson2Html(txtJson.Text);
            SetHtmlContentToEditor(htmlData, true, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlData"></param>
        /// <param name="viaJson">Признак необходимости выполнения преобразования через JSON: 1) HTML -> JSON, 2) JSON -> HTML</param>
        private void CreateTableOnEditorByHtmlData(string htmlData, bool viaJson)
        {
            if (viaJson)
            {
                var transformParams = new Html2JsonTransformParameters
                {
                    NeedDoubleTransformation = false
                };

                // HTML -> JSON
                var jsonData = Utils.TransformHtml2Json(htmlData, transformParams);

                // JSON -> HTML
                htmlData = Utils.TransformJson2Html(jsonData);
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
            var htmlData = rtfDocUserControl.RichEditControl.Document.GetHtmlContent(RichTextUtils.TextRange.All, null, rtfDocUserControl.RichEditControl.Options.Export.Html);
            var transformParams = transformParamsUserControl.GetParameters();
            transformParams.NeedDoubleTransformation = false;

            var jsonData = Utils.TransformHtml2Json(htmlData, transformParams);

            InsertNewJsonData(jsonData, false);
        }

        public void InsertNewJsonData(string jsonData, bool updateTable)
        {
            txtJson.Text = jsonData;
            txtJson.ActiveView.Selection.CaretPosition = new TextPosition(0, 0);
            //txtJson.Format();

            if (updateTable)
                CreateTableOnEditorByJsonData();

            tcgEditors.SelectedTabPage = lcgJsonEditor;
        }

        public void InsertNewHtmlData(string htmlData, bool updateTable)
        {
            txtHtml.Text = htmlData;

            if (updateTable)
                CreateTableOnEditorByHtmlData(txtHtml.Text, ceTransformViaJson.Checked);

            tcgEditors.SelectedTabPage = lcgHtmlEditor;
        }

        private void tcgEditors_SelectedPageChanged(object sender, DevExpress.XtraLayout.LayoutTabPageChangedEventArgs e)
        {
            AdjustParametersTab();
        }

        private void cmdHtml2Editor_Click(object sender, EventArgs e)
        {
            try
            {
                Utils.ShowProgressForm();
                CreateTableOnEditorByHtmlData(txtHtml.Text, ceTransformViaJson.Checked);
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
                    rtfDocUserControl.RichEditControl.Document.GetHtmlContent(RichTextUtils.TextRange.All, null,
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
    }
}
