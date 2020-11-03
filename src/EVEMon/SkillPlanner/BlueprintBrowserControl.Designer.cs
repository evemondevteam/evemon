using EVEMon.Common.Enumerations;

namespace EVEMon.SkillPlanner
{
    partial class BlueprintBrowserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gbRequiredSkills = new System.Windows.Forms.GroupBox();
            this.requiredSkillsControl = new EVEMon.SkillPlanner.RequiredSkillsControl();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tpManufacturing = new System.Windows.Forms.TabPage();
            this.gbManufBoM = new System.Windows.Forms.GroupBox();
            this.lvManufacturing = new System.Windows.Forms.ListView();
            this.chManufItem = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chManufQuantityBase = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chManufQuantityYou = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BlueprintAttributeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showInMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showInMenuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.exportToCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblNoItemManufacturing = new System.Windows.Forms.Label();
            this.tpResearchME = new System.Windows.Forms.TabPage();
            this.gbResearchMEBoM = new System.Windows.Forms.GroupBox();
            this.lvResearchME = new System.Windows.Forms.ListView();
            this.lblNoItemME = new System.Windows.Forms.Label();
            this.tpResearchTE = new System.Windows.Forms.TabPage();
            this.gbResearchTEBoM = new System.Windows.Forms.GroupBox();
            this.lvResearchTE = new System.Windows.Forms.ListView();
            this.lblNoItemTE = new System.Windows.Forms.Label();
            this.tpCopying = new System.Windows.Forms.TabPage();
            this.gbCopyBoM = new System.Windows.Forms.GroupBox();
            this.lvCopying = new System.Windows.Forms.ListView();
            this.lblNoItemCopy = new System.Windows.Forms.Label();
            this.tpInvention = new System.Windows.Forms.TabPage();
            this.gbInvBoM = new System.Windows.Forms.GroupBox();
            this.lvInvention = new System.Windows.Forms.ListView();
            this.lblNoItemInvention = new System.Windows.Forms.Label();
            this.tpReactions = new System.Windows.Forms.TabPage();
            this.tlpModifiers = new System.Windows.Forms.TableLayoutPanel();
            this.flpUpper = new System.Windows.Forms.FlowLayoutPanel();
            this.lblMaterialEfficiency = new System.Windows.Forms.Label();
            this.nudME = new System.Windows.Forms.NumericUpDown();
            this.lblTimeEfficiency = new System.Windows.Forms.Label();
            this.nudTE = new System.Windows.Forms.NumericUpDown();
            this.flpLower = new System.Windows.Forms.FlowLayoutPanel();
            this.cbFacility = new System.Windows.Forms.ComboBox();
            this.cbImplantSet = new System.Windows.Forms.ComboBox();
            this.blueprintSelectControl = new EVEMon.SkillPlanner.BlueprintSelectControl();
            this.lblProduce = new System.Windows.Forms.Label();
            this.lblProductionLimit = new System.Windows.Forms.Label();
            this.lblProductionTime = new System.Windows.Forms.Label();
            this.lblItem = new System.Windows.Forms.Label();
            this.lblRunsPerCopy = new System.Windows.Forms.Label();
            this.lblProductionBaseTime = new System.Windows.Forms.Label();
            this.lblProductionCharTime = new System.Windows.Forms.Label();
            this.lblResearchMECharTime = new System.Windows.Forms.Label();
            this.lblResearchMEBaseTime = new System.Windows.Forms.Label();
            this.lblResearchME = new System.Windows.Forms.Label();
            this.lblResearchCopyCharTime = new System.Windows.Forms.Label();
            this.lblResearchCopyBaseTime = new System.Windows.Forms.Label();
            this.lblResearchCopy = new System.Windows.Forms.Label();
            this.lblResearchTECharTime = new System.Windows.Forms.Label();
            this.lblResearchTEBaseTime = new System.Windows.Forms.Label();
            this.lblResearchTE = new System.Windows.Forms.Label();
            this.lblInvention = new System.Windows.Forms.Label();
            this.gbGeneralInfo = new System.Windows.Forms.GroupBox();
            this.lblSuccessProbability = new System.Windows.Forms.Label();
            this.lblProbability = new System.Windows.Forms.Label();
            this.InventBlueprintListBox = new System.Windows.Forms.ListBox();
            this.gbResearching = new System.Windows.Forms.GroupBox();
            this.gbManufacturing = new System.Windows.Forms.GroupBox();
            this.pnlAttributes = new System.Windows.Forms.Panel();
            this.gbReactions = new System.Windows.Forms.GroupBox();
            this.lblReactionCharTime = new System.Windows.Forms.Label();
            this.lblReactionTime = new System.Windows.Forms.Label();
            this.lblReactionBaseTime = new System.Windows.Forms.Label();
            this.gbInvention = new System.Windows.Forms.GroupBox();
            this.lblInventionCharTime = new System.Windows.Forms.Label();
            this.lblInventionTime = new System.Windows.Forms.Label();
            this.lblInventionBaseTime = new System.Windows.Forms.Label();
            this.gbReactionsBoM = new System.Windows.Forms.GroupBox();
            this.lvReactions = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.scObjectBrowser)).BeginInit();
            this.scObjectBrowser.Panel1.SuspendLayout();
            this.scObjectBrowser.Panel2.SuspendLayout();
            this.scObjectBrowser.SuspendLayout();
            this.pnlDetails.SuspendLayout();
            this.pnlBrowserHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scDetailsRight)).BeginInit();
            this.scDetailsRight.Panel1.SuspendLayout();
            this.scDetailsRight.Panel2.SuspendLayout();
            this.scDetailsRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scDetails)).BeginInit();
            this.scDetails.Panel1.SuspendLayout();
            this.scDetails.Panel2.SuspendLayout();
            this.scDetails.SuspendLayout();
            this.gbDescription.SuspendLayout();
            this.gbRequiredSkills.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tpManufacturing.SuspendLayout();
            this.gbManufBoM.SuspendLayout();
            this.BlueprintAttributeContextMenu.SuspendLayout();
            this.tpResearchME.SuspendLayout();
            this.gbResearchMEBoM.SuspendLayout();
            this.tpResearchTE.SuspendLayout();
            this.gbResearchTEBoM.SuspendLayout();
            this.tpCopying.SuspendLayout();
            this.gbCopyBoM.SuspendLayout();
            this.tpInvention.SuspendLayout();
            this.gbInvBoM.SuspendLayout();
            this.tpReactions.SuspendLayout();
            this.tlpModifiers.SuspendLayout();
            this.flpUpper.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudME)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTE)).BeginInit();
            this.flpLower.SuspendLayout();
            this.gbGeneralInfo.SuspendLayout();
            this.gbResearching.SuspendLayout();
            this.gbManufacturing.SuspendLayout();
            this.pnlAttributes.SuspendLayout();
            this.gbReactions.SuspendLayout();
            this.gbInvention.SuspendLayout();
            this.gbReactionsBoM.SuspendLayout();
            this.SuspendLayout();
            // 
            // scObjectBrowser
            // 
            // 
            // scObjectBrowser.Panel1
            // 
            this.scObjectBrowser.Panel1.Controls.Add(this.blueprintSelectControl);
            this.scObjectBrowser.Size = new System.Drawing.Size(1034, 700);
            // 
            // pnlDetails
            // 
            this.pnlDetails.Size = new System.Drawing.Size(866, 630);
            this.pnlDetails.TabIndex = 1;
            // 
            // pnlBrowserHeader
            // 
            this.pnlBrowserHeader.Size = new System.Drawing.Size(866, 70);
            // 
            // scDetailsRight
            // 
            // 
            // scDetailsRight.Panel2
            // 
            this.scDetailsRight.Panel2.Controls.Add(this.gbRequiredSkills);
            this.scDetailsRight.Size = new System.Drawing.Size(240, 630);
            this.scDetailsRight.SplitterDistance = 522;
            // 
            // scDetails
            // 
            // 
            // scDetails.Panel1
            // 
            this.scDetails.Panel1.Controls.Add(this.tabControl);
            this.scDetails.Panel1.Controls.Add(this.tlpModifiers);
            this.scDetails.Size = new System.Drawing.Size(866, 630);
            this.scDetails.SplitterDistance = 622;
            // 
            // gbDescription
            // 
            this.gbDescription.Controls.Add(this.pnlAttributes);
            this.gbDescription.Size = new System.Drawing.Size(240, 522);
            this.gbDescription.Controls.SetChildIndex(this.tbDescription, 0);
            this.gbDescription.Controls.SetChildIndex(this.pnlAttributes, 0);
            // 
            // tbDescription
            // 
            this.tbDescription.BackColor = System.Drawing.SystemColors.Control;
            this.tbDescription.Size = new System.Drawing.Size(234, 503);
            // 
            // gbRequiredSkills
            // 
            this.gbRequiredSkills.Controls.Add(this.requiredSkillsControl);
            this.gbRequiredSkills.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbRequiredSkills.Location = new System.Drawing.Point(0, 0);
            this.gbRequiredSkills.Name = "gbRequiredSkills";
            this.gbRequiredSkills.Size = new System.Drawing.Size(240, 104);
            this.gbRequiredSkills.TabIndex = 0;
            this.gbRequiredSkills.TabStop = false;
            this.gbRequiredSkills.Text = "Required Skills";
            // 
            // requiredSkillsControl
            // 
            this.requiredSkillsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.requiredSkillsControl.Location = new System.Drawing.Point(3, 16);
            this.requiredSkillsControl.MinimumSize = new System.Drawing.Size(187, 0);
            this.requiredSkillsControl.Name = "requiredSkillsControl";
            this.requiredSkillsControl.Size = new System.Drawing.Size(234, 85);
            this.requiredSkillsControl.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tpManufacturing);
            this.tabControl.Controls.Add(this.tpResearchME);
            this.tabControl.Controls.Add(this.tpResearchTE);
            this.tabControl.Controls.Add(this.tpCopying);
            this.tabControl.Controls.Add(this.tpInvention);
            this.tabControl.Controls.Add(this.tpReactions);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.HotTrack = true;
            this.tabControl.Location = new System.Drawing.Point(0, 65);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(622, 565);
            this.tabControl.TabIndex = 1;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tpManufacturing
            // 
            this.tpManufacturing.Controls.Add(this.gbManufBoM);
            this.tpManufacturing.Location = new System.Drawing.Point(4, 22);
            this.tpManufacturing.Name = "tpManufacturing";
            this.tpManufacturing.Padding = new System.Windows.Forms.Padding(3);
            this.tpManufacturing.Size = new System.Drawing.Size(614, 539);
            this.tpManufacturing.TabIndex = 0;
            this.tpManufacturing.Text = "Manufacturing";
            this.tpManufacturing.UseVisualStyleBackColor = true;
            // 
            // gbManufBoM
            // 
            this.gbManufBoM.Controls.Add(this.lvManufacturing);
            this.gbManufBoM.Controls.Add(this.lblNoItemManufacturing);
            this.gbManufBoM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbManufBoM.Location = new System.Drawing.Point(3, 3);
            this.gbManufBoM.Name = "gbManufBoM";
            this.gbManufBoM.Size = new System.Drawing.Size(608, 533);
            this.gbManufBoM.TabIndex = 1;
            this.gbManufBoM.TabStop = false;
            this.gbManufBoM.Text = "Bill of Materials";
            // 
            // lvManufacturing
            // 
            this.lvManufacturing.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chManufItem,
            this.chManufQuantityBase,
            this.chManufQuantityYou});
            this.lvManufacturing.ContextMenuStrip = this.BlueprintAttributeContextMenu;
            this.lvManufacturing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvManufacturing.FullRowSelect = true;
            this.lvManufacturing.HideSelection = false;
            this.lvManufacturing.Location = new System.Drawing.Point(3, 16);
            this.lvManufacturing.MultiSelect = false;
            this.lvManufacturing.Name = "lvManufacturing";
            this.lvManufacturing.Size = new System.Drawing.Size(602, 514);
            this.lvManufacturing.TabIndex = 0;
            this.lvManufacturing.UseCompatibleStateImageBehavior = false;
            this.lvManufacturing.View = System.Windows.Forms.View.Details;
            this.lvManufacturing.DoubleClick += new System.EventHandler(this.propertiesList_DoubleClick);
            // 
            // chManufItem
            // 
            this.chManufItem.Text = "Item";
            this.chManufItem.Width = 120;
            // 
            // chManufQuantityBase
            // 
            this.chManufQuantityBase.Text = "Quantity (Base)";
            this.chManufQuantityBase.Width = 86;
            // 
            // chManufQuantityYou
            // 
            this.chManufQuantityYou.Text = "Quantity (You)";
            this.chManufQuantityYou.Width = 82;
            // 
            // BlueprintAttributeContextMenu
            // 
            this.BlueprintAttributeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showInMenuItem,
            this.showInMenuSeparator,
            this.exportToCSVToolStripMenuItem});
            this.BlueprintAttributeContextMenu.Name = "ShipPropertiesContextMenu";
            this.BlueprintAttributeContextMenu.Size = new System.Drawing.Size(189, 54);
            this.BlueprintAttributeContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.BlueprintAttributeContextMenu_Opening);
            // 
            // showInMenuItem
            // 
            this.showInMenuItem.Name = "showInMenuItem";
            this.showInMenuItem.Size = new System.Drawing.Size(188, 22);
            this.showInMenuItem.Text = "Show In Item Browser";
            this.showInMenuItem.Click += new System.EventHandler(this.propertiesList_DoubleClick);
            // 
            // showInMenuSeparator
            // 
            this.showInMenuSeparator.Name = "showInMenuSeparator";
            this.showInMenuSeparator.Size = new System.Drawing.Size(185, 6);
            // 
            // exportToCSVToolStripMenuItem
            // 
            this.exportToCSVToolStripMenuItem.Name = "exportToCSVToolStripMenuItem";
            this.exportToCSVToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.exportToCSVToolStripMenuItem.Text = "Export To CSV...";
            this.exportToCSVToolStripMenuItem.Click += new System.EventHandler(this.exportToCSVToolStripMenuItem_Click);
            // 
            // lblNoItemManufacturing
            // 
            this.lblNoItemManufacturing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNoItemManufacturing.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblNoItemManufacturing.Location = new System.Drawing.Point(3, 16);
            this.lblNoItemManufacturing.Name = "lblNoItemManufacturing";
            this.lblNoItemManufacturing.Size = new System.Drawing.Size(602, 514);
            this.lblNoItemManufacturing.TabIndex = 4;
            this.lblNoItemManufacturing.Text = "No item required.";
            this.lblNoItemManufacturing.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tpResearchME
            // 
            this.tpResearchME.Controls.Add(this.gbResearchMEBoM);
            this.tpResearchME.Location = new System.Drawing.Point(4, 22);
            this.tpResearchME.Name = "tpResearchME";
            this.tpResearchME.Padding = new System.Windows.Forms.Padding(3);
            this.tpResearchME.Size = new System.Drawing.Size(230, 199);
            this.tpResearchME.TabIndex = 1;
            this.tpResearchME.Text = "Researching Material Efficiency";
            this.tpResearchME.UseVisualStyleBackColor = true;
            // 
            // gbResearchMEBoM
            // 
            this.gbResearchMEBoM.Controls.Add(this.lvResearchME);
            this.gbResearchMEBoM.Controls.Add(this.lblNoItemME);
            this.gbResearchMEBoM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbResearchMEBoM.Location = new System.Drawing.Point(3, 3);
            this.gbResearchMEBoM.Name = "gbResearchMEBoM";
            this.gbResearchMEBoM.Size = new System.Drawing.Size(224, 193);
            this.gbResearchMEBoM.TabIndex = 1;
            this.gbResearchMEBoM.TabStop = false;
            this.gbResearchMEBoM.Text = "Bill of Materials";
            // 
            // lvResearchME
            // 
            this.lvResearchME.ContextMenuStrip = this.BlueprintAttributeContextMenu;
            this.lvResearchME.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvResearchME.Location = new System.Drawing.Point(3, 16);
            this.lvResearchME.MultiSelect = false;
            this.lvResearchME.Name = "lvResearchME";
            this.lvResearchME.Size = new System.Drawing.Size(218, 174);
            this.lvResearchME.TabIndex = 1;
            this.lvResearchME.UseCompatibleStateImageBehavior = false;
            this.lvResearchME.View = System.Windows.Forms.View.Details;
            this.lvResearchME.DoubleClick += new System.EventHandler(this.propertiesList_DoubleClick);
            // 
            // lblNoItemME
            // 
            this.lblNoItemME.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNoItemME.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblNoItemME.Location = new System.Drawing.Point(3, 16);
            this.lblNoItemME.Name = "lblNoItemME";
            this.lblNoItemME.Size = new System.Drawing.Size(218, 174);
            this.lblNoItemME.TabIndex = 2;
            this.lblNoItemME.Text = "No item required.";
            this.lblNoItemME.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tpResearchTE
            // 
            this.tpResearchTE.Controls.Add(this.gbResearchTEBoM);
            this.tpResearchTE.Location = new System.Drawing.Point(4, 22);
            this.tpResearchTE.Name = "tpResearchTE";
            this.tpResearchTE.Padding = new System.Windows.Forms.Padding(3);
            this.tpResearchTE.Size = new System.Drawing.Size(230, 199);
            this.tpResearchTE.TabIndex = 4;
            this.tpResearchTE.Text = "Researching Time Efficiency";
            this.tpResearchTE.UseVisualStyleBackColor = true;
            // 
            // gbResearchTEBoM
            // 
            this.gbResearchTEBoM.Controls.Add(this.lvResearchTE);
            this.gbResearchTEBoM.Controls.Add(this.lblNoItemTE);
            this.gbResearchTEBoM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbResearchTEBoM.Location = new System.Drawing.Point(3, 3);
            this.gbResearchTEBoM.Name = "gbResearchTEBoM";
            this.gbResearchTEBoM.Size = new System.Drawing.Size(224, 193);
            this.gbResearchTEBoM.TabIndex = 1;
            this.gbResearchTEBoM.TabStop = false;
            this.gbResearchTEBoM.Text = "Bill of Materials";
            // 
            // lvResearchTE
            // 
            this.lvResearchTE.ContextMenuStrip = this.BlueprintAttributeContextMenu;
            this.lvResearchTE.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvResearchTE.Location = new System.Drawing.Point(3, 16);
            this.lvResearchTE.MultiSelect = false;
            this.lvResearchTE.Name = "lvResearchTE";
            this.lvResearchTE.Size = new System.Drawing.Size(218, 174);
            this.lvResearchTE.TabIndex = 1;
            this.lvResearchTE.UseCompatibleStateImageBehavior = false;
            this.lvResearchTE.View = System.Windows.Forms.View.Details;
            this.lvResearchTE.DoubleClick += new System.EventHandler(this.propertiesList_DoubleClick);
            // 
            // lblNoItemTE
            // 
            this.lblNoItemTE.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNoItemTE.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblNoItemTE.Location = new System.Drawing.Point(3, 16);
            this.lblNoItemTE.Name = "lblNoItemTE";
            this.lblNoItemTE.Size = new System.Drawing.Size(218, 174);
            this.lblNoItemTE.TabIndex = 2;
            this.lblNoItemTE.Text = "No item required.";
            this.lblNoItemTE.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tpCopying
            // 
            this.tpCopying.Controls.Add(this.gbCopyBoM);
            this.tpCopying.Location = new System.Drawing.Point(4, 22);
            this.tpCopying.Name = "tpCopying";
            this.tpCopying.Padding = new System.Windows.Forms.Padding(3);
            this.tpCopying.Size = new System.Drawing.Size(230, 199);
            this.tpCopying.TabIndex = 3;
            this.tpCopying.Text = "Copying";
            this.tpCopying.UseVisualStyleBackColor = true;
            // 
            // gbCopyBoM
            // 
            this.gbCopyBoM.Controls.Add(this.lvCopying);
            this.gbCopyBoM.Controls.Add(this.lblNoItemCopy);
            this.gbCopyBoM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbCopyBoM.Location = new System.Drawing.Point(3, 3);
            this.gbCopyBoM.Name = "gbCopyBoM";
            this.gbCopyBoM.Size = new System.Drawing.Size(224, 193);
            this.gbCopyBoM.TabIndex = 1;
            this.gbCopyBoM.TabStop = false;
            this.gbCopyBoM.Text = "Bill of Materials";
            // 
            // lvCopying
            // 
            this.lvCopying.ContextMenuStrip = this.BlueprintAttributeContextMenu;
            this.lvCopying.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvCopying.Location = new System.Drawing.Point(3, 16);
            this.lvCopying.MultiSelect = false;
            this.lvCopying.Name = "lvCopying";
            this.lvCopying.Size = new System.Drawing.Size(218, 174);
            this.lvCopying.TabIndex = 1;
            this.lvCopying.UseCompatibleStateImageBehavior = false;
            this.lvCopying.View = System.Windows.Forms.View.Details;
            this.lvCopying.DoubleClick += new System.EventHandler(this.propertiesList_DoubleClick);
            // 
            // lblNoItemCopy
            // 
            this.lblNoItemCopy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNoItemCopy.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblNoItemCopy.Location = new System.Drawing.Point(3, 16);
            this.lblNoItemCopy.Name = "lblNoItemCopy";
            this.lblNoItemCopy.Size = new System.Drawing.Size(218, 174);
            this.lblNoItemCopy.TabIndex = 3;
            this.lblNoItemCopy.Text = "No item required.";
            this.lblNoItemCopy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tpInvention
            // 
            this.tpInvention.Controls.Add(this.gbInvBoM);
            this.tpInvention.Location = new System.Drawing.Point(4, 22);
            this.tpInvention.Name = "tpInvention";
            this.tpInvention.Padding = new System.Windows.Forms.Padding(3);
            this.tpInvention.Size = new System.Drawing.Size(230, 199);
            this.tpInvention.TabIndex = 2;
            this.tpInvention.Text = "Invention";
            this.tpInvention.UseVisualStyleBackColor = true;
            // 
            // gbInvBoM
            // 
            this.gbInvBoM.Controls.Add(this.lvInvention);
            this.gbInvBoM.Controls.Add(this.lblNoItemInvention);
            this.gbInvBoM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbInvBoM.Location = new System.Drawing.Point(3, 3);
            this.gbInvBoM.Name = "gbInvBoM";
            this.gbInvBoM.Size = new System.Drawing.Size(224, 193);
            this.gbInvBoM.TabIndex = 0;
            this.gbInvBoM.TabStop = false;
            this.gbInvBoM.Text = "Bill of Materials";
            // 
            // lvInvention
            // 
            this.lvInvention.ContextMenuStrip = this.BlueprintAttributeContextMenu;
            this.lvInvention.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvInvention.Location = new System.Drawing.Point(3, 16);
            this.lvInvention.MultiSelect = false;
            this.lvInvention.Name = "lvInvention";
            this.lvInvention.Size = new System.Drawing.Size(218, 174);
            this.lvInvention.TabIndex = 2;
            this.lvInvention.UseCompatibleStateImageBehavior = false;
            this.lvInvention.View = System.Windows.Forms.View.Details;
            this.lvInvention.DoubleClick += new System.EventHandler(this.propertiesList_DoubleClick);
            // 
            // lblNoItemInvention
            // 
            this.lblNoItemInvention.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNoItemInvention.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblNoItemInvention.Location = new System.Drawing.Point(3, 16);
            this.lblNoItemInvention.Name = "lblNoItemInvention";
            this.lblNoItemInvention.Size = new System.Drawing.Size(218, 174);
            this.lblNoItemInvention.TabIndex = 5;
            this.lblNoItemInvention.Text = "No item required.";
            this.lblNoItemInvention.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tpReactions
            // 
            this.tpReactions.Controls.Add(this.gbReactionsBoM);
            this.tpReactions.Location = new System.Drawing.Point(4, 22);
            this.tpReactions.Name = "tpReactions";
            this.tpReactions.Size = new System.Drawing.Size(614, 539);
            this.tpReactions.TabIndex = 5;
            this.tpReactions.Text = "Reactions";
            this.tpReactions.UseVisualStyleBackColor = true;
            // 
            // tlpModifiers
            // 
            this.tlpModifiers.AutoSize = true;
            this.tlpModifiers.ColumnCount = 1;
            this.tlpModifiers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpModifiers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpModifiers.Controls.Add(this.flpUpper, 0, 0);
            this.tlpModifiers.Controls.Add(this.flpLower, 0, 1);
            this.tlpModifiers.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpModifiers.Location = new System.Drawing.Point(0, 0);
            this.tlpModifiers.Name = "tlpModifiers";
            this.tlpModifiers.RowCount = 2;
            this.tlpModifiers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpModifiers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpModifiers.Size = new System.Drawing.Size(622, 65);
            this.tlpModifiers.TabIndex = 1;
            // 
            // flpUpper
            // 
            this.flpUpper.AutoSize = true;
            this.flpUpper.Controls.Add(this.lblMaterialEfficiency);
            this.flpUpper.Controls.Add(this.nudME);
            this.flpUpper.Controls.Add(this.lblTimeEfficiency);
            this.flpUpper.Controls.Add(this.nudTE);
            this.flpUpper.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpUpper.Location = new System.Drawing.Point(3, 3);
            this.flpUpper.Name = "flpUpper";
            this.flpUpper.Size = new System.Drawing.Size(616, 26);
            this.flpUpper.TabIndex = 0;
            // 
            // lblMaterialEfficiency
            // 
            this.lblMaterialEfficiency.Location = new System.Drawing.Point(3, 0);
            this.lblMaterialEfficiency.Name = "lblMaterialEfficiency";
            this.lblMaterialEfficiency.Size = new System.Drawing.Size(116, 23);
            this.lblMaterialEfficiency.TabIndex = 1;
            this.lblMaterialEfficiency.Text = "Material Efficiency (%) :";
            this.lblMaterialEfficiency.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // nudME
            // 
            this.nudME.Location = new System.Drawing.Point(125, 3);
            this.nudME.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudME.Name = "nudME";
            this.nudME.ReadOnly = true;
            this.nudME.Size = new System.Drawing.Size(45, 20);
            this.nudME.TabIndex = 9;
            this.nudME.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudME.ValueChanged += new System.EventHandler(this.nudME_ValueChanged);
            // 
            // lblTimeEfficiency
            // 
            this.lblTimeEfficiency.Location = new System.Drawing.Point(176, 0);
            this.lblTimeEfficiency.Name = "lblTimeEfficiency";
            this.lblTimeEfficiency.Size = new System.Drawing.Size(103, 23);
            this.lblTimeEfficiency.TabIndex = 2;
            this.lblTimeEfficiency.Text = "Time Efficiency (%) :";
            this.lblTimeEfficiency.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // nudTE
            // 
            this.nudTE.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudTE.Location = new System.Drawing.Point(285, 3);
            this.nudTE.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudTE.Name = "nudTE";
            this.nudTE.ReadOnly = true;
            this.nudTE.Size = new System.Drawing.Size(45, 20);
            this.nudTE.TabIndex = 11;
            this.nudTE.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudTE.ValueChanged += new System.EventHandler(this.nudTE_ValueChanged);
            // 
            // flpLower
            // 
            this.flpLower.AutoSize = true;
            this.flpLower.Controls.Add(this.cbFacility);
            this.flpLower.Controls.Add(this.cbImplantSet);
            this.flpLower.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpLower.Location = new System.Drawing.Point(3, 35);
            this.flpLower.Name = "flpLower";
            this.flpLower.Size = new System.Drawing.Size(616, 27);
            this.flpLower.TabIndex = 6;
            // 
            // cbFacility
            // 
            this.cbFacility.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFacility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFacility.FormattingEnabled = true;
            this.cbFacility.Location = new System.Drawing.Point(3, 3);
            this.cbFacility.Name = "cbFacility";
            this.cbFacility.Size = new System.Drawing.Size(225, 21);
            this.cbFacility.TabIndex = 2;
            this.cbFacility.SelectedIndexChanged += new System.EventHandler(this.cbFacility_SelectedIndexChanged);
            // 
            // cbImplantSet
            // 
            this.cbImplantSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbImplantSet.DisplayMember = "Name";
            this.cbImplantSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbImplantSet.FormattingEnabled = true;
            this.cbImplantSet.Location = new System.Drawing.Point(234, 3);
            this.cbImplantSet.Name = "cbImplantSet";
            this.cbImplantSet.Size = new System.Drawing.Size(195, 21);
            this.cbImplantSet.TabIndex = 2;
            this.cbImplantSet.SelectedIndexChanged += new System.EventHandler(this.cbImplantSet_SelectedIndexChanged);
            // 
            // blueprintSelectControl
            // 
            this.blueprintSelectControl.AutoSize = true;
            this.blueprintSelectControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blueprintSelectControl.Location = new System.Drawing.Point(0, 0);
            this.blueprintSelectControl.Margin = new System.Windows.Forms.Padding(0);
            this.blueprintSelectControl.Name = "blueprintSelectControl";
            this.blueprintSelectControl.Size = new System.Drawing.Size(163, 700);
            this.blueprintSelectControl.TabIndex = 0;
            // 
            // lblProduce
            // 
            this.lblProduce.AutoSize = true;
            this.lblProduce.BackColor = System.Drawing.SystemColors.Control;
            this.lblProduce.Location = new System.Drawing.Point(6, 15);
            this.lblProduce.Name = "lblProduce";
            this.lblProduce.Size = new System.Drawing.Size(55, 13);
            this.lblProduce.TabIndex = 1;
            this.lblProduce.Text = "Produces:";
            // 
            // lblProductionLimit
            // 
            this.lblProductionLimit.AutoSize = true;
            this.lblProductionLimit.BackColor = System.Drawing.SystemColors.Control;
            this.lblProductionLimit.Location = new System.Drawing.Point(6, 110);
            this.lblProductionLimit.Name = "lblProductionLimit";
            this.lblProductionLimit.Size = new System.Drawing.Size(104, 13);
            this.lblProductionLimit.TabIndex = 3;
            this.lblProductionLimit.Text = "Max Runs Per Copy:";
            // 
            // lblProductionTime
            // 
            this.lblProductionTime.AutoSize = true;
            this.lblProductionTime.BackColor = System.Drawing.SystemColors.Control;
            this.lblProductionTime.Location = new System.Drawing.Point(6, 16);
            this.lblProductionTime.Name = "lblProductionTime";
            this.lblProductionTime.Size = new System.Drawing.Size(104, 13);
            this.lblProductionTime.TabIndex = 4;
            this.lblProductionTime.Text = "Manufacturing Time:";
            // 
            // lblItem
            // 
            this.lblItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblItem.AutoEllipsis = true;
            this.lblItem.BackColor = System.Drawing.SystemColors.Control;
            this.lblItem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblItem.ForeColor = System.Drawing.Color.Blue;
            this.lblItem.Location = new System.Drawing.Point(7, 30);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(210, 13);
            this.lblItem.TabIndex = 8;
            this.lblItem.Text = "Item";
            this.lblItem.Click += new System.EventHandler(this.lblItem_Click);
            // 
            // lblRunsPerCopy
            // 
            this.lblRunsPerCopy.AutoSize = true;
            this.lblRunsPerCopy.BackColor = System.Drawing.SystemColors.Control;
            this.lblRunsPerCopy.Location = new System.Drawing.Point(110, 110);
            this.lblRunsPerCopy.Name = "lblRunsPerCopy";
            this.lblRunsPerCopy.Size = new System.Drawing.Size(78, 13);
            this.lblRunsPerCopy.TabIndex = 12;
            this.lblRunsPerCopy.Text = "Runs Per Copy";
            // 
            // lblProductionBaseTime
            // 
            this.lblProductionBaseTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProductionBaseTime.AutoEllipsis = true;
            this.lblProductionBaseTime.BackColor = System.Drawing.SystemColors.Control;
            this.lblProductionBaseTime.Location = new System.Drawing.Point(16, 29);
            this.lblProductionBaseTime.Name = "lblProductionBaseTime";
            this.lblProductionBaseTime.Size = new System.Drawing.Size(200, 13);
            this.lblProductionBaseTime.TabIndex = 13;
            this.lblProductionBaseTime.Text = "Base Time";
            // 
            // lblProductionCharTime
            // 
            this.lblProductionCharTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProductionCharTime.AutoEllipsis = true;
            this.lblProductionCharTime.BackColor = System.Drawing.SystemColors.Control;
            this.lblProductionCharTime.Location = new System.Drawing.Point(16, 42);
            this.lblProductionCharTime.Name = "lblProductionCharTime";
            this.lblProductionCharTime.Size = new System.Drawing.Size(200, 13);
            this.lblProductionCharTime.TabIndex = 14;
            this.lblProductionCharTime.Text = "You";
            // 
            // lblResearchMECharTime
            // 
            this.lblResearchMECharTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResearchMECharTime.AutoEllipsis = true;
            this.lblResearchMECharTime.BackColor = System.Drawing.SystemColors.Control;
            this.lblResearchMECharTime.Location = new System.Drawing.Point(16, 42);
            this.lblResearchMECharTime.Name = "lblResearchMECharTime";
            this.lblResearchMECharTime.Size = new System.Drawing.Size(200, 13);
            this.lblResearchMECharTime.TabIndex = 17;
            this.lblResearchMECharTime.Text = "You";
            // 
            // lblResearchMEBaseTime
            // 
            this.lblResearchMEBaseTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResearchMEBaseTime.AutoEllipsis = true;
            this.lblResearchMEBaseTime.BackColor = System.Drawing.SystemColors.Control;
            this.lblResearchMEBaseTime.Location = new System.Drawing.Point(16, 29);
            this.lblResearchMEBaseTime.Name = "lblResearchMEBaseTime";
            this.lblResearchMEBaseTime.Size = new System.Drawing.Size(200, 13);
            this.lblResearchMEBaseTime.TabIndex = 16;
            this.lblResearchMEBaseTime.Text = "Base Time";
            // 
            // lblResearchME
            // 
            this.lblResearchME.AutoSize = true;
            this.lblResearchME.BackColor = System.Drawing.SystemColors.Control;
            this.lblResearchME.Location = new System.Drawing.Point(6, 16);
            this.lblResearchME.Name = "lblResearchME";
            this.lblResearchME.Size = new System.Drawing.Size(122, 13);
            this.lblResearchME.TabIndex = 15;
            this.lblResearchME.Text = "Research Material Time:";
            // 
            // lblResearchCopyCharTime
            // 
            this.lblResearchCopyCharTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResearchCopyCharTime.AutoEllipsis = true;
            this.lblResearchCopyCharTime.BackColor = System.Drawing.SystemColors.Control;
            this.lblResearchCopyCharTime.Location = new System.Drawing.Point(16, 88);
            this.lblResearchCopyCharTime.Name = "lblResearchCopyCharTime";
            this.lblResearchCopyCharTime.Size = new System.Drawing.Size(200, 13);
            this.lblResearchCopyCharTime.TabIndex = 20;
            this.lblResearchCopyCharTime.Text = "You";
            // 
            // lblResearchCopyBaseTime
            // 
            this.lblResearchCopyBaseTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResearchCopyBaseTime.AutoEllipsis = true;
            this.lblResearchCopyBaseTime.BackColor = System.Drawing.SystemColors.Control;
            this.lblResearchCopyBaseTime.Location = new System.Drawing.Point(16, 75);
            this.lblResearchCopyBaseTime.Name = "lblResearchCopyBaseTime";
            this.lblResearchCopyBaseTime.Size = new System.Drawing.Size(200, 13);
            this.lblResearchCopyBaseTime.TabIndex = 19;
            this.lblResearchCopyBaseTime.Text = "Base Time";
            // 
            // lblResearchCopy
            // 
            this.lblResearchCopy.AutoSize = true;
            this.lblResearchCopy.BackColor = System.Drawing.SystemColors.Control;
            this.lblResearchCopy.Location = new System.Drawing.Point(6, 62);
            this.lblResearchCopy.Name = "lblResearchCopy";
            this.lblResearchCopy.Size = new System.Drawing.Size(216, 13);
            this.lblResearchCopy.TabIndex = 18;
            this.lblResearchCopy.Text = "Research Copy Time (Single Run Per Copy):";
            // 
            // lblResearchTECharTime
            // 
            this.lblResearchTECharTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResearchTECharTime.AutoEllipsis = true;
            this.lblResearchTECharTime.BackColor = System.Drawing.SystemColors.Control;
            this.lblResearchTECharTime.Location = new System.Drawing.Point(16, 134);
            this.lblResearchTECharTime.Name = "lblResearchTECharTime";
            this.lblResearchTECharTime.Size = new System.Drawing.Size(200, 13);
            this.lblResearchTECharTime.TabIndex = 23;
            this.lblResearchTECharTime.Text = "You";
            // 
            // lblResearchTEBaseTime
            // 
            this.lblResearchTEBaseTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResearchTEBaseTime.AutoEllipsis = true;
            this.lblResearchTEBaseTime.BackColor = System.Drawing.SystemColors.Control;
            this.lblResearchTEBaseTime.Location = new System.Drawing.Point(16, 121);
            this.lblResearchTEBaseTime.Name = "lblResearchTEBaseTime";
            this.lblResearchTEBaseTime.Size = new System.Drawing.Size(200, 13);
            this.lblResearchTEBaseTime.TabIndex = 22;
            this.lblResearchTEBaseTime.Text = "Base Time";
            // 
            // lblResearchTE
            // 
            this.lblResearchTE.AutoSize = true;
            this.lblResearchTE.BackColor = System.Drawing.SystemColors.Control;
            this.lblResearchTE.Location = new System.Drawing.Point(6, 108);
            this.lblResearchTE.Name = "lblResearchTE";
            this.lblResearchTE.Size = new System.Drawing.Size(140, 13);
            this.lblResearchTE.TabIndex = 21;
            this.lblResearchTE.Text = "Research Productivity Time:";
            // 
            // lblInvention
            // 
            this.lblInvention.AutoSize = true;
            this.lblInvention.BackColor = System.Drawing.SystemColors.Control;
            this.lblInvention.Location = new System.Drawing.Point(6, 46);
            this.lblInvention.Name = "lblInvention";
            this.lblInvention.Size = new System.Drawing.Size(45, 13);
            this.lblInvention.TabIndex = 24;
            this.lblInvention.Text = "Invents:";
            // 
            // gbGeneralInfo
            // 
            this.gbGeneralInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbGeneralInfo.BackColor = System.Drawing.SystemColors.Control;
            this.gbGeneralInfo.Controls.Add(this.lblSuccessProbability);
            this.gbGeneralInfo.Controls.Add(this.lblProbability);
            this.gbGeneralInfo.Controls.Add(this.InventBlueprintListBox);
            this.gbGeneralInfo.Controls.Add(this.lblProduce);
            this.gbGeneralInfo.Controls.Add(this.lblProductionLimit);
            this.gbGeneralInfo.Controls.Add(this.lblInvention);
            this.gbGeneralInfo.Controls.Add(this.lblItem);
            this.gbGeneralInfo.Controls.Add(this.lblRunsPerCopy);
            this.gbGeneralInfo.Location = new System.Drawing.Point(3, 5);
            this.gbGeneralInfo.Name = "gbGeneralInfo";
            this.gbGeneralInfo.Size = new System.Drawing.Size(224, 130);
            this.gbGeneralInfo.TabIndex = 26;
            this.gbGeneralInfo.TabStop = false;
            this.gbGeneralInfo.Text = "GENERAL INFORMATION";
            // 
            // lblSuccessProbability
            // 
            this.lblSuccessProbability.AutoSize = true;
            this.lblSuccessProbability.BackColor = System.Drawing.SystemColors.Control;
            this.lblSuccessProbability.Location = new System.Drawing.Point(6, 94);
            this.lblSuccessProbability.Name = "lblSuccessProbability";
            this.lblSuccessProbability.Size = new System.Drawing.Size(102, 13);
            this.lblSuccessProbability.TabIndex = 25;
            this.lblSuccessProbability.Text = "Success Probability:";
            // 
            // lblProbability
            // 
            this.lblProbability.AutoSize = true;
            this.lblProbability.BackColor = System.Drawing.SystemColors.Control;
            this.lblProbability.Location = new System.Drawing.Point(110, 94);
            this.lblProbability.Name = "lblProbability";
            this.lblProbability.Size = new System.Drawing.Size(66, 13);
            this.lblProbability.TabIndex = 26;
            this.lblProbability.Text = "Probability %";
            // 
            // InventBlueprintListBox
            // 
            this.InventBlueprintListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InventBlueprintListBox.BackColor = System.Drawing.SystemColors.Control;
            this.InventBlueprintListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.InventBlueprintListBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.InventBlueprintListBox.ForeColor = System.Drawing.Color.Blue;
            this.InventBlueprintListBox.Location = new System.Drawing.Point(9, 61);
            this.InventBlueprintListBox.Name = "InventBlueprintListBox";
            this.InventBlueprintListBox.Size = new System.Drawing.Size(209, 26);
            this.InventBlueprintListBox.Sorted = true;
            this.InventBlueprintListBox.TabIndex = 1;
            this.InventBlueprintListBox.TabStop = false;
            this.InventBlueprintListBox.SelectedIndexChanged += new System.EventHandler(this.InventBlueprintListBox_SelectedIndexChanged);
            this.InventBlueprintListBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.InventBlueprintListBox_MouseMove);
            // 
            // gbResearching
            // 
            this.gbResearching.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbResearching.BackColor = System.Drawing.SystemColors.Control;
            this.gbResearching.Controls.Add(this.lblResearchME);
            this.gbResearching.Controls.Add(this.lblResearchTECharTime);
            this.gbResearching.Controls.Add(this.lblResearchMEBaseTime);
            this.gbResearching.Controls.Add(this.lblResearchTEBaseTime);
            this.gbResearching.Controls.Add(this.lblResearchMECharTime);
            this.gbResearching.Controls.Add(this.lblResearchTE);
            this.gbResearching.Controls.Add(this.lblResearchCopy);
            this.gbResearching.Controls.Add(this.lblResearchCopyCharTime);
            this.gbResearching.Controls.Add(this.lblResearchCopyBaseTime);
            this.gbResearching.Location = new System.Drawing.Point(3, 202);
            this.gbResearching.Name = "gbResearching";
            this.gbResearching.Size = new System.Drawing.Size(224, 155);
            this.gbResearching.TabIndex = 1;
            this.gbResearching.TabStop = false;
            this.gbResearching.Text = "RESEARCHING";
            // 
            // gbManufacturing
            // 
            this.gbManufacturing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbManufacturing.BackColor = System.Drawing.SystemColors.Control;
            this.gbManufacturing.Controls.Add(this.lblProductionCharTime);
            this.gbManufacturing.Controls.Add(this.lblProductionTime);
            this.gbManufacturing.Controls.Add(this.lblProductionBaseTime);
            this.gbManufacturing.Location = new System.Drawing.Point(3, 137);
            this.gbManufacturing.Name = "gbManufacturing";
            this.gbManufacturing.Size = new System.Drawing.Size(224, 60);
            this.gbManufacturing.TabIndex = 27;
            this.gbManufacturing.TabStop = false;
            this.gbManufacturing.Text = "MANUFACTURING";
            // 
            // pnlAttributes
            // 
            this.pnlAttributes.BackColor = System.Drawing.SystemColors.Control;
            this.pnlAttributes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlAttributes.Controls.Add(this.gbReactions);
            this.pnlAttributes.Controls.Add(this.gbInvention);
            this.pnlAttributes.Controls.Add(this.gbResearching);
            this.pnlAttributes.Controls.Add(this.gbGeneralInfo);
            this.pnlAttributes.Controls.Add(this.gbManufacturing);
            this.pnlAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAttributes.Location = new System.Drawing.Point(3, 16);
            this.pnlAttributes.Name = "pnlAttributes";
            this.pnlAttributes.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.pnlAttributes.Size = new System.Drawing.Size(234, 503);
            this.pnlAttributes.TabIndex = 1;
            // 
            // gbReactions
            // 
            this.gbReactions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbReactions.BackColor = System.Drawing.SystemColors.Control;
            this.gbReactions.Controls.Add(this.lblReactionCharTime);
            this.gbReactions.Controls.Add(this.lblReactionTime);
            this.gbReactions.Controls.Add(this.lblReactionBaseTime);
            this.gbReactions.Location = new System.Drawing.Point(3, 432);
            this.gbReactions.Name = "gbReactions";
            this.gbReactions.Size = new System.Drawing.Size(224, 64);
            this.gbReactions.TabIndex = 29;
            this.gbReactions.TabStop = false;
            this.gbReactions.Text = "REACTIONS";
            // 
            // lblReactionCharTime
            // 
            this.lblReactionCharTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblReactionCharTime.AutoEllipsis = true;
            this.lblReactionCharTime.BackColor = System.Drawing.SystemColors.Control;
            this.lblReactionCharTime.Location = new System.Drawing.Point(16, 42);
            this.lblReactionCharTime.Name = "lblReactionCharTime";
            this.lblReactionCharTime.Size = new System.Drawing.Size(200, 13);
            this.lblReactionCharTime.TabIndex = 24;
            this.lblReactionCharTime.Text = "You";
            // 
            // lblReactionTime
            // 
            this.lblReactionTime.AutoSize = true;
            this.lblReactionTime.BackColor = System.Drawing.SystemColors.Control;
            this.lblReactionTime.Location = new System.Drawing.Point(6, 16);
            this.lblReactionTime.Name = "lblReactionTime";
            this.lblReactionTime.Size = new System.Drawing.Size(79, 13);
            this.lblReactionTime.TabIndex = 15;
            this.lblReactionTime.Text = "Reaction Time:";
            // 
            // lblReactionBaseTime
            // 
            this.lblReactionBaseTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblReactionBaseTime.AutoEllipsis = true;
            this.lblReactionBaseTime.BackColor = System.Drawing.SystemColors.Control;
            this.lblReactionBaseTime.Location = new System.Drawing.Point(16, 29);
            this.lblReactionBaseTime.Name = "lblReactionBaseTime";
            this.lblReactionBaseTime.Size = new System.Drawing.Size(200, 13);
            this.lblReactionBaseTime.TabIndex = 16;
            this.lblReactionBaseTime.Text = "Base Time";
            // 
            // gbInvention
            // 
            this.gbInvention.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbInvention.BackColor = System.Drawing.SystemColors.Control;
            this.gbInvention.Controls.Add(this.lblInventionCharTime);
            this.gbInvention.Controls.Add(this.lblInventionTime);
            this.gbInvention.Controls.Add(this.lblInventionBaseTime);
            this.gbInvention.Location = new System.Drawing.Point(3, 362);
            this.gbInvention.Name = "gbInvention";
            this.gbInvention.Size = new System.Drawing.Size(224, 64);
            this.gbInvention.TabIndex = 28;
            this.gbInvention.TabStop = false;
            this.gbInvention.Text = "INVENTION";
            // 
            // lblInventionCharTime
            // 
            this.lblInventionCharTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInventionCharTime.AutoEllipsis = true;
            this.lblInventionCharTime.BackColor = System.Drawing.SystemColors.Control;
            this.lblInventionCharTime.Location = new System.Drawing.Point(16, 42);
            this.lblInventionCharTime.Name = "lblInventionCharTime";
            this.lblInventionCharTime.Size = new System.Drawing.Size(200, 13);
            this.lblInventionCharTime.TabIndex = 24;
            this.lblInventionCharTime.Text = "You";
            // 
            // lblInventionTime
            // 
            this.lblInventionTime.AutoSize = true;
            this.lblInventionTime.BackColor = System.Drawing.SystemColors.Control;
            this.lblInventionTime.Location = new System.Drawing.Point(6, 16);
            this.lblInventionTime.Name = "lblInventionTime";
            this.lblInventionTime.Size = new System.Drawing.Size(80, 13);
            this.lblInventionTime.TabIndex = 15;
            this.lblInventionTime.Text = "Invention Time:";
            // 
            // lblInventionBaseTime
            // 
            this.lblInventionBaseTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInventionBaseTime.AutoEllipsis = true;
            this.lblInventionBaseTime.BackColor = System.Drawing.SystemColors.Control;
            this.lblInventionBaseTime.Location = new System.Drawing.Point(16, 29);
            this.lblInventionBaseTime.Name = "lblInventionBaseTime";
            this.lblInventionBaseTime.Size = new System.Drawing.Size(200, 13);
            this.lblInventionBaseTime.TabIndex = 16;
            this.lblInventionBaseTime.Text = "Base Time";
            // 
            // gbReactionsBoM
            // 
            this.gbReactionsBoM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbReactionsBoM.Controls.Add(this.lvReactions);
            this.gbReactionsBoM.Location = new System.Drawing.Point(3, 3);
            this.gbReactionsBoM.Name = "gbReactionsBoM";
            this.gbReactionsBoM.Size = new System.Drawing.Size(608, 533);
            this.gbReactionsBoM.TabIndex = 0;
            this.gbReactionsBoM.TabStop = false;
            this.gbReactionsBoM.Text = "Reaction Materials";
            // 
            // lvReactions
            // 
            this.lvReactions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvReactions.ContextMenuStrip = this.BlueprintAttributeContextMenu;
            this.lvReactions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvReactions.FullRowSelect = true;
            this.lvReactions.HideSelection = false;
            this.lvReactions.Location = new System.Drawing.Point(3, 16);
            this.lvReactions.MultiSelect = false;
            this.lvReactions.Name = "lvReactions";
            this.lvReactions.Size = new System.Drawing.Size(602, 514);
            this.lvReactions.TabIndex = 1;
            this.lvReactions.UseCompatibleStateImageBehavior = false;
            this.lvReactions.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Item";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Quantity (Base)";
            this.columnHeader2.Width = 86;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Quantity (You)";
            this.columnHeader3.Width = 82;
            // 
            // BlueprintBrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.DoubleBuffered = true;
            this.Name = "BlueprintBrowserControl";
            this.Size = new System.Drawing.Size(1034, 700);
            this.Controls.SetChildIndex(this.scObjectBrowser, 0);
            this.scObjectBrowser.Panel1.ResumeLayout(false);
            this.scObjectBrowser.Panel1.PerformLayout();
            this.scObjectBrowser.Panel2.ResumeLayout(false);
            this.scObjectBrowser.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scObjectBrowser)).EndInit();
            this.scObjectBrowser.ResumeLayout(false);
            this.pnlDetails.ResumeLayout(false);
            this.pnlBrowserHeader.ResumeLayout(false);
            this.pnlBrowserHeader.PerformLayout();
            this.scDetailsRight.Panel1.ResumeLayout(false);
            this.scDetailsRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scDetailsRight)).EndInit();
            this.scDetailsRight.ResumeLayout(false);
            this.scDetails.Panel1.ResumeLayout(false);
            this.scDetails.Panel1.PerformLayout();
            this.scDetails.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scDetails)).EndInit();
            this.scDetails.ResumeLayout(false);
            this.gbDescription.ResumeLayout(false);
            this.gbRequiredSkills.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tpManufacturing.ResumeLayout(false);
            this.gbManufBoM.ResumeLayout(false);
            this.BlueprintAttributeContextMenu.ResumeLayout(false);
            this.tpResearchME.ResumeLayout(false);
            this.gbResearchMEBoM.ResumeLayout(false);
            this.tpResearchTE.ResumeLayout(false);
            this.gbResearchTEBoM.ResumeLayout(false);
            this.tpCopying.ResumeLayout(false);
            this.gbCopyBoM.ResumeLayout(false);
            this.tpInvention.ResumeLayout(false);
            this.gbInvBoM.ResumeLayout(false);
            this.tpReactions.ResumeLayout(false);
            this.tlpModifiers.ResumeLayout(false);
            this.tlpModifiers.PerformLayout();
            this.flpUpper.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudME)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTE)).EndInit();
            this.flpLower.ResumeLayout(false);
            this.gbGeneralInfo.ResumeLayout(false);
            this.gbGeneralInfo.PerformLayout();
            this.gbResearching.ResumeLayout(false);
            this.gbResearching.PerformLayout();
            this.gbManufacturing.ResumeLayout(false);
            this.gbManufacturing.PerformLayout();
            this.pnlAttributes.ResumeLayout(false);
            this.gbReactions.ResumeLayout(false);
            this.gbReactions.PerformLayout();
            this.gbInvention.ResumeLayout(false);
            this.gbInvention.PerformLayout();
            this.gbReactionsBoM.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbRequiredSkills;
        private RequiredSkillsControl requiredSkillsControl;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tpManufacturing;
        private System.Windows.Forms.ListView lvManufacturing;
        private System.Windows.Forms.TabPage tpInvention;
        private System.Windows.Forms.GroupBox gbManufBoM;
        private System.Windows.Forms.ColumnHeader chManufItem;
        private BlueprintSelectControl blueprintSelectControl;
        private System.Windows.Forms.GroupBox gbInvBoM;
        private System.Windows.Forms.ListView lvInvention;
        private System.Windows.Forms.Label lblProduce;
        private System.Windows.Forms.Label lblProductionTime;
        private System.Windows.Forms.Label lblProductionLimit;
        private System.Windows.Forms.Label lblRunsPerCopy;
        private System.Windows.Forms.NumericUpDown nudTE;
        private System.Windows.Forms.NumericUpDown nudME;
        private System.Windows.Forms.Label lblItem;
        private System.Windows.Forms.Label lblProductionCharTime;
        private System.Windows.Forms.Label lblProductionBaseTime;
        private System.Windows.Forms.Label lblResearchMECharTime;
        private System.Windows.Forms.Label lblResearchMEBaseTime;
        private System.Windows.Forms.Label lblResearchME;
        private System.Windows.Forms.Label lblResearchTECharTime;
        private System.Windows.Forms.Label lblResearchTEBaseTime;
        private System.Windows.Forms.Label lblResearchTE;
        private System.Windows.Forms.Label lblResearchCopyCharTime;
        private System.Windows.Forms.Label lblResearchCopyBaseTime;
        private System.Windows.Forms.Label lblResearchCopy;
        private System.Windows.Forms.Label lblInvention;
        private System.Windows.Forms.TabPage tpCopying;
        private System.Windows.Forms.TabPage tpResearchTE;
        private System.Windows.Forms.GroupBox gbResearchTEBoM;
        private System.Windows.Forms.ListView lvResearchTE;
        private System.Windows.Forms.GroupBox gbCopyBoM;
        private System.Windows.Forms.ListView lvCopying;
        private System.Windows.Forms.TabPage tpResearchME;
        private System.Windows.Forms.GroupBox gbResearchMEBoM;
        private System.Windows.Forms.ListView lvResearchME;
        private System.Windows.Forms.GroupBox gbGeneralInfo;
        private System.Windows.Forms.GroupBox gbManufacturing;
        private System.Windows.Forms.GroupBox gbResearching;
        private System.Windows.Forms.Panel pnlAttributes;
        private System.Windows.Forms.ComboBox cbFacility;
        private System.Windows.Forms.ComboBox cbImplantSet;
        private System.Windows.Forms.ListBox InventBlueprintListBox;
        private System.Windows.Forms.Label lblMaterialEfficiency;
        private System.Windows.Forms.Label lblTimeEfficiency;
        private System.Windows.Forms.TableLayoutPanel tlpModifiers;
        private System.Windows.Forms.FlowLayoutPanel flpUpper;
        private System.Windows.Forms.FlowLayoutPanel flpLower;
        private System.Windows.Forms.GroupBox gbInvention;
        private System.Windows.Forms.Label lblInventionTime;
        private System.Windows.Forms.Label lblInventionBaseTime;
        private System.Windows.Forms.Label lblNoItemTE;
        private System.Windows.Forms.Label lblNoItemME;
        private System.Windows.Forms.ContextMenuStrip BlueprintAttributeContextMenu;
        private System.Windows.Forms.ToolStripMenuItem exportToCSVToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader chManufQuantityBase;
        private System.Windows.Forms.ColumnHeader chManufQuantityYou;
        private System.Windows.Forms.Label lblInventionCharTime;
        private System.Windows.Forms.Label lblNoItemCopy;
        private System.Windows.Forms.Label lblSuccessProbability;
        private System.Windows.Forms.Label lblProbability;
        private System.Windows.Forms.Label lblNoItemManufacturing;
        private System.Windows.Forms.Label lblNoItemInvention;
        private System.Windows.Forms.ToolStripMenuItem showInMenuItem;
        private System.Windows.Forms.ToolStripSeparator showInMenuSeparator;
        private System.Windows.Forms.TabPage tpReactions;
        private System.Windows.Forms.GroupBox gbReactions;
        private System.Windows.Forms.Label lblReactionCharTime;
        private System.Windows.Forms.Label lblReactionTime;
        private System.Windows.Forms.Label lblReactionBaseTime;
        private System.Windows.Forms.GroupBox gbReactionsBoM;
        private System.Windows.Forms.ListView lvReactions;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}
