using Android.Content;
using Android.Content.PM;
using Android.Provider;
using AndroidUri = Android.Net.Uri;

namespace PowerCloud.Ite2
{
    /// <summary>
    /// Intent: An intent is an abstract description of an operation to be performed.
    /// </summary>
    public static class MediaPicker
    {
        static bool PlatformIsCaptureSupported
            => Platform.AppContext.PackageManager.HasSystemFeature(PackageManager.FeatureCameraAny);

        static Task<FileResult> PlatformPickPhotoAsync(MediaPickerOptions options)
            => PlatformPickAsync(options, true);

        static Task<FileResult> PlatformPickVideoAsync(MediaPickerOptions options)
            => PlatformPickAsync(options, false);


        static async Task<FileResult> PlatformPickAsync(MediaPickerOptions options, bool photo)
        {
            // We only need the permission when accessing the file, but it's more natural
            // to ask the user first, then show the picker.
            await Permissions.EnsureGrantedAsync<Permissions.StorageRead>();

            
            var intent = new Intent(Intent.ActionGetContent);
            intent.SetType(photo ? FileSystem.MimeTypes.ImageAll : FileSystem.MimeTypes.VideoAll);

            var pickerIntent = Intent.CreateChooser(intent, options?.Title);

            try
            {
                string path = null;
                void OnResult(Intent intent)
                {
                    // The uri returned is only temporary and only lives as long as the Activity that requested it,
                    // so this means that it will always be cleaned up by the time we need it because we are using
                    // an intermediate activity.

                    path = FileSystem.EnsurePhysicalPath(intent.Data);
                }

                await IntermediateActivity.StartAsync(pickerIntent, Platform.requestCodeMediaPicker, onResult: OnResult);

                return new FileResult(path);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        static Task<FileResult> PlatformCapturePhotoAsync(MediaPickerOptions options)
            => PlatformCaptureAsync(options, true);

        static Task<FileResult> PlatformCaptureVideoAsync(MediaPickerOptions options)
            => PlatformCaptureAsync(options, false);

        static async Task<FileResult> PlatformCaptureAsync(MediaPickerOptions options, bool photo)
        {
            await Permissions.EnsureGrantedAsync<Permissions.Camera>();
            await Permissions.EnsureGrantedAsync<Permissions.StorageWrite>();

            var capturePhotoIntent = new Intent(photo ? MediaStore.ActionImageCapture : MediaStore.ActionVideoCapture);

            if (!Platform.IsIntentSupported(capturePhotoIntent))
                throw new FeatureNotSupportedException($"Either there was no camera on the device or '{capturePhotoIntent.Action}' was not added to the <queries> element in the app's manifest file. See more: https://developer.android.com/about/versions/11/privacy/package-visibility");

            capturePhotoIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
            capturePhotoIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);

            try
            {
                var activity = Platform.GetCurrentActivity(true);

                // Create the temporary file
                var ext = photo
                    ? FileSystem.Extensions.Jpg
                    : FileSystem.Extensions.Mp4;
                var fileName = Guid.NewGuid().ToString("N") + ext;
                var tmpFile = FileSystem.GetEssentialsTemporaryFile(Platform.AppContext.CacheDir, fileName);

                // Set up the content:// uri
                AndroidUri outputUri = null;
                void OnCreate(Intent intent)
                {
                    // Android requires that using a file provider to get a content:// uri for a file to be called
                    // from within the context of the actual activity which may share that uri with another intent
                    // it launches.

                    //outputUri ??= Xamarin.Essentials.FileProvider.GetUriForFile(Platform.AppContext, Platform.AppContext.PackageName + ".fileProvider", tmpFile);
                    outputUri = AndroidX.Core.Content.FileProvider.GetUriForFile(activity, Platform.AppContext.PackageName + ".fileProvider", tmpFile);

                    intent.PutExtra(MediaStore.ExtraOutput, outputUri);
                }

                // Start the capture process
                await IntermediateActivity.StartAsync(capturePhotoIntent, Platform.requestCodeMediaCapture, OnCreate);

                // Return the file that we just captured
                return new FileResult(tmpFile.AbsolutePath);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }


        #region Shared <-- Entry Point of PickPhotoAsync
        public static bool IsCaptureSupported
            => PlatformIsCaptureSupported;

        public static Task<FileResult> PickPhotoAsync(MediaPickerOptions options = null) =>
            PlatformPickPhotoAsync(options);

        public static Task<FileResult> CapturePhotoAsync(MediaPickerOptions options = null)
        {
            if (!IsCaptureSupported)
                throw new FeatureNotSupportedException();

            return PlatformCapturePhotoAsync(options);
        }




        public static Task<FileResult> PickVideoAsync(MediaPickerOptions options = null) =>
            PlatformPickVideoAsync(options);

        public static Task<FileResult> CaptureVideoAsync(MediaPickerOptions options = null)
        {
            if (!IsCaptureSupported)
                throw new FeatureNotSupportedException();

            return PlatformCaptureVideoAsync(options);
        }
        #endregion
    }

    public class MediaPickerOptions
    {
        public string Title { get; set; }
    }

    static class ExceptionUtils
    {
#if NETSTANDARD1_0 || NETSTANDARD2_0
        internal static NotImplementedInReferenceAssemblyException NotSupportedOrImplementedException =>
            new NotImplementedInReferenceAssemblyException();
#else
        internal static FeatureNotSupportedException NotSupportedOrImplementedException =>
            new FeatureNotSupportedException($"This API is not supported on Androis Device");
#endif

    }

    public class NotImplementedInReferenceAssemblyException : NotImplementedException
    {
        public NotImplementedInReferenceAssemblyException()
            : base("This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.")
        {
        }
    }

    public class FeatureNotSupportedException : NotSupportedException
    {
        public FeatureNotSupportedException()
        {
        }

        public FeatureNotSupportedException(string message)
            : base(message)
        {
        }

        public FeatureNotSupportedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class FeatureNotEnabledException : InvalidOperationException
    {
        public FeatureNotEnabledException()
        {
        }

        public FeatureNotEnabledException(string message)
            : base(message)
        {
        }

        public FeatureNotEnabledException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}