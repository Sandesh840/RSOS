using Application.Interfaces.Services;
using Application.Interfaces.Repositories;
using Application.DTOs.Tracking;

namespace Data.Implementation.Services;

public class StudentVideoResponseService : IStudentVideoTrackingService
{
    private readonly IGenericRepository _genericRepository;

    public StudentVideoResponseService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<List<StudentVideoTrackingResponseDTO>> GetStudentVideoTrackingByStudentId(int studentId)
    {
        var result = await _genericRepository.GetAsync<tblStudentVideoTracking>(x => x.StudentId == studentId && x.IsActive);

        return result.Select(x => new StudentVideoTrackingResponseDTO()
        {
            Id = x.Id,
            SubjectId = x.SubjectId,
            VideoId = x.VideoId,
            StudentId = x.StudentId,
            Class = x.Class,
            IsCompleted = x.IsCompleted ? 1 : 0,
            PlayTimeInSeconds = x.PlayTimeInSeconds,
            PercentageCompleted = x.PercentageCompleted,
            VideoDurationInSeconds = x.VideoDurationInSeconds
        }).ToList();
    }

    public async Task UpsertStudentVideoTracking(StudentVideoTrackingRequestDTO studentVideoTracking)
    {
        var videoTracking = await _genericRepository.GetFirstOrDefaultAsync<tblStudentVideoTracking>(x => 
            x.SubjectId == studentVideoTracking.SubjectId && x.Class == studentVideoTracking.Class && 
            x.StudentId == studentVideoTracking.StudentId && x.VideoId == studentVideoTracking.VideoId);

        var playTimeRatio = (decimal)studentVideoTracking.PlayTimeInSeconds! / (decimal)studentVideoTracking.VideoDurationInSeconds!;

        if (videoTracking == null)
        {
            var studentVideoTrackingModel = new tblStudentVideoTracking()
            {
                SubjectId = studentVideoTracking.SubjectId,
                VideoId = studentVideoTracking.VideoId,
                StudentId = studentVideoTracking.StudentId,
                Class = studentVideoTracking.Class,
                IsCompleted = playTimeRatio >= (decimal)0.9,
                PlayTimeInSeconds = studentVideoTracking.PlayTimeInSeconds,
                PercentageCompleted = studentVideoTracking.PercentageCompleted,
                VideoDurationInSeconds = studentVideoTracking.VideoDurationInSeconds,
                IsActive = true,
                CreatedBy = studentVideoTracking.StudentId,
                CreatedOn = DateTime.Now, 
            };

            await _genericRepository.InsertAsync(studentVideoTrackingModel);
        }
        else
        {
            videoTracking.PlayTimeInSeconds = studentVideoTracking.PlayTimeInSeconds;
            videoTracking.PercentageCompleted = studentVideoTracking.PercentageCompleted;
            videoTracking.VideoDurationInSeconds = studentVideoTracking.VideoDurationInSeconds;
            videoTracking.IsCompleted = playTimeRatio >= (decimal)0.9;

            await _genericRepository.UpdateAsync(videoTracking);
        }
    }
}