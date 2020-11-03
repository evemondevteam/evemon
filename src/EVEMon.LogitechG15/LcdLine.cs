using System;
using System.Drawing;
using EVEMon.Common;
using EVEMon.Common.Factories;

namespace EVEMon.LogitechG15
{
    internal sealed class LcdLine
    {
        private readonly string m_text;
        private readonly Font m_font;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LcdLine"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        internal LcdLine(string text, Font font)
        {
            m_text = text;
            m_font = font;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        internal float Height { get; private set; }

        /// <summary>
        /// Renders the specified canvas.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="overlay">The overlay.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="defaultOffset">The default offset.</param>
        internal void Render(Graphics canvas, Graphics overlay, float offset, float defaultOffset)
        {
            double percentage;
            // Values are serialized using current culture so should be parsed using current
            // culture
            if (double.TryParse(m_text, out percentage))
                RenderProgressLine(canvas, overlay, offset, defaultOffset, percentage);
            else
                RenderTextLine(canvas, offset);
        }

        /// <summary>
        /// Renders the text line.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="offset">The offset.</param>
        private void RenderTextLine(Graphics canvas, float offset)
        {
            RectangleF lineRect = new RectangleF(new PointF(0f, offset), canvas.MeasureString(m_text, m_font));
            using (Brush brush = new SolidBrush(Color.Black))
            {
                canvas.DrawString(m_text, m_font, brush, lineRect);
            }

            Height = lineRect.Height;
        }

        /// <summary>
        /// Renders the progress line.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="overlay">The overlay.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="defaultOffset">The default offset.</param>
        /// <param name="percentage">The percentage.</param>
        private void RenderProgressLine(Graphics canvas, Graphics overlay, float offset, float defaultOffset, double percentage)
        {
            int left = 0;
            int width = LcdDisplay.G15Width - 1;
            const int Pad = 2;

            if (Settings.G15.ShowEVETime)
            {
                string eveTime = EveMonClient.EVEServer.ServerDateTime.ToShortTimeString();
                SizeF eveTimeSize = canvas.MeasureString(eveTime, m_font);
                RectangleF eveTimeRect = new RectangleF(new PointF(left, offset), eveTimeSize);
                left = (int)eveTimeSize.Width + Pad;
                width -= left;
                using (Brush brush = new SolidBrush(Color.Black))
                {
                    canvas.DrawString(eveTime, m_font, brush, eveTimeRect);
                }
            }

            if (Settings.G15.ShowSystemTime)
            {
                string systemTime = DateTime.Now.ToShortTimeString();
                SizeF systemTimeSize = canvas.MeasureString(systemTime, m_font);
                RectangleF systemTimeRect = new RectangleF(new PointF(LcdDisplay.G15Width + 1 - systemTimeSize.Width, offset),
                    systemTimeSize);
                width -= (int)systemTimeSize.Width + Pad;
                using (Brush brush = new SolidBrush(Color.Black))
                {
                    canvas.DrawString(systemTime, m_font, brush, systemTimeRect);
                }
            }

            string text = $"{percentage:P2}";
            SizeF textSize = canvas.MeasureString(text, m_font);
            RectangleF barRect = new RectangleF(left, offset - defaultOffset - (Environment.Is64BitProcess ? 0 : 1),
                width, textSize.Height - 1);
            float textLeft = (barRect.Width - textSize.Width) / 2;
            RectangleF textRect = new RectangleF(new PointF(left + textLeft, offset), textSize);

            int barFill = Convert.ToInt16(percentage * width - Pad);

            using (Pen pen = new Pen(Color.Black))
            {
                canvas.DrawRectangle(pen, barRect.Left, barRect.Top, barRect.Width, barRect.Height);
            }

            using (Brush brush = new SolidBrush(Color.Black))
            {
                var progressFont = FontFactory.GetFont(m_font.FontFamily, m_font.Size);
                canvas.FillRectangle(brush, barRect.Left + 1, barRect.Top + 1, barFill, barRect.Height - Pad);
                overlay.DrawString(text, progressFont, brush, textRect);
            }

            Height = barRect.Height + 1;
        }

        #endregion
    }
}
