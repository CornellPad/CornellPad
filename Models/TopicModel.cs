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

using SQLiteNetExtensions.Attributes;

namespace CornellPad.Models;

/// <summary>
/// The model for a topic in the database.
/// </summary>
[Table("TopicEntry")]
public partial class TopicModel : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    [Column("ID")]
    public int Id { get; set; }

    [Column("LibraryID"), ForeignKey(typeof(LibraryModel))]
    public int LibraryId { get; set; }

    [NotNull]
    public string TopicName { get; set; }

    [ObservableProperty]
    int numberOfNotes;

    public string GlyphFamily { get; set; }

    public string GlyphValue { get; set; }

    [Ignore]
    public GlyphCollectionItem Glyph
    {
        get
        {
            return new GlyphCollectionItem()
            {
                GlyphValue = this.GlyphValue,
                GlyphFamily = this.GlyphFamily
            };
        }
        set
        {
            if (value.GlyphFamily != this.GlyphFamily ||
                value.GlyphValue != this.GlyphValue)
            {
                this.GlyphFamily = value.GlyphFamily;
                this.GlyphValue = value.GlyphValue;
            }
        }
    }


    [OneToMany(CascadeOperations = CascadeOperation.All)]
    public ObservableCollection<NoteModel> NotePages { get; set; }
}
