namespace MuzipCompress
{
    partial class MuzipButton
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
            this.SuspendLayout();
            // 
            // MuzipButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Name = "MuzipButton";
            this.Size = new System.Drawing.Size(76, 38);
            this.Load += new System.EventHandler(this.MuzipButton_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MuzipButton_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MuzipButton_MouseClick);
            this.MouseEnter += new System.EventHandler(this.MuzipButton_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.MuzipButton_MouseLeave);
            this.Resize += new System.EventHandler(this.MuzipButton_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
