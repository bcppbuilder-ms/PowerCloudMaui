using Android.Content.PM;
using Android.OS;
using Android.Runtime;

#if __ANDROID_29__
using AndroidX.Core.App;
using AndroidX.Core.Content;
#else
using Android.Support.V4.App;
using Android.Support.V4.Content;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace PowerCloud.Ite2
{
    public static partial class Permissions
    {
        static object locker = new object();



        public static bool IsDeclaredInManifest(string permission)
        {
            var context = Platform.AppContext;
            var packageInfo = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.Permissions);
            var requestedPermissions = packageInfo?.RequestedPermissions;

            return requestedPermissions?.Any(r => r.Equals(permission, StringComparison.OrdinalIgnoreCase)) ?? false;
        }



        internal static void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
            => BasePlatformPermission.OnRequestPermissionsResult(requestCode, permissions, grantResults);


        public abstract partial class BasePlatformPermission : BasePermission
        {
            /// <summary>
            /// All requests
            /// </summary>
            static readonly Dictionary<string, (int requestCode, TaskCompletionSource<PermissionStatus> tcs)> requests =
                   new Dictionary<string, (int, TaskCompletionSource<PermissionStatus>)>();

            static readonly object locker = new object();
            static int requestCode;

            public virtual (string androidPermission, bool isRuntime)[] RequiredPermissions { get; }

            public override Task<PermissionStatus> CheckStatusAsync()
            {
                if (RequiredPermissions == null || RequiredPermissions.Length <= 0)
                    return Task.FromResult(PermissionStatus.Granted);

                var context = Platform.AppContext;
                var targetsMOrHigher = context.ApplicationInfo.TargetSdkVersion >= BuildVersionCodes.M;

                foreach (var (androidPermission, isRuntime) in RequiredPermissions)
                {
                    var ap = androidPermission;
                    if (!IsDeclaredInManifest(ap))
                        throw new PermissionException($"You need to declare using the permission: `{androidPermission}` in your AndroidManifest.xml");

                    var status = PermissionStatus.Granted;

                    if (targetsMOrHigher)
                    {
                        if (ContextCompat.CheckSelfPermission(context, androidPermission) != Permission.Granted)
                            status = PermissionStatus.Denied;
                    }
                    else
                    {
                        if (PermissionChecker.CheckSelfPermission(context, androidPermission) != PermissionChecker.PermissionGranted)
                            status = PermissionStatus.Denied;
                    }

                    if (status != PermissionStatus.Granted)
                        return Task.FromResult(PermissionStatus.Denied);
                }

                return Task.FromResult(PermissionStatus.Granted);
            }

            public override async Task<PermissionStatus> RequestAsync()
            {
                // Check status before requesting first
                if (await CheckStatusAsync() == PermissionStatus.Granted)
                    return PermissionStatus.Granted;

                TaskCompletionSource<PermissionStatus> tcs;
                var doRequest = true;

                var runtimePermissions = RequiredPermissions.Where(p => p.isRuntime)
                    ?.Select(p => p.androidPermission)?.ToArray();

                // We may have no runtime permissions required, in this case
                // knowing they all exist in the manifest from the Check call above is sufficient
                if (runtimePermissions == null || !runtimePermissions.Any())
                    return PermissionStatus.Granted;

                var permissionId = string.Join(';', runtimePermissions);

                lock (locker)
                {
                    if (requests.ContainsKey(permissionId))
                    {
                        tcs = requests[permissionId].tcs;
                        doRequest = false;
                    }
                    else
                    {
                        tcs = new TaskCompletionSource<PermissionStatus>();

                        requestCode = Platform.NextRequestCode();

                        requests.Add(permissionId, (requestCode, tcs));
                    }
                }

                if (!doRequest)
                    return await tcs.Task;

                if (!MainThread.IsMainThread)
                    throw new PermissionException("Permission request must be invoked on main thread.");

                ActivityCompat.RequestPermissions(Platform.GetCurrentActivity(true), runtimePermissions.ToArray(), requestCode);

                var result = await tcs.Task;

                if (requests.ContainsKey(permissionId))
                    requests.Remove(permissionId);

                return result;
            }

            public override void EnsureDeclared()
            {
                if (RequiredPermissions == null || RequiredPermissions.Length <= 0)
                    return;

                foreach (var (androidPermission, isRuntime) in RequiredPermissions)
                {
                    var ap = androidPermission;
                    if (!IsDeclaredInManifest(ap))
                        throw new PermissionException($"You need to declare using the permission: `{androidPermission}` in your AndroidManifest.xml");
                }
            }

            public override bool ShouldShowRationale()
            {
                if (RequiredPermissions == null || RequiredPermissions.Length <= 0)
                    return false;

                var activity = Platform.GetCurrentActivity(true);
                foreach (var (androidPermission, isRuntime) in RequiredPermissions)
                {
                    if (isRuntime && ActivityCompat.ShouldShowRequestPermissionRationale(activity, androidPermission))
                        return true;
                }

                return false;
            }

            internal static void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
            {
                lock (locker)
                {
                    // Check our pending requests for one with a matching request code
                    foreach (var kvp in requests)
                    {
                        if (kvp.Value.requestCode == requestCode)
                        {
                            var tcs = kvp.Value.tcs;

                            // Look for any denied requests, and deny the whole request if so
                            // Remember, each PermissionType is tied to 1 or more android permissions
                            // so if any android permissions denied the whole PermissionType is considered denied
                            if (grantResults.Any(g => g == Permission.Denied))
                                tcs.TrySetResult(PermissionStatus.Denied);
                            else
                                tcs.TrySetResult(PermissionStatus.Granted);
                            break;
                        }
                    }
                }
            }
        }


        #region on Share path
        public static Task<PermissionStatus> CheckStatusAsync<TPermission>()
            where TPermission : BasePermission, new() =>
                new TPermission().CheckStatusAsync();

            public static Task<PermissionStatus> RequestAsync<TPermission>()
                where TPermission : BasePermission, new() =>
                    new TPermission().RequestAsync();

            public static bool ShouldShowRationale<TPermission>()
                where TPermission : BasePermission, new() =>
                    new TPermission().ShouldShowRationale();

            internal static void EnsureDeclared<TPermission>()
                where TPermission : BasePermission, new() =>
                    new TPermission().EnsureDeclared();

            internal static async Task EnsureGrantedAsync<TPermission>()
                where TPermission : BasePermission, new()
            {
                var status = await RequestAsync<TPermission>();

                if (status != PermissionStatus.Granted)
                    throw new PermissionException($"{typeof(TPermission).Name} permission was not granted: {status}");
            }

            public abstract partial class BasePermission
            {
                [Preserve]
                public BasePermission()
                {
                }

                public abstract Task<PermissionStatus> CheckStatusAsync();

                public abstract Task<PermissionStatus> RequestAsync();

                public abstract void EnsureDeclared();

                public abstract bool ShouldShowRationale();
            }

            public partial class Battery
            {
            }

            public partial class CalendarRead
            {
            }

            public partial class CalendarWrite
            {
            }

            public partial class Camera
            {
            }

            public partial class ContactsRead
            {
            }

            public partial class ContactsWrite
            {
            }

            public partial class Flashlight
            {
            }

            public partial class LaunchApp
            {
            }

            public partial class LocationWhenInUse
            {
            }

            public partial class LocationAlways
            {
            }

            public partial class Maps
            {
            }

            public partial class Media
            {
            }

            public partial class Microphone
            {
            }

            public partial class NetworkState
            {
            }

            public partial class Phone
            {
            }

            public partial class Photos
            {
            }

            public partial class Reminders
            {
            }

            public partial class Sensors
            {
            }

            public partial class Sms
            {
            }

            public partial class Speech
            {
            }

            public partial class StorageRead
            {
            }

            public partial class StorageWrite
            {
            }

            public partial class Vibrate
            {
            }
        #endregion

        public partial class Battery : BasePlatformPermission
        {
        }

        public partial class CalendarRead : BasePlatformPermission
        {
        }

        public partial class CalendarWrite : BasePlatformPermission
        {
        }

        public partial class Camera : BasePlatformPermission
        {
        }

        public partial class ContactsRead : BasePlatformPermission
        {
        }

        public partial class ContactsWrite : BasePlatformPermission
        {
        }

        public partial class Flashlight : BasePlatformPermission
        {
        }

        public partial class LaunchApp : BasePlatformPermission
        {
        }

        public partial class LocationWhenInUse : BasePlatformPermission
        {
        }

        public partial class LocationAlways : BasePlatformPermission
        {
        }

        public partial class Maps : BasePlatformPermission
        {
        }

        public partial class Media : BasePlatformPermission
        {
        }

        public partial class Microphone : BasePlatformPermission
        {
        }

        public partial class NetworkState : BasePlatformPermission
        {
        }

        public partial class Phone : BasePlatformPermission
        {
        }

        public partial class Photos : BasePlatformPermission
        {
        }

        public partial class Reminders : BasePlatformPermission
        {
        }

        public partial class Sensors : BasePlatformPermission
        {
        }

        public partial class Sms : BasePlatformPermission
        {
        }

        public partial class Speech : BasePlatformPermission
        {
        }

        public partial class StorageRead : BasePlatformPermission
        {
        }

        public partial class StorageWrite : BasePlatformPermission
        {
        }

        public partial class Vibrate : BasePlatformPermission
        {
        }
    }


    public class PermissionException : UnauthorizedAccessException
    {
        public PermissionException(string message)
            : base(message)
        {
        }
    }


    public enum PermissionStatus
    {
        // Permission is in an unknown state
        Unknown = 0,

        // Denied by user
        Denied = 1,

        // Feature is disabled on device
        Disabled = 2,

        // Granted by user
        Granted = 3,

        // Restricted (only iOS)
        Restricted = 4
    }
}