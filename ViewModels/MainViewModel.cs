using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GachaLife.Models;
using GachaLife.Services;

namespace GachaLife.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly StorageService _storage = new();
    private readonly TasksViewModel _tasksPage;
    private readonly BannersViewModel _bannersPage;
    private readonly PlaceholderViewModel _collectionPage =
        new("Коллекция", "Здесь появятся все награды, которые вы выбили.");

    public GameState State { get; }

        // Режим разработчика: true только в отладочной сборке
    #if DEBUG
    public bool IsDevMode => true;
    #else
    public bool IsDevMode => false;
    #endif

    [ObservableProperty] private object? _currentPage;

    public int Gems
    {
        get => State.Gems;
        private set
        {
            if (State.Gems != value)
            {
                State.Gems = value;
                OnPropertyChanged();
            }
        }
    }

    public MainViewModel()
    {
        State = _storage.Load();
        _tasksPage = new TasksViewModel(this);
        _bannersPage = new BannersViewModel(this, new GachaEngine());
        CurrentPage = _tasksPage;
    }

    public void AddGems(int delta)
    {
        Gems = Math.Max(0, Gems + delta);
        Save();
    }

    public void Save() => _storage.Save(State);

    [RelayCommand]
    private void Navigate(string? page)
    {
        CurrentPage = page switch
        {
            "Banner" => (object)_bannersPage,
            "Collection" => _collectionPage,
            "History" => new HistoryViewModel(State),
            "Dev" => new DevViewModel(this),
            _ => _tasksPage
        };
    }
}