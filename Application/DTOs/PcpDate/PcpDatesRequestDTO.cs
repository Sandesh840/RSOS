using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.PcpDate;

public class PcpDatesRequestDTO
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
    public DateTime StartDate { get; set; }

    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
    public DateTime EndDate { get; set; }
}