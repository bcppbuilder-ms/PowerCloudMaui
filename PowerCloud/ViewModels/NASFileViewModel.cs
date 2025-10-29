using Microsoft.Maui.Controls.PlatformConfiguration.TizenSpecific;

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

        public string SizeStr
        {
            get
            {
                if (Size < 1024)
                    return $"{Size} B";
                else if (Size < 1024 * 1024)
                    return $"{(Size / 1024.0):F2} KB";
                else if (Size < 1024 * 1024 * 1024)
                    return $"{(Size / (1024.0 * 1024.0)):F2} MB";
                else
                    return $"{(Size / (1024.0 * 1024.0 * 1024.0)):F2} GB";
            }
        }

        public string FileInfoStr
        {
            get
            {
                return $"{Path.GetExtension(Name)} ({SizeStr})";
            }
        }

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
                if (mineType.StartsWith("image"))
                {
                    if (usingThumb && thumbNail.Length > 0)
                        imageSrc = ImageSource.FromStream(() => new MemoryStream(thumbNail));
                    else
                        imageSrc = ImageSource.FromFile("file_picture.png");
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
                        if (mineType.StartsWith("audio"))
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

        private bool show2Columns = false;
        public bool ShowTwoColumns { get { return show2Columns; } set { SetPropertyValue(ref show2Columns, value); ImageSrc2 = null; } }


        public byte[] buff2Col = new byte[0];


        private ImageSource prvImageSrc2;
        public ImageSource ImageSrc2
        {
            get
            {
                return prvImageSrc2;
            }

            set
            {
                ImageSource imageSrc2 = null;
                if (mineType.StartsWith("image"))
                {
                    if (show2Columns && buff2Col.Length > 0)
                        imageSrc2 = ImageSource.FromStream(() => new MemoryStream(buff2Col));
                    else
                        imageSrc2 = ImageSource.FromFile("file_picture.png");
                }
                else
                {
                    if (mineType == "folder")
                    {
                        //Device.RuntimePlatform == Device.Android)
                        imageSrc2 = ImageSource.FromFile("file_folder.png");
                    }
                    else
                    {
                        if (mineType.StartsWith("audio"))
                        {
                            imageSrc2 = ImageSource.FromFile("file_audio.png");
                        }
                        else if (mineType.StartsWith("video"))
                        {
                            imageSrc2 = ImageSource.FromFile("file_video.png");
                        }
                        else
                        {
                            imageSrc2 = ImageSource.FromFile("file_text.png");
                        }
                    }
                }
                SetPropertyValue(ref prvImageSrc2, imageSrc2);
            }
        }

        int mediaType;
        public int MediaType { get { return mediaType; } set { SetPropertyValue(ref mediaType, value); } }

        bool selected;
        public bool Selected {  get { return selected; } set { SetPropertyValue(ref selected, value); } }
    }
}