using Application.DTOs.EBook;
using Application.DTOs.EContentBooks;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Data.Implementation.Services;

public class EBookService : IEBookService
{
    private readonly IGenericRepository _genericRepository;

    public EBookService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<List<EBookResponseDTO>> GetAllEBooks(int? classId, int? subjectCode, string? volume)
    {
        var ebooks = await _genericRepository.GetAsync<tblEbook>(x =>
            (!classId.HasValue || x.Class == classId) &&
            (!subjectCode.HasValue || x.CodeNo == subjectCode) &&
            (string.IsNullOrEmpty(volume) || x.Volume == volume) && x.IsActive);

        return ebooks.OrderBy(x => x.Sequence).Select(x => new EBookResponseDTO()
        {
            Id = x.Id,
            Code = x.CodeNo,
            Class = x.Class,
            Volume = x.Volume,
            FileName = x.FileName ?? "No PDF",
            NameOfBook = x.NameOfBook ?? ""
        }).ToList();
    }
    
    public async Task<List<EContentBookResponseDTO>> GetAllEBooks(int classId, bool isActive)
    {
        var allSubjects = await _genericRepository.GetAsync<tblSubject>(x=> x.Class == classId);

        if (isActive)
        {
            var ebooks = await _genericRepository.GetAsync<tblEbook>(
                x => (x.Class == classId) && x.IsActive == isActive);

            var combinedList = allSubjects
                .Select(subject => new
                {
                    Subject = subject,
                    Ebooks = ebooks.Where(ebook => ebook.CodeNo == subject.Id).ToList()
                })
                .SelectMany(result => result.Ebooks.DefaultIfEmpty(), (result, ebook) => new EContentBookResponseDTO
                {
                    Id = ebook?.Id ?? 0,
                    SubjectName = result?.Subject.Title ?? "",
                    NameOfBook = ebook?.NameOfBook ?? "-",
                    Volume = ebook?.Volume ?? "-",
                    FileName = ebook?.FileName ?? "-",
                    IsActive = ebook?.IsActive ?? false,
                    UploadedDate = ebook?.CreatedOn.ToString("dd-MM-yyyy h:mm:ss tt") ?? ""
                }).ToList();

            return combinedList;
        }
        else
        {
            var ebooks = await _genericRepository.GetAsync<tblEbookArchive>(
                x => x.Class == classId);

            var combinedList = allSubjects
                .Select(subject => new
                {
                    Subject = subject,
                    Ebooks = ebooks.Where(ebook => ebook.CodeNo == subject.Id).ToList()
                })
                .SelectMany(result => result.Ebooks.DefaultIfEmpty(), (result, ebook) => new EContentBookResponseDTO
                {
                    Id = ebook?.Id ?? 0,
                    SubjectName = result.Subject.Title ?? "",
                    NameOfBook = ebook?.NameOfBook ?? "-",
                    Volume = ebook?.Volume ?? "-",
                    FileName = ebook?.FileName ?? "-",
                    IsActive = false,
                    UploadedDate = ebook?.CreatedOn.ToString("dd-MM-yyyy h:mm:ss tt") ?? ""
                }).ToList();

            return combinedList;   
        }
        
    }

    public async Task<bool> UploadEContentBook(EContentBookRequestDTO eBookRequest)
    {
        if (eBookRequest.Id == 0)
        {
            var existingEBook = await _genericRepository.GetFirstOrDefaultAsync<tblEbook>(x =>
                x.CodeNo == eBookRequest.SubjectId && x.Class == eBookRequest.ClassId && x.Volume == eBookRequest.Volume);

            if (existingEBook != null) return false;
            
            var model = new tblEbook
            {
                CodeNo = eBookRequest.SubjectId ?? 0,
                NameOfBook = eBookRequest.NameOfBook,
                Volume = eBookRequest.Volume ?? "",
                FileName = eBookRequest.EBookFile?.FileName ,
                Class = eBookRequest.ClassId ?? 0,
                IsActive = true,
                CreatedBy = eBookRequest.CreatedBy ?? 1,
                CreatedOn = DateTime.Now,
            };

            await _genericRepository.InsertAsync(model);
        }
        else
        {
            var eBook = await _genericRepository.GetFirstOrDefaultAsync<tblEbook>(x => x.Id == eBookRequest.Id);

            if (eBook == null) return false;

            eBook.NameOfBook = eBookRequest.NameOfBook;

            if (eBookRequest.EBookFile != null)
            {
                eBook.FileName = eBookRequest.EBookFile?.FileName;
            }

            await _genericRepository.UpdateAsync(eBook);
        }
        
        return true;
    }

    public async Task<EContentBookRequestDTO> GetEBookById(int ebookId)
    {
        var eBook = await _genericRepository.GetFirstOrDefaultAsync<tblEbook>(x => x.Id == ebookId);

        var result = new EContentBookRequestDTO()
        {
            Id = eBook!.Id,
            SubjectId = eBook.CodeNo,
            NameOfBook = eBook.NameOfBook,
            Volume = eBook.Volume ?? "",
            ClassId = eBook.Class,
            SubjectName = (await _genericRepository.GetFirstOrDefaultAsync<tblSubject>(x => x.Id == eBook.CodeNo))!
                .Title
        };

        return result;
    }
    
    public async Task DeleteEBook(int ebookId)
    {
        var ebook = await _genericRepository.GetFirstOrDefaultAsync<tblEbook>(x => x.Id == ebookId);

        if (ebook != null)
        {
            var ebookArchive = new tblEbookArchive()
            {
                Class = ebook.Class,
                Volume = ebook.Volume,
                FileName = ebook.FileName,
                IsActive = true,
                CreatedBy = ebook.CreatedBy,
                CreatedOn = DateTime.Now,
                NameOfBook = ebook.NameOfBook,
                Sequence = ebook.Sequence,
                CodeNo = ebook.CodeNo
            };
            
            await _genericRepository.InsertAsync(ebookArchive);
            
            await _genericRepository.DeleteAsync(ebook);
        }
    }
}