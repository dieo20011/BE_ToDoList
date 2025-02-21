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

        [HttpGet("get")]
        public async Task<IActionResult> GetTodoList()
        {
            var todos = await _mongoDBService.GetTodoList();
            return SuccessResult(todos);
        }


        [HttpPost("add")]
        public async Task<IActionResult> AddToDoList([FromBody] TodoItem todo)
        {
            await _mongoDBService.AddTodoItem(todo);
            return Ok(new { message = "OK" });
        }
    }
}
