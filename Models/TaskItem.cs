namespace GachaLife.Models;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "";
    public TaskKind Kind { get; set; } = TaskKind.OneTime;
    public Difficulty Difficulty { get; set; } = Difficulty.Easy;

    // Для одноразовых задач
    public bool IsDone { get; set; }

    // Для ежедневных задач
    public DateOnly? LastCompletedOn { get; set; }
    public int Streak { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}