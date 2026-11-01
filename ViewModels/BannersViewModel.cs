using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GachaLife.Models;
using GachaLife.Services;

namespace GachaLife.ViewModels;

public partial class BannersViewModel : ObservableObject
{
    private readonly MainViewModel _main;
    private readonly GachaEngine _engine;
    
    private readonly ImageService _images = new();

    public ObservableCollection<BannerViewModel> Banners { get; } = new();
    public ObservableCollection<PullResultViewModel> LastResults { get; } = new();

    [ObservableProperty] private BannerViewModel? _selectedBanner;
    [ObservableProperty] private string _newRewardTitle = string.Empty;
    [ObservableProperty] private string _statusMessage = string.Empty;

    // Переключатели редкости для новой награды
    [ObservableProperty] private bool _isStar3 = true;
    [ObservableProperty] private bool _isStar4;
    [ObservableProperty] private bool _isStar5;

    private Rarity SelectedRarity => IsStar5 ? Rarity.Star5 : IsStar4 ? Rarity.Star4 : Rarity.Star3;

    public BannersViewModel(MainViewModel main, GachaEngine engine)
    {
        _main = main;
        _engine = engine;

        foreach (var b in _main.State.Banners)
            Banners.Add(CreateBannerViewModel(b));

        SelectedBanner = Banners.FirstOrDefault();
        SelectedBanner?.SelectCommand.Execute(null);
    }

    private BannerViewModel CreateBannerViewModel(Banner banner)
    {
        var vm = new BannerViewModel(banner, () => _main.Save(), selected =>
        {
            foreach (var x in Banners) x.IsSelected = (x == selected);
            SelectedBanner = selected;
            LastResults.Clear();
            StatusMessage = string.Empty;
        });

        foreach (var reward in banner.Rewards)
            vm.Rewards.Add(CreateRewardViewModel(vm, reward));

        return vm;
    }

        // Удаление награды всегда относится именно к своему баннеру
    private RewardViewModel CreateRewardViewModel(BannerViewModel bannerVm, Reward reward)
    {
        return new RewardViewModel(reward, _images, _main.Save, rewardVm =>
        {
            bannerVm.Model.Rewards.Remove(rewardVm.Model);
            bannerVm.Rewards.Remove(rewardVm);
            bannerVm.RefreshFeatured();
            _main.Save();
        });
    }

    [RelayCommand]
    private void AddBanner()
    {
        var banner = new Banner { Name = $"Баннер {Banners.Count + 1}", PullCost = 160 };
        _main.State.Banners.Add(banner);

        var vm = CreateBannerViewModel(banner);
        Banners.Add(vm);

        vm.SelectCommand.Execute(null);
        _main.Save();
    }

    [RelayCommand]
    private void DeleteBanner()
    {
        if (SelectedBanner == null) return;

        _main.State.Banners.Remove(SelectedBanner.Model);
        Banners.Remove(SelectedBanner);

        SelectedBanner = Banners.FirstOrDefault();
        SelectedBanner?.SelectCommand.Execute(null);
        LastResults.Clear();
        _main.Save();
    }

    [RelayCommand]
    private void AddReward()
    {
        var banner = SelectedBanner;
        if (banner == null || string.IsNullOrWhiteSpace(NewRewardTitle)) return;

        var reward = new Reward { Title = NewRewardTitle.Trim(), Rarity = SelectedRarity };
        banner.Model.Rewards.Add(reward);
        banner.Rewards.Add(CreateRewardViewModel(banner, reward));

        banner.RefreshFeatured();
        NewRewardTitle = string.Empty;
        _main.Save();
    }

    [RelayCommand] private void PullOnce() => ExecutePull(1);
    [RelayCommand] private void PullTen() => ExecutePull(10);

    private void ExecutePull(int count)
    {
        var banner = SelectedBanner;
        if (banner == null) return;

        if (banner.Model.Rewards.Count == 0)
        {
            StatusMessage = "Добавьте в баннер хотя бы одну награду.";
            return;
        }

        int totalCost = banner.Model.PullCost * count;
        if (_main.Gems < totalCost)
        {
            StatusMessage = $"Недостаточно ◆! Нужно {totalCost}, у вас {_main.Gems}.";
            return;
        }

        StatusMessage = string.Empty;
        _main.AddGems(-totalCost);

        var rolledRewards = _engine.Pull(banner.Model, count);

        LastResults.Clear();
        int index = 0;
        foreach (var reward in rolledRewards)
        {
            LastResults.Add(new PullResultViewModel(reward, index++));
            

            _main.State.History.Insert(0, new PullRecord
            {
                RewardTitle = reward.Title,
                Rarity = reward.Rarity,
                BannerId = banner.Model.Id,
                PulledAt = DateTime.Now,
                BannerName = banner.Model.Name,
            });
        }

        banner.RefreshPity();
        _main.Save();
    }
}