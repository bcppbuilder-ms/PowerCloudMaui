using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Kotlin.Contracts;
using PowerCloud.ViewModels;

namespace PowerCloud.Platforms
{
    public class Ite2DeviceInfoService2 : IIte2DeviceInfo2
    {
        public Task<string?> GetExternalPath(AccountViewModel user)
        {

            //StorageManager mgr = StorageManager.FromContext(Application.Context);
            //IList<StorageVolume> klist = mgr.StorageVolumes;
            //foreach (StorageVolume item in klist)
            //{
            //    string des = item.GetDescription(Application.Context);
            //    item.CreateAccessIntent("").
            //    if (des == "SDCARD")
            //    {
            //        Java.IO.File io = item.Directory;
            //    }
            //}

            string? s = Android.OS.Environment.DirectoryPictures;

            return Task.Run<string?>(() => s);
        }

        public async Task DownloadToDevice(MainNasFileViewModel mvm)
        {
            //Stream androidStream = GetJavaStream(androidStream);
            if (mvm.PayLoad.Count < 1)
                return;

            NASFileViewModel file = mvm.PayLoad[0];
            System.IO.Stream inputStream = await mvm.GetDownloadStream(file);
            downloadFile(mvm.FileSelected, inputStream);
        }



        private bool downloadFile(NASFileViewModel file, System.IO.Stream inputstream)
        {
            ContentValues values = new ContentValues();
            ContentResolver contentResolver = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.ApplicationContext.ContentResolver; //CrossCurrentActivity.Current.AppContext.ContentResolver;

            //var appName = Xamarin.Essentials.AppInfo.Name;
            var xName = AppInfo.PackageName;

            Android.Net.Uri xuriCollection;
            if ((int)Build.VERSION.SdkInt > 29)
                xuriCollection = MediaStore.Downloads.GetContentUri(MediaStore.VolumeExternalPrimary);
            else
                xuriCollection = MediaStore.Downloads.ExternalContentUri;


            values.Put(MediaStore.IMediaColumns.Title, file.Name);
            values.Put(MediaStore.IMediaColumns.MimeType, file.MimeType);
            values.Put(MediaStore.IMediaColumns.Size, file.Size);
            values.Put(MediaStore.IMediaColumns.DisplayName, file.Name);
            //values.Put("_display_name", file.Name);

            //values.Put(MediaStore.Audio.Media.InterfaceConsts.Artist, appName);
            //values.Put(MediaStore.Audio.Media.InterfaceConsts.IsRingtone, true);
            //values.Put(MediaStore.Audio.Media.InterfaceConsts.IsNotification, true);
            //values.Put(MediaStore.Audio.Media.InterfaceConsts.IsAlarm, true);
            //values.Put(MediaStore.Audio.Media.InterfaceConsts.IsMusic, false);




            Android.Net.Uri newUri = contentResolver.Insert(xuriCollection, values);
            System.IO.Stream save;

            save = contentResolver.OpenOutputStream(newUri);
            CopyFile(inputstream, save);

            save.Close();
            inputstream.Close();

            return true;
        }

        #region helper
        void CopyFile(System.IO.Stream instream, System.IO.Stream outstream)
        {
            var buffer = new byte[1024];
            int read;
            while (true)
            {
                read = instream.Read(buffer);
                if (read <= 0)
                    break;
                outstream.Write(buffer, 0, read);
            }
        }

        private void deleteAllMyFiles(Context context)
        {
            Java.IO.File tf2 = context.GetExternalFilesDir(""); // Android.OS.Environment.DirectoryDownloads);
            deleteFiles(tf2);
        }

        private void deleteFiles(Java.IO.File tf2)
        {
            if (tf2.IsDirectory)
            {
                if (tf2.Name.StartsWith("._") || tf2.Name == "lib" || tf2.Name.EndsWith("cache"))
                    return;

                Java.IO.File[] dirs = tf2.ListFiles();
                foreach (Java.IO.File item in dirs)
                    deleteFiles(item);
            }
            else
                tf2.Delete();
        }

        private void copyFromStream(System.IO.Stream strm, string fileName)
        {
            FileStream fi = File.Create(fileName);
            byte[] buff = new byte[4000];

            int r = 4000;
            while (strm.CanRead)
            {
                r = strm.Read(buff, 0, 4000);
                fi.Write(buff, 0, r);
                if (r < 4000)
                    break;
            }
            fi.Close();
        }

        private void readAllSub(Java.IO.File tf1)
        {
            if (tf1.IsDirectory)
            {
                Java.IO.File[] li = tf1.ListFiles();
                if (li == null)
                    return;

                string s = string.Empty;
                foreach (Java.IO.File item in li)
                {
                    s += (item.IsDirectory ? " * " : "   ") + item.Name;
                }

                foreach (Java.IO.File item in li)
                {
                    if (item.IsDirectory)
                        readAllSub(item);
                }
            }
        }
        #endregion

        public Task<Stream?> GetDownloadStream(NASFileViewModel file)
        {
            ContentValues values = new ContentValues();
            ContentResolver? contentResolver = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.ApplicationContext?.ContentResolver; //CrossCurrentActivity.Current.AppContext.ContentResolver;

            //var appName = Xamarin.Essentials.AppInfo.Name;
            var appName = AppInfo.PackageName;


            //string dldir = Environment.DirectoryDownloads;

            //string[] xs1 = Directory.GetFiles(dldir);
            //string[] xs2 = Directory.GetDirectories(dldir);

            Android.Net.Uri xuriCollection;
            if ((int)Build.VERSION.SdkInt > 29)
                xuriCollection = MediaStore.Downloads.GetContentUri(MediaStore.VolumeExternalPrimary);
            else
                xuriCollection = MediaStore.Downloads.ExternalContentUri;

            values.Put(MediaStore.IMediaColumns.Title, file.Name);
            values.Put(MediaStore.IMediaColumns.MimeType, file.MimeType);
            values.Put(MediaStore.IMediaColumns.Size, file.Size);

            values.Put(MediaStore.IMediaColumns.DisplayName, file.Name);
            //values.Put("_display_name", file.Name);

            //values.Put(MediaStore.Audio.Media.InterfaceConsts.Artist, appName);
            //values.Put(MediaStore.Audio.Media.InterfaceConsts.IsRingtone, true);
            //values.Put(MediaStore.Audio.Media.InterfaceConsts.IsNotification, true);
            //values.Put(MediaStore.Audio.Media.InterfaceConsts.IsAlarm, true);
            //values.Put(MediaStore.Audio.Media.InterfaceConsts.IsMusic, false);

            Android.Net.Uri newUri = contentResolver.Insert(xuriCollection, values);

            return Task.Run(() => contentResolver.OpenOutputStream(newUri));
        }

        public /*async*/ void GetPhotoLibrary(MainNasFileViewModel prmMvm)
        {
            ////Ite2.MediaPickerOptions options = new PowerCloud.Droid.Ite2.MediaPickerOptions() { Title = "-Richard-" };
            ////var result = await Ite2.MediaPicker.PickPhotoAsync(options);




            ////string deviceName = Xamarin.Essentials.DeviceInfo.Name;
            ////deviceName = deviceName.Replace(' ', '_');
            ////UpLoadTarget = "home/backup/" + deviceName;




            ////NE201Instance = prmMvm;
            ////if (!await NE201Instance.fmr.NE201IsFileExist(UpLoadTarget))
            ////{
            ////    string[] paths = UpLoadTarget.Split(new char[] { System.IO.Path.DirectorySeparatorChar });
            ////    await prmMvm.fmr.NE201AddFolder("home", string.Join(System.IO.Path.DirectorySeparatorChar, paths, 1, paths.Length - 1));
            ////}
            ////int r = AllMediaFiles.Count;



            var intent = new Intent(Ite2.Platform.CurrentActivity, typeof(ShowPhoto));
            intent.PutExtra("NasViewModel", Ite2.Platform.MainBundle);
            Ite2.Platform.CurrentActivity.StartActivityForResult(intent, 9);

            //GetPhotoLibraryOld(prmMvm).GetAwaiter().GetResult();
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sS">"image|video" + "//" + fileName</param>
        /// <returns></returns>
        public async Task<bool> UploadMediaFile(AlbumItem item)
        {

            Android.Net.Uri mediaUri = MediaStore.Images.Media.ExternalContentUri;
            if (!item.IsImage)
                mediaUri = MediaStore.Video.Media.ExternalContentUri;

            if (await NE201Instance.fmr.NE201IsFileExist(System.IO.Path.Combine(UpLoadTarget, item.FileName)))
            {
                return false;
            }

            Android.Net.Uri kuri = Android.Net.Uri.WithAppendedPath(mediaUri, item.Id);

            string path = Ite2.FileSystem.EnsurePhysicalPath(kuri);

            TimeSpan old = NE201Instance.fmr.SharedHttp.Timeout;
            if (old < TimeSpan.FromMinutes(30))
            {
                NE201Instance.fmr.ResetHttpClient();
                NE201Instance.fmr.SharedHttp.Timeout = TimeSpan.FromMinutes(30);
            }
            //bool result = NE201FileUploadSync(new Ite2.FileResult(path), UpLoadTarget);
            bool result = await NE201Instance.fmr.NE201FileUpload(new FileResult(path), UpLoadTarget);
            if (!result)
            {
                string r = NE201Instance.fmr.ResultMessage;
            }

            //NE201Instance.fmr.SharedHttp.Timeout = old;

            if (File.Exists(path))
                File.Delete(path);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sS">"image|video" + "//" + fileName</param>
        /// <returns></returns>
        public async Task<byte[]> GetThumbnail(AlbumItem item)
        {
            Android.Net.Uri mediaUri = MediaStore.Images.Media.ExternalContentUri;
            if (!item.IsImage)
            {
                mediaUri = MediaStore.Video.Media.ExternalContentUri;
            }
            Android.Net.Uri fileUri = Android.Net.Uri.WithAppendedPath(mediaUri, item.Id);

            ContentResolver contentResolver = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.ApplicationContext.ContentResolver; ; //CrossCurrentActivity.Current.AppContext.ContentResolver;

            //int width = item.Width;
            //int height = item.Height;

            Android.Util.Size thumbSize;
            int sizeFactor = 480;
            //////if (width > height)
            //////{
            //////    thumbSize = new Size(sizeFactor, (int)(sizeFactor * (float)(height / (float)width)));
            //////}
            //////else if (height > width)
            //////{
            //////    thumbSize = new Size((int)(sizeFactor * (float)(width / (float)height)), sizeFactor);
            //////}
            //////else
            thumbSize = new Android.Util.Size(sizeFactor, sizeFactor);

            try
            {
                Bitmap x = contentResolver.LoadThumbnail(fileUri, thumbSize, null);

                MemoryStream thumbnailBytes = new MemoryStream();
                await x.CompressAsync(Bitmap.CompressFormat.Jpeg, 25, thumbnailBytes);
                return thumbnailBytes.ToArray();
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        public async Task<bool> CopyToLocal(AlbumItem item, string target)
        {
            bool result = true;

            Android.Net.Uri mediaUri = MediaStore.Images.Media.ExternalContentUri;
            if (!item.IsImage)
            {
                mediaUri = MediaStore.Video.Media.ExternalContentUri;
            }

            Android.Net.Uri fileUri = Android.Net.Uri.WithAppendedPath(mediaUri, item.Id);

            //ContentResolver contentResolver = CrossCurrentActivity.Current.AppContext.ContentResolver;
            string path = Ite2.FileSystem.EnsurePhysicalPath(fileUri);

            //bool result = NE201FileUploadSync(new Ite2.FileResult(path), UpLoadTarget);
            //Xamarin.Essentials.FileResult xf = new Xamarin.Essentials.FileResult(path);
            FileResult xf = new FileResult(path);

            System.IO.Stream source = await xf.OpenReadAsync();

            System.IO.Stream newFile = new FileStream(target, FileMode.Create);
            source.CopyTo(newFile);


            if (File.Exists(path))
                File.Delete(path);

            return result;
        }








        public static string UpLoadTarget = "home/backup/thisPhone";
        public static MainNasFileViewModel NE201Instance;

        public /*async*/ void GetPhotoLibraryOld(MainNasFileViewModel prmMvm)
        {

            //PowerCloud.Droid.Ite2.MediaPickerOptions options = new PowerCloud.Droid.Ite2.MediaPickerOptions() { Title = "-Richard-" };
            //var result = await PowerCloud.Droid.Ite2.MediaPicker.PickPhotoAsync(options);


            #region just for study
            //int n = 0;
            //int ptr = 0;
            //int withData = 0;
            //string s = "";
            //string s2 = "";
            #endregion

            foreach (Ite2MediaItem item in AllMediaFiles)
            {
                #region just for study
                //s += item.Scheme + " : //" + item.Authority + item.Path + "?" + item.Query + "#" + item.Fragment + " \r\n";
                ////s2 += item.UserInfo + "@" + item.Host + ":" + item.Port + "/r/n";
                //s2 += fnames[ptr++] + "\r\n"; 


                //////contentResolver.LoadThumbnail(item, new Android.Util.Size(96, 96), null);


                //n++;
                //AssetFileDescriptor xfile = contentResolver.OpenAssetFileDescriptor(item, "r");
                //ExifInterface ifx = new ExifInterface(xfile.FileDescriptor);
                //if (ifx.HasThumbnail)
                //    withData++;
                ////byte[] r = ifx.GetThumbnail();
                ////if (r.Length > 0)
                ////    withData++;
                #endregion
                string path = Ite2.FileSystem.EnsurePhysicalPath(item.AndroidUri);

                //prmMvm.fmr.NE201FileUpload(new Ite2.FileResult(path), UpLoadTarget).GetAwaiter().GetResult();
            }


        }

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

        public static void ListPhotoFiles()
        {
            var context = Ite2.Platform.AppContext;
            var packageInfo = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.Permissions);
            var requestedPermissions = packageInfo?.RequestedPermissions;

            var uri = MediaStore.Images.Media.ExternalContentUri;
            //var uri = MediaStore.Images.Media.InternalContentUri;
            ContentResolver contentResolver = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.ApplicationContext.ContentResolver; // CrossCurrentActivity.Current.AppContext.ContentResolver;

            List<Ite2MediaItem> listOfAllImages = new List<Ite2MediaItem>();
            listPhotoIds.Item1 = new List<int>();
            listPhotoIds.Item2 = new List<string>();

            string[] projection = { MediaStore.IMediaColumns.DisplayName, IBaseColumns.Id, MediaStore.IMediaColumns.Size,
                //MediaStore.IMediaColumns.Data,
                MediaStore.IMediaColumns.DateModified, MediaStore.IMediaColumns.Width, MediaStore.IMediaColumns.Height };

            var cursor = contentResolver.Query(uri, projection, null, null, null);

            int column_index_file_name = cursor.GetColumnIndexOrThrow(MediaStore.IMediaColumns.DisplayName);

            while (cursor.MoveToNext())
            {
                int id = cursor.GetInt(cursor.GetColumnIndexOrThrow(IBaseColumns.Id));
                string ss = cursor.GetString(column_index_file_name);
                Android.Net.Uri kuri = Android.Net.Uri.WithAppendedPath(MediaStore.Images.Media.ExternalContentUri, id.ToString());

                //string filename = cursor.GetString(cursor.GetColumnIndexOrThrow(MediaStore.IMediaColumns.Data));
                string sizeStr = cursor.GetString(cursor.GetColumnIndexOrThrow(MediaStore.IMediaColumns.Size));
                long size = long.Parse(sizeStr);
                //string filename = cursor.GetString(cursor.GetColumnIndexOrThrow(MediaStore.IMediaColumns.Data));
                //long size = new FileInfo(filename).Length;

                //string cdate = File.GetCreationTime(filename).ToString("yyyy/MM/dd - HH:mm:ss");
                string cdate = cursor.GetString(cursor.GetColumnIndexOrThrow(MediaStore.IMediaColumns.DateModified));
                long datelong = long.Parse(cdate);
                TimeSpan tmpDate = TimeSpan.FromSeconds(datelong);
                cdate = (new DateTime(1970, 1, 1, 8, 0, 0) + tmpDate).ToString("yyyy/MM/dd - HH:mm:ss");

                int width = cursor.GetInt(cursor.GetColumnIndexOrThrow(MediaStore.IMediaColumns.Width));
                int height = cursor.GetInt(cursor.GetColumnIndexOrThrow(MediaStore.IMediaColumns.Height));

                Ite2MediaItem xMedia = new Ite2MediaItem(kuri, ss, cdate, id, true, size, width, height);
                xMedia.LocalFullpath = ss;
                listOfAllImages.Add(xMedia);
                listPhotoIds.Item1.Add(id);
                ListPhotoIds.Item2.Add(ss);  // + "||" + ss);
            }





            uri = MediaStore.Video.Media.ExternalContentUri;
            //uri = MediaStore.Video.Media.InternalContentUri;

            projection = new string[] { MediaStore.IMediaColumns.DisplayName, IBaseColumns.Id, MediaStore.IMediaColumns.Size,
                //MediaStore.IMediaColumns.Data,
                MediaStore.IMediaColumns.DateModified, MediaStore.IMediaColumns.Width, MediaStore.IMediaColumns.Height };

            cursor = contentResolver.Query(uri, projection, null, null, null);

            column_index_file_name = cursor.GetColumnIndexOrThrow(MediaStore.IMediaColumns.DisplayName);

            while (cursor.MoveToNext())
            {
                int id = cursor.GetInt(cursor.GetColumnIndexOrThrow(IBaseColumns.Id));
                string ss = cursor.GetString(column_index_file_name);
                Android.Net.Uri kuri = Android.Net.Uri.WithAppendedPath(MediaStore.Video.Media.ExternalContentUri, id.ToString());

                //string filename = cursor.GetString(cursor.GetColumnIndexOrThrow(MediaStore.IMediaColumns.Data));
                string sizeStr = cursor.GetString(cursor.GetColumnIndexOrThrow(MediaStore.IMediaColumns.Size));
                long size = long.Parse(sizeStr);
                //string filename = cursor.GetString(cursor.GetColumnIndexOrThrow(MediaStore.IMediaColumns.Data));
                //long size = new FileInfo(filename).Length;

                //string cdate = File.GetCreationTime(filename).ToString("yyyy/MM/dd - HH:mm:ss");
                string cdate = cursor.GetString(cursor.GetColumnIndexOrThrow(MediaStore.IMediaColumns.DateModified));
                long datelong = long.Parse(cdate);
                TimeSpan tmpDate = TimeSpan.FromSeconds(datelong);
                cdate = (new DateTime(1970, 1, 1, 8, 0, 0) + tmpDate).ToString("yyyy/MM/dd - HH:mm:ss");

                int width = cursor.GetInt(cursor.GetColumnIndexOrThrow(MediaStore.IMediaColumns.Width));
                int height = cursor.GetInt(cursor.GetColumnIndexOrThrow(MediaStore.IMediaColumns.Height));

                Ite2MediaItem xMedia = new Ite2MediaItem(kuri, ss, cdate, id, false, size, width, height);
                xMedia.LocalFullpath = ss;
                listOfAllImages.Add(xMedia);
            }
            if (allMedias != null)
                allMedias.Clear();

            allMedias = listOfAllImages;
        }

        public static (List<int>, List<string>) listPhotoIds;

        public static (List<int>, List<string>) ListPhotoIds { get { return listPhotoIds; } }

        public string SelectedAccessToken { get; set; } = string.Empty;
        public string SelectedRefreshToken { get; set; } = string.Empty;
        public string SelectedNasAddress { get; set; } = string.Empty;
    }

    public class Ite2MediaItem
    {
        public Ite2MediaItem(Android.Net.Uri netUri, string disp, string cdate, int id, bool isimg, long size, int width, int height)
        {
            AndroidUri = netUri;
            DisplayName = disp;
            Id = id;
            IsImage = isimg;
            DateAdded = cdate;
            FileSize = size;
            Width = width;
            Height = height;
        }

        public Android.Net.Uri AndroidUri
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
