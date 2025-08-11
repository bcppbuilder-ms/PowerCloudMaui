using PowerCloud.ViewModels;

namespace PowerCloud
{
    //public interface IAccountFiler
    //{
    //    IEnumerable<AccountViewModel> Load();
    //    void Save(IEnumerable<AccountViewModel> users);
    //}

    public interface IAccountFiler2
    {
        IEnumerable<AccountViewModel> Load();
        void Save(IEnumerable<AccountViewModel> users);
    }
}