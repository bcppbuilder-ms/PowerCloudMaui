using MobileCoreServices;
using UIKit;

using Photos;
using Foundation;
using CoreGraphics;
using CoreFoundation;
using CoreMedia;
using AVFoundation;
using ObjCRuntime;

using PowerCloud.ViewModels;
using CoreAnimation;
using System.Runtime.InteropServices;

namespace PowerCloud.Platforms
{

    public class Ite2DeviceInfoService2 : IIte2DeviceInfo2
    {
        public static UIWindow MyWindow;
        //public static NSUserDefaults IPCData = NSUserDefaults.StandardUserDefaults;
        public static NSUserDefaults IPCData; //= new NSUserDefaults("group.com.companyname.PowerCloud", NSUserDefaultsType.SuiteName);
        public Ite2DeviceInfoService2()
        {
            IPCData = new NSUserDefaults("group.com.ite2.powercloud.maui", NSUserDefaultsType.SuiteName);
        }

        #region Computed Properties
        /// <summary>
        /// Returns the delegate of the current running application
        /// </summary>
        /// <value>The this app.</value>
        public AppDelegate ThisApp
        {
            get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
        }
        #endregion

        public async Task<string> GetExternalPath(AccountViewModel user)
        {
            exceptionDesc = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(externalPath))
                    externalPath = await iphonePath();
            }
            catch (Exception allErr)
            {
                exceptionDesc = allErr.ToString();
            }
            return externalPath;
        }

        string externalPath = string.Empty;
        string exceptionDesc = null;
        async Task<string> iphonePath()
        {
            string sss2 = UTType.Folder;
            //string sss3 = UTType.Folder.Description;
            FilePickerFileType customFileType = //UTType.Folder;
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { sss2 } }
                });

            var options = new PickOptions
            {
                PickerTitle = "Please select a file",
                FileTypes = customFileType,
            };

            //FileResult result = await FileMover.PickAsync(options); <-- UI instancy malfunction
            FileResult result = await FilePicker.PickAsync(options);
            if (result == null)
                return string.Empty;

            return result.FullPath;
        }

        public async Task DownloadToDevice(MainNasFileViewModel mvm)
        {
            //Stream androidStream = GetJavaStream(androidStream);
            if (mvm.PayLoad.Count < 1)
                return;

            NASFileViewModel file = mvm.PayLoad[0];
            Stream inputStream = await mvm.GetDownloadStream(file);
            downloadFile(mvm.FileSelected, inputStream);
        }



        private bool downloadFile(NASFileViewModel file, Stream inputstream)
        {
            string pathName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            IEnumerable<string> dir = Directory.EnumerateDirectories(pathName);

            string downloadPath = Path.Combine(pathName, "Download");
            bool created = false;
            foreach (string ss in dir)
            {
                if (ss == downloadPath)
                {
                    created = true;
                    break;
                }
            }
            if (!created)
                Directory.CreateDirectory(downloadPath);

            string newFile = Path.Combine(downloadPath, file.Name);
            FileStream fi = File.Create(newFile);

            inputstream.CopyTo(fi);

            fi.Close();
            inputstream.Close();


            return true;
        }



        public async Task<Stream> GetDownloadStream(NASFileViewModel file)
        {
            if (string.IsNullOrEmpty(externalPath))
            {
                await GetExternalPath(App.PC2ViewModel.UserSelected);
            }


            string newFile = Path.Combine(externalPath, file.Name);


            FileStream fi = File.Create(newFile);
            return fi;
        }




        //
        //
        //
        //
        private UIImage GetVideoThumbnail(string path)
        {
            try
            {
                CMTime actualTime;
                NSError outError;
                using (var asset = AVAsset.FromUrl(NSUrl.FromFilename(path)))
                using (var imageGen = new AVAssetImageGenerator(asset))
                using (var imageRef = imageGen.CopyCGImageAtTime(new CMTime(1, 1), out actualTime, out outError))
                {
                    return UIImage.FromImage(imageRef);
                }
            }
            catch
            {
                return null;
            }
        }



        //
        //
        // Photo backup Sub program
        //
        //
        public /*async*/ void GetPhotoLibrary(MainNasFileViewModel prmMvm)
        {
            //Ite2.FileResult x = await Ite2.MediaPicker.PickPhotoAsync();
            //string s = x.FullPath;


            #region for study
            //////statements for showing all photo

            ////PHImageRequestOptions options = new PHImageRequestOptions();
            ////options.ResizeMode = PHImageRequestOptionsResizeMode.Fast;
            ////options.DeliveryMode = PHImageRequestOptionsDeliveryMode.FastFormat;
            ////options.Synchronous = true;

            ////var assets = PHAsset.FetchAssets(PHAssetMediaType.Image, null);
            ////PHImageManager manager = new PHImageManager();

            ////int r = 0;
            ////List<UIImage> images = new List<UIImage>();
            ////List<NSDictionary> infoList = new List<NSDictionary>();
            ////foreach (PHAsset asset in assets)
            ////{
            ////    manager.RequestImageForAsset(asset, PHImageManager.MaximumSize, PHImageContentMode.Default, options,
            ////                                (image, info) =>
            ////                                {
            ////                                    infoList.Add(info);
            ////                                    images.Add(image);
            ////                                });
            ////    r++;
            ////}

            ////PHVideoRequestOptions videoOptions = new PHVideoRequestOptions();
            ////var videoAssets = PHAsset.FetchAssets(PHAssetMediaType.Video, null);
            ////int n = 0;
            ////foreach (PHAsset asset in videoAssets)
            ////{
            ////    //manager.RequestAvAsset(null, )
            ////    n++;
            ////}

            //QLThumbnailGenerationRequest xreq = new 
            //    QLThumbnailGenerationRequest(url, new CGSize(300.00, 300.00), UIScreen.MainScreen.Scale, QLThumbnailGenerationRequestRepresentationTypes.Thumbnail);

            // Get our own windows saved at AppDelegate.cs
            #endregion

            //var plist = new NSUserDefaults("group.com.companyname.PowerCloud"); //AppDelegate.IPCData;
            //string s2 = SelectedNasAddress;
            string s1 = IPCData.StringForKey("NASAddress");

            UIWindow x = MyWindow;

            LineLayout layout = new LineLayout()
            {
                HeaderReferenceSize = new CGSize(160, 100)
            };

            UICollectionViewFlowLayout flowLayout = new UICollectionViewFlowLayout()
            {
                ItemSize = new CGSize(80, 80),
                HeaderReferenceSize = new CGSize(120, 120),
                SectionInset = new UIEdgeInsets(2, 2, 2, 2),
                ScrollDirection = UICollectionViewScrollDirection.Vertical,
                MinimumInteritemSpacing = 10, // minimum spacing between cells
                MinimumLineSpacing = 10 // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
            };


            photoViewController cvc = new photoViewController(flowLayout);
            //cvc.CollectionView.ContentInset = new UIEdgeInsets(50, 0, 0, 0);


            cvc.oldController = x.RootViewController;
            var navController = new UINavigationController(cvc);

            // change current view
            x.RootViewController = navController;
        }




        public List<AlbumItem> GetNativeFileIDs()
        {
            List<AlbumItem> list = new List<AlbumItem>();
            foreach (Ite2MediaItem item in AllMediaFiles)
            {
                if (item.IsImage)
                {
                    list.Add(new AlbumItem()
                    {
                        Id = item.Id.ToString(),
                        IsImage = true,
                        FileName = item.DisplayName,
                        FileLastModifiedDate = item.DateAdded,
                        FileSize = item.FileSize.ToString(),
                        Width = item.Width,
                        Height = item.Height,
                        LocalMediaFullpath = item.LocalFullpath
                    });
                }
                else
                {
                    list.Add(new AlbumItem()
                    {
                        Id = item.Id.ToString(),
                        IsImage = false,
                        FileName = item.DisplayName,
                        FileLastModifiedDate = item.DateAdded,
                        FileSize = item.FileSize.ToString(),
                        Width = item.Width,
                        Height = item.Height,
                        LocalMediaFullpath = item.LocalFullpath
                    });
                }
            }

            return list;
        }

        public Task<bool> UploadMediaFile(AlbumItem item)
        {
            return Task.Run(() => false);
        }

        public Task<byte[]> GetThumbnail(AlbumItem item)
        {
            int n = 0;
            foreach (Ite2MediaItem ba in allMedias)
            {
                if (ba.Id.ToString() == item.Id)
                {
                    NFloat compQuality = 25;
                    byte[] resizedImg = allImages[n].AsJPEG(compQuality).ToArray();
                    return Task.Run(() => resizedImg);
                }
                n++;
            }

            return Task.Run(() => Array.Empty<byte>());
        }

        public async Task<bool> CopyToLocal(AlbumItem source, string target)
        {
            int n = 0;
            foreach (Ite2MediaItem ba in allMedias)
            {
                if (ba.Id.ToString() == source.Id)
                {
                    NFloat compQuality = 1.0f;
                    byte[] resizedImg = allImages[n].AsJPEG(compQuality).ToArray();
                    var stream = new FileStream(target, FileMode.Create);
                    string ext = Path.GetExtension(target);
                    await stream.WriteAsync(resizedImg, 0, resizedImg.Length);
                    stream.Close();

                    return true;
                }
                n++;
            }

            return false;
        }

        static List<UIImage> allImages;
        static List<Ite2MediaItem> allMedias;
        public static List<Ite2MediaItem> AllMediaFiles
        {
            get
            {
                if (allMedias == null || allMedias.Count == 0)
                {
                    allMedias = new List<Ite2MediaItem>();
                    ListPhotoFiles();
                }

                return allMedias;
            }
        }



        string userDataNasAddress;
        string userDataAccessToken;
        string userDataRefreshToken;
        public string SelectedAccessToken
        {
            get { return userDataAccessToken; }
            set
            {
                userDataAccessToken = value;
                IPCData.SetString(value, "AToken");
                string s = IPCData.StringForKey("memo1");
                //CFPreferences.AppSynchronize();
            }
        }
        public string SelectedRefreshToken
        {
            get { return userDataRefreshToken; }
            set
            {
                userDataRefreshToken = value;
                IPCData.SetString(value, "RToken");
                string s = IPCData.StringForKey("memo1");
                //CFPreferences.AppSynchronize();
            }
        }
        public string SelectedNasAddress
        {
            get { return userDataNasAddress; }
            set
            {
                //string s = AppDelegate.IPCData.StringForKey("NASAddress");
                //if (s != value)
                //{
                //    s = value;
                //}
                userDataNasAddress = value;
                IPCData.SetString(value, "Ne201Ip");
                //CFPreferences.AppSynchronize();
                string s = IPCData.StringForKey("memo1");
            }
        }

        public static void ListPhotoFiles()
        {
            if (allMedias != null)
                allMedias.Clear();

            List<Ite2MediaItem> listOfAllImages = new List<Ite2MediaItem>();

            PHImageRequestOptions imageRequestOptions = new PHImageRequestOptions();
            PHFetchOptions pfetchOpts = new PHFetchOptions
            {
                SortDescriptors = new NSSortDescriptor[] { new NSSortDescriptor("creationDate", true) },
            };
            PHFetchResult allPhotos = PHAsset.FetchAssets(pfetchOpts);

            imageRequestOptions.Version = PHImageRequestOptionsVersion.Current;
            imageRequestOptions.DeliveryMode = PHImageRequestOptionsDeliveryMode.FastFormat;
            imageRequestOptions.ResizeMode = PHImageRequestOptionsResizeMode.Fast;
            imageRequestOptions.Synchronous = true;

            PHImageManager manager = new PHImageManager();

            allImages = new List<UIImage>();
            List<string> infoList = new List<string>();
            int err1 = 0;
            int err2 = 0;
            int idCnt = 0;
            foreach (PHAsset asset in allPhotos)
            {
                infoList.Add(asset.Description);
                string cdate = asset.CreationDate.Description;
                //PHAssetSourceType r = asset.SourceType;
                if (asset.MediaType == PHAssetMediaType.Image)
                {
                    idCnt++;
                    manager.RequestImageForAsset(
                        asset, PHImageManager.MaximumSize, PHImageContentMode.Default, imageRequestOptions,
                            (image, info) =>
                            {
                                //Ite2.FileResult xfresult = null;
                                long size = 0;
                                string ss = "";
                                NSUrl kuri = null;
                                int width = 0;
                                int height = 0;

                                string s2 = "";
                                string s3 = "";
                                bool ok = true;

                                if (info != null)
                                {
                                    ////PHImageResultIsDegradedKey
                                    ////PHImageResultResquetIDKey
                                    s2 = info.DescriptionInStringsFileFormat;
                                    s3 = info["PHImageResultRequestIDKey"].ToString();
                                }
                                else
                                {
                                    err1++;
                                    ok = false;
                                }

                                if (image != null)
                                {
                                    if (image.CGImage != null)
                                    {
                                        width = (int)image.CGImage.Width;
                                        height = (int)image.CGImage.Height;
                                    }

                                    //xfresult = new Ite2.Ite2UIImageFileResult(image);
                                }
                                else
                                {
                                    err2++;
                                    ok = false;
                                }

                                if (ok)
                                {
                                    ss = $"IMAGE_{s3}.jpg";    //xfresult.FileName;
                                    Ite2MediaItem xMedia = new Ite2MediaItem(kuri, ss, cdate, int.Parse(s3), true, size, width, height);
                                    xMedia.LocalFullpath = ss;
                                    listOfAllImages.Add(xMedia);
                                    allImages.Add(image);
                                    //xfresult = null;
                                }
                            });
                }
            }

            allMedias = listOfAllImages;
        }

        //
        //
        // End Photo backup subprgram
        //
        //
    }















    public class photoViewController : UICollectionViewController
    {
        //
        //
        const string cellReuseIdentifier = "GridViewCell";

        public PHFetchResult FetchResult { get; set; }
        public PHAssetCollection AssetCollection { get; set; }

        readonly PHCachingImageManager imageManager = new PHCachingImageManager();
        CGSize thumbnailSize = new CGSize(192f, 192.0f);
        //CGRect previousPreheatRect;

        static NSString headerId = new NSString("Header");

        static NSString photoCellId = new NSString("PhotoCell");
        List<IPhoto> photos;
        //
        //

        public photoViewController(UICollectionViewLayout layout)
            : base(layout)
        {
            if (FetchResult == null)
            {
                var allPhotosOptions = new PHFetchOptions
                {
                    SortDescriptors = new NSSortDescriptor[] { new NSSortDescriptor("creationDate", true) }
                };
                FetchResult = PHAsset.FetchAssets(allPhotosOptions);
            }

            photos = new List<IPhoto>();
            int i = 0;
            foreach (PHAsset asset in FetchResult)
            {
                string photoName = asset.LocalIdentifier;
                imageManager.RequestImageForAsset(asset, thumbnailSize, PHImageContentMode.Default, null, (image, info) =>
                {
                    // The cell may have been recycled by the time this handler gets called;
                    // Set the cell's thumbnail image if it's still showing the same asset.
                    if (photoName == asset.LocalIdentifier && image != null)
                    {
                        Photo photo = new Photo(image);
                        //photo.Image = image;
                        i++;
                        photos.Add(photo);
                    }
                });
                //if (i >= 20)
                //    break;
            }
            for (; i < 20; i++)
                photos.Add(new Photo(UIImage.FromFile("file_empty.png")));
        }

        public UIViewController oldController;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //CollectionView.RegisterClassForCell(typeof(GridViewCell), cellReuseIdentifier);
            CollectionView.RegisterClassForCell(typeof(PhotoCell), photoCellId);
            CollectionView.RegisterClassForSupplementaryView(typeof(Header), UICollectionElementKindSection.Header, headerId);

            UIMenuController.SharedMenuController.MenuItems = new UIMenuItem[] {
                new UIMenuItem ("Custom", new Selector ("custom"))
            };

            View.BackgroundColor = UIColor.White;
            Title = "My Custom View Controller";

            //ResetCachedAssets();

            #region just for test
            var btn = UIButton.FromType(UIButtonType.System);
            btn.Frame = new CGRect(20, 80, 280, 44);
            btn.SetTitle("Go Home", UIControlState.Normal);
            // click events
            btn.TouchUpInside += (sender, e) =>
            {
                //this.NavigationController.PushViewController(user, true);
                Ite2DeviceInfoService2.MyWindow.RootViewController = oldController;
            };

            var btn2 = UIButton.FromType(UIButtonType.System);
            btn2.Frame = new CGRect(20, 144, 280, 44);
            btn2.SetTitle("Message", UIControlState.Normal);
            // click events
            btn2.TouchUpInside += (sender, e) =>
            {

                var alertController = UIAlertController.Create("New Album", null, UIAlertControllerStyle.Alert);
                alertController.AddTextField(textField =>
                {
                    textField.Text = "Ha ha...";
                    textField.Placeholder = "Album Name";
                });

                alertController.AddAction(UIAlertAction.Create("Create", UIAlertActionStyle.Default, action =>
                {
                    var textField = alertController.TextFields[0];

                    var title = textField.Text;
                }));
                PresentViewController(alertController, true, null);
            };

            View.AddSubview(btn);
            View.AddSubview(btn2);
            #endregion
        }

        #region old implement
        //protected override void Dispose(bool disposing)
        //{
        //    PHPhotoLibrary.SharedPhotoLibrary.UnregisterChangeObserver(this);
        //    base.Dispose(disposing);
        //}

        //public override void ViewWillAppear(bool animated)
        //{
        //    base.ViewWillAppear(animated);

        //    // Add button to the navigation bar if the asset collection supports adding content.

        //    //View.LayoutIfNeeded();
        //    UpdateItemSize();
        //}

        //public override void ViewWillLayoutSubviews()
        //{
        //    base.ViewWillLayoutSubviews();
        //    UpdateItemSize();
        //}

        //public override void ViewDidAppear(bool animated)
        //{
        //    base.ViewDidAppear(animated);
        //    UpdateCachedAssets();
        //}

        //void UpdateItemSize()
        //{
        //    var viewWidth = View.Bounds.Width;

        //    var desiredItemWidth = 100f;
        //    var columns = Math.Max(Math.Floor(viewWidth / desiredItemWidth), 4f);
        //    var padding = 1f;
        //    var itemWidth = Math.Floor((viewWidth - (columns - 1f) * padding) / columns);
        //    var itemSize = new CGSize(itemWidth, itemWidth);

        //    if (Layout is UICollectionViewFlowLayout layout)
        //    {
        //        layout.ItemSize = itemSize;
        //        layout.MinimumInteritemSpacing = padding;
        //        layout.MinimumLineSpacing = padding;
        //    }

        //    // Determine the size of the thumbnails to request from the PHCachingImageManager
        //    var scale = UIScreen.MainScreen.Scale;
        //    thumbnailSize = new CGSize(itemSize.Width * scale, itemSize.Height * scale);
        //}
        #endregion

        #region UICollectionView

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return photos.Count;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            ////var asset = (PHAsset)FetchResult[indexPath.Item];

            ////// Dequeue an GridViewCell.
            ////var cell = (GridViewCell)collectionView.DequeueReusableCell(cellReuseIdentifier, indexPath);

            ////// Request an image for the asset from the PHCachingImageManager.
            ////cell.RepresentedAssetIdentifier = asset.LocalIdentifier;
            ////imageManager.RequestImageForAsset(asset, thumbnailSize, PHImageContentMode.AspectFill, null, (image, info) =>
            ////{
            ////    // The cell may have been recycled by the time this handler gets called;
            ////    // Set the cell's thumbnail image if it's still showing the same asset.
            ////    if (cell.RepresentedAssetIdentifier == asset.LocalIdentifier && image != null)
            ////        cell.ThumbnailImage = image;
            ////});
            ////return cell;

            PhotoCell photoCell = (PhotoCell)collectionView.DequeueReusableCell(photoCellId, indexPath);
            Photo photo = (Photo)photos[indexPath.Row];

            photoCell.Image = photo.Image;

            return photoCell;
        }

        public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
        {
            var headerView = (Header)collectionView.DequeueReusableSupplementaryView(elementKind, headerId, indexPath);
            headerView.Text = "Supplementary View";
            return headerView;
        }

        ////public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        ////{
        ////    var cell = collectionView.CellForItem(indexPath);
        ////    cell.ContentView.BackgroundColor = UIColor.Yellow;
        ////}

        ////public override void ItemUnhighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        ////{
        ////    var cell = collectionView.CellForItem(indexPath);
        ////    cell.ContentView.BackgroundColor = UIColor.White;
        ////}

        ////public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
        ////{
        ////    return true;
        ////}

        ////public override bool ShouldShowMenu(UICollectionView collectionView, NSIndexPath indexPath)
        ////{
        ////    return true;
        ////}

        ////public override bool CanPerformAction(UICollectionView collectionView, Selector action, NSIndexPath indexPath, NSObject sender)
        ////{
        ////    // Selector should be the same as what's in the custom UIMenuItem
        ////    if (action == new Selector("custom"))
        ////        return true;
        ////    else
        ////        return false;
        ////}

        ////public override void PerformAction(UICollectionView collectionView, Selector action, NSIndexPath indexPath, NSObject sender)
        ////{
        ////    System.Diagnostics.Debug.WriteLine("code to perform action");
        ////}

        ////// CanBecomeFirstResponder and CanPerform are needed for a custom menu item to appear
        ////public override bool CanBecomeFirstResponder
        ////{
        ////    get
        ////    {
        ////        return true;
        ////    }
        ////}


        ////public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        ////{
        ////    base.WillRotate(toInterfaceOrientation, duration);

        ////    var lineLayout = CollectionView.CollectionViewLayout as LineLayout;
        ////    if (lineLayout != null)
        ////    {
        ////        if ((toInterfaceOrientation == UIInterfaceOrientation.Portrait) || (toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown))
        ////            lineLayout.SectionInset = new UIEdgeInsets(400, 0, 400, 0);
        ////        else
        ////            lineLayout.SectionInset = new UIEdgeInsets(220, 0.0f, 200, 0.0f);
        ////    }
        ////}

        #endregion

        #region UIScrollView

        //public override void Scrolled(UIScrollView scrollView)
        //{
        //    UpdateCachedAssets();
        //}

        //#endregion

        //#region Asset Caching
        //void ResetCachedAssets()
        //{
        //    imageManager.StopCaching();
        //    previousPreheatRect = CGRect.Empty;
        //}

        //void UpdateCachedAssets()
        //{
        //    // Update only if the view is visible.
        //    bool isViewVisible = IsViewLoaded && View.Window != null;
        //    if (!isViewVisible)
        //        return;

        //    // The preheat window is twice the height of the visible rect.
        //    var visibleRect = new CGRect(CollectionView.ContentOffset, CollectionView.Bounds.Size);
        //    var preheatRect = visibleRect.Inset(0f, -0.5f * visibleRect.Height);

        //    // Update only if the visible area is significantly different from the last preheated area.
        //    nfloat delta = NMath.Abs(preheatRect.GetMidY() - previousPreheatRect.GetMidY());
        //    if (delta <= CollectionView.Bounds.Height / 3f)
        //        return;

        //    // Compute the assets to start caching and to stop caching.
        //    var rects = ComputeDifferenceBetweenRect(previousPreheatRect, preheatRect);
        //    var addedAssets = rects.added
        //                           .SelectMany(rect => CollectionView.GetIndexPaths(rect))
        //                           .Select(indexPath => FetchResult.ObjectAt(indexPath.Item))
        //                           .Cast<PHAsset>()
        //                           .ToArray();

        //    var removedAssets = rects.removed
        //                             .SelectMany(rect => CollectionView.GetIndexPaths(rect))
        //                             .Select(indexPath => FetchResult.ObjectAt(indexPath.Item))
        //                             .Cast<PHAsset>()
        //                             .ToArray();

        //    // Update the assets the PHCachingImageManager is caching.
        //    imageManager.StartCaching(addedAssets, thumbnailSize, PHImageContentMode.AspectFill, null);
        //    imageManager.StopCaching(removedAssets, thumbnailSize, PHImageContentMode.AspectFill, null);

        //    // Store the preheat rect to compare against in the future.
        //    previousPreheatRect = preheatRect;
        //}

        //(IEnumerable<CGRect> added, IEnumerable<CGRect> removed) ComputeDifferenceBetweenRect(CGRect oldRect, CGRect newRect)
        //{
        //    if (!oldRect.IntersectsWith(newRect))
        //    {
        //        return (new CGRect[] { newRect }, new CGRect[] { oldRect });
        //    }

        //    var oldMaxY = oldRect.GetMaxY();
        //    var oldMinY = oldRect.GetMinY();
        //    var newMaxY = newRect.GetMaxY();
        //    var newMinY = newRect.GetMinY();

        //    var added = new List<CGRect>();
        //    var removed = new List<CGRect>();

        //    if (newMaxY > oldMaxY)
        //        added.Add(new CGRect(newRect.X, oldMaxY, newRect.Width, newMaxY - oldMaxY));

        //    if (oldMinY > newMinY)
        //        added.Add(new CGRect(newRect.X, newMinY, newRect.Width, oldMinY - newMinY));

        //    if (newMaxY < oldMaxY)
        //        removed.Add(new CGRect(newRect.X, newMaxY, newRect.Width, oldMaxY - newMaxY));

        //    if (oldMinY < newMinY)
        //        removed.Add(new CGRect(newRect.X, oldMinY, newRect.Width, newMinY - oldMinY));

        //    return (added, removed);
        //}

        #endregion
    }

    #region Header
    public class Header : UICollectionReusableView
    {
        UILabel label;

        public string Text
        {
            get
            {
                return label.Text;
            }
            set
            {
                label.Text = value;
                SetNeedsDisplay();
            }
        }

        [Export("initWithFrame:")]
        public Header(CGRect frame) : base(frame)
        {
            label = new UILabel() { Frame = new CGRect(0, 0, 300, 50), BackgroundColor = UIColor.Yellow };
            AddSubview(label);
        }
    }
    #endregion

    #region GridLayout
    public class GridLayout : UICollectionViewFlowLayout
    {
        public GridLayout()
        {
        }

        public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
        {
            return true;
        }

        public override UICollectionViewLayoutAttributes LayoutAttributesForItem(NSIndexPath path)
        {
            return base.LayoutAttributesForItem(path);
        }

        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
        {
            return base.LayoutAttributesForElementsInRect(rect);
        }

        //public override SizeF CollectionViewContentSize
        //{
        //    get
        //    {
        //        return CollectionView.Bounds.Size;
        //    }
        //}
    }

    #endregion


    #region LinearLayout

    public class LineLayout : UICollectionViewFlowLayout
    {
        public const float ITEM_SIZE = 200.0f;
        public const int ACTIVE_DISTANCE = 200;
        public const float ZOOM_FACTOR = 0.3f;

        public LineLayout()
        {
            ItemSize = new CGSize(ITEM_SIZE, ITEM_SIZE);
            ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            SectionInset = new UIEdgeInsets(400, 0, 400, 0);
            MinimumLineSpacing = 50.0f;
        }

        public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
        {
            return true;
        }

        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
        {
            var array = base.LayoutAttributesForElementsInRect(rect);
            var visibleRect = new CGRect(CollectionView.ContentOffset, CollectionView.Bounds.Size);

            foreach (var attributes in array)
            {
                if (attributes.Frame.IntersectsWith(rect))
                {
                    float distance = (float)(visibleRect.GetMidX() - attributes.Center.X);
                    float normalizedDistance = distance / ACTIVE_DISTANCE;
                    if (Math.Abs(distance) < ACTIVE_DISTANCE)
                    {
                        float zoom = 1 + ZOOM_FACTOR * (1 - Math.Abs(normalizedDistance));
                        attributes.Transform3D = CATransform3D.MakeScale(zoom, zoom, 1.0f);
                        attributes.ZIndex = 1;
                    }
                }
            }
            return array;
        }

        public override CGPoint TargetContentOffset(CGPoint proposedContentOffset, CGPoint scrollingVelocity)
        {
            float offSetAdjustment = float.MaxValue;
            float horizontalCenter = (float)(proposedContentOffset.X + (this.CollectionView.Bounds.Size.Width / 2.0));
            CGRect targetRect = new CGRect(proposedContentOffset.X, 0.0f, this.CollectionView.Bounds.Size.Width, this.CollectionView.Bounds.Size.Height);
            var array = base.LayoutAttributesForElementsInRect(targetRect);
            foreach (var layoutAttributes in array)
            {
                float itemHorizontalCenter = (float)layoutAttributes.Center.X;
                if (Math.Abs(itemHorizontalCenter - horizontalCenter) < Math.Abs(offSetAdjustment))
                {
                    offSetAdjustment = itemHorizontalCenter - horizontalCenter;
                }
            }
            return new CGPoint(proposedContentOffset.X + offSetAdjustment, proposedContentOffset.Y);
        }

    }
    #endregion

    #region UICollectionViewExtension
    public static class UICollectionViewExtensions
    {
        public static IEnumerable<NSIndexPath> GetIndexPaths(this UICollectionView collectionView, CGRect rect)
        {
            return collectionView.CollectionViewLayout
                                 .LayoutAttributesForElementsInRect(rect)
                                 .Select(attr => attr.IndexPath);
        }
    }
    #endregion


    #region GridViewCell
    public class GridViewCell : UICollectionViewCell
    {
        UIImageView imageView;
        UIImage Image { set { imageView.Image = value; } }


        UIImage thumbnailImage;
        public UIImage ThumbnailImage
        {
            get { return thumbnailImage; }
            set
            {
                thumbnailImage = value;
                Image = thumbnailImage;
            }
        }

        public string RepresentedAssetIdentifier { get; set; }

        [Export("initWithFrame:")]
        public GridViewCell(CGRect frame) : base(frame)
        {
            BackgroundView = new UIView { BackgroundColor = UIColor.Orange };

            SelectedBackgroundView = new UIView { BackgroundColor = UIColor.Green };

            ContentView.Layer.BorderColor = UIColor.LightGray.CGColor;
            ContentView.Layer.BorderWidth = 2.0f;
            ContentView.BackgroundColor = UIColor.White;
            ContentView.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);

            imageView = new UIImageView(UIImage.FromFile("file_empty.png"));
            imageView.Center = ContentView.Center;
            imageView.Transform = CGAffineTransform.MakeScale(0.7f, 0.7f);


            ContentView.AddSubview(imageView);
        }

        //public override void PrepareForReuse()
        //{
        //    base.PrepareForReuse();
        //    //ImageView.Image = null;
        //    Ite2ImageView.Image = null;
        //}
    }
    #endregion


    #region PhotoCell
    public interface IPhoto
    {
        string Name { get; set; }

        UIImage Image { get; set; }
    }

    public class Photo : IPhoto
    {
        public Photo(UIImage img)
        {
            image = img;
        }

        string name = "";
        public string Name { get { return name; } set { name = value; } }

        UIImage image;
        public UIImage Image { get { return image; } set { image = value; } }
    }

    public class PhotoCell : UICollectionViewCell
    {
        UIImageView imageView;

        [Export("initWithFrame:")]
        public PhotoCell(CGRect frame) : base(frame)
        {
            BackgroundView = new UIView { BackgroundColor = UIColor.Orange };

            SelectedBackgroundView = new UIView { BackgroundColor = UIColor.Green };

            ContentView.Layer.BorderColor = UIColor.LightGray.CGColor;
            ContentView.Layer.BorderWidth = 2.0f;
            ContentView.BackgroundColor = UIColor.White;

            NFloat scale = 0.8f;
            ContentView.Transform = CGAffineTransform.MakeScale(scale, scale);

            imageView = new UIImageView(frame);

            imageView.Center = new CGPoint(ContentView.Center.X + frame.Width * (1f - scale) / 2f, ContentView.Center.Y + frame.Width * (1f - scale) / 2f);
            //imageView.Center = ContentView.Center;
            imageView.Transform = CGAffineTransform.MakeScale(0.95f, 0.95f);

            ContentView.AddSubview(imageView);
        }

        public UIImage Image
        {
            set
            {
                imageView.Image = value;
            }
        }

        [Export("custom")]
        void Custom()
        {
            // Put all your custom menu behavior code here
            Console.WriteLine("custom in the cell");
        }


        public override bool CanPerform(Selector action, NSObject withSender)
        {
            if (action == new Selector("custom"))
                return true;
            else
                return false;
        }
    }
    #endregion



    #region PotoTableViewController
    public partial class photoTableViewController : UITableViewController, IPHPhotoLibraryChangeObserver
    {
        //
        //
        public enum Section
        {
            AllPhotos,
            SmartAlbums,
            UserCollections
        }

        const string allPhotosIdentifier = "allPhotos";
        const string collectionIdentifier = "collection";

        const string showAllPhotos = "showAllPhotos";
        const string showCollection = "showCollection";

        PHFetchResult allPhotos;
        PHFetchResult smartAlbums;
        PHFetchResult userCollections;

        readonly string[] sectionLocalizedTitles = { "", "Smart Albums", "Albums" };
        //
        //

        public photoTableViewController() { }


        public UIViewController oldController;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.White;
            Title = "My Custom View Controller";

            //var user = new UIViewController();
            //user.View.BackgroundColor = UIColor.Magenta;

            var btn = UIButton.FromType(UIButtonType.System);
            btn.Frame = new CGRect(20, 80, 280, 44);
            btn.SetTitle("Go Home", UIControlState.Normal);
            // click events
            btn.TouchUpInside += (sender, e) =>
            {
                //this.NavigationController.PushViewController(user, true);
                Ite2DeviceInfoService2.MyWindow.RootViewController = oldController;
            };

            var btn2 = UIButton.FromType(UIButtonType.System);
            btn2.Frame = new CGRect(20, 144, 280, 44);
            btn2.SetTitle("Message", UIControlState.Normal);
            // click events
            btn2.TouchUpInside += (sender, e) =>
            {

                var alertController = UIAlertController.Create("New Album", null, UIAlertControllerStyle.Alert);
                alertController.AddTextField(textField =>
                {
                    textField.Text = "Ha ha...";
                    textField.Placeholder = "Album Name";
                });

                alertController.AddAction(UIAlertAction.Create("Create", UIAlertActionStyle.Default, action =>
                {
                    var textField = alertController.TextFields[0];

                    var title = textField.Text;
                    //if (!string.IsNullOrEmpty(title))
                    //{
                    //    // Create a new album with the title entered.
                    //    PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
                    //    {
                    //        PHAssetCollectionChangeRequest.CreateAssetCollection(title);
                    //    }, (success, error) =>
                    //    {
                    //        if (!success)
                    //            Console.WriteLine($"error creating album: {error}");
                    //    });
                    //}
                }));
                PresentViewController(alertController, true, null);
            };

            //
            //
            //
            //
            var allPhotosOptions = new PHFetchOptions
            {
                SortDescriptors = new NSSortDescriptor[] { new NSSortDescriptor("creationDate", true) },
            };

            allPhotos = PHAsset.FetchAssets(allPhotosOptions);
            //
            //
            //
            //

            allPhotos = PHAsset.FetchAssets(allPhotosOptions);
            smartAlbums = PHAssetCollection.FetchAssetCollections(PHAssetCollectionType.SmartAlbum, PHAssetCollectionSubtype.AlbumRegular, null);
            userCollections = PHCollection.FetchTopLevelUserCollections(null);

            //UITableViewController uiTableController = new UITableViewController()
            //{
            //    Title = "Photos"
            //};

            //TableView = new UITableView(View.Bounds);

            TableView.Source = new TableSource(allPhotos);

            View.AddSubview(btn);
            View.AddSubview(btn2);

        }

        protected override void Dispose(bool disposing)
        {
            //PHPhotoLibrary.SharedPhotoLibrary.UnregisterChangeObserver(this);

            base.Dispose(disposing);
        }


        #region Segues

        //public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        //{
        //    var destination = (segue.DestinationViewController as UINavigationController)?.TopViewController as AssetGridViewController;
        //    if (destination == null)
        //        throw new InvalidProgramException("unexpected view controller for segue");

        //    var cell = sender as UITableViewCell;
        //    if (destination == null)
        //        throw new InvalidProgramException("unexpected cell for segue");

        //    destination.Title = cell.TextLabel.Text;

        //    switch (segue.Identifier)
        //    {
        //        case showAllPhotos:
        //            destination.FetchResult = allPhotos;
        //            break;

        //        case showCollection:
        //            // get the asset collection for the selected row
        //            var indexPath = TableView.IndexPathForCell(cell);
        //            PHCollection collection = null;
        //            switch ((Section)indexPath.Section)
        //            {
        //                case Section.SmartAlbums:
        //                    collection = (PHAssetCollection)smartAlbums.ObjectAt(indexPath.Row);
        //                    break;

        //                case Section.UserCollections:
        //                    collection = (PHCollection)userCollections.ObjectAt(indexPath.Row);
        //                    break;
        //            }

        //            // configure the view controller with the asset collection
        //            var assetCollection = collection as PHAssetCollection;
        //            if (assetCollection == null)
        //                throw new InvalidProgramException("expected asset collection");

        //            destination.FetchResult = PHAsset.FetchAssets(assetCollection, null);
        //            destination.AssetCollection = assetCollection;
        //            break;
        //    }
        //}

        #endregion


        #region Table View

        public override nint NumberOfSections(UITableView tableView)
        {
            return Enum.GetValues(typeof(Section)).Length;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            switch ((Section)(int)section)
            {
                case Section.AllPhotos:
                    return 1;
                case Section.SmartAlbums:
                    return smartAlbums.Count;
                case Section.UserCollections:
                    return userCollections.Count;
            }

            throw new InvalidProgramException();
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            switch ((Section)indexPath.Section)
            {
                case Section.AllPhotos:
                    {
                        var cell = tableView.DequeueReusableCell(allPhotosIdentifier, indexPath);
                        cell.TextLabel.Text = "All Photos";
                        return cell;
                    }
                case Section.SmartAlbums:
                    {
                        var cell = tableView.DequeueReusableCell(collectionIdentifier, indexPath);
                        var collection = (PHCollection)smartAlbums.ObjectAt(indexPath.Row);
                        cell.TextLabel.Text = collection.LocalizedTitle;
                        return cell;
                    }
                case Section.UserCollections:
                    {
                        var cell = tableView.DequeueReusableCell(collectionIdentifier, indexPath);
                        var collection = (PHCollection)userCollections.ObjectAt(indexPath.Row);
                        cell.TextLabel.Text = collection.LocalizedTitle;

                        return cell;
                    }
            }

            throw new InvalidProgramException();
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return sectionLocalizedTitles[(int)section];
        }

        #endregion





        #region IPHPhotoLibraryChangeObserver

        public void PhotoLibraryDidChange(PHChange changeInstance)
        {
            // Change notifications may be made on a background queue. Re-dispatch to the
            // main queue before acting on the change as we'll be updating the UI.
            DispatchQueue.MainQueue.DispatchSync(() =>
            {
                // Check each of the three top-level fetches for changes.

                // Update the cached fetch result. 
                var changeDetails = changeInstance.GetFetchResultChangeDetails(allPhotos);
                if (changeDetails != null)
                {
                    // Update the cached fetch result.
                    allPhotos = changeDetails.FetchResultAfterChanges;
                    // (The table row for this one doesn't need updating, it always says "All Photos".)
                }

                // Update the cached fetch results, and reload the table sections to match.
                changeDetails = changeInstance.GetFetchResultChangeDetails(smartAlbums);
                if (changeDetails != null)
                {
                    smartAlbums = changeDetails.FetchResultAfterChanges;
                    TableView.ReloadSections(NSIndexSet.FromIndex((int)Section.SmartAlbums), UITableViewRowAnimation.Automatic);
                }

                changeDetails = changeInstance.GetFetchResultChangeDetails(userCollections);
                if (changeDetails != null)
                {
                    userCollections = changeDetails.FetchResultAfterChanges;
                    TableView.ReloadSections(NSIndexSet.FromIndex((int)Section.UserCollections), UITableViewRowAnimation.Automatic);
                }
            });
        }

        #endregion
    }

    public class TableSource : UITableViewSource
    {

        PHFetchResult TableItems;
        string CellIdentifier = "TableCell";

        public TableSource(PHFetchResult allItems)
        {
            TableItems = allItems;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return TableItems.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier);
            PHAsset item = (PHAsset)TableItems[indexPath.Row];

            //if there are no cells to reuse, create a new one
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
            }

            cell.TextLabel.Text = item.CreationDate.ToString(); // + " | " + item.Description;

            return cell;
        }
    }
    #endregion

    public class Ite2MediaItem
    {
        public Ite2MediaItem(NSUrl netUri, string disp, string cdate, int id, bool isimg, long size, int width, int height)
        {
            NativeID = netUri;
            DisplayName = disp;
            Id = id;
            IsImage = isimg;
            DateAdded = cdate;
            FileSize = size;
            Width = width;
            Height = height;
        }

        public NSUrl NativeID
        { get; }
        public string DisplayName { get; }
        public int Id { get; }
        public bool IsImage { get; }
        public string DateAdded { get; }
        public long FileSize { get; }
        public int Width { get; }
        public int Height { get; }

        public string LocalFullpath { get; set; }
    }
}