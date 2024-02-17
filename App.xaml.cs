/*******************************************************************
 Copyright 2024 Digital Brain Lice

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

using CornellPad.Services.Interfaces;
using MetroLog;
using Microsoft.Extensions.Logging;

namespace CornellPad;

// TODO: Change the License headers in the source files; Digital Brain Lice isn't the owner of the code base, CornellPad is.
// TODO: A 'print-to-PDF' feature would be a huge boon to users; QuestPDF is a great choice here.

/// <summary>
/// Enumerates the four valid theme settings for CornellPad.
/// <list type="table">
///     <item>
///         <term>Auto</term>
///         <description>Sets the application to use the system theme.</description>
///     </item>
///     <item>
///         <term>Dark</term>
///         <description>Sets the application to use the Dark theme, regardless of the user's system settings.</description>
///     </item>
///     <item>
///         <term>Light</term>
///         <description>Sets the application to use the Light theme, regardless of the user's system settings.</description>
///     </item>
///     <item>
///         <term>VI</term>
///         <description>Sets the application to use the system theme, but only provides color, typography, and UI element size
///         values for the <b>V</b>isually <b>I</b>mpared theme. Thus, the UI will only be able to show our VI theme values.
///         </description>
///     </item>
/// </list>
/// </summary>
public enum CornellPadTheme
{
    Auto,
    Dark,
    Light,
    VI
}

public partial class App : Application
{
    private readonly IDataService _dataService;
    private readonly ILogger<App> _logger;

    private ResourceDictionary _styleThemeDictionary;

    private CornellPadTheme _currentTheme;

    public App(IDataService dataService, ILogger<App> logger)
    {
        InitializeComponent();

        if (logger != null)
            _logger = logger;
        else
        {
            #region debug_logger
#if DEBUG
            Debug.WriteLine("Null exception in App constructor: ILogger<App>");
#endif
            #endregion

            throw new ArgumentNullException(nameof(logger));
        }

        if (dataService != null)
            _dataService = dataService;
        else
        {
            #region debug_dataservice

#if DEBUG
            Debug.WriteLine("Null exception in App constructor: IDataService");
#elif !DEBUG
            _logger.LogCritical("Null exception in App constructor: IDataService");
#endif

            #endregion

            throw new ArgumentNullException(nameof(dataService));
        }

        // New mapper for the Entry control
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
        {
            /* WINDOWS Underline Removal
             * 
             * To remove the border and focus underline, the App.xaml file for
             * the Windows Platform must be edited. See that file for the changes
             * that need to be made.
             */
#if ANDROID

            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);

#elif IOS || MACCATALYST

            handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;

#endif
        });

        // New mapper for the Editor control
        Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping(nameof(Editor), (handler, view) =>
        {
            /* WINDOWS Underline Removal
             * 
             * To remove the border and focus underline, the App.xaml file for
             * the Windows Platform must be edited. See that file for the changes
             * that need to be made.
             */
#if ANDROID

            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);

#elif IOS || MACCATALYST

            handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;

#endif
        });

        // New mapper for the SearchBar control
        Microsoft.Maui.Handlers.SearchBarHandler.Mapper.AppendToMapping(nameof(SearchBar), (handler, view) =>
        {
            /* WINDOWS Underline Removal
             * 
             * To remove the border and focus underline, the App.xaml file for
             * the Windows Platform must be edited. See that file for the changes
             * that need to be made.
             */
#if ANDROID

            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);

#elif IOS || MACCATALYST

            handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;

#endif
        });


        /***********************
         * Theme Handling Code
         ***********************/
        CornellPadTheme cornellPadTheme = _dataService.ReadSettings().CurrentTheme;
        SetCurrentTheme(cornellPadTheme);


        MainPage = new AppShell();
    }

    /// <summary>
    /// Gets the enum value for the currently set theme.
    /// </summary>
    /// <returns>Enum for the currently set theme.</returns>
    public CornellPadTheme GetCurrentTheme()
    {
        return _currentTheme;
    }

    /// <summary>
    /// Sets the currently active theme, based on the value passed in through
    /// the method's <paramref name="newTheme"/> argument.Returns the enum
    /// value for the replaced theme.
    /// </summary>
    /// <param name="newTheme">Enum value for the new theme to be set as current.</param>
    /// <returns>Enum value for the old theme which was replaced.</returns>
    public CornellPadTheme SetCurrentTheme(CornellPadTheme newTheme)
    {
        CornellPadTheme oldTheme = _currentTheme;

        _currentTheme = newTheme;

        SetThemeDictionary(_currentTheme);

        return oldTheme;
    }

    /// <summary>
    /// Uses the <paramref name="theme"/> parameter to determine which Styles resource
    /// dictionary to load and merge. It will clear the previously loaded Styles
    /// resource dictionary that is merged, if any, and then load and merge the requested
    /// theme's resource dictionary.
    /// </summary>
    /// <param name="theme">An enum indicating what theme should be loaded.</param>
    private void SetThemeDictionary(CornellPadTheme theme)
    {
        if (_styleThemeDictionary != null)
        {
            Resources.MergedDictionaries.Remove(_styleThemeDictionary);
        }

        ResourceDictionary newTheme;

        switch (theme)
        {
            case CornellPadTheme.Dark:
                Application.Current.UserAppTheme = AppTheme.Dark;
                newTheme = new Resources.Styles.Styles();
                break;
            case CornellPadTheme.Light:
                Application.Current.UserAppTheme = AppTheme.Light;
                newTheme = new Resources.Styles.Styles();
                break;
            case CornellPadTheme.VI:
                Application.Current.UserAppTheme = AppTheme.Unspecified;
                newTheme = new Resources.Styles.StylesVI();
                break;
            default:
                Application.Current.UserAppTheme = AppTheme.Unspecified;
                newTheme = new Resources.Styles.Styles();
                break;
        }

        Resources.MergedDictionaries.Add(newTheme);
        _styleThemeDictionary = newTheme;
    }

    protected override void OnSleep()
    {
        var currentPage = (MainPage as AppShell).CurrentPage;

        if (currentPage != null && currentPage.GetType() == typeof(NoteView))
        {
            var pageBindingContext = currentPage.BindingContext as NoteViewModel;

            if (pageBindingContext != null)
            {
                pageBindingContext.SaveNoteData();
            }
        }

        base.OnSleep();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);



        #region Desktop_MinSize
#if WINDOWS || MACCATALYST

        var settings = _dataService.ReadSettings();

        double minHeight = 819; // Force a minimum of
        double minWidth = 378;  // 19.5x9 aspect ratio
        
        window.Destroying += (sender, eventArgs) =>
        {
            var currentWindow = sender as Window;

            // We have to get fresh data; this is called at end-of-life.
            var currentSettings = _dataService.ReadSettings();

            if (currentSettings is null || currentWindow is null)
                return;

            currentSettings.WindowPositionX = currentWindow.X;
            currentSettings.WindowPositionY = currentWindow.Y;

            currentSettings.WindowSizeX = currentWindow.Width;
            currentSettings.WindowSizeY = currentWindow.Height;

            _dataService.UpdateSettings(currentSettings);
        };

#endif
        #endregion



        #region Desktop_WindowsOnlySizePosition
#if WINDOWS

        window.MinimumHeight = minHeight;
        window.MinimumWidth = minWidth;

        if (settings?.WindowPositionX != -1 && settings?.WindowPositionY != -1)
        {
            window.X = settings.WindowPositionX;
            window.Y = settings.WindowPositionY;
        }

        if (settings?.WindowSizeX != -1 && settings?.WindowSizeY != -1)
        {
            window.Height = settings.WindowSizeY;
            window.Width = settings.WindowSizeX;
        }

#endif
        #endregion



        #region Desktop_MacCatalystOnlySize
#if MACCATALYST

        if (settings is null || settings?.WindowSizeX == -1 || settings?.WindowSizeY == -1)
        {
            window.MinimumHeight = minHeight;
            window.MinimumWidth = minWidth;

            return window;
        }

        // MacCatalyst window-size workaround; force absolute size...
        window.MinimumHeight = settings.WindowSizeY;
        window.MaximumHeight = settings.WindowSizeY;
        window.MinimumWidth = settings.WindowSizeX;
        window.MaximumWidth = settings.WindowSizeX;

        // ...then, schedule the Min/Max value's reset.
        Dispatcher.Dispatch( () =>
        {
            window.MinimumHeight = minHeight;
            window.MinimumWidth = minWidth;
            window.MaximumHeight = double.PositiveInfinity;
            window.MaximumWidth = double.PositiveInfinity;
        });

#endif
        #endregion



        return window;
    }
}