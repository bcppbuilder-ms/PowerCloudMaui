using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PowerCloud.ViewModels
{
    public class MainViewModel : BindViewModel
    {
        public readonly AccountManager accountManager;

        public MainViewModel(IAccountFiler flr, IIte2DeviceInfo devInfo)
        {
            accountManager = new AccountManager(flr, null, null, devInfo);
            if (accountManager == null)
                throw new NullReferenceException();
            Accounts = accountManager.Accounts as ObservableCollection<AccountViewModel>;
            RecentAccounts = new ObservableCollection<AccountViewModel>();
            for (int i = 0; i < Accounts.Count && i < 3; i++)
            {
                RecentAccounts.Add(Accounts[i]);
            }
            foreach (AccountViewModel item in RecentAccounts)
                Accounts.RemoveAt(0);

            if (RecentAccounts.Count > 0)
                itemSelected = RecentAccounts[0];
            else
                itemSelected = null;
        }

        //ObservableCollection<AccountViewModel> recentList;
        public ObservableCollection<AccountViewModel> RecentAccounts { get; set; } //{ get { return recentList; } set { SetPropertyValue(ref recentList, value); } }
        //ObservableCollection<AccountViewModel> acciLst;
        public ObservableCollection<AccountViewModel> Accounts { get; set; } //{ get { return acciLst; } set { SetPropertyValue(ref acciLst, value); } }


        ObservableCollection<AccountViewModel> tmpAll = new ObservableCollection<AccountViewModel>();
        public ObservableCollection<AccountViewModel> AllLoginAccounts
        {
            get
            {
                if (tmpAll == null || tmpAll.Count == 0)
                {
                    foreach (AccountViewModel item in RecentAccounts)
                    {
                        if (IsLogin(item))
                        {
                            if (!tmpAll.Contains(item))
                                tmpAll.Add(item);
                        }
                    }
                    foreach (AccountViewModel item in Accounts)
                    {
                        if (IsLogin(item))
                        {
                            if (!tmpAll.Contains(item))
                                tmpAll.Add(item);
                        }
                    }
                }


                return tmpAll;
            }

            //set { tmpAll = value; }
        }

        ObservableCollection<AccountViewModel> tmpAllLogout = new ObservableCollection<AccountViewModel>();
        public ObservableCollection<AccountViewModel> AllLogoutAccounts
        {
            get
            {
                if (tmpAllLogout == null || tmpAllLogout.Count == 0)
                {
                    if (RecentAccounts != null)
                    {
                        foreach (AccountViewModel item in RecentAccounts)
                        {
                            if (!IsLogin(item))
                            {
                                if (!tmpAllLogout.Contains(item))
                                    tmpAllLogout.Add(item);
                            }
                        }
                    }
                    if (Accounts != null)
                    {
                        foreach (AccountViewModel item in Accounts)
                        {
                            if (!IsLogin(item))
                            {
                                if (!tmpAllLogout.Contains(item))
                                    tmpAllLogout.Add(item);
                            }
                        }
                    }
                }


                return tmpAllLogout;
            }

            //set { tmpAllLogout = value; }
        }

        public bool IsLogin(AccountViewModel item = null)
        {
            if (item == null)
                item = this.UserSelected;
            if (item == null)
                return false;

            if (string.IsNullOrEmpty(item.AccessToken) || item.AccessToken == "unknown" ||
                item.SystemInfo == null || (item.SystemInfo?.LoginAt ?? default) == default ||
                item.SystemInfo.LoginAt.AddDays(13.9) < DateTime.Now)
                return false;

            return true;
        }

        ObservableCollection<AccountViewModel> tmpEveryone = new ObservableCollection<AccountViewModel>();
        public ObservableCollection<AccountViewModel> EveryAccount
        {
            get
            {
                if (tmpEveryone == null || tmpEveryone.Count == 0)
                {
                    foreach (AccountViewModel item in RecentAccounts)
                    {
                        if (!tmpEveryone.Contains(item))
                            tmpEveryone.Add(item);
                    }
                    foreach (AccountViewModel item in Accounts)
                    {
                        if (!tmpEveryone.Contains(item))
                            tmpEveryone.Add(item);
                    }
                }
                return tmpEveryone;
            }

            //set { if (value != null) tmpEveryone = value; }
        }

        public string PicViewPath;

        AccountViewModel itemSelected;
        public AccountViewModel UserSelected
        {
            get { return itemSelected; }
            set
            {
                SetPropertyValue(ref itemSelected, value);
                if (itemSelected != null)
                {
                    URNAME = itemSelected.UserName;
                    URIP = itemSelected.UserNasLink;
                }
            }
        }

        AccountViewModel tmpSelected;
        public AccountViewModel TmpSelectedUser
        {
            get { return tmpSelected; }
            set
            {
                SetPropertyValue(ref tmpSelected, value);
            }
        }

        public ICommand SetSelectedCommand => new Command<AccountViewModel>(SetSelected);
        private void SetSelected(AccountViewModel file)
        {
            TmpSelectedUser = file;
        }



        string urname, urip;
        public string URNAME
        {
            get
            {
                if (itemSelected == null)
                    return string.Empty;
                return itemSelected.UserName;
            }
            set { SetPropertyValue(ref urname, value); }
        }
        public string URIP
        {
            get
            {
                if (itemSelected == null)
                    return string.Empty;
                return itemSelected.UserNasLink;
            }
            set { SetPropertyValue(ref urip, value); }
        }

        public string LoginMessage = string.Empty;

        string tmpIp = string.Empty;
        public string TmpIp { get { return tmpIp; } set { SetPropertyValue(ref tmpIp, value); } }

        public void SaveAccounts()
        {
            accountManager.Save(RecentAccounts, Accounts);
        }

        public MainNasFileViewModel MultiSelectVM;

        //public void TakePicture(AccountViewModel quote)
        //{
        //    accountManager.TakePicture(quote);
        //}

        //public void PDFRender(string accessTokne, MainNasFileViewModel fvm)
        //{
        //    accountManager.PDFRender(accessTokne, fvm);
        //}

        public string TestStr()
        {
            return "Haha ... ";
        }

        public AlbumItem TargetAlbumItem;
    }

    public class AlbumItem : BindViewModel
    {
        public string LocalMediaFullpath = string.Empty;
        public string LocalThumbPath = string.Empty;
        public byte[] thumbNail = Array.Empty<byte>();
        string fname;
        public string FileName { get { return fname; } set { SetPropertyValue(ref fname, value); } }

        string fmat;
        public string FileFormat { get { return fmat; } set { SetPropertyValue(ref fmat, value); } }

        string flm;
        public string FileLastModifiedDate { get { return flm; } set { SetPropertyValue(ref flm, value); } }

        public string fsize;
        public string FileSize { get { return fsize; } set { SetPropertyValue(ref fsize, value); } }

        bool isimg;
        public bool IsImage { get { return isimg; } set { SetPropertyValue(ref isimg, value); } }

        string id;
        public string Id { get { return id; } set { SetPropertyValue(ref id, value); } }

        int width;
        public int Width { get { return width; } set { SetPropertyValue(ref width, value); } }

        int height;
        public int Height { get { return height; } set { SetPropertyValue(ref height, value); } }

        ImageSource imgs;
        public ImageSource ImageSrc
        {
            get
            {
                return imgs;
            }

            set
            {
                SetPropertyValue(ref imgs, value);
            }
        }

        public override string ToString()
        {
            string s = string.Empty;
            if (IsImage)
                s = "image/";
            else
                s = "video/";
            return s + Id + "//" + FileName + "//" + FileLastModifiedDate + "//" + FileSize + "//" + Width + "//" + Height;
        }
    }
}
