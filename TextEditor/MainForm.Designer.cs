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
            tabControlMain = new XtraTabControl();
            tabPageEditor = new XtraTabPage();
            textEditorUserControl = new TextEditorUserControl();
            tabPageImportFromHtml = new XtraTabPage();
            ((System.ComponentModel.ISupportInitialize)tabControlMain).BeginInit();
            tabControlMain.SuspendLayout();
            tabPageEditor.SuspendLayout();
            SuspendLayout();
            // 
            // tabControlMain
            // 
            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.Location = new Point(0, 0);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedTabPage = tabPageEditor;
            tabControlMain.Size = new Size(1139, 568);
            tabControlMain.TabIndex = 4;
            tabControlMain.TabPages.AddRange(new XtraTabPage[] { tabPageEditor, tabPageImportFromHtml });
            // 
            // tabPageEditor
            // 
            tabPageEditor.Controls.Add(textEditorUserControl);
            tabPageEditor.ImageOptions.Image = (Image)resources.GetObject("tabPageEditor.ImageOptions.Image");
            tabPageEditor.ImageOptions.SvgImageSize = new Size(16, 16);
            tabPageEditor.Name = "tabPageEditor";
            tabPageEditor.Size = new Size(1137, 540);
            tabPageEditor.Text = "Преобразование через редактор";
            // 
            // textEditorUserControl
            // 
            textEditorUserControl.Dock = DockStyle.Fill;
            textEditorUserControl.Location = new Point(0, 0);
            textEditorUserControl.Name = "textEditorUserControl";
            textEditorUserControl.Size = new Size(1137, 540);
            textEditorUserControl.TabIndex = 0;
            // 
            // tabPageImportFromHtml
            // 
            tabPageImportFromHtml.ImageOptions.Image = (Image)resources.GetObject("tabPageImportFromHtml.ImageOptions.Image");
            tabPageImportFromHtml.ImageOptions.SvgImageSize = new Size(16, 16);
            tabPageImportFromHtml.Name = "tabPageImportFromHtml";
            tabPageImportFromHtml.Size = new Size(1137, 540);
            tabPageImportFromHtml.Text = "Преобразование без редактора";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1139, 568);
            Controls.Add(tabControlMain);
            IconOptions.Image = (Image)resources.GetObject("MainForm.IconOptions.Image");
            IconOptions.LargeImage = Properties.Resources.tablelayout_32x32;
            Name = "MainForm";
            Text = "TextEditor";
            WindowState = FormWindowState.Maximized;
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)tabControlMain).EndInit();
            tabControlMain.ResumeLayout(false);
            tabPageEditor.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        //private DevExpress.XtraBars.Navigation.TabPane tabControlMain;
        private DevExpress.XtraTab.XtraTabControl tabControlMain;
        private DevExpress.XtraTab.XtraTabPage tabPageImportFromHtml;
        private DevExpress.XtraTab.XtraTabPage tabPageEditor;
        private TextEditorUserControl textEditorUserControl;
    }
}

