using GachaLife.Models;

namespace GachaLife.ViewModels;

public static class RarityInfo
{
    public static string Stars(Rarity rarity) => new string('★', (int)rarity);

    public static string Color(Rarity rarity) => rarity switch
    {
        Rarity.Star5 => "#F5C451",
        Rarity.Star4 => "#B084FF",
        _ => "#6FA8FF"
    };
}