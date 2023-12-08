using TextEditor;
using TextEditor.TransformParameters;

namespace TextEditor
{
    partial class HtmlImportUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            cmdJsonToHtml = new DevExpress.XtraEditors.SimpleButton();
            transformParamsUserControl = new TransformParametersUserControl();
            cmdHtmlToEditor = new DevExpress.XtraEditors.SimpleButton();
            cmdCopyJson2Buffer = new DevExpress.XtraEditors.SimpleButton();
            cmdFormatJson = new DevExpress.XtraEditors.SimpleButton();
            txtHtml = new ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor();
            txtJson = new ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor();
            cmdHtml2Json = new DevExpress.XtraEditors.SimpleButton();
            cmdFormatHtml = new DevExpress.XtraEditors.SimpleButton();
            cmdJsonToEditor = new DevExpress.XtraEditors.SimpleButton();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
            layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
            layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
            layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem7 = new DevExpress.XtraLayout.EmptySpaceItem();
            emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            simpleSeparator2 = new DevExpress.XtraLayout.SimpleSeparator();
            emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitterItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem10).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem8).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem9).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)simpleSeparator2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(cmdJsonToHtml);
            layoutControl1.Controls.Add(transformParamsUserControl);
            layoutControl1.Controls.Add(cmdHtmlToEditor);
            layoutControl1.Controls.Add(cmdCopyJson2Buffer);
            layoutControl1.Controls.Add(cmdFormatJson);
            layoutControl1.Controls.Add(txtHtml);
            layoutControl1.Controls.Add(txtJson);
            layoutControl1.Controls.Add(cmdHtml2Json);
            layoutControl1.Controls.Add(cmdFormatHtml);
            layoutControl1.Controls.Add(cmdJsonToEditor);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new Rectangle(1004, 311, 650, 400);
            layoutControl1.Root = Root;
            layoutControl1.Size = new Size(1008, 592);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // cmdJsonToHtml
            // 
            cmdJsonToHtml.Location = new Point(529, 502);
            cmdJsonToHtml.Name = "cmdJsonToHtml";
            cmdJsonToHtml.Size = new Size(136, 37);
            cmdJsonToHtml.StyleController = layoutControl1;
            cmdJsonToHtml.TabIndex = 5;
            cmdJsonToHtml.Text = "HTML <- JSON";
            cmdJsonToHtml.Click += cmdJsonToHtml_Click;
            // 
            // transformParamsUserControl
            // 
            transformParamsUserControl.Location = new Point(2, 502);
            transformParamsUserControl.Name = "transformParamsUserControl";
            transformParamsUserControl.Size = new Size(362, 78);
            transformParamsUserControl.TabIndex = 3;
            // 
            // cmdHtmlToEditor
            // 
            cmdHtmlToEditor.Location = new Point(669, 543);
            cmdHtmlToEditor.Name = "cmdHtmlToEditor";
            cmdHtmlToEditor.Size = new Size(136, 37);
            cmdHtmlToEditor.StyleController = layoutControl1;
            cmdHtmlToEditor.TabIndex = 9;
            cmdHtmlToEditor.Text = "HTML -> Редактор";
            cmdHtmlToEditor.Click += cmdHtmlToEditor_Click;
            // 
            // cmdCopyJson2Buffer
            // 
            cmdCopyJson2Buffer.Location = new Point(669, 502);
            cmdCopyJson2Buffer.Name = "cmdCopyJson2Buffer";
            cmdCopyJson2Buffer.Size = new Size(136, 37);
            cmdCopyJson2Buffer.StyleController = layoutControl1;
            cmdCopyJson2Buffer.TabIndex = 6;
            cmdCopyJson2Buffer.Text = "Копировать JSON\r\nв буфер обмена";
            cmdCopyJson2Buffer.Click += CmdCopyJson2BufferClick;
            // 
            // cmdFormatJson
            // 
            cmdFormatJson.Location = new Point(529, 543);
            cmdFormatJson.Name = "cmdFormatJson";
            cmdFormatJson.Size = new Size(136, 37);
            cmdFormatJson.StyleController = layoutControl1;
            cmdFormatJson.TabIndex = 8;
            cmdFormatJson.Text = "Форматировать JSON";
            cmdFormatJson.Click += cmdFormatJson_Click;
            // 
            // txtHtml
            // 
            txtHtml.AllowDrop = true;
            txtHtml.AutoScroll = true;
            txtHtml.BorderStyle = BorderStyle.None;
            txtHtml.Cursor = Cursors.IBeam;
            txtHtml.IsLineNumberMarginVisible = true;
            txtHtml.Location = new Point(1, 22);
            txtHtml.Name = "txtHtml";
            txtHtml.Size = new Size(491, 467);
            txtHtml.TabIndex = 0;
            // 
            // txtJson
            // 
            txtJson.AllowDrop = true;
            txtJson.AutoScroll = true;
            txtJson.BorderStyle = BorderStyle.None;
            txtJson.IsLineNumberMarginVisible = true;
            txtJson.Location = new Point(504, 22);
            txtJson.Name = "txtJson";
            txtJson.Size = new Size(503, 467);
            txtJson.TabIndex = 2;
            // 
            // cmdHtml2Json
            // 
            cmdHtml2Json.Location = new Point(389, 502);
            cmdHtml2Json.Name = "cmdHtml2Json";
            cmdHtml2Json.Size = new Size(136, 37);
            cmdHtml2Json.StyleController = layoutControl1;
            cmdHtml2Json.TabIndex = 4;
            cmdHtml2Json.Text = "HTML -> JSON";
            cmdHtml2Json.Click += CmdHtml2JsonClick;
            // 
            // cmdFormatHtml
            // 
            cmdFormatHtml.Location = new Point(389, 543);
            cmdFormatHtml.Name = "cmdFormatHtml";
            cmdFormatHtml.Size = new Size(136, 37);
            cmdFormatHtml.StyleController = layoutControl1;
            cmdFormatHtml.TabIndex = 7;
            cmdFormatHtml.Text = "Форматировать HTML\r\n(не рекомендуется)";
            cmdFormatHtml.Click += cmdFormatHtml_Click;
            // 
            // cmdJsonToEditor
            // 
            cmdJsonToEditor.Location = new Point(809, 543);
            cmdJsonToEditor.Name = "cmdJsonToEditor";
            cmdJsonToEditor.Size = new Size(136, 37);
            cmdJsonToEditor.StyleController = layoutControl1;
            cmdJsonToEditor.TabIndex = 10;
            cmdJsonToEditor.Text = "JSON -> Редактор";
            cmdJsonToEditor.Click += cmdJsonToEditor_Click;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { splitterItem1, layoutControlGroup1, layoutControlGroup2, layoutControlItem6, layoutControlItem1, emptySpaceItem4, layoutControlItem10, emptySpaceItem6, layoutControlItem8, layoutControlItem3, layoutControlItem2, emptySpaceItem5, layoutControlItem7, layoutControlItem9, emptySpaceItem7, emptySpaceItem1, simpleSeparator2, emptySpaceItem3, emptySpaceItem2 });
            Root.Name = "Root";
            Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            Root.Size = new Size(1008, 592);
            Root.TextVisible = false;
            // 
            // splitterItem1
            // 
            splitterItem1.AllowHotTrack = true;
            splitterItem1.IsCollapsible = DevExpress.Utils.DefaultBoolean.True;
            splitterItem1.Location = new Point(493, 0);
            splitterItem1.Name = "splitterItem1";
            splitterItem1.Size = new Size(10, 490);
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem5 });
            layoutControlGroup1.Location = new Point(0, 0);
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            layoutControlGroup1.Size = new Size(493, 490);
            layoutControlGroup1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            layoutControlGroup1.Text = "HTML данные";
            // 
            // layoutControlItem5
            // 
            layoutControlItem5.Control = txtHtml;
            layoutControlItem5.Location = new Point(0, 0);
            layoutControlItem5.MinSize = new Size(104, 24);
            layoutControlItem5.Name = "layoutControlItem5";
            layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            layoutControlItem5.Size = new Size(491, 467);
            layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem5.TextSize = new Size(0, 0);
            layoutControlItem5.TextVisible = false;
            // 
            // layoutControlGroup2
            // 
            layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem4 });
            layoutControlGroup2.Location = new Point(503, 0);
            layoutControlGroup2.Name = "layoutControlGroup2";
            layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            layoutControlGroup2.Size = new Size(505, 490);
            layoutControlGroup2.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            layoutControlGroup2.Text = "JSON данные";
            // 
            // layoutControlItem4
            // 
            layoutControlItem4.Control = txtJson;
            layoutControlItem4.Location = new Point(0, 0);
            layoutControlItem4.Name = "layoutControlItem4";
            layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            layoutControlItem4.Size = new Size(503, 467);
            layoutControlItem4.TextSize = new Size(0, 0);
            layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            layoutControlItem6.Control = cmdHtml2Json;
            layoutControlItem6.Location = new Point(387, 500);
            layoutControlItem6.MaxSize = new Size(140, 41);
            layoutControlItem6.MinSize = new Size(140, 41);
            layoutControlItem6.Name = "layoutControlItem6";
            layoutControlItem6.Size = new Size(140, 41);
            layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem6.TextSize = new Size(0, 0);
            layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = cmdFormatJson;
            layoutControlItem1.Location = new Point(527, 541);
            layoutControlItem1.MaxSize = new Size(140, 41);
            layoutControlItem1.MinSize = new Size(140, 41);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new Size(140, 41);
            layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem1.TextSize = new Size(0, 0);
            layoutControlItem1.TextVisible = false;
            // 
            // emptySpaceItem4
            // 
            emptySpaceItem4.AllowHotTrack = false;
            emptySpaceItem4.Location = new Point(998, 500);
            emptySpaceItem4.Name = "emptySpaceItem4";
            emptySpaceItem4.Size = new Size(10, 82);
            emptySpaceItem4.TextSize = new Size(0, 0);
            // 
            // layoutControlItem10
            // 
            layoutControlItem10.Control = transformParamsUserControl;
            layoutControlItem10.Location = new Point(0, 500);
            layoutControlItem10.MaxSize = new Size(366, 0);
            layoutControlItem10.MinSize = new Size(366, 5);
            layoutControlItem10.Name = "layoutControlItem10";
            layoutControlItem10.Size = new Size(366, 82);
            layoutControlItem10.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem10.TextSize = new Size(0, 0);
            layoutControlItem10.TextVisible = false;
            // 
            // emptySpaceItem6
            // 
            emptySpaceItem6.AllowHotTrack = false;
            emptySpaceItem6.Location = new Point(0, 582);
            emptySpaceItem6.MaxSize = new Size(0, 10);
            emptySpaceItem6.MinSize = new Size(10, 10);
            emptySpaceItem6.Name = "emptySpaceItem6";
            emptySpaceItem6.Size = new Size(1008, 10);
            emptySpaceItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            emptySpaceItem6.TextSize = new Size(0, 0);
            // 
            // layoutControlItem8
            // 
            layoutControlItem8.Control = cmdJsonToHtml;
            layoutControlItem8.Location = new Point(527, 500);
            layoutControlItem8.MaxSize = new Size(140, 41);
            layoutControlItem8.MinSize = new Size(140, 41);
            layoutControlItem8.Name = "layoutControlItem8";
            layoutControlItem8.Size = new Size(140, 41);
            layoutControlItem8.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem8.TextSize = new Size(0, 0);
            layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.Control = cmdCopyJson2Buffer;
            layoutControlItem3.Location = new Point(667, 500);
            layoutControlItem3.MaxSize = new Size(140, 41);
            layoutControlItem3.MinSize = new Size(140, 41);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Size = new Size(140, 41);
            layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem3.TextSize = new Size(0, 0);
            layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.Control = cmdFormatHtml;
            layoutControlItem2.Location = new Point(387, 541);
            layoutControlItem2.MaxSize = new Size(140, 41);
            layoutControlItem2.MinSize = new Size(140, 41);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new Size(140, 41);
            layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem2.TextSize = new Size(0, 0);
            layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem5
            // 
            emptySpaceItem5.AllowHotTrack = false;
            emptySpaceItem5.Location = new Point(947, 541);
            emptySpaceItem5.Name = "emptySpaceItem5";
            emptySpaceItem5.Size = new Size(51, 41);
            emptySpaceItem5.TextSize = new Size(0, 0);
            // 
            // layoutControlItem7
            // 
            layoutControlItem7.Control = cmdJsonToEditor;
            layoutControlItem7.Location = new Point(807, 541);
            layoutControlItem7.MaxSize = new Size(140, 41);
            layoutControlItem7.MinSize = new Size(140, 41);
            layoutControlItem7.Name = "layoutControlItem7";
            layoutControlItem7.Size = new Size(140, 41);
            layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem7.TextSize = new Size(0, 0);
            layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            layoutControlItem9.Control = cmdHtmlToEditor;
            layoutControlItem9.Location = new Point(667, 541);
            layoutControlItem9.MaxSize = new Size(140, 41);
            layoutControlItem9.MinSize = new Size(140, 41);
            layoutControlItem9.Name = "layoutControlItem9";
            layoutControlItem9.Size = new Size(140, 41);
            layoutControlItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem9.TextSize = new Size(0, 0);
            layoutControlItem9.TextVisible = false;
            // 
            // emptySpaceItem7
            // 
            emptySpaceItem7.AllowHotTrack = false;
            emptySpaceItem7.Location = new Point(807, 500);
            emptySpaceItem7.Name = "emptySpaceItem7";
            emptySpaceItem7.Size = new Size(191, 41);
            emptySpaceItem7.TextSize = new Size(0, 0);
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.AllowHotTrack = false;
            emptySpaceItem1.Location = new Point(377, 500);
            emptySpaceItem1.MaxSize = new Size(10, 0);
            emptySpaceItem1.MinSize = new Size(10, 10);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new Size(10, 82);
            emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            emptySpaceItem1.TextSize = new Size(0, 0);
            // 
            // simpleSeparator2
            // 
            simpleSeparator2.AllowHotTrack = false;
            simpleSeparator2.Location = new Point(376, 500);
            simpleSeparator2.Name = "simpleSeparator2";
            simpleSeparator2.Size = new Size(1, 82);
            // 
            // emptySpaceItem3
            // 
            emptySpaceItem3.AllowHotTrack = false;
            emptySpaceItem3.Location = new Point(0, 490);
            emptySpaceItem3.MaxSize = new Size(0, 10);
            emptySpaceItem3.MinSize = new Size(10, 10);
            emptySpaceItem3.Name = "emptySpaceItem3";
            emptySpaceItem3.Size = new Size(1008, 10);
            emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            emptySpaceItem3.TextSize = new Size(0, 0);
            // 
            // emptySpaceItem2
            // 
            emptySpaceItem2.AllowHotTrack = false;
            emptySpaceItem2.Location = new Point(366, 500);
            emptySpaceItem2.MaxSize = new Size(10, 0);
            emptySpaceItem2.MinSize = new Size(10, 10);
            emptySpaceItem2.Name = "emptySpaceItem2";
            emptySpaceItem2.Size = new Size(10, 82);
            emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            emptySpaceItem2.TextSize = new Size(0, 0);
            // 
            // HtmlImportUserControl
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(layoutControl1);
            Name = "HtmlImportUserControl";
            Size = new Size(1008, 592);
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)splitterItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem4).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem10).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem6).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem8).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem5).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem7).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem9).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem7).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)simpleSeparator2).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.SimpleButton cmdFormatHtml;
        private DevExpress.XtraEditors.SimpleButton cmdCopyJson2Buffer;
        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor txtJson;
        private ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.SyntaxEditor txtHtml;
        private DevExpress.XtraEditors.SimpleButton cmdHtml2Json;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.SplitterItem splitterItem1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraEditors.SimpleButton cmdJsonToEditor;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.SimpleSeparator simpleSeparator2;
        private DevExpress.XtraEditors.SimpleButton cmdFormatJson;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
        private DevExpress.XtraEditors.SimpleButton cmdHtmlToEditor;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private TransformParametersUserControl transformParamsUserControl;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem6;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraEditors.SimpleButton cmdJsonToHtml;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem7;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
    }
}
