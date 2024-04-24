using System.IO;
using ActiproSoftware.Text;
using ActiproSoftware.UI.WinForms.Controls.SyntaxEditor;
using Microsoft.Win32;
using DevExpress.XtraEditors;
using Newtonsoft.Json.Linq;
using DevExpress.XtraSplashScreen;
using TransformService;
using Newtonsoft.Json;
using TextEditor.TransformParameters;

namespace TextEditor
{
    internal static class Utils
    {
        public static void UpdateRegistry()
        {
            var key = GetRegistryKey(@"SOFTWARE\Actipro Software\WinForms Controls\21.1",
                RegistryKeyType.HKeyLocalMachine);
            WriteValuesToRegistry(key);
            key?.Close();

            key = GetRegistryKey(@"SOFTWARE\Wow6432Node\Actipro Software\WinForms Controls\21.1",
                RegistryKeyType.HKeyLocalMachine);
            WriteValuesToRegistry(key);
            key?.Close();

            //var key = GetRegistryKey(@"SOFTWARE\Actipro Software\WinForms Controls\21.1", RegistryKeyType.HKeyCurrentUser);
            //WriteValuesToRegistry(key);
            //key?.Close();
        }

        private static RegistryKey GetRegistryKey(string registryPath, RegistryKeyType keyType)
        {
            return keyType == RegistryKeyType.HKeyLocalMachine
                ? Registry.LocalMachine.OpenSubKey(registryPath, true) ??
                  Registry.LocalMachine.CreateSubKey(registryPath)
                : Registry.CurrentUser.OpenSubKey(registryPath, true) ??
                  Registry.CurrentUser.CreateSubKey(registryPath);
        }

        private static void WriteValuesToRegistry(RegistryKey key)
        {
            WriteValueToRegistry(key, "Licensee", "BOARD4ALL");
            WriteValueToRegistry(key, "LicenseKey", "WIN211-8PYU2-Y6C23-KVVE2-DFCG");
            WriteValueToRegistry(key, "LicenseType", "Full release");
        }

        private static void WriteValueToRegistry(RegistryKey key, string subKey, string subKeyValue)
        {
            key?.SetValue(subKey, subKeyValue);
            //key?.Close();
        }

        private enum RegistryKeyType
        {
            HKeyLocalMachine,
            HKeyCurrentUser
        }

        public static void Format(this SyntaxEditor editor)
        {
            var document = editor.Document;

            var textFormatter = document.Language.GetService<ITextFormatter>();

            textFormatter?.Format(document.CurrentSnapshot,
                TextPositionRange.CreateCollection(document.CurrentSnapshot.SnapshotRange.PositionRange, false),
                TextFormatMode.All);

            editor.ActiveView.Selection.CaretPosition = new TextPosition(0, 0);
        }

        public static void CopyJsonToClipBoard(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
                return;

            try
            {
                var singleLineText = JObject.Parse(jsonData).ToString(Formatting.None);

                if (!string.IsNullOrEmpty(singleLineText))
                    Clipboard.SetText(singleLineText);
            }
            catch (Exception e)
            {
                ProcessException(e);
            }
        }

        public static void ProcessException(Exception ex)
        {
            Utils.CloseProgressForm();
            XtraMessageBox.Show("При выполнении операции возникла следующая ошибка: " + ex.Message, "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void SetDragAndDropEventsHandlers(SyntaxEditor editor)
        {
            editor.AllowDrop = true;
            editor.DragOver += EditorOnDragOver;
            editor.DragDrop += EditorOnDragDrop;
        }

        private static void EditorOnDragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private static void EditorOnDragDrop(object sender, DragEventArgs e)
        {
            try
            {
                ShowProgressForm();
                if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                    if (files.Length > 0)
                        ((SyntaxEditor)sender).Text = File.ReadAllText(files[0]);
                }
            }
            catch (Exception exception)
            {
                ProcessException(exception);
            }
            finally
            {
                CloseProgressForm();
            }
        }

        public static void ShowProgressForm(bool useFadeIn = false, bool useFadeOut = false, string caption = null,
            string description = null)
        {
            ProgressForm.Caption = caption ?? "Пожалуйста подождите";
            ProgressForm.Description = description ?? "Операция выполняется...";

            SplashScreenManager.ShowForm(typeof(ProgressForm), useFadeIn, useFadeOut);
        }

        public static void CloseProgressForm()
        {
            SplashScreenManager.CloseForm(false);
        }

        public static string TransformHtml2Json(string htmlData, JsonTransformViewParameters transformViewParams)
        {
            var html2JsonTransformParameters = transformViewParams.GetHtml2JsonTransformParameters();
            var jsonData = new Html2JsonTransformer().Transform(htmlData, html2JsonTransformParameters);

            if (transformViewParams.CopyJsonToClipboardAfterTransformation)
                CopyJsonToClipBoard(jsonData);

            return jsonData;
        }

        public static string TransformJson2Html(string jsonData, Json2HtmlTransformParameters transformParams = null) =>
            new Json2HtmlTransformer().Transform(jsonData, transformParams);

        //public static string TransformJson2Html(string jsonData,
        //    JsonTransformViewParameters transformViewParams = null) =>
        //    new Json2HtmlTransformer().Transform(jsonData, transformViewParams?.GetJson2HtmlTransformParameters());

        public static string TransformHtml2Html(string htmlData, Html2HtmlTransformParameters transformParams) =>
            new Html2HtmlTransformer().Transform(htmlData, transformParams);

        //public static HtmlTransformViewParameters ConvertToHtmlTransformParameters(
        //    this JsonTransformViewParameters transformParams) =>
        //    new()
        //    {
        //        MakeAllListsHierarchical = transformParams.MakeAllListsHierarchical
        //    };

        public static Html2HtmlTransformParameters ConvertToHtml2HtmlTransformParameters(
            this HtmlTransformViewParameters transformParams) =>
            new()
            {
                MakeAllListsHierarchical = transformParams.MakeAllListsHierarchical
            };

        public static Json2HtmlTransformParameters ConvertToJson2HtmlTransformParameters(
            this HtmlTransformViewParameters transformParams) =>
            new()
            {
                MakeAllListsHierarchical = transformParams.MakeAllListsHierarchical
            };


        //public static void SaveParameters(Html2JsonTransformViewParameters parameters, string paramName)
        //{
        //    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        //    var paramsInJson = JsonConvert.SerializeObject(parameters);

        //    if (config.AppSettings.Settings[paramName] == null)
        //    {
        //        config.AppSettings.Settings.Add(paramName, paramsInJson);
        //    }
        //    else
        //    {
        //        config.AppSettings.Settings[paramName].Value = paramsInJson;
        //    }

        //    config.Save(ConfigurationSaveMode.Modified);
        //    ConfigurationManager.RefreshSection("appSettings");
        //}

        //public static Html2JsonTransformViewParameters LoadParameters(string paramName)
        //{
        //    try
        //    {
        //        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //        var jsonData = config.AppSettings.Settings[paramName]?.Value;

        //        if (jsonData != null)
        //            return JsonConvert.DeserializeObject<Html2JsonTransformViewParameters>(jsonData);

        //        //return loadedParameters;
        //    }
        //    catch
        //    {
        //        // ignored
        //    }

        //    return new Html2JsonTransformViewParameters();
        //}

        public static void SaveParameters(object parameters, string paramName)
        {
            try
            {
                var jsonFilePath = GetJsonFilePath();

                var existingSettings = LoadAllParameters(); // Загрузить все настройки
                existingSettings[paramName] = JObject.FromObject(parameters);

                // Сохраняем или обновляем настройки для заданной секции
                var paramsInJson = JsonConvert.SerializeObject(existingSettings, Formatting.Indented);
                File.WriteAllText(jsonFilePath, paramsInJson);
            }
            catch
            {
                // Обработка ошибок
            }
        }

        public static T LoadParameters<T>(string paramName) where T : class, new()
        {
            var allParameters = LoadAllParameters();

            return allParameters.TryGetValue(paramName, out var parameters)
                ? parameters == null ? new T() : parameters.ToObject<T>()
                : new T();
        }

        private static Dictionary<string, JObject> LoadAllParameters()
        {
            try
            {
                var jsonFilePath = GetJsonFilePath();

                if (File.Exists(jsonFilePath))
                {
                    var jsonData = File.ReadAllText(jsonFilePath);
                    return JsonConvert.DeserializeObject<Dictionary<string, JObject>>(jsonData) ??
                           new Dictionary<string, JObject>();
                }
            }
            catch
            {
                // Обработка ошибок
            }

            return new Dictionary<string, JObject>();
        }

        private static string GetJsonFilePath()
        {
            // Получаем путь к папке, содержащей исполняемый файл
            var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var exeDirectory = Path.GetDirectoryName(exePath);

            // Создаем полный путь к JSON-файлу рядом с исполняемым файлом
            const string jsonFileName = "TextEditor.settings.json"; // Название файла может быть любым
            var jsonFilePath = Path.Combine(exeDirectory ?? string.Empty, jsonFileName);

            return jsonFilePath;
        }
    }
}