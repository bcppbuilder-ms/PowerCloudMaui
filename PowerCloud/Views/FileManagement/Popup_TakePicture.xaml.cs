using GameKit;
using PowerCloud.ViewModels;

namespace PowerCloud.Views.FileManagement;

public partial class Popup_TakePicture : ContentPage
{
	public Popup_TakePicture(MainNasFileViewModel mvmPrm)
	{
        InitializeComponent();

        mvm = mvmPrm;
    }

    MainNasFileViewModel mvm;

    private async void Remove_page_Tapped(object sender, System.EventArgs e)
    {
        //await Navigation.RemovePopupPageAsync(this);
    }

    private async void takePhoto_Clicked(object sender, System.EventArgs e)
    {
        if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
        {
            await DisplayAlert("No Camera", ":( No camera available.", "OK");
            return;
        }

        MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
        {
            Directory = "Test",
            SaveToAlbum = true,
            CompressionQuality = 75,
            CustomPhotoSize = 50,
            PhotoSize = PhotoSize.MaxWidthHeight,
            MaxWidthHeight = 2000,
            DefaultCamera = CameraDevice.Front
        });

        if (file == null)
            return;

        localFileName = file.Path;

        image.Source = ImageSource.FromStream(() =>
        {
            var stream = file.GetStream();
            file.Dispose();
            return stream;
        });

        FileResult x = new FileResult(localFileName);

        await Navigation.PushPopupAsync(new FileManagement_Popup_AddFile(mvm, x));
        await Navigation.RemovePopupPageAsync(this);
    }

    string localFileName;

    //private async void pickPhoto_Clicked(object sender, System.EventArgs e)
    //{
    //    if (!CrossMedia.Current.IsPickPhotoSupported)
    //    {
    //        await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
    //        return;
    //    }
    //    MediaFile file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
    //    {
    //        PhotoSize = PhotoSize.Medium,

    //    });

    //    if (file == null)
    //        return;

    //    localFileName = file.Path;

    //    image.Source = ImageSource.FromStream(() =>
    //    {
    //        var stream = file.GetStream();
    //        file.Dispose();
    //        return stream;
    //    });

    //    FileResult x = new FileResult(localFileName);

    //    await Navigation.PushPopupAsync(new FileManagement_Popup_AddFile(mvm, x));
    //    await Navigation.RemovePopupPageAsync(this);
    //}

    private async void takeVideo_Clicked(object sender, System.EventArgs e)
    {
        if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported)
        {
            await DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
            return;
        }

        string newName = nextFileName("video.mp4");

        MediaFile file = await CrossMedia.Current.TakeVideoAsync(new StoreVideoOptions
        {
            Name = newName,
            Directory = "DefaultVideos",
        });

        if (file == null)
            return;

        localFileName = file.Path;

        file.Dispose();

        string longs = Path.Combine(FileSystem.AppDataDirectory, "Movies", "DefaultVideos");
        if (!Directory.Exists(longs))
        {
            Directory.CreateDirectory(longs);
        }
        else
        {
            //string vpath = Path.GetDirectoryName(localFileName);
            //string[] allv = Directory.GetFiles(vpath);
            //string[] allf = Directory.GetFiles(longs);
            ////foreach (string ff in allf)
            ////{
            ////    //if (Path.GetExtension(ff) == ".mp4")
            ////    //    File.Delete(ff);
            ////    string s = ff;
            ////}
        }



        FileResult x = new FileResult(localFileName);

        string extFileStr = Path.Combine(longs, x.FileName);
        if (File.Exists(extFileStr))
            File.Delete(extFileStr);

        File.Move(x.FullPath, extFileStr);
        x = new FileResult(Path.Combine(longs, x.FileName));

        await Navigation.PushPopupAsync(new FileManagement_Popup_AddFile(mvm, x));
        await Navigation.RemovePopupPageAsync(this);
    }

    private async void pickVideo_Clicked(object sender, EventArgs e)
    {
        if (!CrossMedia.Current.IsPickVideoSupported)
        {
            await DisplayAlert("Videos Not Supported", ":( Permission not granted to videos.", "OK");
            return;
        }


        FileResult file = await FilePicker.PickAsync();


        if (file == null)
            return;

        NE201FileManager mr = NE201FileManager.FileManagerFactory(App.PC2ViewModel.UserSelected);
        await mr.NE201FileUpload(file, mvm.PrevPath);
        //await mvm.ListView_RefreshFolder();
        await mvm.readAllFileList(mvm.PrevPath, mvm.NASFiles.Count + 1);
        await Navigation.RemovePopupPageAsync(this);
    }



    private string nextFileName(string sFileName)
    {
        ///storage/emulated/0/Android/data/com.companyname.powercloud/files/Movies/DefaultVideos/video000001_1.mp4

        //string spath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        //spath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string[] dinfo = new string[0];
        string longs = Path.Combine(FileSystem.AppDataDirectory, "Movies", "DefaultVideos");
        if (Directory.Exists(longs))
            dinfo = Directory.GetFiles(longs);

        string fileName = Path.GetFileName(sFileName);
        string fileExt = Path.GetExtension(sFileName);

        string newFileName = fileName.Substring(0, fileName.Length - fileExt.Length);
        string nameTemp = newFileName;

        for (int i = newFileName.Length; i > 0; i--)
        {
            if (newFileName[i - 1] < '0' || newFileName[i - 1] > '9')
            {
                nameTemp = newFileName.Substring(0, i);
                break;
            }
        }


        int maxCount = 0;
        string NewName = string.Empty;
        foreach (string fullName in dinfo)
        {
            string serialName = Path.GetFileName(fullName);
            if (serialName.EndsWith(fileExt) && serialName.StartsWith(nameTemp))
            {
                string fn = serialName.Substring(0, serialName.Length - fileExt.Length);
                string nstr = string.Empty;
                for (int i = fn.Length; i > 0; i--)
                {
                    if (fn[i - 1] >= '0' && fn[i - 1] <= '9')
                        nstr = fn[i - 1] + nstr;
                    else
                        break;
                }
                if (!string.IsNullOrEmpty(nstr))
                {
                    int now = 0;
                    int.TryParse(nstr, out now);
                    if (now > maxCount)
                        maxCount = now;
                }
            }
        }
        maxCount++;

        newFileName = nameTemp + (1000000 + maxCount).ToString().Substring(1) + fileExt;

        return newFileName;
    }
}