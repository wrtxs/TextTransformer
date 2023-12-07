﻿using ActiproSoftware.Text;
using ActiproSoftware.Text.Languages.JavaScript.Implementation;
using ActiproSoftware.Text.Languages.Xml.Implementation;
using DevExpress.XtraEditors;
using TableEditor.TransformParameters;

namespace TableEditor
{
    public partial class HtmlImportUserControl : XtraUserControl, ISupportParameters
    {
        public event EventHandler<JsonExportEventArgs> JsonToEditorEvent;
        public event EventHandler<HtmlExportEventArgs> HtmlToEditorEvent;

        private const string ParamsSectionName = "HtmlImportParameters";

        public HtmlImportUserControl()
        {
            InitializeComponent();
            txtHtml.Document.Language = new XmlSyntaxLanguage();
            txtHtml.Document.Language.RegisterIndentProvider(new XmlIndentProvider());
            txtJson.Document.Language = new JsonSyntaxLanguage();
            txtJson.Document.Language.RegisterIndentProvider(new JsonIndentProvider());

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

        private void CodeDocumentTextChanged(object sender, ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.EditorSnapshotChangedEventArgs e)
        {
            AdjustControlsState();
        }

        // TODO пересмотреть
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

            txtJson.Text = Utils.TransformHtml2Json(txtHtml.Text, transformParams);

            //txtJson.Format();
        }

        /// TODO пересмотреть
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
            var jsonTransformParams = transformParamsUserControl.GetParameters<JsonTransformViewParameters>();
            var htmlTransformParams = jsonTransformParams.ConvertToHtmlTransformParameters();

            txtHtml.Text = Utils.TransformJson2Html(txtJson.Text, htmlTransformParams);
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

        public void LoadParameters()
        {
            var parameters = Utils.LoadParameters<JsonTransformViewParameters>(ParamsSectionName);
            transformParamsUserControl.SetParameters(parameters);
        }

        public void SaveParameters()
        {
            var parameters = transformParamsUserControl.GetParameters();
            Utils.SaveParameters(parameters, ParamsSectionName);
        }
    }
}