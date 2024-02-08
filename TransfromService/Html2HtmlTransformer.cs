using System;

namespace TransfromService
{
    public class Html2HtmlTransformer
    {
        public string Transform(string htmlData, Html2HtmlTransformParameters transformParams)
        {
            if (string.IsNullOrEmpty(htmlData.Trim()))
                return null;

            var result = htmlData;

            if (transformParams.MakeAllListsHierarchical)
                result = HtmlUtils.MakeAllListsHierarchical(result);

            return result;
        }

        public class Html2HtmlTransformParameters : ICloneable
        {
            /// <summary>
            /// Признак трансформации всех списков из плоского представления в древовидное
            /// </summary>
            public virtual bool MakeAllListsHierarchical { get; set; } = false;

            public object Clone()
            {
                return MemberwiseClone();
            }
        }
    }
}