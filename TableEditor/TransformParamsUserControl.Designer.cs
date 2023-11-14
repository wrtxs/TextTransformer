namespace TableEditor
{
    partial class TransformParamsUserControl
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.ceRemoveFormatting = new DevExpress.XtraEditors.CheckEdit();
            this.rgCellValueFormat = new DevExpress.XtraEditors.RadioGroup();
            this.ceReplaceTabsBySpaces = new DevExpress.XtraEditors.CheckEdit();
            this.ceProcessTextColor = new DevExpress.XtraEditors.CheckEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.simpleSeparator1 = new DevExpress.XtraLayout.SimpleSeparator();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ceRemoveFormatting.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgCellValueFormat.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceReplaceTabsBySpaces.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceProcessTextColor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.ceRemoveFormatting);
            this.layoutControl1.Controls.Add(this.rgCellValueFormat);
            this.layoutControl1.Controls.Add(this.ceReplaceTabsBySpaces);
            this.layoutControl1.Controls.Add(this.ceProcessTextColor);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(626, 332, 650, 400);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(370, 93);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // ceRemoveFormatting
            // 
            this.ceRemoveFormatting.Location = new System.Drawing.Point(164, 60);
            this.ceRemoveFormatting.Name = "ceRemoveFormatting";
            this.ceRemoveFormatting.Properties.Appearance.Options.UseTextOptions = true;
            this.ceRemoveFormatting.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.ceRemoveFormatting.Properties.Caption = "Убирать форматирование из HTML";
            this.ceRemoveFormatting.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.ceRemoveFormatting.Size = new System.Drawing.Size(204, 20);
            this.ceRemoveFormatting.StyleController = this.layoutControl1;
            this.ceRemoveFormatting.TabIndex = 25;
            // 
            // rgCellValueFormat
            // 
            this.rgCellValueFormat.Location = new System.Drawing.Point(86, 12);
            this.rgCellValueFormat.Name = "rgCellValueFormat";
            this.rgCellValueFormat.Properties.Columns = 1;
            this.rgCellValueFormat.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Html"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Text")});
            this.rgCellValueFormat.Size = new System.Drawing.Size(55, 58);
            this.rgCellValueFormat.StyleController = this.layoutControl1;
            this.rgCellValueFormat.TabIndex = 24;
            // 
            // ceReplaceTabsBySpaces
            // 
            this.ceReplaceTabsBySpaces.Location = new System.Drawing.Point(164, 36);
            this.ceReplaceTabsBySpaces.Name = "ceReplaceTabsBySpaces";
            this.ceReplaceTabsBySpaces.Properties.Appearance.Options.UseTextOptions = true;
            this.ceReplaceTabsBySpaces.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.ceReplaceTabsBySpaces.Properties.Caption = "Заменять табы пробелами";
            this.ceReplaceTabsBySpaces.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.ceReplaceTabsBySpaces.Size = new System.Drawing.Size(204, 20);
            this.ceReplaceTabsBySpaces.StyleController = this.layoutControl1;
            this.ceReplaceTabsBySpaces.TabIndex = 23;
            // 
            // ceProcessTextColor
            // 
            this.ceProcessTextColor.Location = new System.Drawing.Point(164, 12);
            this.ceProcessTextColor.Name = "ceProcessTextColor";
            this.ceProcessTextColor.Properties.Appearance.Options.UseTextOptions = true;
            this.ceProcessTextColor.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.ceProcessTextColor.Properties.Caption = "Обрабатывать цвет текста";
            this.ceProcessTextColor.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.ceProcessTextColor.Size = new System.Drawing.Size(204, 20);
            this.ceProcessTextColor.StyleController = this.layoutControl1;
            this.ceProcessTextColor.TabIndex = 22;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem3,
            this.emptySpaceItem4,
            this.emptySpaceItem5,
            this.simpleSeparator1,
            this.layoutControlItem2,
            this.layoutControlItem4,
            this.emptySpaceItem3,
            this.emptySpaceItem1,
            this.emptySpaceItem2});
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(370, 93);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.ceReplaceTabsBySpaces;
            this.layoutControlItem1.Location = new System.Drawing.Point(162, 34);
            this.layoutControlItem1.MaxSize = new System.Drawing.Size(208, 24);
            this.layoutControlItem1.MinSize = new System.Drawing.Size(208, 24);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(208, 24);
            this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.rgCellValueFormat;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 10);
            this.layoutControlItem3.MaxSize = new System.Drawing.Size(141, 62);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(141, 62);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 0, 2, 2);
            this.layoutControlItem3.Size = new System.Drawing.Size(141, 62);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.Text = "Формат ячеек";
            this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem3.TextLocation = DevExpress.Utils.Locations.Left;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(71, 20);
            this.layoutControlItem3.TextToControlDistance = 5;
            // 
            // emptySpaceItem4
            // 
            this.emptySpaceItem4.AllowHotTrack = false;
            this.emptySpaceItem4.Location = new System.Drawing.Point(141, 10);
            this.emptySpaceItem4.MaxSize = new System.Drawing.Size(10, 72);
            this.emptySpaceItem4.MinSize = new System.Drawing.Size(10, 72);
            this.emptySpaceItem4.Name = "emptySpaceItem4";
            this.emptySpaceItem4.Size = new System.Drawing.Size(10, 72);
            this.emptySpaceItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem5
            // 
            this.emptySpaceItem5.AllowHotTrack = false;
            this.emptySpaceItem5.Location = new System.Drawing.Point(152, 10);
            this.emptySpaceItem5.MaxSize = new System.Drawing.Size(10, 72);
            this.emptySpaceItem5.MinSize = new System.Drawing.Size(10, 72);
            this.emptySpaceItem5.Name = "emptySpaceItem5";
            this.emptySpaceItem5.Size = new System.Drawing.Size(10, 72);
            this.emptySpaceItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
            // 
            // simpleSeparator1
            // 
            this.simpleSeparator1.AllowHotTrack = false;
            this.simpleSeparator1.Location = new System.Drawing.Point(151, 10);
            this.simpleSeparator1.Name = "simpleSeparator1";
            this.simpleSeparator1.Size = new System.Drawing.Size(1, 72);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.ceProcessTextColor;
            this.layoutControlItem2.Location = new System.Drawing.Point(162, 10);
            this.layoutControlItem2.MaxSize = new System.Drawing.Size(208, 24);
            this.layoutControlItem2.MinSize = new System.Drawing.Size(208, 24);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(208, 24);
            this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.ceRemoveFormatting;
            this.layoutControlItem4.Location = new System.Drawing.Point(162, 58);
            this.layoutControlItem4.MaxSize = new System.Drawing.Size(208, 24);
            this.layoutControlItem4.MinSize = new System.Drawing.Size(208, 24);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(208, 24);
            this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(0, 0);
            this.emptySpaceItem3.MaxSize = new System.Drawing.Size(168, 10);
            this.emptySpaceItem3.MinSize = new System.Drawing.Size(168, 10);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(370, 10);
            this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 82);
            this.emptySpaceItem1.MaxSize = new System.Drawing.Size(359, 11);
            this.emptySpaceItem1.MinSize = new System.Drawing.Size(359, 11);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(370, 11);
            this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 72);
            this.emptySpaceItem2.MaxSize = new System.Drawing.Size(141, 10);
            this.emptySpaceItem2.MinSize = new System.Drawing.Size(141, 10);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(141, 10);
            this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // TransformParamsUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "TransformParamsUserControl";
            this.Size = new System.Drawing.Size(370, 93);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ceRemoveFormatting.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgCellValueFormat.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceReplaceTabsBySpaces.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceProcessTextColor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.CheckEdit ceReplaceTabsBySpaces;
        private DevExpress.XtraEditors.CheckEdit ceProcessTextColor;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.RadioGroup rgCellValueFormat;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem5;
        private DevExpress.XtraLayout.SimpleSeparator simpleSeparator1;
        private DevExpress.XtraEditors.CheckEdit ceRemoveFormatting;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
    }
}
