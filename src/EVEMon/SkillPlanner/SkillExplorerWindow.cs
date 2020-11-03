using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Factories;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;
using EVEMon.Common.Models.Collections;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// Skill Explorer Window - allows easy exploration of skills.
    /// </summary>
    /// <remarks>
    /// This class is the Skill Explorer - or "What items/ships other skills does this skill enable for me" form.
    /// For a given skill, we show 2 tree controls (with splitter bars seperating them), one for skills, one for either ships or items, depending on the skill we're exploring.
    /// (There is no skill that enables both a ship and an item)
    /// Each tree shows a node for each level of the chosen skill with child nodes for all items that this level
    /// is a direct prerequisite for. If we can use the item, it is shown in a normal font. If this level of skill would 
    /// enable use of the item, we show it dimmed. If this level is one of a number of missing skills for the item, then
    /// we show the item in red (with a tooltip indicating the other missing skills).
    /// The user can chose to view the trees in alphabetic order, or by the categories (alpha is default).
    /// For items, you can chose to show just the base T1/T2 items, or all variants of an item (default is base items)
    /// 
    /// There's a dropdown listbox that keeps track of what skills you've been exploring in this session.
    /// 
    /// Each node has a context menu that enables you to view the item in the relevant browser, add missing skills to 
    /// your plan, show list of all prerequisites (including costs if unowned), and for the skill tree, use the selected skill as a new base skill.
    /// 
    /// Doubleclicking a ship/item leaf node will show the selected item in the browser.
    /// Doubleclicking a skill in the tree will set that skill as the base skill we're exploring
    /// </remarks>
    public partial class SkillExplorerWindow : EVEMonForm
    {
        private Plan m_plan;
        private Character m_character;
        private Skill m_skill;
        private bool m_hasItems;
        private bool m_hasShips;
        private bool m_hasBlueprints;
        private bool m_allSkillsExpanded;
        private bool m_allObjectsExpanded;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillExplorerWindow"/> class.
        /// Constructor for designer and WindowsFactory.
        /// </summary>
        public SkillExplorerWindow()
        {
            InitializeComponent();
            splitContainer.RememberDistanceKey = "SkillExplorer";

            toolTip.SetToolTip(cbHistory, "A history of the skills that you have been looking at.");

            tvSkills.MouseUp += tvSkills_MouseDown;
            tvSkills.MouseMove += tvSkills_MouseMove;
            tvEntity.MouseUp += tvEntity_MouseDown;
            tvEntity.MouseMove += tvEntity_MouseMove;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillExplorerWindow"/> class.
        /// Constructor for WindowsFactory.
        /// </summary>
        /// <param name="plan">The plan.</param>
        public SkillExplorerWindow(Plan plan)
            :this()
        {
            Plan = plan;
        }

        #endregion


        #region Inherited Events

        /// <summary>
        /// On load, restores the window rectangle from the settings.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            EveMonClient.PlanNameChanged += EveMonClient_PlanNameChanged;
            EveMonClient.CharacterUpdated += EveMonClient_CharacterUpdated;
        }


        /// <summary>
        /// Unsubscribe events on closing.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            EveMonClient.CharacterUpdated -= EveMonClient_CharacterUpdated;
            EveMonClient.PlanNameChanged -= EveMonClient_PlanNameChanged;
        }

        #endregion


        #region Internal Properties

        /// <summary>
        /// Gets or sets the plan.
        /// </summary>
        /// <value>
        /// The plan.
        /// </value>
        internal Plan Plan
        {
            get { return m_plan; }
            set
            {
                if (m_plan == value)
                    return;

                m_plan = value;
                m_character = (Character)m_plan.Character;

                UpdatePlanName();
            }
        }


        #endregion

        #region Setters

        /// <summary>
        /// Sets the skill.
        /// </summary>
        /// <param name="skill">The skill.</param>
        private void SetSkill(Skill skill)
        {
            if (m_skill == skill)
                return;

            // We can't unset a skill
            if (skill == null)
                return;

            m_skill = skill;
            m_character = skill.Character;

            // If already in history combo, remove it to reinsert it at the top
            if (cbHistory.Items.Contains(m_skill))
                cbHistory.Items.RemoveAt(cbHistory.Items.IndexOf(m_skill));

            cbHistory.Items.Insert(0, m_skill);
            cbHistory.SelectedIndex = 0;

            UpdateContent();
        }
        
        #endregion


        #region Content creation and update

        /// <summary>
        /// Update all the content.
        /// </summary>
        private void UpdateContent()
        {
            UpdateHeader();
            UpdateTrees();
        }

        /// <summary>
        /// Update the character and plan name on the skill header group title.
        /// </summary>
        private void UpdatePlanName()
        {
            if (m_character != null)
                grpPlanName.Text = $"{m_character.Name}";

            if (m_plan != null)
                grpPlanName.Text += $" - {m_plan.Name}";
        }

        /// <summary>
        /// Sets the details for the skill we're analysing.
        /// Work out if we know the skill, if so, how many  points.
        /// If we don't know it, show if we could train it now or not
        /// and indicate if we own the book, if not, show the cost.
        /// </summary>
        private void UpdateHeader()
        {
            StringBuilder sb = new StringBuilder();
            if (m_skill.IsTraining)
                sb.Append($"{m_skill.Name} - In Training ");

            if (m_skill.IsKnown)
            {
                sb.Append($"Trained to level {m_skill.Level} with " +
                          $"{(m_skill.SkillPoints > 0 ? $"{m_skill.SkillPoints:N0} sp" : "0 sp")}");
            }
            else if (m_skill.Character != null)
            {
                sb.Append($"Not Trained - prereqs {(m_skill.ArePrerequisitesMet ? string.Empty : "not ")}met, " +
                          $"skillbook is {(m_skill.IsOwned ? "owned" : $"not owned, book costs {m_skill.FormattedCost} ISK")}");
            }
            else
                sb.Append($"Skillbook costs {m_skill.FormattedCost} ISK");

            lblSkillInfo.Text = sb.ToString();
            tmrAutoUpdate.Enabled = m_skill.IsTraining;
        }

        /// <summary>
        /// Recreate the trees and their headers.
        /// 
        /// Items, ships and skills may be presented alphabetically (skill level > enabled skill) or grouped (skill level > enabled skill group > enabled skill).
        /// 
        /// Each item is colored appropriately : normal if we know all the prereqs for the item,
        /// dimmed if this level enables the item or red if we need other skills to enable the item in
        /// addition to this level of skill.
        /// </summary>
        private void UpdateTrees()
        {
            UpdateSkillsTree();
            UpdateItemsTree();
            UpdateTreesHeaders();
        }

        /// <summary>
        /// Update the trees' headers : "blueprints", "items" or "ships".
        /// </summary>
        private void UpdateTreesHeaders()
        {
            if (m_hasBlueprints && m_hasItems)
            {
                lblItems.BackColor = Color.Thistle;
                pnlItemHeader.BackColor = Color.Thistle;
                lblItems.Text = @"Enabled Blueprints and Items";
            }
            else if (m_hasShips && m_hasBlueprints)
            {
                lblItems.BackColor = Color.Lavender;
                pnlItemHeader.BackColor = Color.Lavender;
                lblItems.Text = @"Enabled Ships and Blueprints";
            }
            else if (m_hasShips && m_hasItems)
            {
                lblItems.BackColor = Color.Honeydew;
                pnlItemHeader.BackColor = Color.Honeydew;
                lblItems.Text = @"Enabled Ships and Items";
            }
            else if (m_hasBlueprints)
            {
                lblItems.BackColor = Color.LightBlue;
                pnlItemHeader.BackColor = Color.LightBlue;
                lblItems.Text = @"Enabled Blueprints";
            }
            else if (m_hasShips)
            {
                lblItems.BackColor = Color.LightCyan;
                pnlItemHeader.BackColor = Color.LightCyan;
                lblItems.Text = @"Enabled Ships";
            }
            else if (m_hasItems)
            {
                lblItems.BackColor = Color.MistyRose;
                pnlItemHeader.BackColor = Color.MistyRose;
                lblItems.Text = @"Enabled Items";
            }
            else
            {
                lblItems.BackColor = Color.WhiteSmoke;
                pnlItemHeader.BackColor = Color.WhiteSmoke;
                lblItems.Text = @"Enabled Ships, Blueprints or Items";
            }
        }

        /// <summary>
        /// Set up the enabled Skills tree.
        /// </summary>
        private void UpdateSkillsTree()
        {
            tvSkills.BeginUpdate();
            try
            {
                tvSkills.Nodes.Clear();
                if (m_skill == null)
                    return;

                IEnumerable<Skill> skills = m_skill.Character?.Skills ?? SkillCollection.Skills;

                for (int i = 1; i <= 5; i++)
                {
                    SkillLevel skillLevel = new SkillLevel(m_skill, i);

                    // Gets the enabled skills and check it's not empty
                    List<Skill> enabledSkills = skills
                        .Where(x => x.Prerequisites.Any(y => y.Skill == m_skill && y.Level == i) && x.IsPublic)
                        .ToList();

                    if (!enabledSkills.Any())
                        continue;

                    // Add a node for this skill level
                    AddNode(i, skillLevel, enabledSkills);
                    m_allSkillsExpanded = true;
                }

                // No enabled skill found for any level ?
                if (tvSkills.Nodes.Count == 0)
                    tvSkills.Nodes.Add(new TreeNode("No skills enabled by this skill"));
            }
            finally
            {
                tvSkills.EndUpdate();
            }
        }

        /// <summary>
        /// Adds the node.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="skillLevel">The skill level.</param>
        /// <param name="enabledSkills">The enabled skills.</param>
        private void AddNode(int i, SkillLevel skillLevel, IEnumerable<Skill> enabledSkills)
        {
            TreeNode levelNode = new TreeNode(skillLevel.ToString());
            if (m_skill.Level >= i)
                levelNode.Text += @" (Trained)";

            levelNode.ForeColor = Color.DarkBlue;

            // Is it a plain alphabetical presentation ?
            if (rbShowAlpha.Checked)
            {
                foreach (Skill skill in enabledSkills.OrderBy(x => x.Name))
                {
                    levelNode.Nodes.Add(CreateNode(skill, skill.Prerequisites.ToList()));
                }
            }
                // Or do we need to group skills by their groups ?
            else if (rbShowTree.Checked)
            {
                foreach (IGrouping<SkillGroup, Skill> group in enabledSkills.GroupBy(
                    x => x.Group).ToArray().OrderBy(x => x.Key.Name))
                {
                    TreeNode groupNode = new TreeNode(group.Key.Name);
                    foreach (Skill skill in group.ToArray().OrderBy(x => x.Name))
                    {
                        groupNode.Nodes.Add(CreateNode(skill, skill.Prerequisites.ToList()));
                    }
                    levelNode.Nodes.Add(groupNode);
                }
            }

            // Add node
            levelNode.Expand();
            tvSkills.Nodes.Add(levelNode);
        }

        /// <summary>
        /// Set up the items/ships/blueprints tree.
        /// </summary>
        private void UpdateItemsTree()
        {
            m_hasShips = false;
            m_hasItems = false;
            m_hasBlueprints = false;

            tvEntity.BeginUpdate();
            try
            {
                tvEntity.Nodes.Clear();
                if (m_skill == null)
                    return;

                List<Item> items = StaticItems.AllItems
                    .Concat(StaticBlueprints.AllBlueprints)
                    // exclude skills
                    .Where(x => x.MarketGroup.ParentGroup != null &&
                                x.MarketGroup.ParentGroup.ID != DBConstants.SkillsMarketGroupID)
                    .Where(x => x.Prerequisites.Any(y => y.Skill == m_skill.StaticData))
                    .Where(x => !cbShowBaseOnly.Checked || x.MetaGroup == ItemMetaGroup.T1 || x.MetaGroup == ItemMetaGroup.T2)
                    .ToList();

                // Scroll through levels
                for (int i = 1; i <= 5; i++)
                {
                    SkillLevel skillLevel = new SkillLevel(m_skill, i);

                    // Gets the enabled objects and check it's not empty
                    List<Item> enabledObjects = items
                        .Where(x => x.Prerequisites.Any(y => y.Skill == m_skill.StaticData && y.Level == i))
                        .ToList();

                    if (!enabledObjects.Any())
                        continue;

                    // Add a node for this skill level
                    AddNode(i, skillLevel, enabledObjects);
                    m_allObjectsExpanded = true;
                }
                
                // No enabled skill found for any level ?
                if (tvEntity.Nodes.Count == 0)
                    tvEntity.Nodes.Add(new TreeNode("No ships, blueprints or items enabled by this skill"));
            }
            finally
            {
                tvEntity.EndUpdate();
            }
        }

        /// <summary>
        /// Adds the node.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="skillLevel">The skill level.</param>
        /// <param name="enabledObjects">The enabled objects.</param>
        private void AddNode(int i, SkillLevel skillLevel, IList<Item> enabledObjects)
        {
            // Add a node for this skill level
            TreeNode levelNode = new TreeNode(skillLevel.ToString());
            if (m_skill.Level >= i)
                levelNode.Text += @" (Trained)";

            levelNode.ForeColor = Color.DarkBlue;

            // Is it a plain alphabetical presentation ?
            if (rbShowAlpha.Checked)
                GroupByAlphabet(i, levelNode, enabledObjects);
            // Or do we need to group items by their groups ?
            else if (rbShowTree.Checked)
                GroupByMarketGroup(i, levelNode, enabledObjects);

            // Add node
            levelNode.Expand();
            tvEntity.Nodes.Add(levelNode);
        }

        /// <summary>
        /// AllGroups the by alphabet.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="levelNode">The level node.</param>
        /// <param name="enabledObjects">The enabled objects.</param>
        private void GroupByAlphabet(int i, TreeNode levelNode, IList<Item> enabledObjects)
        {
            foreach (Ship ship in enabledObjects.OfType<Ship>().OrderBy(x => x.Name))
            {
                levelNode.Nodes.Add(CreateNode(ship, ship.Prerequisites.ToCharacter(m_character).ToList()));
                m_hasShips = true;
            }

            foreach (Blueprint blueprint in enabledObjects.OfType<Blueprint>().OrderBy(x => x.Name))
            {
                List<BlueprintActivity> listOfActivities = blueprint.Prerequisites
                    .Where(x => x.Skill == m_skill.StaticData && x.Level == i)
                    .Select(x => x.Activity).ToList();

                TreeNode node = CreateNode(blueprint, blueprint.Prerequisites
                    .Where(x => listOfActivities.Contains(x.Activity)).ToCharacter(m_character).ToList());

                node.Text =
                    $"{node.Text} ({string.Join(", ", listOfActivities.Select(activity => activity.GetDescription()))})";
                levelNode.Nodes.Add(node);
                m_hasBlueprints = true;
            }

            foreach (Item item in enabledObjects.Where(x => !(x is Ship) && !(x is Blueprint)).OrderBy(x => x.Name))
            {
                levelNode.Nodes.Add(CreateNode(item, item.Prerequisites.ToCharacter(m_character).ToList()));
                m_hasItems = true;
            }
        }

        /// <summary>
        /// AllGroups the by market group.
        /// </summary>
        /// <param name="levelNode">The level node.</param>
        /// <param name="i">The i.</param>
        /// <param name="enabledObjects">The enabled objects.</param>
        private void GroupByMarketGroup(int i, TreeNode levelNode, IList<Item> enabledObjects)
        {
            // Add ships
            IGrouping<MarketGroup, Ship>[] shipsToAdd = enabledObjects.OfType<Ship>()
                .GroupBy(x => x.MarketGroup.ParentGroup).ToArray();

            foreach (IGrouping<MarketGroup, Ship> shipGroup in shipsToAdd.OrderBy(x => x.Key.Name))
            {
                TreeNode groupNode = new TreeNode(shipGroup.Key.Name);
                foreach (Ship ship in shipGroup.OrderBy(x => x.Name))
                {
                    groupNode.Nodes.Add(CreateNode(ship, ship.Prerequisites.ToCharacter(m_skill.Character).ToList()));
                }
                levelNode.Nodes.Add(groupNode);
                m_hasShips = true;
            }

            // Add blueprints recursively                       
            foreach (TreeNode node in StaticBlueprints.BlueprintMarketGroups
                .SelectMany(
                    blueprintMarketGroup =>
                        CreateMarketGroupsNode(blueprintMarketGroup, enabledObjects.OfType<Blueprint>().ToList(), i)))
            {
                levelNode.Nodes.Add(node);
                m_hasBlueprints = true;
            }

            // Add items recursively
            foreach (TreeNode node in StaticItems.MarketGroups
                .SelectMany(marketGroup => CreateMarketGroupsNode(marketGroup, enabledObjects
                    .Where(x => !(x is Ship) && !(x is Blueprint)).ToList())))
            {
                levelNode.Nodes.Add(node);
                m_hasItems = true;
            }
        }

        /// <summary>
        /// Recursively creates tree nodes for the children market groups of the given group.
        /// The added blueprints will be the ones which require the current skill (<see cref="m_skill"/>) at the specified level.
        /// </summary>
        /// <param name="blueprintMarketGroup">The blueprint market group.</param>
        /// <param name="blueprints">The blueprints.</param>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        private IEnumerable<TreeNode> CreateMarketGroupsNode(BlueprintMarketGroup blueprintMarketGroup,
                                                             IList<Blueprint> blueprints, int level)
        {
            // Add categories
            foreach (BlueprintMarketGroup category in blueprintMarketGroup.SubGroups)
            {
                List<TreeNode> children = CreateMarketGroupsNode(category, blueprints, level).ToList();
                if (!children.Any())
                    continue;

                TreeNode node = new TreeNode(category.Name);
                node.Nodes.AddRange(children.ToArray());
                yield return node;
            }

            // Add blueprints
            foreach (Blueprint blueprint in blueprints.Where(x => x.MarketGroup == blueprintMarketGroup))
            {
                List<BlueprintActivity> listOfActivities = blueprint.Prerequisites
                    .Where(x => x.Skill == m_skill.StaticData && x.Level == level)
                    .Select(x => x.Activity).ToList();

                TreeNode node = CreateNode(blueprint, blueprint.Prerequisites
                    .Where(x => listOfActivities.Contains(x.Activity)).ToCharacter(m_character).ToList());

                node.Text = $"{node.Text} " +
                            $"({string.Join(", ", listOfActivities.Select(activity => activity.GetDescription()))})";
                yield return node;
            }
        }

        /// <summary>
        /// Recursively creates tree nodes for the children market groups of the given group.
        /// The added items will be the ones which require the current skill (<see cref="m_skill"/>) at the specified level.
        /// </summary>
        /// <param name="marketGroup">The market group.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        private IEnumerable<TreeNode> CreateMarketGroupsNode(MarketGroup marketGroup, IList<Item> items)
        {
            // Add categories
            foreach (MarketGroup category in marketGroup.SubGroups)
            {
                List<TreeNode> children = CreateMarketGroupsNode(category, items).ToList();
                if (!children.Any())
                    continue;

                TreeNode node = new TreeNode(category.Name);
                node.Nodes.AddRange(children.ToArray());
                yield return node;
            }

            // Add items
            foreach (Item item in items.Where(x => x.MarketGroup == marketGroup))
            {
                yield return CreateNode(item, item.Prerequisites.ToCharacter(m_character).ToList());
            }
        }

        /// <summary>
        /// Adds a node to the tree, and colors it appropriately, and set the tooltip.
        /// Also sets the tag of the node to the Skill object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="prerequisites">The object's prerequisites.</param>
        /// <returns></returns>
        private TreeNode CreateNode(object obj, IList<SkillLevel> prerequisites)
        {
            TreeNode node = new TreeNode(obj.ToString()) { ToolTipText = string.Empty, Tag = obj };

            // When all prereqs satisifed, keep the default color
            if (prerequisites.All(x => x.IsTrained))
                return node;

            // Are all other prerequisites known ?
            if (prerequisites.All(x => x.IsTrained || x.Skill == m_skill))
            {
                node.ForeColor = Color.Gray;
                return node;
            }

            // Then we need to list the other prerequisites
            StringBuilder sb = new StringBuilder("Also Need To Train:");
            foreach (SkillLevel prereq in CreatePrereqList(prerequisites.Where(x => x.Skill != m_skill && !x.IsTrained)))
            {
                sb
                    .AppendLine()
                    .Append(prereq);
            }

            node.ToolTipText = sb.ToString();
            node.ForeColor = Color.Red;
            return node;
        }

        /// <summary>
        /// Creates a prereq skills list for a blueprint with the given activity.
        /// </summary>
        /// <param name="prerequisites">The prerequisites of a blueprint activities</param>
        private static IEnumerable<SkillLevel> CreatePrereqList(IEnumerable<SkillLevel> prerequisites)
        {
            List<SkillLevel> prereqList = new List<SkillLevel>();
            foreach (SkillLevel prereq in prerequisites.Where(prereq => prereqList.All(x => x.ToString() != prereq.ToString())))
            {
                prereqList.Add(prereq);
            }

            return prereqList;
        }

        #endregion


        #region Global events and auto-update

        /// <summary>
        /// Occurs whenever the plan name changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_PlanNameChanged(object sender, PlanChangedEventArgs e)
        {
            if (e.Plan != m_plan)
                return;

            UpdatePlanName();
        }

        /// <summary>
        /// occurs whenever the character is updated from CCP, skills are estimed to have completed, etc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != m_character)
                return;

            UpdateContent();
        }

        /// <summary>
        /// Update skill header every 30s if we're training it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrAutoUpdate_Tick(object sender, EventArgs e)
        {
            UpdateHeader();
        }

        #endregion


        #region Skills context Menu

        /// <summary>
        /// Returns the currently selected skill or null if non is selected.
        /// </summary>
        /// <returns></returns>
        private Skill GetSelectedSkill() => tvSkills.SelectedNode?.Tag as Skill;

        /// <summary>
        /// When the user clicks, we check whether we must display the context menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void tvSkills_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            tvSkills.Cursor = Cursors.Default;

            // Updates selection
            tvSkills.SelectedNode = tvSkills.GetNodeAt(e.Location);

            // Show menu
            cmSkills.Show(tvSkills, e.Location);
        }

        /// <summary>
        /// When the mouse moves over the list, we change the cursor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void tvSkills_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;

            tvSkills.Cursor = tvSkills.GetNodeCount(true) > 1
                ? CustomCursors.ContextMenu
                : Cursors.Default;
        }

        /// <summary>
        /// Context menu opening, updates the menus status
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void cmSkills_Opening(object sender, CancelEventArgs e)
        {
            if (tvSkills.GetNodeCount(true) < 2)
            {
                e.Cancel = true;
                return;
            }

            planToMenu.Visible = m_plan != null && tvSkills.SelectedNode != null;
            planToSeparator.Visible = m_plan != null && tvSkills.SelectedNode != null;
            showInSkillBrowserMenu.Visible = showInBrowserSeperator.Visible =  tvSkills.SelectedNode != null;

            // "Expand All" and "Collapse All" menus
            tsmiSkillsCollapseAll.Enabled = tsmiSkillsCollapseAll.Visible = m_allSkillsExpanded;
            tsmiSkillsExpandAll.Enabled = tsmiSkillsExpandAll.Visible = !tsmiSkillsCollapseAll.Enabled;
            
            // "Plan to N" menus
            Skill skill = GetSelectedSkill();

            // Update the "show in skill explorer" menu
            showInSkillExplorerMenu.Visible = skill != null && m_skill != skill;

            // Update the "show prerequisites" menu
            showPrerequisitiesMenu.Visible = skill != null && !skill.ArePrerequisitesMet;

            showInMenuSeperator.Visible = skill != null;

            if (skill == null)
                return;

            // Update the "plan to X" menus
            planToMenu.Enabled = skill.Level < 5;

            if (m_plan == null)
                return;

            for (int i = 0; i <= 5; i++)
            {
                m_plan.UpdatesRegularPlanToMenu(planToMenu.DropDownItems[i], skill, i);
            }
        }

        /// <summary>
        /// Skill context menu > Plan to level 
        /// theres a menu item for each level, each one is tagged with a 
        /// string representing level number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void planToLevelMenuItem_Click(object sender, EventArgs e)
        {
            IPlanOperation operation = ((ToolStripMenuItem)sender).Tag as IPlanOperation;
            if (operation == null)
                return;

            PlanWindow planWindow = PlanWindow.ShowPlanWindow(plan: operation.Plan);
            if (planWindow == null)
                return;

            PlanHelper.SelectPerform(new PlanToOperationWindow(operation), planWindow, operation);
        }

        /// <summary>
        /// Skill context menu > Show me what this skill unlocks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showInSkillExplorerMenu_Click(object sender, EventArgs e)
        {
            SetSkill(GetSelectedSkill());
        }

        /// <summary>
        /// Skill context menu > Show skill in browser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showInSkillBrowserMenu_Click(object sender, EventArgs e)
        {
            Skill skill = GetSelectedSkill();

            // Open the skill browser
            PlanWindow.ShowPlanWindow(m_character, m_plan).ShowSkillInBrowser(skill);
        }

        /// <summary>
        /// Treeview's context menu > Expand All
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void tsmiExpandAll_Click(object sender, EventArgs e)
        {
            tvSkills.ExpandAll();
            m_allSkillsExpanded = true;
        }

        /// <summary>
        /// Treeview's context menu > Collapse All
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void tsmiColapseAll_Click(object sender, EventArgs e)
        {
            tvSkills.CollapseAll();
            m_allSkillsExpanded = false;
        }

        /// <summary>
        /// Skill Menu - Show all prereq stats in a dialog box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showPrerequisitiesMenu_Click(object sender, EventArgs e)
        {
            Skill skill = GetSelectedSkill();
            if (skill == null)
                return;

            int index = 0;
            StringBuilder sb = new StringBuilder();
            foreach (SkillLevel prereq in skill.Prerequisites)
            {
                FormatPrerequisite(sb, prereq, ref index);
            }

            MessageBox.Show(sb.ToString(), @"Untrained Prerequisites for " + skill.Name, MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        /// <summary>
        /// Helper method for the Show Prereqs menu used by both ship and item trees.
        /// This method adds one prereq to the string that will be displayed in the
        /// dialog box.
        /// </summary>
        /// <param name="sb">The StringBuilder object.</param>
        /// <param name="prereq">The prereq</param>
        /// <param name="index">The index.</param>
        private static void FormatPrerequisite(StringBuilder sb, SkillLevel prereq, ref int index)
        {
            if (prereq.Skill.IsKnown)
            {
                if (prereq.IsTrained)
                    return;

                // We know this prereq, but not to a high enough level
                index++;
                string level = prereq.Skill.Level > 0
                    ? $"(Trained to level {prereq.Skill.RomanLevel})"
                    : "(Not yet trained)";
                sb.AppendLine($"{index}. {prereq} {level}");
                return;
            }

            // We don't know this prereq at all
            index++;
            sb
                .Append($"{index}. {prereq} ")
                .Append($"(Prereqs {(prereq.Skill.Prerequisites.AreTrained() ? string.Empty : "not ")}met, ")
                .AppendLine($"skillbook {(prereq.Skill.IsOwned ? "owned)" : $"not owned, costs {prereq.Skill.FormattedCost} ISK)")}");
        }

        #endregion


        #region Items Ships and Blueprints context menu

        /// <summary>
        /// Returns the currently selected item or null if non is selected.
        /// </summary>
        /// <returns></returns>
        private Item GetSelectedItem() => tvEntity.SelectedNode?.Tag as Item;

        /// <summary>
        /// Gets the selected item activities.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private List<string> GetSelectedItemActivities(Item entity)
        {
            List<string> list =
                tvEntity.SelectedNode.Text.Replace(entity.Name, string.Empty).Trim().Trim("()".ToCharArray()).Split(',').ToList();

            if (list.First().Length == 0)
                list[0] = BlueprintActivity.None.GetDescription();

            return list;
        }

        /// <summary>
        /// When the user clicks, we check whether we must display the context menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void tvEntity_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            tvEntity.Cursor = Cursors.Default;

            // Updates selection
            tvEntity.SelectedNode = tvEntity.GetNodeAt(e.Location);

            // Show menu
            cmEntity.Show(tvEntity, e.Location);
        }

        /// <summary>
        /// When the mouse moves over the list, we change the cursor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void tvEntity_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;

            tvEntity.Cursor = tvEntity.GetNodeCount(true) > 1
                ? CustomCursors.ContextMenu
                : Cursors.Default;
        }

        /// <summary>
        /// Context menu opening, updates the menus status
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void cmEntity_Opening(object sender, CancelEventArgs e)
        {
            if (tvEntity.GetNodeCount(true) < 2)
            {
                e.Cancel = true;
                return;
            }

            Item item = GetSelectedItem();

            planToObject.Visible =
                planToObjectSeperator.Visible = m_plan != null && tvEntity.SelectedNode != null && item != null;
            showObjectInBrowserMenuItem.Visible =
                showObjectInBrowserSeperator.Visible = tvEntity.SelectedNode != null && item != null;

            if (item != null)
            {
                Blueprint blueprint = StaticBlueprints.GetBlueprintByID(item.ID);
                Ship ship = item as Ship;

                string text = ship != null ? "Ship" : blueprint != null ? "Blueprint" : "Item";

                showObjectInBrowserMenuItem.Text = $"Show In {text} Browser...";
            }

            // "Expand All" and "Collapse All" menus
            tsmiObjectsCollapseAll.Enabled = tsmiObjectsCollapseAll.Visible = m_allObjectsExpanded;
            tsmiObjectsExpandAll.Enabled = tsmiObjectsExpandAll.Visible = !tsmiObjectsCollapseAll.Enabled;

            List<string> listOfActivities = item != null ? GetSelectedItemActivities(item): new List<string>();

            // "Add to plan" is enabled if we don't know all the prereqs 
            // and we're not already planning at least one of the unknown prereqs
            planToObject.Enabled = item != null && m_plan != null && item.Prerequisites
                .Where(x => listOfActivities.Contains(x.Activity.GetDescription())).ToCharacter(m_character)
                .Any(x => !x.IsTrained && !m_plan.IsPlanned(x.Skill, x.Level));

            bool untrainedPrerequisitiesExists = item != null && !item.Prerequisites.ToCharacter(m_character).AreTrained();

            // Update the "show prerequisites" menu
            showObjectPrerequisitiesMenu.Visible = untrainedPrerequisitiesExists;
            showObjectInMenuSeperator.Visible = untrainedPrerequisitiesExists;
        }

        /// <summary>
        /// Shared context menu - add ship/item/blueprint to plan.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void planToObjectMenuItem_Click(object sender, EventArgs e)
        {
            Item entity = GetSelectedItem();

            if (entity == null)
                return;

            List<string> listOfActivities = GetSelectedItemActivities(entity);
            IPlanOperation operation = m_plan
                .TryAddSet(entity.Prerequisites
                    .Where(x => listOfActivities.Contains(x.Activity.GetDescription())), entity.Name);

            if (operation == null)
                return;

            PlanWindow planWindow = PlanWindow.ShowPlanWindow(plan: operation.Plan);
            if (planWindow == null)
                return;

            PlanHelper.Perform(new PlanToOperationWindow(operation), planWindow);
        }

        /// <summary>
        /// Shared Ship/Blueprint/Item Show Prereqs menu.
        /// Builds a nicely formatted list of prereqs for the ship/item/blueprint
        /// and shows them in a dialog box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsShowItemPrereqs_Click(object sender, EventArgs e)
        {
            Item entity = GetSelectedItem();
            if (entity == null)
                return;

            List<string> listOfActivities = GetSelectedItemActivities(entity);
            IEnumerable<SkillLevel> prereqList = CreatePrereqList(entity.Prerequisites.Where(
                x => listOfActivities.Contains(x.Activity.GetDescription())).ToCharacter(m_character));

            int index = 0;
            StringBuilder sb = new StringBuilder();
            foreach (SkillLevel prereq in prereqList)
            {
                FormatPrerequisite(sb, prereq, ref index);
            }

            MessageBox.Show(sb.ToString(), @"Untrained Prerequisites for " + entity.Name, MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        /// <summary>
        /// Treeview's context menu > Expand All
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void tsmiObjectExpandAll_Click(object sender, EventArgs e)
        {
            tvEntity.ExpandAll();
            m_allObjectsExpanded = true;
        }

        /// <summary>
        /// Treeview's context menu > Collapse All
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void tsmiObjectCollapseAll_Click(object sender, EventArgs e)
        {
            tvEntity.CollapseAll();
            m_allObjectsExpanded = false;
        }

        #endregion


        #region Controls' events

        /// <summary>
        /// Toggling the radio buttons to switch between sorted list and category views.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbShowAlpha_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTrees();
        }

        /// <summary>
        /// Toggling the "Show base items/show variants",
        /// just redisplay the items tree.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbShowBaseOnly_CheckedChanged(object sender, EventArgs e)
        {
            UpdateItemsTree();
        }

        /// <summary>
        /// Occurs when the window closes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Doubleclicks on a ship/item leaf node will show the ship/item in the browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvEntity_DoubleClick(object sender, EventArgs e)
        {
            Item item = GetSelectedItem();
            if (item == null)
                return;

            Item ship = item as Ship;
            if (ship != null)
            {
                PlanWindow.ShowPlanWindow(m_character, m_plan).ShowShipInBrowser(ship);
                return;
            }

            Item blueprint = item as Blueprint;
            if (blueprint != null)
            {
                PlanWindow.ShowPlanWindow(m_character, m_plan).ShowBlueprintInBrowser(blueprint);
                return;
            }

            PlanWindow.ShowPlanWindow(m_character, m_plan).ShowItemInBrowser(item);
        }

        /// <summary>
        /// Sklll context menu > Show me what this skill unlocks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvSkills_DoubleClick(object sender, EventArgs e)
        {
            SetSkill(GetSelectedSkill());
        }

        /// <summary>
        /// We want to go look at a skill in the history list again.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            Skill skill = (Skill)cbHistory.Items[cbHistory.SelectedIndex];
            SetSkill(m_character?.Skills[skill.ID] ?? skill);
        }

        #endregion


        #region Skill Explorer Window factory

        /// <summary>
        /// Shows the skill in the skill explorer.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <exception cref="System.ArgumentNullException">skill</exception>
        internal void ShowSkillInExplorer(Skill skill)
        {
            if (skill == null)
                return;

            // Quit if it's an "Unknown" skill
            if (skill.ID == int.MaxValue)
                return;

            SetSkill(skill);
        }

        /// <summary>
        /// Shows the skill explorer window.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="plan">The plan.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">skill</exception>
        internal static SkillExplorerWindow ShowSkillExplorerWindow(Character character, Plan plan = null)
        {
            // If no character is associated, open a unique Data Browser (non-associated character)
            if (character == null && plan == null)
                return WindowsFactory.ShowUnique<SkillExplorerWindow>();

            // Check if a Skill Planner is already open
            // (a Skill Planner has the same Tag as a Data Browser but it has a plan attached)
            PlanWindow planWindow = WindowsFactory.GetByTag<PlanWindow, Character>(character ?? (Character)plan.Character);

            SkillExplorerWindow skillExplorerWindow;

            // Do we have a Skill Planner open?
            if (planWindow?.Plan != null)
            {
                // Activate
                skillExplorerWindow =
                    WindowsFactory.ShowByTag<SkillExplorerWindow, Character>(character ?? (Character)planWindow.Plan?.Character);

                // If a plan was passed, assign the new plan
                if (plan != null)
                    skillExplorerWindow.Plan = planWindow.Plan;

                return skillExplorerWindow;
            }

            // No plan window found, open a new Skill Explorer
            if (planWindow == null || plan == null)
            {
                // Open a new Skill Explorer associated with the character
                if (plan == null)
                    return WindowsFactory.ShowByTag<SkillExplorerWindow, Character>(character);

                // Open a new Skill Explorer (use the plan as tag for the creating the window)
                // (This should be the only time a plan is used as the tag)
                skillExplorerWindow = WindowsFactory.ShowByTag<SkillExplorerWindow, Plan>(plan);

                // Change the tag (we changed it to the character for window lookup)
                WindowsFactory.ChangeTag<SkillExplorerWindow, Plan, Character>(plan, (Character)plan.Character);

                return skillExplorerWindow;
            }

            // Activate
            skillExplorerWindow = WindowsFactory.ShowByTag<SkillExplorerWindow, Character>(character);

            // It's a Data Browser, transform it to a Skill Planner
            skillExplorerWindow.Plan = plan;

            return skillExplorerWindow;
        }

        #endregion
    }
}