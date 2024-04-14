using System.Runtime.InteropServices;
using DevExpress.XtraRichEdit;

namespace TextEditor.Editors.RichTextEdit
{
    internal class RichEditControlExceptionHandler
    {
        private readonly RichEditControl _control;

        public RichEditControlExceptionHandler(RichEditControl control)
        {
            this._control = control;
        }

        public void Install()
        {
            if (_control != null)
                _control.UnhandledException += OnRichEditControlUnhandledException;
        }

        protected virtual void OnRichEditControlUnhandledException(object sender, RichEditUnhandledExceptionEventArgs e)
        {
            try
            {
                if (e.Exception != null)
                    throw e.Exception;
            }
            catch (RichEditUnsupportedFormatException ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                e.Handled = true;
            }
            catch (ExternalException ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                e.Handled = true;
            }
            catch (System.IO.IOException ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                e.Handled = true;
            }
        }
    }
}