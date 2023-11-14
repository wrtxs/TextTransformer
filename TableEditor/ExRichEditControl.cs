using System.ComponentModel;
using System.IO;
using DevExpress.Portable.Input;
using DevExpress.Portable.Input.Internal;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Mouse;

namespace TableEditor
{
    internal class ExRichEditControl : RichEditControl
    {
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
                ((ExRichEditControl)View.Control).FormatCalculatorEnabled
                    ? ApplyStyleCursor
                    : base.Calculate(hitTestResult, physicalPoint);
        }
    }
}
