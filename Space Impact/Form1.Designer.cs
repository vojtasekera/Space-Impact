namespace Space_Impact
{
    partial class Form1
    {
        /// <summary>
        /// Vyžaduje se proměnná návrháře.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Uvolněte všechny používané prostředky.
        /// </summary>
        /// <param name="disposing">hodnota true, když by se měl spravovaný prostředek odstranit; jinak false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kód generovaný Návrhářem Windows Form

        /// <summary>
        /// Metoda vyžadovaná pro podporu Návrháře - neupravovat
        /// obsah této metody v editoru kódu.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pbGame = new System.Windows.Forms.PictureBox();
            this.bStart = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.tbLevel = new System.Windows.Forms.TextBox();
            this.pbHUD = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbGame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbHUD)).BeginInit();
            this.SuspendLayout();
            // 
            // pbGame
            // 
            this.pbGame.BackColor = System.Drawing.Color.Transparent;
            this.pbGame.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbGame.Location = new System.Drawing.Point(0, 81);
            this.pbGame.Margin = new System.Windows.Forms.Padding(0);
            this.pbGame.Name = "pbGame";
            this.pbGame.Size = new System.Drawing.Size(1186, 729);
            this.pbGame.TabIndex = 0;
            this.pbGame.TabStop = false;
            // 
            // bStart
            // 
            this.bStart.Location = new System.Drawing.Point(410, 240);
            this.bStart.Margin = new System.Windows.Forms.Padding(2);
            this.bStart.Name = "bStart";
            this.bStart.Size = new System.Drawing.Size(191, 46);
            this.bStart.TabIndex = 1;
            this.bStart.Text = "Spustit hru";
            this.bStart.UseVisualStyleBackColor = true;
            this.bStart.Click += new System.EventHandler(this.bStart_Click);
            // 
            // timer
            // 
            this.timer.Interval = 30;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // tbLevel
            // 
            this.tbLevel.Location = new System.Drawing.Point(410, 206);
            this.tbLevel.Margin = new System.Windows.Forms.Padding(2);
            this.tbLevel.Name = "tbLevel";
            this.tbLevel.Size = new System.Drawing.Size(161, 20);
            this.tbLevel.TabIndex = 3;
            // 
            // pbHUD
            // 
            this.pbHUD.Dock = System.Windows.Forms.DockStyle.Top;
            this.pbHUD.Location = new System.Drawing.Point(0, 0);
            this.pbHUD.Margin = new System.Windows.Forms.Padding(0);
            this.pbHUD.Name = "pbHUD";
            this.pbHUD.Size = new System.Drawing.Size(1186, 81);
            this.pbHUD.TabIndex = 4;
            this.pbHUD.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1186, 827);
            this.Controls.Add(this.pbHUD);
            this.Controls.Add(this.tbLevel);
            this.Controls.Add(this.bStart);
            this.Controls.Add(this.pbGame);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Space Impact";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pbGame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbHUD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbGame;
        private System.Windows.Forms.Button bStart;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.TextBox tbLevel;
        private System.Windows.Forms.PictureBox pbHUD;
    }
}

