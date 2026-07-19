using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data;
using TaskManagementApi.DTOs;
using TaskManagementApi.Models;
using TaskStatusModel = TaskManagementApi.Models.TaskStatus;

namespace TaskManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    // POST: /api/tasks
    [HttpPost]
    public async Task<ActionResult<TaskResponseDto>> CreateTask(
        [FromBody] CreateTaskDto dto)
    {
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == dto.CategoryId);

        if (!categoryExists)
        {
            return BadRequest(new
            {
                message = "CategoryId tidak ditemukan."
            });
        }

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            Status = dto.Status,
            DueDate = dto.DueDate,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return Ok(await MapToResponse(task.Id));
    }

    // GET: /api/tasks
    [HttpGet]
    public async Task<ActionResult> GetTasks(
        TaskStatusModel? status,
        Guid? categoryId,
        int page = 1,
        int pageSize = 10)
    {
        var query = _context.Tasks
            .Include(t => t.Category)
            .Where(t => !t.IsDeleted)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(t => t.CategoryId == categoryId.Value);
        }

        var totalItems = await query.CountAsync();

        var tasks = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                CategoryId = t.CategoryId,
                CategoryName = t.Category.Name,
                Status = t.Status,
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();

        return Ok(new
        {
            page,
            pageSize,
            totalItems,
            totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            data = tasks
        });
    }

    // GET: /api/tasks/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponseDto>> GetTaskById(Guid id)
    {
        var task = await _context.Tasks
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (task == null)
        {
            return NotFound(new
            {
                message = "Task tidak ditemukan."
            });
        }

        return Ok(new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            CategoryId = task.CategoryId,
            CategoryName = task.Category.Name,
            Status = task.Status,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        });
    }

    // PUT: /api/tasks/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskResponseDto>> UpdateTask(
        Guid id,
        [FromBody] UpdateTaskDto dto)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (task == null)
        {
            return NotFound(new
            {
                message = "Task tidak ditemukan."
            });
        }

        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == dto.CategoryId);

        if (!categoryExists)
        {
            return BadRequest(new
            {
                message = "CategoryId tidak ditemukan."
            });
        }

        var statusChanged = task.Status != dto.Status;

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.CategoryId = dto.CategoryId;
        task.Status = dto.Status;
        task.DueDate = dto.DueDate;

        if (statusChanged)
        {
            task.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return Ok(await MapToResponse(task.Id));
    }

    // PATCH: /api/tasks/{id}/status
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<TaskResponseDto>> UpdateStatus(
        Guid id,
        [FromBody] UpdateTaskStatusDto dto)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (task == null)
        {
            return NotFound(new
            {
                message = "Task tidak ditemukan."
            });
        }

        task.Status = dto.Status;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(await MapToResponse(task.Id));
    }

    // DELETE: /api/tasks/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (task == null)
        {
            return NotFound(new
            {
                message = "Task tidak ditemukan."
            });
        }

        task.IsDeleted = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<TaskResponseDto?> MapToResponse(Guid taskId)
    {
        return await _context.Tasks
            .Include(t => t.Category)
            .Where(t => t.Id == taskId)
            .Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                CategoryId = t.CategoryId,
                CategoryName = t.Category.Name,
                Status = t.Status,
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }
}