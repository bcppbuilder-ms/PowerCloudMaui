namespace PowerCloud.Models
{
    public class ImgGroup : List<BackupItem>
    {
        public string Name { get; private set; }

        public ImgGroup(string name, List<BackupItem> animals) : base(animals)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
