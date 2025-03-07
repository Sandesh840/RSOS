using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Model.Models;

namespace Data.Persistence;

public partial class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<AggregatedCounter> AggregatedCounters { get; set; }

    public virtual DbSet<Counter> Counters { get; set; }

    public virtual DbSet<Hash> Hashes { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<JobParameter> JobParameters { get; set; }

    public virtual DbSet<JobQueue> JobQueues { get; set; }

    public virtual DbSet<List> Lists { get; set; }

    public virtual DbSet<Schema> Schemas { get; set; }

    public virtual DbSet<Server> Servers { get; set; }

    public virtual DbSet<Set> Sets { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<tblChatBotMessage> tblChatBotMessages { get; set; }

    public virtual DbSet<tblCommon> tblCommons { get; set; }

    public virtual DbSet<tblCommonsArchive> tblCommonsArchives { get; set; }

    public virtual DbSet<tblContent> tblContents { get; set; }

    public virtual DbSet<tblContentArchive> tblContentArchives { get; set; }

    public virtual DbSet<tblEbook> tblEbooks { get; set; }

    public virtual DbSet<tblEbookArchive> tblEbookArchives { get; set; }

    public virtual DbSet<tblEnrollmentStatus> tblEnrollmentStatuses { get; set; }

    public virtual DbSet<tblExceptionLog> tblExceptionLogs { get; set; }

    public virtual DbSet<tblFAQ> tblFAQs { get; set; }

    public virtual DbSet<tblNewsAndAlert> tblNewsAndAlerts { get; set; }

    public virtual DbSet<tblNotification> tblNotifications { get; set; }
    public virtual DbSet<tblScheme> tblScheme { get; set; }
    public virtual DbSet<tblSchemeFile> tblSchemeFile { get; set; }

    public virtual DbSet<tblPCPDate> tblPCPDates { get; set; }

    public virtual DbSet<tblQuestion> tblQuestions { get; set; }

    public virtual DbSet<tblQuestionPaperSheet> tblQuestionPaperSheets { get; set; }

    public virtual DbSet<tblQuestionsArchive> tblQuestionsArchives { get; set; }

    public virtual DbSet<tblStudentLoginDetail> tblStudentLoginDetails { get; set; }

    public virtual DbSet<tblStudentLoginHistory> tblStudentLoginHistories { get; set; }

    public virtual DbSet<tblStudentResponse> tblStudentResponses { get; set; }

    public virtual DbSet<tblStudentScore> tblStudentScores { get; set; }

    public virtual DbSet<tblStudentVideoTracking> tblStudentVideoTrackings { get; set; }

    public virtual DbSet<tblSubject> tblSubjects { get; set; }
    public virtual DbSet<tblStudentDetails> tblStudentDetails { get; set; }

    public virtual DbSet<tblUser> tblUsers { get; set; }
    public virtual DbSet<tblSession> tblSession { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AggregatedCounter>(entity =>
        {
            entity.HasKey(e => e.Key).HasName("PK_HangFire_CounterAggregated");

            entity.ToTable("AggregatedCounter", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_AggregatedCounter_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Counter>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Id }).HasName("PK_HangFire_Counter");

            entity.ToTable("Counter", "HangFire");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Hash>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Field }).HasName("PK_HangFire_Hash");

            entity.ToTable("Hash", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Hash_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Field).HasMaxLength(100);
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_HangFire_Job");

            entity.ToTable("Job", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Job_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.HasIndex(e => e.StateName, "IX_HangFire_Job_StateName").HasFilter("([StateName] IS NOT NULL)");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            entity.Property(e => e.StateName).HasMaxLength(20);
        });

        modelBuilder.Entity<JobParameter>(entity =>
        {
            entity.HasKey(e => new { e.JobId, e.Name }).HasName("PK_HangFire_JobParameter");

            entity.ToTable("JobParameter", "HangFire");

            entity.Property(e => e.Name).HasMaxLength(40);

            entity.HasOne(d => d.Job).WithMany(p => p.JobParameters)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_HangFire_JobParameter_Job");
        });

        modelBuilder.Entity<JobQueue>(entity =>
        {
            entity.HasKey(e => new { e.Queue, e.Id }).HasName("PK_HangFire_JobQueue");

            entity.ToTable("JobQueue", "HangFire");

            entity.Property(e => e.Queue).HasMaxLength(50);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FetchedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<List>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Id }).HasName("PK_HangFire_List");

            entity.ToTable("List", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_List_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Schema>(entity =>
        {
            entity.HasKey(e => e.Version).HasName("PK_HangFire_Schema");

            entity.ToTable("Schema", "HangFire");

            entity.Property(e => e.Version).ValueGeneratedNever();
        });

        modelBuilder.Entity<Server>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_HangFire_Server");

            entity.ToTable("Server", "HangFire");

            entity.HasIndex(e => e.LastHeartbeat, "IX_HangFire_Server_LastHeartbeat");

            entity.Property(e => e.Id).HasMaxLength(200);
            entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
        });

        modelBuilder.Entity<Set>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Value }).HasName("PK_HangFire_Set");

            entity.ToTable("Set", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Set_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.HasIndex(e => new { e.Key, e.Score }, "IX_HangFire_Set_Score");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Value).HasMaxLength(256);
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => new { e.JobId, e.Id }).HasName("PK_HangFire_State");

            entity.ToTable("State", "HangFire");

            entity.HasIndex(e => e.CreatedAt, "IX_HangFire_State_CreatedAt");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(20);
            entity.Property(e => e.Reason).HasMaxLength(100);

            entity.HasOne(d => d.Job).WithMany(p => p.States)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_HangFire_State_Job");
        });

        modelBuilder.Entity<tblChatBotMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblChatB__3214EC07D950FF27");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LanguageId).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<tblCommon>(entity =>
        {
            entity.HasKey(e => new { e.CommonId, e.Flag, e.LanguageId }).HasName("PK_Common");

            entity.HasIndex(e => new { e.Flag, e.IsActive }, "Idx_Commons").IsDescending(false, true);

            entity.Property(e => e.Flag).HasDefaultValue(1);
            entity.Property(e => e.CorrectAnswer).HasDefaultValue(0);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Score).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<tblCommonsArchive>(entity =>
        {
            entity.HasKey(e => new { e.CommonId, e.Flag, e.LanguageId }).HasName("PK_CommonArchive");

            entity.ToTable("tblCommonsArchive");

            entity.Property(e => e.Flag).HasDefaultValue(1);
            entity.Property(e => e.CorrectAnswer).HasDefaultValue(0);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Score).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<tblContent>(entity =>
        {
            entity.HasKey(e => new { e.Class, e.SubjectId, e.ChapterNo, e.PartNo }).HasName("PK_Content");

            entity.HasIndex(e => new { e.Class, e.SubjectId }, "Idx_Content");

            entity.Property(e => e.ChapterName).HasMaxLength(200);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Faculty).HasMaxLength(200);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PartName).HasMaxLength(200);
            entity.Property(e => e.YouTubeLink).IsUnicode(false);
        });

        modelBuilder.Entity<tblContentArchive>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblConte__3214EC0729D280F6");

            entity.ToTable("tblContentArchive");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.YouTubeLink).IsUnicode(false);
        });

        modelBuilder.Entity<tblEbook>(entity =>
        {
            entity.HasKey(e => new { e.Class, e.CodeNo, e.Volume }).HasName("PK_Ebook");

            entity.HasIndex(e => new { e.Class, e.CodeNo }, "Idx_Ebook");

            entity.Property(e => e.Volume)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.NameOfBook).HasMaxLength(150);
        });

        modelBuilder.Entity<tblEbookArchive>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblEbook__3214EC073C6733F0");

            entity.ToTable("tblEbookArchive");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.NameOfBook).HasMaxLength(150);
            entity.Property(e => e.Volume)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<tblEnrollmentStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_EnrollmentStatus");

            entity.ToTable("tblEnrollmentStatus");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Reason).HasMaxLength(200);
        });

        modelBuilder.Entity<tblExceptionLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ExceptionLogs");

            entity.Property(e => e.DateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<tblFAQ>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_FAQ");

            entity.HasIndex(e => e.IsActive, "Idx_FAQ").IsDescending();

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Sequence).HasDefaultValue(1);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<tblNewsAndAlert>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_NewsAndAlerts");

            entity.HasIndex(e => new { e.IsActive, e.ValidTill }, "Idx_NewsAndAlerts").IsDescending();

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Header).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ValidFrom).HasColumnType("datetime");
        });

        modelBuilder.Entity<tblNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Notification");

            entity.HasIndex(e => new { e.IsActive, e.ValidTill }, "Idx_Notifications").IsDescending();

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Header).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UploadedFileName).HasMaxLength(200);
            entity.Property(e => e.UploadedFileUrl).HasMaxLength(200);
            entity.Property(e => e.ValidFrom).HasColumnType("datetime");
        });


        modelBuilder.Entity<tblScheme>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Scheme");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Header).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.URL).HasMaxLength(200);
            //entity.Property(e => e.IsTriggered).HasDefaultValue(false);

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<tblSchemeFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_SchemeFile");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FileUrl).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.scheme)
                  .WithMany(p => p.SchemeFile)
                  .HasForeignKey(d => d.FileId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_SchemeFile_Scheme");
        });


        modelBuilder.Entity<tblPCPDate>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK_PCPDate")
                .IsClustered(false);

            entity.HasIndex(e => e.Id, "Idx_PCPDates")
                .IsDescending()
                .IsClustered();

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<tblQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Question");

            entity.HasIndex(e => new { e.Class, e.SubjectId }, "Idx_Questions");

            entity.HasIndex(e => e.Flag, "Idx_Questions_Flag");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Flag).HasDefaultValue(1);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsMandatory).HasDefaultValue(true);
        });

        modelBuilder.Entity<tblQuestionPaperSheet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_QuestionSheet");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UploadedFileName).HasMaxLength(200);
            entity.Property(e => e.UploadedFileUrl).HasMaxLength(200);
        });

        modelBuilder.Entity<tblQuestionsArchive>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_QuestionArchive");

            entity.ToTable("tblQuestionsArchive");

            entity.HasIndex(e => new { e.Class, e.SubjectId }, "Idx_QuestionsArchive");

            entity.HasIndex(e => e.Flag, "Idx_QuestionsArchive_Flag");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Flag).HasDefaultValue(1);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsMandatory).HasDefaultValue(true);
        });

        modelBuilder.Entity<tblStudentLoginDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_StudentLoginDetails");

            entity.HasIndex(e => new { e.SSOID, e.LoginTime }, "Idx_StudentLoginDetails").IsDescending(false, true);

            entity.Property(e => e.DeviceRegistrationToken).HasMaxLength(500);
            entity.Property(e => e.LoginTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SSOID).HasMaxLength(200);
        });

        modelBuilder.Entity<tblStudentLoginHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_StudentLoginHistory");

            entity.ToTable("tblStudentLoginHistory");

            entity.HasIndex(e => new { e.SSOID, e.LastAccessedTime }, "Idx_StudentLoginHistory").IsDescending(false, true);

            entity.Property(e => e.DateOfBirth).HasMaxLength(100);
            entity.Property(e => e.Enrollment).HasMaxLength(100);
            entity.Property(e => e.LastAccessedTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SSOID).HasMaxLength(200);
        });

        modelBuilder.Entity<tblStudentResponse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_StudentResponse");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<tblStudentScore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_StudentScore");

            entity.ToTable("tblStudentScore");

            entity.HasIndex(e => e.StudentId, "Idx_StudentScore");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.GUID).HasMaxLength(1000);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<tblStudentVideoTracking>(entity =>
        {
            entity.HasKey(e => new { e.SubjectId, e.Class, e.StudentId, e.VideoId }).HasName("PK_StudentVideoTracking");

            entity.ToTable("tblStudentVideoTracking");

            entity.HasIndex(e => new { e.SubjectId, e.Class, e.StudentId, e.VideoId }, "Idx_StudentVideoTracking");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PercentageCompleted).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<tblSubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Subject");

            entity.HasIndex(e => e.Class, "Idx_Subjects");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.TitleInHindi).HasMaxLength(200);
        });

        modelBuilder.Entity<tblUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Users");

            entity.Property(e => e.Password)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        //modelBuilder.Entity<tblStudentDetails>(entity =>
        //{
        //    entity.HasKey(e => e.Id).HasName("PK_StudentDetails");

        //    entity.Property(e => e.Id).ValueGeneratedNever();
        //    entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
        //});

        OnModelCreatingPartial(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
            
        optionsBuilder.UseSqlServer(connectionString, builder =>
        {
            builder.EnableRetryOnFailure();
        });

        base.OnConfiguring(optionsBuilder);
    }
    
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
