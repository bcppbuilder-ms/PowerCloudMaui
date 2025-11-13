
using System.ComponentModel;

namespace PowerCloud.Views.FileManagement
{
    public static class Ite2VideoAutoPlay
    {
        public static readonly BindableProperty AutoPlayWhenProperty =
            BindableProperty.CreateAttached(
                "AutoPlayWhen",
                typeof(bool),
                typeof(Ite2VideoAutoPlay),
                false,
                propertyChanged: OnAutoPlayWhenChanged);

        public static bool GetAutoPlayWhen(BindableObject view) =>
            (bool)view.GetValue(AutoPlayWhenProperty);

        public static void SetAutoPlayWhen(BindableObject view, bool value) =>
            view.SetValue(AutoPlayWhenProperty, value);

        static void OnAutoPlayWhenChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is VisualElement ve)) 
                return;

            if ((bool)newValue)
                ve.PropertyChanged += Ve_PropertyChanged;
            else
                ve.PropertyChanged -= Ve_PropertyChanged;
        }

        private static void Ve_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 只處理可見性變更
            if (e.PropertyName != nameof(VisualElement.IsVisible)) 
                return;

            var ve = sender as VisualElement;
            if (ve == null)
                return;

            // MediaElement 型別在不同平台/套件命名不同，
            // 這裡以動態方式呼叫 Play/Pause（可改成強型別）
            var playMethod = ve.GetType().GetMethod("Play");
            var pauseMethod = ve.GetType().GetMethod("Pause");

            if (ve.IsVisible)
            {
                playMethod?.Invoke(ve, null);
            }
            else
            {
                pauseMethod?.Invoke(ve, null);
            }
        }
    }
}
