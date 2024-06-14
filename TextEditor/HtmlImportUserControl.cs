using ActiproSoftware.Text;
using ActiproSoftware.Text.Languages.JavaScript.Implementation;
using ActiproSoftware.Text.Languages.Xml.Implementation;
using DevExpress.XtraEditors;
using TextEditor.TransformParameters;

namespace TextEditor
{
    public partial class HtmlImportUserControl : XtraUserControl, IConfigurable
    {
        public event EventHandler<JsonExportEventArgs> JsonToEditorEvent;
        public event EventHandler<HtmlExportEventArgs> HtmlToEditorEvent;

        private const string ParamsSectionName = "HtmlImportUtilsParameters";

        public HtmlImportUserControl()
        {
            InitializeComponent();

            // Задаем пробелы вместо таба
            txtJson.Document.TabSize = 4;
            txtJson.Document.AutoConvertTabsToSpaces = true;

            txtJson.Document.Language = new JsonSyntaxLanguage();
            txtJson.Document.Language.RegisterIndentProvider(new JsonIndentProvider());

            txtHtml.Document.Language = new XmlSyntaxLanguage();
            txtHtml.Document.Language.RegisterIndentProvider(new XmlIndentProvider());

            txtJson.DocumentTextChanged += CodeDocumentTextChanged;
            txtHtml.DocumentTextChanged += CodeDocumentTextChanged;

            Utils.SetDragAndDropEventsHandlers(txtJson);
            Utils.SetDragAndDropEventsHandlers(txtHtml);

            AdjustControlsState();
        }

        //private void TxtJson_DragEnter(object sender, DragEventArgs e)
        //{
        //    e.Effect = DragDropEffects.Copy;
        //}

        private void CodeDocumentTextChanged(object sender,
            ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.EditorSnapshotChangedEventArgs e)
        {
            AdjustControlsState();
        }

        private void CmdHtml2JsonClick(object sender, EventArgs e)
        {
            try
            {
                Utils.ShowProgressForm();
                TransformHtmlToJson();
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

        private void TransformHtmlToJson()
        {
            var transformParams = transformParamsUserControl.GetParameters<JsonTransformViewParameters>();
            //transformParams.NeedDoubleTransformation = true;

            //txtJson.Text = Utils.TransformHtml2Json(txtHtml.Text, transformParams);
            txtJson.Document.SetText(TextChangeTypes.ReplaceAll,
                Utils.TransformHtml2Json(txtHtml.Text, transformParams));

            //txtJson.Format();
        }

        private void cmdJsonToHtml_Click(object sender, EventArgs e)
        {
            try
            {
                Utils.ShowProgressForm();
                TransformJsonToHtml();
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

        private void TransformJsonToHtml()
        {
            var jsonTransformViewParameters = transformParamsUserControl.GetParameters<JsonTransformViewParameters>();
            //var htmlTransformParams = jsonTransformViewParameters.GetJson2HtmlTransformParameters();// ConvertToHtmlTransformParameters();

            txtHtml.Document.SetText(TextChangeTypes.ReplaceAll,
                Utils.TransformJson2Html(txtJson.Text, jsonTransformViewParameters?.GetJson2HtmlTransformParameters()));

            //txtHtml.Text = Utils.TransformJson2Html(txtJson.Text, htmlTransformParams);
        }

        private void CmdCopyJson2BufferClick(object sender, EventArgs e)
        {
            Utils.CopyJsonToClipBoard(txtJson.Text);
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

        private void cmdJsonToEditor_Click(object sender, EventArgs e)
        {
            try
            {
                Utils.ShowProgressForm();
                OnJsonToEditorEvent();
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

        private void cmdHtmlToEditor_Click(object sender, EventArgs e)
        {
            try
            {
                Utils.ShowProgressForm();
                OnHtmlToEditorEvent();
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

        private void OnJsonToEditorEvent()
        {
            JsonToEditorEvent?.Invoke(this, new JsonExportEventArgs(txtJson.Text));
        }

        private void OnHtmlToEditorEvent()
        {
            HtmlToEditorEvent?.Invoke(this, new HtmlExportEventArgs(txtHtml.Text));
        }

        public class JsonExportEventArgs : EventArgs
        {
            public string JsonData { get; }

            public JsonExportEventArgs(string jsonData)
            {
                JsonData = jsonData;
            }
        }

        public class HtmlExportEventArgs : EventArgs
        {
            public string HtmlData { get; }

            public HtmlExportEventArgs(string htmlData)
            {
                HtmlData = htmlData;
            }
        }

        private void AdjustControlsState()
        {
            cmdHtmlToEditor.Enabled = cmdFormatHtml.Enabled =
                cmdHtml2Json.Enabled = txtHtml.Document.CurrentSnapshot.HasContent;
            cmdJsonToHtml.Enabled = cmdFormatJson.Enabled = cmdCopyJson2Buffer.Enabled =
                cmdJsonToEditor.Enabled = txtJson.Document.CurrentSnapshot.HasContent;
        }

        #region IConfigurable

        public void LoadParameters(bool enableWorkbookEditor)
        {
            var htmlImportViewParameters = Utils.LoadParameters<HtmlImportViewParameters>(ParamsSectionName);
            transformParamsUserControl.SetParameters(htmlImportViewParameters);
        }

        public void SaveParameters()
        {
            var htmlImportViewParameters = transformParamsUserControl.GetParameters();
            Utils.SaveParameters(htmlImportViewParameters, ParamsSectionName);
        }

        #endregion
    }
}