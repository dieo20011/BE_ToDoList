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

        private const int MaxPageSize = 100;

        [HttpGet("get/{userId}")]
        public async Task<IActionResult> GetHolidays(
            string? userId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ErrorResult("User ID is required");
            if (pageIndex < 1)
                return ErrorResult("Page index must be at least 1");
            if (pageSize < 1 || pageSize > MaxPageSize)
                return ErrorResult($"Page size must be between 1 and {MaxPageSize}");

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
        public async Task<IActionResult> GetHolidayById(string? userId, string? holidayId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ErrorResult("User ID is required");
            if (string.IsNullOrWhiteSpace(holidayId))
                return ErrorResult("Holiday ID is required");

            var holiday = await _mongoDBService.GetHolidayByIdAsync(holidayId, userId);
            if (holiday == null)
                return ErrorResult("Holiday not found");

            return SuccessResult(holiday);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddHoliday([FromBody] HolidayDTO? holidayDto)
        {
            if (holidayDto == null)
                return ErrorResult("Invalid request data");
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return ErrorResult(errors.Count > 0 ? string.Join("; ", errors) : "Invalid request data");
            }
            if (string.IsNullOrWhiteSpace(holidayDto.UserId))
                return ErrorResult("UserId is required");
            if (holidayDto.FromDate > holidayDto.ToDate)
                return ErrorResult("FromDate must not be later than ToDate");

            await _mongoDBService.CreateHolidayAsync(holidayDto, holidayDto.UserId);
            return SuccessResult("Thêm ngày nghỉ thành công");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateHoliday(string? id, [FromBody] HolidayDTO? holidayDto)
        {
            if (string.IsNullOrWhiteSpace(id))
                return ErrorResult("Holiday ID is required");
            if (holidayDto == null)
                return ErrorResult("Invalid request data");
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return ErrorResult(errors.Count > 0 ? string.Join("; ", errors) : "Invalid request data");
            }
            if (string.IsNullOrWhiteSpace(holidayDto.UserId))
                return ErrorResult("UserId is required");
            if (holidayDto.FromDate > holidayDto.ToDate)
                return ErrorResult("FromDate must not be later than ToDate");

            var success = await _mongoDBService.UpdateHolidayAsync(id, holidayDto.UserId, holidayDto);
            if (!success)
                return ErrorResult("Holiday not found");

            return SuccessResult("Cập nhật ngày nghỉ thành công");
        }

        [HttpDelete("delete/{userId}/{id}")]
        public async Task<IActionResult> DeleteHoliday(string? userId, string? id)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ErrorResult("User ID is required");
            if (string.IsNullOrWhiteSpace(id))
                return ErrorResult("Holiday ID is required");

            var success = await _mongoDBService.DeleteHolidayAsync(id, userId);
            if (!success)
                return ErrorResult("Holiday not found");

            return SuccessResult("Xóa ngày nghỉ thành công");
        }

        [HttpGet("all/{userId}")]
        public async Task<IActionResult> GetAllHolidays(string? userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ErrorResult("User ID is required");

            var holidays = await _mongoDBService.GetAllHolidaysAsync(userId);
            return SuccessResult(holidays);
        }
    }
} 