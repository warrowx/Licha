using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GachaLife.Models;
using GachaLife.Services;

namespace GachaLife.ViewModels;

public partial class RewardViewModel : ObservableObject
{
    private readonly ImageService _images;
    private readonly Action _onChanged;
    private readonly Action<RewardViewModel> _onDelete;

    public Reward Model { get; }

    public RewardViewModel(Reward model, ImageService images, Action onChanged, Action<RewardViewModel> onDelete)
    {
        Model = model;
        _images = images;
        _onChanged = onChanged;
        _onDelete = onDelete;
    }

    public string Title => Model.Title;
    public string Stars => RarityInfo.Stars(Model.Rarity);
    public string Color => RarityInfo.Color(Model.Rarity);

    // Полный путь к картинке или null, если картинки нет (или файл пропал с диска)
    public string? ImagePath
    {
        get
        {
            if (string.IsNullOrEmpty(Model.ImageFileName)) return null;

            string path = _images.GetFullPath(Model.ImageFileName);
            return File.Exists(path) ? path : null;
        }
    }

    public bool HasImage => ImagePath != null;
    public bool CanHaveImage => Model.Rarity >= Rarity.Star4;

    // Принимает путь к выбранному файлу: копирует его, запоминает имя, сохраняет
    public void SetImage(string sourcePath)
    {
        Model.ImageFileName = _images.Import(sourcePath);
        OnPropertyChanged(nameof(ImagePath));
        OnPropertyChanged(nameof(HasImage));
        _onChanged();
    }

    [RelayCommand]
    private void Delete() => _onDelete(this);
}