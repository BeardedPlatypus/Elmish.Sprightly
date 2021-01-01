using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sprightly.Presentation.Views
{
    [ValueConversion(typeof(Common.PageType),
                     typeof(PageType))]
    public class PageTypeConverter : IValueConverter
    {
        public object Convert(object value, 
                              Type targetType, 
                              object parameter, 
                              CultureInfo culture)
        {
            if (!(value is Common.PageType pageType)) 
                return DependencyProperty.UnsetValue;

            if (pageType.IsStartingPage)   
                return PageType.StartingPage;
            if (pageType.IsNewProjectPage) 
                return PageType.NewProjectPage;
            if (pageType.IsProjectPage)    
                return PageType.ProjectPage;

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, 
                                  Type targetType, 
                                  object parameter, 
                                  CultureInfo culture)
        {
            if (!(value is PageType))
                return DependencyProperty.UnsetValue;

            return value switch
            {
                PageType.StartingPage => Common.PageType.StartingPage,
                PageType.NewProjectPage => Common.PageType.NewProjectPage,
                PageType.ProjectPage => Common.PageType.ProjectPage,
                _ => DependencyProperty.UnsetValue
            };
        }
    }
}