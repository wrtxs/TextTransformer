using System.Collections;
using System.ComponentModel;
using DevExpress.XtraEditors;

namespace TextEditor.TransformParameters
{
    public partial class TransformParametersUserControl : XtraUserControl
    {
        public TransformParametersUserControl()
        {
            InitializeComponent();
            propertyGridControl.CustomPropertyDescriptors += PropertyGridControl_CustomPropertyDescriptors;
        }

        public T GetParameters<T>() where T : class
        {
            return GetParameters() as T;
        }

        public T GetCloneParameters<T>() where T : class, ICloneable
        {
            return (GetParameters() as T)?.Clone() as T;
        }

        public object GetParameters()
        {
            return propertyGridControl.SelectedObject;

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

        //public void SetParameters(Html2JsonTransformParameters parameters)
        //{
        //    SetParameters(new Html2JsonTransformViewParameters(parameters));
        //}

        public void SetParameters(object parameters)
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

            propertyGridControl.SelectedObject = parameters;
            //propertyGridControl.BestFit();
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

            private static PropertyOrderAttribute GetPropertyOrderAttribute(MemberDescriptor property) =>
                (PropertyOrderAttribute)property.Attributes[typeof(PropertyOrderAttribute)];
        }
    }
}