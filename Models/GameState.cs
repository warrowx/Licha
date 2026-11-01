namespace GachaLife.Models;

public class GameState
{
    public int Gems { get; set; }
    public List<TaskItem> Tasks { get; set; } = new();
    public List<Banner> Banners { get; set; } = new();
    public List<PullRecord> History { get; set; } = new();
}