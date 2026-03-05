using Microsoft.AspNetCore.Mvc;
using ToDoList_FS.Model;

namespace ToDoList_FS.Controllers
{
    [Route("api/courts/{courtId}/players")]
    [ApiController]
    public class PlayerController : BaseAPIController
    {
        private readonly MongoDBService _mongoDBService;

        public PlayerController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService ?? throw new ArgumentNullException(nameof(mongoDBService));
        }

        /// <summary>
        /// Get all players of a court.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPlayersByCourtId(string? courtId)
        {
            if (string.IsNullOrWhiteSpace(courtId))
                return ErrorResult("Invalid court ID");

            var players = await _mongoDBService.GetPlayersByCourtIdAsync(courtId);
            return SuccessResult(players);
        }

        /// <summary>
        /// Add a player to a court. Name min 2 characters.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddPlayer(string? courtId, [FromBody] CreatePlayerRequest? request)
        {
            if (string.IsNullOrWhiteSpace(courtId))
                return ErrorResult("Invalid court ID");
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

            if (string.IsNullOrWhiteSpace(request.CourtId) || !string.Equals(request.CourtId, courtId, StringComparison.OrdinalIgnoreCase))
                request = new CreatePlayerRequest { CourtId = courtId, Name = request.Name ?? string.Empty };

            var result = await _mongoDBService.AddPlayerAsync(request);
            if (result.IsSuccess && result.Data != null)
                return SuccessResult(result.Data, result.Message ?? "Player added successfully");
            return ErrorResult(result.Message);
        }

        /// <summary>
        /// Update a single set checkbox for a player (realtime tracking). CheckboxIndex 0..11.
        /// </summary>
        [HttpPatch("checkbox")]
        public async Task<IActionResult> UpdateCheckbox([FromBody] UpdatePlayerCheckboxRequest? request)
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
            if (string.IsNullOrWhiteSpace(request.PlayerId))
                return ErrorResult("Player ID is required");

            var result = await _mongoDBService.UpdatePlayerCheckboxAsync(request);
            if (result.IsSuccess)
                return SuccessResult(result.Message);
            return ErrorResult(result.Message);
        }

        /// <summary>
        /// Update payment status for a player.
        /// </summary>
        [HttpPatch("payment")]
        public async Task<IActionResult> UpdatePayment([FromBody] UpdatePlayerPaymentRequest? request)
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
            if (string.IsNullOrWhiteSpace(request.PlayerId))
                return ErrorResult("Player ID is required");

            var result = await _mongoDBService.UpdatePlayerPaymentAsync(request);
            if (result.IsSuccess)
                return SuccessResult(result.Message);
            return ErrorResult(result.Message);
        }

        /// <summary>
        /// Delete a player from a court.
        /// </summary>
        [HttpDelete("{playerId}")]
        public async Task<IActionResult> DeletePlayer(string? courtId, string? playerId)
        {
            if (string.IsNullOrWhiteSpace(courtId))
                return ErrorResult("Invalid court ID");
            if (string.IsNullOrWhiteSpace(playerId))
                return ErrorResult("Player ID is required");

            var result = await _mongoDBService.DeletePlayerAsync(courtId, playerId);
            if (result.IsSuccess)
                return SuccessResult(result.Message);
            return ErrorResult(result.Message);
        }
    }
}
