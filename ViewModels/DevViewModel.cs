using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GachaLife.ViewModels;

public partial class DevViewModel : ObservableObject
{
    private readonly MainViewModel _main;

    [ObservableProperty] private string _gemsInput = "";
    [ObservableProperty] private string _message = "";

    public DevViewModel(MainViewModel main)
    {
        _main = main;
    }

    // Прибавить или отнять гемы: параметр приходит из кнопки (например "100" или "-100")
    [RelayCommand]
    private void AddGems(string? amount)
    {
        if (!int.TryParse(amount, out int delta)) return;

        _main.AddGems(delta);
        Message = $"Гемов сейчас: {_main.Gems}";
    }

    // Установить точное значение
    [RelayCommand]
    private void SetGems()
    {
        if (!int.TryParse(GemsInput, out int target) || target < 0)
        {
            Message = "Введите целое число от 0.";
            return;
        }

        _main.AddGems(target - _main.Gems);
        Message = $"Гемов сейчас: {_main.Gems}";
    }
}