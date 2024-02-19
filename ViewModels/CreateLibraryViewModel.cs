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

namespace CornellPad.ViewModels;

public partial class CreateLibraryViewModel : BaseViewModel
{
    /////////////////////////////////////////////////////////////////////////////////
    // Members & Member Mutators
    /////////////////////////////////////////////////////////////////////////////////
    private IDataService _dataService;
    private readonly ILogger<CreateLibraryViewModel> _logger;

    private LibraryViewModel _libraryViewModel;
    private string _name;
    private string _description;

    public string Name
    {
        get => _name;
        set
        {
            if (value == _name)
                return;

            _name = value;
            OnPropertyChanged();

            if (string.IsNullOrWhiteSpace(value))
            {
                IsButtonEnabled = false;
                OnPropertyChanged(nameof(IsButtonEnabled));
                return;
            }

            if (!string.IsNullOrWhiteSpace(_name) && !string.IsNullOrWhiteSpace(_description))
            {
                IsButtonEnabled = true;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (value == _description)
                return;

            _description = value;
            OnPropertyChanged();

            if (string.IsNullOrWhiteSpace(value))
            {
                IsButtonEnabled = false;
                OnPropertyChanged(nameof(IsButtonEnabled));
                return;
            }

            if (!string.IsNullOrWhiteSpace(_name) && !string.IsNullOrWhiteSpace(_description))
            {
                IsButtonEnabled = true;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }
    }

    public bool IsButtonEnabled { get; set; }

    public bool MakeLibrarySelected { get; set; }

    /////////////////////////////////////////////////////////////////////////////////
    // Methods
    /////////////////////////////////////////////////////////////////////////////////
    public CreateLibraryViewModel(IDataService dataService, LibraryViewModel libraryViewModel, ILogger<CreateLibraryViewModel> logger)
    {
        if (logger != null)
            _logger = logger;
        else
        {
            #region debug_logger
#if DEBUG
            Debug.WriteLine("Null exception in CreateLibraryViewModel constructor: ILogger<CreateLibraryViewModel>");
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
            Debug.WriteLine("Null exception in CreateLibraryViewModel constructor: IDataService");
#elif !DEBUG
            _logger.LogCritical("Null exception in CreateLibraryViewModel constructor: IDataService");
#endif

            #endregion

            throw new ArgumentNullException(nameof(dataService));
        }

        _libraryViewModel = libraryViewModel;

        IsButtonEnabled = false;
        MakeLibrarySelected = false;
    }

    [RelayCommand]
    public async Task CreateLibrary()
    {
        var libraryModel = new LibraryModel()
        {
            Name = this.Name,
            Description = this.Description,
            CreatedDate = DateTime.Now
        };

        _dataService.CreateLibrary(libraryModel);

        Name = string.Empty;
        Description = string.Empty;

        if (MakeLibrarySelected)
        {
            var settings = _dataService.ReadSettings();
            settings.CurrentLibraryId = libraryModel.Id;
            _dataService.UpdateSettings(settings);

            await Shell.Current.GoToAsync(
                    $"///{nameof(LibraryView)}",
                    true,
                    new Dictionary<string, object>
                    {
                        {"NewLibraryModel", libraryModel}
                    });
        }
    }
}
