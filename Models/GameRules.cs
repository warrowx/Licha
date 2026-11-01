namespace GachaLife.Models;

public static class GameRules
{
    // Цена одной крутки
    public const int PullCost = 50;

    // Шансы (остальное достаётся 3★)
    public const double Chance5Star = 0.03;
    public const double Chance4Star = 0.17;

    // Пити: на какой крутке награда гарантирована
    public const int Pity5Star = 50;
    public const int Pity4Star = 10;

    public static int GemsFor(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Easy => 10,
        Difficulty.Medium => 20,
        Difficulty.Hard => 40,
        _ => 10
    };
}