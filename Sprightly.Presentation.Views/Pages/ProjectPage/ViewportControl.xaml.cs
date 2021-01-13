using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Sprightly.Common.KoboldLayer;
using Sprightly.Common.KoboldLayer.Components;

namespace Sprightly.Presentation.Views.Pages.ProjectPage
{
    /// <summary>
    /// Interaction logic for ViewportControl.xaml
    /// </summary>
    public partial class ViewportControl : UserControl
    {
        public static readonly DependencyProperty HasInitialisedCommandProperty = 
            DependencyProperty.Register(nameof(HasInitialisedCommand), 
                                        typeof(ICommand), 
                                        typeof(ViewportControl), 
                                        new PropertyMetadata(default(ICommand)));

        public ICommand HasInitialisedCommand
        {
            get => (ICommand) GetValue(HasInitialisedCommandProperty);
            set => SetValue(HasInitialisedCommandProperty, value);
        }

        public static readonly DependencyProperty RenderStrategyProperty = 
            DependencyProperty.Register(nameof(RenderStrategy),
                                        typeof(IRenderStrategy), 
                                        typeof(ViewportControl), 
                                        new FrameworkPropertyMetadata(
                                            default(IRenderStrategy), 
                                            FrameworkPropertyMetadataOptions.AffectsRender,
                                            new PropertyChangedCallback(OnRenderStrategyChanged)));

        public IRenderStrategy RenderStrategy
        {
            get => (IRenderStrategy) GetValue(RenderStrategyProperty);
            set => SetValue(RenderStrategyProperty, value);
        }

        private ViewportHost _viewportHost;
        private readonly IViewport _viewport;

        public ViewportControl()
        {
            InitializeComponent();
            _viewport = ViewportFactory.Create();
        }

        private static void OnRenderStrategyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ViewportControl;

            if (control._viewportHost != null)
            {
                control._viewportHost.RenderStrategy = (IRenderStrategy) e.NewValue;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitializeViewport();
        }

        private void InitializeViewport()
        {
            _viewportHost = new ViewportHost(ViewportCanvas.ActualWidth, ViewportCanvas.ActualHeight, _viewport);
            _viewportHost.RenderStrategy = RenderStrategy;

            ViewportCanvas.Child = _viewportHost;

            _viewportHost.MessageHook += new HwndSourceHook(ControlMsgFilter);

            HasInitialisedCommand?.Execute(null);
        }

        private IntPtr ControlMsgFilter(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            handled = false;
            return IntPtr.Zero;
        }
    }
}
