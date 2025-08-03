namespace PatientRegistrationApp
{
    partial class Form1
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
            this.btnTestAdd = new System.Windows.Forms.Button();
            this.btnTestGet = new System.Windows.Forms.Button();
            this.btnTestUpdate = new System.Windows.Forms.Button();
            this.btnTestDelete = new System.Windows.Forms.Button();
            this.txtPatientId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnTestAdd
            // 
            this.btnTestAdd.Location = new System.Drawing.Point(188, 237);
            this.btnTestAdd.Name = "btnTestAdd";
            this.btnTestAdd.Size = new System.Drawing.Size(75, 54);
            this.btnTestAdd.TabIndex = 0;
            this.btnTestAdd.Text = "test add";
            this.btnTestAdd.UseVisualStyleBackColor = true;
            this.btnTestAdd.Click += new System.EventHandler(this.btnTestAdd_Click);
            // 
            // btnTestGet
            // 
            this.btnTestGet.Location = new System.Drawing.Point(269, 237);
            this.btnTestGet.Name = "btnTestGet";
            this.btnTestGet.Size = new System.Drawing.Size(75, 54);
            this.btnTestGet.TabIndex = 1;
            this.btnTestGet.Text = "test get";
            this.btnTestGet.UseVisualStyleBackColor = true;
            this.btnTestGet.Click += new System.EventHandler(this.btnTestGet_Click);
            // 
            // btnTestUpdate
            // 
            this.btnTestUpdate.Location = new System.Drawing.Point(350, 237);
            this.btnTestUpdate.Name = "btnTestUpdate";
            this.btnTestUpdate.Size = new System.Drawing.Size(75, 54);
            this.btnTestUpdate.TabIndex = 2;
            this.btnTestUpdate.Text = "test update";
            this.btnTestUpdate.UseVisualStyleBackColor = true;
            this.btnTestUpdate.Click += new System.EventHandler(this.btnTestUpdate_Click);
            // 
            // btnTestDelete
            // 
            this.btnTestDelete.Location = new System.Drawing.Point(431, 237);
            this.btnTestDelete.Name = "btnTestDelete";
            this.btnTestDelete.Size = new System.Drawing.Size(75, 54);
            this.btnTestDelete.TabIndex = 3;
            this.btnTestDelete.Text = "test delete";
            this.btnTestDelete.UseVisualStyleBackColor = true;
            this.btnTestDelete.Click += new System.EventHandler(this.btnTestDelete_Click);
            // 
            // txtPatientId
            // 
            this.txtPatientId.Location = new System.Drawing.Point(188, 115);
            this.txtPatientId.Name = "txtPatientId";
            this.txtPatientId.Size = new System.Drawing.Size(75, 22);
            this.txtPatientId.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(119, 118);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "patient ID";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(712, 446);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPatientId);
            this.Controls.Add(this.btnTestDelete);
            this.Controls.Add(this.btnTestUpdate);
            this.Controls.Add(this.btnTestGet);
            this.Controls.Add(this.btnTestAdd);
            this.Name = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnTestAdd;
        private System.Windows.Forms.Button btnTestGet;
        private System.Windows.Forms.Button btnTestUpdate;
        private System.Windows.Forms.Button btnTestDelete;
        private System.Windows.Forms.TextBox txtPatientId;
        private System.Windows.Forms.Label label1;
    }
}

