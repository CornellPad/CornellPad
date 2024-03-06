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

namespace CornellPad.Services.Interfaces;

public interface IDataService
{
    void Init();

    public void CloseConnection();

    public void OpenConnection();

    public string GetDatabaseName();

    public string GetDatabaseExtention();

    public string GetDatabasePath();

    public string GetDatabaseFullPath();

    public void BackupDB(string destinationDatabasePath);

    /*************************************************/
    // SQLite Settings methods
    /*************************************************/
    public SettingsModel ReadSettings();

    public int UpdateSettings(SettingsModel settingsModel);

    public int GetCacheSize();

    public void SetCacheSize(int size);

    public int GetPageSize();

    public void SetPageSize(int size);

    public string GetLockingMode();

    public void SetLockingMode(string lockingMode);

    /// <summary>
    /// Retrieves the 'temp_store' setting from SQLite. The enum value
    /// that is shown in this PRAGMA's documentation is not used, as
    /// it doesn't appear that the PRAGMA returns the enum value when
    /// queried. It only returns numerical values.<br></br><br></br>
    /// It is important to note that "File" is <em>not</em> directly
    /// supported by the app. See the PRAGMA for
    /// <see href="https://www.sqlite.org/pragma.html#pragma_temp_store">temp_store</see>
    /// for more information.
    /// 
    /// </summary>
    /// <returns>
    /// <list type="bullet">
    ///     <listheader>
    ///         <term>Num Value</term>
    ///         <description>Enum Value</description>
    ///     </listheader>
    ///     <item>
    ///         <term>0</term>
    ///         <description>Default (supported)</description>
    ///     </item>
    ///     <item>
    ///         <term>1</term>
    ///         <description>File (<em><b>not</b> supported</em>)</description>
    ///     </item>
    ///     <item>
    ///         <term>2</term>
    ///         <description>Memory (supported)</description>
    ///     </item>
    /// </list>
    /// </returns>
    public int GetTempStore();

    public void SetTempStore(int tempStore);

    public int GetAutoVacuum();

    public void SetAutoVacuum(int autoVacuum);

    public void ManualVacuum();

    /*************************************************/
    // Library-level methods
    /*************************************************/
    int CreateLibrary(LibraryModel pageEntry);

    List<LibraryModel> ReadLibraries();

    int UpdateLibrary(LibraryModel pageEntry);

    void UpdateLibraryWithChildren(LibraryModel pageEntry);

    int DeleteLibrary(LibraryModel pageEntry);

    /*************************************************/
    // Topic-level methods
    /*************************************************/
    /// <summary>
    /// Creates a Topic within the data source.
    /// </summary>
    /// <param name="topicEntry">The TopicModel object
    /// to be created in the data source.</param>
    /// <returns>An integer value indicating how many
    /// items were added to the data source, if any.</returns>
    int CreateTopic(TopicModel topicEntry);

    /// <summary>
    /// Read all of the TopicModel objects from the
    /// data source that has an Id value that matches
    /// the one passed in through <paramref name="libraryId"/>
    /// </summary>
    /// <param name="libraryId">The Id value of the
    /// Library to read the TopicModels from.</param>
    /// <returns>A strongly typed List
    /// of TopicModel objects if successful;
    /// an empty List otherwise.</returns>
    List<TopicModel> ReadTopics(int libraryId);

    /// <summary>
    /// Updates the data source with the TopicModel
    /// object passed in through <paramref name="topicEntry"/>
    /// </summary>
    /// <param name="topicEntry">The TopicModel object
    /// to be updated in the data source.</param>
    /// <returns></returns>
    int UpdateTopic(TopicModel topicEntry);

    /// <summary>
    /// Deletes the TopicModel object passed in through
    /// <paramref name="topicEntry"/> from the data source.
    /// It would be advisable to inform the user that
    /// this is an irreversable action before execution.
    /// </summary>
    /// <param name="topicEntry">The TopicModel object
    /// to be deleted from the data source.</param>
    /// <returns></returns>
    int DeleteTopic(TopicModel topicEntry);

    /*************************************************/
    // NotePage-level methods
    /*************************************************/
    int CreateNotePage(NoteModel pageEntry);

    NoteModel ReadNotePage(int pageId);

    List<NoteModel> ReadAllNotePages(int topicId);

    int UpdateNotePage(NoteModel pageEntry);

    int DeleteNotePage(NoteModel pageEntry);

    int UpdateNotePageSummary(int pageId, string noteSummary);

    /*************************************************/
    // NoteEntry-level methods
    /*************************************************/
    int CreateNoteEntry(NoteEntry noteEntry);

    NoteEntry ReadNoteEntry(int noteId);

    List<NoteEntry> ReadAllNoteEntries(int pageId);

    int UpdateNoteEntry(NoteEntry noteEntry);

    int DeleteNoteEntry(NoteEntry noteEntry);
}
