using System;
using System.Windows;
using System.Globalization;
using System.Windows.Data;

namespace Sprightly.Presentation.Views.Pages.ProjectPage.Toolboxes.SpriteToolbox
{
    [ValueConversion(typeof(Common.PageType),
                     typeof(int))]
    public class DetailTypeConverter : IValueConverter
    {
        public object Convert(object value, 
                              Type targetType, 
                              object parameter, 
                              CultureInfo culture)
        {
            if (!(value is Common.DetailType detailType)) 
                return DependencyProperty.UnsetValue;

            if (detailType.IsNone)    
                return DetailType.None;
            if (detailType.IsTexture)   
                return DetailType.Texture;
            if (detailType.IsSprite) 
                return DetailType.Sprite;

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, 
                                  Type targetType, 
                                  object parameter, 
                                  CultureInfo culture)
        {
            if (!(value is DetailType))
                return DependencyProperty.UnsetValue;

            return value switch
            {
                DetailType.None => Common.DetailType.None,
                DetailType.Texture => Common.DetailType.Texture,
                DetailType.Sprite => Common.DetailType.Sprite,
                _ => DependencyProperty.UnsetValue,
            };
        }
    }
}