using AnimatedIconMaui.ViewModels;

namespace AnimatedIconMaui.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        BindingContext = new MainViewModel();
    }
}