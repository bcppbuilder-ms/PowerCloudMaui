using PowerCloud.ViewModels;
using System.Collections.ObjectModel;

namespace PowerCloud.Views.Account;

public partial class Login_NAS_IPList : ContentPage
{

    MainViewModel vm;
    public ObservableCollection<AccountViewModel> IPList { get; set; }
    
    public Login_NAS_IPList()
	{
		InitializeComponent();

        vm = App.PC2ViewModel;
        vm.EveryAccount.Clear();

        IPList = new ObservableCollection<AccountViewModel>();
        foreach (AccountViewModel item in vm.EveryAccount) {
            if (!string.IsNullOrEmpty(item.UserNasLink))
                IPList.Add(item);
        }
        BindingContext = this;
    }
}