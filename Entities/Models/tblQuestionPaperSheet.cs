using System;
using System.Collections.Generic;

namespace Data;

public partial class tblQuestionPaperSheet
{
    public int Id { get; set; }

    public int PaperType { get; set; }

    public int Class { get; set; }

    public int SubjectId { get; set; }

    public string UploadedFileName { get; set; } = null!;

    public string UploadedFileUrl { get; set; } = null!;

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? LastUpdatedOn { get; set; }
}
