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

using SQLiteNetExtensions.Attributes;

namespace CornellPad.Models;

/// <summary>
/// Holds all data members that represent a group of
/// note entries on a single topic. Also includes a
/// summary of the contained notes, commonly referred
/// to as a <em>"What I Learned Today"</em> section.
/// </summary>
[Table("NotePage")]
public partial class NoteModel : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    [Column("ID")]
    public int Id { get; set; }



    [Column("TopicID"), ForeignKey(typeof(TopicModel))]
    public int TopicId { get; set; }



    [Column("NoteTitle")]
    public string Title { get; set; }



    [Column("CreatedOn")]
    public DateTime CreationDate { get; set; }


    
    [ObservableProperty]
    int numberOfEntries;



    string summary;
    public string Summary
    {
        get { return summary; }
        set
        {
            if (summary == value)
                return;

            summary = value;
            OnPropertyChanged();
        }
    }



    [OneToMany(CascadeOperations = CascadeOperation.All)]
    public ObservableCollection<NoteEntry> NoteEntries { get; set; }

}

