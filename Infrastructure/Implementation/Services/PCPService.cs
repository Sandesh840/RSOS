using Application.DTOs.PCP;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using ClosedXML.Excel;

namespace Data.Implementation.Services;

public class PCPService : IPCPService
{
    private readonly IGenericRepository _genericRepository;

    public PCPService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<PCPQuestionsResponseDTO> GetPCPQuestionsByClass(int classId, int type)
    {
        var subjects = await _genericRepository.GetAsync<tblSubject>(x => x.Class == classId);

        subjects = subjects.OrderBy(x => x.SubjectCode);

        var questions = new List<Question>();

        foreach (var subject in subjects)
        {
            var questionSheets =
                await _genericRepository.GetAsync<tblQuestionPaperSheet>(x =>
                    x.SubjectId == subject.Id && x.PaperType == type);

            var questionPaperSheets = questionSheets as tblQuestionPaperSheet[] ?? questionSheets.ToArray();
            
            questions.Add(new Question()
            {
                SubjectId = subject.Id,
                SubjectCode = subject.SubjectCode ?? 0,
                SubjectName = subject.Title,
                AttachmentId = questionPaperSheets.Where(x => x.IsActive).MaxBy(x => x.Id)?.Id ?? 0,
                PaperLastUploadedDate = questionPaperSheets.Where(x => x.IsActive).MaxBy(x => x.Id)?.CreatedOn.ToString("dd-MM-yyyy HH:mm:ss") ?? null
            });
        }
        
        var result = new PCPQuestionsResponseDTO()
        {
            ClassId = classId,
            PaperTypeId = type,
            PaperType = type == 1 ? "Practice Paper" : "ePCP Final Paper",
            Questions = questions
        };
        
        return result;
    }
    
    public async Task<List<PCPQuestionResponseDTO>> GetUploadedQuestionSheets(int subjectCode, int type)
    {
        var questionSheets =
            await _genericRepository.GetAsync<tblQuestionPaperSheet>(x =>
                x.SubjectId == subjectCode && x.PaperType == type);

        var subject = await _genericRepository.GetByIdAsync<tblSubject>(subjectCode);
        
        return questionSheets.OrderByDescending(x => x.Id).Select(x => new PCPQuestionResponseDTO()
        {
            Id = x.Id,
            Code = subject?.SubjectCode ?? 1,
            Subject = subject?.Title ?? "",
            Type = type == 1 ? "ePCP Final Paper" : "Practice Paper",
            IsArchived = !x.IsActive,
            UploadedDate = x.CreatedOn.ToString("dd-MM-yyyy HH-mm-ss")
        }).ToList();
    }

    public async Task<(string, string)> GetUploadedQuestionSheetById(int questionSheetId)
    {
        var questionSheet = await _genericRepository.GetByIdAsync<tblQuestionPaperSheet>(questionSheetId);

        return questionSheet == null ? ("", "") : (questionSheet.UploadedFileName, questionSheet.UploadedFileUrl);
    }

    public async Task<(bool, string)> IsUploadedSheetValid(PCPQuestionRequestDTO question)
    {
        try
        {
            var subject = await _genericRepository.GetFirstOrDefaultAsync<tblSubject>(x => 
                x.SubjectCode == question.Code);
            
            if (subject == null) return (false, "Subject with the respective code could not be found.");

            using var workbook = new XLWorkbook(question.QuestionSheet.OpenReadStream());
   
            var worksheet = workbook.Worksheet(1);

            if (question.PaperTypeId == 1)
            {
                var questionsList = worksheet.Rows().Skip(1)
                    .Select(row => new 
                    {
                        ClassId = row.Cell(1).GetValue<int?>() ?? 0, 
                        SubjectCode = row.Cell(2).GetValue<int?>() ?? 0, 
                        IsMandatory = row.Cell(3).GetValue<string?>()?.ToUpper() == "YES", 
                        ChapterName = row.Cell(4).GetValue<string?>()?.Trim() ?? "", 
                        PartNumber = row.Cell(5).GetValue<int?>() ?? 0, 
                        SequenceNumber = row.Cell(6).GetValue<int?>() ?? 0, 
                        Question = row.Cell(7).GetValue<string?>()?.Trim(),
                        Commons = new List<string>()
                        {
                            row.Cell(8).GetValue<string?>() ?? "",
                            row.Cell(9).GetValue<string?>() ?? "",
                            row.Cell(10).GetValue<string?>() ?? "",
                            row.Cell(11).GetValue<string?>() ?? "",
                        },
                        Language = row.Cell(12).GetValue<string?>()?.Trim().ToUpper() is "HINDI" or "हिंदी" or "1" ? 1 : row.Cell(12).GetValue<string?>()?.Trim().ToUpper() is "ENGLISH" or "2" ? 2 : 0,
                        CorrectAnswer = row.Cell(13).GetValue<string?>()?.Trim().ToUpper() ?? "A"
                    }).ToList();
                
                var isInValid = questionsList.Any(x =>
                    x.ClassId == 0 || x.SubjectCode == 0 || x.SequenceNumber == 0 || x.PartNumber == 0 ||
                    x.Commons.Any(string.IsNullOrEmpty) || string.IsNullOrEmpty(x.ChapterName) || string.IsNullOrEmpty(x.Question));

                if (isInValid)
                {
                    return (false, "Please do not leave any fields empty.");
                }
                
                await DeletePracticeContents(subject.Id);
                
                foreach (var item in questionsList)
                {
                    if (item.ClassId != question.Class)
                        return (false, "Please insert the same value of class for all the columns in the following sheet.");
                
                    if (item.SubjectCode != question.Code)
                        return (false, "Please insert the same value of subject code for all the columns in the following sheet.");
                
                    if (item.CorrectAnswer != "A" && item.CorrectAnswer != "B" && item.CorrectAnswer != "C" && item.CorrectAnswer != "D")
                        return (false, "Please insert a valid correct option value from the provided options.");
                    
                    if (item.Language == 0)
                        return (false, "Please insert a valid language value, only Hindi, हिंदी or English are accepted.");

                    var content = await _genericRepository.GetFirstOrDefaultAsync<tblContent>(x =>
                        x.ChapterName == item.ChapterName && x.PartNo == item.PartNumber);
                    
                    if (content == null)
                    {
                        var contents = await _genericRepository.GetAsync<tblContent>(x => 
                            x.SubjectId == subject.Id);
        
                        var contentsList = contents as tblContent[] ?? contents.ToArray();
                        
                        var existingChapterNumbers = contentsList.Select(x => x.ChapterNo);

                        var chapterNumbers = existingChapterNumbers as int[] ?? existingChapterNumbers.ToArray();
                        
                        var maxChapterNumber = chapterNumbers.Any() ? chapterNumbers.Max() : 0;

                        if (maxChapterNumber < 1000)
                        {
                            maxChapterNumber = 1000;
                        }
                        
                        var contentModel = new tblContent()
                        {
                            SubjectId = subject.Id,
                            ChapterName = item.ChapterName,
                            ChapterNo = maxChapterNumber + 1,
                            PartNo = item.PartNumber,
                            CreatedBy = 1,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            Class = question.Class,
                            Sequence = (maxChapterNumber + 1) * 1000 + item.PartNumber,
                            Description = $"RSOS Class {question.Class} {subject.Title} Chapter {maxChapterNumber + 1} | Rajasthan State Open School Class {question.Class} {subject.Title}",
                            Faculty = "",
                            PartName = $"Part {item.PartNumber}",
                            TimeInSeconds = 0,
                            YouTubeLink = "-"
                        };

                        await _genericRepository.InsertAsync(contentModel);
                    }
                }
        
                return (true, "Sheet successfully validated.");
            }
            else
            {
                var questionsList = worksheet.Rows().Skip(1)
                    .Select(row => new
                    {
                        ClassId = row.Cell(1).GetValue<int?>() ?? 0,
                        SubjectCode = row.Cell(2).GetValue<int?>() ?? 0,
                        IsMandatory = row.Cell(3).GetValue<string?>()?.Trim().ToUpper() == "YES",
                        SequenceNumber = row.Cell(4).GetValue<int?>() ?? 0,
                        Question = row.Cell(5).GetValue<string?>()?.Trim(),
                        Commons = new List<string>()
                        {
                            row.Cell(6).GetValue<string?>() ?? "",
                            row.Cell(7).GetValue<string?>() ?? "",
                            row.Cell(8).GetValue<string?>() ?? "",
                            row.Cell(9).GetValue<string?>() ?? "",
                        },
                        Language = row.Cell(10).GetValue<string?>()?.Trim().ToUpper() is "HINDI" or "हिंदी" or "1" ? 1 : row.Cell(10).GetValue<string?>()?.Trim().ToUpper() is "ENGLISH" or "2" ? 2 : 0,
                        CorrectAnswer = row.Cell(11).GetValue<string?>()?.Trim().ToUpper() ?? "A"
                    }).ToList();

                var isInValid = questionsList.Any(x =>
                    x.ClassId == 0 || x.SubjectCode == 0 || x.SequenceNumber == 0 ||
                    x.Commons.Any(string.IsNullOrEmpty) || string.IsNullOrEmpty(x.Question));

                if (isInValid)
                {
                    return (false, "Please do not leave any fields empty.");
                }

                foreach (var item in questionsList)
                {
                    if (item.ClassId != question.Class)
                        return (false,
                            "Please insert the same value of class for all the columns in the following sheet.");

                    if (item.SubjectCode != question.Code)
                        return (false,
                            "Please insert the same value of subject code for all the columns in the following sheet.");

                    if (item.CorrectAnswer != "A" && item.CorrectAnswer != "B" && item.CorrectAnswer != "C" &&
                        item.CorrectAnswer != "D")
                        return (false, "Please insert a valid correct option value from the provided options.");
                    
                    if (item.Language == 0)
                        return (false, "Please insert a valid language value, only Hindi, हिंदी or English are accepted.");
                }

                return (true, "Sheet successfully validated.");
            }
        }
        catch (Exception)
        {
            return (false, "An exception occured while processing your request, please upload a valid file");
        }
    }

    public async Task<bool> UploadQuestions(PCPQuestionRequestDTO question)
    {
        try
        {
            var subject = await _genericRepository.GetFirstOrDefaultAsync<tblSubject>(x => 
            x.SubjectCode == question.Code);

            if(subject == null) return false;
            
            using var workbook = new XLWorkbook(question.QuestionSheet.OpenReadStream());
       
            var worksheet = workbook.Worksheet(1);
            
            await UploadQuestionsToArchive(subject.Id, question.PaperTypeId);

            if (question.PaperTypeId == 1)
            {
                var questionsList = worksheet.Rows().Skip(1)
                .Select(row => new 
                {
                    ClassId = row.Cell(1).GetValue<int?>() ?? 0, 
                    SubjectCode = row.Cell(2).GetValue<int?>() ?? 0, 
                    IsMandatory = row.Cell(3).GetValue<string?>()?.ToUpper() == "YES", 
                    ChapterName = row.Cell(4).GetValue<string?>()?.Trim() ?? "", 
                    PartNumber = row.Cell(5).GetValue<int?>() ?? 0, 
                    SequenceNumber = row.Cell(6).GetValue<int?>() ?? 0, 
                    Question = row.Cell(7).GetValue<string?>()?.Trim(),
                    Commons = new List<string>()
                    {
                        row.Cell(8).GetValue<string?>()?.Trim() ?? "",
                        row.Cell(9).GetValue<string?>()?.Trim() ?? "",
                        row.Cell(10).GetValue<string?>()?.Trim() ?? "",
                        row.Cell(11).GetValue<string?>()?.Trim() ?? "",
                    },
                    Language = row.Cell(12).GetValue<string?>()?.Trim().ToUpper() is "HINDI" or "हिंदी" ? 1 : row.Cell(12).GetValue<string?>()?.Trim().ToUpper() is "ENGLISH" or "2" ? 2 : 0,
                    CorrectAnswer = row.Cell(13).GetValue<string?>()?.Trim().ToUpper() ?? "A"
                }).ToList();

                foreach (var item in questionsList)
                {
                    var content = await _genericRepository.GetFirstOrDefaultAsync<tblContent>(x =>
                        x.ChapterName == item.ChapterName && x.PartNo == item.PartNumber);
                    
                    var questionModel = new tblQuestion()
                    {
                        Class = item.ClassId,
                        SubjectId = subject.Id,
                        TopicId = content?.Id ?? 0,
                        Question = item.Question,
                        IsMandatory = item.IsMandatory,
                        Sequence = item.SequenceNumber,
                        QuestionTypeId = 1,
                        PaperType = question.PaperTypeId,
                        IsActive = true,
                        CreatedBy = 1,
                        CreatedOn = DateTime.Now,
                    };

                    var questionId = await _genericRepository.InsertAsync(questionModel);

                    var insertedQuestionModel = await _genericRepository.GetByIdAsync<tblQuestion>(questionId);

                    if (insertedQuestionModel == null) continue;
                    
                    insertedQuestionModel.Flag = questionId;
                        
                    await _genericRepository.UpdateAsync(insertedQuestionModel);

                    var correctAnswer = item.CorrectAnswer switch
                    {
                        "A" => 0,
                        "B" => 1,
                        "C" => 2,
                        "D" => 3,
                        _ => 0
                    };

                    var score = subject.MaximumMarks > 5 ? 1m : 0.5m;
                    
                    for (var i = 0; i < item.Commons.Count; i++)
                    {
                        var commonModel = new tblCommon()
                        {
                            Flag = questionId,
                            Score = score,
                            LanguageId = item.Language,
                            Value = item.Commons[i],
                            CommonId = i + 1,
                            IsActive = true,
                            CorrectAnswer = correctAnswer == i ? 1 : 0,
                            CreatedBy = 1,
                            CreatedOn = DateTime.Now,
                        };
                        
                        await _genericRepository.InsertAsync(commonModel);
                        
                    }
                }
            }
            else
            {
                var questionsList = worksheet.Rows().Skip(1)
                .Select(row => new 
                {
                    ClassId = row.Cell(1).GetValue<int?>() ?? 0, 
                    SubjectId = subject.Id, 
                    IsMandatory = row.Cell(3).GetValue<string?>()?.Trim()?.ToUpper() == "YES", 
                    SequenceNumber = row.Cell(4).GetValue<int?>() ?? 0, 
                    Question = row.Cell(5).GetValue<string?>()?.Trim(),
                    Commons = new List<string>()
                    {
                        row.Cell(6).GetValue<string?>()?.Trim() ?? "",
                        row.Cell(7).GetValue<string?>()?.Trim() ?? "",
                        row.Cell(8).GetValue<string?>()?.Trim() ?? "",
                        row.Cell(9).GetValue<string?>()?.Trim() ?? "",
                    },
                    Language = row.Cell(10).GetValue<string?>()?.Trim().ToUpper() is "HINDI" or "हिंदी" or "1" ? 1 : row.Cell(12).GetValue<string?>()?.Trim().ToUpper() is "ENGLISH" or "2" ? 2 : 0,
                    CorrectAnswer = row.Cell(11).GetValue<string?>()?.Trim().ToUpper() ?? "A"
                }).ToList();

                foreach (var item in questionsList)
                {
                    var questionModel = new tblQuestion()
                    {
                        Class = item.ClassId,
                        SubjectId = subject.Id,
                        TopicId = 0,
                        Question = item.Question,
                        IsMandatory = item.IsMandatory,
                        Sequence = item.SequenceNumber,
                        QuestionTypeId = 1,
                        PaperType = question.PaperTypeId,
                        IsActive = true,
                        CreatedBy = 1,
                        CreatedOn = DateTime.Now,
                    };

                    var questionId = await _genericRepository.InsertAsync(questionModel);

                    var insertedQuestionModel = await _genericRepository.GetByIdAsync<tblQuestion>(questionId);

                    if (insertedQuestionModel == null) continue;
                    
                    insertedQuestionModel.Flag = questionId;
                        
                    await _genericRepository.UpdateAsync(insertedQuestionModel);

                    var correctAnswer = item.CorrectAnswer switch
                    {
                        "A" => 0,
                        "B" => 1,
                        "C" => 2,
                        "D" => 3,
                        _ => 0
                    };
                    
                    for (var i = 0; i < item.Commons.Count; i++)
                    {
                        var commonModel = new tblCommon()
                        {
                            Flag = questionId,
                            Score = 1,
                            LanguageId = item.Language,
                            Value = item.Commons[i],
                            CommonId = i + 1,
                            IsActive = true,
                            CorrectAnswer = correctAnswer == i ? 1 : 0,
                            CreatedBy = 1,
                            CreatedOn = DateTime.Now,
                        };
                        
                        await _genericRepository.InsertAsync(commonModel);
                        
                    }
                }
            }

            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task UploadQuestionsWorksheet(PCPQuestionSheetRequestDTO questionSheet)
    {
        var questionPaperSheet = new tblQuestionPaperSheet()
        {
            SubjectId = questionSheet.SubjectId,
            Class = questionSheet.Class,
            UploadedFileName = questionSheet.UploadedFileName,
            UploadedFileUrl = questionSheet.UploadedFileUrl,
            PaperType = questionSheet.PaperType,
            IsActive = true,
            CreatedBy = 1,
            CreatedOn = DateTime.Now
        };

        await _genericRepository.InsertAsync(questionPaperSheet);
    }

    public async Task ArchiveQuestions(int subjectId, int type)
    {
        await UploadQuestionsToArchive(subjectId, type);
        
        await DeletePracticeContents(subjectId);
    }
    
    private async Task UploadQuestionsToArchive(int subjectId, int type)
    {
        var questions = await _genericRepository.GetAsync<tblQuestion>(x => 
            x.SubjectId == subjectId && x.PaperType == type);

        var commons = await _genericRepository.GetAsync<tblCommon>(x => 
            questions.Select(z => z.Flag).Contains(x.Flag));

        var questionsList = questions as tblQuestion[] ?? questions.ToArray();

        var questionsArchive = questionsList.Select(x => new tblQuestionsArchive()
        {
            SubjectId = x.SubjectId,
            Class = x.Class,
            Question = x.Question,
            Sequence = x.Sequence,
            TopicId = x.TopicId,
            QuestionTypeId = x.QuestionTypeId,
            Flag = x.Flag,
            IsMandatory = x.IsMandatory,
            PaperType = x.PaperType,
            IsActive = false,
            CreatedOn = x.CreatedOn,
            CreatedBy = x.CreatedBy,
        });

        var commonsList = commons as tblCommon[] ?? commons.ToArray();
        
        var commonsArchive = commonsList.Select(x => new tblCommonsArchive()
        {
            CommonId = x.CommonId,
            Flag = x.Flag,
            Value = x.Value,
            CorrectAnswer = x.CorrectAnswer,
            Score = x.Score,
            LanguageId = x.LanguageId,
            IsActive = false,
            CreatedOn = x.CreatedOn,
            CreatedBy = x.CreatedBy,
        });

        await _genericRepository.AddMultipleEntityAsync(questionsArchive);
        
        await _genericRepository.AddMultipleEntityAsync(commonsArchive);

        await _genericRepository.RemoveMultipleEntityAsync(questionsList);

        await _genericRepository.RemoveMultipleEntityAsync(commonsList);

        var subjectQuestionSheets =
            await _genericRepository.GetAsync<tblQuestionPaperSheet>(x =>
                x.SubjectId == subjectId && x.PaperType == type);

        foreach (var sheet in subjectQuestionSheets)
        {
            sheet.IsActive = false;

            await _genericRepository.UpdateAsync(sheet);
        }
    }

    private async Task DeletePracticeContents(int subjectId)
    {
        var subject = await _genericRepository.GetFirstOrDefaultAsync<tblSubject>(x => x.Id == subjectId);

        if (subject != null)
        {
            var contents =
                await _genericRepository.GetAsync<tblContent>(x => x.SubjectId == subject.Id && x.YouTubeLink == "-");

            await _genericRepository.RemoveMultipleEntityAsync(contents);
        }
    }
}