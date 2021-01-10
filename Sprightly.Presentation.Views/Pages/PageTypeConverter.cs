using System;
using System.Windows;
using System.Globalization;
using System.Windows.Data;

namespace Sprightly.Presentation.Views.Pages
{
    [ValueConversion(typeof(Common.PageType),
                     typeof(int))]
    public class PageTypeConverter : IValueConverter
    {
        public object Convert(object value, 
                              Type targetType, 
                              object parameter, 
                              CultureInfo culture)
        {
            if (!(value is Common.PageType pageType)) 
                return DependencyProperty.UnsetValue;

            if (pageType.IsProjectPage)    
                return 0;
            if (pageType.IsStartingPage)   
                return 1;
            if (pageType.IsNewProjectPage) 
                return 2;

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, 
                                  Type targetType, 
                                  object parameter, 
                                  CultureInfo culture)
        {
            if (!(value is int))
                return DependencyProperty.UnsetValue;

            return value switch
            {
                0 => Common.PageType.ProjectPage,
                1 => Common.PageType.NewProjectPage,
                2 => Common.PageType.StartingPage,
                _ => DependencyProperty.UnsetValue
            };
        }
    }
}