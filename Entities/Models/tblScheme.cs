using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data;

public partial class tblScheme
{
    [Key]
    public int Id { get; set; }

    public string Header { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? URL { get; set; }

    [ValidateNever]
    public List<tblSchemeFile> SchemeFile { get; set; }

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? LastUpdatedOn { get; set; }

    public bool IsTriggered { get; set; }

}
