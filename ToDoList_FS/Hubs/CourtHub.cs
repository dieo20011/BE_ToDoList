using Microsoft.AspNetCore.SignalR;
using ToDoList_FS.Model;

namespace ToDoList_FS.Hubs
{
    public class CourtHub : Hub
    {
        private readonly MongoDBService _mongoDBService;

        public CourtHub(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService ?? throw new ArgumentNullException(nameof(mongoDBService));
        }

        public async Task JoinCourt(string courtId, string password, string? displayName)
        {
            if (string.IsNullOrWhiteSpace(courtId))
            {
                throw new HubException("Court ID is required");
            }

            var isValid = await _mongoDBService.VerifyCourtPasswordAsync(courtId, password);
            if (!isValid)
            {
                throw new HubException("Invalid password");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, courtId);
            await Clients.Caller.SendAsync("JoinedCourt", courtId, displayName ?? Context.ConnectionId);
        }

        public Task LeaveCourt(string courtId)
        {
            if (string.IsNullOrWhiteSpace(courtId))
                return Task.CompletedTask;
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, courtId);
        }

        public Task BroadcastCourtAdded(object court)
        {
            return Clients.All.SendAsync("CourtAdded", court);
        }

        public Task BroadcastPlayerAdded(string courtId, object player)
        {
            if (string.IsNullOrWhiteSpace(courtId))
                return Task.CompletedTask;
            return Clients.Group(courtId).SendAsync("PlayerAdded", player);
        }

        public Task BroadcastCheckboxUpdate(string courtId, string playerId, int checkboxIndex, bool isChecked, string? updatedBy)
        {
            if (string.IsNullOrWhiteSpace(courtId))
                return Task.CompletedTask;
            var payload = new
            {
                playerId,
                checkboxIndex,
                isChecked,
                updatedBy = updatedBy ?? Context.ConnectionId
            };
            return Clients.Group(courtId).SendAsync("CheckboxUpdated", payload);
        }

        public Task BroadcastPaymentUpdate(string courtId, object request)
        {
            if (string.IsNullOrWhiteSpace(courtId))
                return Task.CompletedTask;
            return Clients.Group(courtId).SendAsync("PaymentUpdated", request);
        }
    }
}
