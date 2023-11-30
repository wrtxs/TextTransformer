using System.Collections;
using System.ComponentModel;
using DevExpress.XtraEditors;
using TransfromService;

namespace TableEditor.TransformParameters
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
            return propertyGridControl.SelectedObject as Html2JsonTransformViewParameters;

            //Html2JsonTransformParameters.ValueFormat valueFormat;
            //var processTextColor = ceProcessTextColor.CheckState == CheckState.Checked;
            //var replaceTabsBySpaces = ceReplaceTabsBySpaces.CheckState == CheckState.Checked;
            //var removeFormatting = ceRemoveFormatting.CheckState == CheckState.Checked;

            //switch (rgCellValueFormat.SelectedIndex)
            //{
            //    case 0:
            //        valueFormat = Html2JsonTransformParameters.ValueFormat.Html;
            //        break;
            //    case 1:
            //        valueFormat = Html2JsonTransformParameters.ValueFormat.Text;
            //        break;
            //    default:
            //        valueFormat = Html2JsonTransformParameters.ValueFormat.Html;
            //        break;
            //}

            //return new Html2JsonTransformParameters
            //{
            //    TargetFormat = valueFormat,
            //    ProcessTextColor = processTextColor,
            //    ReplaceTabsBySpaces = replaceTabsBySpaces,
            //    RemoveFormatting = removeFormatting
            //};
        }

        public void SetParameters(Html2JsonTransformParameters parameters)
        {
            //ceProcessTextColor.Checked = parameters.ProcessTextColor;
            //ceReplaceTabsBySpaces.Checked = parameters.ReplaceTabsBySpaces;
            //ceRemoveFormatting.Checked = parameters.RemoveFormatting;

            //switch (parameters.TargetFormat)
            //{
            //    case Html2JsonTransformParameters.ValueFormat.Html:
            //        rgCellValueFormat.SelectedIndex = 0;
            //        break;
            //    case Html2JsonTransformParameters.ValueFormat.Text:
            //        rgCellValueFormat.SelectedIndex = 1;
            //        break;
            //    default:
            //        rgCellValueFormat.SelectedIndex = 0;
            //        break;
            //}
            propertyGridControl.CustomPropertyDescriptors += PropertyGridControl_CustomPropertyDescriptors;

            propertyGridControl.SelectedObject = new Html2JsonTransformViewParameters(parameters);
            propertyGridControl.BestFit();
        }

        private void PropertyGridControl_CustomPropertyDescriptors(object sender,
            DevExpress.XtraVerticalGrid.Events.CustomPropertyDescriptorsEventArgs e)
        {
            if (e.Context.PropertyDescriptor == null)
            {
                e.Properties = e.Properties.Sort(new PropertiesComparer());
            }
        }

        private class PropertiesComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x is PropertyDescriptor xProp && y is PropertyDescriptor yProp)
                {
                    // Получаем атрибуты PropertyOrderAttribute для свойств
                    var xOrderAttribute = GetPropertyOrderAttribute(xProp);
                    var yOrderAttribute = GetPropertyOrderAttribute(yProp);

                    return xOrderAttribute?.Value.CompareTo(yOrderAttribute?.Value) ?? 0;
                }

                return 0;
            }

            private static PropertyOrderAttribute GetPropertyOrderAttribute(MemberDescriptor property) => (PropertyOrderAttribute)property.Attributes[typeof(PropertyOrderAttribute)];
        }
    }
}