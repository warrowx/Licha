using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GachaLife.Models;

namespace GachaLife.ViewModels;

public partial class TaskViewModel : ObservableObject
{
    private readonly Action<TaskViewModel, bool> _onToggled;
    private readonly Action<TaskViewModel> _onDeleted;
    private bool _isDone;

    public TaskItem Model { get; }

    public TaskViewModel(TaskItem model, Action<TaskViewModel, bool> onToggled, Action<TaskViewModel> onDeleted)
    {
        Model = model;
        _onToggled = onToggled;
        _onDeleted = onDeleted;

        // Поле, а не свойство: при загрузке награду начислять не нужно
        _isDone = model.Kind == TaskKind.Daily
            ? model.LastCompletedOn == DateOnly.FromDateTime(DateTime.Today)
            : model.IsDone;
    }

    public string Title => Model.Title;

    public string Subtitle => Model.Kind == TaskKind.Daily
        ? $"Ежедневная · серия {Model.Streak}"
        : "Одноразовая";

    public string RewardText => $"+{GameRules.GemsFor(Model.Difficulty)} ◆";

    public bool IsDone
    {
        get => _isDone;
        set
        {
            if (SetProperty(ref _isDone, value))
                _onToggled(this, value);
        }
    }

    public void RefreshText() => OnPropertyChanged(nameof(Subtitle));

    [RelayCommand]
    private void Delete() => _onDeleted(this);
}