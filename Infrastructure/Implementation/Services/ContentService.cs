using Application.DTOs.Content;
using Application.DTOs.Dropdown;
using Application.DTOs.Subject;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using ClosedXML.Excel;

namespace Data.Implementation.Services;

public class ContentService : IContentService
{
    private readonly IGenericRepository _genericRepository;

    public ContentService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<List<ContentResponseDTO>> GetAllContents(int? classId, int? subjectId)
    {
        var subject = subjectId.HasValue ? await _genericRepository.GetByIdAsync<tblSubject>(subjectId) : new tblSubject();
        
        var content = await _genericRepository.GetAsync<tblContent>(x => 
            (!classId.HasValue || x.Class == classId) && 
            (!subjectId.HasValue || x.SubjectId == subjectId) && x.IsActive);

        var result = content.OrderBy(x => x.Sequence).Select(x => new ContentResponseDTO()
        {
            Id = x.Id,
            Class = x.Class,
            SubjectId = x.SubjectId,
            SubjectName = subject?.Title ?? "",
            Faculty = x.Faculty ?? "",
            ChapterName = x.ChapterName,
            Description = x.Description,
            PartName = x.PartName,
            ChapterNo = x.ChapterNo,
            PartNo = x.PartNo,
            TimeInSeconds = x.TimeInSeconds,
            YouTubeLink = x.YouTubeLink,
        }).ToList();

        return result;
    }

    public async Task<EContentResponseDTO> GetAllContents(EContentRequestDTO content)
    {
        var subject = await _genericRepository.GetByIdAsync<tblSubject>(content.SubjectId);

        if (content.IsActive)
        {
            var contentList = await _genericRepository.GetAsync<tblContent>(x => 
                x.Class == content.ClassId && 
                x.SubjectId == content.SubjectId);

            if (content.ContentType != 0)
            {
                contentList = content.ContentType == 1 ? 
                    contentList.Where(x => x.YouTubeLink != "-") : 
                    contentList.Where(x => x.YouTubeLink == "-");
            }

            contentList = contentList.OrderBy(x => x.ChapterNo)
                .ThenBy(x => x.PartNo);

            var result = new List<Contents>();
            
            foreach (var contentItem in contentList)
            {
                result.Add(new Contents()
                {
                    Id = contentItem.Id,
                    Class = contentItem.Class,
                    SubjectId = contentItem.SubjectId,
                    SubjectName = subject?.Title ?? "",
                    Faculty = contentItem.Faculty ?? "",
                    ChapterName = contentItem.ChapterName ?? "",
                    Description = contentItem.Description ?? "",
                    PartName = contentItem.PartName ?? "",
                    ChapterNo = contentItem.ChapterNo,
                    PartNo = contentItem.PartNo,
                    TimeInSeconds = contentItem.TimeInSeconds,
                    YouTubeLink = contentItem.YouTubeLink,
                    Sequence = contentItem.Sequence ?? 0,
                    SubjectCode = subject?.SubjectCode ?? 0,
                    IsActive = contentItem.IsActive,
                    UploadedDate = contentItem.CreatedOn.ToString("dd-MM-yyyy h:mm:ss tt"),
                    IsDeletable = content.ContentType == 1 || await _genericRepository.GetFirstOrDefaultAsync<tblQuestion>(z => z.TopicId == contentItem.Id) == null
                });
            }

            var eContent = new EContentResponseDTO()
            {
                SubjectId = subject!.Id,
                Class = content.ClassId,
                SubjectCode = subject.SubjectCode ?? 0,
                SubjectName = subject.Title ?? "",
                ContentsList = result,
                IsActive = true,
                ContentType = content.ContentType
            };
            
            return eContent;
        }
        else
        {
            var contentList = await _genericRepository.GetAsync<tblContentArchive>(x => 
                x.Class == content.ClassId && 
                x.SubjectId == content.SubjectId);

            if (content.ContentType != 0)
            {
                contentList = content.ContentType == 1 ? 
                    contentList.Where(x => x.YouTubeLink != "-") : 
                    [];
            }

            contentList = contentList.OrderBy(x => x.ChapterNo)
                .ThenBy(x => x.PartNo);

            var result = new List<Contents>();
            
            foreach (var contentItem in contentList)
            {
                result.Add(new Contents()
                {
                    Id = contentItem.Id,
                    Class = contentItem.Class,
                    SubjectId = contentItem.SubjectId,
                    SubjectName = subject?.Title ?? "",
                    Faculty = contentItem.Faculty ?? "",
                    ChapterName = contentItem.ChapterName ?? "",
                    Description = contentItem.Description ?? "",
                    PartName = contentItem.PartName ?? "",
                    ChapterNo = contentItem.ChapterNo,
                    PartNo = contentItem.PartNo,
                    TimeInSeconds = contentItem.TimeInSeconds,
                    YouTubeLink = contentItem.YouTubeLink,
                    Sequence = contentItem.Sequence ?? 0,
                    SubjectCode = subject?.SubjectCode ?? 0,
                    IsActive = false,
                    UploadedDate = contentItem.CreatedOn.ToString("dd-MM-yyyy h:mm:ss tt"),
                    IsDeletable = content.ContentType == 1 || await _genericRepository.GetFirstOrDefaultAsync<tblQuestion>(z => z.TopicId == contentItem.Id) == null
                });
            }

            var eContent = new EContentResponseDTO()
            {
                SubjectId = subject!.Id,
                Class = content.ClassId,
                SubjectCode = subject.SubjectCode ?? 0,
                SubjectName = subject.Title ?? "",
                ContentsList = result,
                IsActive = false,
                ContentType = content.ContentType
            };
            
            return eContent;
        }
    }

    public async Task<(bool, string)> UpsertContents(Contents content)
    {
        if (content.YouTubeLink == "-" || content.TimeInSeconds == 0)
        {
            return (false, "Please insert a valid YouTube link and time frame in seconds for the link.");
        }
        
        if (content.Id != 0)
        {
            var contentModel = await _genericRepository.GetFirstOrDefaultAsync<tblContent>(x => x.Id == content.Id);

            if (contentModel == null) return (false, "eContent not found.");
            
            contentModel.Faculty = content.Faculty;
            contentModel.ChapterName = content.ChapterName;
            contentModel.Description = "RSOS Content Model";
            contentModel.PartName = content.PartName;
            contentModel.TimeInSeconds = content.TimeInSeconds;
            contentModel.YouTubeLink = content.YouTubeLink;

            await _genericRepository.UpdateAsync(contentModel);

            return (true, "eContent successfully updated.");
        }
        else
        {
            var existingContent = await _genericRepository.GetFirstOrDefaultAsync<tblContent>(x =>
                x.Class == content.Class && x.SubjectId == content.SubjectId &&
                x.ChapterNo == content.ChapterNo && x.PartNo == content.PartNo);

            if (existingContent != null)
            {
                return (false, "An existing eContent on the same subject and class exists with the same chapter number and part number");
            }

            var contentModel = new tblContent()
            {
                Faculty = content.Faculty,
                ChapterName = content.ChapterName,
                ChapterNo = content.ChapterNo,
                Description = "RSOS Content Model",
                PartName = content.PartName,
                PartNo = content.PartNo,
                TimeInSeconds = content.TimeInSeconds,
                YouTubeLink = content.YouTubeLink,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
                SubjectId = content.SubjectId,
                Class = content.Class,
                Sequence = content.ChapterNo * 1000 + content.PartNo,
                IsActive = true,
            };

            await _genericRepository.InsertAsync(contentModel);

            return (true, "eContent successfully added.");
        }
    }

    public async Task<(bool, bool)> UpdateContentStatus(int contentId)
    {
        var content = await _genericRepository.GetFirstOrDefaultAsync<tblContent>(x => x.Id == contentId);

        if (content == null) return (false, false);

        if (content.YouTubeLink == "-")
        {
            var questionPaper = await _genericRepository.GetFirstOrDefaultAsync<tblQuestion>(x => x.TopicId == content.Id);

            if (questionPaper != null) return (false, false);
        }

        var contentArchive = new tblContentArchive()
        {
            SubjectId = content.SubjectId,
            Class = content.Class,
            Sequence = content.Sequence,
            Description = content.Description,
            PartName = content.PartName,
            ChapterNo = content.ChapterNo,
            PartNo = content.PartNo,
            TimeInSeconds = content.TimeInSeconds,
            YouTubeLink = content.YouTubeLink,
            CreatedBy = content.CreatedBy,
            CreatedOn = DateTime.Now,
            ChapterName = content.ChapterName,
            Faculty = content.Faculty,
            IsActive = true
        };

        await _genericRepository.InsertAsync(contentArchive);
        
        await _genericRepository.DeleteAsync(content);

        return (true, !content.IsActive);
    }

    public async Task<List<SelectListModel>> GetSubjectsByClass(int classId)
    {
        var subjects = await _genericRepository.GetAsync<tblSubject>(x => x.Class == classId);

        var result = subjects.Select(x => new SelectListModel()
        {
            Id = x.Id,
            Value = $"({x.SubjectCode}) {x.Title}"
        }).ToList();

        return result;
    }

    public async Task<SubjectResponseDTO> GetSubjectById(int subjectId)
    {
        var subject = await _genericRepository.GetByIdAsync<tblSubject>(subjectId);

        return new SubjectResponseDTO()
        {
            Id = subject!.Id,
            Class = subject.Class ?? 0,
            SubjectCode = subject.SubjectCode,
            Title = subject.Title,
            TitleInHindi = subject.TitleInHindi
        };
    }
    
    public async Task<Contents> GetContentById(int contentId)
    {
        var content = await _genericRepository.GetFirstOrDefaultAsync<tblContent>(x => x.Id == contentId);

        var subject = await _genericRepository.GetByIdAsync<tblSubject>(content!.SubjectId);
        
        var result = new Contents()
        {
            Id = content.Id,
            Class = content.Class,
            SubjectId = content.SubjectId,
            SubjectName = subject!.Title,
            SubjectCode = subject.SubjectCode ?? 0,
            ChapterName = content.ChapterName,
            ChapterNo = content.ChapterNo,
            PartNo = content.PartNo,
            TimeInSeconds = content.TimeInSeconds,
            Sequence = content.Sequence ?? 0,
            PartName = content.PartName,
            Faculty = content.Faculty ?? "",
            YouTubeLink = content.YouTubeLink,
            ContentType = content.YouTubeLink == "-" ? 2 : 1
        };

        return result;
    }

    public async Task<EContentUploadDTO> GetContentsDetails(int subjectId)
    {
        var subject = await _genericRepository.GetByIdAsync<tblSubject>(subjectId);

        if (subject == null) return new EContentUploadDTO();

        var result = new EContentUploadDTO()
        {
            ClassId = subject.Class ?? 10,
            SubjectCode = subject.SubjectCode ?? 0,
            Subject = subject.Title
        };

        return result;
    }
    
    public async Task<(bool, string)> IsUploadedSheetValid(EContentUploadDTO content)
    {
        try
        {
            var subject = await _genericRepository.GetFirstOrDefaultAsync<tblSubject>(x => 
                x.SubjectCode == content.SubjectCode);

            if(subject == null) return (false, "Could not find a valid subject based on the entry.");
            
            using var workbook = new XLWorkbook(content.Contents.OpenReadStream());
       
            var worksheet = workbook.Worksheet(1);
            
            var contentsList = worksheet.Rows().Skip(1)
                .Select(row => new 
                {
                    ClassId = row.Cell(1).GetValue<int?>() ?? 0, 
                    SubjectCode = row.Cell(2).GetValue<int?>() ?? 0, 
                    SubjectName = row.Cell(3).GetValue<string?>() ?? "", 
                    Faculty = row.Cell(4).GetValue<string?>() ?? "", 
                    ChapterNo = row.Cell(5).GetValue<int?>() ?? 0, 
                    ChapterName = row.Cell(6).GetValue<string?>()?.Trim() ?? "", 
                    PartNo = row.Cell(7).GetValue<int?>() ?? 0, 
                    PartName = row.Cell(8).GetValue<string?>()?.Trim() ?? "", 
                    TimeInSeconds = row.Cell(9).GetValue<int?>() ?? 0,
                    YouTubeLink = row.Cell(10).GetValue<string?>()?.Trim() ?? "",
                }).ToList();
            
            var isInValid = contentsList.Any(x =>
                x.ClassId == 0 || x.SubjectCode == 0 || x.ChapterNo == 0 || x.PartNo == 0 || x.TimeInSeconds == 0 ||
                string.IsNullOrEmpty(x.ChapterName) || string.IsNullOrEmpty(x.PartName) || string.IsNullOrEmpty(x.Faculty) || string.IsNullOrEmpty(x.YouTubeLink));

            if (isInValid)
            {
                return (false, "Please do not leave any fields empty.");
            }
            
            foreach (var item in contentsList)
            {
                if (item.ClassId != content.ClassId)
                    return (false,
                        "Please insert the same value of class for all the columns in the following sheet.");

                if (item.SubjectCode != subject.SubjectCode)
                    return (false,
                        "Please insert the same value of subject code for all the columns in the following sheet.");
                
                if (item.SubjectName != subject.Title)
                    return (false,
                        "Please insert the same value of subject code for all the columns in the following sheet.");
            }
            
            return (true, "Sheet successfully validated.");
        }
        catch (Exception e)
        {
            return (false, "An exception occured while processing your request, please upload a valid file");
        }
    }

    public async Task<(int, int)> UploadContents(EContentUploadDTO content)
    {
        try
        {
            var subject = await _genericRepository.GetFirstOrDefaultAsync<tblSubject>(x => 
                x.SubjectCode == content.SubjectCode);

            if(subject == null) return (0, 0);
            
            using var workbook = new XLWorkbook(content.Contents.OpenReadStream());
       
            var worksheet = workbook.Worksheet(1);
            
            var contentsList = worksheet.Rows().Skip(1)
                .Select(row => new 
                {
                    ClassId = row.Cell(1).GetValue<int?>() ?? 0, 
                    SubjectCode = row.Cell(2).GetValue<int?>() ?? 0, 
                    SubjectName = row.Cell(3).GetValue<string?>() ?? "", 
                    Faculty = row.Cell(4).GetValue<string?>() ?? "", 
                    ChapterNo = row.Cell(5).GetValue<int?>() ?? 0, 
                    ChapterName = row.Cell(6).GetValue<string?>()?.Trim() ?? "", 
                    PartNo = row.Cell(7).GetValue<int?>() ?? 0, 
                    PartName = row.Cell(8).GetValue<string?>()?.Trim() ?? "", 
                    TimeInSeconds = row.Cell(9).GetValue<int?>() ?? 0,
                    YouTubeLink = row.Cell(10).GetValue<string?>()?.Trim() ?? "",
                }).ToList();

            var index = 0;
            
            var totalContents = contentsList.Count;
            
            foreach (var contentModule in contentsList)
            {
                var existingContent = await _genericRepository.GetFirstOrDefaultAsync<tblContent>(x =>
                    x.SubjectId == subject.Id && x.Class == content.ClassId && x.PartNo == contentModule.PartNo &&
                    x.ChapterNo == contentModule.ChapterNo);

                if (existingContent != null) continue;
                {
                    var contentModel = new tblContent()
                    {
                        SubjectId = subject.Id,
                        ChapterName = contentModule.ChapterName,
                        ChapterNo = contentModule.ChapterNo,
                        PartNo = contentModule.PartNo,
                        CreatedBy = 1,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        Class = contentModule.ClassId,
                        Sequence = contentModule.ChapterNo * 1000 + contentModule.PartNo,
                        Description = $"RSOS Class {contentModule.ClassId} {subject.Title}",
                        Faculty = contentModule.Faculty,
                        PartName = contentModule.PartName,
                        TimeInSeconds = contentModule.TimeInSeconds,
                        YouTubeLink = contentModule.YouTubeLink
                    };

                    await _genericRepository.InsertAsync(contentModel);

                    index++;
                }
            }

            return (index, totalContents);
        }
        catch (Exception e)
        {
            return (0, 0);
        }
    }

    public async Task<List<ContentChapterDTO>> GetAllChapter(int? classId, int? subjectId,string? youTubeLink)
    {
        var subject = subjectId.HasValue ? await _genericRepository.GetByIdAsync<tblSubject>(subjectId) : new tblSubject();

        var content = await _genericRepository.GetAsync<tblContent>(x =>
            (!classId.HasValue || x.Class == classId) &&
            (!subjectId.HasValue || x.SubjectId == subjectId) && x.IsActive && (youTubeLink == null || x.YouTubeLink == youTubeLink));

        var result = content.OrderBy(x => x.Sequence).Select(x => new ContentChapterDTO()
        {
            Id = x.Id,
            Class = x.Class,
            SubjectId = x.SubjectId,
            SubjectName = subject?.Title ?? "",
            ChapterName = x.ChapterName,
        }).ToList();

        return result;
    }

}