using GachaLife.Models;

namespace GachaLife.ViewModels;

public class HistoryItemViewModel
{
    public string Title { get; }
    public string Stars { get; }
    public string Color { get; }
    public string Details { get; }

    public HistoryItemViewModel(PullRecord record, string bannerName)
    {
        Title = record.RewardTitle;
        Stars = RarityInfo.Stars(record.Rarity);
        Color = RarityInfo.Color(record.Rarity);
        Details = $"{bannerName} · {record.PulledAt:dd.MM.yyyy HH:mm}";
    }
}