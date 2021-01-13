using System;
using System.Windows;
using System.Globalization;
using System.Windows.Data;
using Sprightly.Common.KoboldLayer.Components;
using Sprightly.Presentation.Views.Pages.ProjectPage.RenderStrategies;

namespace Sprightly.Presentation.Views.Pages.ProjectPage
{
    [ValueConversion(typeof(Common.RenderStrategyType),
                     typeof(IRenderStrategy))]
    public class RenderStrategyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Common.RenderStrategyType renderStrategyType))
                return DependencyProperty.UnsetValue;

            switch (renderStrategyType)
            {
                case Common.RenderStrategyType.Texture {Item: var textureInfo}:
                    return CreateTextureRenderStrategy(textureInfo);
                case var t when t.IsNoSelection:
                    return CreateNothingSelectedRenderStrategy();
            }

            return DependencyProperty.UnsetValue;
        }

        private static IRenderStrategy CreateTextureRenderStrategy(Common.TextureRenderStrategyInfo textureRenderStrategyInfo) =>
            new TextureRenderStrategy(textureRenderStrategyInfo.TextureId);

        private static IRenderStrategy CreateNothingSelectedRenderStrategy() =>
            new NothingSelectedRenderStrategy();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("It is currently not supported to convert a render strategy back to a render strategy type.");
        }
    }
}