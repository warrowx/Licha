using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GachaLife.Models;

namespace GachaLife.ViewModels;

public partial class BannerViewModel : ObservableObject
{
    private readonly Action _onChanged;
    private readonly Action<BannerViewModel> _onSelect;

    public Banner Model { get; }
    public ObservableCollection<RewardViewModel> Rewards { get; } = new();

    [ObservableProperty] private bool _isSelected;

    public BannerViewModel(Banner model, Action onChanged, Action<BannerViewModel> onSelect)
    {
        Model = model;
        _onChanged = onChanged;
        _onSelect = onSelect;

        // Витрина баннера обновляется, когда меняется список наград
        Rewards.CollectionChanged += (_, _) => RefreshFeatured();
    }

    public string Name
    {
        get => Model.Name;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) value = "Баннер";
            if (Model.Name == value) return;
            Model.Name = value;
            OnPropertyChanged();
            _onChanged();
        }
    }

    public int PullCost
    {
        get => Model.PullCost;
        set
        {
            Model.PullCost = Math.Clamp(value, 1, 10000);
            OnPropertyChanged();
            OnPropertyChanged(nameof(PullOnceText));
            OnPropertyChanged(nameof(PullTenText));
            _onChanged();
        }
    }

    // ===== Шансы (в процентах, как текст) =====

    public string Chance5Text
    {
        get => FormatPercent(Model.Chance5Star);
        set => TrySetChance(value, isFiveStar: true);
    }

    public string Chance4Text
    {
        get => FormatPercent(Model.Chance4Star);
        set => TrySetChance(value, isFiveStar: false);
    }

    public string Chance3Info =>
        $"3★ получает остаток: {FormatPercent(Math.Max(0, 1 - Model.Chance5Star - Model.Chance4Star))}%";

    private static string FormatPercent(double fraction) =>
        (fraction * 100).ToString("0.##", CultureInfo.InvariantCulture);

    private void TrySetChance(string text, bool isFiveStar)
    {
        // Принимаем и точку, и запятую
        if (double.TryParse(text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double percent)
            && percent >= 0 && percent <= 100)
        {
            double value = percent / 100.0;
            double other = isFiveStar ? Model.Chance4Star : Model.Chance5Star;

            // Сумма шансов 5★ и 4★ не может быть больше 100%
            if (value + other <= 1.0 + 1e-9)
            {
                if (isFiveStar) Model.Chance5Star = value;
                else Model.Chance4Star = value;
                _onChanged();
            }
        }

        // В любом случае обновляем поля: при неверном вводе вернётся прежнее значение
        OnPropertyChanged(nameof(Chance5Text));
        OnPropertyChanged(nameof(Chance4Text));
        OnPropertyChanged(nameof(Chance3Info));
    }

    // ===== Тексты кнопок и пити =====

    public string PullOnceText => $"Крутить ×1 · {Model.PullCost} ◆";
    public string PullTenText => $"Крутить ×10 · {Model.PullCost * 10} ◆";

    public string Pity5Text => $"До гарантии 5★: {GameRules.Pity5Star - Model.PullsSince5Star}";
    public string Pity4Text => $"До гарантии 4★ или выше: {GameRules.Pity4Star - Model.PullsSince4Star}";

    // ===== Витрина: одна главная 5★ награда и до трёх наград 4★ =====

    public List<RewardViewModel> Featured5Items =>
        Rewards.Where(r => r.Model.Rarity == Rarity.Star5).Take(1).ToList();

    public List<RewardViewModel> Featured4Items =>
        Rewards.Where(r => r.Model.Rarity == Rarity.Star4).Take(3).ToList();

    public void RefreshFeatured()
    {
        OnPropertyChanged(nameof(Featured5Items));
        OnPropertyChanged(nameof(Featured4Items));
    }

    public void RefreshPity()
    {
        OnPropertyChanged(nameof(Pity5Text));
        OnPropertyChanged(nameof(Pity4Text));
        OnPropertyChanged(nameof(PullOnceText));
        OnPropertyChanged(nameof(PullTenText));
    }

    [RelayCommand]
    private void Select() => _onSelect(this);
}