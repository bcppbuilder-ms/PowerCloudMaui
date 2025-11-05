using PowerCloud.NasHttp;
using PowerCloud.ViewModels;

namespace PowerCloud.Views.FileManagement;

public partial class Popup_AddFile : CommunityToolkit.Maui.Views.Popup
{
	public Popup_AddFile(MainNasFileViewModel prmMvm, FileResult photo)
	{
		InitializeComponent();

        fileInDevie = photo;
        mvm = prmMvm;
        NativeFileName.Text = photo.FileName;
    }

    MainNasFileViewModel? mvm;
    FileResult fileInDevie;


    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }

    private async void Upload_Clicked(object sender, EventArgs e)
    {
        ActIndicator.IsRunning = true;

        string uploadFileName = NewName.Text;
        if (string.IsNullOrEmpty(uploadFileName))
            uploadFileName = NativeFileName.Text;

        FileInfo finfo = new FileInfo(fileInDevie.FullPath);

        long fileSize = finfo.Length;
        DateTime lwDt = finfo.LastWriteTime;
        DateTime createDt = finfo.CreationTime;
        string mime = fileInDevie.ContentType;
        if (string.IsNullOrEmpty(NativeFileName.Text))
            return;
        else
        {
            string fileExt = Path.GetExtension(fileInDevie.FileName);
            if (fileExt == ".jpg" && !uploadFileName.EndsWith(".jpg"))
                uploadFileName += ".jpg";
            else if (fileExt == ".mp4" && !uploadFileName.EndsWith(".mp4"))
                uploadFileName += ".mp4";
        }


        NE201FileManager ne201 = NE201FileManager.FileManagerFactory(App.PC2ViewModel.UserSelected);
        if (await ne201.NE201FileUpload(fileInDevie, mvm.PrevPath, uploadFileName))
        {
            NASFileViewModel item = new NASFileViewModel()
            {
                LastWriteTime = lwDt.ToString("R"),
                CreationTime = createDt.ToString("R"),
                Name = uploadFileName,
                PathName = mvm.PrevPath,
                MimeType = mime,
                Size = fileSize,
                UsingThumb = mvm.UseThumbNail,
                CanMultiSelect = true
            };
            mvm.NASFiles.Insert(0, item);


            ////await mvm.ListView_RefreshFolder();
            //await mvm.readAllFileList(mvm.PrevPath, mvm.NASFiles.Count + 1);
            ////await Task.Delay(2000);
        }
        else
        {
            await AppShell.Current.CurrentPage.DisplayAlert("Failed", "File Upload is failed.", "Finish");
        }
        ActIndicator.IsRunning = false;

        await CloseAsync();
    }
}