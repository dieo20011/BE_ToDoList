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
            await _mongoDBService.AddTask(todo);
            return SuccessResult("Thêm task thành công");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateTask(string id, [FromBody] TodoItem todo)
        {
            await _mongoDBService.UpdateTask(id, todo);
            return SuccessResult("Cập nhật task thành công");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            await _mongoDBService.DeleteTask(id);
            return SuccessResult("Xóa task thành công");
        }
    }
}
