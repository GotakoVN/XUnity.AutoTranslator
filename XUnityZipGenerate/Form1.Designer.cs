namespace XUnityZipGenerate
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
            this.button1 = new System.Windows.Forms.Button();
            this.textFolderZip = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textGeneratedPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textGameName = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(910, 186);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(193, 60);
            this.button1.TabIndex = 0;
            this.button1.Text = "Chọn";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textFolderZip
            // 
            this.textFolderZip.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textFolderZip.Location = new System.Drawing.Point(45, 192);
            this.textFolderZip.Name = "textFolderZip";
            this.textFolderZip.Size = new System.Drawing.Size(849, 44);
            this.textFolderZip.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 139);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(347, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Chọn thư mục chứa các file cần zip";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 284);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(215, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "Password của file zip";
            // 
            // textGeneratedPassword
            // 
            this.textGeneratedPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textGeneratedPassword.Location = new System.Drawing.Point(48, 326);
            this.textGeneratedPassword.Name = "textGeneratedPassword";
            this.textGeneratedPassword.ReadOnly = true;
            this.textGeneratedPassword.Size = new System.Drawing.Size(846, 38);
            this.textGeneratedPassword.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(48, 500);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 25);
            this.label3.TabIndex = 5;
            this.label3.Text = "Đang chờ ...";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(385, 25);
            this.label4.TabIndex = 6;
            this.label4.Text = "Tên game (nhìn log của XUnity để biết)";
            // 
            // textGameName
            // 
            this.textGameName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textGameName.Location = new System.Drawing.Point(45, 65);
            this.textGameName.Name = "textGameName";
            this.textGameName.Size = new System.Drawing.Size(849, 44);
            this.textGameName.TabIndex = 7;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(380, 392);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(330, 91);
            this.button2.TabIndex = 8;
            this.button2.Text = "Nhấn đây tạo zip và password";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1115, 554);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textGameName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textGeneratedPassword);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textFolderZip);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "XUnityZipGenerate";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

      #endregion

      private System.Windows.Forms.Button button1;
      private System.Windows.Forms.TextBox textFolderZip;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TextBox textGeneratedPassword;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.TextBox textGameName;
      private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
      private System.Windows.Forms.Button button2;
   }
}

