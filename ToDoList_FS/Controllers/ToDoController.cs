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

        /// <summary>
        /// Get todo list for a user with optional status filter
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="status">Optional filter: 0=All, 1=Pending, 2=InProgress, 3=Done</param>
        /// <returns>List of todo items</returns>
        /// <example>
        /// GET /api/todo/get/123           - Get all tasks for user 123
        /// GET /api/todo/get/123?status=1  - Get only pending tasks
        /// GET /api/todo/get/123?status=2  - Get only in-progress tasks
        /// GET /api/todo/get/123?status=3  - Get only completed tasks
        /// </example>
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
            await _mongoDBService.AddTask(todo);
            return SuccessResult("Thêm task thành công");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateTask(string id, [FromBody] TodoItem todo)
        {
            await _mongoDBService.UpdateTask(id, todo);
            return SuccessResult("Cập nhật task thành công");
        }

        /// <summary>
        /// Delete a todo item
        /// </summary>
        /// <param name="id">Todo item ID to delete</param>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            await _mongoDBService.DeleteTask(id);
            return SuccessResult("Xóa task thành công");
        }
    }
}
