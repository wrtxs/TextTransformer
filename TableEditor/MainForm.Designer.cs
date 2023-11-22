namespace TableEditor
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            tabPaneMain = new DevExpress.XtraBars.Navigation.TabPane();
            tnpImportFromHtml = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            _htmlImportUserControl = new HtmlImportUserControl();
            tnpEditTable = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            tableEditorUserControl = new TableEditorUserControl();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tabPaneMain).BeginInit();
            tabPaneMain.SuspendLayout();
            tnpImportFromHtml.SuspendLayout();
            tnpEditTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(tabPaneMain);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = Root;
            layoutControl1.Size = new Size(1139, 568);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // tabPaneMain
            // 
            tabPaneMain.AllowDrop = true;
            tabPaneMain.Controls.Add(tnpImportFromHtml);
            tabPaneMain.Controls.Add(tnpEditTable);
            tabPaneMain.Location = new Point(2, 2);
            tabPaneMain.Name = "tabPaneMain";
            tabPaneMain.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] { tnpEditTable, tnpImportFromHtml });
            tabPaneMain.RegularSize = new Size(1135, 564);
            tabPaneMain.SelectedPage = tnpEditTable;
            tabPaneMain.Size = new Size(1135, 564);
            tabPaneMain.TabIndex = 4;
            tabPaneMain.Text = "tabPane1";
            // 
            // tnpImportFromHtml
            // 
            tnpImportFromHtml.Caption = "Импорт таблицы из HTML в JSON";
            tnpImportFromHtml.Controls.Add(_htmlImportUserControl);
            tnpImportFromHtml.ImageOptions.Image = (Image)resources.GetObject("tnpImportFromHtml.ImageOptions.Image");
            tnpImportFromHtml.ImageOptions.SvgImageSize = new Size(16, 16);
            tnpImportFromHtml.ItemShowMode = DevExpress.XtraBars.Navigation.ItemShowMode.ImageAndText;
            tnpImportFromHtml.Name = "tnpImportFromHtml";
            tnpImportFromHtml.Properties.ShowMode = DevExpress.XtraBars.Navigation.ItemShowMode.ImageAndText;
            tnpImportFromHtml.Size = new Size(1135, 531);
            // 
            // _htmlImportUserControl
            // 
            _htmlImportUserControl.Dock = DockStyle.Fill;
            _htmlImportUserControl.Location = new Point(0, 0);
            _htmlImportUserControl.Name = "_htmlImportUserControl";
            _htmlImportUserControl.Size = new Size(1135, 531);
            _htmlImportUserControl.TabIndex = 0;
            // 
            // tnpEditTable
            // 
            tnpEditTable.Caption = "Редактирование таблицы";
            tnpEditTable.Controls.Add(tableEditorUserControl);
            tnpEditTable.ImageOptions.Image = (Image)resources.GetObject("tnpEditTable.ImageOptions.Image");
            tnpEditTable.ImageOptions.SvgImageSize = new Size(16, 16);
            tnpEditTable.ItemShowMode = DevExpress.XtraBars.Navigation.ItemShowMode.ImageAndText;
            tnpEditTable.Name = "tnpEditTable";
            tnpEditTable.Properties.ShowMode = DevExpress.XtraBars.Navigation.ItemShowMode.ImageAndText;
            tnpEditTable.Size = new Size(1135, 531);
            // 
            // tableEditorUserControl
            // 
            tableEditorUserControl.Dock = DockStyle.Fill;
            tableEditorUserControl.Location = new Point(0, 0);
            tableEditorUserControl.Name = "tableEditorUserControl";
            tableEditorUserControl.Size = new Size(1135, 531);
            tableEditorUserControl.TabIndex = 0;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem });
            Root.Name = "Root";
            Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            Root.Size = new Size(1139, 568);
            Root.TextVisible = false;
            // 
            // layoutControlItem
            // 
            layoutControlItem.Control = tabPaneMain;
            layoutControlItem.Location = new Point(0, 0);
            layoutControlItem.Name = "layoutControlItem";
            layoutControlItem.Size = new Size(1139, 568);
            layoutControlItem.TextSize = new Size(0, 0);
            layoutControlItem.TextVisible = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1139, 568);
            Controls.Add(layoutControl1);
            IconOptions.Image = (Image)resources.GetObject("MainForm.IconOptions.Image");
            IconOptions.LargeImage = Properties.Resources.tablelayout_32x32;
            Name = "MainForm";
            Text = "Редактор таблиц";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tabPaneMain).EndInit();
            tabPaneMain.ResumeLayout(false);
            tnpImportFromHtml.ResumeLayout(false);
            tnpEditTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraBars.Navigation.TabPane tabPaneMain;
        private DevExpress.XtraBars.Navigation.TabNavigationPage tnpImportFromHtml;
        private DevExpress.XtraBars.Navigation.TabNavigationPage tnpEditTable;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem;
        private HtmlImportUserControl _htmlImportUserControl;
        private TableEditorUserControl tableEditorUserControl;
    }
}

