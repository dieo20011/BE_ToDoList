using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
namespace ToDoList_FS.Model
{
    // Enum for task status
    public enum TaskStatus
    {
        All = 3,        // Used for filtering
        Pending = 0,    // Task is pending
        InProgress = 1, // Task is in progress
        Done = 2        // Task is completed
    }

    // Model gốc cho MongoDB
    public class TodoItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("title")]
        public string? Title { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("status")]
        public int Status { get; set; }

        [BsonElement("fromDate")]
        public string? FromDate { get; set; }

        [BsonElement("toDate")]
        public string? ToDate { get; set; }

        [BsonElement("userId")]
        public string? UserId { get; set; }
    }

    // Model cho GET/Response (không cần [Required])
    public class TodoItemResponse
    {
        [BsonId]
        public string? Id { get; set; }
        [BsonElement("title")]
        public string? Title { get; set; }
        [BsonElement("description")]
        public string? Description { get; set; }
        [BsonElement("status")]
        public int Status { get; set; }
        [BsonElement("fromDate")]
        public string? FromDate { get; set; }
        [BsonElement("toDate")]
        public string? ToDate { get; set; }
        [BsonElement("userId")]
        public string? UserId { get; set; }
    }

    // Model cho POST/PUT (có [Required])
    public class TodoItemRequest
    {
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public int Status { get; set; }
        [Required]
        public string? FromDate { get; set; }
        [Required]
        public string? ToDate { get; set; }
        [Required]
        public string? UserId { get; set; }
    }
}
