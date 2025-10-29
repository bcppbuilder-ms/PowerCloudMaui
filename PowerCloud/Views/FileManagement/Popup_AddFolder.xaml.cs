using PowerCloud.NasHttp;
using PowerCloud.ViewModels;

namespace PowerCloud.Views.FileManagement;


public partial class Popup_AddFolder : CommunityToolkit.Maui.Views.Popup
{
    public Popup_AddFolder(MainNasFileViewModel mvmPrm)
	{
		InitializeComponent();

        mvm = mvmPrm;
    }
    MainNasFileViewModel mvm;

    //Btn_Clicked_ClosePopup
    void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => CloseAsync();

    private async void Btn_Clicked_AddFolder(object sender, EventArgs e)
    {
        //string FolderName = await DisplayPromptAsync(
        //        "New Folder", "Input folder name, please.", "OK", "Cancel", "Input folder name here", 50, null, "NewFolder"
        //    );

        string FolderName = EntryFolderName.Text;

        if (string.IsNullOrEmpty(FolderName))
        {
            await AppShell.Current.CurrentPage.DisplayAlert("Error", "No new folder was created.", "Close");
        }
        else
        {
            NE201FileManager fmgr = NE201FileManager.FileManagerFactory(App.PC2ViewModel.UserSelected);
            string error = await fmgr.NE201AddFolder(mvm.PrevPath, FolderName);

            if (string.IsNullOrEmpty(error))
            {
                NASFileViewModel item = new NASFileViewModel()
                {
                    LastWriteTime = DateTime.Now.ToString("R"),
                    Name = FolderName,
                    PathName = mvm.PrevPath,
                    MimeType = "folder",
                    Size = 0,
                    UsingThumb = mvm.UseThumbNail,
                    CanMultiSelect = false
                };
                mvm.NASFiles.Insert(0, item);
                //mvm.PageCollectionView.ScrollTo(0, -1, ScrollToPosition.Start);

                ////await mvm.ListView_RefreshFolder();
                //await mvm.readAllFileList(mvm.PrevPath, mvm.NASFiles.Count + 1);
            }
            else
            {
                await AppShell.Current.CurrentPage.DisplayAlert("Failed", "Folder creation is failed.\r\n" + error, "Finish");
            }
        }
        await CloseAsync();
    }
}