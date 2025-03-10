using Microsoft.AspNetCore.Mvc;
using ToDoList_FS.Model;

namespace ToDoList_FS.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseAPIController
    {
        public readonly MongoDBService _mongoDbSVC;

        public AuthController(MongoDBService mongoDbSVC)
        {
            _mongoDbSVC = mongoDbSVC;
        }
        //Get User By Id
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _mongoDbSVC.GetUserById(id);
            if(user == null)
            {
                return NotFound("User not found");
            }
            return SuccessResult(new { id = user.Id, fullname = user.FullName, username = user.UserName });
        }
        //Update User Information
        [HttpPut("update-info/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateRequest request)
        {
            if(string.IsNullOrWhiteSpace(request.FullName) || string.IsNullOrWhiteSpace(request.UserName))
            {

                return ErrorResult("Username or FullName must be not null");
            }
            var isUpdated = await _mongoDbSVC.UpdateUser(id, request.FullName, request.UserName);
            if(!isUpdated)
            {
                return NotFound("User not found");
            }
            return SuccessResult("Update success");
        }
        //Update User Password
        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> UpdatePassword(string id, [FromBody] PasswordUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OldPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest("Mật khẩu không được để trống.");

            var isUpdated = await _mongoDbSVC.UpdatePassword(id, request.OldPassword, request.NewPassword);

            return isUpdated ? SuccessResult("Cập nhật mật khẩu thành công.") : ErrorResult("Không thể cập nhật mật khẩu.");
        }
        //Sign up
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserSignUpRequest user)
        {
            var result = await _mongoDbSVC.RegisterUser(user.UserName, user.Password, user.FullName);
            if(result == "Username đã tồn tại")
            {
                return ErrorResult("Username or password invalid");
            }
            return SuccessResult(result);
        }
        //Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var result = await _mongoDbSVC.Login(user.UserName, user.Password);
            if(result == "Invalid username or password")
            {
                return ErrorResult("Invalid username or password");
            }
            return SuccessResult(result);
        }
    }
}
