namespace PowerCloud.Platforms
{
    // Photo: contains image resource ID and caption:
    // Photo album: holds image resource IDs and caption:
    public class PhotoAlbum
    {
        // Built-in photo collection - this could be replaced with
        // a photo database:

        static List<Photo> mBuiltInPhotos;

        // Array of photos that make up the album:
        public List<Photo> mPhotos;

        // Create an instance copy of the built-in photo list and
        // create the random number generator:
        public PhotoAlbum()
        {
            mBuiltInPhotos = new List<Photo>();
            foreach (Ite2MediaItem item in Ite2DeviceInfoService.AllMediaFiles)
            {
                if (item.IsImage)
                    mBuiltInPhotos.Add(new Photo(item.Id, item.DateAdded));
            }
            mPhotos = mBuiltInPhotos;
        }

        // Return the number of photos in the photo album:
        public int NumPhotos
        {
            get { return mPhotos.Count; }
        }

        // Indexer (read only) for accessing a photo:
        public Photo this[int i]
        {
            get { return mPhotos[i]; }
        }
    }

    public class Photo
    {
        public Photo(int id, string caption)
        {
            PhotoID = id;
            Caption = caption;
        }

        // Return the ID of the photo:
        public int PhotoID { get; }

        // Return the Caption of the photo:
        public string Caption { get; }
    }

}