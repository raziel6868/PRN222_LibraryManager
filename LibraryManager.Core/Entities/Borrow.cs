using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LibraryManager.Core.Entities;

public partial class Borrow
{
    public int BorrowId { get; set; }

    public int CustomerId { get; set; }

    public int BookId { get; set; }

    public DateTime RequestDate { get; set; }

    public DateTime? BorrowDate { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public string Status { get; set; } = null!;

    public bool CreatedByCustomer { get; set; }

    public int? ProcessedByStaffId { get; set; }

    public decimal FineAmount { get; set; }

    public bool IsFinePaid { get; set; }

    [JsonIgnore]
    public virtual Book Book { get; set; } = null!;

    [JsonIgnore]
    public virtual Customer Customer { get; set; } = null!;

    [JsonIgnore]
    public virtual Staff? ProcessedByStaff { get; set; }
}
