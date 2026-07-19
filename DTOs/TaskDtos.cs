using System.ComponentModel.DataAnnotations;
using TaskStatusModel = TaskManagementApi.Models.TaskStatus;

namespace TaskManagementApi.DTOs;

public class CreateTaskDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    public TaskStatusModel Status { get; set; } = TaskStatusModel.Todo;

    public DateTime? DueDate { get; set; }
}

public class UpdateTaskDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    public TaskStatusModel Status { get; set; }

    public DateTime? DueDate { get; set; }
}

public class UpdateTaskStatusDto
{
    public TaskStatusModel Status { get; set; }
}

public class TaskResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public TaskStatusModel Status { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
