using System;
using System.Collections.Generic;

namespace Data;

public partial class tblEnrollmentStatus
{
    public int Id { get; set; }

    public int EnrollmentId { get; set; }

    public bool Status { get; set; }

    public string Reason { get; set; } = null!;

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? LastUpdatedOn { get; set; }
}
