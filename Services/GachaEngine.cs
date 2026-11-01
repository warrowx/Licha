using GachaLife.Models;

namespace GachaLife.Services;

public class GachaEngine
{
    private readonly Random _random;

    public GachaEngine(Random? random = null)
    {
        _random = random ?? Random.Shared;
    }

    // Одна крутка. Возвращает null, если в баннере нет наград.
    public Reward? Pull(Banner banner)
    {
        if (banner.Rewards.Count == 0) return null;

        Rarity rolled = RollRarity(banner);

        // Счётчики пити считаем по выпавшей редкости, а не по реальной награде:
        // иначе при отсутствии 5★ в баннере гарантия срабатывала бы каждую крутку
        banner.PullsSince5Star++;
        banner.PullsSince4Star++;
        if (rolled == Rarity.Star5)
        {
            banner.PullsSince5Star = 0;
            banner.PullsSince4Star = 0;
        }
        else if (rolled == Rarity.Star4)
        {
            banner.PullsSince4Star = 0;
        }

        Rarity actual = ResolveRarity(banner, rolled);
        var pool = banner.Rewards.Where(r => r.Rarity == actual).ToList();
        return pool[_random.Next(pool.Count)];
    }

    // Несколько круток подряд
    public List<Reward> Pull(Banner banner, int count)
    {
        var result = new List<Reward>();
        for (int i = 0; i < count; i++)
        {
            var reward = Pull(banner);
            if (reward == null) break;
            result.Add(reward);
        }
        return result;
    }
    
    private Rarity RollRarity(Banner banner)
    {
        if (banner.PullsSince5Star + 1 >= GameRules.Pity5Star)
            return Rarity.Star5;

        double roll = _random.NextDouble();

        if (roll < banner.Chance5Star)
            return Rarity.Star5;

        if (banner.PullsSince4Star + 1 >= GameRules.Pity4Star)
            return Rarity.Star4;

        if (roll < banner.Chance5Star + banner.Chance4Star)
            return Rarity.Star4;

        return Rarity.Star3;
    }

    // Если игрок не вписал награды нужной редкости, берём ближайшую ниже, потом выше
    private static Rarity ResolveRarity(Banner banner, Rarity wanted)
    {
        if (banner.Rewards.Any(r => r.Rarity == wanted))
            return wanted;

        for (int r = (int)wanted - 1; r >= 3; r--)
            if (banner.Rewards.Any(x => (int)x.Rarity == r))
                return (Rarity)r;

        for (int r = (int)wanted + 1; r <= 5; r++)
            if (banner.Rewards.Any(x => (int)x.Rarity == r))
                return (Rarity)r;

        return banner.Rewards[0].Rarity;
    }

    // Для проверки баланса: крутит копию баннера и считает результаты
    public Dictionary<Rarity, int> Simulate(Banner banner, int pulls)
    {
        var copy = new Banner { Rewards = banner.Rewards };
        var counts = new Dictionary<Rarity, int>();

        for (int i = 0; i < pulls; i++)
        {
            var reward = Pull(copy);
            if (reward == null) break;
            counts[reward.Rarity] = counts.GetValueOrDefault(reward.Rarity) + 1;
        }

        return counts;
    }
}