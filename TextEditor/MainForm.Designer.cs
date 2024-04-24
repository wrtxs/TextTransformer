using DevExpress.XtraReports.Design;
using DevExpress.XtraTab;
using XtraTabControl = DevExpress.XtraTab.XtraTabControl;

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
            tabPaneMain = new XtraTabControl();
            tnpEditor = new XtraTabPage();
            textEditorUserControl = new TextEditorUserControl();
            tnpImportFromHtml = new XtraTabPage();
            ((System.ComponentModel.ISupportInitialize)tabPaneMain).BeginInit();
            tabPaneMain.SuspendLayout();
            tnpEditor.SuspendLayout();
            SuspendLayout();
            // 
            // tabPaneMain
            // 
            tabPaneMain.AllowDrop = true;
            tabPaneMain.Dock = DockStyle.Fill;
            tabPaneMain.Location = new Point(0, 0);
            tabPaneMain.Name = "tabPaneMain";
            tabPaneMain.SelectedTabPage = tnpEditor;
            tabPaneMain.Size = new Size(1139, 568);
            tabPaneMain.TabIndex = 4;
            tabPaneMain.TabPages.AddRange(new XtraTabPage[] { tnpEditor, tnpImportFromHtml });
            // 
            // tnpEditor
            // 
            tnpEditor.Controls.Add(textEditorUserControl);
            tnpEditor.ImageOptions.Image = (Image)resources.GetObject("tnpEditor.ImageOptions.Image");
            tnpEditor.ImageOptions.SvgImageSize = new Size(16, 16);
            tnpEditor.Name = "tnpEditor";
            tnpEditor.Size = new Size(1137, 540);
            tnpEditor.Text = "Преобразование через редактор";
            // 
            // textEditorUserControl
            // 
            textEditorUserControl.Dock = DockStyle.Fill;
            textEditorUserControl.Location = new Point(0, 0);
            textEditorUserControl.Name = "textEditorUserControl";
            textEditorUserControl.Size = new Size(1137, 540);
            textEditorUserControl.TabIndex = 0;
            // 
            // tnpImportFromHtml
            // 
            tnpImportFromHtml.ImageOptions.Image = (Image)resources.GetObject("tnpImportFromHtml.ImageOptions.Image");
            tnpImportFromHtml.ImageOptions.SvgImageSize = new Size(16, 16);
            tnpImportFromHtml.Name = "tnpImportFromHtml";
            tnpImportFromHtml.Size = new Size(1137, 540);
            tnpImportFromHtml.Text = "Преобразование без редактора";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1139, 568);
            Controls.Add(tabPaneMain);
            IconOptions.Image = (Image)resources.GetObject("MainForm.IconOptions.Image");
            IconOptions.LargeImage = Properties.Resources.tablelayout_32x32;
            Name = "MainForm";
            Text = "TextEditor";
            WindowState = FormWindowState.Maximized;
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)tabPaneMain).EndInit();
            tabPaneMain.ResumeLayout(false);
            tnpEditor.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        //private DevExpress.XtraBars.Navigation.TabPane tabPaneMain;
        private DevExpress.XtraTab.XtraTabControl tabPaneMain;
        private DevExpress.XtraTab.XtraTabPage tnpImportFromHtml;
        private DevExpress.XtraTab.XtraTabPage tnpEditor;
        private TextEditorUserControl textEditorUserControl;
    }
}

