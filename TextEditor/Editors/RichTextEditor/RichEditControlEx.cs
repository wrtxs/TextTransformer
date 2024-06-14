using System.ComponentModel;
using System.IO;
using DevExpress.Portable.Input;
using DevExpress.Portable.Input.Internal;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Mouse;

namespace TextEditor.Editors.RichTextEditor
{
    internal class RichEditControlEx : RichEditControl
    {
        public RichEditControlEx()
        {
        }

        protected override InnerRichEditControl CreateInnerControl()
        {
            return new ExInnerRichEditControl(this);
        }

        [DefaultValue(false)] public bool FormatCalculatorEnabled { get; set; }

        private class ExInnerRichEditControl : InnerRichEditControl
        {
            public ExInnerRichEditControl(IInnerRichEditControlOwner owner)
                : base(owner)
            {
            }

            protected override MouseCursorCalculator CreateMouseCursorCalculator()
            {
                return new ExMouseCursorCalculator(ActiveView);
            }
        }

        private class ExMouseCursorCalculator : MouseCursorCalculator
        {
            private static DesktopCursor _applyStyleCursor;

            private static DesktopCursor ApplyStyleCursor =>
                _applyStyleCursor ??= new DesktopCursor
                {
                    PlatformCursor = new Cursor(new MemoryStream(Properties.Resources.applyStyle))
                };

            public ExMouseCursorCalculator(RichEditView view)
                : base(view)
            {
            }

            public override IPortableCursor Calculate(RichEditHitTestResultCore hitTestResult, Point physicalPoint) =>
                //=>
                //((ExRichEditControl)View.Control).FormatCalculatorEnabled
                //    ? DevExpress.XtraRichEdit.Utils.RichEditCursors.Hand
                //    : base.Calculate(hitTestResult, physicalPoint);
                ((RichEditControlEx)View.Control).FormatCalculatorEnabled
                    ? ApplyStyleCursor
                    : base.Calculate(hitTestResult, physicalPoint);
        }

        //public ClipboardFormat GetClipboardFormat()
        //{
        //    return ClipboardFormat.All;
        //    //return copyToClipboardInHtmlFormatItem.Down ? ClipboardFormat.Html : ClipboardFormat.All;
        //}

        //public TableMetadata GetTableMetadata() => _editorService.GetTableMetadata();
        //#region IEditorService
        //public ClipboardFormat GetClipboardFormat()
        //{
        //    return ClipboardFormat.All;
        //    //return copyToClipboardInHtmlFormatItem.Down ? ClipboardFormat.Html : ClipboardFormat.All;
        //}
        //#endregion
    }
}