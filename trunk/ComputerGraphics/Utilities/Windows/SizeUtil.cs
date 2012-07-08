namespace Utilities.Windows
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;

    public static class SizeUtil
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        public static Window downWindow;
        public static Window upWindow;
        public static Size WorkArea { get; set; }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public static void WindowLoaded(object sender, RoutedEventArgs e)
        {
            var w = (Window) sender;
            w.Closing += WindowClosing;
            WorkArea = SystemParameters.WorkArea.Size;

            w.Width = WorkArea.Width/4;
            w.Height = WorkArea.Height;

            w.MaxWidth = WorkArea.Width/4;
            w.MaxHeight = WorkArea.Height;

            w.Left = WorkArea.Width*3/4;
            w.Top = 0;

            RECT rect;
            GetClientRect(new WindowInteropHelper(w).Handle, out rect);

            w.Width = rect.Width;

            upWindow = new Window();
            downWindow = new Window();

            upWindow.SizeChanged += WindowOnSizeChanged;
            downWindow.SizeChanged += WindowOnSizeChanged;

            upWindow.Loaded += UpWindowOnLoaded;
            downWindow.Loaded += DownWindowOnLoaded;

            upWindow.Show();
            downWindow.Show();
        }

        private static void WindowOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            upWindow.Width = sizeChangedEventArgs.NewSize.Width;
            upWindow.Height = sizeChangedEventArgs.NewSize.Height;
            downWindow.Width = sizeChangedEventArgs.NewSize.Width;
            downWindow.Height = sizeChangedEventArgs.NewSize.Height;
        }

        private static void SetWindowSize(Window w)
        {
            w.Width = WorkArea.Width*3/8;
            w.Height = w.Width;
            var hwnd = new WindowInteropHelper(w).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
            RECT rect;
            GetClientRect(new WindowInteropHelper(w).Handle, out rect);
            w.Width = rect.Width;
            w.Height = rect.Height;
            w.MaxWidth = rect.Width;
            w.MaxHeight = rect.Height;
        }

        private static void DownWindowOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var w = (Window)sender;
            SetWindowSize(w);
            w.Left = WorkArea.Width*3/8;
            w.Top = WorkArea.Height/2 - w.Height/2;
        }

        private static void UpWindowOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var w = (Window) sender;
            SetWindowSize(w);
            w.Left = 0;
            w.Top = WorkArea.Height / 2 - w.Height / 2;
        }

        private static void WindowClosing(object sender, CancelEventArgs e)
        {
            upWindow.Close();
            downWindow.Close();
        }

        #region Native Methods

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        public static RECT GetClientRect(IntPtr hWnd)
        {
            RECT result;
            GetClientRect(hWnd, out result);
            return result;
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public readonly int Left;
            public readonly int Top;
            public int Right;
            public int Bottom;

            public RECT(int left_, int top_, int right_, int bottom_)
            {
                Left = left_;
                Top = top_;
                Right = right_;
                Bottom = bottom_;
            }

            public int Height
            {
                get { return Bottom - Top; }
            }

            public int Width
            {
                get { return Right - Left; }
            }

            public Size Size
            {
                get { return new Size(Width, Height); }
            }

            public Point Location
            {
                get { return new Point(Left, Top); }
            }

            // Handy method for converting to a System.Drawing.Rectangle
            public Rect ToRectangle()
            {
                return new Rect(Left, Top, Right, Bottom);
            }

            public static RECT FromRectangle(Rect rectangle)
            {
                return new Rect(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
            }

            public override int GetHashCode()
            {
                return Left ^ ((Top << 13) | (Top >> 0x13))
                       ^ ((Width << 0x1a) | (Width >> 6))
                       ^ ((Height << 7) | (Height >> 0x19));
            }

            #region Operator overloads

            public static implicit operator Rect(RECT rect)
            {
                return rect.ToRectangle();
            }

            public static implicit operator RECT(Rect rect)
            {
                return FromRectangle(rect);
            }

            #endregion
        }

        #endregion
    }
}