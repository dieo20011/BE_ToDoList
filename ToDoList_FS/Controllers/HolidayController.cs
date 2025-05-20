using Microsoft.AspNetCore.Mvc;
using ToDoList_FS.Model;

namespace ToDoList_FS.Controllers
{
    [Route("api/holiday")]
    [ApiController]
    public class HolidayController : BaseAPIController
    {
        private readonly MongoDBService _mongoDBService;

        public HolidayController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet("get/{userId}")]
        public async Task<IActionResult> GetHolidays(
            string userId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null)
        {
            var queryParams = new HolidayQueryParams
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Keyword = keyword
            };

            var result = await _mongoDBService.GetHolidaysAsync(userId, queryParams);
            return SuccessResult(result);
        }

        [HttpGet("detail/{userId}/{holidayId}")]
        public async Task<IActionResult> GetHolidayById(string userId, string holidayId)
        {
            var holiday = await _mongoDBService.GetHolidayByIdAsync(holidayId, userId);
            if (holiday == null)
                return ErrorResult("Holiday not found");

            return SuccessResult(holiday);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddHoliday([FromBody] HolidayDTO holidayDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = "Invalid data", errors });
            }
            if (string.IsNullOrEmpty(holidayDto.UserId))
                return ErrorResult("UserId is required");

            await _mongoDBService.CreateHolidayAsync(holidayDto, holidayDto.UserId);
            return SuccessResult("Thêm ngày nghỉ thành công");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateHoliday(string id, [FromBody] HolidayDTO holidayDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = "Invalid data", errors });
            }
            if (string.IsNullOrEmpty(holidayDto.UserId))
                return ErrorResult("UserId is required");

            var success = await _mongoDBService.UpdateHolidayAsync(id, holidayDto.UserId, holidayDto);
            if (!success)
                return ErrorResult("Holiday not found");

            return SuccessResult("Cập nhật ngày nghỉ thành công");
        }

        [HttpDelete("delete/{userId}/{id}")]
        public async Task<IActionResult> DeleteHoliday(string userId, string id)
        {
            var success = await _mongoDBService.DeleteHolidayAsync(id, userId);
            if (!success)
                return ErrorResult("Holiday not found");

            return SuccessResult("Xóa ngày nghỉ thành công");
        }

        [HttpGet("all/{userId}")]
        public async Task<IActionResult> GetAllHolidays(string userId)
        {
            var holidays = await _mongoDBService.GetAllHolidaysAsync(userId);
            return SuccessResult(holidays);
        }
    }
} 