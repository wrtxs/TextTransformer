namespace TableEditor.TransformParameters
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
            propertyGridControl = new DevExpress.XtraVerticalGrid.PropertyGridControl();
            ((System.ComponentModel.ISupportInitialize)propertyGridControl).BeginInit();
            SuspendLayout();
            // 
            // propertyGridControl
            // 
            propertyGridControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            propertyGridControl.Dock = DockStyle.Fill;
            propertyGridControl.Location = new Point(0, 0);
            propertyGridControl.Name = "propertyGridControl";
            propertyGridControl.OptionsBehavior.AllowSort = false;
            propertyGridControl.OptionsBehavior.PropertySort = DevExpress.XtraVerticalGrid.PropertySort.NoSort;
            propertyGridControl.OptionsBehavior.ResizeRowHeaders = false;
            propertyGridControl.OptionsView.AllowReadOnlyRowAppearance = DevExpress.Utils.DefaultBoolean.True;
            propertyGridControl.OptionsView.ShowRootCategories = false;
            propertyGridControl.Size = new Size(367, 214);
            propertyGridControl.TabIndex = 7;
            // 
            // TransformParamsUserControl
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(propertyGridControl);
            Name = "TransformParamsUserControl";
            Size = new Size(367, 214);
            ((System.ComponentModel.ISupportInitialize)propertyGridControl).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraVerticalGrid.PropertyGridControl propertyGridControl;
    }
}
