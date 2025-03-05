using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Authentication.Session;

public class SessionResponseDTO
{
    public int Id { get; set; }

    public int SessionId { get; set; }

    [StringLength(150)]
    public string Session { get; set; } = null!;

    public int Stream { get; set; }

    public bool IsPcp { get; set; }

    public bool IsAdmit { get; set; }

    public bool IsReeval { get; set; }

    public bool IsResult { get; set; }
}
