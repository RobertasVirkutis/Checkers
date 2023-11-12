namespace Checkers
{
    partial class StartNewDialogForm
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
            yesButton = new Button();
            noButton = new Button();
            GameEndedLabel = new Label();
            WinnerLabel = new Label();
            PlayAgainLabel = new Label();
            SuspendLayout();
            // 
            // yesButton
            // 
            yesButton.Location = new Point(48, 112);
            yesButton.Name = "yesButton";
            yesButton.Size = new Size(117, 33);
            yesButton.TabIndex = 0;
            yesButton.Text = "Taip";
            yesButton.UseVisualStyleBackColor = true;
            yesButton.Click += yesButton_Click;
            // 
            // noButton
            // 
            noButton.Location = new Point(191, 112);
            noButton.Name = "noButton";
            noButton.Size = new Size(117, 33);
            noButton.TabIndex = 1;
            noButton.Text = "Ne";
            noButton.UseVisualStyleBackColor = true;
            noButton.Click += noButton_Click;
            // 
            // GameEndedLabel
            // 
            GameEndedLabel.AutoSize = true;
            GameEndedLabel.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point);
            GameEndedLabel.Location = new Point(97, 9);
            GameEndedLabel.Name = "GameEndedLabel";
            GameEndedLabel.Size = new Size(161, 25);
            GameEndedLabel.TabIndex = 2;
            GameEndedLabel.Text = "Žaidimas baigtas!\r\n";
            // 
            // WinnerLabel
            // 
            WinnerLabel.AutoSize = true;
            WinnerLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            WinnerLabel.Location = new Point(88, 45);
            WinnerLabel.Name = "WinnerLabel";
            WinnerLabel.Size = new Size(238, 19);
            WinnerLabel.TabIndex = 3;
            WinnerLabel.Text = "Laimėtojo spalva: <placeHolder>.";
            // 
            // PlayAgainLabel
            // 
            PlayAgainLabel.AutoSize = true;
            PlayAgainLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            PlayAgainLabel.Location = new Point(88, 81);
            PlayAgainLabel.Name = "PlayAgainLabel";
            PlayAgainLabel.Size = new Size(179, 19);
            PlayAgainLabel.TabIndex = 4;
            PlayAgainLabel.Text = "Ar norite žaisti dar kartą?";
            // 
            // StartNewDialogForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(355, 157);
            Controls.Add(PlayAgainLabel);
            Controls.Add(WinnerLabel);
            Controls.Add(GameEndedLabel);
            Controls.Add(noButton);
            Controls.Add(yesButton);
            Name = "StartNewDialogForm";
            Text = "Žaidimas baigtas";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button yesButton;
        private Button noButton;
        private Label GameEndedLabel;
        private Label WinnerLabel;
        private Label PlayAgainLabel;
    }
}