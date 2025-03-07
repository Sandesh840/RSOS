using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Model.Models;

namespace Application.DTOs.Scheme;

public class SchemeResponseDTO
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? URL { get; set; }

    [ValidateNever]
    public List<tblSchemeFile> SchemeFile { get; set; }

    public int IsActive { get; set; }

    public int IsTriggered { get; set; }

    public DateTime CreatedOn { get; set; }    
}