using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Timers;
using EVEMon.Common;
using EVEMon.Common.Collections;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Factories;
using EVEMon.Common.Models;
using LogitechLcd.NET;

namespace EVEMon.LogitechG15
{
    internal sealed class LcdDisplay : IDisposable
    {
        internal const int G15Width = LogitechLcdConstants.LogiLCDMonoWidth;
        private const int G15Height = LogitechLcdConstants.LogiLCDMonoHeight;
        private const float G15DpiX = 46;
        private const float G15DpiY = 46;

        private static LcdDisplay s_singleInstance;

        private readonly Font m_defaultFont;
        private readonly Bitmap m_bmpLCD;
        private readonly Bitmap m_bmpLCDX;
        private readonly Graphics m_lcdCanvas;
        private readonly Graphics m_lcdOverlay;
        private readonly List<LcdLine> m_lcdLines = new List<LcdLine>();
        private readonly Object m_lock = new Object();
        private readonly Timer m_buttonPressedCheckTimer;
        private readonly float m_defaultOffset;

        private CCPCharacter m_currentCharacter;
        private CCPCharacter m_refreshCharacter;
        private int m_oldButtonState;
        private DateTime m_buttonStateHld;
        private DateTime m_paintTime;
        private DateTime m_holdTime;
        private LcdState m_state;
        private DateTime m_cycleTime;
        private DateTime m_cycleQueueInfoTime;
        private bool m_showingCycledQueueInfo;
        private int m_completedSkills;
        private bool m_disposed;


        #region Events

        /// <summary>
        /// Fired whenever a button has been pressed which require EVEMon to requery the API for the specified character.
        /// </summary>
        public static event EventHandler<CharacterChangedEventArgs> ApiUpdateRequested;

        /// <summary>
        /// Fired whenever the current character changed (because of a button press).
        /// </summary>
        public static event EventHandler<CharacterChangedEventArgs> CurrentCharacterChanged;

        /// <summary>
        /// Fired whenever the auto cycle should change (because of a button press).
        /// </summary>
        public static event EventHandler<CycleEventArgs> AutoCycleChanged;

        #endregion


        #region Instantiation

        /// <summary>
        /// Initializes a new instance of the <see cref="LcdDisplay"/> class.
        /// </summary>
        private LcdDisplay()
        {
            m_defaultFont = FontFactory.GetFont("Microsoft Sans Serif", 13.5f);

            m_bmpLCD = new Bitmap(G15Width, G15Height, PixelFormat.Format24bppRgb);
            m_bmpLCD.SetResolution(G15DpiX, G15DpiY);
            m_lcdCanvas = Graphics.FromImage(m_bmpLCD);
            m_lcdCanvas.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            m_bmpLCDX = new Bitmap(G15Width, G15Height, PixelFormat.Format24bppRgb);
            m_bmpLCDX.SetResolution(G15DpiX, G15DpiY);
            m_lcdOverlay = Graphics.FromImage(m_bmpLCDX);
            m_lcdOverlay.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            m_buttonStateHld = DateTime.Now;
            m_cycleTime = DateTime.Now.AddSeconds(10);
            m_cycleQueueInfoTime = DateTime.Now.AddSeconds(5);
            m_showingCycledQueueInfo = false;

            m_defaultOffset = Environment.Is64BitProcess ? -2f : 0f;

            Cycle = false;
            ShowSystemTime = false;
            CycleSkillQueueTime = false;

            m_buttonPressedCheckTimer = new Timer { Interval = 100 };
            m_buttonPressedCheckTimer.Elapsed += ButtonPressedCheckTimerOnElapsed;
            m_buttonPressedCheckTimer.Start();

            LcdInterface.Open("EVEMon");
        }

        /// <summary>
        /// Instances this instance.
        /// </summary>
        /// <returns></returns>
        internal static LcdDisplay Instance() => s_singleInstance ?? (s_singleInstance = new LcdDisplay());

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether to cycle the displayed info.
        /// </summary>
        /// <value><c>true</c> if set to cycle; otherwise, <c>false</c>.</value>
        internal bool Cycle { private get; set; }

        /// <summary>
        /// Gets or sets the cycle interval.
        /// </summary>
        /// <value>The cycle interval.</value>
        internal int CycleInterval { private get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the system's time.
        /// </summary>
        /// <value><c>true</c> if set to show the system's time; otherwise, <c>false</c>.</value>
        internal bool ShowSystemTime { private get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the EVE's time.
        /// </summary>
        /// <value><c>true</c> if set to show the EVE's time; otherwise, <c>false</c>.</value>
        internal bool ShowEVETime { private get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to cycle the skill queue time.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if set to cycle skill queue time; otherwise, <c>false</c>.
        /// </value>
        internal bool CycleSkillQueueTime { private get; set; }

        /// <summary>
        /// Gets or sets the cycle completion interval.
        /// </summary>
        /// <value>The cycle completion interval.</value>
        internal int CycleCompletionInterval { private get; set; }

        /// <summary>
        /// Gets or sets the first character to complete skill.
        /// </summary>
        /// <value>The first character to complete skill.</value>
        internal CCPCharacter FirstCharacterToCompleteSkill { private get; set; }

        /// <summary>
        /// Gets or sets the current character.
        /// </summary>
        /// <value>The current character.</value>
        internal CCPCharacter CurrentCharacter
        {
            private get { return MonitoredCharacters.Contains(m_currentCharacter) ? m_currentCharacter : null; }
            set { m_currentCharacter = value; }
        }

        /// <summary>
        /// Gets the monitored characters.
        /// </summary>
        /// <value>The characters.</value>
        private static IEnumerable<CCPCharacter> MonitoredCharacters => EveMonClient.MonitoredCharacters.OfType<CCPCharacter>();

        #endregion


        #region Cleanup

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="LcdDisplay"/> is reclaimed by garbage collection.
        /// </summary>
        ~LcdDisplay()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="isDisposing">
        /// <c>true</c> to release both managed and unmanaged resources;
        /// <c>false</c> to release only unmanaged resources.
        /// </param>
        private void Dispose(bool isDisposing)
        {
            if (!m_disposed)
            {
                if (isDisposing || s_singleInstance != null)
                    LcdInterface.Close();

                m_buttonPressedCheckTimer.Stop();
                m_buttonPressedCheckTimer.Dispose();
                m_bmpLCD.Dispose();
                m_bmpLCDX.Dispose();
                s_singleInstance = null;
            }
            m_disposed = true;
        }

        #endregion


        #region Painting

        /// <summary>
        /// Performs the repainting of the screen.
        /// </summary>
        internal void Paint()
        {
            TimeSpan test = TimeSpan.FromTicks(m_paintTime.Ticks - DateTime.Now.Ticks);
            if (test.TotalMilliseconds > 0)
                return;

            TimeSpan now = TimeSpan.FromTicks(DateTime.Now.Ticks - m_holdTime.Ticks);
            if (m_state == LcdState.SplashScreen && now.TotalSeconds > 4)
                SwitchState(LcdState.Character);

            if (m_state == LcdState.CharacterList && now.TotalMilliseconds > 2000)
                SwitchState(LcdState.Character);

            if (m_state == LcdState.SkillComplete && now.TotalSeconds > 14)
                SwitchState(LcdState.Character);

            if ((m_state == LcdState.CycleSettings || m_state == LcdState.Refreshing) && now.TotalSeconds > 2)
                SwitchState(LcdState.Character);

            switch (m_state)
            {
                case LcdState.SplashScreen:
                    m_paintTime = m_paintTime.AddSeconds(2);
                    PaintSplash();
                    return;
                case LcdState.Character:
                    if (Cycle)
                    {
                        if (TimeSpan.FromTicks(DateTime.Now.Ticks - m_cycleTime.Ticks).TotalSeconds > CycleInterval)
                        {
                            m_cycleTime = DateTime.Now;
                            MoveToNextChar();

                            // When moving to next character
                            // we reset the queue info timer
                            m_cycleQueueInfoTime = DateTime.Now;
                            m_showingCycledQueueInfo = false;
                        }
                    }
                    m_paintTime = m_paintTime.AddSeconds(1);
                    PaintsCharacter();
                    return;
                case LcdState.CharacterList:
                    m_paintTime = m_paintTime.AddSeconds(2);
                    PaintCharactersList();
                    return;
                case LcdState.SkillComplete:
                    m_paintTime = m_paintTime.AddMilliseconds(800);
                    PaintSkillCompletionMessage();
                    return;
                case LcdState.CycleSettings:
                    m_paintTime = m_paintTime.AddSeconds(2);
                    PaintCycleSettings();
                    return;
                case LcdState.Refreshing:
                    if (m_refreshCharacter != null)
                    {
                        m_paintTime = m_paintTime.AddSeconds(2);
                        PaintRefreshingMessage();
                    }
                    return;
                default:
                    return;
            }
        }

        /// <summary>
        /// Paints the current character's training informations, this is the regular painting operation.
        /// </summary>
        private void PaintsCharacter()
        {
            m_lcdLines.Clear();

            if (!MonitoredCharacters.Any())
            {
                m_lcdLines.Add(new LcdLine("No CCP Characters To Display", m_defaultFont));
                RenderLines();
                UpdateLcdDisplay();
                return;
            }

            if (CycleSkillQueueTime)
            {
                if (TimeSpan.FromTicks(DateTime.Now.Ticks - m_cycleQueueInfoTime.Ticks).TotalSeconds > CycleCompletionInterval)
                {
                    m_cycleQueueInfoTime = DateTime.Now;
                    m_showingCycledQueueInfo = !m_showingCycledQueueInfo;
                }
            }

            if (CurrentCharacter == null)
                return;

            m_lcdLines.Add(new LcdLine(CurrentCharacter.AdornedName, m_defaultFont));

            QueuedSkill queuedSkill = CurrentCharacter.SkillQueue.CurrentlyTraining;
            if (CurrentCharacter.SkillQueue.IsTraining)
            {
                if (m_showingCycledQueueInfo)
                {
                    if (CurrentCharacter.SkillQueue.LessThanWarningThreshold)
                    {
                        // Place holder for skill queue training time rendering
                        m_lcdLines.Add(new LcdLine(" ", m_defaultFont));
                    }
                    else if (CurrentCharacter.SkillQueue.Count > 1)
                    {
                        // If more then one skill is in queue, show queue finish time
                        string endTimeText = CurrentCharacter.SkillQueue.EndTime
                            .Subtract(DateTime.UtcNow).ToDescriptiveText(DescriptiveTextOptions.SpaceBetween);
                        m_lcdLines.Add(new LcdLine($"Queue ends in {endTimeText}", m_defaultFont));
                    }
                    else
                    {
                        // Show the skill in training
                        m_lcdLines.Add(new LcdLine($"{queuedSkill}", m_defaultFont));
                    }
                }
                else
                {
                    // Show the skill in training
                    m_lcdLines.Add(new LcdLine($"{queuedSkill}", m_defaultFont));
                }

                m_lcdLines.Add(new LcdLine(queuedSkill.EndTime.Subtract(DateTime.UtcNow).ToDescriptiveText(
                    DescriptiveTextOptions.SpaceBetween), m_defaultFont));
            }
            else
            {
                if (CurrentCharacter.SkillQueue.IsPaused)
                {
                    m_lcdLines.Add(new LcdLine($"{queuedSkill}", m_defaultFont));
                    m_lcdLines.Add(new LcdLine("Skill Training Is Paused", m_defaultFont));
                }
                else
                {
                    m_lcdLines.Add(new LcdLine("No Skill In Training", m_defaultFont));
                    m_lcdLines.Add(new LcdLine("Skill Queue Is Empty", m_defaultFont));
                }
            }

            m_lcdLines.Add(new LcdLine($"{queuedSkill?.FractionCompleted ?? 0}", m_defaultFont));

            RenderLines();
            RenderWalletBalance();
            RenderSkillQueueInfo();
            RenderCompletionTime();

            UpdateLcdDisplay();
        }

        /// <summary>
        /// Renders the lines.
        /// </summary>
        private void RenderLines()
        {
            ClearGraphics();

            float offset = m_defaultOffset;

            foreach (LcdLine lcdLine in m_lcdLines)
            {
                lcdLine.Render(m_lcdCanvas, m_lcdOverlay, offset, m_defaultOffset);
                offset += lcdLine.Height;
            }
        }

        /// <summary>
        /// Renders the wallet balance.
        /// </summary>
        private void RenderWalletBalance()
        {
            decimal balance = CurrentCharacter.Balance;
            string walletBalance = $"{balance:N2} ISK";
            SizeF balanceSize = m_lcdCanvas.MeasureString(walletBalance, m_defaultFont);
            SizeF charNameSize = m_lcdCanvas.MeasureString(CurrentCharacter.AdornedName, m_defaultFont);
            float availableWidth = G15Width - charNameSize.Width;

            if (availableWidth < balanceSize.Width)
            {
                walletBalance = AbbreviationFormat(balance, availableWidth);
                balanceSize = m_lcdCanvas.MeasureString(walletBalance, m_defaultFont);
            }

            RectangleF line = new RectangleF(new PointF(G15Width - balanceSize.Width, 0f + m_defaultOffset), balanceSize);
            using (Brush brush = new SolidBrush(Color.Black))
            {
                m_lcdCanvas.DrawString(walletBalance, m_defaultFont, brush, line);
            }
        }

        /// <summary>
        /// Renders the skill queue info.
        /// </summary>
        private void RenderSkillQueueInfo()
        {
            if (CurrentCharacter.IsTraining &&
                CurrentCharacter.SkillQueue.LessThanWarningThreshold &&
                m_showingCycledQueueInfo)
            {
                UpdateSkillQueueTrainingTime();
            }
        }

        /// <summary>
        /// Renders the completion time.
        /// </summary>
        private void RenderCompletionTime()
        {
            if (!CurrentCharacter.IsTraining)
                return;

            DateTime completionDateTime = CurrentCharacter.CurrentlyTrainingSkill.EndTime.ToLocalTime();
            string completionDateTimeText = $"{completionDateTime.ToShortDateString()}  {completionDateTime.ToShortTimeString()}";
            SizeF completionDateTimeSize = m_lcdCanvas.MeasureString(completionDateTimeText, m_defaultFont);
            RectangleF timeLine = new RectangleF(new PointF(G15Width - completionDateTimeSize.Width, 22f + m_defaultOffset),
                completionDateTimeSize);
            using (Brush brush = new SolidBrush(Color.Black))
            {
                m_lcdCanvas.DrawString(completionDateTimeText, m_defaultFont, brush, timeLine);
            }
        }

        /// <summary>
        /// Paints a message for skill completion.
        /// </summary>
        private void PaintSkillCompletionMessage()
        {
            m_lcdLines.Clear();

            if (!MonitoredCharacters.Any())
            {
                m_lcdLines.Add(new LcdLine("No CCP Characters To Display", m_defaultFont));
                RenderLines();
                UpdateLcdDisplay();
                return;
            }

            if (CurrentCharacter == null || CurrentCharacter.SkillQueue.LastCompleted == null)
                return;

            m_lcdLines.Add(new LcdLine(CurrentCharacter.AdornedName, m_defaultFont));
            m_lcdLines.Add(new LcdLine("has finished training", m_defaultFont));

            m_lcdLines.Add(m_completedSkills > 1
                ? new LcdLine($"{m_completedSkills} skills",
                    m_defaultFont)
                : new LcdLine(CurrentCharacter.SkillQueue.LastCompleted.ToString(), m_defaultFont));

            int skillCount = CurrentCharacter.SkillQueue.Count;

            m_lcdLines.Add(skillCount == 0
                ? new LcdLine("NO SKILLS IN QUEUE", m_defaultFont)
                : new LcdLine($"{skillCount} more skill{(skillCount == 1 ? String.Empty : "s")} in queue", m_defaultFont));

            RenderLines();
            UpdateLcdDisplay();
        }

        /// <summary>
        /// Paints the characters list.
        /// </summary>
        private void PaintCharactersList()
        {
            m_lcdLines.Clear();

            if (!MonitoredCharacters.Any())
            {
                m_lcdLines.Add(new LcdLine("No CCP Characters To Display", m_defaultFont));
                RenderLines();
                UpdateLcdDisplay();
                return;
            }

            // Creates a reordered list with the selected character on top
            List<CCPCharacter> charList = new List<CCPCharacter>();

            int currentCharacterIndex = Math.Max(0, MonitoredCharacters.IndexOf(CurrentCharacter));
            for (int i = currentCharacterIndex; i < MonitoredCharacters.Count(); i++)
            {
                charList.Add(MonitoredCharacters.ElementAt(i));
            }

            for (int i = 0; i < currentCharacterIndex; i++)
            {
                charList.Add(MonitoredCharacters.ElementAt(i));
            }

            // Perform the painting
            ClearGraphics();

            foreach (CCPCharacter character in charList)
            {
                m_lcdLines.Add(new LcdLine(character.AdornedName, m_defaultFont));
            }

            RenderLines();
            RenderSelector();
            UpdateLcdDisplay();
        }

        /// <summary>
        /// Renders the selector.
        /// </summary>
        private void RenderSelector()
        {
            using (Brush brush = new SolidBrush(Color.Black))
            {
                m_lcdOverlay.FillRectangle(brush, 0, 0, G15Width, 11);
            }
        }

        /// <summary>
        /// Paints the cycling settings on the screen.
        /// </summary>
        private void PaintCycleSettings()
        {
            ClearGraphics();
            m_lcdLines.Clear();

            string status = Cycle ? "on" : "off";
            string statusMsg = $"Autocycle is now {status}";
            m_lcdLines.Add(new LcdLine(statusMsg, m_defaultFont));

            string cycleMsg = $"Cycle Time is: {CycleInterval}s";
            m_lcdLines.Add(new LcdLine(cycleMsg, m_defaultFont));

            RenderLines();
            UpdateLcdDisplay();
        }

        /// <summary>
        /// Paints a waiting message while we're updating the characters.
        /// </summary>
        private void PaintRefreshingMessage()
        {
            ClearGraphics();
            m_lcdLines.Clear();

            m_lcdLines.Add(new LcdLine("Refreshing Character Information", m_defaultFont));
            m_lcdLines.Add(new LcdLine("of", m_defaultFont));
            m_lcdLines.Add(new LcdLine(m_refreshCharacter.AdornedName, m_defaultFont));

            RenderLines();
            UpdateLcdDisplay();
        }

        /// <summary>
        /// Paints the EVEMon icon at the initialization of the LCD screen.
        /// </summary>
        private void PaintSplash()
        {
            // Clear the graphics
            ClearGraphics();

            // Load the icon
            using (Bitmap splashLogo = new Bitmap(Properties.Resources.LCDSplash))
            {
                // Display the splash logo
                int left = G15Width / 2 - splashLogo.Width / 2;
                int top = G15Height / 2 - splashLogo.Height / 2;
                m_lcdCanvas.DrawImage(splashLogo, new Rectangle(left, top, splashLogo.Width, splashLogo.Height));
                UpdateLcdDisplay();
            }
        }

        /// <summary>
        /// Clears the graphics.
        /// </summary>
        private void ClearGraphics()
        {
            m_lcdCanvas.Clear(Color.White);
            m_lcdOverlay.Clear(Color.White);
        }

        /// <summary>
        /// Updates the skill queue training time info.
        /// </summary>
        private void UpdateSkillQueueTrainingTime()
        {
            TimeSpan skillQueueEndTime = CurrentCharacter.SkillQueue.EndTime.Subtract(DateTime.UtcNow);
            TimeSpan timeLeft = SkillQueue.WarningThresholdTimeSpan.Subtract(skillQueueEndTime);

            // Prevents the "(none)" text from being displayed
            if (timeLeft < TimeSpan.FromSeconds(1))
                return;

            // Less than minute ? Display seconds
            string endTimeText = skillQueueEndTime < TimeSpan.FromMinutes(1)
                ? skillQueueEndTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas)
                : skillQueueEndTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas, includeSeconds: false);

            string skillQueueEndTimeText = $"Queue ends in {endTimeText}";
            SizeF size = m_lcdCanvas.MeasureString(skillQueueEndTimeText, m_defaultFont);
            RectangleF line = new RectangleF(new PointF(0f, 11f + m_defaultOffset), size);
            using (Brush brush = new SolidBrush(Color.Black))
            {
                m_lcdCanvas.FillRectangle(brush, line.Left, line.Top + (Environment.Is64BitProcess ? 2 : 0),
                    G15Width, size.Height - 1);
                m_lcdOverlay.DrawString(skillQueueEndTimeText, m_defaultFont, brush, line);
            }
        }

        /// <summary>
        /// Fetches the content of the <see cref="Graphics"/> object to the G15 screen.
        /// </summary>
        private unsafe void UpdateLcdDisplay()
        {
            // Locking should not be necessary but i'll keep it here
            lock (m_lock)
            {
                byte[] buffer = new byte[m_bmpLCD.Width * m_bmpLCD.Height];
                Rectangle rect = new Rectangle(0, 0, m_bmpLCD.Width, m_bmpLCD.Height);

                BitmapData bmData = m_bmpLCD.LockBits(rect, ImageLockMode.ReadOnly, m_bmpLCD.PixelFormat);
                try
                {
                    BitmapData bmDataX = m_bmpLCDX.LockBits(rect, ImageLockMode.ReadOnly, m_bmpLCDX.PixelFormat);
                    try
                    {
                        // Extract bits per pixel and length infos
                        int bpp = bmData.Stride / m_bmpLCD.Width;

                        // Copy the content of the bitmap to our buffers 
                        // Unsafe code removes the boundaries checks - a lot faster.
                        fixed (byte* buffer0 = buffer)
                        {
                            byte* output = buffer0;
                            byte* inputX = (byte*)bmDataX.Scan0.ToPointer();
                            byte* input = (byte*)bmData.Scan0.ToPointer();

                            for (int i = 0; i < m_bmpLCD.Height; i++)
                            {
                                for (int j = 0; j < m_bmpLCD.Width; j++)
                                {
                                    *output = (byte)(*input ^ *inputX);
                                    inputX += bpp;
                                    input += bpp;
                                    output++;
                                }
                            }
                        }
                    }
                    finally
                    {
                        m_bmpLCDX.UnlockBits(bmDataX);
                    }
                }
                finally
                {
                    m_bmpLCD.UnlockBits(bmData);
                }

                // Fetches the buffer to the LCD screen
                LcdInterface.DisplayBitmap(buffer);
            }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Formats the wallet balance value in an abbreviation form.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="width"></param>
        /// <returns>Abbreviated balance value</returns>
        private string AbbreviationFormat(decimal value, float width)
        {
            string balance;
            int suffixIndex = 0;
            float newWidth;

				if (width <= 0.0f) {
					 return String.Empty;
				}

				do
            {
                value /= 1000M;
                suffixIndex++;

                switch (suffixIndex)
                {
                    case 1:
                        balance = $"{value:N2} K ISK";
                        break;
                    case 2:
                        balance = $"{value:N2} M ISK";
                        break;
                    case 3:
                        balance = $"{value:N2} B ISK";
                        break;
                    // We have no room to show the wallet balance
                    default:
                        balance = String.Empty;
                        break;
                }

                SizeF size = m_lcdCanvas.MeasureString(balance, m_defaultFont);
                newWidth = size.Width;
            } while (newWidth > width);

            return balance;
        }

        #endregion


        #region Controlling logic

        /// <summary>
        /// Occurs when some of the G15 screen buttons are pressed.
        /// </summary>
        /// <returns></returns>
        private void ButtonPressedCheckTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var pressedButtons = 0;

            if (LcdInterface.ReadSoftButton((int)LogitechLcdConstants.LogiLcdMonoButton.Button0))
                pressedButtons |= (int)LogitechLcdConstants.LogiLcdMonoButton.Button0;

            if (LcdInterface.ReadSoftButton((int)LogitechLcdConstants.LogiLcdMonoButton.Button1))
                pressedButtons |= (int)LogitechLcdConstants.LogiLcdMonoButton.Button1;

            if (LcdInterface.ReadSoftButton((int)LogitechLcdConstants.LogiLcdMonoButton.Button2))
                pressedButtons |= (int)LogitechLcdConstants.LogiLcdMonoButton.Button2;

            if (LcdInterface.ReadSoftButton((int)LogitechLcdConstants.LogiLcdMonoButton.Button3))
                pressedButtons |= (int)LogitechLcdConstants.LogiLcdMonoButton.Button3;

            if (m_oldButtonState == pressedButtons)
                return;

            // Gets all buttons who haven't been pressed last time
            int press = (m_oldButtonState ^ pressedButtons) & pressedButtons;

            // Displays the characters' list or move to the next char if the list is already displayed.
            if ((press & (int)LogitechLcdConstants.LogiLcdMonoButton.Button0) != 0)
                DisplayCharactersList();

            // Move to the first character to complete his training
            if ((press & (int)LogitechLcdConstants.LogiLcdMonoButton.Button1) != 0)
            {
                // Select next skill ready char
                if (!MonitoredCharacters.Any())
                    return;

                CurrentCharacter = FirstCharacterToCompleteSkill;

                AutoCycleChanged?.ThreadSafeInvoke(this, new CycleEventArgs(false));

                SwitchState(LcdState.Character);
            }

            // Forces a refresh from CCP
            if ((press & (int)LogitechLcdConstants.LogiLcdMonoButton.Button2) != 0)
            {
                if (m_state == LcdState.Character || m_state == LcdState.CharacterList)
                {
                    m_refreshCharacter = CurrentCharacter;
                    ApiUpdateRequested?.ThreadSafeInvoke(this, new CharacterChangedEventArgs(m_refreshCharacter));

                    SwitchState(LcdState.Refreshing);
                }
            }

            // Switch autocycle ON/OFF
            if ((press & (int)LogitechLcdConstants.LogiLcdMonoButton.Button3) != 0)
            {
                // Switch autocycle on/off
                SwitchCycle();

                AutoCycleChanged?.ThreadSafeInvoke(this, new CycleEventArgs(Cycle));

                SwitchState(LcdState.CycleSettings);
                m_cycleTime = DateTime.Now;
            }

            m_oldButtonState = pressedButtons;
        }

        /// <summary>
        /// Moves the selection to the next character.
        /// </summary>
        private void MoveToNextChar()
        {
            if (!MonitoredCharacters.Any())
                return;

            // Move to next char
            int index = MonitoredCharacters.IndexOf(CurrentCharacter);
            if (++index >= MonitoredCharacters.Count())
                index = 0;

            CurrentCharacter = MonitoredCharacters.ElementAt(index);

            // Requests new data
            CurrentCharacterChanged?.ThreadSafeInvoke(this, new CharacterChangedEventArgs(CurrentCharacter));
        }

        /// <summary>
        /// Switches the state and updates some of the internal times variables.
        /// </summary>
        /// <param name="state"></param>
        internal void SwitchState(LcdState state)
        {
            m_state = state;
            m_paintTime = DateTime.Now;
            m_holdTime = DateTime.Now;
        }

        /// <summary>
        /// Updates the characters' list.
        /// First call displays the list, the second one moves the selection
        /// </summary>
        private void DisplayCharactersList()
        {
            TimeSpan now = TimeSpan.FromTicks(DateTime.Now.Ticks - m_buttonStateHld.Ticks);
            m_buttonStateHld = DateTime.Now;

            if (now.TotalMilliseconds < 2000)
            {
                Cycle = false;
                MoveToNextChar();
            }

            SwitchState(LcdState.CharacterList);
        }

        /// <summary>
        /// On skill completion, switch to the display of the proper message.
        /// </summary>
        internal void SkillCompleted(Character character, int skillCount)
        {
            CurrentCharacter = character as CCPCharacter;
            m_completedSkills = skillCount;
            SwitchState(LcdState.SkillComplete);
        }

        /// <summary>
        /// Switches the cycling setting.
        /// </summary>
        private void SwitchCycle()
        {
            Cycle = !Cycle;
        }

        #endregion
    }
}