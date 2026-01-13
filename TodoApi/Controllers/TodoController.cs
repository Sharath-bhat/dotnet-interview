using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _todoService;
        private readonly ILogger<TodoController> _logger;

        public TodoController(ITodoRepository todoService, ILogger<TodoController> logger)
        {
            _todoService = todoService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Todo>> CreateTodo([FromBody] CreateTodoRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var todo = new Todo
                {
                    Title = request.Title,
                    Description = request.Description,
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _todoService.CreateAsync(todo);
                return CreatedAtAction(nameof(GetTodo), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating todo");
                return StatusCode(500, "An error occurred while creating the todo");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
        {
            try
            {
                var todos = await _todoService.GetAllAsync();
                return Ok(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving todos");
                return StatusCode(500, "An error occurred while retrieving todos");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Todo>> GetTodo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid todo ID");
            }

            try
            {
                var todo = await _todoService.GetByIdAsync(id);
                if (todo == null)
                {
                    return NotFound();
                }

                return Ok(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving todo with ID {TodoId}", id);
                return StatusCode(500, "An error occurred while retrieving the todo");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Todo>> UpdateTodo(int id, [FromBody] UpdateTodoRequest request)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid todo ID");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingTodo = await _todoService.GetByIdAsync(id);
                if (existingTodo == null)
                {
                    return NotFound();
                }

                var todo = new Todo
                {
                    Title = request.Title,
                    Description = request.Description,
                    IsCompleted = request.IsCompleted,
                    CreatedAt = existingTodo.CreatedAt
                };

                var result = await _todoService.UpdateAsync(id, todo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating todo with ID {TodoId}", id);
                return StatusCode(500, "An error occurred while updating the todo");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid todo ID");
            }

            try
            {
                var result = await _todoService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting todo with ID {TodoId}", id);
                return StatusCode(500, "An error occurred while deleting the todo");
            }
        }
    }
}