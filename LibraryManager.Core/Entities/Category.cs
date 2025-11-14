using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LibraryManager.Core.Entities;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
