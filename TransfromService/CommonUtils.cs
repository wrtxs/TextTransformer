using System;

namespace TransfromService
{
    public static class CommonUtils
    {
        public static void CopyValues<TBase, TDerived>(TBase source, TDerived destination)
            where TBase : class where TDerived : class
        {
            if (source == null || destination == null)
            {
                throw new ArgumentNullException("Source and destination objects must not be null");
            }

            var baseType = typeof(TBase);
            var derivedType = typeof(TDerived);

            var propertiesToCopy = baseType.GetProperties();

            foreach (var property in propertiesToCopy)
            {
                var derivedProperty = derivedType.GetProperty(property.Name);

                if (derivedProperty != null && derivedProperty.CanWrite)
                {
                    derivedProperty.SetValue(destination, property.GetValue(source));
                }
            }
        }
    }
}