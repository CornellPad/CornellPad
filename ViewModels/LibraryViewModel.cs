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

using CommunityToolkit.Maui.Core;
using CornellPad.Services.Interfaces;
using MetroLog;
using Microsoft.Extensions.Logging;

namespace CornellPad.ViewModels;

public partial class LibraryViewModel : BaseViewModel, IQueryAttributable
{
    /////////////////////////////////////////////////////////////////////////////////
    // Members & Member Mutators
    /////////////////////////////////////////////////////////////////////////////////
    private LibraryModel _libraryModel;

    private readonly IDataService _dataService;

    private readonly IPopupService _popupService;
    private readonly ILogger<LibraryViewModel> _logger;

    /// <summary>
    /// 'Back Button' navs result in duplicate TopicModel creates without this bool.
    /// </summary>
    public bool CanUpdateEntries = false;

    [ObservableProperty]
    private ObservableCollection<TopicModel> topicEntries;

    /////////////////////////////////////////////////////////////////////////////////
    // Methods
    /////////////////////////////////////////////////////////////////////////////////
    public LibraryViewModel(IDataService dataService, IPopupService popupService, ILogger<LibraryViewModel> logger)
    {
        if (logger != null)
            _logger = logger;
        else
        {
            #region debug_logger
#if DEBUG
            Debug.WriteLine("Null exception in NoteViewModel constructor: ILogger<NoteViewModel>");
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
            Debug.WriteLine("Null exception in LibraryViewModel constructor: IDataService");
#elif !DEBUG
            _logger.LogCritical("Null exception in LibraryViewModel constructor: IDataService. Cannot continue.");
#endif
            #endregion

            throw new ArgumentNullException(nameof(dataService));
        }

        if (popupService != null)
            _popupService = popupService;
        else
        {
            #region debug_popupservice
#if DEBUG
            Debug.WriteLine("Null exception in LibraryViewModel constructor: IPopupService");
#elif !DEBUG
            _logger.LogCritical("Null exception in LibraryViewModel constructor: IPopupService. Cannot continue.");
#endif
            #endregion

            throw new ArgumentNullException(nameof(popupService));
        }

        int currentLibraryId = _dataService.ReadSettings().CurrentLibraryId;
        try
        {
            _libraryModel = _dataService.ReadLibraries().Where(lib => lib.Id == currentLibraryId).ToList()[0];
        }
        catch (ArgumentOutOfRangeException)
        {
            // Under normal conditions, we should never be ABLE to reach this error code.
            var libraryList = _dataService.ReadLibraries();

            if (libraryList is null)
            {
                _logger.LogCritical("Null exception in LibraryViewModel constructor: {var}", nameof(libraryList));
                return;
            }

            if (libraryList.Count > 1)
            {
                // We have more than one library, but neither matches the settings 'currentLibraryId'.
                _logger.LogWarning("No found library ID values match ID for currently selected library. Setting current library to first found libraries ID value.");
            }


            _libraryModel = libraryList.FirstOrDefault();

            // This is a pretty heavy-handed way of dealing with this error. Is there a better way?
            var settingsModel = _dataService.ReadSettings();
            settingsModel.CurrentLibraryId = _libraryModel.Id;
            _dataService.UpdateSettings(settingsModel);
        }

        TopicEntries = _libraryModel.TopicEntries;

        Title = $"Library: {_libraryModel.Name}";

        foreach (var topic in _libraryModel.TopicEntries)
        {
            int count = _dataService.ReadAllNotePages(topic.Id).Count;
            topic.NumberOfNotes = count;
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (CanUpdateEntries && query.ContainsKey("NewTopicModel"))
        {
            TopicModel topicModel = query["NewTopicModel"] as TopicModel;
            topicModel.LibraryId = _libraryModel.Id;

            if (_dataService.CreateTopic(topicModel) == 1)
                TopicEntries.Add(topicModel);

            CanUpdateEntries = false;
        }
        else if (query.ContainsKey("NewLibraryModel"))
        {
            if (query["NewLibraryModel"] is LibraryModel libraryModel)
            {
                if (libraryModel.TopicEntries != null)
                {
                    foreach (var topic in libraryModel.TopicEntries)
                    {
                        topic.NumberOfNotes = _dataService.ReadAllNotePages(topic.Id).Count;
                    }
                }
                else
                    libraryModel.TopicEntries = new();

                _libraryModel = libraryModel;
                TopicEntries = _libraryModel.TopicEntries;
                Title = $"Library: {libraryModel.Name}";
            }
        }
    }

    [RelayCommand]
    async Task GoToTopicViewAsync(TopicModel topicModel)
    {
        var list = _dataService?.ReadAllNotePages(topicModel.Id);
        if (list.Count > 0)
            topicModel.NotePages = new ObservableCollection<NoteModel>(list);
        else
            topicModel.NotePages = new ObservableCollection<NoteModel>();

        await Shell.Current.GoToAsync(
            $"{nameof(TopicView)}",
            true,
            new Dictionary<string, object>
            {
                {"TopicModel", topicModel}
            });
    }

    [RelayCommand]
    async Task GoToCreateTopicViewAsync()
    {
        CanUpdateEntries = true;

        await Shell.Current.GoToAsync($"{nameof(CreateTopicView)}", true);
    }

    [RelayCommand]
    async Task ShowCreateTopicDialogAsync()
    {
        var results = await _popupService.ShowPopupAsync<CreateTopicViewModel>() as TopicModel;

        if (results is null || 
            string.IsNullOrEmpty(results.TopicName) ||
            string.IsNullOrEmpty(results.GlyphValue) ||
            string.IsNullOrEmpty(results.GlyphFamily))
        {
            await _popupService.ShowPopupAsync<ErrorWarningViewModel>(onPresenting: vm =>
            {
                vm.Title = "Error: Empty Values";
                vm.PopupMessage = "A topic could not be created, because one or more required values were missing.\n\nYou must input a topic name and select a topic glyph to create a topic.";
            });
            return;
        }

        results.LibraryId = _libraryModel.Id;

        _dataService.CreateTopic(results);
        TopicEntries.Add(results);
    }

    [RelayCommand]
    async Task DeleteTopic(TopicModel topicModel)
    {
        var result = await _popupService.ShowPopupAsync<DeletionWarningViewModel>(onPresenting: vm =>
        {
            vm.PopupMessage = "You are about to delete a topic. This will also delete all notes and note entries associated with this topic.\n\nThis can NOT be undone! Are you sure you want to do this?";
        });

        bool canConinue = (bool)result;
        if (canConinue == false)
            return;

        TopicEntries.Remove(topicModel);
        _dataService?.DeleteTopic(topicModel);
    }
}
