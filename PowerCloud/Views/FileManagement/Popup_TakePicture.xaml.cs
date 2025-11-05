using CommunityToolkit.Maui.Extensions;
using PowerCloud.ViewModels;

namespace PowerCloud.Views.FileManagement;

public partial class Popup_TakePicture : CommunityToolkit.Maui.Views.Popup
{
	public Popup_TakePicture(MainNasFileViewModel prmMvm)
	{
        InitializeComponent();

        mvm = prmMvm;
    }

    MainNasFileViewModel mvm;

    private void ClosePopup_Tapped(object sender, TappedEventArgs e)
    {
        CloseAsync();
    }

    private async void takePhoto_Clicked(object sender, System.EventArgs e)
    {
        FileResult? photo = null;

        try
        {
            MediaPickerOptions options = new()
            {
                Title = "Take a photo",
            };

            photo = await MediaPicker.CapturePhotoAsync(options);

            if (photo != null)
            {
                var stream = await photo.OpenReadAsync();
                image.Source = ImageSource.FromStream(() => stream);

                //FileResult x = new FileResult(photo.FullPath);
        }

        }
        catch (FeatureNotSupportedException)
        {
            await AppShell.Current.CurrentPage.DisplayAlert("Error", "Camera not supported on this device.", "OK");
            return;
        }
        catch (PermissionException)
        {
            await AppShell.Current.CurrentPage.DisplayAlert("Error", "Camera permission denied.", "OK");
            return;
        }
        catch (Exception ex)
        {
            await AppShell.Current.CurrentPage.DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
            return;
        }
        ////if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
        ////{
        ////    await DisplayAlert("No Camera", ":( No camera available.", "OK");
        ////    return;
        ////}

        ////MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
        ////{
        ////    Directory = "Test",
        ////    SaveToAlbum = true,
        ////    CompressionQuality = 75,
        ////    CustomPhotoSize = 50,
        ////    PhotoSize = PhotoSize.MaxWidthHeight,
        ////    MaxWidthHeight = 2000,
        ////    DefaultCamera = CameraDevice.Front
        ////});

        if (photo == null)
            return;

        localFileName = photo.FullPath;

        if (mvm == null)
            return;

        var popup = new Popup_AddFile(mvm, photo);
        //popup.VerticalOptions = LayoutOptions.Center;
        //popup.HorizontalOptions = LayoutOptions.Fill;
        popup.Margin = new Thickness(0, 0, 0, 0);

        Shell.Current.ShowPopup(popup);
    }

    string localFileName;

    private async void pickPhoto_Clicked(object sender, System.EventArgs e)
    {
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
    }

    private async void takeVideo_Clicked(object sender, System.EventArgs e)
    {
        FileResult? video = null;

        try
        {
            MediaPickerOptions options = new()
            {
                Title = "Take a Video",
            };

            video = await MediaPicker.CaptureVideoAsync(options);

            if (video != null)
            {
                var stream = await video.OpenReadAsync();
                image.Source = ImageSource.FromStream(() => stream);

                //FileResult x = new FileResult(photo.FullPath);
            }

        }
        catch (FeatureNotSupportedException)
        {
            await AppShell.Current.CurrentPage.DisplayAlert("Error", "Camera not supported on this device.", "OK");
            return;
        }
        catch (PermissionException)
        {
            await AppShell.Current.CurrentPage.DisplayAlert("Error", "Camera permission denied.", "OK");
            return;
        }
        catch (Exception ex)
        {
            await AppShell.Current.CurrentPage.DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
            return;
        }

        if (video == null)
            return;

        localFileName = video.FullPath;

        if (mvm == null)
            return;

        var popup = new Popup_AddFile(mvm, video);
        //popup.VerticalOptions = LayoutOptions.Center;
        //popup.HorizontalOptions = LayoutOptions.Fill;
        popup.Margin = new Thickness(0, 0, 0, 0);

        Shell.Current.ShowPopup(popup);
    }

    private async void pickVideo_Clicked(object sender, EventArgs e)
    {
        ////if (!CrossMedia.Current.IsPickVideoSupported)
        ////{
        ////    await DisplayAlert("Videos Not Supported", ":( Permission not granted to videos.", "OK");
        ////    return;
        ////}


        ////FileResult file = await FilePicker.PickAsync();


        ////if (file == null)
        ////    return;

        ////NE201FileManager mr = NE201FileManager.FileManagerFactory(App.PC2ViewModel.UserSelected);
        ////await mr.NE201FileUpload(file, mvm.PrevPath);
        //////await mvm.ListView_RefreshFolder();
        ////await mvm.readAllFileList(mvm.PrevPath, mvm.NASFiles.Count + 1);
        ////await Navigation.RemovePopupPageAsync(this);
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