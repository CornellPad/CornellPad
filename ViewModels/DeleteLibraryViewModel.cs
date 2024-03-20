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

public partial class DeleteLibraryViewModel : BaseViewModel
{
    /////////////////////////////////////////////////////////////////////////////////
    // Members & Member Mutators
    /////////////////////////////////////////////////////////////////////////////////
    private IDataService _dataService;
    private IPopupService _popupService;
    private readonly ILogger<DeleteLibraryViewModel> _logger;

    //public ObservableCollection<LibraryModel> Libraries { get; set; }

    [ObservableProperty]
    ObservableCollection<LibraryModel> libraries;

    [ObservableProperty]
    bool isLibraryModelSelected;

    [ObservableProperty]
    LibraryModel selectedLibraryModel;

    [ObservableProperty]
    string deleteLibraryButtonText;

    /////////////////////////////////////////////////////////////////////////////////
    // Methods
    /////////////////////////////////////////////////////////////////////////////////
    public DeleteLibraryViewModel(IDataService dataService, IPopupService popupService, ILogger<DeleteLibraryViewModel> logger)
    {
        if (logger != null)
            _logger = logger;
        else
        {
            #region debug_logger
#if DEBUG
            Debug.WriteLine("Null exception in DeleteLibraryViewModel constructor: ILogger<DeleteLibraryViewModel>");
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
            Debug.WriteLine("Null exception in DeleteLibraryViewModel constructor: IDataService");
#elif !DEBUG
            _logger.LogCritical("Null exception in DeleteLibraryViewModel constructor: IDataService");
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
            Debug.WriteLine("Null exception in DeleteLibraryViewModel constructor: IPopupService");
#elif !DEBUG
            _logger.LogCritical("Null exception in DeleteLibraryViewModel constructor: IPopupService");
#endif

            #endregion

            throw new ArgumentNullException(nameof(popupService));
        }

        SelectedLibraryModel = _dataService.ReadLibraries().Where(lib => lib.Id == _dataService.ReadSettings().CurrentLibraryId).ToList()[0];

        IsLibraryModelSelected = false;

        Libraries = new ObservableCollection<LibraryModel>(_dataService.ReadLibraries());
    }

    [RelayCommand]
    public void SetSelectedLibrary(LibraryModel library)
    {
        LibraryModelComparer comp = new();
        var currentLibrary = _dataService.ReadLibraries().Where(lib => lib.Id == _dataService.ReadSettings().CurrentLibraryId).ToList()[0];
        if (comp.Equals(library, currentLibrary))
        {
            SelectedLibraryModel = currentLibrary;
            IsLibraryModelSelected = false;
            return;
        }

        SelectedLibraryModel = library;

        DeleteLibraryButtonText = $"Delete {SelectedLibraryModel.Name}";
        IsLibraryModelSelected = true;
    }

    [RelayCommand]
    public async Task DeleteSelectedLibraryAsync()
    {
        var result = await _popupService.ShowPopupAsync<DeletionWarningViewModel>(onPresenting: vm =>
        {
            vm.PopupMessage = "You are about to delete a library. This will also delete all topics, notes, and note entries associated with this library.\n\nThis can NOT be undone! Are you sure you want to do this?";
        });

        bool canContinue = (bool)result;
        if (canContinue == false)
            return;

        Libraries.Remove( SelectedLibraryModel );
        _dataService.DeleteLibrary( SelectedLibraryModel );

        SelectedLibraryModel = _dataService.ReadLibraries().Where(lib => lib.Id == _dataService.ReadSettings().CurrentLibraryId).ToList()[0];
        IsLibraryModelSelected = false;
    }

    [RelayCommand]
    public void PageAppearing()
    {
        var dataServiceLibraries = _dataService.ReadLibraries();
        Libraries.Clear();

        foreach (var libraryModel in dataServiceLibraries)
        {
            //if (!Libraries.Contains(libraryModel, new LibraryModelComparer()))
                Libraries.Add(libraryModel);
        }

        SelectedLibraryModel = _dataService.ReadLibraries().Where(lib => lib.Id == _dataService.ReadSettings().CurrentLibraryId).ToList()[0];
        IsLibraryModelSelected = false;
    }
}
