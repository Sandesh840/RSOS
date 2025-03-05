using System;
using System.Collections.Generic;

namespace Data;

public partial class tblStudentScore
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int Class { get; set; }

    public int SubjectId { get; set; }

    public int TopicId { get; set; }

    public string Score { get; set; } = null!;

    public bool IsEdited { get; set; }

    public bool IsUploaded { get; set; }

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? LastUpdatedOn { get; set; }

    public string GUID { get; set; } = null!;
}
