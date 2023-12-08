namespace TextEditor
{
    partial class ProgressForm
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
            progressPanel = new DevExpress.XtraWaitForm.ProgressPanel();
            SuspendLayout();
            // 
            // progressPanel
            // 
            progressPanel.Appearance.BackColor = Color.Transparent;
            progressPanel.Appearance.Options.UseBackColor = true;
            progressPanel.Caption = "Пожалуйста подождите";
            progressPanel.Description = "Операция выполняется...";
            progressPanel.Location = new Point(12, 12);
            progressPanel.Name = "progressPanel";
            progressPanel.Size = new Size(235, 55);
            progressPanel.TabIndex = 1;
            progressPanel.Text = "progressPanel1";
            // 
            // ProgressForm
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(248, 79);
            Controls.Add(progressPanel);
            MaximizeBox = false;
            Name = "ProgressForm";
            ShowIcon = false;
            Text = "ProgressForm";
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraWaitForm.ProgressPanel progressPanel;
    }
}