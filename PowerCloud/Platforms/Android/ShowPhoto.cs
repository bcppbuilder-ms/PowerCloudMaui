using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace PowerCloud.Platforms
{
    [Activity(Label = "ShowPhoto")]
    public class ShowPhoto : Activity
    {
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        PhotoAlbumAdapter mAdapter;
        PhotoAlbum mPhotoAlbum;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here


            var context = Ite2.Platform.AppContext;
            var packageInfo = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.Permissions);
            var requestedPermissions = packageInfo?.RequestedPermissions;

            //////var uri = MediaStore.Images.Media.ExternalContentUri;
            //////ContentValues values = new ContentValues();
            //////ContentResolver contentResolver = CrossCurrentActivity.Current.AppContext.ContentResolver;

            ////////List<string> myPhotos = ListPhotoFiles();
            //////(List<int> myPhotos, List<string> fnames) = Ite2DeviceInfoService.ListPhotoIds;

            //////SetContentView(Resource.Layout.myTest);
            mPhotoAlbum = new PhotoAlbum();

            SetContentView(Resource.Layout.myTest);

            // Get our RecyclerView layout:
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            //............................................................
            // Layout Manager Setup:

            // Use the built-in linear layout manager:
            //mLayoutManager = new LinearLayoutManager(this);
            mLayoutManager = new GridLayoutManager(this, 3);

            // Or use the built-in grid layout manager (two horizontal rows):
            // mLayoutManager = new GridLayoutManager
            //        (this, 2, GridLayoutManager.Horizontal, false);

            // Plug the layout manager into the RecyclerView:
            mRecyclerView.SetLayoutManager(mLayoutManager);

            //............................................................
            // Adapter Setup:

            // Create an adapter for the RecyclerView, and pass it the
            // data set (the photo album) to manage:
            mAdapter = new PhotoAlbumAdapter(mPhotoAlbum);

            // Register the item click handler (below) with the adapter:
            mAdapter.ItemClick += OnItemClick;

            // Plug the adapter into the RecyclerView:
            mRecyclerView.SetAdapter(mAdapter);

            //............................................................
            // Random Pick Button:

            // Get the button for randomly swapping a photo:
            Android.Widget.Button QuitBtn = FindViewById<Android.Widget.Button>(Resource.Id.quitButton);

            // Handler for the Random Pick Button:
            QuitBtn.Click += delegate
            {
                if (mPhotoAlbum != null)
                {
                    Finish();
                }
            };

            FindViewById<Android.Widget.Button>(Resource.Id.goHome_Button).Click += (sender, args) =>
            {
                //var intent = new Intent(this, typeof(SecondActivity));
                //StartActivity(intent);

                //int n = 0;
                foreach (Ite2MediaItem item in Ite2DeviceInfoService2.AllMediaFiles)
                {
                    ////if (Ite2DeviceInfoService.NE201Instance.fmr.NE201IsFileExistSync(Path.Combine(Ite2DeviceInfoService.UpLoadTarget, item.DisplayName))
                    ////    || item.IsImage)
                    ////{
                    ////    continue;
                    ////}
                    ////string path = Ite2.FileSystem.EnsurePhysicalPath(item.AndroidUri);

                    //////if (!item.IsImage)
                    //////    Toast.MakeText(this, $"{++n} video have been uploaded.", ToastLength.Short).Show();
                    ////Ite2.FileResult x = new Ite2.FileResult(path);
                    ////bool isDone = Ite2DeviceInfoService.NE201FileUpload(x, Ite2DeviceInfoService.UpLoadTarget);

                    //if (await Ite2DeviceInfoService.NE201Instance.fmr.NE201IsFileExist(Path.Combine(Ite2DeviceInfoService.UpLoadTarget, item.DisplayName))
                    //    || item.IsImage)
                    //{
                    //    continue;
                    //}

                    //string path = Ite2.FileSystem.EnsurePhysicalPath(item.AndroidUri);

                    //Ite2.FileResult x = new Ite2.FileResult(path);
                    //bool isDone = await Ite2DeviceInfoService.NE201FileUpload(x, Ite2DeviceInfoService.UpLoadTarget);

                    //if (File.Exists(path))
                    //    File.Delete(path);
                }

                //Toast.MakeText(this, "Photo Total: " + Ite2DeviceInfoService.AllMediaFiles.Count + " have been uploaded.", ToastLength.Long).Show();
                Finish();
            };

            Android.Widget.Toast.MakeText(this, "Photo Total: " + Ite2DeviceInfoService2.AllMediaFiles.Count, ToastLength.Long).Show();
        }

        void OnItemClick(object sender, int position)
        {
            // Display a toast that briefly shows the enumeration of the selected photo:
            int photoNum = position + 1;
            Android.Widget.Toast.MakeText(this, "This is photo number " + photoNum, ToastLength.Short).Show();
        }
    }

    [Activity(Label = "myTest")]
    public class myTest : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.myTest);
            FindViewById<Android.Widget.Button>(Resource.Id.goHome_Button).Click += (sender, args) =>
            {
                //var intent = new Intent(this, typeof(SecondActivity));
                //StartActivity(intent);
                this.Finish();
            };

            //SetContentView(goHomeButton);

        }
    }
}
