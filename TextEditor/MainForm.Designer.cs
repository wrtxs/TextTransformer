namespace TextEditor
{
    sealed partial class MainForm
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
            htmlImportUserControl = new HtmlImportUserControl();
            tnpEditor = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            textEditorUserControl = new TextEditorUserControl();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tabPaneMain).BeginInit();
            tabPaneMain.SuspendLayout();
            tnpImportFromHtml.SuspendLayout();
            tnpEditor.SuspendLayout();
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
            tabPaneMain.Controls.Add(tnpEditor);
            tabPaneMain.Location = new Point(2, 2);
            tabPaneMain.Name = "tabPaneMain";
            tabPaneMain.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] { tnpEditor, tnpImportFromHtml });
            tabPaneMain.RegularSize = new Size(1135, 564);
            tabPaneMain.SelectedPage = tnpEditor;
            tabPaneMain.Size = new Size(1135, 564);
            tabPaneMain.TabIndex = 4;
            tabPaneMain.Text = "tabPane1";
            // 
            // tnpImportFromHtml
            // 
            tnpImportFromHtml.Caption = "Преобразование без представления";
            tnpImportFromHtml.Controls.Add(htmlImportUserControl);
            tnpImportFromHtml.ImageOptions.Image = (Image)resources.GetObject("tnpImportFromHtml.ImageOptions.Image");
            tnpImportFromHtml.ImageOptions.SvgImageSize = new Size(16, 16);
            tnpImportFromHtml.ItemShowMode = DevExpress.XtraBars.Navigation.ItemShowMode.ImageAndText;
            tnpImportFromHtml.Name = "tnpImportFromHtml";
            tnpImportFromHtml.Properties.ShowMode = DevExpress.XtraBars.Navigation.ItemShowMode.ImageAndText;
            tnpImportFromHtml.Size = new Size(1135, 564);
            // 
            // htmlImportUserControl
            // 
            htmlImportUserControl.Dock = DockStyle.Fill;
            htmlImportUserControl.Location = new Point(0, 0);
            htmlImportUserControl.Name = "htmlImportUserControl";
            htmlImportUserControl.Size = new Size(1135, 564);
            htmlImportUserControl.TabIndex = 0;
            // 
            // tnpEditor
            // 
            tnpEditor.Caption = "Преобразование через представление";
            tnpEditor.Controls.Add(textEditorUserControl);
            tnpEditor.ImageOptions.Image = (Image)resources.GetObject("tnpEditor.ImageOptions.Image");
            tnpEditor.ImageOptions.SvgImageSize = new Size(16, 16);
            tnpEditor.ItemShowMode = DevExpress.XtraBars.Navigation.ItemShowMode.ImageAndText;
            tnpEditor.Name = "tnpEditor";
            tnpEditor.Properties.ShowMode = DevExpress.XtraBars.Navigation.ItemShowMode.ImageAndText;
            tnpEditor.Size = new Size(1135, 531);
            // 
            // textEditorUserControl
            // 
            textEditorUserControl.Dock = DockStyle.Fill;
            textEditorUserControl.Location = new Point(0, 0);
            textEditorUserControl.Name = "textEditorUserControl";
            textEditorUserControl.Size = new Size(1135, 531);
            textEditorUserControl.TabIndex = 0;
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
            Text = "TextEditor";
            WindowState = FormWindowState.Maximized;
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tabPaneMain).EndInit();
            tabPaneMain.ResumeLayout(false);
            tnpImportFromHtml.ResumeLayout(false);
            tnpEditor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraBars.Navigation.TabPane tabPaneMain;
        private DevExpress.XtraBars.Navigation.TabNavigationPage tnpImportFromHtml;
        private DevExpress.XtraBars.Navigation.TabNavigationPage tnpEditor;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem;
        private HtmlImportUserControl htmlImportUserControl;
        private TextEditorUserControl textEditorUserControl;
    }
}

