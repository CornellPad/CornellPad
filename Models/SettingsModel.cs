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

namespace CornellPad.Models;

[Table("Settings")]
public class SettingsModel
{
    [PrimaryKey, AutoIncrement]
    [Column("ID")]
    public int Id { get; set; }

    [Column("CurrentLibraryID")]
    public int CurrentLibraryId { get; set; }

    [Column("ApplicationTheme")]
    public CornellPadTheme CurrentTheme { get; set; }

    [Column("HideDBSettingsWarning")]
    public bool IsHidingDBSettingsWarning { get; set; }

    [Column("AppDBCacheSize")]
    public int AppCacheSize { get; set; }

    [Column("AppDBLockingMode")]
    public string AppLockingMode { get; set; }

    [Column("AppDBTempStore")]
    public int AppTempStore { get; set; }

    public double WindowSizeX { get; set; }
    public double WindowSizeY { get; set;}

    public double WindowPositionX { get; set; }
    public double WindowPositionY { get; set;}
}
