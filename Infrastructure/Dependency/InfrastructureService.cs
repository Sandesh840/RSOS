using Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Data.Implementation.Repositories;
using Microsoft.Extensions.Configuration;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Data.Implementation.Services;
using Data.Persistence.Seed;
using Microsoft.Extensions.DependencyInjection;
using Common.Utilities;
using Hangfire;
using Microsoft.Extensions.Hosting;

namespace Data.Dependency;

public static class InfrastructureService
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        //services.AddScoped<ActionNameLoggingFilter>();
        services.AddScoped<ApplicationDbContext>();
        services.AddHttpClient();
        services.AddMemoryCache();
        services.AddSingleton<LoginDictionary>();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString,
                b => b.MigrationsAssembly("Infrastructure")));

        services.Configure<JwtSettings>(configuration.GetSection("JWT"));
        services.Configure<RsosSettings>(configuration.GetSection("RSOS"));
        services.Configure<HangfireSettings>(configuration.GetSection("Hangfire"));
        services.Configure<VersionSetting>(configuration.GetSection("Version"));
        services.Configure<AuthenticationSettings>(configuration.GetSection("Authentication"));

        services.AddHangfire(config => config.UseSqlServerStorage(connectionString));
        services.AddHangfireServer();
        services.AddTransient<HangfireService>();
        services.AddHostedService<HangfireService>();

        services.AddScoped<IGenericRepository, GenericRepository>();

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IChatBotService, ChatBotService>();
        services.AddScoped<IConfigurationService, ConfigurationService>();
        services.AddScoped<IContentService, ContentService>();
        services.AddScoped<IDbInitializerService, DbInitializerService>();
        services.AddScoped<IEBookService, EBookService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();
        services.AddScoped<IFAQService, FAQService>();
        services.AddScoped<IModelService, ModelService>();
        services.AddScoped<INewsAndAlertService, NewsAndAlertService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ISchemeService, SchemeService>();
        services.AddScoped<IPcpDatesService, PcpDatesService>();
        services.AddScoped<IPCPService, PCPService>();
        services.AddScoped<IPdfService, PDFService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IStudentVideoTrackingService, StudentVideoResponseService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICachePolicy, CachePolicy>();
        services.AddScoped<IBulkUploadService, BulkUploadService>();

        services.AddHostedService<PreloadCacheService>();

        return services;
    }
}