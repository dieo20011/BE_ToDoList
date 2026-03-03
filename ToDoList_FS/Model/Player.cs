using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ToDoList_FS.Model
{
    /// <summary>
    /// Number of set checkboxes per player (matches FE).
    /// </summary>
    public static class PlayerConstants
    {
        public const int CheckboxCount = 12;
    }

    /// <summary>
    /// Player entity stored in MongoDB.
    /// </summary>
    public class Player
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("courtId")]
        public string CourtId { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("checkboxes")]
        public bool[] Checkboxes { get; set; } = Array.Empty<bool>();

        [BsonElement("isPaid")]
        public bool IsPaid { get; set; }

        [BsonElement("createdDate")]
        public DateTime CreatedDate { get; set; }
    }

    /// <summary>
    /// Request DTO for adding a player to a court.
    /// </summary>
    public class CreatePlayerRequest
    {
        [Required(ErrorMessage = "Court ID is required")]
        public string CourtId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Player name is required")]
        [MinLength(2, ErrorMessage = "Player name must be at least 2 characters")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request DTO for updating a single checkbox (set played).
    /// </summary>
    public class UpdatePlayerCheckboxRequest
    {
        [Required(ErrorMessage = "Player ID is required")]
        public string PlayerId { get; set; } = string.Empty;

        /// <summary>
        /// Index 0..11 for the set checkbox.
        /// </summary>
        public int CheckboxIndex { get; set; }

        public bool IsChecked { get; set; }
    }

    /// <summary>
    /// Request DTO for updating payment status.
    /// </summary>
    public class UpdatePlayerPaymentRequest
    {
        [Required(ErrorMessage = "Player ID is required")]
        public string PlayerId { get; set; } = string.Empty;

        public bool IsPaid { get; set; }
    }

    /// <summary>
    /// Response DTO for player.
    /// </summary>
    public class PlayerResponse
    {
        public string Id { get; set; } = string.Empty;
        public string CourtId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool[] Checkboxes { get; set; } = Array.Empty<bool>();
        public bool IsPaid { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
