using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Android.Provider;
//using Plugin.CurrentActivity;
using Android.Content;
using Android.Graphics;

namespace PowerCloud.Platforms
{
    public class PhotoAlbumAdapter : RecyclerView.Adapter
    {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;

        // Underlying data set (a photo album):
        PhotoAlbum mPhotoAlbum;

        // Load the adapter with the data set (photo album) at construction time:
        public PhotoAlbumAdapter(PhotoAlbum photoAlbum)
        {
            mPhotoAlbum = photoAlbum;
        }

        // Create a new photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            Android.Views.View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.PhotoCardView, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            PhotoViewHolder vh = new PhotoViewHolder(itemView, OnClick);
            return vh;
        }

        // Fill in the contents of the photo card (invoked by the layout manager):
        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            PhotoViewHolder vh = holder as PhotoViewHolder;

            // Set the ImageView and TextView in this ViewHolder's CardView 
            // from this position in the photo album:
            //vh.Image.SetImageResource(mPhotoAlbum[position].PhotoID);

            ContentResolver contentResolver = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.ApplicationContext.ContentResolver; //CrossCurrentActivity.Current.AppContext.ContentResolver;
            Bitmap x = contentResolver.LoadThumbnail(Android.Net.Uri.WithAppendedPath(MediaStore.Images.Media.ExternalContentUri, mPhotoAlbum[position].PhotoID.ToString()), new Android.Util.Size(144, 81), null);
            vh.Image.SetImageBitmap(x);
            //vh.Image.SetImageURI(Android.Net.Uri.WithAppendedPath(MediaStore.Images.Media.ExternalContentUri, mPhotoAlbum[position].PhotoID.ToString()));
            vh.Caption.Text = mPhotoAlbum[position].Caption;
        }

        // Return the number of photos available in the photo album:
        public override int ItemCount
        {
            get { return mPhotoAlbum.NumPhotos; }
        }

        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }

    public class PhotoViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }
        public TextView Caption { get; private set; }

        // Get references to the views defined in the CardView layout.
        public PhotoViewHolder(Android.Views.View itemView, Action<int> listener)
            : base(itemView)
        {
            // Locate and cache view references:
            Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            Caption = itemView.FindViewById<TextView>(Resource.Id.textView);

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}