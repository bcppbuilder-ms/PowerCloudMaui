using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerCloud.Services;

public interface IPlatformFilePicker
{
    // �^�Ǥw�ƻs��֨����������|�F�Y�ϥΪ̨����^�� null/empty list
    Task<string?> PickFileAsync(string[]? allowedMimeTypes = null);
    Task<List<string>?> PickMultipleFilesAsync(string[]? allowedMimeTypes = null);
}