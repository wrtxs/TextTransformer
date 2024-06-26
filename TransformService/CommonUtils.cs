﻿using System;

namespace TransformService
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

        public static string ReplaceFirstOccurrence(this string original, string oldValue, string newValue)
        {
            var index = original.IndexOf(oldValue, StringComparison.Ordinal);
            return index != -1 ? original[..index] + newValue + original[(index + oldValue.Length)..] : original;
        }
    }
}