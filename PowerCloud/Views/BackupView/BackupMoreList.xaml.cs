using PowerCloud.ViewModels;
using LayoutAlignment = Microsoft.Maui.Primitives.LayoutAlignment;

namespace PowerCloud.Views.BackupView;

public partial class BackupMoreList : ContentPage
{
	public BackupMoreList()
	{
		InitializeComponent();
        BindingContext = new BackupMoreListViewModel();
    }


}