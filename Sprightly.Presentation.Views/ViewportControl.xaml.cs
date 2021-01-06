using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Sprightly.Common.KoboldLayer.Components;

namespace Sprightly.Presentation.Views
{
    /// <summary>
    /// Interaction logic for ViewportControl.xaml
    /// </summary>
    public partial class ViewportControl : UserControl
    {
        private ViewportHost _viewportHost;
        private readonly IViewport _viewport;

        public ViewportControl()
        {
            InitializeComponent();
            _viewport = new Viewport();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitializeViewport();
        }

        private void InitializeViewport()
        {
            _viewportHost = new ViewportHost(ViewportCanvas.ActualWidth, ViewportCanvas.ActualHeight, _viewport);
            ViewportCanvas.Child = _viewportHost;

            _viewportHost.MessageHook += new HwndSourceHook(ControlMsgFilter);
        }

        private IntPtr ControlMsgFilter(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            handled = false;
            return IntPtr.Zero;
        }
    }
}
