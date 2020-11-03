using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Extensions;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// This control shows the value of an attribute in form of cells.
    /// Also it allows to change value by clicking on a cell.
    /// </summary>
    public class AttributeBarControl : Control
    {
        private Pen m_borderPen = Pens.Black;
        private Pen m_outerBorderPen = Pens.LightGray;
        private SolidBrush m_inactiveBrush = new SolidBrush(Color.DimGray);
        private SolidBrush m_basePointBrush = new SolidBrush(Color.LightGray);
        private SolidBrush m_spentPointBrush = new SolidBrush(Color.LimeGreen);
        
        private MouseEventArgs m_mouseEvent;

        private int m_points = 5;
        private int m_baseValue;
        private long m_value;
        private int m_tileWidth = 6;
        private int m_tileHeight = 20;
        private long m_highlightedItem = -1;

        private readonly Timer m_timer = new Timer();

        /// <summary>
        /// Initializes a new instance of <see cref="AttributeBarControl"/>.
        /// </summary>
        public AttributeBarControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.Opaque |
                     ControlStyles.UserPaint, true);
            UpdateStyles();

            m_timer.Interval = 500;
            m_timer.Tick += m_timer_Tick;
        }

        /// <summary>
        /// Gets or sets the delta value.
        /// </summary>
        /// <value>The delta value.</value>
        internal long DeltaValue { get; set; }

        /// <summary>
        /// Gets or sets the highlighed value.
        /// </summary>
        /// <value>The highlighed value.</value>
        internal long HighlightedValue { private get; set; }

        /// <summary>
        /// Gets or sets the color of the border between cells.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Black")]
        public Color BorderColor
        {
            get { return m_borderPen.Color; }
            set
            {
                m_borderPen = new Pen(value);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the outer border color.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "LightGray")]
        public Color OuterBorderColor
        {
            get { return m_outerBorderPen.Color; }
            set
            {
                m_outerBorderPen = new Pen(value);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the color of an inactive cell.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "DimGray")]
        public Color InactiveColor
        {
            get { return m_inactiveBrush.Color; }
            set
            {
                m_inactiveBrush = new SolidBrush(value);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the color of a cell for base point.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "LightGray")]
        public Color BasePointColor
        {
            get { return m_basePointBrush.Color; }
            set
            {
                m_basePointBrush = new SolidBrush(value);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the color of a cell for spent point.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "LimeGreen")]
        public Color SpentPointColor
        {
            get { return m_spentPointBrush.Color; }
            set
            {
                m_spentPointBrush = new SolidBrush(value);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of points (cells).
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(5)]
        public int MaxPoints
        {
            get { return m_points; }
            set
            {
                if (m_points == value)
                    return;

                m_points = value;
                int width = m_tileWidth * m_points + 3;
                Size = new Size(width, Height);
            }
        }

        /// <summary>
        /// Gets or sets the base value for the attribute.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(0)]
        public int BaseValue
        {
            get { return m_baseValue; }
            set
            {
                if (m_baseValue == value)
                    return;

                m_baseValue = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the value of the attribute.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(0)]
        public long Value
        {
            get { return m_value; }
            set
            {
                if (m_value == value)
                    return;

                m_value = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Occurs when value changing.
        /// </summary>
        [Category("Behavior")]
        public event EventHandler<AttributeValueChangingEventArgs> ValueChanging;

        /// <summary>
        /// Occurs when value changed.
        /// </summary>
        [Category("Behavior")]
        public event EventHandler<AttributeValueChangedEventArgs> ValueChanged;

        /// <summary>
        /// Occurs when highlighting.
        /// </summary>
        [Category("Behavior")]
        public event EventHandler<AttributeHighlightingEventArgs> Highlighting;

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Resize"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            m_tileHeight = Height - 4;

            // Calculate tile width
            m_tileWidth = (Width - 3) / m_points;

            // Calculate width of control from width of tiles
            int width = m_tileWidth * m_points + 3;
            if (Width != width)
                Size = new Size(width, Height);
        }

        /// <summary>
        /// Returns the index of a tile at specified point.
        /// </summary>
        /// <param name="location">Location to check</param>
        /// <returns>Tile index</returns>
        private int GetValueAt(Point location)
        {
            if (location.Y == 0 || location.Y == Height - 1)
                return -1;

            if (location.X == 0 || location.X == Width - 1)
                return -1;

            // Return not more than the number of cells
            return Math.Min((location.X - 1) / m_tileWidth + 1, m_points);
        }

        /// <summary>
        /// Changes highlighted tile and causes invalidation of it's region.
        /// </summary>
        /// <param name="newHighlight">Index of the new highlighted tile. Can be -1 if no one is highlighted.</param>
        private void ChangeHighlight(long newHighlight)
        {
            long previousHighlighted = m_highlightedItem;
            m_highlightedItem = newHighlight;

            if (previousHighlighted < 0 && m_highlightedItem < 0)
                return;

            // Invalidate changed areas
            if (m_highlightedItem >= 0)
                Invalidate(new Rectangle(Convert.ToInt32(m_highlightedItem * m_tileWidth + 1), 2, m_tileWidth, m_tileHeight));

            if (previousHighlighted >= 0)
                Invalidate(new Rectangle(Convert.ToInt32(previousHighlighted * m_tileWidth + 1), 2, m_tileWidth, m_tileHeight));
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseMove"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!Enabled)
                return;

            // Change cursor if required
            int value = GetValueAt(e.Location);
            Cursor = value == -1 ? Cursors.Arrow : Cursors.Hand;

            HighlightedValue = value;
            if (HighlightedValue >= 0 && HighlightedValue < m_baseValue)
                HighlightedValue = m_baseValue;

            Highlighting?.ThreadSafeInvoke(this, new AttributeHighlightingEventArgs(HighlightedValue));

            // To zero-based value
            HighlightedValue--;

            if (m_highlightedItem == HighlightedValue)
                return;

            ChangeHighlight(HighlightedValue);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseLeave"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            ChangeHighlight(-1);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseClick"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Store the single mouse click event
            m_mouseEvent = e;
            m_timer.Start();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseDoubleClick"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            // Store the double mouse click event
            m_mouseEvent = e;
            m_timer.Start();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (Graphics g = e.Graphics)
            {
                // Draw the borders
                g.DrawRectangle(m_outerBorderPen, 0, 0, Width - 1, Height - 1);
                g.DrawRectangle(m_borderPen, 1, 1, Width - 3, Height - 3);

                // Draw the tiles
                for (int iTile = 0; iTile < m_points; iTile++)
                {
                    DrawTile(g, iTile);
                }
            }
        }

        /// <summary>
        /// Handles the Tick event of the m_timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void m_timer_Tick(object sender, EventArgs e)
        {
            m_timer.Stop();

            if (!Enabled)
                return;

            // Actions according to mouse clicks
            switch (m_mouseEvent.Clicks)
            {
                case 1:
                    {
                        base.OnMouseClick(m_mouseEvent);

                        int newValue = GetValueAt(m_mouseEvent.Location);

                        if (newValue == -1)
                            return;

                        if (newValue < m_baseValue)
                            newValue = m_baseValue;

                        DeltaValue = newValue - m_value;

                        if (DeltaValue == 0)
                            return;

                        // Fires the value changing event
                        ValueChanging?.ThreadSafeInvoke(this, new AttributeValueChangingEventArgs(DeltaValue));

                        if (DeltaValue == 0)
                            return;

                        Value += DeltaValue;
                    }
                    break;
                case 2:
                    {
                        base.OnMouseDoubleClick(m_mouseEvent);

                        Value = m_baseValue;
                    }
                    break;
            }

            // Fires the value changed event
            ValueChanged?.ThreadSafeInvoke(this, new AttributeValueChangedEventArgs());
        }

        /// <summary>
        /// Draws a tile.
        /// </summary>
        /// <param name="g">A <see cref="System.Drawing.Graphics"/> object for drawing</param>
        /// <param name="iTile">Index of the tile</param>
        private void DrawTile(Graphics g, int iTile)
        {
            // Select brush
            SolidBrush brush;
            if (iTile >= m_value)
                brush = m_inactiveBrush;
            else if (iTile >= m_baseValue - 1)
                brush = m_spentPointBrush;
            else
                brush = m_basePointBrush;

            if (iTile == m_highlightedItem)
            {
                // Highlight cell color
                const int Shift = 50;
                brush = ShiftBrushColor(brush.Color, Shift);
            }

            int x = 1 + iTile * m_tileWidth;

            // Draw the tile
            g.FillRectangle(brush, x, 2, m_tileWidth, m_tileHeight);

            if (iTile == m_highlightedItem)
                brush.Dispose();

            // Draw the tile's border
            g.DrawLine(m_borderPen, x, 2, x, Height - 2);
        }

        /// <summary>
        /// Makes a color lighter or darker.
        /// </summary>
        /// <param name="brushColor">Color of the brush.</param>
        /// <param name="shift">Color shift</param>
        /// <returns></returns>
        private static SolidBrush ShiftBrushColor(Color brushColor, int shift)
        {
            Color color = Color.FromArgb(Math.Min(brushColor.R + shift, 255),
                                         Math.Min(brushColor.G + shift, 255),
                                         Math.Min(brushColor.B + shift, 255));
            return new SolidBrush(color);
        }
    }
}