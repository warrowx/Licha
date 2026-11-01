using GachaLife.Models;

namespace GachaLife.ViewModels;

public class HistoryViewModel
{
    public List<HistoryItemViewModel> Items { get; }
    public string SummaryText { get; }
    public bool IsEmpty { get; }

    public HistoryViewModel(GameState state)
    {
        var bannerNames = state.Banners.ToDictionary(b => b.Id, b => b.Name);

        // Показываем последние 500 круток, новые сверху
        Items = state.History
            .OrderByDescending(r => r.PulledAt)
            .Take(500)
            .Select(r => new HistoryItemViewModel(r, GetBannerName(r, bannerNames)))
            .ToList();

        int total = state.History.Count;
        int five = state.History.Count(r => r.Rarity == Rarity.Star5);
        int four = state.History.Count(r => r.Rarity == Rarity.Star4);
        int three = state.History.Count(r => r.Rarity == Rarity.Star3);

        SummaryText = $"Всего круток: {total} · 5★: {five} · 4★: {four} · 3★: {three}";
        IsEmpty = total == 0;
    }

    // Баннер жив: текущее имя. Удалён: имя из записи с пометкой.
    private static string GetBannerName(PullRecord record, Dictionary<Guid, string> names)
    {
        if (names.TryGetValue(record.BannerId, out var name))
            return name;

        return string.IsNullOrWhiteSpace(record.BannerName)
            ? "Удалённый баннер"
            : record.BannerName + " (удалён)";
    }
}