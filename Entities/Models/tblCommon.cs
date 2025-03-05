using System;
using System.Collections.Generic;

namespace Data;

public partial class tblCommon
{
    public int Id { get; set; }

    public int CommonId { get; set; }

    public int Flag { get; set; }

    public string? Value { get; set; }

    public int LanguageId { get; set; }

    public decimal? Score { get; set; }

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? LastUpdatedOn { get; set; }

    public int? CorrectAnswer { get; set; }
}
