using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using PowerCloud.NasHttp;
using PowerCloud.ViewModels;

namespace PowerCloud.Views.FileManagement;


public partial class Popup_Add : CommunityToolkit.Maui.Views.Popup
{
    public Popup_Add(MainNasFileViewModel mvmPrm)
	{
		InitializeComponent();
        //PowerCloud 檔案管理 3按鈕
        DesiredSize = new(0.95 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density));

        mvm = mvmPrm;
    }
    MainNasFileViewModel? mvm;

    //private void Btn_Popup_AddFolder(object sender, EventArgs e)
    //{
    //    //var popup = new PopupTestContentView();
    //    var popup = new Popup_AddFolder();
    //    //popup-end
    //    //popup.VerticalOptions = LayoutAlignment.End;
    //    //popup.HorizontalOptions = LayoutAlignment.Fill;

    //    //popup-center
    //    popup.VerticalOptions = LayoutAlignment.Center;
    //    popup.HorizontalOptions = LayoutAlignment.Fill;

    //    this.ShowPopup(popup);
    //}


    //Btn_Clicked_ClosePopup
    //https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/views/popup
    async void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => await CloseAsync();

    async private void Tap_Upload_Tapped(object sender, EventArgs e)
    {
        if (mvm == null)
            return;

        ActIndicator.IsRunning = true;
        NE201FileManager fmgr = NE201FileManager.FileManagerFactory(App.PC2ViewModel.UserSelected);

        //var customFileType =
        //new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        //{
        //    { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // or general UTType values
        //    { DevicePlatform.Android, new[] { "application/comics" } },
        //    { DevicePlatform.UWP, new[] { ".cbr", ".cbz" } },
        //    { DevicePlatform.Tizen, new[] { "*/*" } },
        //    { DevicePlatform.macOS, new[] { "cbr", "cbz" } }, // or general UTType values
        //});
        var options = new PickOptions
        {
            PickerTitle = "Please select files",
            //FileTypes = customFileType,
        };
        if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            //var result = await MediaPicker.PickPhotoAsync();
            var result = await FilePicker.PickMultipleAsync(options);
            if (result != null)
            {

                int n = 0;
                foreach (FileResult fresult in result)
                {
                    string fullName = Path.Combine(mvm.PrevPath, fresult.FileName);
                    fullName = await mvm.GetNewFileName(fullName);
                    if (await fmgr.NE201FileUpload(fresult, mvm.PrevPath, Path.GetFileName(fullName)))
                    {
                        n++;
                        FileInfo finfo = new FileInfo(fresult.FullPath);
                        NASFileViewModel newFile = new NASFileViewModel()
                        {
                            MimeType = fresult.ContentType,
                            Name = Path.GetFileName(fullName),
                            PathName = mvm.PrevPath,
                            Size = finfo.Length,
                            LastWriteTime = finfo.LastWriteTime.ToString("R")
                        };
                        if (mvm.UseThumbNail)
                        {
                            newFile.thumbNail = await fmgr.NE201ImageThumbnail(Path.Combine(fresult.FileName, mvm.PrevPath), mvm.thumbSize);
                        }
                        newFile.UsingThumb = mvm.UseThumbNail;
                        mvm.NASFiles.Insert(0, newFile);
                        n++;
                    }
                }

                ////await mvm.readAllFileList(mvm.PrevPath, mvm.NASFiles.Count + n);
                ////mvm.ScrollToAnchor();
                
                
                ActIndicator.IsRunning = false;
                if (n > 0)
                    await AppShell.Current.CurrentPage.DisplayAlert($"訊息", "上傳完成", "結束");
                else
                    await AppShell.Current.CurrentPage.DisplayAlert($"訊息", "未完成上傳", "中斷");

                //if (AppShell.Current.Navigation.NavigationStack.Count > 0)
                //    await AppShell.Current.Navigation.RemovePage(this); //.RemovePopupPageAsync(this);
                await CloseAsync();
            }
            else
            {
                fmgr.ResultMessage = "Can not find local file.";
            }
        }
        else
        {
            var result = await FilePicker.PickMultipleAsync(options); // await MediaPicker.PickPhotoAsync();
                                                                      //var result = await MediaPicker.PickPhotoAsync();
            if (result != null)
            {
                int n = 0;
                ActIndicator.IsRunning = true;
                foreach (FileResult fresult in result)
                {
                    string fullName = Path.Combine(mvm.PrevPath, fresult.FileName);
                    fullName = await mvm.GetNewFileName(fullName);
                    if (await fmgr.NE201FileUpload(fresult, mvm.PrevPath, Path.GetFileName(fullName)))
                    {
                        n++;
                        FileInfo finfo = new FileInfo(fresult.FullPath);
                        NASFileViewModel newFile = new NASFileViewModel()
                        {
                            MimeType = fresult.ContentType,
                            Name = Path.GetFileName(fullName),
                            PathName = mvm.PrevPath,
                            Size = finfo.Length,
                            LastWriteTime = finfo.LastWriteTime.ToString("R")
                        };
                        if (mvm.UseThumbNail)
                        {
                            newFile.thumbNail = await fmgr.NE201ImageThumbnail(Path.Combine(fresult.FileName, mvm.PrevPath), mvm.thumbSize);
                        }
                        newFile.UsingThumb = mvm.UseThumbNail;
                        mvm.NASFiles.Insert(0, newFile);
                    }
                }

                ActIndicator.IsRunning = false;
                if (n > 0)
                    await AppShell.Current.CurrentPage.DisplayAlert($"訊息", "上傳完成", "結束");
                else
                    await AppShell.Current.CurrentPage.DisplayAlert($"訊息", "未完成上傳", "中斷");

                //if (AppShell.Current.Navigation.NavigationStack.Count > 0)
                //    await AppShell.Current.Navigation.RemovePage(this); //.RemovePopupPageAsync(this);
                await CloseAsync();
            }
            else
            {
                fmgr.ResultMessage = "Can not find local file.";
            }
        }
    }

    private void Popup_AddFolder_Tapped(object sender, TappedEventArgs e)
    {
        //await AppShell.Current.CurrentPage.Navigation.PushModalAsync(new opup_AddFolder(mvm));
        if (mvm == null)
            return;

        var popup = new Popup_AddFolder(mvm);
        //popup-end
        popup.VerticalOptions = LayoutOptions.End;
        popup.HorizontalOptions = LayoutOptions.Fill;
        //popup-center
        //popup.VerticalOptions = LayoutAlignment.Center;
        //popup.HorizontalOptions = LayoutAlignment.Fill;
        Shell.Current.ShowPopup(popup);
    }

    private void Tap_Camera_Tapped(object sender, TappedEventArgs e)
    {
        if (mvm == null)
            return;

        var popup = new Popup_TakePicture(mvm);
        //popup-end
        popup.VerticalOptions = LayoutOptions.Center;
        popup.HorizontalOptions = LayoutOptions.Fill;
        //popup-center
        //popup.VerticalOptions = LayoutAlignment.Center;
        //popup.HorizontalOptions = LayoutAlignment.Fill;
        Shell.Current.ShowPopup(popup);

        ////if (mvm == null) 
        ////    return;

        ////mvm.UsingCamera = true;

        ////await AppShell.Current.CurrentPage.Navigation.PushAsync(new UsingCamera());

        ////Close();

        #region backup for community toolkit popup page
        //FileResult picResult = await MediaPicker.CapturePhotoAsync();
        //if (picResult != null)
        //{
        //    await Task.Delay(TimeSpan.FromSeconds(0.3));

        //    PopupResult popupResult;
        //    popupResult = new PopupResult
        //    {
        //        ReturnData = "Test0001.image"
        //    };
        //    var result = await Navigation.ShowPopupAsync(new MyPopup(popupResult));

        //    //picResult.FileName = result.ReturnData;
        //    saveToServer(picResult, popupResult.ReturnData);
        //    await mvm.ListView_RefreshFolder();
        //}
        //string s = picResult.FileName;
        //picResult = null;

        //////PopupResult popupResult;
        //////popupResult = new PopupResult
        //////{
        //////    ReturnData = "Test0001.image"
        //////};
        //////var result = await Navigation.ShowPopupAsync(new MyPopup(popupResult));
        //////await DisplayAlert("Result", $"Result: {result.ReturnData}", "OK");
        #endregion
    }
}