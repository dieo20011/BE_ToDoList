using Microsoft.AspNetCore.Mvc;
using ToDoList_FS.Model;
using System.Collections.Generic;

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
            return SuccessResult(new { id = user.Id, fullname = user.FullName, username = user.UserName, email = user.Email });
        }
        //Update User Information
        [HttpPut("update-info/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateRequest request)
        {
            // Kiểm tra từng trường và tạo thông báo lỗi cụ thể
            List<string> missingFields = new List<string>();
            
            if(string.IsNullOrWhiteSpace(request.FullName))
                missingFields.Add("FullName");
            
            if(string.IsNullOrWhiteSpace(request.UserName))
                missingFields.Add("UserName");
            
            if(string.IsNullOrWhiteSpace(request.Email))
                missingFields.Add("Email");
            
            if(missingFields.Count > 0)
            {
                string errorMessage = $"The following fields cannot be empty: {string.Join(", ", missingFields)}";
                return ErrorResult(errorMessage);
            }
            
            var isUpdated = await _mongoDbSVC.UpdateUser(id, request.FullName, request.UserName, request.Email);
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
            // Kiểm tra từng trường và tạo thông báo lỗi cụ thể
            List<string> missingFields = new List<string>();
            
            if(string.IsNullOrWhiteSpace(request.OldPassword))
                missingFields.Add("OldPassword");
            
            if(string.IsNullOrWhiteSpace(request.NewPassword))
                missingFields.Add("NewPassword");
            
            if(missingFields.Count > 0)
            {
                string errorMessage = $"The following fields cannot be empty: {string.Join(", ", missingFields)}";
                return ErrorResult(errorMessage);
            }

            var isUpdated = await _mongoDbSVC.UpdatePassword(id, request.OldPassword, request.NewPassword);

            return isUpdated 
                ? SuccessResult("Cập nhật mật khẩu thành công.") 
                : ErrorResult("Không thể cập nhật mật khẩu. Vui lòng kiểm tra lại mật khẩu cũ.");
        }
        //Sign up
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserSignUpRequest user)
        {
            // Kiểm tra từng trường và tạo thông báo lỗi cụ thể
            List<string> missingFields = new List<string>();
            
            if(string.IsNullOrWhiteSpace(user.FullName))
                missingFields.Add("FullName");
            
            if(string.IsNullOrWhiteSpace(user.UserName))
                missingFields.Add("UserName");
            
            if(string.IsNullOrWhiteSpace(user.Password))
                missingFields.Add("Password");
            
            if(string.IsNullOrWhiteSpace(user.Email))
                missingFields.Add("Email");
            
            if(missingFields.Count > 0)
            {
                string errorMessage = $"The following fields cannot be empty: {string.Join(", ", missingFields)}";
                return ErrorResult(errorMessage);
            }
            
            var result = await _mongoDbSVC.RegisterUser(user.UserName, user.Password, user.FullName, user.Email);
            if(result == "Username đã tồn tại")
            {
                return ErrorResult("Username already exists");
            }
            return SuccessResult(result);
        }
        //Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest user)
        {
            // Kiểm tra từng trường và tạo thông báo lỗi cụ thể
            List<string> missingFields = new List<string>();
            
            if(string.IsNullOrWhiteSpace(user.UserName))
                missingFields.Add("UserName");
            
            if(string.IsNullOrWhiteSpace(user.Password))
                missingFields.Add("Password");
            
            if(missingFields.Count > 0)
            {
                string errorMessage = $"The following fields cannot be empty: {string.Join(", ", missingFields)}";
                return ErrorResult(errorMessage);
            }
            
            var result = await _mongoDbSVC.Login(user.UserName, user.Password);
            if(result == "Invalid username or password")
            {
                return ErrorResult("Invalid username or password");
            }
            return SuccessResult(result);
        }
    }
}
