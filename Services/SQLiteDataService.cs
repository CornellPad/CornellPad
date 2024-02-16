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
using SQLiteNetExtensions.Extensions;
using System.Linq.Expressions;

namespace CornellPad.Services;

public class SQLiteDataService : IDataService
{
    SQLiteConnection _connection;
    private readonly ILogger<SQLiteDataService> _logger;

    public SQLiteDataService(ILogger<SQLiteDataService> logger)
    {
        _logger = logger;

        if (_connection is null)
            Init();
    }

    public void Init()
    {
        /*
        string databaseName = "CornellPadDB.db3";
        var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string path = Path.Combine(basePath, databaseName);
        // */

        _connection = new SQLiteConnection(
            GetDatabaseFullPath(),
            SQLite.SQLiteOpenFlags.ReadWrite |
            SQLite.SQLiteOpenFlags.Create
            );


        // TODO: This is for Android and iOS, where we don't have direct access to the entire filesystem to delete any previous DBs. REMOVE THIS ONCE DEVELOPMENT IS DONE!
        if (false)
        {
            _ = _connection.DeleteAll<SettingsModel>();
            _ = _connection.DeleteAll<LibraryModel>();
            _ = _connection.DeleteAll<TopicModel>();
            _ = _connection.DeleteAll<NoteModel>();
            _ = _connection.DeleteAll<NoteEntry>();

            // Clear the DB sequence data
            _ = _connection.Execute("DELETE FROM sqlite_sequence WHERE name = 'Settings';");
            _ = _connection.Execute("DELETE FROM sqlite_sequence WHERE name = 'Library';");
            _ = _connection.Execute("DELETE FROM sqlite_sequence WHERE name = 'TopicEntry';");
            _ = _connection.Execute("DELETE FROM sqlite_sequence WHERE name = 'NotePage';");
            _ = _connection.Execute("DELETE FROM sqlite_sequence WHERE name = 'NoteEntry';");
        }
        
        

        _connection.CreateTable<SettingsModel>();
        _connection.CreateTable<LibraryModel>();
        _connection.CreateTable<TopicModel>();
        _connection.CreateTable<NoteModel>();
        _connection.CreateTable<NoteEntry>();

        // Since it is application-wide, we create the SettingsModel here, if needed
        try
        {
            SettingsModel model = _connection.GetAllWithChildren<SettingsModel>()[0];

            // These settings are session-based; if we don't set these each time,
            // SQLite will use the defaults.
            SetCacheSize(model.AppCacheSize);
            SetLockingMode(model.AppLockingMode);
            SetTempStore(model.AppTempStore);

            UpdateSettings(model); // forces SQLite to enforce EXCLUSIVE lock; otherwise, SHARED will be used until a write operation.
        }
        catch (ArgumentOutOfRangeException)
        {
            SettingsModel model = new SettingsModel();

            model.CurrentTheme = CornellPadTheme.Auto;
            model.IsHidingDBSettingsWarning = false;

            model.AppCacheSize = GetCacheSize();
            model.AppLockingMode = GetLockingMode();
            model.AppTempStore = GetTempStore();

            // Minus one indicates that the user hasn't resized or repositioned the app yet.
            model.WindowPositionX = -1;
            model.WindowPositionY = -1;
            model.WindowSizeX = -1;
            model.WindowSizeY = -1;

            int inserted = _connection.Insert(model);

            #region debug_settings
#if DEBUG
            Debug.WriteLine(inserted == 1 ? "Success" : "Failure", "'System.ArgumentOutOfRangeException' handled. The SettingsModel was created");
#endif
#endregion
        }

        // We must have at least one library, so the user can immediately start using the app.
        try
        {
            _ = _connection.GetAllWithChildren<LibraryModel>()[0];
        }
        catch (ArgumentOutOfRangeException)
        {
            var libraryModel = new LibraryModel()
            {
                Name = "default",
                CreatedDate = DateTime.Now,
                Description = "The default library.",
            };
            int inserted = CreateLibrary(libraryModel);

            var settings = ReadSettings();
            settings.CurrentLibraryId = libraryModel.Id;
            int updated = UpdateSettings(settings);

            #region debug_library
#if DEBUG
            Debug.WriteLine((inserted == 1 && updated == 1) ? "Success" : "Failure", "'System.ArgumentOutOfRangeException' handled. 'default' library was created and setting were updated");
#endif
#endregion
        }
    }

    public void CloseConnection()
    {
        _connection.Close();
    }

    public void OpenConnection()
    {
        Init();
    }

    public string GetDatabaseName()
    {
        return "CornellPadDB";
    }

    public string GetDatabaseExtention()
    {
        return ".db3";
    }

    public string GetDatabasePath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        //var basePath = FileSystem.Current.AppDataDirectory;
    }

    public string GetDatabaseFullPath()
    {
        return Path.Combine(GetDatabasePath(), GetDatabaseName() + GetDatabaseExtention() );
    }

    public void BackupDB(string destinationDatabasePath)
    {
        _connection.Backup(destinationDatabasePath);
    }

    /*************************************************/
    // Settings methods
    /*************************************************/
    public SettingsModel ReadSettings()
    {
        return _connection.GetAllWithChildren<SettingsModel>()[0];
    }

    public int UpdateSettings(SettingsModel settingsModel)
    {
        return _connection.Update(settingsModel);
    }

    public int GetCacheSize()
    {
        var cmd = _connection.CreateCommand("PRAGMA cache_size");
        var result = cmd.ExecuteScalar<int>();

        return result;
    }

    public void SetCacheSize(int size)
    {
        try
        {
            _connection.Execute($"PRAGMA cache_size = {size}");
        }
        catch (SQLite.SQLiteException e)
        {
#if DEBUG
            Debug.WriteLine(e.Message);
#elif !DEBUG
            _logger.LogError("Set PRAGMA cache_size Exception Message: {message}\n    Stack Trace: {trace}", e.Message, e.StackTrace);
#endif
        }
    }

    public int GetPageSize()
    {
        var cmd = _connection.CreateCommand("PRAGMA page_size");
        var result = cmd.ExecuteScalar<int>();

        return result;
    }

    public void SetPageSize(int size)
    {
        if (!int.IsPow2(size) || size < 512 || size > 65536)
            return;

        try
        {
            _connection.Execute($"PRAGMA page_size = {size}");
            ManualVacuum();
        }
        catch (SQLite.SQLiteException e)
        {
#if DEBUG
            Debug.WriteLine(e.Message);
#elif !DEBUG
            _logger.LogError("Set PRAGMA page_size Exception Message: {message}\n    Stack Trace: {trace}", e.Message, e.StackTrace);
#endif
        }
    }

    public string GetLockingMode()
    {
        var cmd = _connection.CreateCommand("PRAGMA locking_mode");
        var result = cmd.ExecuteScalar<string>();

        return result;
    }

    public void SetLockingMode(string lockingMode)
    {
        lockingMode = lockingMode.ToLower();

        if (lockingMode != "normal" && lockingMode != "exclusive")
            return;

        try
        {
            var cmd = _connection.CreateCommand($"PRAGMA locking_mode = {lockingMode}");
            var result = cmd.ExecuteScalar<string>();
        }
        catch (SQLite.SQLiteException e)
        {
#if DEBUG
            Debug.WriteLine(e.Message);
#elif !DEBUG
            _logger.LogError("Set PRAGMA locking_mode Exception Message: {message}\n    Stack Trace: {trace}", e.Message, e.StackTrace);
#endif
        }
    }

    public int GetTempStore()
    {
        var cmd = _connection.CreateCommand("PRAGMA temp_store");
        var result = cmd.ExecuteScalar<int>();

        return result;
    }

    public void SetTempStore(int tempStore)
    {
        if (tempStore != 0 && tempStore != 2)
            return;

        try
        {
            var cmd = _connection.CreateCommand($"PRAGMA temp_store = {tempStore}");
            var result = cmd.ExecuteScalar<int>();
        }
        catch (SQLite.SQLiteException e)
        {
#if DEBUG
            Debug.WriteLine(e.Message);
#elif !DEBUG
            _logger.LogError("Set PRAGMA temp_store Exception Message: {message}\n    Stack Trace: {trace}", e.Message, e.StackTrace);
#endif
        }
    }

    public int GetAutoVacuum()
    {
        var cmd = _connection.CreateCommand("PRAGMA auto_vacuum");
        var result = cmd.ExecuteScalar<int>();

        return result;
    }

    public void SetAutoVacuum(int autoVacuum)
    {
        try
        {
            _connection.Execute($"PRAGMA auto_vacuum = {autoVacuum}");
            ManualVacuum();
        }
        catch (SQLite.SQLiteException e)
        {
#if DEBUG
            Debug.WriteLine(e.Message);
#elif !DEBUG
            _logger.LogError("Set PRAGMA auto_vacuum Exception Message: {message}\n    Stack Trace: {trace}", e.Message, e.StackTrace);
#endif
        }
    }

    public void ManualVacuum()
    {
        _connection.Execute("VACUUM");
    }

    /*************************************************/
    // Library-level methods
    /*************************************************/
    #region Library-Level_Methods
    public int CreateLibrary(LibraryModel libraryEntry)
    {
        return _connection.Insert(libraryEntry);
    }

    public List<LibraryModel> ReadLibraries()
    {
        return _connection.GetAllWithChildren<LibraryModel>();
    }

    public int UpdateLibrary(LibraryModel libraryEntry)
    {
        return _connection.Update(libraryEntry);
    }

    public void UpdateLibraryWithChildren(LibraryModel libraryEntry)
    {
        _connection.UpdateWithChildren(libraryEntry);
    }

    public int DeleteLibrary(LibraryModel libraryEntry)
    {
        var topicEntries = ReadTopics(libraryEntry.Id);

        if (topicEntries?.Count > 0)
        {
            foreach (var topic in topicEntries)
            {
                DeleteTopic(topic);
            }
        }

        return _connection.Delete(libraryEntry);
    }
    #endregion

    /*************************************************/
    // Topic-level methods
    /*************************************************/
    #region Topic-Level_Methods
    public int CreateTopic(TopicModel topicEntry)
    {
        return _connection.Insert(topicEntry);
    }

    public List<TopicModel> ReadTopics(int libraryId)
    {
        return _connection.GetAllWithChildren<TopicModel>().Where(t =>  t.LibraryId == libraryId).ToList();
    }

    public int UpdateTopic(TopicModel topicEntry)
    {
        return _connection.Update(topicEntry);
    }

    public int DeleteTopic(TopicModel topicEntry)
    {
        var notePages = ReadAllNotePages(topicEntry.Id);

        if (notePages?.Count > 0)
        {
            foreach (var note in notePages)
            {
                DeleteNotePage(note);
            }
        }

        return _connection.Delete(topicEntry);
    }
    #endregion

    /*************************************************/
    // NotePage-level methods
    /*************************************************/
    #region NotePage-Level_Methods
    public int CreateNotePage(NoteModel notePage)
    {
        return _connection.Insert(notePage);
    }

    public NoteModel ReadNotePage(int pageId)
    {
        return _connection.Get<NoteModel>(pageId);
    }

    public List<NoteModel> ReadAllNotePages(int topicId)
    {
        return _connection.Query<NoteModel>("SELECT * FROM NotePage WHERE TopicID = ?", topicId);
    }

    public int UpdateNotePage(NoteModel notePage)
    {
        return _connection.Update(notePage);
    }

    public int DeleteNotePage(NoteModel notePage)
    {
        var noteEntries = ReadAllNoteEntries(notePage.Id);

        if (noteEntries?.Count > 0)
        {
            foreach (var entry in noteEntries)
            {
                DeleteNoteEntry(entry);
            }
        }

        return _connection.Delete(notePage);
    }

    public int UpdateNotePageSummary(int pageId, string noteSummary)
    {
        var result = _connection.Query<NoteModel>("UPDATE NotePage SET Summary = ? WHERE ID = ?", noteSummary, pageId);

        return result.Count;
    }
    #endregion

    /*************************************************/
    // NoteEntry-level methods
    /*************************************************/
    #region NoteEntry-Level_Methods
    public int CreateNoteEntry(NoteEntry noteEntry)
    {
        return _connection.Insert(noteEntry);
    }

    public NoteEntry ReadNoteEntry(int noteId)
    {
        return _connection.Get<NoteEntry>(noteId);
    }

    public List<NoteEntry> ReadAllNoteEntries(int pageId)
    {
        return _connection.GetAllWithChildren<NoteEntry>().Where(e => e.PageId == pageId).ToList();
    }

    public int UpdateNoteEntry(NoteEntry noteEntry)
    {
        return _connection.Update(noteEntry);
    }

    public int DeleteNoteEntry(NoteEntry noteEntry)
    {
        return _connection.Delete(noteEntry);
    }
    #endregion

    /*************************************************/
    // Helper methods
    /*************************************************/
    #region Helper_Methods
    /// <summary>
    /// A very slick little method that uses binary & to
    /// compute whether <paramref name="num"/> is a power
    /// of two or not.
    /// 
    /// This method is from the user Greg Hewgill, and can
    /// be found on StackOverflow:
    /// 
    /// https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
    /// 
    /// </summary>
    /// <param name="num"></param>
    /// <returns>true if <paramref name="num"/> is a power of two; false otherwise.</returns>
    private bool IsPowerofTwo(int num)
    {
        return (num != 0) && ((num & (num - 1)) == 0);
    }
    #endregion

}
