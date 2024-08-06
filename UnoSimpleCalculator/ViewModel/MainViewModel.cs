using UnoSimpleCalculator.Logic;

namespace UnoSimpleCalculator.ViewModel;

public partial class MainViewModel : ObservableObject
{
    private readonly IThemeService _themeService;

    private bool _isDarkTheme;
    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (SetProperty(ref _isDarkTheme, value))
            {
                _themeService.SetThemeAsync(value ? AppTheme.Dark : AppTheme.Light);
            }
        }
    }

    public MainViewModel(IThemeService themeService)
    {
        _themeService = themeService;
        IsDarkTheme = _themeService.IsDark;
        _themeService.ThemeChanged += (_, _) => IsDarkTheme = themeService.IsDark;
    }

    [ObservableProperty]
    private Calculator _calculator = new();

    [RelayCommand]
    private void Input(string key) => Calculator = Calculator.Input(key);
}
