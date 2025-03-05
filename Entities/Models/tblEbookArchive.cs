using System;
using System.Collections.Generic;

namespace Data;

public partial class tblEbookArchive
{
    public int Id { get; set; }

    public int CodeNo { get; set; }

    public string? NameOfBook { get; set; }

    public string Volume { get; set; } = null!;

    public string? FileName { get; set; }

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? LastUpdatedOn { get; set; }

    public int Class { get; set; }

    public int? Sequence { get; set; }
}
