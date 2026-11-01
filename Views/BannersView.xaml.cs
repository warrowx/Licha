using System.Windows;
using System.Windows.Controls;
using GachaLife.Animations;
using GachaLife.ViewModels;
using Microsoft.Win32;

namespace GachaLife.Views;

public partial class BannersView : UserControl
{
    public BannersView()
    {
        InitializeComponent();
    }

    // Окно выбора файла относится к интерфейсу, поэтому живёт здесь, а не во ViewModel.
    // ViewModel получает от нас только готовый путь к файлу.
    private void PickImage_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: RewardViewModel reward })
            return;

        var dialog = new OpenFileDialog
        {
            Title = "Выберите картинку для награды",
            Filter = "Картинки|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Все файлы|*.*"
        };

        if (dialog.ShowDialog() == true)
            reward.SetImage(dialog.FileName);
    }

    // Карточка результата появилась на экране: запускаем анимацию из отдельного файла
        // Карточка результата появилась на экране
    private void ResultCard_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is not Border card || card.DataContext is not PullResultViewModel result)
            return;

        // Появление играем один раз, а свечение 4★ и 5★ живёт, пока карточка на экране
        bool isFirstTime = !result.IsAnimationPlayed;
        result.IsAnimationPlayed = true;

        PullAnimations.PlayResultCard(card, result.Index, result.Rarity, result.Color, isFirstTime);
    }
}