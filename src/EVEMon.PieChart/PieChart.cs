using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using EVEMon.Common.Extensions;

namespace EVEMon.PieChart
{
    /// <summary>
    ///   Object representing a pie chart.
    /// </summary>
    public sealed class PieChart3D : IDisposable
    {
        /// <summary>
        ///   Slice relative height.
        /// </summary>
        private float m_sliceRelativeHeight;

        /// <summary>
        ///   Initial angle from which chart is drawn.
        /// </summary>
        private float m_initialAngle;

        /// <summary>
        ///   Array of ordered pie slices constituting the chart, starting from 
        ///   270 degrees axis.
        /// </summary>
        private PieSlice[] m_pieSlices;

        /// <summary>
        ///   Collection of reordered pie slices mapped to original order.
        /// </summary>
        private readonly ArrayList m_pieSlicesMapping = new ArrayList();

        /// <summary>
        ///   Array of values to be presented by the chart.
        /// </summary>
        private decimal[] m_values;

        /// <summary>
        ///   Array of colors used for individual pie slices.
        /// </summary>
        private readonly Color[] m_colors;

        /// <summary>
        ///   Array of texts displayed by slices.
        /// </summary>
        private readonly string[] m_texts;

        /// <summary>
        ///  Font used by the chart.
        /// </summary>
        private Font m_font;

        /// <summary>
        ///  ForeColor used by the chart.
        /// </summary>
        private Color m_foreColor;

        /// <summary>
        /// 
        /// </summary>
        private EdgeColorType m_edgeColorType;

        /// <summary>
        /// 
        /// </summary>
        private ShadowStyle m_shadowStyle;

        /// <summary>
        /// 
        /// </summary>
        private float m_edgeLineWidth;

        /// <summary>
        ///   Array of relative displacements from the common center.
        /// </summary>
        private float[] m_sliceRelativeDisplacements;

        /// <summary>
        ///   Index of the currently highlighted pie slice.
        /// </summary>
        private int m_highlightedIndex = -1;

        /// <summary>
        ///    
        /// </summary>
        private bool m_fitToBoundingRectangle;

        /// <summary>
        ///   The width of the bounding rectangle.
        /// </summary>
        private float m_width;

        /// <summary>
        ///   The height of the bounding rectangle.
        /// </summary>
        private float m_height;

        /// <summary>
        ///   The y-coordinate of the bounding rectangle top edge.
        /// </summary>
        private float m_top;

        /// <summary>
        ///  The x-coordinate of the bounding rectangle left edge.
        /// </summary>
        private float m_left;

        /// <summary>
        ///   Flag indicating if object has been disposed.
        /// </summary>
        private bool m_disposed;

        /// <summary>
        ///   Initializes an empty instance of <c>PieChart3D</c>.
        /// </summary>
        private PieChart3D()
        {
            m_fitToBoundingRectangle = true;
            m_shadowStyle = ShadowStyle.NoShadow;
            m_colors = new[]
            {
                Color.Red,
                Color.Green,
                Color.Blue,
                Color.Yellow,
                Color.Purple,
                Color.Olive,
                Color.Navy,
                Color.Aqua,
                Color.Lime,
                Color.Maroon,
                Color.Teal,
                Color.Fuchsia
            };
            m_font = System.Windows.Forms.Control.DefaultFont;
            m_foreColor = SystemColors.WindowText;
            m_edgeColorType = EdgeColorType.SystemColor;
            m_edgeLineWidth = 1F;
        }

        /// <summary>
        ///   Initializes an instance of a flat <c>PieChart3D</c> with 
        ///   specified bounds, values to chart and relative thickness.
        /// </summary>
        /// <param name="boundingRectX">
        ///   x-coordinate of the upper-left corner of the rectangle that 
        ///   bounds the chart.
        /// </param>
        /// <param name="boundingRectY">
        ///   y-coordinate of the upper-left corner of the rectangle that 
        ///   bounds the chart.
        /// </param>
        /// <param name="boundingRectWidth">
        ///   Width of the rectangle that bounds the chart.
        /// </param>
        /// <param name="boundingRectHeight">
        ///   Height of the rectangle that bounds the chart.
        /// </param>
        /// <param name="values">
        ///   An array of <c>decimal</c> values to chart.
        /// </param>
        public PieChart3D(float boundingRectX, float boundingRectY, float boundingRectWidth, float boundingRectHeight,
            decimal[] values)
            : this()
        {
            m_left = boundingRectX;
            m_top = boundingRectY;
            m_width = boundingRectWidth;
            m_height = boundingRectHeight;
            Values(values);
        }

        /// <summary>
        ///   Initializes an instance of <c>PieChart3D</c> with specified 
        ///   bounds, values to chart and relative thickness.
        /// </summary>
        /// <param name="boundingRectX">
        ///   x-coordinate of the upper-left corner of the rectangle bounding 
        ///   the chart.
        /// </param>
        /// <param name="boundingRectY">
        ///   y-coordinate of the upper-left corner of the rectangle bounding
        ///   the chart.
        /// </param>
        /// <param name="width">
        ///   Width of the rectangle bounding the chart.
        /// </param>
        /// <param name="height">
        ///   Height of the rectangle bounding the chart.
        /// </param>
        /// <param name="values">
        ///   An array of <c>decimal</c> values to chart.
        /// </param>
        /// <param name="sliceRelativeHeight">
        ///   Thickness of the pie slice to chart relative to the height of the
        ///   bounding rectangle.
        /// </param>
        public PieChart3D(float boundingRectX, float boundingRectY, float width, float height,
            decimal[] values, float sliceRelativeHeight)
            : this(boundingRectX, boundingRectY, width, height, values)
        {
            m_sliceRelativeHeight = sliceRelativeHeight;
        }

        /// <summary>
        ///   Initializes a new instance of <c>PieChart3D</c> with given bounds, 
        ///   array of values and pie slice thickness.
        /// </summary>
        /// <param name="boundingRectangle">
        ///   Bounding rectangle.
        /// </param>
        /// <param name="values">
        ///   Array of values to initialize with.
        /// </param>
        /// <param name="sliceRelativeHeight"></param>
        public PieChart3D(RectangleF boundingRectangle, decimal[] values, float sliceRelativeHeight)
            : this(
                boundingRectangle.X, boundingRectangle.Y, boundingRectangle.Width, boundingRectangle.Height, values,
                sliceRelativeHeight)
        {
        }

        /// <summary>
        ///   Initializes a new instance of <c>PieChart3D</c> with given bounds,
        ///   array of values and relative pie slice height.
        /// </summary>
        /// <param name="boundingRectX">
        ///   x-coordinate of the upper-left corner of the rectangle bounding 
        ///   the chart.
        /// </param>
        /// <param name="boundingRectY">
        ///   y-coordinate of the upper-left corner of the rectangle bounding
        ///   the chart.
        /// </param>
        /// <param name="width">
        ///   Width of the rectangle bounding the chart.
        /// </param>
        /// <param name="height">
        ///   Height of the rectangle bounding the chart.
        /// </param>
        /// <param name="values">
        ///   An array of <c>decimal</c> values to chart.
        /// </param>
        /// <param name="sliceColors">
        ///   An array of colors used to render slices.
        /// </param>
        /// <param name="sliceRelativeHeight">
        ///   Thickness of the slice to chart relative to the height of the
        ///   bounding rectangle.
        /// </param>
        public PieChart3D(float boundingRectX, float boundingRectY, float width, float height,
            decimal[] values, Color[] sliceColors, float sliceRelativeHeight)
            : this(boundingRectX, boundingRectY, width, height, values, sliceRelativeHeight)
        {
            m_colors = sliceColors;
        }

        /// <summary>
        ///   Initializes a new instance of <c>PieChart3D</c> with given bounds,
        ///   array of values and corresponding colors.
        /// </summary>
        /// <param name="boundingRectangle">
        ///   Bounding rectangle.
        /// </param>
        /// <param name="values">
        ///   Array of values to chart.
        /// </param>
        /// <param name="sliceColors">
        ///   Colors used for rendering individual slices.
        /// </param>
        /// <param name="sliceRelativeHeight">
        ///   Pie slice relative height.
        /// </param>
        public PieChart3D(RectangleF boundingRectangle, decimal[] values, Color[] sliceColors, float sliceRelativeHeight)
            : this(boundingRectangle, values, sliceRelativeHeight)
        {
            m_colors = sliceColors;
        }

        /// <summary>
        ///   Initializes a new instance of <c>PieChart3D</c> with given bounds,
        ///   array of values and relative pie slice height.
        /// </summary>
        /// <param name="boundingRectX">
        ///   x-coordinate of the upper-left corner of the rectangle bounding 
        ///   the chart.
        /// </param>
        /// <param name="boundingRectY">
        ///   y-coordinate of the upper-left corner of the rectangle bounding
        ///   the chart.
        /// </param>
        /// <param name="width">
        ///   Width of the rectangle bounding the chart.
        /// </param>
        /// <param name="height">
        ///   Height of the rectangle bounding the chart.
        /// </param>
        /// <param name="values">
        ///   An array of <c>decimal</c> values to chart.
        /// </param>
        /// <param name="sliceColors">
        ///   An array of colors used to render slices.
        /// </param>
        /// <param name="sliceRelativeHeight">
        ///   Thickness of the slice to chart relative to the height of the
        ///   bounding rectangle.
        /// </param>
        /// <param name="texts">
        ///   An array of strings that are displayed on corresponding slice.
        /// </param>
        public PieChart3D(float boundingRectX, float boundingRectY, float width, float height,
            decimal[] values, Color[] sliceColors, float sliceRelativeHeight, string[] texts)
            : this(boundingRectX, boundingRectY, width, height, values, sliceColors, sliceRelativeHeight)
        {
            m_texts = texts;
        }

        /// <summary>
        ///   Initializes a new instance of <c>PieChart3D</c> with given bounds,
        ///   array of values and relative pie slice height.
        /// </summary>
        /// <param name="boundingRectX">
        ///   x-coordinate of the upper-left corner of the rectangle bounding 
        ///   the chart.
        /// </param>
        /// <param name="boundingRectY">
        ///   y-coordinate of the upper-left corner of the rectangle bounding
        ///   the chart.
        /// </param>
        /// <param name="width">
        ///   Width of the rectangle bounding the chart.
        /// </param>
        /// <param name="height">
        ///   Height of the rectangle bounding the chart.
        /// </param>
        /// <param name="values">
        ///   An array of <c>decimal</c> values to chart.
        /// </param>
        /// <param name="sliceRelativeHeight">
        ///   Thickness of the slice to chart relative to the height of the
        ///   bounding rectangle.
        /// </param>
        /// <param name="texts">
        ///   An array of strings that are displayed on corresponding slice.
        /// </param>
        public PieChart3D(float boundingRectX, float boundingRectY, float width, float height,
            decimal[] values, float sliceRelativeHeight, string[] texts)
            : this(boundingRectX, boundingRectY, width, height, values, sliceRelativeHeight)
        {
            m_texts = texts;
        }

        /// <summary>
        ///   <c>Finalize</c> method.
        /// </summary>
        ~PieChart3D()
        {
            Dispose(false);
        }

        /// <summary>
        ///   Implementation of <c>IDisposable</c> interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Disposes of all pie slices.
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            if (disposing)
            {
                foreach (PieSlice slice in m_pieSlices)
                {
                    slice.Dispose();
                }
            }
            m_disposed = true;
        }

        /// <summary>
        ///   Sets values to be displayed on the chart.
        /// </summary>
        private void Values(decimal[] chartValues)
        {
            Debug.Assert(chartValues != null && chartValues.Length > 0);
            m_values = chartValues;
        }

        /// <summary>
        ///   Sets the font of the text displayed by the control.
        /// </summary>
        public void Font(Font textFont)
        {
            m_font = textFont;
        }

        /// <summary>
        ///   Sets the foreground color of the control used to draw text.
        /// </summary>
        public void ForeColor(Color color)
        {
            m_foreColor = color;
        }

        /// <summary>
        ///   Sets slice edge color mode. If set to <c>PenColor</c> (default),
        ///   then value set by <c>EdgeColor</c> property is used.
        /// </summary>
        public void ColorTypeOfEdge(EdgeColorType edgeColorType)
        {
            m_edgeColorType = edgeColorType;
        }

        /// <summary>
        ///   Sets slice edge line width. If not set, default value is 1.
        /// </summary>
        public void EdgeLineWidth(float lineWidth)
        {
            m_edgeLineWidth = lineWidth;
        }

        /// <summary>
        ///   Sets slice height, relative to the top ellipse semi-axis. Must be
        ///   less than or equal to 0.5.
        /// </summary>
        public void SliceRelativeHeight(float relativeHeight)
        {
            Debug.Assert(relativeHeight <= 0.5F);
            m_sliceRelativeHeight = relativeHeight;
        }

        /// <summary>
        ///   Sets the slice displacement relative to the ellipse semi-axis.
        ///   Must be less than 1.
        /// </summary>
        public void SliceRelativeDisplacement(float relativeDisplacement)
        {
            Debug.Assert(IsDisplacementValid(relativeDisplacement));
            m_sliceRelativeDisplacements = new[] { relativeDisplacement };
        }

        /// <summary>
        ///   Sets the slice displacement relative to the ellipse semi-axis.
        ///   Must be less than 1.
        /// </summary>
        public void SliceRelativeDisplacements(float[] relativeDisplacements)
        {
            Debug.Assert(AreDisplacementsValid(relativeDisplacements));
            m_sliceRelativeDisplacements = relativeDisplacements;
        }

        /// <summary>
        ///   Gets or sets the size of the entire pie chart.
        /// </summary>
        public void ChartSize(SizeF size)
        {
            m_width = size.Width;
            m_height = size.Height;
        }

        /// <summary>
        ///   Gets the y-coordinate of the bounding rectangle bottom edge.
        /// </summary>
        public float Bottom => m_top + m_height;

        /// <summary>
        ///   Gets the x-coordinate of the bounding rectangle right edge.
        /// </summary>
        public float Right => m_left + m_width;

        /// <summary>
        ///   Gets or sets the x-coordinate of the upper-left corner of the 
        ///   bounding rectangle.
        /// </summary>
        public float PointX
        {
            get { return m_left; }
            set { m_left = value; }
        }

        /// <summary>
        ///   Gets or sets the y-coordinate of the upper-left corner of the bounding rectangle.
        /// </summary>
        public float PointY
        {
            get { return m_top; }
            set { m_top = value; }
        }

        /// <summary>
        ///   Sets the shadowing style used.
        /// </summary>
        public void StyleOfShadow(ShadowStyle shadowStyle)
        {
            m_shadowStyle = shadowStyle;
        }

        /// <summary>
        ///   Sets the flag that controls if chart is fit to bounding rectangle exactly.
        /// </summary>
        public void FitToBoundingRectangle(bool fitToBoundingRect)
        {
            m_fitToBoundingRectangle = fitToBoundingRect;
        }

        /// <summary>
        ///   Sets the initial angle from which pies are placed.
        /// </summary>
        public void InitialAngle(float angle)
        {
            m_initialAngle = angle % 360;
            if (m_initialAngle < 0)
                m_initialAngle += 360;
        }

        /// <summary>
        ///   Sets the index of the highlighted pie.
        /// </summary>
        public void HighlightedIndex(int index)
        {
            m_highlightedIndex = index;
        }

        /// <summary>
        ///   Draws the chart.
        /// </summary>
        /// <param name="graphics">
        ///   <c>Graphics</c> object used for drawing.
        /// </param>
        public void Draw(Graphics graphics)
        {
            Debug.Assert(m_values != null && m_values.Length > 0);
            InitializePieSlices();
            if (m_fitToBoundingRectangle)
            {
                RectangleF newBoundingRectangle = GetFittingRectangle();
                ReadjustSlices(newBoundingRectangle);
            }
            DrawBottoms(graphics);
            if (m_sliceRelativeHeight > 0F)
                DrawSliceSides(graphics);
            DrawTops(graphics);
        }

        /// <summary>
        /// Draws strings by individual slices. Position of the text is
        /// calculated by overridable <c>GetTextPosition</c> method of the
        /// <c>PieSlice</c> type.
        /// </summary>
        /// <param name="graphics"><c>Graphics</c> object.</param>
        /// <exception cref="System.ArgumentNullException">graphics</exception>
        public void PlaceTexts(Graphics graphics)
        {
            Debug.Assert(m_font != null);
            Debug.Assert(m_foreColor != Color.Empty);

            graphics.ThrowIfNull(nameof(graphics));

            using (StringFormat drawFormat = new StringFormat())
            {
                drawFormat.Alignment = StringAlignment.Center;
                drawFormat.LineAlignment = StringAlignment.Center;

                using (Brush fontBrush = new SolidBrush(m_foreColor))
                {
                    int num = 0;
                    PointF[] points = new PointF[m_pieSlices.Length];
                    foreach (PieSlice slice in m_pieSlices)
                    {
                        if (!string.IsNullOrEmpty(slice.Text))
                        {
                            PointF point = slice.GetTextPosition;

                            foreach (PointF oldpoint in points)
                            {
                                for (int x = 0; x <= 1; x++)
                                {
                                    float diffy = oldpoint.Y - point.Y;
                                    float diffx = oldpoint.X - point.X;

                                    if (diffy < 0)
                                        diffy *= -1;
                                    if (diffx < 0)
                                        diffx *= -1;

                                    if (diffx < 70 && diffy < 16)
                                        point = slice.GetTextPositionOut(x == 0 ? 4.5f : 2.2f);
                                }
                            }

                            points[num] = point;
                            graphics.DrawString(slice.Text, m_font, fontBrush, point, drawFormat);
                        }

                        num++;
                    }
                }
            }
        }

        /// <summary>
        ///   Searches the chart to find the index of the pie slice which 
        ///   contains point given. Search order goes in the direction opposite
        ///   to drawing order.
        /// </summary>
        /// <param name="point">
        ///   <c>PointF</c> point for which pie slice is searched for.
        /// </param>
        /// <returns>
        ///   Index of the corresponding pie slice, or -1 if none is found.
        /// </returns>
        public int FindPieSliceUnderPoint(PointF point)
        {
            // first check tops
            for (int i = 0; i < m_pieSlices.Length; ++i)
            {
                PieSlice slice = m_pieSlices[i];
                if (slice.PieSliceContainsPoint(point))
                    return (int)m_pieSlicesMapping[i];
            }
            // split the backmost (at 270 degrees) pie slice
            ArrayList pieSlicesList = new ArrayList(m_pieSlices);
            PieSlice[] splitSlices = m_pieSlices[0].Split(270F);
            if (splitSlices.Length > 1)
            {
                pieSlicesList[0] = splitSlices[1];
                if (splitSlices[0].SweepAngle > 0F)
                    pieSlicesList.Add(splitSlices[0]);
            }
            PieSlice[] pieSlices = (PieSlice[])pieSlicesList.ToArray(typeof(PieSlice));
            int indexFound = -1;
            // if not found yet, then check for periferies
            int incrementIndex = 0;
            int decrementIndex = pieSlices.Length - 1;
            while (incrementIndex <= decrementIndex)
            {
                PieSlice sliceLeft = pieSlices[decrementIndex];
                float angle1 = 270 - sliceLeft.StartAngle;
                PieSlice sliceRight = pieSlices[incrementIndex];
                float angle2 = (sliceRight.EndAngle + 90) % 360;
                Debug.Assert(angle2 >= 0);
                if (angle2 < angle1)
                {
                    if (sliceRight.PeripheryContainsPoint(point))
                        indexFound = incrementIndex;
                    ++incrementIndex;
                }
                else
                {
                    if (sliceLeft.PeripheryContainsPoint(point))
                        indexFound = decrementIndex;
                    --decrementIndex;
                }
            }
            // check for start/stop sides, starting from the foremost
            if (indexFound < 0)
            {
                int foremostPieIndex = GetForemostPieSlice(pieSlices);
                if (foremostPieIndex == -1)
                    Debug.Assert(false, "Foremost pie slice not found");

                // check for start sides from the foremost slice to the left 
                // side
                int i = foremostPieIndex;
                while (i < pieSlices.Length)
                {
                    PieSlice sliceLeft = pieSlices[i];
                    if (sliceLeft.StartSideContainsPoint(point))
                    {
                        indexFound = i;
                        break;
                    }
                    ++i;
                }
                // if not found yet, check end sides from the foremost to the right
                // side
                if (indexFound < 0)
                {
                    i = foremostPieIndex;
                    while (i >= 0)
                    {
                        PieSlice sliceLeft = pieSlices[i];
                        if (sliceLeft.EndSideContainsPoint(point))
                        {
                            indexFound = i;
                            break;
                        }
                        --i;
                    }
                }
            }
            // finally search for bottom sides
            if (indexFound < 0)
            {
                for (int i = 0; i < m_pieSlices.Length; ++i)
                {
                    PieSlice slice = m_pieSlices[i];
                    if (slice.BottomSurfaceSectionContainsPoint(point))
                        return (int)m_pieSlicesMapping[i];
                }
            }
            if (indexFound <= -1)
                return -1;

            indexFound %= m_pieSlicesMapping.Count;
            return (int)m_pieSlicesMapping[indexFound];
        }

        /// <summary>
        ///   Return the index of the foremost pie slice i.e. the one crossing
        ///   90 degrees boundary.
        /// </summary>
        /// <param name="pieSlices">
        ///   Array of <c>PieSlice</c> objects to examine.
        /// </param>
        /// <returns>
        ///   Index of the foremost pie slice.
        /// </returns>
        private static int GetForemostPieSlice(IList<PieSlice> pieSlices)
        {
            if (pieSlices == null || pieSlices.Count <= 0)
                return -1;

            for (int i = 0; i < pieSlices.Count; ++i)
            {
                PieSlice pieSlice = pieSlices[i];
                if (((pieSlice.StartAngle <= 90) && (pieSlice.StartAngle + pieSlice.SweepAngle >= 90)) ||
                    ((pieSlice.StartAngle + pieSlice.SweepAngle > 360) && (pieSlice.StartAngle <= 450) &&
                     pieSlice.StartAngle + pieSlice.SweepAngle >= 450))
                    return i;
            }

            return -1;
        }

        /// <summary>
        ///   Finds the smallest rectangle int which chart fits entirely.
        /// </summary>
        /// <returns>
        ///   <c>RectangleF</c> into which all member slices fit.
        /// </returns>
        private RectangleF GetFittingRectangle()
        {
            RectangleF boundingRectangle = m_pieSlices[0].GetFittingRectangle();
            for (int i = 1; i < m_pieSlices.Length; ++i)
            {
                boundingRectangle = RectangleF.Union(boundingRectangle, m_pieSlices[i].GetFittingRectangle());
            }
            return boundingRectangle;
        }

        /// <summary>
        ///   Readjusts each slice for new bounding rectangle. 
        /// </summary>
        /// <param name="newBoundingRectangle">
        ///   <c>RectangleF</c> representing new boundary.
        /// </param>
        private void ReadjustSlices(RectangleF newBoundingRectangle)
        {
            float xResizeFactor = m_width / newBoundingRectangle.Width;
            float yResizeFactor = m_height / newBoundingRectangle.Height;
            float xOffset = newBoundingRectangle.X - m_left;
            float yOffset = newBoundingRectangle.Y - m_top;
            foreach (PieSlice slice in m_pieSlices)
            {
                float x = slice.BoundingRectangle.X - xOffset;
                float y = slice.BoundingRectangle.Y - yOffset;
                float width = slice.BoundingRectangle.Width * xResizeFactor;
                float height = slice.BoundingRectangle.Height * yResizeFactor;
                float sliceHeight = slice.SliceHeight * yResizeFactor;
                slice.Readjust(x, y, width, height, sliceHeight);
            }
        }

        /// <summary>
        ///   Finds the largest displacement.
        /// </summary>
        private float LargestDisplacement
        {
            get
            {
                float value = 0F;
                for (int i = 0; i < m_sliceRelativeDisplacements.Length && i < m_values.Length; ++i)
                {
                    if (m_sliceRelativeDisplacements[i] > value)
                        value = m_sliceRelativeDisplacements[i];
                }
                return value;
            }
        }

        /// <summary>
        ///   Gets the top ellipse size.
        /// </summary>
        private SizeF TopEllipseSize
        {
            get
            {
                float factor = 1 + LargestDisplacement;
                float widthTopEllipse = m_width / factor;
                float heightTopEllipse = m_height / factor * (1 - m_sliceRelativeHeight);
                return new SizeF(widthTopEllipse, heightTopEllipse);
            }
        }

        /// <summary>
        ///   Gets the ellipse defined by largest displacement.
        /// </summary>
        private SizeF LargestDisplacementEllipseSize
        {
            get
            {
                float factor = LargestDisplacement;
                float widthDisplacementEllipse = TopEllipseSize.Width * factor;
                float heightDisplacementEllipse = TopEllipseSize.Height * factor;
                return new SizeF(widthDisplacementEllipse, heightDisplacementEllipse);
            }
        }

        /// <summary>
        ///   Calculates the pie height.
        /// </summary>
        private float PieHeight => m_height / (1 + LargestDisplacement) * m_sliceRelativeHeight;

        /// <summary>
        ///   Initializes pies.
        /// </summary>
        /// Creates a list of pies, starting with the pie that is crossing the 
        /// 270 degrees boundary, i.e. "backmost" pie that always has to be 
        /// drawn first to ensure correct surface overlapping.
        private void InitializePieSlices()
        {
            // calculates the sum of values required to evaluate sweep angles 
            // for individual pies
            double sum = m_values.Sum(itemValue => (double)itemValue);

            // some values and indices that will be used in the loop
            SizeF topEllipeSize = TopEllipseSize;
            SizeF largestDisplacementEllipseSize = LargestDisplacementEllipseSize;
            int maxDisplacementIndex = m_sliceRelativeDisplacements.Length - 1;
            float largestDisplacement = LargestDisplacement;
            ArrayList listPieSlices = new ArrayList();
            m_pieSlicesMapping.Clear();
            int colorIndex = 0;
            int backPieIndex = -1;
            int displacementIndex = 0;
            double startAngle = m_initialAngle;
            for (int i = 0; i < m_values.Length; ++i)
            {
                decimal itemValue = m_values[i];
                double sweepAngle = (double)itemValue / sum * 360;
                // displacement from the center of the ellipse
                float xDisplacement = m_sliceRelativeDisplacements[displacementIndex];
                float yDisplacement = m_sliceRelativeDisplacements[displacementIndex];
                if (xDisplacement > 0F)
                {
                    Debug.Assert(largestDisplacement > 0F);
                    SizeF pieDisplacement = GetSliceDisplacement((float)(startAngle + sweepAngle / 2),
                        m_sliceRelativeDisplacements[displacementIndex]);
                    xDisplacement = pieDisplacement.Width;
                    yDisplacement = pieDisplacement.Height;
                }
                PieSlice slice;
                if (i == m_highlightedIndex)
                {
                    using (PieSlice pieSlice =
                        CreatePieSliceHighlighted(m_left + largestDisplacementEllipseSize.Width / 2 + xDisplacement,
                            m_top + largestDisplacementEllipseSize.Height / 2 + yDisplacement,
                            topEllipeSize.Width, topEllipeSize.Height, PieHeight,
                            (float)startAngle, (float)sweepAngle, m_colors[colorIndex],
                            m_shadowStyle, m_edgeColorType, m_edgeLineWidth))
                    {
                        slice = (PieSlice)pieSlice.Clone();
                    }
                }
                else
                {
                    using (PieSlice pieSlice = CreatePieSlice(m_left + largestDisplacementEllipseSize.Width / 2 + xDisplacement,
                        m_top + largestDisplacementEllipseSize.Height / 2 + yDisplacement,
                        topEllipeSize.Width, topEllipeSize.Height, PieHeight,
                        (float)startAngle, (float)sweepAngle, m_colors[colorIndex],
                        m_shadowStyle, m_edgeColorType, m_edgeLineWidth))
                    {
                        slice = (PieSlice)pieSlice.Clone();
                    }
                }
                slice.Text = m_texts[i];
                // the backmost pie is inserted to the front of the list for correct drawing
                if (backPieIndex > -1 || ((startAngle <= 270) && (startAngle + sweepAngle > 270)) ||
                    ((startAngle >= 270) && (startAngle + sweepAngle > 630)))
                {
                    ++backPieIndex;
                    listPieSlices.Insert(backPieIndex, slice);
                    m_pieSlicesMapping.Insert(backPieIndex, i);
                }
                else
                {
                    listPieSlices.Add(slice);
                    m_pieSlicesMapping.Add(i);
                }
                // increment displacementIndex only if there are more displacements available
                if (displacementIndex < maxDisplacementIndex)
                    ++displacementIndex;
                ++colorIndex;
                // if all colors have been exhausted, reset color index
                if (colorIndex >= m_colors.Length)
                    colorIndex = 0;
                // prepare for the next pie slice
                startAngle += sweepAngle;
                if (startAngle > 360)
                    startAngle -= 360;
            }
            m_pieSlices = (PieSlice[])listPieSlices.ToArray(typeof(PieSlice));
        }

        /// <summary>
        ///   Creates a <c>PieSlice</c> object.
        /// </summary>
        /// <param name="boundingRectLeft">
        ///   x-coordinate of the upper-left corner of the rectangle that is 
        ///   used to draw the top surface of the slice.
        /// </param>
        /// <param name="boundingRectTop">
        ///   y-coordinate of the upper-left corner of the rectangle that is 
        ///   used to draw the top surface of the slice.
        /// </param>
        /// <param name="boundingRectWidth">
        ///   Width of the rectangle that is used to draw the top surface of 
        ///   the slice.
        /// </param>
        /// <param name="boundingRectHeight">
        ///   Height of the rectangle that is used to draw the top surface of 
        ///   the slice.
        /// </param>
        /// <param name="sliceHeight">
        ///   Slice height.
        /// </param>
        /// <param name="startAngle">
        ///   Starting angle.
        /// </param>
        /// <param name="sweepAngle">
        ///   Sweep angle.
        /// </param>
        /// <param name="color">
        ///   Color used for slice rendering.
        /// </param>
        /// <param name="shadowStyle">
        ///   Shadow style used for slice rendering.
        /// </param>
        /// <param name="edgeColorType">
        ///   Edge lines color type.
        /// </param>
        /// <param name="edgeLineWidth">
        ///   Edge lines width.
        /// </param>
        /// <returns>
        ///   <c>PieSlice</c> object with given values.
        /// </returns>
        private static PieSlice CreatePieSlice(float boundingRectLeft, float boundingRectTop, float boundingRectWidth,
            float boundingRectHeight, float sliceHeight, float startAngle, float sweepAngle,
            Color color, ShadowStyle shadowStyle, EdgeColorType edgeColorType,
            float edgeLineWidth)
            => new PieSlice(boundingRectLeft, boundingRectTop, boundingRectWidth, boundingRectHeight, sliceHeight,
                startAngle, sweepAngle, color, shadowStyle, edgeColorType, edgeLineWidth);

        /// <summary>
        ///   Creates highlighted <c>PieSlice</c> object.
        /// </summary>
        /// <param name="boundingRectLeft">
        ///   x-coordinate of the upper-left corner of the rectangle that is 
        ///   used to draw the top surface of the slice.
        /// </param>
        /// <param name="boundingRectTop">
        ///   y-coordinate of the upper-left corner of the rectangle that is 
        ///   used to draw the top surface of the slice.
        /// </param>
        /// <param name="boundingRectWidth">
        ///   Width of the rectangle that is used to draw the top surface of 
        ///   the slice.
        /// </param>
        /// <param name="boundingRectHeight">
        ///   Height of the rectangle that is used to draw the top surface of 
        ///   the slice.
        /// </param>
        /// <param name="sliceHeight">
        ///   Slice height.
        /// </param>
        /// <param name="startAngle">
        ///   Starting angle.
        /// </param>
        /// <param name="sweepAngle">
        ///   Sweep angle.
        /// </param>
        /// <param name="color">
        ///   Color used for slice rendering.
        /// </param>
        /// <param name="shadowStyle">
        ///   Shadow style used for slice rendering.
        /// </param>
        /// <param name="edgeColorType">
        ///   Edge lines color type.
        /// </param>
        /// <param name="edgeLineWidth">
        ///   Edge lines width.
        /// </param>
        /// <returns>
        ///   <c>PieSlice</c> object with given values.
        /// </returns>
        private static PieSlice CreatePieSliceHighlighted(float boundingRectLeft, float boundingRectTop,
            float boundingRectWidth, float boundingRectHeight, float sliceHeight,
            float startAngle, float sweepAngle, Color color,
            ShadowStyle shadowStyle, EdgeColorType edgeColorType,
            float edgeLineWidth)
        {
            Color highLightedColor = ColorUtil.CreateColorWithCorrectedLightness(color, ColorUtil.BrightnessEnhancementFactor1);

            return new PieSlice(boundingRectLeft, boundingRectTop, boundingRectWidth, boundingRectHeight, sliceHeight,
                startAngle, sweepAngle, highLightedColor, shadowStyle, edgeColorType,
                edgeLineWidth);
        }

        /// <summary>
        ///   Calculates the displacement for given angle.
        /// </summary>
        /// <param name="angle">
        ///   Angle (in degrees).
        /// </param>
        /// <param name="displacementFactor">
        ///   Displacement factor.
        /// </param>
        /// <returns>
        ///   <c>SizeF</c> representing displacement.
        /// </returns>
        private SizeF GetSliceDisplacement(float angle, float displacementFactor)
        {
            Debug.Assert(displacementFactor > 0F && displacementFactor <= 1F);
            if (Math.Abs(displacementFactor) < float.Epsilon)
                return SizeF.Empty;
            float xDisplacement = (float)(TopEllipseSize.Width * displacementFactor / 2 * Math.Cos(angle * Math.PI / 180));
            float yDisplacement = (float)(TopEllipseSize.Height * displacementFactor / 2 * Math.Sin(angle * Math.PI / 180));
            return new SizeF(xDisplacement, yDisplacement);
        }

        /// <summary>
        ///   Draws outer peripheries of all slices.
        /// </summary>
        /// <param name="graphics">
        ///   <c>Graphics</c> used for drawing.
        /// </param>
        private void DrawSliceSides(Graphics graphics)
        {
            ArrayList pieSlicesList = new ArrayList(m_pieSlices);
            PieSlice ps;
            // if the first pie slice (crossing 270 i.e. back) is crossing 90 
            // (front) axis too, we have to split it
            if ((m_pieSlices[0].StartAngle > 90) && (m_pieSlices[0].StartAngle <= 270) &&
                (m_pieSlices[0].StartAngle + m_pieSlices[0].SweepAngle > 450))
            {
                ps = (PieSlice)pieSlicesList[0];
                // this one is split at 0 deg to avoid line of split to be
                // visible on the periphery
                PieSlice[] splitSlices = ps.Split(0F);
                pieSlicesList[0] = splitSlices[0];
                if (splitSlices[1].SweepAngle > 0F)
                    pieSlicesList.Insert(1, splitSlices[1]);
            }
            else if (((m_pieSlices[0].StartAngle > 270) && (m_pieSlices[0].StartAngle + m_pieSlices[0].SweepAngle > 450)) ||
                     ((m_pieSlices[0].StartAngle < 90) && (m_pieSlices[0].StartAngle + m_pieSlices[0].SweepAngle > 270)))
            {
                ps = (PieSlice)pieSlicesList[0];
                // this one is split at 180 deg to avoid line of split to be
                // visible on the periphery
                PieSlice[] splitSlices = ps.Split(180F);
                pieSlicesList[0] = splitSlices[1];
                if (splitSlices[1].SweepAngle > 0F)
                    pieSlicesList.Add(splitSlices[0]);
            }
            // first draw the backmost pie slice
            ps = (PieSlice)pieSlicesList[0];
            ps.DrawSides(graphics);
            // draw pie slices from the backmost to forward
            int incrementIndex = 1;
            int decrementIndex = pieSlicesList.Count - 1;
            while (incrementIndex < decrementIndex)
            {
                PieSlice sliceLeft = (PieSlice)pieSlicesList[decrementIndex];
                float angle1 = sliceLeft.StartAngle - 90;
                if (angle1 > 180 || angle1 < 0)
                    angle1 = 0;
                PieSlice sliceRight = (PieSlice)pieSlicesList[incrementIndex];
                float angle2 = (450 - sliceRight.EndAngle) % 360;
                if (angle2 > 180 || angle2 < 0)
                    angle2 = 0;
                Debug.Assert(angle1 >= 0);
                Debug.Assert(angle2 >= 0);
                if (angle2 >= angle1)
                {
                    sliceRight.DrawSides(graphics);
                    ++incrementIndex;
                }
                else if (angle2 < angle1)
                {
                    sliceLeft.DrawSides(graphics);
                    --decrementIndex;
                }
            }
            ps = (PieSlice)pieSlicesList[decrementIndex];
            ps.DrawSides(graphics);
        }

        /// <summary>
        ///   Draws bottom sides of all pie slices.
        /// </summary>
        /// <param name="graphics">
        ///   <c>Graphics</c> used for drawing.
        /// </param>
        private void DrawBottoms(Graphics graphics)
        {
            foreach (PieSlice slice in m_pieSlices)
            {
                slice.DrawBottom(graphics);
            }
        }

        /// <summary>
        ///   Draws top sides of all pie slices.
        /// </summary>
        /// <param name="graphics">
        ///   <c>Graphics</c> used for drawing.
        /// </param>
        private void DrawTops(Graphics graphics)
        {
            foreach (PieSlice slice in m_pieSlices)
            {
                slice.DrawTop(graphics);
            }
        }

        /// <summary>
        ///   Helper function used in assertions. Checks the validity of 
        ///   slice displacements.
        /// </summary>
        /// <param name="displacements">
        ///   Array of displacements to check.
        /// </param>
        /// <returns>
        ///   <c>true</c> if all displacements have a valid value; otherwise 
        ///   <c>false</c>.
        /// </returns>
        private static bool AreDisplacementsValid(IEnumerable<float> displacements)
            => displacements.All(IsDisplacementValid);

        /// <summary>
        ///   Helper function used in assertions. Checks the validity of 
        ///   a slice displacement.
        /// </summary>
        /// <param name="value">
        ///   Displacement value to check.
        /// </param>
        /// <returns>
        ///   <c>true</c> if displacement has a valid value; otherwise 
        ///   <c>false</c>.
        /// </returns>
        private static bool IsDisplacementValid(float value) => value >= 0F && value <= 1F;
    }
}