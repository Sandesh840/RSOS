using System.ComponentModel.DataAnnotations;

namespace Data;

public partial class tblSession
{
    [Key]
    public int Id { get; set; }

    public int SessionId { get; set; }

    [StringLength(150)]
    public string Session { get; set; } = null!;

    public int Stream { get; set; }

    public bool IsPcp { get; set; }

    public bool IsAdmit { get; set; }

    public bool IsReeval { get; set; }

    public bool IsResult { get; set; }

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? LastUpdatedOn { get; set; }
}
