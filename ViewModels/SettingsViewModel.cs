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
using CommunityToolkit.Maui.Storage;
using CornellPad.Services.Interfaces;
using MetroLog;
using Microsoft.Extensions.Logging;

namespace CornellPad.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    /////////////////////////////////////////////////////////////////////////////////
    // Members & Member Mutators
    /////////////////////////////////////////////////////////////////////////////////
    private IDataService _dataService;
    private IPopupService _popupService;
    private readonly ILogger<SettingsViewModel> _logger;

    [ObservableProperty]
    bool isAdvancedDBSettingsExpanded;

    [ObservableProperty]
    bool isAutoThemeEnabled;

    [ObservableProperty]
    bool isDarkThemeEnabled;

    [ObservableProperty]
    bool isLightThemeEnabled;

    [ObservableProperty]
    bool isVIThemeEnabled;

    [ObservableProperty]
    int cacheSize;

    [ObservableProperty]
    int pageSizeIndex;

    [ObservableProperty]
    int lockingModeIndex;

    [ObservableProperty]
    int tempStoreIndex;

    [ObservableProperty]
    int autoVacuumIndex;

    /////////////////////////////////////////////////////////////////////////////////
    // Methods
    /////////////////////////////////////////////////////////////////////////////////

    public SettingsViewModel(IDataService dataService, IPopupService popupService, ILogger<SettingsViewModel> logger)
    {
        if (logger != null)
            _logger = logger;
        else
        {
            #region debug_logger
#if DEBUG
            Debug.WriteLine("Null exception in SettingsViewModel constructor: ILogger<SettingsViewModel>");
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
            Debug.WriteLine("Null exception in SettingsViewModel constructor: IDataService");
#elif !DEBUG
            _logger.LogCritical("Null exception in SettingsViewModel constructor: IDataService");
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
            Debug.WriteLine("Null exception in SettingsViewModel constructor: IPopupService");
#elif !DEBUG
            _logger.LogCritical("Null exception in SettingsViewModel constructor: IPopupService");
#endif

            #endregion

            throw new ArgumentNullException(nameof(popupService));
        }

        // These shouldn't be -1 if set correctly from the DBMS.
        PageSizeIndex = -1;
        LockingModeIndex = -1;
        TempStoreIndex = -1;
        AutoVacuumIndex = -1;

        CacheSize = _dataService.GetCacheSize();
        PageSizeIndex = PageSizeToIndex( _dataService.GetPageSize() );
        LockingModeIndex = LockingModeToIndex( _dataService.GetLockingMode() );
        TempStoreIndex = TempStoreToIndex( _dataService.GetTempStore() );
        AutoVacuumIndex = _dataService.GetAutoVacuum();

        IsAdvancedDBSettingsExpanded = false;

        IsAutoThemeEnabled = false;
        IsDarkThemeEnabled = false;
        IsLightThemeEnabled = false;
        IsVIThemeEnabled = false;

        switch(_dataService.ReadSettings().CurrentTheme)
        {
            case CornellPadTheme.Dark:
                IsDarkThemeEnabled = true;
                break;
            case CornellPadTheme.Light:
                IsLightThemeEnabled = true;
                break;
            case CornellPadTheme.VI:
                IsVIThemeEnabled = true;
                break;
            default:
                IsAutoThemeEnabled = true;
                break;
        }
    }

    [RelayCommand]
    public async Task ChangeCurrentAppTheme(string theme)
    {
        await _popupService.ShowPopupAsync<ErrorWarningViewModel>(onPresenting: vm =>
        {
            vm.Title = "Theme Change";
            vm.PopupMessage = "We apologize for the inconvenience, but due to the app's code structure, your changes cannot be displayed properly without a restart.\n\nYour changes will be visible the next time the app is started.";
        });

        var model = _dataService.ReadSettings();

        IsAutoThemeEnabled = false;
        IsDarkThemeEnabled = false;
        IsLightThemeEnabled = false;
        IsVIThemeEnabled = false;

        switch (theme)
        {
            case "Dark":
                model.CurrentTheme = CornellPadTheme.Dark;
                IsDarkThemeEnabled = true;
                break;
            case "Light":
                model.CurrentTheme = CornellPadTheme.Light;
                IsLightThemeEnabled = true;
                break;
            case "VI":
                model.CurrentTheme = CornellPadTheme.VI;
                IsVIThemeEnabled = true;
                break;
            default:
                model.CurrentTheme = CornellPadTheme.Auto;
                IsAutoThemeEnabled = true;
                break;
        }

        _dataService.UpdateSettings(model);
    }

    /*-------------------
     * Popup Commands
     --------------------*/
    [RelayCommand]
    public async Task DisplayCacheSizePopup()
    {
        await _popupService.ShowPopupAsync<SettingsInfoViewModel>(onPresenting: vm =>
        {
            vm.Title = "Cache Size";
            vm.PopupMessage = "SQLite's cache size impacts performance and memory usage. A larger cache improves speed by reducing storage I/O, but uses more memory.\n\n" +
                              "Balancing cache size is key for optimal performance without wasting resources.\n\n" +
                              "Cache size and page size are related, and should be adjusted together for best results.";
        });
    }

    [RelayCommand]
    public async Task DisplayPageSizePopup()
    {
        await _popupService.ShowPopupAsync<SettingsInfoViewModel>(onPresenting: vm =>
        {
            vm.Title = "Page Size";
            vm.PopupMessage = "In SQLite, pages are fixed-size blocks used to store data. During read/write actions, only relevant pages are affected.\n\n" +
                              "Matching page size to storage media \"Cluster Size\" can increase performance and decrease the database file's storage footprint.\n\n" +
                              "Changing this triggers a full VACUUM. Also, see \"Cache Size\" info.";
        });
    }

    [RelayCommand]
    public async Task DisplayLockingModePopup()
    {
        await _popupService.ShowPopupAsync<SettingsInfoViewModel>(onPresenting: vm =>
        {
            vm.Title = "Locking Mode";
            vm.PopupMessage = "SQLite's Locking Mode manages DB file locks.\n\n" +
                              "NORMAL mode acquires/releases locks per transaction.\n\n" +
                              "EXCLUSIVE mode acquires locks normally but doesn't release them post-transaction, retaining access control. This leads to fewer storage I/O actions, which can result in better performance.";
        });
    }

    [RelayCommand]
    public async Task DisplayTempStorePopup()
    {
        await _popupService.ShowPopupAsync<SettingsInfoViewModel>(onPresenting: vm =>
        {
            vm.Title = "Temp Store";
            vm.PopupMessage = "SQLite's temp store in DEFAULT uses system storage for temp files, using more storage space. In this mode, these temp files are NOT removed on app exit.\n\n" +
                              "MEMORY mode stores temp files in RAM, enhancing speed but using more memory. This IS cleaned up on app exit.\n\n" +
                              "Affects temp files for actions like sorting.";
        });
    }

    [RelayCommand]
    public async Task DisplayAutoVacuumPopup()
    {
        await _popupService.ShowPopupAsync<SettingsInfoViewModel>(onPresenting: vm =>
        {
            vm.Title = "Auto Vacuum";
            vm.PopupMessage = "SQLite's 'Auto Vacuum' compacts the DB file by reclaiming space from deleted data, but can slow down delete operations and increase storage I/O operations.\n\n" +
                              "It's beneficial for space management, but consider it's performance impact.\n\n" +
                              "Changing this will cause a full VACUUM to occur.";
        });
    }

    [RelayCommand]
    public async Task DisplayManualVacuumPopup()
    {
        await _popupService.ShowPopupAsync<SettingsInfoViewModel>(onPresenting: vm =>
        {
            vm.Title = "Manual Vacuum";
            vm.PopupMessage = "SQLite's full VACUUM rebuilds the database to reclaim space and reduce fragmentation.\n\n" +
                              "It removes traces of deleted data, which can be a security measure. However, it's time-consuming and will temporarily increase the database size.\n\n" +
                              "Sporatic manual VACUUMs OR 'Auto Vacuum' is advised, if vacuuming is used at all.";
        });
    }

    [RelayCommand]
    public async Task DisplayDBSettingsWarningPopup()
    {
        if (!IsAdvancedDBSettingsExpanded)
            return;

        var model = _dataService.ReadSettings();
        if (model is null || model.IsHidingDBSettingsWarning)
            return;

        var result = await _popupService.ShowPopupAsync<DBSettingsWarningViewModel>(onPresenting: vm =>
        {
            vm.PopupMessage = "These settings are for advanced users only.\n\n" +
                              "Inputting the wrong settings can result in degraded performance, and in rare instances, could even lead to data corruption.\n\n" +
                              "Research should be done for each setting at the links provided in the \"Database Management Links\" expander.";

            // This is fine, since the popup will only display if the user hasn't checked the box to disable the warning.
            vm.DisableDBSettingsWarning = false;
        });

        bool disableDBSettingsWarning = (bool)result;
        if (disableDBSettingsWarning)
        {
            model.IsHidingDBSettingsWarning = true;
            _dataService.UpdateSettings(model);
        }
    }

    /*---------------------
     * UI Control Commands
     ----------------------*/
    [RelayCommand]
    public async Task BackupDatabase()
    {
        FolderPickerResult result;

        result = await FolderPicker.PickAsync(default);

        if (result.IsSuccessful)
        {
            string name, path;

            result.Folder.Deconstruct(out path, out name);

            DateTime now = DateTime.Now;

            string dateTime = now.Month.ToString() + now.Day.ToString() + now.Year.ToString() + "_" + now.Hour.ToString() + now.Minute.ToString() + now.Second.ToString() + now.Nanosecond.ToString();

            string fileName = _dataService.GetDatabaseName() + "_" + dateTime + _dataService.GetDatabaseExtention();
            string fullPath = Path.Combine(path, fileName);

            // This is necessary; otherwise, SQLite-Net throws an exception.
            var backupCon = new SQLiteConnection(fullPath);
            backupCon.Close();

            if (!File.Exists(fullPath))
                return;

            try
            {
                _dataService.BackupDB(fullPath);
            }
            catch (Exception ex)
            {
                #region BackupDB_Exception
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                #endregion

                // TODO: We need to handle this exception. What can we do at this point? Do we have proper permissions? Does the location actually exist?
            }
        }
        else
        {
            // TODO: show the user an error popup, informing them that the location selection operation was unsuccessful.
            #region FolderPickerResult_Exception
#if DEBUG
            Debug.WriteLine(result.Exception.Message);
#endif
            #endregion

            Console.WriteLine(result.Exception.Message);
            //Logger.Error(result.Exception.Message, result.Exception.StackTrace); // We NEED loggin in the app...this is stupid!
            await _popupService.ShowPopupAsync<ErrorWarningViewModel>(onPresenting: vm =>
            {
                vm.Title = "Error: Invalid Choice";
                vm.PopupMessage = "We apologize for the error, but it appears that the OS couldn't resolve your folder selection.\n\nPlease try selecting a different location.";
            });
        }
    }

    [RelayCommand]
    public async Task RestoreDatabase()
    {
        var db3FileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.Android, new[] { "*/*" } }, // MIME type "application/vnd.sqlite3" does not work, even with .sqlite extension.
            { DevicePlatform.iOS, new[] {"public.database"} }, // UTI
            { DevicePlatform.MacCatalyst, new[] {"db3"} }, // File extension
            { DevicePlatform.WinUI, new[] {".db3"} } // File extension
        });

        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Pick DB to Restore",
            FileTypes = db3FileType
        });

        if (result is null)
            return;

        if (result.FileName.EndsWith("db3", StringComparison.OrdinalIgnoreCase))
        {
            _dataService.CloseConnection();

            try
            {
                File.Copy(result.FullPath, _dataService.GetDatabaseFullPath(), true);
            }
            catch (UnauthorizedAccessException uae)
            {
                #region UnauthorizedAccessException_Debug
#if DEBUG
                Debug.WriteLine(uae.Message);
#endif
                #endregion
            }
            catch (PathTooLongException ptle)
            {
                #region PathTooLongException_Debug
#if DEBUG
                Debug.WriteLine(ptle.Message);
#endif
                #endregion
            }
            catch (DirectoryNotFoundException dnfe)
            {
                #region DirectoryNotFoundException_Debug
#if DEBUG
                Debug.WriteLine(dnfe.Message);
#endif
                #endregion
            }
            catch (FileNotFoundException fnfe)
            {
                #region FileNotFoundException_Debug
#if DEBUG
                Debug.WriteLine(fnfe.Message);
#endif
                #endregion
            }
            catch (IOException ioe)
            {
                #region IOException_Debug
#if DEBUG
                Debug.WriteLine(ioe.Message);
#endif
                #endregion
            }
            catch
            {
                // TODO: We need to inform the user that something unexpected has happened, and they should create a bug report with steps on how to reproduce this error.
            }

            _dataService.OpenConnection();

            // We have to nav to LibraryView passing a valid LibraryModel; otherwise, we are stuck with the old one even after restoring the DB.
            int currentLibraryId = _dataService.ReadSettings().CurrentLibraryId;
            LibraryModel model = null;
            try
            {
                model = _dataService.ReadLibraries().Where(lib => lib.Id == currentLibraryId).ToList()[0];
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            if (model == null)
                return;

            await Shell.Current.GoToAsync(
                        $"///{nameof(LibraryView)}",
                        true,
                        new Dictionary<string, object>
                        {
                        {"NewLibraryModel", model}
                        });
        }
        else
        {
            await _popupService.ShowPopupAsync<ErrorWarningViewModel>(onPresenting: vm =>
            {
                vm.Title = "Error: File Format";
                vm.PopupMessage = "The wrong file format was\nselected.\n\nCornellPad only supports\n.db3 databases, which are\ncreated by the SQLite DBMS\nused by CornellPad.\n\nPlease choose a database file\nto restore.";
            });
        }
    }

    [RelayCommand]
    public void UpdateCacheSize(TextChangedEventArgs args)
    {
        if (string.IsNullOrWhiteSpace(args.NewTextValue))
            return;

        int number;
        if (!int.TryParse(args.NewTextValue, out number))
        {
            #region UpdateCacheSize_Error
#if DEBUG
            Debug.WriteLine("Error: Non-numerical value entered.");
#endif
            #endregion

            return;
        }

        #region UpdateCacheSize_Test
#if DEBUG
        Debug.WriteLine("CacheSize = " + CacheSize);
#endif
        #endregion

        _dataService.SetCacheSize(CacheSize);

        var settingsModel = _dataService.ReadSettings();
        settingsModel.AppCacheSize = CacheSize;
        _dataService.UpdateSettings(settingsModel);
    }

    [RelayCommand]
    public void UpdatePageSize()
    {
        int newPageSize = IndexToPageSize();
        _dataService.SetPageSize(newPageSize);
    }

    [RelayCommand]
    public void UpdateLockingMode()
    {
        string newLockingMode = IndexToLockingMode();
        _dataService.SetLockingMode(newLockingMode);

        var settingsModel = _dataService.ReadSettings();
        settingsModel.AppLockingMode = newLockingMode;
        _dataService.UpdateSettings(settingsModel);
    }

    [RelayCommand]
    public void UpdateTempStore()
    {
        int newTempStore = IndexToTempStore();
        _dataService.SetTempStore(newTempStore);

        var settingsModel = _dataService.ReadSettings();
        settingsModel.AppTempStore = newTempStore;
        _dataService.UpdateSettings(settingsModel);
    }

    [RelayCommand]
    public void UpdateAutoVacuum()
    {
        _dataService.SetAutoVacuum(AutoVacuumIndex);
    }

    [RelayCommand]
    public void ManualVacuum()
    {
        _dataService.ManualVacuum();
    }

    /*-------------------
     * Helper Methods
     --------------------*/
    private int PageSizeToIndex(int val)
    {
        int index;

        switch(val)
        {
            case 512:
                index = 0;
                break;
            case 1024:
                index = 1;
                break;
            case 2048:
                index = 2;
                break;
            case 4096:
                index = 3;
                break;
            case 8192:
                index = 4;
                break;
            case 16384:
                index = 5;
                break;
            case 32768:
                index = 6;
                break;
            case 65536:
                index = 7;
                break;
            default:
                index = -1;
                break;
        }

        return index;
    }

    private int IndexToPageSize()
    {
        int val;

        switch (PageSizeIndex)
        {
            case 0:
                val = 512;
                break;
            case 1:
                val = 1024;
                break;
            case 2:
                val = 2048;
                break;
            case 3:
                val = 4096;
                break;
            case 4:
                val = 8192;
                break;
            case 5:
                val = 16384;
                break;
            case 6:
                val = 32768;
                break;
            case 7:
                val = 65536;
                break;
            default:
                // SQLite default for page_size
                val = 4096;
                break;
        }

        return val;
    }

    private int LockingModeToIndex(string val)
    {
        int index;
        val = val.ToLower();

        switch(val)
        {
            case "normal":
                index = 0;
                break;
            case "exclusive":
                index = 1;
                break;
            default:
                index = -1;
                break;
        }

        return index;
    }

    private string IndexToLockingMode()
    {
        string val;

        switch (LockingModeIndex)
        {
            case 0:
                val = "normal";
                break;
            case 1:
                val = "exclusive";
                break;
            default:
                val = "normal";
                break;
        }

        return val;
    }

    private int TempStoreToIndex(int val)
    {
        int index;

        switch (val)
        {
            case 0:
                index = 0;
                break;
            case 2:
                index = 1;
                break;
            default:
                index = -1;
                break;
        }

        return index;
    }

    private int IndexToTempStore()
    {
        switch (TempStoreIndex)
        {
            case 0:
                return 0;
            case 1:
                return 2;
            default:
                return 0;
        }
    }

}
