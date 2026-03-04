using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using ToDoList_FS.Model;

namespace ToDoList_FS.Hubs
{
    public class CourtHub : Hub
    {
        private readonly MongoDBService _mongoDBService;

        /// <summary>
        /// Tracks connectionId → (courtId, displayName) for UserLeft on disconnect.
        /// </summary>
        private static readonly ConcurrentDictionary<string, (string CourtId, string DisplayName)> _connectionMap = new();

        public CourtHub(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService ?? throw new ArgumentNullException(nameof(mongoDBService));
        }

        public async Task JoinCourt(string courtId, string password, string? displayName)
        {
            if (string.IsNullOrWhiteSpace(courtId))
                throw new HubException("Court ID is required");

            var isValid = await _mongoDBService.VerifyCourtPasswordAsync(courtId, password);
            if (!isValid)
                throw new HubException("Invalid password");

            var name = string.IsNullOrWhiteSpace(displayName) ? Context.ConnectionId : displayName.Trim();

            await Groups.AddToGroupAsync(Context.ConnectionId, courtId);
            _connectionMap[Context.ConnectionId] = (courtId, name);

            // Notify caller
            await Clients.Caller.SendAsync("JoinedCourt", courtId, name);
            // Broadcast to others in the group so they can update their member list
            await Clients.OthersInGroup(courtId).SendAsync("UserJoined", name);
        }

        public async Task LeaveCourt(string courtId)
        {
            if (string.IsNullOrWhiteSpace(courtId))
                return;

            if (_connectionMap.TryRemove(Context.ConnectionId, out var info))
                await Clients.OthersInGroup(info.CourtId).SendAsync("UserLeft", info.DisplayName);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, courtId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connectionMap.TryRemove(Context.ConnectionId, out var info))
            {
                await Clients.Group(info.CourtId).SendAsync("UserLeft", info.DisplayName);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, info.CourtId);
            }
            await base.OnDisconnectedAsync(exception);
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

            _connectionMap.TryGetValue(Context.ConnectionId, out var info);
            var payload = new
            {
                playerId,
                checkboxIndex,
                isChecked,
                updatedBy = updatedBy ?? info.DisplayName ?? Context.ConnectionId
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
