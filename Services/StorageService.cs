using System.IO;
using System.Text.Json;
using GachaLife.Models;

namespace GachaLife.Services;

public class StorageService
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
    private readonly string _path;

    public StorageService()
    {
        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GachaLife");
        Directory.CreateDirectory(dir);
        _path = Path.Combine(dir, "save.json");
    }

    public GameState Load()
    {
        try
        {
            if (File.Exists(_path))
                return JsonSerializer.Deserialize<GameState>(File.ReadAllText(_path)) ?? new GameState();
        }
        catch
        {
            // Повреждённый файл: начинаем с чистого состояния
        }
        return new GameState();
    }

    public void Save(GameState state)
    {
        File.WriteAllText(_path, JsonSerializer.Serialize(state, Options));
    }
}