using System;
using System.Collections.Generic;

namespace Data;

public partial class tblContent
{
    public int Id { get; set; }

    public int SubjectId { get; set; }

    public int Class { get; set; }

    public string? Faculty { get; set; }

    public string ChapterName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string PartName { get; set; } = null!;

    public int ChapterNo { get; set; }

    public int PartNo { get; set; }

    public int TimeInSeconds { get; set; }

    public string YouTubeLink { get; set; } = null!;

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? LastUpdatedOn { get; set; }

    public int? Sequence { get; set; }
}
