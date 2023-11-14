using System;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using TransfromService;

namespace TableEditor
{
    public partial class TransformParamsUserControl : XtraUserControl
    {
        public TransformParamsUserControl()
        {
            InitializeComponent();
            SetParameters(new Html2JsonTransformParameters());
        }

        public Html2JsonTransformParameters GetParameters()
        {
            CellValueFormat cellValueFormat;
            bool processTextColor = ceProcessTextColor.CheckState == CheckState.Checked;
            bool replaceTabsBySpaces = ceReplaceTabsBySpaces.CheckState == CheckState.Checked;
            bool removeFormatting = ceRemoveFormatting.CheckState == CheckState.Checked;

            switch (rgCellValueFormat.SelectedIndex)
            {
                case 0:
                    cellValueFormat = CellValueFormat.Html;
                    break;
                case 1:
                    cellValueFormat = CellValueFormat.Text;
                    break;
                default:
                    cellValueFormat = CellValueFormat.Html;
                    break;
            }

            return new Html2JsonTransformParameters
            {
                CellValueFormat = cellValueFormat,
                ProcessTextColor = processTextColor,
                ReplaceTabsBySpaces = replaceTabsBySpaces,
                RemoveFormatting = removeFormatting
            };
        }

        public void SetParameters(Html2JsonTransformParameters parameters)
        {
            ceProcessTextColor.Checked = parameters.ProcessTextColor;
            ceReplaceTabsBySpaces.Checked = parameters.ReplaceTabsBySpaces;
            ceRemoveFormatting.Checked = parameters.RemoveFormatting;

            switch (parameters.CellValueFormat)
            {
                case CellValueFormat.Html:
                    rgCellValueFormat.SelectedIndex = 0;
                    break;
                case CellValueFormat.Text:
                    rgCellValueFormat.SelectedIndex = 1;
                    break;
                default:
                    rgCellValueFormat.SelectedIndex = 0;
                    break;
            }
        }
    }
}