namespace TextEditor.TransformParameters
{

    internal class PropertyOrderAttribute : Attribute
    {
        public int Value { get; set; }
        public PropertyOrderAttribute(int value) => Value = value;
    }
}