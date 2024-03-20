/*******************************************************************
 Copyright 2024 CornellPad

 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0

 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
 or implied. See the License for the specific language governing
 permissions and limitations under the License.
 *******************************************************************/

using System.Globalization;

namespace CornellPad.DataTypes;

public class GlyphCollectionItem
{
    public string GlyphFamily { get; set; }
    public string GlyphValue { get; set; }
}

public class TopicCreationGlyphItemConverter : IValueConverter, IMarkupExtension
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        GlyphCollectionItem glyphCollectionItem = (GlyphCollectionItem)value;
        if (glyphCollectionItem is null)
        {
            return null;
        }

        return new FontImageSource
        {
            FontFamily = glyphCollectionItem.GlyphFamily,
            Glyph = glyphCollectionItem.GlyphValue,
            Color = GlyphThemeColor(),
            Size = 64
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }

    private Color GlyphThemeColor()
    {
        Color currentColor = new Color(1.0f, 0.0f, 0.0f); // Shouldn't ever see red if things work.

        var currentApp = Application.Current as CornellPad.App;

        // Provide some color values, if the cast fails.
        if (currentApp is null)
        {
            AppTheme currentTheme = Application.Current.RequestedTheme;

            switch (currentTheme)
            {
                case AppTheme.Light:
                    if (App.Current.Resources.TryGetValue("Black", out var colorvalueLM))
                        currentColor = colorvalueLM as Color;
                    break;
                case AppTheme.Dark:
                    if (App.Current.Resources.TryGetValue("Grey_900", out var colorvalueDM))
                        currentColor = colorvalueDM as Color;
                    break;
                default:
                    if (App.Current.Resources.TryGetValue("White", out var colorvalue))
                        currentColor = colorvalue as Color;
                    break;
            }

            return currentColor;
        }

        // We CAN use our theme enum to dictate what color the glyph will be.
        switch (currentApp.GetCurrentTheme())
        {
            case CornellPadTheme.Auto:
                AppTheme currentTheme = Application.Current.RequestedTheme;
                if (currentTheme == AppTheme.Light || currentTheme == AppTheme.Unspecified)
                {
                    if (App.Current.Resources.TryGetValue("Black", out var colorValLM))
                        currentColor = colorValLM as Color;
                }
                else
                {
                    if (App.Current.Resources.TryGetValue("Grey_900", out var colorValDM))
                        currentColor = colorValDM as Color;
                }
                break;
            case CornellPadTheme.Dark:
                if (App.Current.Resources.TryGetValue("Grey_900", out var colorvalueDM))
                    currentColor = colorvalueDM as Color;
                break;
            case CornellPadTheme.Light:
                if (App.Current.Resources.TryGetValue("Black", out var colorvalueLM))
                    currentColor = colorvalueLM as Color;
                break;
            default:
                if (App.Current.Resources.TryGetValue("White", out var colorvalue))
                    currentColor = colorvalue as Color;
                break;
        }

        return currentColor;
    }
}

public class GlyphCollectionItemConverter : IValueConverter, IMarkupExtension
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        GlyphCollectionItem glyphCollectionItem = (GlyphCollectionItem)value;
        if (glyphCollectionItem is null)
        {
            return null;
        }

        return new FontImageSource
        {
            FontFamily = glyphCollectionItem.GlyphFamily,
            Glyph = glyphCollectionItem.GlyphValue,
            Color = GlyphThemeColor(),
            Size = 64
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }

    private Color GlyphThemeColor()
    {
        Color currentColor = new Color(1.0f, 0.0f, 0.0f); // Shouldn't ever see red if things work.

        var currentApp = Application.Current as CornellPad.App;

        // Provide some color values, if the cast fails.
        if (currentApp is null)
        {
            AppTheme currentTheme = Application.Current.RequestedTheme;

            switch (currentTheme)
            {
                case AppTheme.Light:
                    if (App.Current.Resources.TryGetValue("Black", out var colorvalueLM))
                        currentColor = colorvalueLM as Color;
                    break;
                case AppTheme.Dark:
                    if (App.Current.Resources.TryGetValue("Grey_900", out var colorvalueDM))
                        currentColor = colorvalueDM as Color;
                    break;
                default:
                    if (App.Current.Resources.TryGetValue("Black", out var colorvalue))
                        currentColor = colorvalue as Color;
                    break;
            }

            return currentColor;
        }

        // We CAN use our theme enum to dictate what color the glyph will be.
        switch (currentApp.GetCurrentTheme())
        {
            case CornellPadTheme.Auto:
                AppTheme currentTheme = Application.Current.RequestedTheme;
                if (currentTheme == AppTheme.Dark)
                {
                    if (App.Current.Resources.TryGetValue("Grey_900", out var colorValDM))
                        currentColor = colorValDM as Color;
                }
                else
                {
                    if (App.Current.Resources.TryGetValue("Black", out var colorValDM))
                        currentColor = colorValDM as Color;
                }
                break;
            case CornellPadTheme.Dark:
                if (App.Current.Resources.TryGetValue("Grey_900", out var colorvalueDM))
                    currentColor = colorvalueDM as Color;
                break;
            default:
                if (App.Current.Resources.TryGetValue("Black", out var colorvalue))
                    currentColor = colorvalue as Color;
                break;
        }

        return currentColor;
    }
}

public class SummaryGlyphConverter : IValueConverter, IMarkupExtension
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        GlyphCollectionItem glyphCollectionItem = (GlyphCollectionItem)value;
        if (glyphCollectionItem is null)
        {
            return null;
        }

        return new FontImageSource
        {
            FontFamily = glyphCollectionItem.GlyphFamily,
            Glyph = glyphCollectionItem.GlyphValue,
            Color = GlyphThemeColor(),
            Size = 250
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }

    private Color GlyphThemeColor()
    {
        Color currentColor = new Color(1.0f, 0.0f, 0.0f); // Shouldn't ever see red if things work.

        var currentApp = Application.Current as CornellPad.App;
        
        // We need valid color values, and something is better than nothing.
        if (currentApp is null)
        {
            AppTheme currentTheme = Application.Current.RequestedTheme;

            switch (currentTheme)
            {
                case AppTheme.Light:
                    if (App.Current.Resources.TryGetValue("Black_20percent", out var colorvalueLM))
                        currentColor = colorvalueLM as Color;
                    break;
                case AppTheme.Dark:
                    if (App.Current.Resources.TryGetValue("Black_33percent", out var colorvalueDM))
                        currentColor = colorvalueDM as Color;
                    break;
                default:
                    if (App.Current.Resources.TryGetValue("Black", out var colorvalue))
                        currentColor = colorvalue as Color;
                    break;
            }

            return currentColor;
        }

        switch (currentApp.GetCurrentTheme())
        {
            case CornellPadTheme.Auto:
                AppTheme currentTheme = Application.Current.RequestedTheme;
                if (currentTheme == AppTheme.Light)
                {
                    if (App.Current.Resources.TryGetValue("Black_20percent", out var colorValLM))
                        currentColor = colorValLM as Color;
                }
                else
                {
                    if (App.Current.Resources.TryGetValue("Black_33percent", out var colorValDM))
                        currentColor = colorValDM as Color;
                }
                break;
            case CornellPadTheme.Dark:
                if (App.Current.Resources.TryGetValue("Black_33percent", out var colorvalueDM))
                    currentColor = colorvalueDM as Color;
                break;
            case CornellPadTheme.Light:
                if (App.Current.Resources.TryGetValue("Black_20percent", out var colorvalueLM))
                    currentColor = colorvalueLM as Color;
                break;
            default:
                if (App.Current.Resources.TryGetValue("Black", out var colorvalue))
                    currentColor = colorvalue as Color;
                break;
        }

        return currentColor;
    }
}
