using System.IO;
using ActiproSoftware.Text;
using ActiproSoftware.UI.WinForms.Controls.SyntaxEditor;
using Microsoft.Win32;
using DevExpress.XtraEditors;
using Newtonsoft.Json.Linq;
using DevExpress.XtraSplashScreen;
using TransfromService;

namespace TableEditor
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
                var singleLineText = JObject.Parse(jsonData).ToString(Newtonsoft.Json.Formatting.None);

                if (!string.IsNullOrEmpty(singleLineText))
                    Clipboard.SetText(singleLineText);
            }
            catch (Exception e)
            {
                Utils.ProcessException(e);
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
                Utils.ShowProgressForm();
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                    if (files.Length > 0)
                        ((SyntaxEditor)sender).Text = File.ReadAllText(files[0]);
                }
            }
            catch (Exception exception)
            {
                Utils.ProcessException(exception);
            }
            finally
            {
                Utils.CloseProgressForm();
            }
        }

        public static void ShowProgressForm(bool useFadeIn = false, bool useFadeOut = false, string caption = null, string description = null)
        {
            ProgressForm.Caption = caption ?? "Пожалуйста подождите";
            ProgressForm.Description = description ?? "Операция выполняется...";

            SplashScreenManager.ShowForm(typeof(ProgressForm), useFadeIn, useFadeOut);
        }

        public static void CloseProgressForm()
        {
            SplashScreenManager.CloseForm(false);
        }

        public static string TransformHtml2Json(string htmlData, Html2JsonTransformParameters transformParams) =>
            new Html2JsonTransformer().Transform(htmlData, transformParams);

        public static string TransformJson2Html(string jsonData) => new Json2HtmlTransformer().Transform(jsonData);
    }
}
