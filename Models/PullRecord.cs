namespace GachaLife.Models;

public class PullRecord
{
    public Guid BannerId { get; set; }

    // Название баннера на момент крутки, чтобы оно не терялось после удаления баннера
    public string BannerName { get; set; } = "";

    public string RewardTitle { get; set; } = "";
    public Rarity Rarity { get; set; }
    public DateTime PulledAt { get; set; } = DateTime.Now;
}