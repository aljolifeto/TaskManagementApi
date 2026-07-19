using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.DTOs;

public class CreateCategoryDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}

public class CategoryResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}