namespace PowerCloud.Controls;

public partial class AccountCardTemplate : ContentView
{

    public static readonly BindableProperty AccountIconProperty = BindableProperty.Create(nameof(AccountIcon), typeof(string), typeof(AccountCardTemplate), string.Empty);
    public string AccountIcon
    {
        get => (string)GetValue(AccountCardTemplate.AccountIconProperty);
        set => SetValue(AccountCardTemplate.AccountIconProperty, value);
    }

    public static readonly BindableProperty AccountIdProperty = BindableProperty.Create(nameof(AccountId), typeof(string), typeof(AccountCardTemplate), string.Empty);
    public string AccountId
    {
        get => (string)GetValue(AccountCardTemplate.AccountIdProperty);
        set => SetValue(AccountCardTemplate.AccountIdProperty, value);
    }
    
    public static readonly BindableProperty AccountNasProperty = BindableProperty.Create(nameof(AccountNas), typeof(string), typeof(AccountCardTemplate), string.Empty);
    public string AccountNas
    {
        get => (string)GetValue(AccountCardTemplate.AccountNasProperty);
        set => SetValue(AccountCardTemplate.AccountNasProperty, value);
    }



    public AccountCardTemplate()
	{
		InitializeComponent();
    }
}