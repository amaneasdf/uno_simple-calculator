using UnoSimpleCalculator.ViewModel;

namespace UnoSimpleCalculator;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        DataContext = new MainViewModel(this.GetThemeService());
    }
}
