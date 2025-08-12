namespace PatientRegistrationApp.Forms
{
    partial class DevelopmentForm
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
            this.btnGenerateUsers = new System.Windows.Forms.Button();
            this.btnGeneratePatients = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnGenerateUsers
            // 
            this.btnGenerateUsers.Location = new System.Drawing.Point(32, 28);
            this.btnGenerateUsers.Name = "btnGenerateUsers";
            this.btnGenerateUsers.Size = new System.Drawing.Size(216, 23);
            this.btnGenerateUsers.TabIndex = 0;
            this.btnGenerateUsers.Text = "Generate users";
            this.btnGenerateUsers.UseVisualStyleBackColor = true;
            this.btnGenerateUsers.Click += new System.EventHandler(this.btnGenerateUsers_Click);
            // 
            // btnGeneratePatients
            // 
            this.btnGeneratePatients.Location = new System.Drawing.Point(32, 57);
            this.btnGeneratePatients.Name = "btnGeneratePatients";
            this.btnGeneratePatients.Size = new System.Drawing.Size(216, 23);
            this.btnGeneratePatients.TabIndex = 1;
            this.btnGeneratePatients.Text = "Generate patients";
            this.btnGeneratePatients.UseVisualStyleBackColor = true;
            this.btnGeneratePatients.Click += new System.EventHandler(this.btnGeneratePatients_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(61, 101);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(142, 101);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // DevelopmentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 145);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnGeneratePatients);
            this.Controls.Add(this.btnGenerateUsers);
            this.Name = "DevelopmentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DevelopmentForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGenerateUsers;
        private System.Windows.Forms.Button btnGeneratePatients;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnExit;
    }
}