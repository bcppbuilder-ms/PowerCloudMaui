using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerCloud.Services;

public interface IPlatformFilePicker
{
    // 回傳已複製到快取的本機路徑；若使用者取消回傳 null/empty list
    Task<string?> PickFileAsync(string[]? allowedMimeTypes = null);
    Task<List<string>?> PickMultipleFilesAsync(string[]? allowedMimeTypes = null);
}