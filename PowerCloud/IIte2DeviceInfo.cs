using PowerCloud.ViewModels;

namespace PowerCloud
{
    public interface IIte2DeviceInfo2
    {
        Task<string> GetExternalPath(AccountViewModel user);
        Task DownloadToDevice(MainNasFileViewModel mvm);
        Task<Stream?> GetDownloadStream(NASFileViewModel file);

        void GetPhotoLibrary(MainNasFileViewModel prmMvm);
        List<AlbumItem> GetNativeFileIDs();
        Task<bool> UploadMediaFile(AlbumItem item);
        Task<byte[]> GetThumbnail(AlbumItem item);
        Task<bool> CopyToLocal(AlbumItem source, string target);

        string SelectedAccessToken { get; set; }
        string SelectedRefreshToken { get; set; }
        string SelectedNasAddress { get; set; }
    }
}