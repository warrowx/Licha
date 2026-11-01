namespace GachaLife.Models;

public class Banner
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "Мой баннер";
    public List<Reward> Rewards { get; set; } = new();

    public int PullCost { get; set; } = GameRules.PullCost;

    // Шансы как доли от 1 (0.03 = 3%), остальное достаётся 3★.
    // У старых сохранений будут значения по умолчанию из GameRules.
    public double Chance5Star { get; set; } = GameRules.Chance5Star;
    public double Chance4Star { get; set; } = GameRules.Chance4Star;

    // Счётчики пити: сколько круток прошло с последней 5★ и 4★ награды
    public int PullsSince5Star { get; set; }
    public int PullsSince4Star { get; set; }
}