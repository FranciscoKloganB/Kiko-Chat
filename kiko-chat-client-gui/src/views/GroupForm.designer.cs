namespace kiko_chat_client_gui
{
    partial class GroupForm
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
            this.confirmButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.ipTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.ipBox = new System.Windows.Forms.TextBox();
            this.portBox = new System.Windows.Forms.TextBox();
            this.groupNameTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.groupNameBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // confirmButton
            // 
            this.confirmButton.Location = new System.Drawing.Point(52, 90);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(104, 32);
            this.confirmButton.TabIndex = 3;
            this.confirmButton.Text = "Confirm";
            this.confirmButton.UseVisualStyleBackColor = true;
            this.confirmButton.Click += new System.EventHandler(this.ConfirmGroupForm_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(177, 90);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(109, 32);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelGroupForm_Click);
            // 
            // ipBox
            // 
            this.ipBox.AcceptsReturn = true;
            this.ipBox.AcceptsTab = true;
            this.ipBox.Location = new System.Drawing.Point(12, 12);
            this.ipBox.Name = "ipBox";
            this.ipBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ipBox.Size = new System.Drawing.Size(313, 20);
            this.ipBox.TabIndex = 0;
            this.ipBox.Text = " Host server";
            this.ipTooltip.SetToolTip(this.ipBox, "IP/Port addresses are of the form x.x.x.x:y, where x is is a number with 1 to 3 d" +
        "igits and y is a number with 1 to 5 digits");
            // 
            // portBox
            // 
            this.portBox.AcceptsReturn = true;
            this.portBox.AcceptsTab = true;
            this.portBox.Location = new System.Drawing.Point(12, 38);
            this.portBox.Name = "portBox";
            this.portBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.portBox.Size = new System.Drawing.Size(313, 20);
            this.portBox.TabIndex = 1;
            this.portBox.Text = " Host port";
            this.ipTooltip.SetToolTip(this.portBox, "IP/Port addresses are of the form x.x.x.x:y, where x is is a number with 1 to 3 d" +
        "igits and y is a number with 1 to 5 digits");
            // 
            // groupNameBox
            // 
            this.groupNameBox.AcceptsReturn = true;
            this.groupNameBox.AcceptsTab = true;
            this.groupNameBox.Location = new System.Drawing.Point(12, 64);
            this.groupNameBox.Name = "groupNameBox";
            this.groupNameBox.Size = new System.Drawing.Size(313, 20);
            this.groupNameBox.TabIndex = 2;
            this.groupNameBox.Text = " Group Name";
            this.groupNameTooltip.SetToolTip(this.groupNameBox, "The group name can contain any letters from A to Z (case sensitive) and any numbe" +
        "r from 0 to 9. Up to a total of 24 characters. No spaces or special characters a" +
        "llowed.");
            // 
            // GroupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 134);
            this.Controls.Add(this.portBox);
            this.Controls.Add(this.groupNameBox);
            this.Controls.Add(this.ipBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.confirmButton);
            this.Name = "GroupForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Group Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button confirmButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ToolTip groupNameTooltip;
        private System.Windows.Forms.ToolTip ipTooltip;
        private System.Windows.Forms.TextBox ipBox;
        private System.Windows.Forms.TextBox groupNameBox;
        private System.Windows.Forms.TextBox portBox;
    }
}