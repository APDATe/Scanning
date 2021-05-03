
namespace Scanning
{
    partial class NewForms
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
            this.buttonSkaning = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonSkaning
            // 
            this.buttonSkaning.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSkaning.BackColor = System.Drawing.SystemColors.Window;
            this.buttonSkaning.Location = new System.Drawing.Point(12, 12);
            this.buttonSkaning.MaximumSize = new System.Drawing.Size(232, 91);
            this.buttonSkaning.MinimumSize = new System.Drawing.Size(232, 91);
            this.buttonSkaning.Name = "buttonSkaning";
            this.buttonSkaning.Size = new System.Drawing.Size(232, 91);
            this.buttonSkaning.TabIndex = 0;
            this.buttonSkaning.Text = "Начать сканирование";
            this.buttonSkaning.UseVisualStyleBackColor = false;
            this.buttonSkaning.Click += new System.EventHandler(this.buttonSkaning_Click);
            // 
            // NewForms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(256, 115);
            this.Controls.Add(this.buttonSkaning);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(272, 154);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(272, 154);
            this.Name = "NewForms";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Массовое сканирование";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSkaning;
    }
}