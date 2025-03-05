using Microsoft.Extensions.FileProviders;

namespace Application.Interfaces.Services;

public interface IPdfService
{
    IFileInfo GetCachedPdf(string fileName);
}
