namespace GachaLife.Models;

// Награда, которую игрок сам вписывает в баннер, например «Сходить в кино»
public class Reward
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "";
    public Rarity Rarity { get; set; } = Rarity.Star3;

    // Только имя файла в папке images рядом с сохранением, не полный путь.
    // null означает, что картинки у награды нет.
    public string? ImageFileName { get; set; }
}