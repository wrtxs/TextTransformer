using DevExpress.XtraRichEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TableEditor
{
    internal class RichEditControlExceptionHandler
    {

        readonly RichEditControl control;

        public RichEditControlExceptionHandler(RichEditControl control)
        {
            this.control = control;
        }
        public void Install()
        {
            if (control != null)
                control.UnhandledException += OnRichEditControlUnhandledException;
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
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handled = true;
            }
            catch (ExternalException ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handled = true;
            }
            catch (System.IO.IOException ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handled = true;
            }
        }
    }
}
