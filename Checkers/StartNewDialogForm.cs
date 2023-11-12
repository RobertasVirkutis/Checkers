namespace Checkers
{
    public partial class StartNewDialogForm : Form
    {
        public string Winner { get; set; }
        public StartNewDialogForm(string winner, bool lygiosios = false)
        {
            InitializeComponent();
            WinnerLabel.Text = lygiosios? "Rezultatas: lygiosios." : WinnerLabel.Text.Replace("<placeHolder>", winner);
        }

        private void yesButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void noButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }
    }
}
