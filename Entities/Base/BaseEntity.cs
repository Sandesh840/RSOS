using System.ComponentModel.DataAnnotations;

namespace Model.Base;

public class BaseEntity<TPrimaryKey>
{
    [Key]
    public TPrimaryKey Id { get; set; } = default!;
    
    public bool IsActive { get; set; } = true;
    
    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public int? LastUpdatedBy { get; set; }

    public DateTime? LastUpdatedOn { get; set; }
}