using System;
using System.Collections.Generic;

namespace Data;

public partial class tblStudentLoginHistory
{
    public int Id { get; set; }

    public string SSOID { get; set; } = null!;

    public int AttemptCount { get; set; }

    public DateTime LastAccessedTime { get; set; }

    public string? Enrollment { get; set; }

    public string? DateOfBirth { get; set; }

    public int? StudentId { get; set; }
}
