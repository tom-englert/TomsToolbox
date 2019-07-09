namespace TomsToolbox.Wpf.Interactivity
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interactivity;
    using System.Windows.Interop;
    using System.Windows.Media;

    using JetBrains.Annotations;

    /// <summary>
    /// Hit test values for the <see cref="NcHitTestEventArgs"/>
    /// </summary>
    public enum HitTest
    {
        /// <summary>See documentation of WM_NCHITTEST</summary>
        Nowhere = 0,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        Client = 1,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        Caption = 2,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        SysMenu = 3,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        GrowBox = 4,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        Size = GrowBox,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        Menu = 5,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        HScroll = 6,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        VScroll = 7,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        MinButton = 8,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        MaxButton = 9,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        Left = 10,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        Right = 11,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        Top = 12,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        TopLeft = 13,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        TopRight = 14,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        Bottom = 15,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        BottomLeft = 16,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        BottomRight = 17,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        Border = 18,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        Object = 19,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        Close = 20,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        Help = 21,
        /// <summary>See documentation of WM_NCHITTEST</summary>
        Error = (-2),
        /// <summary>See documentation of WM_NCHITTEST</summary>
        Transparent = (-1),
    }

    /// <summary>
    /// Event args for the <see cref="CustomNonClientAreaBehavior.NcHitTestEvent"/> event.
    /// </summary>
    public class NcHitTestEventArgs : RoutedEventArgs
    {
        private HitTest _hitTest;

        /// <summary>
        /// Initializes a new instance of the <see cref="NcHitTestEventArgs"/> class.
        /// </summary>
        public NcHitTestEventArgs()
        {
            RoutedEvent = CustomNonClientAreaBehavior.NcHitTestEvent;
        }

        /// <summary>
        /// Gets or sets the hit test result.
        /// </summary>
        public HitTest HitTest
        {
            get => _hitTest;
            set
            {
                _hitTest = value;
                Handled = true;
            }
        }
    }

    /// <summary>
    /// Behavior to emulate correct non client area handling for transparent windows that draw their own border and caption.
    /// </summary>
    public class CustomNonClientAreaBehavior : Behavior<FrameworkElement>
    {
        [CanBeNull]
        private Window _window;
        private POINT _windowOffset;
        private Matrix _transformFromDevice = Matrix.Identity;
        private Matrix _transformToDevice = Matrix.Identity;

        /// <summary>
        /// Gets or sets the size of the border used to size the window.
        /// </summary>
        /// <value>
        /// The size of the border.
        /// </value>
        public Size BorderSize
        {
            get => this.GetValue<Size>(BorderSizeProperty);
            set => SetValue(BorderSizeProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="BorderSize"/> dependency property
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty BorderSizeProperty =
            DependencyProperty.Register("BorderSize", typeof(Size), typeof(CustomNonClientAreaBehavior), new FrameworkPropertyMetadata(new Size(4, 4)));


        /// <summary>
        /// Gets or sets the size of a corner used to size the window.
        /// </summary>
        /// <value>
        /// The size of the corner.
        /// </value>
        public Size CornerSize
        {
            get => this.GetValue<Size>(CornerSizeProperty);
            set => SetValue(CornerSizeProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="CornerSize"/> dependency property
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty CornerSizeProperty =
            DependencyProperty.Register("CornerSize", typeof(Size), typeof(CustomNonClientAreaBehavior), new FrameworkPropertyMetadata(new Size(8, 8)));


        /// <summary>
        /// Gets or sets a value indicating whether this window has a glass frame.
        /// </summary>
        /// <value>
        /// <c>true</c> if this window has a glass frame; otherwise, <c>false</c>.
        /// </value>
        public bool HasGlassFrame
        {
            get => this.GetValue<bool>(HasGlassFrameProperty);
            set => SetValue(HasGlassFrameProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="HasGlassFrame"/> dependency property
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty HasGlassFrameProperty =
            DependencyProperty.Register("HasGlassFrame", typeof(bool), typeof(CustomNonClientAreaBehavior), new FrameworkPropertyMetadata(true));


        /// <summary>
        /// The WM_NCHITTEST test event equivalent.
        /// </summary>
        [NotNull] public static readonly RoutedEvent NcHitTestEvent = EventManager.RegisterRoutedEvent("NcHitTest", RoutingStrategy.Bubble, typeof(EventHandler<NcHitTestEventArgs>), typeof(CustomNonClientAreaBehavior));

        /// <summary>
        /// Called when the element is attached.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            var frameworkElement = AssociatedObject;

            if (DesignerProperties.GetIsInDesignMode(frameworkElement))
                return;

            var window = _window = frameworkElement.TryFindAncestorOrSelf<Window>();
            if (window == null)
                return;

            window.SourceInitialized += Window_SourceInitialized;
        }

        /// <summary>
        /// Called when the element is detached.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            var window = _window;

            if (window == null)
                return;

            window.SourceInitialized -= Window_SourceInitialized;
            Unregister(window);
        }

        private void Window_SourceInitialized([CanBeNull] object sender, [CanBeNull] EventArgs e)
        {
            var window = _window;

            Debug.Assert(window != null, nameof(window) + " != null");
            var messageSource = (HwndSource)PresentationSource.FromDependencyObject(window);

            if (messageSource == null)
                throw new InvalidOperationException("Window needs to be initialized");

            var compositionTarget = messageSource.CompositionTarget;

            _transformFromDevice = compositionTarget?.TransformFromDevice ?? throw new InvalidOperationException("Window needs to be initialized");
            _transformToDevice = compositionTarget.TransformToDevice;

            messageSource.AddHook(WindowProc);

            var handle = messageSource.Handle;

            using (new DeferSizeToContent(window))
            {
                ShowGlassFrame(compositionTarget, handle);
                NativeMethods.SetWindowRgn(handle, IntPtr.Zero, true);
            }
        }

        private void ShowGlassFrame(IntPtr handle)
        {
            var window = _window;

            Debug.Assert(window != null, nameof(window) + " != null");
            var messageSource = (HwndSource)PresentationSource.FromDependencyObject(window);
            if (messageSource == null)
                throw new InvalidOperationException("Window needs to be initialized");

            ShowGlassFrame(messageSource.CompositionTarget, handle);
        }

        private void ShowGlassFrame([CanBeNull] HwndTarget compositionTarget, IntPtr handle)
        {
            if (compositionTarget == null)
                return;
            
            if (HasGlassFrame)
            {
                try
                {
                    if (NativeMethods.DwmIsCompositionEnabled())
                    {
                        compositionTarget.BackgroundColor = Colors.Transparent;

                        var m = new MARGINS(-1);
                        NativeMethods.DwmExtendFrameIntoClientArea(handle, ref m);
                        return;
                    }
                }
                catch (Exception)
                {
                    // dwmapi.dll not found => incompatible OS
                }
            }

            compositionTarget.BackgroundColor = SystemColors.WindowColor;
        }

        private void Unregister([NotNull] Window window)
        {
            var messageSource = (HwndSource)PresentationSource.FromDependencyObject(window);

            messageSource?.RemoveHook(WindowProc);
        }

        private IntPtr WindowProc(IntPtr windowHandle, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_NCCALCSIZE:
                    var windowPlacement = GetWindowPlacement(windowHandle);
                    if (windowPlacement.showCmd == SW_MAXIMIZE)
                    {
                        var windowInfo = GetWindowInfo(windowHandle);

                        var before = PtrToStructure<RECT>(lParam);
                        NativeMethods.DefWindowProc(windowHandle, WM_NCCALCSIZE, wParam, lParam);
                        var after = PtrToStructure<RECT>(lParam);

                        // Fix for taskbar: taskbar location = bottom, auto hide taskbar, show taskbar on all displays, 3 displays active => on main monitor the task bar does not restore when hovering with the mouse.
                        var monitorinfo = MonitorInfoFromWindow(windowHandle);
                        if (monitorinfo.rcMonitor.Height == monitorinfo.rcWork.Height && monitorinfo.rcMonitor.Width == monitorinfo.rcWork.Width)
                        {
                            --after.Bottom;
                        }

                        after.Top = before.Top + (int)windowInfo.cyWindowBorders;

                        if ((windowInfo.dwExStyle & WS_EX_CLIENTEDGE) != 0)
                        {
                            var x = NativeMethods.GetSystemMetrics(SM_CXEDGE);
                            after.Right += 2 * x;
                            after.Left -= x;
                        }

                        Marshal.StructureToPtr(after, lParam, true);

                        _windowOffset = after.TopLeft - before.TopLeft;
                    }
                    else
                    {
                        _windowOffset = new POINT();
                    }

                    // We do all drawings...
                    handled = true;
                    break;

                case WM_NCACTIVATE:
                    handled = true;
                    // Schedule a redraw, if DWM composition is disabled, DefWindowProc(...WM_NCACTIVATE..) *does* some extra drawing.
                    NativeMethods.RedrawWindow(windowHandle, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.Invalidate | RedrawWindowFlags.NoErase);
                    // Must call DefWindowProc with lParam set to -1, else windows might do some custom drawing in the NC area.
                    return NativeMethods.DefWindowProc(windowHandle, WM_NCACTIVATE, wParam, new IntPtr(-1));

                case WM_NCHITTEST:
                    handled = true;
                    var result = NcHitTest(windowHandle, lParam);
                    return (IntPtr)result;

                case WM_NCRBUTTONUP:
                    var hitTest = wParam.ToInt32();
                    if ((hitTest == (int)HitTest.SysMenu) || (hitTest == (int)HitTest.Caption))
                        ShowSystemMenu(windowHandle, LOWORD(lParam), HIWORD(lParam));
                    break;

                case WM_DWMCOMPOSITIONCHANGED:
                    handled = true;
                    ShowGlassFrame(windowHandle);
                    break;
            }

            return IntPtr.Zero;
        }

        private HitTest NcHitTest(IntPtr windowHandle, IntPtr lParam)
        {
            var window = _window;
            Debug.Assert(window != null, nameof(window) + " != null");

            // Arguments are absolute native coordinates
            var hitPoint = new POINT((short)lParam, (short)((uint)lParam >> 16));

            NativeMethods.GetWindowRect(windowHandle, out var windowRect);

            var topLeft = windowRect.TopLeft + _windowOffset;

            var cornerSize = (SIZE)_transformToDevice.Transform((Point)CornerSize);
            var borderSize = (SIZE)_transformToDevice.Transform((Point)BorderSize);

            if ((window.ResizeMode == ResizeMode.CanResize) || window.ResizeMode == ResizeMode.CanResizeWithGrip)
            {
                if (WindowState.Maximized != window.WindowState)
                {
                    var bottomRight = windowRect.BottomRight;
                    var left = topLeft.X;
                    var top = topLeft.Y;
                    var right = bottomRight.X;
                    var bottom = bottomRight.Y;

                    if ((hitPoint.Y < top) || (hitPoint.Y > bottom) || (hitPoint.X < left) || (hitPoint.X > right))
                        return HitTest.Transparent;

                    if ((hitPoint.Y < (top + cornerSize.Height)) && (hitPoint.X < (left + cornerSize.Width)))
                        return HitTest.TopLeft;
                    if ((hitPoint.Y < (top + cornerSize.Height)) && (hitPoint.X > (right - cornerSize.Width)))
                        return HitTest.TopRight;
                    if ((hitPoint.Y > (bottom - cornerSize.Height)) && (hitPoint.X < (left + cornerSize.Width)))
                        return HitTest.BottomLeft;
                    if ((hitPoint.Y > (bottom - cornerSize.Height)) && (hitPoint.X > (right - cornerSize.Width)))
                        return HitTest.BottomRight;
                    if (hitPoint.Y < (top + borderSize.Height))
                        return HitTest.Top;
                    if (hitPoint.Y > (bottom - borderSize.Height))
                        return HitTest.Bottom;
                    if (hitPoint.X < (left + borderSize.Width))
                        return HitTest.Left;
                    if (hitPoint.X > (right - borderSize.Width))
                        return HitTest.Right;
                }
            }

            // Now check Tag or send an internal NcHitTest event, so any element can override the behavior.
            // The caption must e.g. return HitTest.Caption
            var clientPoint = _transformFromDevice.Transform(hitPoint - topLeft);

            if (window.InputHitTest(clientPoint) is FrameworkElement element)
            {
                if (element.Tag is HitTest value)
                {
                    if (Enum.IsDefined(typeof(HitTest), value))
                        return value;
                }

                var args = new NcHitTestEventArgs();

                element.RaiseEvent(args);

                if (args.Handled)
                    return args.HitTest;
            }

            return HitTest.Client;
        }

        private void ShowSystemMenu(IntPtr handle, int x, int y)
        {
            var systemMenu = NativeMethods.GetSystemMenu(handle, false);

            var windowState = _window?.WindowState;
            var resizeMode = _window?.ResizeMode;

            NativeMethods.EnableMenuItem(systemMenu, SC_RESTORE, MenuFlags(windowState != WindowState.Normal));
            NativeMethods.EnableMenuItem(systemMenu, SC_MOVE, MenuFlags(windowState == WindowState.Normal));
            NativeMethods.EnableMenuItem(systemMenu, SC_SIZE, MenuFlags((windowState == WindowState.Normal) && ((resizeMode == ResizeMode.CanResize) || (resizeMode == ResizeMode.CanResizeWithGrip))));
            NativeMethods.EnableMenuItem(systemMenu, SC_MINIMIZE, MenuFlags((windowState != WindowState.Minimized) && (resizeMode != ResizeMode.NoResize)));
            NativeMethods.EnableMenuItem(systemMenu, SC_MAXIMIZE, MenuFlags((windowState != WindowState.Maximized) && (resizeMode != ResizeMode.NoResize)));

            var cmd = NativeMethods.TrackPopupMenuEx(systemMenu, 256U, x, y, handle, IntPtr.Zero);
            if (cmd == 0)
                return;

            NativeMethods.PostMessage(handle, WM_SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
        }

        // ReSharper disable InconsistentNaming
        private const int SW_MAXIMIZE = 3;

        private const int WM_NCHITTEST = 0x0084;
        private const int WM_NCCALCSIZE = 0x0083;
        private const int WM_NCACTIVATE = 0x0086;
        private const int WM_NCRBUTTONUP = 165;
        private const int WM_DWMCOMPOSITIONCHANGED = 798;
        private const int WM_SYSCOMMAND = 274;

        private const int SC_SIZE = 61440;
        private const int SC_MOVE = 61456;
        private const int SC_MINIMIZE = 61472;
        private const int SC_MAXIMIZE = 61488;
        private const int SC_RESTORE = 61728;

        private const uint MF_ENABLED = 0;
        private const uint MF_GRAYED = 1;
        private const uint MF_DISABLED = 2;

        private const int SM_CXEDGE = 45;
        // private const int SM_CYEDGE = 46;

        private const int WS_EX_CLIENTEDGE = 0x00000200;

        private static uint MenuFlags(bool enabled)
        {
            return enabled ? MF_ENABLED : MF_DISABLED | MF_GRAYED;
        }

        private static int HIWORD(IntPtr i)
        {
            return (short)(i.ToInt32() >> 16);
        }

        private static int LOWORD(IntPtr i)
        {
            return (short)(i.ToInt32() & ushort.MaxValue);
        }

        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        // ReSharper disable UnusedMember.Local
#pragma warning disable 169 // field is never used
#pragma warning disable 649 // field is never assigned

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public POINT TopLeft => new POINT { X = Left, Y = Top };

            public POINT BottomRight => new POINT { X = Right, Y = Bottom };

            public int Width => Right - Left;

            public int Height => Bottom - Top;

            public static implicit operator Rect(RECT r)
            {
                return new Rect(r.TopLeft, r.BottomRight);
            }

            public override string ToString()
            {
                return ((Rect)this).ToString();
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        private struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static POINT operator +(POINT p1, POINT p2)
            {
                return new POINT { X = p1.X + p2.X, Y = p1.Y + p2.Y };
            }

            public static POINT operator -(POINT p1, POINT p2)
            {
                return new POINT { X = p1.X - p2.X, Y = p1.Y - p2.Y };
            }

            public static implicit operator Point(POINT p)
            {
                return new Point(p.X, p.Y);
            }

            public static implicit operator POINT(Point p)
            {
                return new POINT((int)Math.Round(p.X), (int)Math.Round(p.Y));
            }

            public override string ToString()
            {
                return ((Point)this).ToString();
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        private struct SIZE
        {
            public readonly int Width;
            public readonly int Height;

            public SIZE(int width, int height)
            {
                Width = width;
                Height = height;
            }

            public static implicit operator SIZE(Point p)
            {
                return new SIZE((int)Math.Round(p.X), (int)Math.Round(p.Y));
            }

            public static implicit operator Size(SIZE s)
            {
                return new Size(s.Width, s.Height);
            }

            public override string ToString()
            {
                return ((Size)this).ToString();
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        private struct MARGINS
        {
            public readonly int Left;
            public readonly int Right;
            public readonly int Top;
            public readonly int Bottom;

            public MARGINS(int value)
            {
                Left = value;
                Top = value;
                Right = value;
                Bottom = value;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public readonly POINT ptReserved;
            public readonly POINT ptMaxSize;
            public readonly POINT ptMaxPosition;
            public readonly POINT ptMinTrackSize;
            public readonly POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential)]
        private class WINDOWPLACEMENT
        {
            public int length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
            public int flags;
            public int showCmd;
            public POINT ptMinPosition;
            public POINT ptMaxPosition;
            public RECT rcNormalPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO
        {
            public uint cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWINFO
        {
            public int cbSize;
            public RECT rcWindow;
            public RECT rcClient;
            public int dwStyle;
            public int dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public ushort wCreatorVersion;
        }

        [NotNull]
        private static WINDOWPLACEMENT GetWindowPlacement(IntPtr hwnd)
        {
            var lpwndpl = new WINDOWPLACEMENT();
            NativeMethods.GetWindowPlacement(hwnd, lpwndpl);
            return lpwndpl;
        }

        private static MONITORINFO MonitorInfoFromWindow(IntPtr hWnd)
        {
            var hMonitor = NativeMethods.MonitorFromWindow(hWnd, 2);
            var monitorInfo = new MONITORINFO { cbSize = (uint)Marshal.SizeOf(typeof(MONITORINFO)) };
            NativeMethods.GetMonitorInfo(hMonitor, ref monitorInfo);
            return monitorInfo;
        }

        private static WINDOWINFO GetWindowInfo(IntPtr hWnd)
        {
            var pwi = new WINDOWINFO { cbSize = Marshal.SizeOf(typeof(WINDOWINFO)) };
            NativeMethods.GetWindowInfo(hWnd, ref pwi);
            return pwi;
        }

        private static T PtrToStructure<T>(IntPtr ptr)
        {
            return (T)Marshal.PtrToStructure(ptr, typeof(T));
        }

        [Flags]
        private enum RedrawWindowFlags : uint
        {
            /// <summary>
            /// Invalidates the rectangle or region that you specify in lprcUpdate or hrgnUpdate.
            /// You can set only one of these parameters to a non-NULL value. If both are NULL, RDW_INVALIDATE invalidates the entire window.
            /// </summary>
            Invalidate = 0x1,

            /// <summary>Causes the OS to post a WM_PAINT message to the window regardless of whether a portion of the window is invalid.</summary>
            InternalPaint = 0x2,

            /// <summary>
            /// Causes the window to receive a WM_ERASEBKGND message when the window is repainted.
            /// Specify this value in combination with the RDW_INVALIDATE value; otherwise, RDW_ERASE has no effect.
            /// </summary>
            Erase = 0x4,

            /// <summary>
            /// Validates the rectangle or region that you specify in lprcUpdate or hrgnUpdate.
            /// You can set only one of these parameters to a non-NULL value. If both are NULL, RDW_VALIDATE validates the entire window.
            /// This value does not affect internal WM_PAINT messages.
            /// </summary>
            Validate = 0x8,

            NoInternalPaint = 0x10,

            /// <summary>Suppresses any pending WM_ERASEBKGND messages.</summary>
            NoErase = 0x20,

            /// <summary>Excludes child windows, if any, from the repainting operation.</summary>
            NoChildren = 0x40,

            /// <summary>Includes child windows, if any, in the repainting operation.</summary>
            AllChildren = 0x80,

            /// <summary>Causes the affected windows, which you specify by setting the RDW_ALLCHILDREN and RDW_NOCHILDREN values, to receive WM_ERASEBKGND and WM_PAINT messages before the RedrawWindow returns, if necessary.</summary>
            UpdateNow = 0x100,

            /// <summary>
            /// Causes the affected windows, which you specify by setting the RDW_ALLCHILDREN and RDW_NOCHILDREN values, to receive WM_ERASEBKGND messages before RedrawWindow returns, if necessary.
            /// The affected windows receive WM_PAINT messages at the ordinary time.
            /// </summary>
            EraseNow = 0x200,

            Frame = 0x400,

            NoFrame = 0x800
        }

        private sealed class DeferSizeToContent : IDisposable
        {
            [NotNull]
            private readonly Window _window;

            private readonly SizeToContent _sizeToContent;

            public DeferSizeToContent([NotNull] Window window)
            {
                _window = window;
                _sizeToContent = window.SizeToContent;

                window.SizeToContent = SizeToContent.Manual;
            }

            public void Dispose()
            {
                _window.SizeToContent = _sizeToContent;
            }
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

            [DllImport("dwmapi.dll")]
            public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

            [DllImport("dwmapi.dll", EntryPoint = "DwmIsCompositionEnabled", PreserveSig = false)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DwmIsCompositionEnabled();

            [DllImport("user32.dll")]
            public static extern uint TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

            [DllImport("user32.dll")]
            public static extern IntPtr GetSystemMenu(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern int EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowPlacement(IntPtr hwnd, WINDOWPLACEMENT lpwndpl);

            [DllImport("user32.dll")]
            public static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO monitorInfo);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

            [DllImport("user32.dll")]
            public static extern int GetSystemMetrics(int nIndex);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, [MarshalAs(UnmanagedType.Bool)] bool bRedraw);
        }
    }
}
