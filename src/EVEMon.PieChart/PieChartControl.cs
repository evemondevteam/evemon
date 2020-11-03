using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common.Extensions;

namespace EVEMon.PieChart
{
    /// <summary>
    /// Summary description for PieChartControl.
    /// </summary>
    public class PieChartControl : Panel
    {
        private float m_leftMargin;
        private float m_topMargin;
        private float m_rightMargin;
        private float m_bottomMargin;
        private bool m_fitChart;

        private decimal[] m_values;
        private Color[] m_colors;
        private float m_sliceRelativeHeight;
        private float[] m_relativeSliceDisplacements = new[] { 0F };
        private string[] m_texts;
        private string[] m_tootips;
        private ShadowStyle m_shadowStyle = ShadowStyle.GradualShadow;
        private EdgeColorType m_edgeColorType = EdgeColorType.SystemColor;
        private float m_edgeLineWidth = 1F;
        private float m_initialAngle;
        private int m_highlightedIndex = -1;
        private readonly ToolTip m_toolTip;

        private int m_lastX = -1;
        private int m_lastY = -1;

        // These are used for the actual drawing. They are modified depending
        // on wether sorting by size is on or off
        private decimal[] m_drawValues;
        private Color[] m_drawColors;
        private float[] m_drawRelativeSliceDisplacements = new[] { 0F };
        private string[] m_drawToolTipTexts;
        private string[] m_drawTexts;
        private int[] m_sortOrder;

        /// <summary>
        /// Default AutoPopDelay of the ToolTip control.
        /// </summary>
        private int m_defaultToolTipAutoPopDelay;

        /// <summary>
        /// Flag indicating that object has been disposed.
        /// </summary>
        private bool m_disposed;

        private bool m_mouseDown;

        /// <summary>
        /// Initializes the <c>PieChartControl</c>.
        /// </summary>
        public PieChartControl()
        {
            m_texts = null;
            m_tootips = null;
            PieChart = null;
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
            m_toolTip = new ToolTip();
        }

        /// <summary>
        /// Gets or sets the pie chart.
        /// </summary>
        /// <value>
        /// The pie chart.
        /// </value>
        public PieChart3D PieChart { get; private set; }

        /// <summary>
        /// Gets or sets colors to be used for rendering pie slices.
        /// </summary>
        /// <value>
        /// The colors.
        /// </value>
        [Browsable(false)]
        public IEnumerable<Color> Colors
        {
            get { return m_colors; }
            set
            {
                if (value == null)
                    return;

                m_colors = value.ToArray();
                Invalidate();
            }
        }

        /// <summary>
        /// Sets the left margin for the chart.
        /// </summary>
        /// <param name="left">The left.</param>
        public void LeftMargin(float left)
        {
            Debug.Assert(left >= 0);
            m_leftMargin = left;
            Invalidate();
        }

        /// <summary>
        ///   Sets the right margin for the chart.
        /// </summary>
        public void RightMargin(float right)
        {
            Debug.Assert(right >= 0);
            m_rightMargin = right;
            Invalidate();
        }

        /// <summary>
        /// Sets the top margin for the chart.
        /// </summary>
        /// <param name="top">The top.</param>
        public void TopMargin(float top)
        {
            Debug.Assert(top >= 0);
            m_topMargin = top;
            Invalidate();
        }

        /// <summary>
        /// Sets the bottom margin for the chart.
        /// </summary>
        /// <param name="bottom">The bottom.</param>
        public void BottomMargin(float bottom)
        {
            Debug.Assert(bottom >= 0);
            m_bottomMargin = bottom;
            Invalidate();
        }

        /// <summary>
        /// Sets the indicator if chart should fit the bounding rectangle
        /// exactly.
        /// </summary>
        /// <param name="fit">if set to <c>true</c> [fit].</param>
        public void FitChart(bool fit)
        {
            m_fitChart = fit;
            Invalidate();
        }


        /// <summary>
        /// Sets values to be represented by the chart.
        /// </summary>
        /// <param name="chartValues">The chart values.</param>
        public void Values(decimal[] chartValues)
        {
            m_values = chartValues;
            Invalidate();
        }

        /// <summary>
        /// Sets values for slice displacements.
        /// </summary>
        /// <param name="relativeDisplacements">The relative displacements.</param>
        public void SliceRelativeDisplacements(float[] relativeDisplacements)
        {
            m_relativeSliceDisplacements = relativeDisplacements;
            Invalidate();
        }

        /// <summary>
        /// Gets or sets tooltip texts.
        /// </summary>
        /// <param name="sliceTooltips">The slice tooltips.</param>
        public void ToolTips(string[] sliceTooltips)
        {
            m_tootips = sliceTooltips;
        }

        /// <summary>
        /// Sets texts appearing by each pie slice.
        /// </summary>
        /// <param name="sliceTexts">The slice texts.</param>
        public void Texts(string[] sliceTexts)
        {
            m_texts = sliceTexts;
        }

        /// <summary>
        /// Sets pie slice reative height.
        /// </summary>
        /// <param name="relativeHeight">Height of the relative.</param>
        public void SliceRelativeHeight(float relativeHeight)
        {
            m_sliceRelativeHeight = relativeHeight;
            Invalidate();
        }

        /// <summary>
        ///   Sets the shadow style.
        /// </summary>
        public void StyleOfShadow(ShadowStyle shadowStyle)
        {
            m_shadowStyle = shadowStyle;
            Invalidate();
        }

        /// <summary>
        /// Sets the edge color type.
        /// </summary>
        /// <param name="edgeColorType">Type of the edge color.</param>
        public void ColorTypeOfEdge(EdgeColorType edgeColorType)
        {
            m_edgeColorType = edgeColorType;
            Invalidate();
        }

        /// <summary>
        /// Sets the edge lines width.
        /// </summary>
        /// <param name="lineWidth">Width of the line.</param>
        public void EdgeLineWidth(float lineWidth)
        {
            m_edgeLineWidth = lineWidth;
            Invalidate();
        }

        /// <summary>
        ///   Sets the initial angle from which pies are drawn.
        /// </summary>
        public void InitialAngle(float angle)
        {
            float newAngle = angle;

            if (newAngle > 360.0f)
                newAngle -= 360.0f;

            if (newAngle < 0.0f)
                newAngle += 360.0f;

            OnAngleChange(new AngleChangeEventArgs(m_initialAngle, newAngle));

            m_initialAngle = newAngle;
            Invalidate();
        }

        /// <summary>
        /// Handles <c>OnPaint</c> event.
        /// </summary>
        /// <param name="e"><c>PaintEventArgs</c> object.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (HasAnyValue)
                DoDraw(e.Graphics);
        }

        /// <summary>
        /// Sets values for the chart and draws them.
        /// </summary>
        /// <param name="graphics">Graphics object used for drawing.</param>
        private void DoDraw(Graphics graphics)
        {
            if (m_drawValues == null || m_drawValues.Length <= 0)
                return;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            float width = ClientSize.Width - m_leftMargin - m_rightMargin;
            float height = ClientSize.Height - m_topMargin - m_bottomMargin;

            // If the width or height if <=0 an exception would be thrown -> exit method..
            if (width <= 0 || height <= 0)
                return;

            PieChart?.Dispose();

            if (m_drawColors != null && m_drawColors.Length > 0)
            {
                PieChart = new PieChart3D(m_leftMargin, m_topMargin, width, height, m_drawValues, m_drawColors,
                    m_sliceRelativeHeight, m_drawTexts);
            }
            else
            {
                PieChart = new PieChart3D(m_leftMargin, m_topMargin, width, height, m_drawValues, m_sliceRelativeHeight,
                    m_drawTexts);
            }

            PieChart.FitToBoundingRectangle(m_fitChart);
            PieChart.InitialAngle(m_initialAngle);
            PieChart.SliceRelativeDisplacements(m_drawRelativeSliceDisplacements);
            PieChart.ColorTypeOfEdge(m_edgeColorType);
            PieChart.EdgeLineWidth(m_edgeLineWidth);
            PieChart.StyleOfShadow(m_shadowStyle);
            PieChart.HighlightedIndex(m_highlightedIndex);
            PieChart.Draw(graphics);
            PieChart.Font(Font);
            PieChart.ForeColor(ForeColor);
            PieChart.PlaceTexts(graphics);
        }

        /// <summary>
        /// Handles <c>MouseEnter</c> event to activate the tooltip.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            m_defaultToolTipAutoPopDelay = m_toolTip.AutoPopDelay;
            m_toolTip.AutoPopDelay = short.MaxValue;
        }

        /// <summary>
        /// Handles <c>MouseLeave</c> event to disable tooltip.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            m_toolTip.RemoveAll();
            m_toolTip.AutoPopDelay = m_defaultToolTipAutoPopDelay;
            m_highlightedIndex = -1;
            Refresh();
        }

        /// <summary>
        /// Handles <c>MouseDown</c> event
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            m_mouseDown = true;
        }

        /// <summary>
        /// Handles <c>MouseUp</c> event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            m_mouseDown = false;
        }

        /// <summary>
        /// Handles <c>MouseMove</c> event to display tooltip for the pie
        /// slice under pointer and to display slice in highlighted color.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (PieChart == null || m_values == null || m_values.Length <= 0)
                return;

            if (e.X == m_lastX && e.Y == m_lastY)
                return;

            if (m_mouseDown)
            {
                float newAngle = m_initialAngle - (e.X - m_lastX);
                InitialAngle(newAngle);
            }
            else
            {
                int index = PieChart.FindPieSliceUnderPoint(new PointF(e.X, e.Y));

                if (index != m_highlightedIndex)
                {
                    m_highlightedIndex = index;
                    Refresh();
                }

                if (m_highlightedIndex != -1)
                {
                    if (m_drawToolTipTexts == null || m_drawToolTipTexts.Length <= m_highlightedIndex ||
                        m_drawToolTipTexts[m_highlightedIndex].Length == 0)
                    {
                        m_toolTip.SetToolTip(this, m_values[m_highlightedIndex].ToString(CultureInfo.CurrentCulture));
                    }
                    else
                        m_toolTip.SetToolTip(this, m_drawToolTipTexts[m_highlightedIndex]);
                }
                else
                    m_toolTip.RemoveAll();
            }

            m_lastX = e.X;
            m_lastY = e.Y;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources;
        ///  false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            try
            {
                if (disposing)
                {
                    PieChart?.Dispose();

                    m_toolTip.Dispose();
                }
                m_disposed = true;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Gets a flag indicating if at least one value is nonzero.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has any value; otherwise, <c>false</c>.
        /// </value>
        private bool HasAnyValue => m_values != null && m_values.Any(angle => angle != 0);

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <param name="sliceIndex">Index of the slice.</param>
        /// <returns></returns>
        public int GetIndex(int sliceIndex) => m_sortOrder[sliceIndex];

        /// <summary>
        /// Event for when the graph angle changes.
        /// </summary>
        public event EventHandler AngleChange;

        /// <summary>
        /// Event for when the graph angle changes
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnAngleChange(EventArgs e)
        {
            AngleChange?.ThreadSafeInvoke(this, e);
        }

        /// <summary>
        /// Will copy the original data to the vars used for drawing.
        /// The original is needed to use for ordering.
        /// </summary>
        private void CopyDataToDrawVars()
        {
            m_drawValues = (decimal[])m_values.Clone();
            m_drawColors = (Color[])m_colors.Clone();
            m_drawRelativeSliceDisplacements = (float[])m_relativeSliceDisplacements.Clone();
            m_drawToolTipTexts = (string[])m_tootips.Clone();
            m_drawTexts = (string[])m_texts.Clone();


            // fill the sort order to default:
            m_sortOrder = new int[m_values.Length];
            for (int i = 0; i < m_values.Length; i++)
            {
                m_sortOrder[i] = i;
            }
        }

        /// <summary>
        /// Orders the slices.
        /// </summary>
        /// <param name="orderBySize">if set to <c>true</c> [order by size].</param>
        public void OrderSlices(bool orderBySize)
        {
            if (orderBySize && m_values != null)
            {
                // prefill the draw vars
                CopyDataToDrawVars();

                // take a copy of the original values
                // then use it to do the calculations
                decimal[] values = (decimal[])m_values.Clone();
                Color[] colours = (Color[])m_colors.Clone();
                float[] displacements = (float[])m_relativeSliceDisplacements.Clone();
                string[] tooltips = (string[])m_tootips.Clone();
                string[] texts = (string[])m_texts.Clone();

                // reordering the slices
                for (int num = 0; num < values.Length; num++)
                {
                    decimal tempsp = decimal.MinValue;
                    int biggest = -1;
                    for (int y = 0; y < values.Length; y++)
                    {
                        if (biggest == -1)
                        {
                            tempsp = values[y];
                            biggest = y;
                        }
                        if (values[y] <= tempsp || values[y] <= 0)
                            continue;

                        tempsp = values[y];
                        biggest = y;
                    }

                    m_drawValues[num] = values[biggest];
                    m_drawTexts[num] = texts[biggest];
                    m_drawRelativeSliceDisplacements[num] = displacements[biggest];
                    m_drawToolTipTexts[num] = tooltips[biggest];
                    m_drawColors[num] = colours[biggest];
                    m_sortOrder[num] = biggest;
                    values[biggest] = 0;
                }
            }
            else
            {
                if (m_values != null)
                    CopyDataToDrawVars();
            }

            Refresh();
        }
    }
}