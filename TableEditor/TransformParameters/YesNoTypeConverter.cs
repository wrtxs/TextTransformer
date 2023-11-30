using System.ComponentModel;

namespace TableEditor.TransformParameters
{
    public class YesNoTypeConverter : BooleanConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture,
            object value, Type destinationType)
        {
            return value is bool b && destinationType == typeof(string)
                ? b ? "Да" : "Нет"
                : base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture,
            object value)
        {
            if (value is string strValue)
            {
                if (string.Equals(strValue, "Да", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (string.Equals(strValue, "Нет", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}