using System;
using System.Collections.Generic;

namespace Data;

public partial class tblSubject
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? LastUpdatedOn { get; set; }

    public int? SubjectCode { get; set; }

    public string? TitleInHindi { get; set; }

    public int? Class { get; set; }
    
    public int? MaximumMarks { get; set; }
}
