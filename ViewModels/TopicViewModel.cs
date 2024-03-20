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

public partial class TopicViewModel : BaseViewModel, IQueryAttributable
{
    /////////////////////////////////////////////////////////////////////////////////
    // Members & Member Mutators
    /////////////////////////////////////////////////////////////////////////////////
    private IDataService _dataService;
    private IPopupService _popupService;
    private readonly ILogger<TopicViewModel> _logger;

    private TopicModel _topicModel;

    /*
    public String Icon
    {
        get => _topicModel?.Icon;
        set
        {
            if (value != _topicModel?.Icon)
                _topicModel.Icon = value;
        }
    }
    // */
    public GlyphCollectionItem Glyph
    {
        /*
        get
        {
            return new GlyphCollectionItem()
            {
                GlyphFamily = _topicModel?.GlyphFamily,
                GlyphValue = _topicModel?.GlyphValue
            };
        }
        set
        {
            if (value.GlyphFamily != _topicModel?.GlyphFamily &&
                value.GlyphValue != _topicModel?.GlyphValue)
            {
                _topicModel.GlyphFamily = value.GlyphFamily;
                _topicModel.GlyphValue = value.GlyphValue;
            }
        }
        // */
        get => _topicModel?.Glyph;
        set
        {
            if (value != null &&
                _topicModel != null)
            {
                _topicModel.Glyph = value;
            }
        }
    }

    public string TopicName
    {
        get => _topicModel?.TopicName;
        set
        {
            if (value != _topicModel?.TopicName)
                _topicModel.TopicName = value;
        }
    }

    [ObservableProperty]
    private ObservableCollection<NoteModel> notePages;

    /////////////////////////////////////////////////////////////////////////////////
    // Methods
    /////////////////////////////////////////////////////////////////////////////////
    public TopicViewModel(IDataService dataService, IPopupService popupService, ILogger<TopicViewModel> logger)
    {
        if (logger != null)
            _logger = logger;
        else
        {
            #region debug_logger
#if DEBUG
            Debug.WriteLine("Null exception in TopicViewModel constructor: ILogger<TopicViewModel>");
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
            Debug.WriteLine("Null exception in TopicViewModel constructor: IDataService");
#elif !DEBUG
            _logger.LogCritical("Null exception in TopicViewModel constructor: IDataService");
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
            Debug.WriteLine("Null exception in TopicViewModel constructor: IPopupService");
#elif !DEBUG
            _logger.LogCritical("Null exception in TopicViewModel constructor: IPopupService");
#endif

            #endregion

            throw new ArgumentNullException(nameof(popupService));
        }

        Title = "Topic Summary";
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["TopicModel"] is TopicModel topicModel)
        {
            _topicModel = topicModel;
            NotePages = _topicModel.NotePages;

            //Icon = topicModel.Icon;
            //OnPropertyChanged(nameof(Icon));
            Glyph = topicModel.Glyph;
            OnPropertyChanged(nameof(Glyph));

            TopicName = topicModel.TopicName;
            OnPropertyChanged(nameof(TopicName));
        }
    }

    [RelayCommand]
    public async Task GoToNoteView(NoteModel noteModel)
    {
        var noteEntries = _dataService?.ReadAllNoteEntries(noteModel.Id);
        if (noteEntries.Count > 0)
            noteModel.NoteEntries = new ObservableCollection<NoteEntry>(noteEntries);
        else
            noteModel.NoteEntries = new ObservableCollection<NoteEntry>();

        await Shell.Current.GoToAsync(
            $"{nameof(NoteView)}",
            true,
            new Dictionary<string, object>
            {
                {"NoteModel", noteModel}
            });
    }

    [RelayCommand]
    public async Task CreateNoteAsync()
    {
        var result = await _popupService.ShowPopupAsync<CreateNoteViewModel>();

        if (result is null)
            return;

        NoteModel noteModel = new()
        {
            TopicId = _topicModel.Id,
            Title = result as string,
            CreationDate = DateTime.Now
        };

        _topicModel.NotePages?.Add(noteModel);
        _dataService.CreateNotePage(noteModel);

        _topicModel.NumberOfNotes = _topicModel.NotePages.Count;
    }

    [RelayCommand]
    public async Task DeleteNote(NoteModel note)
    {
        var result = await _popupService.ShowPopupAsync<DeletionWarningViewModel>(onPresenting: vm =>
        {
            vm.PopupMessage = "You are about to delete a note. This will also delete all note entries associated with this note.\n\nThis can NOT be undone! Are you sure you want to do this?";
        });

        bool canContinue = (bool)result;
        if (canContinue == false)
            return;

        _topicModel.NotePages?.Remove(note);
        _dataService.DeleteNotePage(note);

        _topicModel.NumberOfNotes = _topicModel.NotePages.Count;
    }

}
