using System.IO;

namespace GachaLife.Services;

public class ImageService
{
    private readonly string _imagesDir;

    public ImageService()
    {
        string appDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GachaLife");
        _imagesDir = Path.Combine(appDir, "images");
        Directory.CreateDirectory(_imagesDir);
    }

    // Копирует выбранный файл в папку images под уникальным именем и возвращает это имя
    public string Import(string sourcePath)
    {
        string extension = Path.GetExtension(sourcePath).ToLowerInvariant();
        string fileName = Guid.NewGuid().ToString("N") + extension;

        File.Copy(sourcePath, Path.Combine(_imagesDir, fileName));
        return fileName;
    }

    // Из имени файла из сохранения собирает полный путь на диске
    public string GetFullPath(string fileName) => Path.Combine(_imagesDir, fileName);
}