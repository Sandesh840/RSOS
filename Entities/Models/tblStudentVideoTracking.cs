using System;
using System.Collections.Generic;

namespace Data;

public partial class tblStudentVideoTracking
{
    public int Id { get; set; }

    public int SubjectId { get; set; }

    public int VideoId { get; set; }

    public int StudentId { get; set; }

    public int Class { get; set; }

    public bool IsCompleted { get; set; }

    public int? PlayTimeInSeconds { get; set; }

    public decimal? PercentageCompleted { get; set; }

    public int? VideoDurationInSeconds { get; set; }

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }
}
