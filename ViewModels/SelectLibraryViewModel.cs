﻿/*******************************************************************
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

using CornellPad.Services.Interfaces;
using MetroLog;
using Microsoft.Extensions.Logging;

namespace CornellPad.ViewModels;

public partial class SelectLibraryViewModel : BaseViewModel
{
    /////////////////////////////////////////////////////////////////////////////////
    // Members & Member Mutators
    /////////////////////////////////////////////////////////////////////////////////
    private IDataService _dataService;
    private readonly ILogger<SelectLibraryViewModel> _logger;

    public ObservableCollection<LibraryModel> Libraries { get; set; }

    [ObservableProperty]
    bool isLibraryModelSelected;

    [ObservableProperty]
    LibraryModel selectedLibraryModel;

    [ObservableProperty]
    string loadLibraryButtonText;

    /////////////////////////////////////////////////////////////////////////////////
    // Methods
    /////////////////////////////////////////////////////////////////////////////////
    public SelectLibraryViewModel(IDataService dataService, LibraryViewModel libraryViewModel, ILogger<SelectLibraryViewModel> logger)
    {
        if (logger != null)
            _logger = logger;
        else
        {
            #region debug_logger

#if DEBUG
            Debug.WriteLine("Null exception in SelectLibraryViewModel constructor: ILogger<SelectLibraryViewModel>");
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
            Debug.WriteLine("Null exception in SelectLibraryViewModel constructor: IDataService");
#elif !DEBUG
            _logger.LogCritical("Null exception in SelectLibraryViewModel constructor: IDataService");
#endif

            #endregion

            throw new ArgumentNullException(nameof(dataService));
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

        LoadLibraryButtonText = $"Load {SelectedLibraryModel.Name}";
        IsLibraryModelSelected = true;
    }

    [RelayCommand]
    public async Task LoadSelectedLibrary()
    {
        var settings = _dataService.ReadSettings();
        settings.CurrentLibraryId = SelectedLibraryModel.Id;
        _dataService.UpdateSettings(settings);

        IsLibraryModelSelected = false;

        await Shell.Current.GoToAsync(
            $"///{nameof(LibraryView)}",
            true,
            new Dictionary<string, object>
            {
                {"NewLibraryModel", SelectedLibraryModel}
            });
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
