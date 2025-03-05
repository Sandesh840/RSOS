using System;
using System.Collections.Generic;

namespace Data;

public partial class tblStudentResponse
{
    public int Id { get; set; }

    public string? GUID { get; set; }

    public string? QuizGUID { get; set; }

    public int StudentId { get; set; }

    public int QuestionId { get; set; }

    public string? QuestionValue { get; set; }

    public bool IsEdited { get; set; }

    public bool IsUploaded { get; set; }

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? LastUpdatedOn { get; set; }
}
