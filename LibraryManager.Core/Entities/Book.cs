using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LibraryManager.Core.Entities;

public partial class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public int CategoryId { get; set; }

    public int AuthorId { get; set; }

    public string? Isbn { get; set; }

    public int? PublishYear { get; set; }

    public string? Summary { get; set; }

    public int TotalCopies { get; set; }

    public int AvailableCopies { get; set; }

    [JsonIgnore]
    public virtual Author Author { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();

    [JsonIgnore]
    public virtual Category Category { get; set; } = null!;
}
