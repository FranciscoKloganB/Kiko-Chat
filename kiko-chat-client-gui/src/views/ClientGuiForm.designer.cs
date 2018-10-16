namespace kiko_chat_client_gui
{
    partial class ClientGuiForm
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
            this.components = new System.ComponentModel.Container();
            this.newMessageBox = new System.Windows.Forms.TextBox();
            this.sendMsgButton = new System.Windows.Forms.Button();
            this.deleteMsgButton = new System.Windows.Forms.Button();
            this.chatMembersBox = new System.Windows.Forms.TextBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.connectTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.disconnectTootlip = new System.Windows.Forms.ToolTip(this.components);
            this.createGroupTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.createGroupButton = new System.Windows.Forms.Button();
            this.groupSelectorBox = new System.Windows.Forms.ComboBox();
            this.groupSelectorTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.joinGroupButton = new System.Windows.Forms.Button();
            this.joinAGroupTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.leaveGroupButton = new System.Windows.Forms.Button();
            this.leaveGroupTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.chatWindow = new System.Windows.Forms.RichTextBox();
            this.changeNickNameTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.applyNickNameChangeButton = new System.Windows.Forms.Button();
            this.nickNameChangeBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // newMessageBox
            // 
            this.newMessageBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newMessageBox.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.newMessageBox.Location = new System.Drawing.Point(34, 604);
            this.newMessageBox.Multiline = true;
            this.newMessageBox.Name = "newMessageBox";
            this.newMessageBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.newMessageBox.Size = new System.Drawing.Size(590, 82);
            this.newMessageBox.TabIndex = 0;
            this.newMessageBox.Text = " Write your message here";
            // 
            // sendMsgButton
            // 
            this.sendMsgButton.Location = new System.Drawing.Point(672, 615);
            this.sendMsgButton.Name = "sendMsgButton";
            this.sendMsgButton.Size = new System.Drawing.Size(118, 33);
            this.sendMsgButton.TabIndex = 1;
            this.sendMsgButton.Text = "Send";
            this.sendMsgButton.UseVisualStyleBackColor = true;
            this.sendMsgButton.Click += new System.EventHandler(this.SendMessageButton_Click);
            // 
            // deleteMsgButton
            // 
            this.deleteMsgButton.Location = new System.Drawing.Point(672, 654);
            this.deleteMsgButton.Name = "deleteMsgButton";
            this.deleteMsgButton.Size = new System.Drawing.Size(118, 32);
            this.deleteMsgButton.TabIndex = 2;
            this.deleteMsgButton.Text = "Delete Message";
            this.deleteMsgButton.UseVisualStyleBackColor = true;
            this.deleteMsgButton.Click += new System.EventHandler(this.DeleteNewMessageButton_Click);
            // 
            // chatMembersBox
            // 
            this.chatMembersBox.Location = new System.Drawing.Point(831, 166);
            this.chatMembersBox.Multiline = true;
            this.chatMembersBox.Name = "chatMembersBox";
            this.chatMembersBox.ReadOnly = true;
            this.chatMembersBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.chatMembersBox.ShortcutsEnabled = false;
            this.chatMembersBox.Size = new System.Drawing.Size(221, 520);
            this.chatMembersBox.TabIndex = 9;
            this.chatMembersBox.TabStop = false;
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(831, 50);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(107, 23);
            this.connectButton.TabIndex = 4;
            this.connectButton.Text = "Connect";
            this.connectTooltip.SetToolTip(this.connectButton, "Connect to selected chat group");
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // disconnectButton
            // 
            this.disconnectButton.Location = new System.Drawing.Point(945, 50);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(107, 23);
            this.disconnectButton.TabIndex = 5;
            this.disconnectButton.Text = "Disconnect";
            this.disconnectTootlip.SetToolTip(this.disconnectButton, "Disconnect from selected chat group");
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // createGroupButton
            // 
            this.createGroupButton.Location = new System.Drawing.Point(874, 79);
            this.createGroupButton.Name = "createGroupButton";
            this.createGroupButton.Size = new System.Drawing.Size(146, 23);
            this.createGroupButton.TabIndex = 6;
            this.createGroupButton.Text = "Create new group";
            this.createGroupTooltip.SetToolTip(this.createGroupButton, "Create a chat group that other users can join");
            this.createGroupButton.UseVisualStyleBackColor = true;
            this.createGroupButton.Click += new System.EventHandler(this.CreateGroupButton_Click);
            // 
            // groupSelectorBox
            // 
            this.groupSelectorBox.FormattingEnabled = true;
            this.groupSelectorBox.Location = new System.Drawing.Point(831, 23);
            this.groupSelectorBox.Name = "groupSelectorBox";
            this.groupSelectorBox.Size = new System.Drawing.Size(221, 21);
            this.groupSelectorBox.TabIndex = 3;
            this.groupSelectorTooltip.SetToolTip(this.groupSelectorBox, "This menu displays all chat groups you are currently on.");
            // 
            // joinGroupButton
            // 
            this.joinGroupButton.Location = new System.Drawing.Point(874, 108);
            this.joinGroupButton.Name = "joinGroupButton";
            this.joinGroupButton.Size = new System.Drawing.Size(146, 23);
            this.joinGroupButton.TabIndex = 7;
            this.joinGroupButton.Text = "Join a group";
            this.joinAGroupTooltip.SetToolTip(this.joinGroupButton, "Join a group created by another user");
            this.joinGroupButton.UseVisualStyleBackColor = true;
            this.joinGroupButton.Click += new System.EventHandler(this.JoinNewGroupButton_Click);
            // 
            // leaveGroupButton
            // 
            this.leaveGroupButton.Location = new System.Drawing.Point(874, 137);
            this.leaveGroupButton.Name = "leaveGroupButton";
            this.leaveGroupButton.Size = new System.Drawing.Size(146, 23);
            this.leaveGroupButton.TabIndex = 8;
            this.leaveGroupButton.Text = "Leave a group";
            this.leaveGroupTooltip.SetToolTip(this.leaveGroupButton, "Leaves the selected group");
            this.leaveGroupButton.UseVisualStyleBackColor = true;
            this.leaveGroupButton.Click += new System.EventHandler(this.LeaveGroupButton_Click);
            // 
            // chatWindow
            // 
            this.chatWindow.Location = new System.Drawing.Point(34, 94);
            this.chatWindow.Name = "chatWindow";
            this.chatWindow.Size = new System.Drawing.Size(756, 491);
            this.chatWindow.TabIndex = 12;
            this.chatWindow.TabStop = false;
            this.chatWindow.Text = "";
            // 
            // applyNickNameChangeButton
            // 
            this.applyNickNameChangeButton.Location = new System.Drawing.Point(34, 50);
            this.applyNickNameChangeButton.Name = "applyNickNameChangeButton";
            this.applyNickNameChangeButton.Size = new System.Drawing.Size(101, 23);
            this.applyNickNameChangeButton.TabIndex = 11;
            this.applyNickNameChangeButton.TabStop = false;
            this.applyNickNameChangeButton.Text = "Apply";
            this.applyNickNameChangeButton.UseVisualStyleBackColor = true;
            this.applyNickNameChangeButton.Click += new System.EventHandler(this.applyNickNameChangeButton_Click);
            // 
            // nickNameChangeBox
            // 
            this.nickNameChangeBox.AcceptsReturn = true;
            this.nickNameChangeBox.AcceptsTab = true;
            this.nickNameChangeBox.Location = new System.Drawing.Point(34, 23);
            this.nickNameChangeBox.Name = "nickNameChangeBox";
            this.nickNameChangeBox.Size = new System.Drawing.Size(214, 20);
            this.nickNameChangeBox.TabIndex = 10;
            this.nickNameChangeBox.Text = " New nickname";
            this.nickNameChangeBox.Enter += new System.EventHandler(this.nickNameChangeBox_Enter);
            this.nickNameChangeBox.Leave += new System.EventHandler(this.nickNameChangeBox_Leave);
            // 
            // ClientGuiForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1088, 711);
            this.Controls.Add(this.nickNameChangeBox);
            this.Controls.Add(this.applyNickNameChangeButton);
            this.Controls.Add(this.chatWindow);
            this.Controls.Add(this.leaveGroupButton);
            this.Controls.Add(this.joinGroupButton);
            this.Controls.Add(this.groupSelectorBox);
            this.Controls.Add(this.createGroupButton);
            this.Controls.Add(this.disconnectButton);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.chatMembersBox);
            this.Controls.Add(this.deleteMsgButton);
            this.Controls.Add(this.sendMsgButton);
            this.Controls.Add(this.newMessageBox);
            this.Name = "ClientGuiForm";
            this.Text = "Kiko\'s Chat v0.1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ClientGuiForm_FormClosed);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox newMessageBox;
        private System.Windows.Forms.Button sendMsgButton;
        private System.Windows.Forms.Button deleteMsgButton;
        private System.Windows.Forms.TextBox chatMembersBox;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.ToolTip connectTooltip;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.ToolTip disconnectTootlip;
        private System.Windows.Forms.ToolTip createGroupTooltip;
        private System.Windows.Forms.Button createGroupButton;
        private System.Windows.Forms.ComboBox groupSelectorBox;
        private System.Windows.Forms.ToolTip groupSelectorTooltip;
        private System.Windows.Forms.Button joinGroupButton;
        private System.Windows.Forms.ToolTip joinAGroupTooltip;
        private System.Windows.Forms.Button leaveGroupButton;
        private System.Windows.Forms.ToolTip leaveGroupTooltip;
        private System.Windows.Forms.RichTextBox chatWindow;
        private System.Windows.Forms.ToolTip changeNickNameTooltip;
        private System.Windows.Forms.Button applyNickNameChangeButton;
        private System.Windows.Forms.TextBox nickNameChangeBox;
    }
}

