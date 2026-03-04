using Microsoft.AspNetCore.Mvc;
using ToDoList_FS.Model;

namespace ToDoList_FS.Controllers
{
    [Route("api/courts")]
    [ApiController]
    public class CourtController : BaseAPIController
    {
        private readonly MongoDBService _mongoDBService;

        public CourtController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService ?? throw new ArgumentNullException(nameof(mongoDBService));
        }

        /// <summary>
        /// Create a new court. Validates name (min 3) and password (min 4).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCourt([FromBody] CreateCourtRequest? request)
        {
            if (request == null)
                return ErrorResult("Invalid request data");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return ErrorResult(errors.Count > 0 ? string.Join("; ", errors) : "Invalid request data");
            }

            var result = await _mongoDBService.CreateCourtAsync(request);
            if (result.IsSuccess && result.Data != null)
                return SuccessResult(result.Data, result.Message ?? "Court created successfully");
            return ErrorResult(result.Message);
        }

        /// <summary>
        /// Get all courts (response does not include password).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCourts()
        {
            var courts = await _mongoDBService.GetCourtsAsync();
            return SuccessResult(courts);
        }

        /// <summary>
        /// Get court by id (response does not include password).
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourtById(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return ErrorResult("Invalid court ID");

            var court = await _mongoDBService.GetCourtByIdAsync(id);
            if (court == null)
                return ErrorResult("Court not found");
            return SuccessResult(court);
        }

        /// <summary>
        /// Verify court password for access. Returns success only; does not return password.
        /// </summary>
        /// <summary>
        /// Delete a court by id. All players belonging to this court are also deleted (cascade).
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourt(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return ErrorResult("Invalid court ID");

            var result = await _mongoDBService.DeleteCourtAsync(id);
            if (result.IsSuccess)
                return SuccessResult(result.Message ?? "Court deleted successfully");
            return ErrorResult(result.Message);
        }

        /// <summary>
        /// Get session history for a court.
        /// </summary>
        [HttpGet("{id}/sessions")]
        public async Task<IActionResult> GetCourtSessions(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return ErrorResult("Invalid court ID");

            var sessions = await _mongoDBService.GetCourtSessionsAsync(id);
            return SuccessResult(sessions);
        }

        /// <summary>
        /// Save a session (payment snapshot) for a court.
        /// </summary>
        [HttpPost("{id}/sessions")]
        public async Task<IActionResult> SaveCourtSession(string? id, [FromBody] SaveCourtSessionRequest? request)
        {
            if (string.IsNullOrWhiteSpace(id))
                return ErrorResult("Invalid court ID");
            if (request == null)
                return ErrorResult("Invalid request data");

            var result = await _mongoDBService.SaveCourtSessionAsync(id, request);
            if (result.IsSuccess && result.Data != null)
                return SuccessResult(result.Data, result.Message ?? "Session saved successfully");
            return ErrorResult(result.Message);
        }

        [HttpPost("{id}/verify-password")]
        public async Task<IActionResult> VerifyPassword(string? id, [FromBody] VerifyCourtPasswordRequest? body)
        {
            if (string.IsNullOrWhiteSpace(id))
                return ErrorResult("Invalid court ID");
            if (body == null)
                return ErrorResult("Password is required");
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return ErrorResult(errors.Count > 0 ? string.Join("; ", errors) : "Invalid request data");
            }
            if (string.IsNullOrWhiteSpace(body.Password))
                return ErrorResult("Password is required");

            var isValid = await _mongoDBService.VerifyCourtPasswordAsync(id, body.Password);
            if (!isValid)
                return ErrorResult("Invalid password");
            return SuccessResult(new { verified = true }, "Password verified successfully");
        }
    }
}
