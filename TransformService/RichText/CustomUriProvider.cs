using DevExpress.Office.Services;
using DevExpress.Office.Services.Implementation;
using DevExpress.Office.Utils;

namespace TransformService.RichText
{
    internal class CustomUriProvider : IUriProvider
    {
        public string CreateCssUri(string rootUri, string styleText, string relativeUri)
        {
            return string.Empty;
        }

        public string CreateImageUri(string rootUri, OfficeImage image, string relativeUri) =>
            string.IsNullOrEmpty(image.Uri)
                ? new DataStringUriProvider().CreateImageUri(rootUri, image, relativeUri)
                : image.Uri;
    }
}
