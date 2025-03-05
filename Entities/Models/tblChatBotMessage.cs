using System;
using System.Collections.Generic;

namespace Data;

public partial class tblChatBotMessage
{
    public int Id { get; set; }

    public int MessageId { get; set; }

    public int MessageSubId { get; set; }

    public string Message { get; set; } = null!;

    public string MessageType { get; set; } = null!;

    public int Clickable { get; set; }

    public decimal? LanguageId { get; set; }

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? LastUpdatedOn { get; set; }
}
