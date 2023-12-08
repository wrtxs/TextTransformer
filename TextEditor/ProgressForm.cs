using DevExpress.XtraWaitForm;

namespace TextEditor
{
    public partial class ProgressForm : WaitForm
    {
        public static string Caption { get; set; }
        public static string Description { get; set; }

        public ProgressForm()
        {
            InitializeComponent();

            progressPanel.Caption = Caption;
            progressPanel.Description = Description;

            progressPanel.Refresh();
        }
    }
}
