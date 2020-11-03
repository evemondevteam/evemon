using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common.Constants;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models;

namespace EVEMon.Common.Controls
{
    public class SkillQueueControl : Control
    {
        private readonly InfiniteDisplayToolTip m_toolTip;

        private static DateTime s_nextRepainting = DateTime.MinValue;

        private SkillQueue m_skillQueue;
        private Color m_lessThanDayFirstColor = Color.Yellow;
        private Color m_lessThanDaySecondColor = Color.DarkKhaki;
        private Color m_moreThanDayFirstColor = Color.LightBlue;
        private Color m_moreThanDaySecondColor = Color.DarkBlue;
        private Color m_emptyColor = Color.DimGray;
        private Color m_borderColor = Color.Gray;
        private Point m_lastLocation = new Point(-1, -1);


        #region Constructors, disposing, global events

        /// <summary>
        /// Creates the skill queue control without an associates skill queue.
        /// </summary>
        public SkillQueueControl()
        {
            m_toolTip = new InfiniteDisplayToolTip(this);

            Disposed += OnDisposed;
            EveMonClient.TimerTick += EveMonClient_TimerTick;
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            EveMonClient.CharacterUpdated += EveMonClient_CharacterUpdated;
        }

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            Disposed -= OnDisposed;
            EveMonClient.TimerTick -= EveMonClient_TimerTick;
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            EveMonClient.CharacterUpdated -= EveMonClient_CharacterUpdated;
            m_toolTip.Dispose();
        }

        /// <summary>
        /// Every second, we checks whether we should update the display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            if (!Visible)
                return;

            if (DateTime.Now > s_nextRepainting)
                Invalidate();
        }

        /// <summary>
        /// When the settings changed, the "SafeForWork" propety which affects our color schemes may have changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            if (Visible)
                Invalidate();
        }

        /// <summary>
        /// When the character changes, we invalidate the repainting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (!Visible)
                return;

            CCPCharacter ccpCharacter = e.Character as CCPCharacter;

            // Current character isn't a CCP character, so can't have a Queue.
            if (ccpCharacter == null)
                return;

            if (m_skillQueue == null || ccpCharacter.SkillQueue != m_skillQueue)
                return;

            Invalidate();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Skill Queue to be rendered.
        /// </summary>
        [Category("Data")]
        [Description("Skill queue to render on the control canvas")]
        public SkillQueue SkillQueue
        {
            get { return m_skillQueue; }
            set
            {
                m_skillQueue = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The first of two colors to be used in the queue for less than day.
        /// </summary>
        [Category("Appearance")]
        [Description("Less than day first color of the component")]
        public Color LessThanDayFirstColor
        {
            get { return m_lessThanDayFirstColor; }

            set
            {
                m_lessThanDayFirstColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The second of two colours to be used in the queue for less than day.
        /// </summary>
        [Category("Appearance")]
        [Description("Less than day second color of the component")]
        public Color LessThanDaySecondColor
        {
            get { return m_lessThanDaySecondColor; }
            set
            {
                m_lessThanDaySecondColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The first of two colors to be used in the queue for more than day.
        /// </summary>
        [Category("Appearance")]
        [Description("Less than day first color of the component")]
        public Color MoreThanDayFirstColor
        {
            get { return m_moreThanDayFirstColor; }

            set
            {
                m_moreThanDayFirstColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The second of two colours to be used in the queue for more than day.
        /// </summary>
        [Category("Appearance")]
        [Description("More than day second color of the component")]
        public Color MoreThanDaySecondColor
        {
            get { return m_moreThanDaySecondColor; }
            set
            {
                m_moreThanDaySecondColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The color used for the free space at the end of the queue.
        /// </summary>
        [Category("Appearance")]
        [Description("Color used for the free space at the end of the queue when there are less than 24h of training queued.")]
        public Color EmptyColor
        {
            get { return m_emptyColor; }
            set
            {
                m_emptyColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The color used for the border of the queue.
        /// </summary>
        [Category("Appearance")]
        [Description("Color used for the border of the queue.")]
        public Color BorderColor
        {
            get { return m_borderColor; }
            set
            {
                m_borderColor = value;
                Invalidate();
            }
        }

        #endregion


        #region Overridden Methods

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible)
                Invalidate();
        }

        /// <summary>
        /// Paint the skill queue to the control surface.
        /// </summary>
        /// <remarks>
        /// e.Graphics is control surface. Width and Height are
        /// derived from the control itself not e.ClipRectangle 
        /// which could point to part of the control.
        /// </remarks>
        /// <param name="e">Paint Event</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            int width = Width;
            int height = Height;

            // If we are in DesignMode we just paint a dummy queue
            if (DesignMode || this.IsDesignModeHosted())
            {
                PaintDesignerQueue(g, width, height);
                return;
            }

            PaintQueue(g, width, height);

            // We need to update the painting only every (skillqueue end time hour / width in pixels)
            s_nextRepainting = DateTime.Now.AddHours((double)m_skillQueue.EndTime.Hour / width);
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Get the less than day first of the two alternating colours.
        /// </summary>
        /// <remarks>
        /// Implements safe for work functionality
        /// </remarks>
        /// <returns>First Colour property, or dark Gray if in safe for work mode</returns>
        private Color GetLessThanDayFirstColor() => Settings.UI.SafeForWork ? Color.LightGray : m_lessThanDayFirstColor;

        /// <summary>
        /// Get the less than day second of the two alternating colours.
        /// </summary>
        /// <remarks>
        /// Implements safe for work functionality
        /// </remarks>
        /// <returns>Second Colour property, or gray if in safe for work mode</returns>
        private Color GetLessThanDaySecondColor() => Settings.UI.SafeForWork ? Color.Gray : m_lessThanDaySecondColor;

        /// <summary>
        /// Get the more than day first of the two alternating colours.
        /// </summary>
        /// <remarks>
        /// Implements safe for work functionality
        /// </remarks>
        /// <returns>First Colour property, or dark Gray if in safe for work mode</returns>
        private Color GetMoreThanDayFirstColor() => Settings.UI.SafeForWork ? Color.DarkGray : m_moreThanDayFirstColor;

        /// <summary>
        /// Get the more than day second of the two alternating colours.
        /// </summary>
        /// <remarks>
        /// Implements safe for work functionality
        /// </remarks>
        /// <returns>Second Colour property, or gray if in safe for work mode</returns>
        private Color GetMoreThanDaySecondColor() => Settings.UI.SafeForWork ? Color.DimGray : m_moreThanDaySecondColor;

        /// <summary>
        /// Gets the color for the free time.
        /// </summary>
        /// <remarks>
        /// Implements safe for work functionality
        /// </remarks>
        /// <returns>Second Colour property, or dim gray if in safe for work mode</returns>
        private Color GetEmptyColor() => Settings.UI.SafeForWork ? Color.DimGray : m_emptyColor;

        /// <summary>
        /// Gets the border color.
        /// </summary>
        /// <remarks>
        /// Implements safe for work functionality
        /// </remarks>
        /// <returns>Second Colour property, or black if in safe for work mode</returns>
        private Color GetBorderColor() => Settings.UI.SafeForWork ? Color.Black : m_borderColor;

        /// <summary>
        /// Paints the first 24 hours of the skill queue including the
        /// point if the queue has more than 24 hours contained within it.
        /// </summary>
        /// <param name="g">Graphics canvas</param>
        /// <param name="width">Width of the canvas</param>
        /// <param name="height">Height of the canvas</param>
        private void PaintQueue(Graphics g, int width, int height)
        {
            Brush[] lessThanDayBrushes =
            {
                new SolidBrush(GetLessThanDayFirstColor()),
                new SolidBrush(GetLessThanDaySecondColor()),
            };
            Brush[] moreThanDayBrushes =
            {
                new SolidBrush(GetMoreThanDayFirstColor()),
                new SolidBrush(GetMoreThanDaySecondColor())
            };

            try
            {
                if (m_skillQueue == null)
                    return;

                int brushNumber = 0;
                float lastX = 0f;
                double oneDaySkillQueueWidth = m_skillQueue.GetOneDaySkillQueueWidth(width);

                foreach (QueuedSkill skill in m_skillQueue)
                {
                    IList<RectangleF> skillRects = m_skillQueue.GetSkillRects(skill, width, height).ToList();
                    RectangleF skillRectFirst = skillRects.First();

                    // Skill starts before the 24h marker
                    if (skillRectFirst.X < oneDaySkillQueueWidth)
                    {
                        // Copy the brush for internal use
                        int internalBrushNumber = brushNumber;

                        // Iterate only through rectangles with width as they tamper with the lastX value
                        foreach (RectangleF skillRect in skillRects.Skip(1).Where(rect => rect.Width > 0))
                        {
                            Brush[] brushes = lessThanDayBrushes;
                            if (oneDaySkillQueueWidth - skillRect.X <= 0)
                            {
                                brushes = moreThanDayBrushes;

                                // Rotate the brush
                                internalBrushNumber = (internalBrushNumber + 1) % brushes.Length;
                            }

                            internalBrushNumber = PaintRect(g, brushes, skillRect, internalBrushNumber);
                            lastX = skillRect.Right;
                        }

                        // Rotate the brush
                        brushNumber = (brushNumber + 1) % lessThanDayBrushes.Length;

                        continue;
                    }

                    brushNumber = PaintRect(g, moreThanDayBrushes, skillRectFirst, brushNumber);
                    lastX = skillRectFirst.Right;
                }

                // If there are less than 24 hours in the queue draw a dark region at the end and the border
                if (!m_skillQueue.LessThanWarningThreshold)
                    return;

                // Empty region
                RectangleF emptyRect = new RectangleF(lastX, 0, width - lastX, Height);
                using (SolidBrush brush = new SolidBrush(GetEmptyColor()))
                {
                    g.FillRectangle(brush, emptyRect);
                }

                // Then the border
                using (Pen pen = new Pen(GetBorderColor(), 1.0f))
                {
                    g.DrawRectangle(pen, 0, 0, width - 1, height - 1);
                }
            }
            finally
            {
                foreach (Brush brush in lessThanDayBrushes.Concat(moreThanDayBrushes))
                {
                    brush.Dispose();
                }
            }
        }

        /// <summary>
        /// Paints the rectangle.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="brushes">The brushes.</param>
        /// <param name="skillRect">The skill rect.</param>
        /// <param name="brushNumber">The brush number.</param>
        /// <returns></returns>
        private static int PaintRect(Graphics g, IReadOnlyList<Brush> brushes, RectangleF skillRect, int brushNumber)
        {
            g.FillRectangle(brushes[brushNumber], skillRect);

            // Rotate the brush
            brushNumber = (brushNumber + 1) % brushes.Count;

            return brushNumber;
        }

        /// <summary>
        /// Displays the skill tool tip.
        /// </summary>
        /// <param name="skillRect">The skill rect.</param>
        /// <param name="skill">The skill.</param>
        private void DisplaySkillToolTip(RectangleF skillRect, QueuedSkill skill)
        {
            const string Format = "{0} {1}\n  Start{2}\t{3}\n  Ends\t{4}";
            string skillName = skill.SkillName;
            string skillLevel = Skill.GetRomanFromInt(skill.Level);
            string skillStart = skill.Owner.IsTraining
                ? skill.StartTime.ToLocalTime().ToAbsoluteDateTimeDescription(DateTimeKind.Local)
                : "Paused";
            string skillEnd = skill.Owner.IsTraining
                ? skill.EndTime.ToLocalTime().ToAbsoluteDateTimeDescription(DateTimeKind.Local)
                : "Paused";
            string startText = skill.StartTime < DateTime.UtcNow ? "ed" : "s";
            string text = string.Format(CultureConstants.DefaultCulture, Format, skillName, skillLevel, startText, skillStart,
                skillEnd);
            Size textSize = TextRenderer.MeasureText(text, Font);
            Size toolTipSize = new Size(textSize.Width + 13, textSize.Height + 11);
            Point tipPoint = new Point((int)(Math.Min(skillRect.Right, Width) + skillRect.Left) / 2 - toolTipSize.Width / 2,
                -toolTipSize.Height);
            tipPoint.Offset(0, -21);
            m_toolTip.Show(text, tipPoint);
        }

        /// <summary>
        /// Displays the free room tool tip.
        /// </summary>
        /// <param name="emptyRect">The empty rect.</param>
        private void DisplayFreeRoomToolTip(RectangleF emptyRect)
        {
            int remaining = EveConstants.MaxSkillsInQueue - m_skillQueue.Count;
            string text = $"Room for {remaining} more skill{(remaining == 1 ? string.Empty : "s")}";
            Size textSize = TextRenderer.MeasureText(text, Font);
            Size toolTipSize = new Size(textSize.Width + 13, textSize.Height + 11);
            Point tipPoint = new Point((int)(emptyRect.Right + emptyRect.Left) / 2 - toolTipSize.Width / 2, -toolTipSize.Height);
            tipPoint.Offset(0, -21);
            m_toolTip.Show(text, tipPoint);
        }

        /// <summary>
        /// Triggers when the mouse is moved displays skill.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (m_skillQueue == null)
                return;

            // Prevent rapid triggering of event when the mouse hasn't moved
            if (e.Location == m_lastLocation)
                return;

            m_lastLocation = e.Location;

            float lastX = 0f;
            foreach (QueuedSkill skill in m_skillQueue)
            {
                // Find the rectangle for the skill
                RectangleF skillRect = m_skillQueue.GetSkillRects(skill, Width, Height).First();
                lastX = skillRect.Right;

                if (!skillRect.Contains(e.Location))
                    continue;

                DisplaySkillToolTip(skillRect, skill);
                return;
            }

            // Are we in the empty space ?
            RectangleF emptyRect = new RectangleF(lastX, 0, Width - lastX, Height);
            if (emptyRect.Contains(e.Location))
            {
                DisplayFreeRoomToolTip(emptyRect);
                return;
            }

            m_toolTip.Hide();
        }

        /// <summary>
        /// Hide tooltip when mouse moves out of skill queue.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            m_toolTip.Hide();
        }

        /// <summary>
        /// Spit out a static skill queue for the designer.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void PaintDesignerQueue(Graphics g, int width, int height)
        {
            using (Brush lessThanDayFirstBrush = new SolidBrush(GetLessThanDayFirstColor()))
            using (Brush lessThanDaySecondBrush = new SolidBrush(GetLessThanDaySecondColor()))
            using (Brush moreThanDayFirstBrush = new SolidBrush(GetMoreThanDayFirstColor()))
            using (Brush moreThanDaySecondBrush = new SolidBrush(GetMoreThanDaySecondColor()))
            using (Brush emptyBrush = new SolidBrush(GetEmptyColor()))
            {
                g.FillRectangle(lessThanDayFirstBrush, new RectangleF(0, 0, width / 5f, height));
                g.FillRectangle(lessThanDaySecondBrush, new RectangleF(width / 5f, 0, width / 5f, height));
                g.FillRectangle(moreThanDayFirstBrush, new RectangleF(width * 2 / 5f, 0, width / 5f, height));
                g.FillRectangle(moreThanDaySecondBrush, new RectangleF(width * 3 / 5f, 0, width / 5f, height));
                g.FillRectangle(emptyBrush, new RectangleF(width * 4 / 5f, 0, width / 5f, height));
            }
        }

        #endregion
    }
}