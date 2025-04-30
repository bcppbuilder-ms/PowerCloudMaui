using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;

namespace PowerCloud.Ite2
{
    public static class AppActions
    {
        internal static bool PlatformIsSupported => Platform.HasApiLevelNMr1;

        static Task<IEnumerable<AppAction>> PlatformGetAsync()
        {
            if (!IsSupported)
                throw new FeatureNotSupportedException();

#if __ANDROID_25__
            return Task.FromResult(Platform.ShortcutManager.DynamicShortcuts.Select(s => s.ToAppAction()));
#else
            return Task.FromResult<IEnumerable<AppAction>>>(null);
#endif
        }

        static Task PlatformSetAsync(IEnumerable<AppAction> actions)
        {
            if (!IsSupported)
                throw new FeatureNotSupportedException();

#if __ANDROID_25__
            Platform.ShortcutManager.SetDynamicShortcuts(actions.Select(a => a.ToShortcutInfo()).ToList());
#endif
            return Task.CompletedTask;
        }

        static AppAction ToAppAction(this ShortcutInfo shortcutInfo) =>
            new AppAction(shortcutInfo.Id, shortcutInfo.ShortLabel, shortcutInfo.LongLabel);

        const string extraAppActionId = "EXTRA_XE_APP_ACTION_ID";
        const string extraAppActionTitle = "EXTRA_XE_APP_ACTION_TITLE";
        const string extraAppActionSubtitle = "EXTRA_XE_APP_ACTION_SUBTITLE";
        const string extraAppActionIcon = "EXTRA_XE_APP_ACTION_ICON";

        internal static AppAction ToAppAction(this Intent intent)
            => new AppAction(
                intent.GetStringExtra(extraAppActionId),
                intent.GetStringExtra(extraAppActionTitle),
                intent.GetStringExtra(extraAppActionSubtitle),
                intent.GetStringExtra(extraAppActionIcon));

        /// <summary>
        /// As a developer, you can define shortcuts to perform specific actions in your app. 
        /// These shortcuts can be displayed in a supported launcher or assistant, like Google Assistant, 
        /// and help your users quickly start common or recommended tasks within your app.
        /// This set of guides teaches you how to create and manage app shortcuts.Additionally, 
        /// you'll learn some best practices that will improve the effectiveness of your shortcuts.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        static ShortcutInfo ToShortcutInfo(this AppAction action)
        {
            var shortcut = new ShortcutInfo.Builder(Platform.AppContext, action.Id)
                .SetShortLabel(action.Title);

            if (!string.IsNullOrWhiteSpace(action.Subtitle))
            {
                shortcut.SetLongLabel(action.Subtitle);
            }

            if (!string.IsNullOrWhiteSpace(action.Icon))
            {
                var iconResId = Platform.AppContext.Resources.GetIdentifier(action.Icon, "drawable", Platform.AppContext.PackageName);

                shortcut.SetIcon(Icon.CreateWithResource(Platform.AppContext, iconResId));
            }

            var intent = new Intent(Platform.Intent.ActionAppAction);
            intent.SetPackage(Platform.AppContext.PackageName);
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            intent.PutExtra(extraAppActionId, action.Id);
            intent.PutExtra(extraAppActionTitle, action.Title);
            intent.PutExtra(extraAppActionSubtitle, action.Subtitle);
            intent.PutExtra(extraAppActionIcon, action.Icon);

            shortcut.SetIntent(intent);

            return shortcut.Build();
        }

        internal static bool IsSupported
                    => PlatformIsSupported;

        public static Task<IEnumerable<AppAction>> GetAsync()
            => PlatformGetAsync();

        public static Task SetAsync(params AppAction[] actions)
            => PlatformSetAsync(actions);

        public static Task SetAsync(IEnumerable<AppAction> actions)
            => PlatformSetAsync(actions);

        public static event EventHandler<AppActionEventArgs> OnAppAction;

        internal static void InvokeOnAppAction(object sender, AppAction appAction)
            => OnAppAction?.Invoke(sender, new AppActionEventArgs(appAction));
    }


    public class AppActionEventArgs : EventArgs
    {
        public AppActionEventArgs(AppAction appAction)
            : base() => AppAction = appAction;

        public AppAction AppAction { get; }
    }


    public class AppAction
    {
        public AppAction(string id, string title, string subtitle = null, string icon = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Title = title ?? throw new ArgumentNullException(nameof(title));

            Subtitle = subtitle;
            Icon = icon;
        }

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string Id { get; set; }

        internal string Icon { get; set; }
    }
}