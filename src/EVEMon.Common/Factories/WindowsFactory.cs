using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using EVEMon.Common.Extensions;
using EVEMon.Common.Helpers;

namespace EVEMon.Common.Factories
{
    /// <summary>
    /// This factory allows us keep unique instances of 
    /// </summary>
    public static class WindowsFactory
    {
        private static readonly object s_syncLock = new object();
        private static readonly List<Form> s_taggedWindows = new List<Form>();
        private static readonly List<Form> s_uniqueWindows = new List<Form>();

        /// <summary>
        /// Gets the window displayed as unique if it exists, null otherwise.
        /// </summary>
        /// <returns></returns>
        public static TForm GetUnique<TForm>()
            where TForm : Form
        {
            lock (s_syncLock)
            {
                return s_uniqueWindows.OfType<TForm>()
                    .FirstOrDefault(uniqueWindow => uniqueWindow != null && !uniqueWindow.IsDisposed);
            }
        }

        /// <summary>
        /// Show the unique window. 
        /// When none exist, it is created using the default constructor.
        /// When it already exists, it is bringed to front, or show when hidden.
        /// </summary>
        /// <returns></returns>
        public static TForm ShowUnique<TForm>()
            where TForm : Form
            => ShowUnique(Activator.CreateInstance<TForm>);

        /// <summary>
        /// Show the unique window.
        /// When none exist, it is created using the provided callback.
        /// When it already exists, it is bringed to front, or show when hidden.
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <param name="creation"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">creation</exception>
        private static TForm ShowUnique<TForm>(Func<TForm> creation)
            where TForm : Form
        {
            creation.ThrowIfNull(nameof(creation));

            lock (s_syncLock)
            {
                try
                {
                    // Does it already exist ?
                    foreach (TForm existingUniqueWindow in s_uniqueWindows.OfType<TForm>()
                        .Where(existingUniqueWindow => existingUniqueWindow != null && !existingUniqueWindow.IsDisposed))
                    {
                        // Bring to front or show
                        if (existingUniqueWindow.Visible)
                            existingUniqueWindow.BringToFront();
                        else
                            existingUniqueWindow.Show();

                        // Give focus and return
                        existingUniqueWindow.Activate();
                        return existingUniqueWindow;
                    }
                }
                    // Catch exception when the window is being disposed
                catch (ObjectDisposedException ex)
                {
                    ExceptionHandler.LogException(ex, true);
                }

                // Create the window and subscribe to its closing for cleanup
                TForm uniqueWindow = creation.Invoke();
                uniqueWindow.Disposed += (sender, args) =>
                {
                    lock (s_syncLock)
                    {
                        s_uniqueWindows.Remove((TForm)sender);
                    }
                };
                s_uniqueWindows.Add(uniqueWindow);

                // Show and return
                uniqueWindow.Show();
                return uniqueWindow;
            }
        }

        /// <summary>
        /// Changes the tag of a stored window.
        /// Usually used to switch the tag of a plan window.
        /// </summary>
        /// <typeparam name="TForm">The type of the form.</typeparam>
        /// <typeparam name="TTag1">The type of the tag1.</typeparam>
        /// <typeparam name="TTag2">The type of the tag2.</typeparam>
        /// <param name="oldTag">The old tag.</param>
        /// <param name="newTag">The new tag.</param>
        public static void ChangeTag<TForm, TTag1, TTag2>(TTag1 oldTag, TTag2 newTag)
            where TForm : Form
            where TTag1 : class
            where TTag2 : class
        {
            lock (s_syncLock)
            {
                TForm taggedWindow = GetByTag<TForm, TTag1>(oldTag);
                taggedWindow.Tag = newTag;
            }
        }

        /// <summary>
        /// Gets the existing form associated with the given tag.
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <typeparam name="TTag"></typeparam>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static TForm GetByTag<TForm, TTag>(TTag tag)
            where TForm : Form
            where TTag : class
        {
            object otag = tag;

            lock (s_syncLock)
            {
                return s_taggedWindows.OfType<TForm>()
                    .FirstOrDefault(existingWindow => existingWindow.Tag == otag && !existingWindow.IsDisposed);
            }
        }

        /// <summary>
        /// Show the window with the given owner and tag.
        /// When none exist, it is created using the public constructor accepting an argument of type <see cref="TTag" />,
        /// or the default constructor if the previous one does not exist.
        /// When it already exists, it is brought to front, or shown when hidden.
        /// </summary>
        /// <typeparam name="TForm">The type of the form.</typeparam>
        /// <typeparam name="TTag">The type of the tag.</typeparam>
        /// <param name="owner">The owner.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static TForm ShowByTag<TForm, TTag>(IWin32Window owner, TTag tag, params object[] args)
            where TForm : Form
            where TTag : class
            => ShowByTag(owner, tag, Create<TForm>, args);

        /// <summary>
        /// Show the window with the given tag.
        /// When none exist, it is created using the public constructor accepting an argument of type <see cref="TTag"/>,
        /// or the default constructor if the previous one does not exist.
        /// When it already exists, it is brought to front, or shown when hidden.
        /// </summary>
        /// <typeparam name="TForm">The type of the form.</typeparam>
        /// <typeparam name="TTag">The type of the tag.</typeparam>
        /// <param name="tag">The tag.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static TForm ShowByTag<TForm, TTag>(TTag tag, params object[] args)
            where TForm : Form
            where TTag : class
            => ShowByTag(null, tag, Create<TForm>, args);

        /// <summary>
        /// Show the window with the given tag.
        /// When none exist, it is created using the provided callback, and the provided tag is then associated with it.
        /// When it already exists, it is brought to front, or shown when hidden.
        /// </summary>
        /// <typeparam name="TForm">The type of the form.</typeparam>
        /// <typeparam name="TTag">The type of the tag.</typeparam>
        /// <param name="owner">The owner.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="creation">The creation.</param>
        /// <param name="pars">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// tag
        /// or
        /// creation
        /// </exception>
        private static TForm ShowByTag<TForm, TTag>(IWin32Window owner, TTag tag, Func<object[], TForm> creation, IEnumerable<object> pars)
            where TForm : Form
            where TTag : class
        {
            tag.ThrowIfNull(nameof(tag));

            creation.ThrowIfNull(nameof(creation));

            object otag = tag;

            lock (s_syncLock)
            {
                // Does it already exist ?
                foreach (TForm existingWindow in s_taggedWindows.OfType<TForm>()
                    .Where(existingWindow => existingWindow.Tag == otag && !existingWindow.IsDisposed))
                {
                    try
                    {
                        // Bring to front or show
                        if (existingWindow.Visible)
                            existingWindow.BringToFront();
                        else
                        {
                            if (owner != null)
                                existingWindow.Show(owner);
                            else
                                existingWindow.Show();
                        }

                        // Give focus and return
                        existingWindow.Activate();
                        return existingWindow;
                    }
                        // Catch exception when the window was disposed
                    catch (ObjectDisposedException ex)
                    {
                        ExceptionHandler.LogException(ex, true);
                    }
                }

                // Combine the tag parameter with the rest
                // Always put the tag parameter first
                object[] parameters = new[] { tag }.Concat(pars).ToArray();

                // Create the window and attach the tag
                TForm window = creation.Invoke(parameters);
                window.Tag = otag;

                // Store it and subscribe to closing for clean up
                s_taggedWindows.Add(window);
                window.Disposed += (sender, args) =>
                {
                    lock (s_syncLock)
                    {
                        s_taggedWindows.Remove((TForm)sender);
                    }
                };

                // Show and return
                if (owner != null)
                    window.Show(owner);
                else
                    window.Show();
                return window;
            }
        }

        /// <summary>
        /// Call the public constructor with the provided arguments.
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        private static TForm Create<TForm>(object[] args)
            where TForm : Form
        {
            // Search for a public instance constructor with the specified arguments
            // If no constructor found, use the default constructor
            ConstructorInfo ctor = typeof(TForm).GetConstructor(args.Select(arg => arg.GetType()).ToArray());
            return (TForm)ctor?.Invoke(args) ?? Activator.CreateInstance<TForm>();
        }

        /// <summary>
        /// Close the window with the given tag.
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <typeparam name="TTag"></typeparam>
        /// <param name="form"></param>
        /// <param name="tag"></param>
        private static void CloseByTag<TForm, TTag>(TForm form, TTag tag)
            where TForm : Form
            where TTag : class
        {
            object otag = tag;

            lock (s_syncLock)
            {
                // While we find windows to close...
                while (true)
                {
                    // Search all the disposed windows or windows with the same tag
                    bool isDisposed = false;
                    TForm formToRemove = null;
                    foreach (TForm existingWindow in s_taggedWindows
                        .Where(taggedWindow => taggedWindow == form).Cast<TForm>())
                    {
                        try
                        {
                            if (existingWindow.Tag != otag)
                                continue;

                            formToRemove = existingWindow;
                            break;
                        }
                            // Catch exception when the window was disposed - we will remove it also by the way
                        catch (ObjectDisposedException ex)
                        {
                            ExceptionHandler.LogException(ex, true);
                            formToRemove = existingWindow;
                            isDisposed = true;
                        }
                    }

                    // Returns if nothing found on this cycle
                    if (formToRemove == null)
                        return;

                    if (isDisposed)
                        s_taggedWindows.Remove(formToRemove);
                    else
                        formToRemove.Close();
                }
            }
        }

        /// <summary>
        /// Gets and closes the window with the given tag.
        /// </summary>
        /// <typeparam name="TForm">The type of the form.</typeparam>
        /// <typeparam name="TTag">The type of the tag.</typeparam>
        /// <param name="tag">The tag.</param>
        public static void GetAndCloseByTag<TForm, TTag>(TTag tag)
            where TForm : Form
            where TTag : class
        {
            TForm window = GetByTag<TForm, TTag>(tag);

            if (window != null)
                CloseByTag(window, tag);
        }

        /// <summary>
        /// Closes all tagged windows.
        /// </summary>
        public static void CloseAllTagged()
        {
            lock (s_syncLock)
            {
                List<Form> formsToClose = s_taggedWindows.ToList();
                foreach (Form existingWindow in formsToClose.Where(form => !form.IsDisposed))
                {
                    existingWindow.Close();
                }
                s_taggedWindows.Clear();
            }
        }
    }
}