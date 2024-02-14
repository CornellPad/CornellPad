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
using System.Diagnostics.CodeAnalysis;

namespace CornellPad.Models;

[Table("Library")]
public partial class LibraryModel : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    [Column("ID")]
    public int Id { get; set; }

    [Column("LibraryName"), SQLite.NotNull]
    public string Name { get; set; }

    [Column("LibraryDescription")]
    public string Description { get; set; }

    [Column("LibraryCreationDate")]
    public DateTime CreatedDate { get; set; }

    [OneToMany(CascadeOperations = CascadeOperation.All)]
    public ObservableCollection<TopicModel> TopicEntries { get; set; }
}

public class LibraryModelComparer : IEqualityComparer<LibraryModel>
{
    public bool Equals(LibraryModel x, LibraryModel y)
    {
        if (x.Id == y.Id &&
            x.Name == y.Name &&
            x.Description == y.Description &&
            x.CreatedDate == y.CreatedDate)
            return true;
        else
            return false;
    }

    public int GetHashCode([DisallowNull] LibraryModel obj)
    {
        return obj.Id.GetHashCode();
    }
}