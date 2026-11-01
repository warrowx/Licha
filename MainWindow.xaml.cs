using System.Windows;
using GachaLife.ViewModels;

namespace GachaLife;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}