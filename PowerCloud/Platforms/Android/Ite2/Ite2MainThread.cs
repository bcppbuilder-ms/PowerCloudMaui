using Android.OS;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PowerCloud.Ite2
{
    public static partial class MainThread
    {
        public static bool IsMainThread => PlatformIsMainThread;


        public static void BeginInvokeOnMainThread(Action action)
        {
            if (IsMainThread)
            {
                action();
            }
            else
            {
                PlatformBeginInvokeOnMainThread(action);
            }
        }


        /// <summary>
        /// important field
        /// </summary>
        static volatile Handler handler;

        static bool PlatformIsMainThread
        {
            get
            {
                if (Platform.HasApiLevel(BuildVersionCodes.M))
                    return Looper.MainLooper.IsCurrentThread;

                return Looper.MyLooper() == Looper.MainLooper;
            }
        }

        /// <summary>
        /// kernel function
        /// </summary>
        /// <param name="action"></param>
        static void PlatformBeginInvokeOnMainThread(Action action)
        {
            if (handler?.Looper != Looper.MainLooper)
                handler = new Handler(Looper.MainLooper);

            handler.Post(action);
        }


        /// <summary>
        /// learn how to use TaskCompletionSource
        /// and. TrySetResult();
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Task InvokeOnMainThreadAsync(Action action)
        {
            if (IsMainThread)
            {
                action();
#if NETSTANDARD1_0
                return Task.FromResult(true);
#else
                return Task.CompletedTask;
#endif
            }

            var tcs = new TaskCompletionSource<bool>();

            BeginInvokeOnMainThread(() =>
            {
                try
                {
                    action();
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });

            return tcs.Task;
        }

        public static Task<T> InvokeOnMainThreadAsync<T>(Func<T> func)
        {
            if (IsMainThread)
            {
                return Task.FromResult(func());
            }

            var tcs = new TaskCompletionSource<T>();

            BeginInvokeOnMainThread(() =>
            {
                try
                {
                    var result = func();
                    tcs.TrySetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });

            return tcs.Task;
        }

        public static Task InvokeOnMainThreadAsync(Func<Task> funcTask)
        {
            if (IsMainThread)
            {
                return funcTask();
            }

            var tcs = new TaskCompletionSource<object>();

            BeginInvokeOnMainThread(
                async () =>
                {
                    try
                    {
                        await funcTask().ConfigureAwait(false);
                        tcs.SetResult(null);
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                });

            return tcs.Task;
        }

        public static Task<T> InvokeOnMainThreadAsync<T>(Func<Task<T>> funcTask)
        {
            if (IsMainThread)
            {
                return funcTask();
            }

            var tcs = new TaskCompletionSource<T>();

            BeginInvokeOnMainThread(
                async () =>
                {
                    try
                    {
                        var ret = await funcTask().ConfigureAwait(false);
                        tcs.SetResult(ret);
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                });

            return tcs.Task;
        }

        public static async Task<SynchronizationContext> GetMainThreadSynchronizationContextAsync()
        {
            SynchronizationContext ret = null;
            await InvokeOnMainThreadAsync(() =>
                ret = SynchronizationContext.Current).ConfigureAwait(false);
            return ret;
        }
    }
}