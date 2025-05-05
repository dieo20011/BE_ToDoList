using Microsoft.AspNetCore.Mvc;
using ToDoList_FS.Model;

namespace ToDoList_FS.Controllers
{
    [Route("api/todo")]
    [ApiController]
    public class ToDoController : BaseAPIController
    {
        private readonly MongoDBService _mongoDBService;

        // Sử dụng Dependency Injection
        public ToDoController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetTodoList(string id, [FromQuery] int? status)
        {
            // Use the provided status or default to 0 (All) if not provided
            int statusValue = status ?? 0;
            var todos = await _mongoDBService.GetTodoList(id, statusValue);
            return SuccessResult(todos);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddTask([FromBody] TodoItem todo)
        {
            // Ensure status is one of the valid values
            if (todo.Status < 1 || todo.Status > 3)
            {
                todo.Status = (int)Model.TaskStatus.Pending; // Default to Pending if invalid
            }
            await _mongoDBService.AddTask(todo);
            return SuccessResult("Thêm task thành công");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateTask(string id, [FromBody] TodoItem todo)
        {
            // Ensure status is one of the valid values
            if (todo.Status < 1 || todo.Status > 3)
            {
                todo.Status = (int)Model.TaskStatus.Pending; // Default to Pending if invalid
            }
            await _mongoDBService.UpdateTask(id, todo);
            return SuccessResult("Cập nhật task thành công");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            await _mongoDBService.DeleteTask(id);
            return SuccessResult("Xóa task thành công");
        }

        [HttpGet("debug/{id}")]
        public async Task<IActionResult> DebugTasks(string id)
        {
            var tasks = await _mongoDBService.GetTodoList(id);
            var debug = tasks.Select(t => new { 
                Id = t.Id, 
                Title = t.Title, 
                Status = t.Status, 
                StatusType = t.Status.GetType().Name
            }).ToList();
            return SuccessResult(debug);
        }

        [HttpGet("task/{id}")]
        public async Task<IActionResult> GetTaskById(string id)
        {
            var task = await _mongoDBService.GetTaskById(id);
            if (task == null)
            {
                return NotFound("Task not found");
            }
            return SuccessResult(task);
        }
    }
}
