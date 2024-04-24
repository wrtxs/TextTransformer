using System.Reflection;
using DevExpress.Utils;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;

namespace TextEditor
{
    public sealed partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        private const string MainParametersSectionName = "MainParameters";

        private HtmlImportUserControl _htmlImportUserControl;

        public class MainParameters
        {
            public bool ShowHtmlImportTab { get; set; } = false;
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

            ProcessHtmlImportTabVisibility();
            ProcessTabPaneMainVisibility();

            ((IConfigurable)textEditorUserControl)?.LoadParameters();
            ((IConfigurable)_htmlImportUserControl)?.LoadParameters();

            FormClosed += OnFormClosed;
        }

        private void ProcessHtmlImportTabVisibility()
        {
            if (_mainParameters.ShowHtmlImportTab) // Необходимо отобразить вкладку импорта
            {
                _htmlImportUserControl = new HtmlImportUserControl();

                tnpImportFromHtml.Controls.Add(_htmlImportUserControl);

                _htmlImportUserControl.Dock = DockStyle.Fill;
                _htmlImportUserControl.Location = new Point(0, 0);
                _htmlImportUserControl.Name = "htmlImportUserControl";
                _htmlImportUserControl.Size = new Size(1135, 531);
                _htmlImportUserControl.TabIndex = 0;

                _htmlImportUserControl.JsonToEditorEvent += HtmlImportUserControlJsonToEditorEvent;
                _htmlImportUserControl.HtmlToEditorEvent += HtmlImportUserControlHtmlToEditorEvent;
            }
            else // Вкладка импорта не отображается (удаляется)
            {
                tabPaneMain.TabPages.Remove(tnpImportFromHtml);
            }
        }

        private void ProcessTabPaneMainVisibility()
        {
            //#if DEBUG
            //            tnpImportFromHtml.Enabled = true;
            //#else
            //            tnpImportFromHtml.Enabled = false;
            //#endif
            if (tabPaneMain.TabPages.Count == 0)
                return;

            if (tabPaneMain.TabPages.Count == 1)
            {
                var tabNavigationPage = tabPaneMain.TabPages[0];
                tabPaneMain.ShowTabHeader = DefaultBoolean.False;
                tabPaneMain.BorderStyle = BorderStyles.NoBorder;
                tabPaneMain.BorderStylePage = BorderStyles.NoBorder;
                tabPaneMain.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
                tabPaneMain.LookAndFeel.UseDefaultLookAndFeel = false;

                //var control = tabNavigationPage.Controls[0];
                //control.Dock = DockStyle.Fill;
                //Controls.Remove(tabPaneMain);
                //Controls.Add(control);
                //tabPaneMain.Hide();
            }
            else if (tabPaneMain.TabPages.Count > 1)
            {
                tabPaneMain.DragOver += TabPaneMain_DragOver;
                tabPaneMain.TabPages[0].Select();
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
        private void TabPaneMain_DragOver(object sender, DragEventArgs e)
        {
            //if (hitInfo != null)
            //{
            //    TabbedControlGroup tcg = hitInfo.Item as TabbedControlGroup;
            //    if (tcg.SelectedTabPageIndex != hitInfo.TabPageIndex)
            //        tcg.SelectedTabPageIndex = hitInfo.TabPageIndex;
            //}
            var s = tabPaneMain.CalcHitInfo(tabPaneMain.PointToClient(new Point
            {
                X = e.X,
                Y = e.Y
            }));

            var tnp = s.Page;

            if (tnp != null)
            {
                if (tabPaneMain.SelectedTabPage != tnp && tnp.Enabled)
                    tabPaneMain.SelectedTabPage = tnp;
            }

            //if (tabPaneMain.CalcHitInfo(tabPaneMain.PointToClient(new System.Drawing.Point
            //    {
            //        X = e.X,
            //        Y = e.Y
            //    })) is TabNavigationPage tnp)
            //{
            //    if (tabPaneMain.SelectedPage != tnp && tnp.Enabled)
            //        tabPaneMain.SelectedPage = tnp;
            //}
        }

        private void HtmlImportUserControlJsonToEditorEvent(object sender,
            HtmlImportUserControl.JsonExportEventArgs e)
        {
            textEditorUserControl.InsertNewJsonData(e.JsonData, true);
            tabPaneMain.SelectedTabPage = tnpEditor;
        }

        private void HtmlImportUserControlHtmlToEditorEvent(object sender, HtmlImportUserControl.HtmlExportEventArgs e)
        {
            textEditorUserControl.InsertNewHtmlData(e.HtmlData, true);
            tabPaneMain.SelectedTabPage = tnpEditor;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Utils.CloseProgressForm();
        }
    }
}