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

            var appVersion = Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;

            Text = @$"Редактор таблиц v.{appVersion}";

            htmlImportUserControl.JsonToEditorEvent += HtmlImportUserControlJsonToEditorEvent;
            htmlImportUserControl.HtmlToEditorEvent += HtmlImportUserControlHtmlToEditorEvent;

            tabPaneMain.DragOver += TabPaneMain_DragOver;

            //#if DEBUG
            //            tnpImportFromHtml.Enabled = true;
            //#else
            //            tnpImportFromHtml.Enabled = false;
            //#endif

            tabPaneMain.SelectedPage = tnpEditTable;

            tableEditorUserControl.LoadParameters();
            htmlImportUserControl.LoadParameters();

            FormClosed += OnFormClosed;
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            tableEditorUserControl.SaveParameters();
            htmlImportUserControl.SaveParameters();
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

            if (tabPaneMain.CalcHitInfo(tabPaneMain.PointToClient(new System.Drawing.Point
                {
                    X = e.X,
                    Y = e.Y
                })) is TabNavigationPage tnp)
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