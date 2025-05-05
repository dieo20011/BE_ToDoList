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
            // Status values:
            // 0 = All (no filter)
            // 1 = Pending
            // 2 = InProgress
            // 3 = Done
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

        [HttpPost("fix-statuses/{id}")]
        public async Task<IActionResult> FixStatuses(string id)
        {
            await _mongoDBService.UpdateAllTaskStatuses(id);
            return SuccessResult("Đã cập nhật status cho tất cả tasks");
        }
    }
}
