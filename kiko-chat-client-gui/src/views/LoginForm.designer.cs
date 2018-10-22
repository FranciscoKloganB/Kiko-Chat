namespace kiko_chat_client_gui
{
    partial class LoginForm
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
            this.nickNameBox = new System.Windows.Forms.TextBox();
            this.clientAddress = new System.Windows.Forms.TextBox();
            this.fullNameBox = new System.Windows.Forms.TextBox();
            this.countryBox = new System.Windows.Forms.TextBox();
            this.emailBox = new System.Windows.Forms.TextBox();
            this.createSettingsButton = new System.Windows.Forms.Button();
            this.usePreviousLoginButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // nickNameBox
            // 
            this.nickNameBox.AcceptsReturn = true;
            this.nickNameBox.AcceptsTab = true;
            this.nickNameBox.ForeColor = System.Drawing.Color.Silver;
            this.nickNameBox.Location = new System.Drawing.Point(12, 53);
            this.nickNameBox.Name = "nickNameBox";
            this.nickNameBox.Size = new System.Drawing.Size(324, 20);
            this.nickNameBox.TabIndex = 2;
            this.nickNameBox.Text = " Nickname *";
            this.nickNameBox.Enter += new System.EventHandler(this.nickNameBox_Enter);
            this.nickNameBox.Leave += new System.EventHandler(this.nickNameBox_Leave);
            // 
            // clientAddress
            // 
            this.clientAddress.ForeColor = System.Drawing.Color.Silver;
            this.clientAddress.Location = new System.Drawing.Point(12, 27);
            this.clientAddress.Name = "clientAddress";
            this.clientAddress.ReadOnly = true;
            this.clientAddress.Size = new System.Drawing.Size(324, 20);
            this.clientAddress.TabIndex = 0;
            this.clientAddress.Text = " Internet Location";
            this.clientAddress.Enter += new System.EventHandler(this.clientAddress_Enter);
            this.clientAddress.Leave += new System.EventHandler(this.clientAddress_Leave);
            // 
            // fullNameBox
            // 
            this.fullNameBox.AcceptsReturn = true;
            this.fullNameBox.AcceptsTab = true;
            this.fullNameBox.ForeColor = System.Drawing.Color.Silver;
            this.fullNameBox.Location = new System.Drawing.Point(12, 79);
            this.fullNameBox.Name = "fullNameBox";
            this.fullNameBox.Size = new System.Drawing.Size(324, 20);
            this.fullNameBox.TabIndex = 3;
            this.fullNameBox.Text = " Full Name";
            this.fullNameBox.Enter += new System.EventHandler(this.fullNameBox_Enter);
            this.fullNameBox.Leave += new System.EventHandler(this.fullNameBox_Leave);
            // 
            // countryBox
            // 
            this.countryBox.AcceptsReturn = true;
            this.countryBox.AcceptsTab = true;
            this.countryBox.ForeColor = System.Drawing.Color.Silver;
            this.countryBox.Location = new System.Drawing.Point(12, 131);
            this.countryBox.Name = "countryBox";
            this.countryBox.Size = new System.Drawing.Size(324, 20);
            this.countryBox.TabIndex = 5;
            this.countryBox.Text = " Country";
            this.countryBox.Enter += new System.EventHandler(this.countryBox_Enter);
            this.countryBox.Leave += new System.EventHandler(this.countryBox_Leave);
            // 
            // emailBox
            // 
            this.emailBox.AcceptsReturn = true;
            this.emailBox.AcceptsTab = true;
            this.emailBox.ForeColor = System.Drawing.Color.Silver;
            this.emailBox.Location = new System.Drawing.Point(12, 105);
            this.emailBox.Name = "emailBox";
            this.emailBox.Size = new System.Drawing.Size(324, 20);
            this.emailBox.TabIndex = 4;
            this.emailBox.Text = " Email";
            this.emailBox.Enter += new System.EventHandler(this.emailBox_Enter);
            this.emailBox.Leave += new System.EventHandler(this.emailBox_Leave);
            // 
            // createSettingsButton
            // 
            this.createSettingsButton.Location = new System.Drawing.Point(29, 157);
            this.createSettingsButton.Name = "createSettingsButton";
            this.createSettingsButton.Size = new System.Drawing.Size(147, 23);
            this.createSettingsButton.TabIndex = 6;
            this.createSettingsButton.Text = "Accept new settings";
            this.createSettingsButton.UseVisualStyleBackColor = true;
            this.createSettingsButton.Click += new System.EventHandler(this.AcceptNewSetting_Click);
            // 
            // usePreviousLoginButton
            // 
            this.usePreviousLoginButton.Location = new System.Drawing.Point(182, 157);
            this.usePreviousLoginButton.Name = "usePreviousLoginButton";
            this.usePreviousLoginButton.Size = new System.Drawing.Size(132, 23);
            this.usePreviousLoginButton.TabIndex = 7;
            this.usePreviousLoginButton.Text = "Use previous settings";
            this.usePreviousLoginButton.UseVisualStyleBackColor = true;
            this.usePreviousLoginButton.Click += new System.EventHandler(this.UsePreviousSettings_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 201);
            this.Controls.Add(this.usePreviousLoginButton);
            this.Controls.Add(this.createSettingsButton);
            this.Controls.Add(this.emailBox);
            this.Controls.Add(this.countryBox);
            this.Controls.Add(this.fullNameBox);
            this.Controls.Add(this.clientAddress);
            this.Controls.Add(this.nickNameBox);
            this.Name = "LoginForm";
            this.Text = "Login";
            this.Load += new System.EventHandler(this.loginForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox nickNameBox;
        private System.Windows.Forms.TextBox clientAddress;
        private System.Windows.Forms.TextBox fullNameBox;
        private System.Windows.Forms.TextBox countryBox;
        private System.Windows.Forms.TextBox emailBox;
        private System.Windows.Forms.Button createSettingsButton;
        private System.Windows.Forms.Button usePreviousLoginButton;
    }
}