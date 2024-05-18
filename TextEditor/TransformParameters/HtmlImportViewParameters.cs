using System.ComponentModel;
using System.Linq;

namespace TextEditor.TransformParameters
{
    public class HtmlImportViewParameters : JsonTransformViewParameters
    {
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var baseProps = base.GetProperties(attributes);
            var propsFromEditor = baseProps.Cast<PropertyDescriptor>()
                .Where(prop => prop.Category == FromEditorCategoryName)
                .Select(prop => ModifyPropertyCategory(prop, FromEditorCategoryName));

            var propsToEditor = baseProps.Cast<PropertyDescriptor>()
                .Where(prop => prop.Category == ToEditorCategoryName)
                .Select(prop => ModifyPropertyCategory(prop, ToEditorCategoryName));

            var otherProps = baseProps.Cast<PropertyDescriptor>()
                .Where(prop => prop.Category != FromEditorCategoryName && prop.Category != ToEditorCategoryName)
                .ToList();

            return new PropertyDescriptorCollection(
                propsFromEditor.Concat(propsToEditor).Concat(otherProps).ToArray());
        }

        private static PropertyDescriptor ModifyPropertyCategory(PropertyDescriptor prop, string newCategory)
        {
            var modifiedCategory = GetModifiedCategoryName(newCategory);
            return TypeDescriptor.CreateProperty(
                prop.ComponentType,
                prop,
                new CategoryAttribute(modifiedCategory));
        }

        private static string GetModifiedCategoryName(string categoryName) =>
            categoryName switch
            {
                FromEditorCategoryName => "HTML -> JSON",
                ToEditorCategoryName => "HTML <- JSON",
                _ => categoryName
            };
    }
}