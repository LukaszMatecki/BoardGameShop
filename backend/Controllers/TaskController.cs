using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AdminTaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "todo";
}

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminTasksController : ControllerBase
{
    private static readonly List<AdminTaskDto> _tasks = new();
    private static int _nextId = 1;

    [HttpGet]
    public IActionResult GetTasks()
    {
        return Ok(_tasks);
    }

    [HttpPost]
    public IActionResult CreateTask(AdminTaskDto dto)
    {
        dto.Id = _nextId++;
        _tasks.Add(dto);
        
        return Ok(dto);
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateTask(int id, AdminTaskDto dto)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task == null) 
            return NotFound();

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Status = dto.Status;

        return Ok(task);
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteTask(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task == null) 
            return NotFound();

        _tasks.Remove(task);
        
        return NoContent();
    }
}