namespace ForexAPITester
{
    partial class LoginCreds
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLiveAPIKey = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDemoAPIKey = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTradeUserName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTradePassword = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "User Name";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(129, 23);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(193, 20);
            this.txtUsername.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(129, 49);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(193, 20);
            this.txtPassword.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Data API Key";
            // 
            // txtLiveAPIKey
            // 
            this.txtLiveAPIKey.Location = new System.Drawing.Point(130, 78);
            this.txtLiveAPIKey.Name = "txtLiveAPIKey";
            this.txtLiveAPIKey.Size = new System.Drawing.Size(192, 20);
            this.txtLiveAPIKey.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 166);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Trade API Key";
            // 
            // txtDemoAPIKey
            // 
            this.txtDemoAPIKey.Location = new System.Drawing.Point(130, 163);
            this.txtDemoAPIKey.Name = "txtDemoAPIKey";
            this.txtDemoAPIKey.Size = new System.Drawing.Size(193, 20);
            this.txtDemoAPIKey.TabIndex = 7;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(247, 226);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 111);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Trade User Name";
            // 
            // txtTradeUserName
            // 
            this.txtTradeUserName.Location = new System.Drawing.Point(129, 108);
            this.txtTradeUserName.Name = "txtTradeUserName";
            this.txtTradeUserName.Size = new System.Drawing.Size(193, 20);
            this.txtTradeUserName.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 140);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Trade Password";
            // 
            // txtTradePassword
            // 
            this.txtTradePassword.Location = new System.Drawing.Point(129, 137);
            this.txtTradePassword.Name = "txtTradePassword";
            this.txtTradePassword.PasswordChar = '*';
            this.txtTradePassword.Size = new System.Drawing.Size(193, 20);
            this.txtTradePassword.TabIndex = 12;
            // 
            // LoginCreds
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 261);
            this.Controls.Add(this.txtTradePassword);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtTradeUserName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtDemoAPIKey);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtLiveAPIKey);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginCreds";
            this.Text = "LoginCreds";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLiveAPIKey;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDemoAPIKey;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTradeUserName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtTradePassword;
    }
}