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

using CommunityToolkit.Maui.Core;
using CornellPad.Services.Interfaces;

namespace CornellPad.ViewModels;

public partial class NoteViewModel : BaseViewModel
{
    /////////////////////////////////////////////////////////////////////////////////
    // Members & Member Mutators
    /////////////////////////////////////////////////////////////////////////////////
    private readonly IDataService _dataService;
    private readonly IPopupService _popupService;

    private NoteModel _noteModel;

    public string WhatILearnedToday
    {
        get => _noteModel?.Summary;
        set
        {
            if (WhatILearnedToday == value)
                return;

            _noteModel.Summary = value;
        }
    }

    public ObservableCollection<NoteEntry> NoteEntries
    {
        get => _noteModel?.NoteEntries;
        set
        {
            if (NoteEntries == value)
                return;

            _noteModel.NoteEntries = value;
        }
    }

    // Needed to allow the private member to remain private, but still allow setting.
    public void SetNoteModel(NoteModel noteModel)
    {
        if (noteModel == null || noteModel == _noteModel)
            return;

        _noteModel = noteModel;
    }

    /////////////////////////////////////////////////////////////////////////////////
    // Methods
    /////////////////////////////////////////////////////////////////////////////////
    public NoteViewModel(IDataService dataService, IPopupService popupService)
    {
        _dataService = dataService;
        _popupService = popupService;
    }

    [RelayCommand]
    void CreateNoteEntry()
    {
        var noteEntry = new NoteEntry()
        {
            Cue = string.Empty,
            Note = string.Empty,
            PageId = _noteModel.Id
        };

        _dataService.CreateNoteEntry(noteEntry);
        _noteModel.NoteEntries.Add(noteEntry);

        _noteModel.NumberOfEntries = _noteModel.NoteEntries.Count;

        // TODO: Next, we would need to scroll the collection view to the new entry and set the cursor in the Notes Editor field. (UPDATE: Currently, this isn't really possible.)
    }

    [RelayCommand]
    async Task DeleteNoteEntry(int noteEntryID)
    {
        var answer = await _popupService.ShowPopupAsync<DeletionWarningViewModel>(onPresenting: vm =>
        {
            vm.PopupMessage = "You are about to delete a note entry. This will delete the cues and notes that are associated with this note entry.\n\nThis can NOT be undone! Are you sure you want to do this?";
        });
        bool response = (bool)answer;

        if (!response)
            return;

        var result = _noteModel.NoteEntries.Where(x => x.Id == noteEntryID).ToList()[0];

        _noteModel.NoteEntries.Remove(result);
        _dataService.DeleteNoteEntry(result);

        _noteModel.NumberOfEntries = _noteModel.NoteEntries.Count;
    }
    
    public void SaveNoteData()
    {
        // Save all of the entries in the database
        foreach (var entry in _noteModel.NoteEntries)
        {
            _dataService.UpdateNoteEntry(entry);
        }
        // Save the NoteModel to the database
        _dataService.UpdateNotePage(_noteModel);
    }
}
