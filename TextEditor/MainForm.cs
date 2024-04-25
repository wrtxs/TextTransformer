using System.Reflection;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;

namespace TextEditor
{
    public sealed partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        private const string MainParametersSectionName = "MainParameters";

        private HtmlImportUserControl _htmlImportUserControl;

        public class MainParameters
        {
            public bool ShowHtmlImportTab { get; set; } = false;
            public bool ShowWorkbookEditorTab { get; set; } = true;
        }

        private MainParameters _mainParameters;

        public MainForm()
        {
            Utils.ShowProgressForm(false, false, null, "Идет загрузка...");

            //this.DoubleBuffered = true;
            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //UpdateStyles();

            LoadParameters();
            InitializeComponent();

            Text += @$" v.{GetAssemblyVersion()}"; // Устанавливаем заголовок приложения

            textEditorUserControl.SetWorkbookEditorTabVisibility(_mainParameters.ShowWorkbookEditorTab);
            ProcessHtmlImportTabVisibility();
            //ProcessTabPaneMainVisibility();

            ((IConfigurable)textEditorUserControl)?.LoadParameters();
            ((IConfigurable)_htmlImportUserControl)?.LoadParameters();

            FormClosed += OnFormClosed;
        }

        private void ProcessHtmlImportTabVisibility()
        {
            if (_mainParameters.ShowHtmlImportTab) // Необходимо отобразить вкладку импорта
            {
                _htmlImportUserControl = new HtmlImportUserControl();
                tabPageImportFromHtml.Controls.Add(_htmlImportUserControl);
                _htmlImportUserControl.Dock = DockStyle.Fill;
                _htmlImportUserControl.JsonToEditorEvent += HtmlImportUserControlJsonToEditorEvent;
                _htmlImportUserControl.HtmlToEditorEvent += HtmlImportUserControlHtmlToEditorEvent;
            }
            else // Вкладка импорта не отображается (удаляется)
            {
                tabControlMain.TabPages.Remove(tabPageImportFromHtml);
            }

            //#if DEBUG
            //            tabPageImportFromHtml.Enabled = true;
            //#else
            //            tabPageImportFromHtml.Enabled = false;
            //#endif
            if (tabControlMain.TabPages.Count == 0)
                return;

            if (tabControlMain.TabPages.Count == 1)
            {
                //var tabNavigationPage = tabControlMain.TabPages[0];
                tabControlMain.ShowTabHeader = DefaultBoolean.False;
                tabControlMain.BorderStyle = BorderStyles.NoBorder;
                tabControlMain.BorderStylePage = BorderStyles.NoBorder;
                tabControlMain.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
                tabControlMain.LookAndFeel.UseDefaultLookAndFeel = false;

                //var control = tabNavigationPage.Controls[0];
                //control.Dock = DockStyle.Fill;
                //Controls.Remove(tabControlMain);
                //Controls.Add(control);
                //tabControlMain.Hide();
            }
            else if (tabControlMain.TabPages.Count > 1)
            {
                tabControlMain.AllowDrop = true;
                tabControlMain.DragOver += TabControlMainDragOver;
                tabControlMain.TabPages[0].Select();
            }
        }

        public void LoadParameters() => _mainParameters = Utils.LoadParameters<MainParameters>(MainParametersSectionName);

        public void SaveParameters() => Utils.SaveParameters(_mainParameters, MainParametersSectionName);

        private string GetAssemblyVersion()
        {
            var version = "1.0";
            var versionAttribute = Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyFileVersionAttribute>();

            if (versionAttribute != null && !string.IsNullOrEmpty(versionAttribute.Version))
            {
                version = versionAttribute.Version;

                var lastIndex = version.LastIndexOf(".0", StringComparison.Ordinal);
                if (lastIndex != -1)
                    version = version.Substring(0, lastIndex);
            }

            return version;
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            SaveParameters();
            ((IConfigurable)textEditorUserControl).SaveParameters();
            ((IConfigurable)_htmlImportUserControl)?.SaveParameters();
        }

        /// <summary>
        /// Обработка перетаскивания файла для смены вкладки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControlMainDragOver(object sender, DragEventArgs e)
        {
            //if (hitInfo != null)
            //{
            //    TabbedControlGroup tcg = hitInfo.Item as TabbedControlGroup;
            //    if (tcg.SelectedTabPageIndex != hitInfo.TabPageIndex)
            //        tcg.SelectedTabPageIndex = hitInfo.TabPageIndex;
            //}
            var s = tabControlMain.CalcHitInfo(tabControlMain.PointToClient(new Point
            {
                X = e.X,
                Y = e.Y
            }));

            var tabPage = s.Page;

            if (tabPage != null)
            {
                if (tabControlMain.SelectedTabPage != tabPage && tabPage.Enabled)
                    tabControlMain.SelectedTabPage = tabPage;
            }

            //if (tabControlMain.CalcHitInfo(tabControlMain.PointToClient(new System.Drawing.Point
            //    {
            //        X = e.X,
            //        Y = e.Y
            //    })) is TabNavigationPage tnp)
            //{
            //    if (tabControlMain.SelectedPage != tnp && tnp.Enabled)
            //        tabControlMain.SelectedPage = tnp;
            //}
        }

        private void HtmlImportUserControlJsonToEditorEvent(object sender,
            HtmlImportUserControl.JsonExportEventArgs e)
        {
            textEditorUserControl.InsertNewJsonData(e.JsonData, true);
            tabControlMain.SelectedTabPage = tabPageEditor;
        }

        private void HtmlImportUserControlHtmlToEditorEvent(object sender, HtmlImportUserControl.HtmlExportEventArgs e)
        {
            textEditorUserControl.InsertNewHtmlData(e.HtmlData, true);
            tabControlMain.SelectedTabPage = tabPageEditor;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Utils.CloseProgressForm();
        }
    }
}