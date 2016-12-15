namespace EVEMon.PluginViewer
{
    partial class PluginViewerForm
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
            this.lblPluginCountHeader = new System.Windows.Forms.Label();
            this.lblPluginCount = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.txtPluginInfo = new System.Windows.Forms.TextBox();
            this.lblLoadedPlugins = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblPluginCountHeader
            // 
            this.lblPluginCountHeader.AutoSize = true;
            this.lblPluginCountHeader.Location = new System.Drawing.Point(12, 12);
            this.lblPluginCountHeader.Name = "lblPluginCountHeader";
            this.lblPluginCountHeader.Size = new System.Drawing.Size(70, 13);
            this.lblPluginCountHeader.TabIndex = 0;
            this.lblPluginCountHeader.Text = "Plugin Count:";
            // 
            // lblPluginCount
            // 
            this.lblPluginCount.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPluginCount.Location = new System.Drawing.Point(93, 11);
            this.lblPluginCount.Name = "lblPluginCount";
            this.lblPluginCount.Size = new System.Drawing.Size(100, 16);
            this.lblPluginCount.TabIndex = 1;
            this.lblPluginCount.Text = "0";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(292, 346);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtPluginInfo
            // 
            this.txtPluginInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPluginInfo.Enabled = false;
            this.txtPluginInfo.Location = new System.Drawing.Point(12, 58);
            this.txtPluginInfo.Multiline = true;
            this.txtPluginInfo.Name = "txtPluginInfo";
            this.txtPluginInfo.Size = new System.Drawing.Size(355, 282);
            this.txtPluginInfo.TabIndex = 4;
            // 
            // lblLoadedPlugins
            // 
            this.lblLoadedPlugins.AutoSize = true;
            this.lblLoadedPlugins.Location = new System.Drawing.Point(12, 42);
            this.lblLoadedPlugins.Name = "lblLoadedPlugins";
            this.lblLoadedPlugins.Size = new System.Drawing.Size(80, 13);
            this.lblLoadedPlugins.TabIndex = 5;
            this.lblLoadedPlugins.Text = "Loaded Plugins";
            // 
            // PluginViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 375);
            this.Controls.Add(this.lblLoadedPlugins);
            this.Controls.Add(this.txtPluginInfo);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblPluginCount);
            this.Controls.Add(this.lblPluginCountHeader);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(394, 413);
            this.Name = "PluginViewerForm";
            this.Text = "Plugin Viewer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPluginCountHeader;
        private System.Windows.Forms.Label lblPluginCount;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtPluginInfo;
        private System.Windows.Forms.Label lblLoadedPlugins;
    }
}