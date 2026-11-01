using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GachaLife.Models;

namespace GachaLife.ViewModels;

public partial class TasksViewModel : ObservableObject
{
    private readonly MainViewModel _main;

    public ObservableCollection<TaskViewModel> Tasks { get; } = new();

    public string TodayText =>
        $"Сегодня, {DateTime.Today.ToString("d MMMM", new CultureInfo("ru-RU"))}";

    [ObservableProperty] private string _newTaskTitle = "";
    [ObservableProperty] private bool _newTaskIsDaily;
    [ObservableProperty] private Difficulty _newTaskDifficulty = Difficulty.Easy;

    // Три флага для переключателей сложности (чтобы не городить конвертеры)
    public bool IsEasy
    {
        get => NewTaskDifficulty == Difficulty.Easy;
        set { if (value) NewTaskDifficulty = Difficulty.Easy; }
    }
    public bool IsMedium
    {
        get => NewTaskDifficulty == Difficulty.Medium;
        set { if (value) NewTaskDifficulty = Difficulty.Medium; }
    }
    public bool IsHard
    {
        get => NewTaskDifficulty == Difficulty.Hard;
        set { if (value) NewTaskDifficulty = Difficulty.Hard; }
    }

    partial void OnNewTaskDifficultyChanged(Difficulty value)
    {
        OnPropertyChanged(nameof(IsEasy));
        OnPropertyChanged(nameof(IsMedium));
        OnPropertyChanged(nameof(IsHard));
    }

    public TasksViewModel(MainViewModel main)
    {
        _main = main;

        var today = DateOnly.FromDateTime(DateTime.Today);
        foreach (var task in _main.State.Tasks)
        {
            // Пропустили больше одного дня: серия сгорела
            if (task.Kind == TaskKind.Daily && task.LastCompletedOn is DateOnly last && last < today.AddDays(-1))
                task.Streak = 0;

            Tasks.Add(CreateViewModel(task));
        }
    }

    private TaskViewModel CreateViewModel(TaskItem task) => new(task, OnTaskToggled, OnTaskDeleted);

    [RelayCommand]
    private void AddTask()
    {
        string title = NewTaskTitle.Trim();
        if (title.Length == 0) return;

        var item = new TaskItem
        {
            Title = title,
            Kind = NewTaskIsDaily ? TaskKind.Daily : TaskKind.OneTime,
            Difficulty = NewTaskDifficulty
        };

        _main.State.Tasks.Add(item);
        Tasks.Add(CreateViewModel(item));
        NewTaskTitle = "";
        _main.Save();
    }

    private void OnTaskToggled(TaskViewModel vm, bool done)
    {
        var task = vm.Model;
        var today = DateOnly.FromDateTime(DateTime.Today);
        int reward = GameRules.GemsFor(task.Difficulty);

        if (done)
        {
            if (task.Kind == TaskKind.Daily)
            {
                task.Streak = task.LastCompletedOn == today.AddDays(-1) ? task.Streak + 1 : 1;
                task.LastCompletedOn = today;
            }
            else
            {
                task.IsDone = true;
            }
            _main.AddGems(reward);
        }
        else
        {
            // Снятие галочки отменяет награду (на случай случайного клика)
            if (task.Kind == TaskKind.Daily)
            {
                task.Streak = Math.Max(0, task.Streak - 1);
                task.LastCompletedOn = task.Streak > 0 ? today.AddDays(-1) : null;
            }
            else
            {
                task.IsDone = false;
            }
            _main.AddGems(-reward);
        }

        vm.RefreshText();
    }

    private void OnTaskDeleted(TaskViewModel vm)
    {
        _main.State.Tasks.Remove(vm.Model);
        Tasks.Remove(vm);
        _main.Save();
    }
}