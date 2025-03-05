using System;
using System.Collections.Generic;

namespace Data;

public partial class tblPCPDate
{
    public int Id { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public int CreatedBy { get; set; }
}
