namespace PowerCloud.ViewModels
{
    public class NASFileViewModel : BindViewModel
    {
        //private string pName;
        //private long pSizw;
        //private string pLastWriteTime;
        //private string pCreationTime;
        //private string pMimeType;
        //private int pFileAttr;
        //private bool pShared;
        //private List<string> pFileAttrContent;
        //private int pMediaType;

        private string name;
        public string Name { get { return name; } set { SetPropertyValue(ref name, value); } }

        private string pathName;
        public string PathName { get { return pathName; } set { SetPropertyValue(ref pathName, value); CanMultiSelect = true; } }

        private long size;
        public long Size { get { return size; } set { SetPropertyValue(ref size, value); } }

        private string lastWriteTime;
        public string LastWriteTime { get { return lastWriteTime; } set { SetPropertyValue(ref lastWriteTime, value); } }

        private string createTime;
        public string CreationTime { get { return createTime; } set { SetPropertyValue(ref createTime, value); } }

        private bool canMultiSelect;
        public bool CanMultiSelect { get { return MimeType != "folder"; } set { SetPropertyValue(ref canMultiSelect, MimeType != "folder"); } }

        private string mineType;
        public string MimeType 
        { 
            get { return mineType; } 
            set { SetPropertyValue(ref mineType, value); } 
        }
        private int fileAttr;
        public int FileAttr { get { return fileAttr; } set { SetPropertyValue(ref fileAttr, value); } }

        private bool shared;
        public bool Shared { get { return shared; } set { SetPropertyValue(ref shared, value); } }

        private List<string> fileAttrContent;
        public List<string> FileAttrContent { get { return fileAttrContent; } set { SetPropertyValue(ref fileAttrContent, value); } }

        private bool usingThumb = false;
        public bool UsingThumb { get { return usingThumb; } set { SetPropertyValue(ref usingThumb, value); ImageSrc = null; } }

        public byte[] thumbNail = new byte[0];
        //public byte[] ThumbNailArea { get { return thumbNail; } set { SetPropertyValue(ref thumbNail, value); } } 

        private ImageSource prvImageSrc;
        public ImageSource ImageSrc
        {
            get
            {
                return prvImageSrc;
            }

            set
            {
                ImageSource imageSrc = null;
                if (usingThumb && thumbNail.Length > 0)
                {
                    imageSrc = ImageSource.FromStream(() => new MemoryStream(thumbNail));
                }
                else
                {
                    if (mineType == "folder")
                    {
                        //Device.RuntimePlatform == Device.Android)
                        imageSrc = ImageSource.FromFile("file_folder.png");
                    }
                    else
                    {
                        if (mineType.StartsWith("image"))
                        {
                            imageSrc = ImageSource.FromFile("file_picture.png");
                        }
                        else if (mineType.StartsWith("audio"))
                        {
                            imageSrc = ImageSource.FromFile("file_audio.png");
                        }
                        else if (mineType.StartsWith("video"))
                        {
                            imageSrc = ImageSource.FromFile("file_video.png");
                        }
                        else
                        {
                            imageSrc = ImageSource.FromFile("file_text.png");
                        }
                    }
                }
                SetPropertyValue(ref prvImageSrc, imageSrc);
            }
        }

        int mediaType;
        public int MediaType { get { return mediaType; } set { SetPropertyValue(ref mediaType, value); } }

        bool selected;
        public bool Selected {  get { return selected; } set { SetPropertyValue(ref selected, value); } }
    }
}