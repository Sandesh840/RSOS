using System;
using System.Collections.Generic;

namespace Data;

public partial class tblFAQ
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int Sequence { get; set; }
}
