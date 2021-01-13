using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Sprightly.Common.KoboldLayer.Components
{
    /// <summary>
    /// <see cref="ViewportHost"/> provides the <see cref="HwndHost"/>
    /// implementation in order to host the
    /// <see cref="kobold_layer.clr.view"/> object.
    /// </summary>
    /// <seealso cref="HwndHost" />
    public sealed class ViewportHost : HwndHost
    {
        #region Constant Interop Values
        // Interop values, see: https://docs.microsoft.com/en-us/windows/win32/winmsg/extended-window-styles
        internal const int WsChild = 0x40000000;
        internal const int WsVisible = 0x10000000;
        internal const int HostId = 0x00000002;
        internal const int WmErasebkgnd = 0x0014;
        internal const int WmCreate = 0x0001;
        internal const int WmPaint = 0x000F;
        internal const int WmShowWindow = 0x0018;
        internal const int WmTimer = 0x0113;
        #endregion

        private IntPtr _hwndHost;
        private readonly IViewport _viewport;

        private readonly int _hostHeight;
        private readonly int _hostWidth;

        /// <summary>
        /// Creates a new <see cref="ViewportHost"/>.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="viewport">The viewport used to render with.</param>
        public ViewportHost(double width, 
                            double height, 
                            IViewport viewport)
        {
            this._viewport = viewport;
            _hostWidth = (int) width;
            _hostHeight = (int) height;
        }

#nullable enable        
        /// <summary>
        /// Gets or sets the render strategy.
        /// </summary>
        public IRenderStrategy? RenderStrategy { get; set; }
#nullable restore

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            _hwndHost = CreateWindowEx(0, "static", "",
                WsChild | WsVisible,
                0, 0,
                _hostWidth,
                _hostHeight,
                hwndParent.Handle,
                (IntPtr) HostId,
                IntPtr.Zero,
                0);

            _viewport.Initialise(_hwndHost);
            RenderFrame();

            return new HandleRef(this, _hwndHost);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            DestroyWindow(hwnd.Handle);
        }

        private void RenderFrame()
        {
            _viewport.BeginRender();

            RenderStrategy?.RenderFrame(_viewport);

            _viewport.FinaliseRender();
        }

        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WmShowWindow:
                    if (RenderTimer == UIntPtr.Zero)
                    {
                        RenderTimer = SetTimer(hwnd, UIntPtr.Zero, 1000, IntPtr.Zero);
                    }

                    handled = false;
                    break;
                case WmErasebkgnd:
                    RenderFrame();
                    handled = true;
                    break;
                case WmTimer:
                    InvalidateRect(hwnd, IntPtr.Zero, false);
                    handled = true;
                    break;
                case WmPaint:
                    RenderFrame();
                    handled = false;
                    break;
                default:
                    handled = false;
                    break;
            }

            return IntPtr.Zero;
        }

        private UIntPtr RenderTimer { get; set; }

        #region P/Invoke Declarations

        [DllImport("user32.dll", EntryPoint = nameof(CreateWindowEx), CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateWindowEx(int dwExStyle,
            string lpszClassName,
            string lpszWindowName,
            int style,
            int x, int y,
            int width, int height,
            IntPtr hwndParent,
            IntPtr hMenu,
            IntPtr hInst,
            [MarshalAs(UnmanagedType.AsAny)] object pvParam);

        [DllImport("user32.dll", EntryPoint = nameof(DestroyWindow), CharSet = CharSet.Unicode)]
        internal static extern bool DestroyWindow(IntPtr hwnd);

        [DllImport("user32.dll", EntryPoint = nameof(SetTimer), CharSet = CharSet.Unicode)]
        internal static extern UIntPtr SetTimer(IntPtr hwnd, UIntPtr nIDEvent, uint uElapse, IntPtr lpTimerFunc);

        [DllImport("user32.dll", EntryPoint = nameof(InvalidateRect), CharSet = CharSet.Unicode)]
        internal static extern bool InvalidateRect(IntPtr hwnd, IntPtr lpRect, bool bErase);
        #endregion
    }
}