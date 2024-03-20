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

using CommunityToolkit.Maui;
using CornellPad.Popups;
using CornellPad.Services;
using CornellPad.Services.Interfaces;
using MetroLog.MicrosoftExtensions;
using Microsoft.Extensions.Logging;

namespace CornellPad;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            // Initialize the .NET MAUI Community Toolkit by adding the below line of code
            .UseMauiCommunityToolkit()
            // After initializing the .NET MAUI Community Toolkit, optionally add additional fonts
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("CrimsonPro-Italic.ttf", "CrimsonPro_Italic");
                fonts.AddFont("CrimsonPro-Regular.ttf", "CrimsonPro_Regular");
                fonts.AddFont("CrimsonPro-SemiBold.ttf", "CrimsonPro_600");
                fonts.AddFont("fabrands.ttf", "FA_Brands");
                fonts.AddFont("faregular.ttf", "FA_Regular");
                fonts.AddFont("fasolid.ttf", "FA_Solid");
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("SourceSans3-Black.ttf", "SourceSansPro_Black");
                fonts.AddFont("SourceSans3-Bold.ttf", "SourceSansPro_Bold");
                fonts.AddFont("SourceSans3-Regular.ttf", "SourceSansPro_Regular");
            });
        /////////////////////////////////////
        // Debugging Service Registrations
        /////////////////////////////////////
#if DEBUG
        // For DEBUG, we'll just use the MAUI default...
        builder.Logging.AddDebug();
#endif
        // ...but we'll include MetroLog no matter what.
        builder.Logging
            .SetMinimumLevel(LogLevel.Warning)
            .AddStreamingFileLogger(options =>
            {
                options.RetainDays = 2;
                options.FolderPath = Path.Combine(
                    FileSystem.CacheDirectory,
                    "CornellPadLogs");
            });


        /////////////////////////////////////
        // Dependency Service Registrations
        /////////////////////////////////////
        builder.Services.AddSingleton<IDataService, SQLiteDataService>();

        // Models-Views-ViewModels
        builder.Services.AddTransient<CreateLibraryView>();
        builder.Services.AddTransient<CreateLibraryViewModel>();

        builder.Services.AddTransient<DeleteLibraryView>();
        builder.Services.AddTransient<DeleteLibraryViewModel>();

        builder.Services.AddTransient<SelectLibraryView>();
        builder.Services.AddTransient<SelectLibraryViewModel>();

        builder.Services.AddTransient<SettingsView>();
        builder.Services.AddTransient<SettingsViewModel>();

        builder.Services.AddTransient<LibraryView>(); // was AddSingleton
        builder.Services.AddTransient<LibraryModel>(); // was AddSingleton
        builder.Services.AddTransient<LibraryViewModel>(); // was AddSingleton


        /*
        builder.Services.AddTransient<CreateTopicView>();
        builder.Services.AddTransient<CreateTopicViewModel>();
        // */


        builder.Services.AddTransient<TopicView>();
        builder.Services.AddTransient<TopicModel>();
        builder.Services.AddTransient<TopicViewModel>();



        builder.Services.AddTransient<CreateNoteView>();
        builder.Services.AddTransient<CreateNoteViewModel>();

        builder.Services.AddTransient<NoteView>();
        builder.Services.AddTransient<NoteModel>();
        builder.Services.AddTransient<NoteViewModel>();

        // Dialogs-ViewModels
        builder.Services.AddTransientPopup<CreateTopicPopup, CreateTopicViewModel>();

        builder.Services.AddTransientPopup<CreateNotePopup, CreateNoteViewModel>();

        builder.Services.AddTransientPopup<DeletionWarningPopup, DeletionWarningViewModel>();

        builder.Services.AddTransientPopup<SettingsInfoPopup, SettingsInfoViewModel>();

        builder.Services.AddTransientPopup<DBSettingsWarningPopup, DBSettingsWarningViewModel>();

        builder.Services.AddTransientPopup<ErrorWarningPopup, ErrorWarningViewModel>();



        return builder.Build();
    }
}