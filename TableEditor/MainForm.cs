using System.Reflection;
using DevExpress.XtraBars.Navigation;

namespace TableEditor
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        public MainForm()
        {
            Utils.ShowProgressForm(false, false, null, "Идет загрузка...");

            InitializeComponent();

            var appVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;

            Text = $"Редактор таблиц v.{appVersion}";

            _htmlImportUserControl.JsonToEditorEvent += HtmlImportUserControlJsonToEditorEvent;
            _htmlImportUserControl.HtmlToEditorEvent += HtmlImportUserControlHtmlToEditorEvent;

            tabPaneMain.DragOver += TabPaneMain_DragOver;

            //#if DEBUG
            //            tnpImportFromHtml.Enabled = true;
            //#else
            //            tnpImportFromHtml.Enabled = false;
            //#endif

            tabPaneMain.SelectedPage = tnpEditTable;
        }

        /// <summary>
        /// Обработка перетаскивания файла для смены вкладки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabPaneMain_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            var tnp = tabPaneMain.CalcHitInfo(this.tabPaneMain.PointToClient(new System.Drawing.Point
            {
                X = e.X,
                Y = e.Y
            })) as TabNavigationPage;
            //if (hitInfo != null)
            //{
            //    TabbedControlGroup tcg = hitInfo.Item as TabbedControlGroup;
            //    if (tcg.SelectedTabPageIndex != hitInfo.TabPageIndex)
            //        tcg.SelectedTabPageIndex = hitInfo.TabPageIndex;
            //}

            if (tnp != null)
            {
                if (tabPaneMain.SelectedPage != tnp && tnp.Enabled)
                    tabPaneMain.SelectedPage = tnp;
            }
        }

        private void HtmlImportUserControlJsonToEditorEvent(object sender,
            HtmlImportUserControl.JsonExportEventArgs e)
        {
            tableEditorUserControl.InsertNewJsonData(e.JsonData, true);
            tabPaneMain.SelectedPage = tnpEditTable;
        }

        private void HtmlImportUserControlHtmlToEditorEvent(object sender, HtmlImportUserControl.HtmlExportEventArgs e)
        {
            tableEditorUserControl.InsertNewHtmlData(e.HtmlData, true);
            tabPaneMain.SelectedPage = tnpEditTable;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Utils.CloseProgressForm();
        }
    }
}