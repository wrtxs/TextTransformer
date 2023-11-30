﻿using System;
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
            Html2JsonTransformParameters.ValueFormat valueFormat;
            var processTextColor = ceProcessTextColor.CheckState == CheckState.Checked;
            var replaceTabsBySpaces = ceReplaceTabsBySpaces.CheckState == CheckState.Checked;
            var removeFormatting = ceRemoveFormatting.CheckState == CheckState.Checked;

            switch (rgCellValueFormat.SelectedIndex)
            {
                case 0:
                    valueFormat = Html2JsonTransformParameters.ValueFormat.Html;
                    break;
                case 1:
                    valueFormat = Html2JsonTransformParameters.ValueFormat.Text;
                    break;
                default:
                    valueFormat = Html2JsonTransformParameters.ValueFormat.Html;
                    break;
            }

            return new Html2JsonTransformParameters
            {
                TargetFormat = valueFormat,
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

            switch (parameters.TargetFormat)
            {
                case Html2JsonTransformParameters.ValueFormat.Html:
                    rgCellValueFormat.SelectedIndex = 0;
                    break;
                case Html2JsonTransformParameters.ValueFormat.Text:
                    rgCellValueFormat.SelectedIndex = 1;
                    break;
                default:
                    rgCellValueFormat.SelectedIndex = 0;
                    break;
            }
        }
    }
}