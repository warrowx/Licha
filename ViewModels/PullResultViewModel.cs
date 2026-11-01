using GachaLife.Models;

namespace GachaLife.ViewModels;

public class PullResultViewModel
{
    public string Title { get; }
    public string Stars { get; }
    public string Color { get; }

    // Редкость и номер в серии нужны интерфейсу для анимации
    public Rarity Rarity { get; }
    public int Index { get; }

    // Чтобы анимация проигрывалась один раз, а не при каждом возврате на страницу
    public bool IsAnimationPlayed { get; set; }

    public PullResultViewModel(Reward reward, int index)
    {
        Title = reward.Title;
        Stars = RarityInfo.Stars(reward.Rarity);
        Color = RarityInfo.Color(reward.Rarity);
        Rarity = reward.Rarity;
        Index = index;
    }
}