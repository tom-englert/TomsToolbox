namespace TomsToolbox.Wpf.Interactivity
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interactivity;
    using System.Windows.Interop;
    using System.Windows.Media;
    using TomsToolbox.Desktop;

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
            get
            {
                return _hitTest;
            }
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
        private Matrix _transformFromDevice = Matrix.Identity;
        private Matrix _transformToDevice = Matrix.Identity;
        private Window _window;

        /// <summary>
        /// Gets or sets the size of the border used to size the window.
        /// </summary>
        /// <value>
        /// The size of the border.
        /// </value>
        public Size BorderSize
        {
            get { return this.GetValue<Size>(BorderSizeProperty); }
            set { SetValue(BorderSizeProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="BorderSize"/> dependency property
        /// </summary>
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
            get { return this.GetValue<Size>(CornerSizeProperty); }
            set { SetValue(CornerSizeProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="CornerSize"/> dependency property
        /// </summary>
        public static readonly DependencyProperty CornerSizeProperty =
            DependencyProperty.Register("CornerSize", typeof(Size), typeof(CustomNonClientAreaBehavior), new FrameworkPropertyMetadata(new Size(8, 8)));


        /// <summary>
        /// The WM_NCHITTEST test event equivalent.
        /// </summary>
        public static readonly RoutedEvent NcHitTestEvent = EventManager.RegisterRoutedEvent("NcHitTest", RoutingStrategy.Bubble, typeof(EventHandler<NcHitTestEventArgs>), typeof(CustomNonClientAreaBehavior));


        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            var clientArea = AssociatedObject;
            Contract.Assume(clientArea != null);

            _window = clientArea.TryFindAncestor<Window>();
            if (_window == null)
                return;

            _window.SourceInitialized += Window_SourceInitialized;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (_window == null)
                return;

            _window.SourceInitialized -= Window_SourceInitialized;

            Unregister(_window);
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            Contract.Assume(_window != null);

            var messageSource = (HwndSource)PresentationSource.FromDependencyObject(_window);

            if (messageSource == null)
                throw new InvalidOperationException("Window needs to be initialized");

            var compositionTarget = messageSource.CompositionTarget;
            if (compositionTarget == null)
                throw new InvalidOperationException("Window needs to be initialized");

            _transformFromDevice = compositionTarget.TransformFromDevice;
            _transformToDevice = compositionTarget.TransformToDevice;

            messageSource.AddHook(WindowProc);
        }

        private void Unregister(Window window)
        {
            Contract.Requires(window != null);

            var messageSource = (HwndSource)PresentationSource.FromDependencyObject(window);

            if (messageSource != null)
                messageSource.RemoveHook(WindowProc);
        }

        private IntPtr WindowProc(IntPtr windowHandle, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeMethods.WM_NCCALCSIZE:
                    // We do all drawings...
                    handled = true;
                    break;

                case NativeMethods.WM_NCACTIVATE:
                    // Must call DefWindowProc with lParam set to -1, else windows might do some custom drawing in the NC area.
                    handled = true;
                    return NativeMethods.DefWindowProc(windowHandle, NativeMethods.WM_NCACTIVATE, wParam, new IntPtr(-1));

                case NativeMethods.WM_NCHITTEST:
                    handled = true;
                    var result = NcHitTest(windowHandle, lParam);
                    return (IntPtr)result;

                case NativeMethods.WM_GETMINMAXINFO:
                    GetMinMaxInfo(windowHandle, lParam);
                    // Don't set handled = true, framework needs to handle Min/Max sizes.
                    break;
            }

            return IntPtr.Zero;
        }

        private HitTest NcHitTest(IntPtr windowHandle, IntPtr lParam)
        {
            var clientArea = AssociatedObject;
            Contract.Assume(clientArea != null);
            var window = _window;
            Contract.Assume(window != null);

            // Arguments are absolute native coordinates
            var hitPoint = new NativeMethods.POINT((short)lParam, (short)((uint)lParam >> 16));

            NativeMethods.RECT windowRect;
            NativeMethods.GetWindowRect(windowHandle, out windowRect);

            var clientRect = clientArea.GetClientRect(window);

            var topLeft = windowRect.TopLeft + _transformToDevice.Transform(clientRect.TopLeft);
            var bottomRight = windowRect.TopLeft + _transformToDevice.Transform(clientRect.BottomRight);

            var left = topLeft.X;
            var top = topLeft.Y;
            var right = bottomRight.X;
            var bottom = bottomRight.Y;

            var cornerSize = (NativeMethods.SIZE)_transformToDevice.Transform((Point)CornerSize);
            var borderSize = (NativeMethods.SIZE)_transformToDevice.Transform((Point)BorderSize);

            if ((window.ResizeMode == ResizeMode.CanResize) || window.ResizeMode == ResizeMode.CanResizeWithGrip)
            {
                if (WindowState.Maximized != window.WindowState)
                {
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
            var element = clientArea.InputHitTest(clientPoint) as FrameworkElement;

            if (element != null)
            {
                if (element.Tag is HitTest)
                {
                    var value = (HitTest)element.Tag;
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

        [ContractVerification(false)]
        private static void GetMinMaxInfo(IntPtr handle, IntPtr parameter)
        {
            // Since we don't have the WS_CAPTION style windows defaults to maximize to monitor area,
            // but we want to behave like a normal window, so we override the defaults and use the work area.
            var mmi = (NativeMethods.MINMAXINFO)Marshal.PtrToStructure(parameter, typeof(NativeMethods.MINMAXINFO));

            var currentScreen = new Screen(handle);

            mmi.ptMaxPosition.X = Math.Abs(currentScreen.WorkingArea.Left - currentScreen.Bounds.Left);
            mmi.ptMaxPosition.Y = Math.Abs(currentScreen.WorkingArea.Top - currentScreen.Bounds.Top);

            if (currentScreen.Primary)
            {
                mmi.ptMaxSize.X = Math.Abs(currentScreen.WorkingArea.Right - currentScreen.WorkingArea.Left);
                mmi.ptMaxSize.Y = Math.Abs(currentScreen.WorkingArea.Bottom - currentScreen.WorkingArea.Top);
            }
            else
            {
                // don't calculate max size on secondary monitor, see remarks in http://msdn.microsoft.com/en-us/library/ms632605%28VS.85%29.aspx
                mmi.ptMaxSize -= mmi.ptMinTrackSize;
            }

            Marshal.StructureToPtr(mmi, parameter, true);
        }

        private class Screen
        {
            private static readonly bool _multiMonitorSupport = (uint)NativeMethods.GetSystemMetrics(80) > 0U;

            public readonly NativeMethods.RECT WorkingArea;
            public readonly NativeMethods.RECT Bounds;
            public readonly bool Primary;

            public Screen(IntPtr handle)
            {
                if (!_multiMonitorSupport)
                {
                    WorkingArea = new NativeMethods.RECT();
                    NativeMethods.SystemParametersInfo(48, 0, ref WorkingArea, 0);
                    Primary = true;
                    Bounds = new NativeMethods.RECT(0, 0, NativeMethods.GetSystemMetrics(0), NativeMethods.GetSystemMetrics(1));
                }
                else
                {
                    var currentScreen = NativeMethods.MonitorFromWindow(new HandleRef(null, handle), 2);
                    var info = new NativeMethods.MONITORINFOEX();
                    NativeMethods.GetMonitorInfo(new HandleRef(null, currentScreen), info);
                    WorkingArea = info.rcWork;
                    Bounds = info.rcMonitor;
                    Primary = (uint)(info.dwFlags & 1) > 0U;
                }
            }
        }

        static class NativeMethods
        {
            public const int WM_GETMINMAXINFO = 0x0024;
            public const int WM_NCHITTEST = 0x0084;
            public const int WM_NCCALCSIZE = 0x0083;
            public const int WM_NCACTIVATE = 0x0086;

            [StructLayout(LayoutKind.Sequential, Pack = 0)]
            public struct RECT
            {
                public readonly int Left;
                public readonly int Top;
                public readonly int Right;
                public readonly int Bottom;

                public RECT(int left, int top, int right, int bottom)
                {
                    Left = left;
                    Top = top;
                    Right = right;
                    Bottom = bottom;
                }

                public POINT TopLeft
                {
                    get { return new POINT { X = Left, Y = Top }; }
                }

                public POINT BottomRight
                {
                    get { return new POINT { X = Right, Y = Bottom }; }
                }

                public static implicit operator Rect(RECT r)
                {
                    return new Rect(r.TopLeft, r.BottomRight);
                }

                public override string ToString()
                {
                    return ((Rect)this).ToString();
                }
            }

            [ContractVerification(false)]
            [StructLayout(LayoutKind.Sequential, Pack = 0)]
            public struct POINT
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

            [ContractVerification(false)]
            [StructLayout(LayoutKind.Sequential, Pack = 0)]
            public struct SIZE
            {
                public int Width;
                public int Height;

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

            [StructLayout(LayoutKind.Sequential)]
            internal struct MINMAXINFO
            {
                public POINT ptReserved;
                public POINT ptMaxSize;
                public POINT ptMaxPosition;
                public POINT ptMinTrackSize;
                public POINT ptMaxTrackSize;
            };

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

            [DllImport("user32.dll")]
            public static extern IntPtr MonitorFromWindow(HandleRef handle, int flags);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern int GetSystemMetrics(int nIndex);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SystemParametersInfo(int nAction, int nParam, ref RECT rc, int nUpdate);

            [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Auto)]
            public class MONITORINFOEX
            {
                internal readonly int cbSize = Marshal.SizeOf(typeof(NativeMethods.MONITORINFOEX));
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
                internal readonly char[] szDevice = new char[32];
                internal NativeMethods.RECT rcMonitor;
                internal NativeMethods.RECT rcWork;
                internal int dwFlags;
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] NativeMethods.MONITORINFOEX info);
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_transformToDevice != null);
            Contract.Invariant(_transformFromDevice != null);
        }
    }
}
