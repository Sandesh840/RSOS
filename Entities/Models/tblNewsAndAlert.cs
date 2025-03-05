using System;
using System.Collections.Generic;

namespace Data;

public partial class tblNewsAndAlert
{
    public int Id { get; set; }

    public string Header { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime ValidTill { get; set; }

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? LastUpdatedOn { get; set; }

    public DateTime ValidFrom { get; set; }
}
