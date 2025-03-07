using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Model.Models;

namespace Application.DTOs.Scheme;

public class SchemeRequestDTO
{
    public int Id { get; set; }
    
    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? URL { get; set; }

    [ValidateNever]
    public List<tblSchemeFile> tblSchemeFile { get; set; }
}
