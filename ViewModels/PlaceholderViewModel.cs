namespace GachaLife.ViewModels;

public class PlaceholderViewModel
{
    public string Title { get; }
    public string Description { get; }

    public PlaceholderViewModel(string title, string description)
    {
        Title = title;
        Description = description;
    }
}