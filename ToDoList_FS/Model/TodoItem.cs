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

    public class TodoItem
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("title")]
        [Required]
        public string Title { get; set; }

        [BsonElement("description")]
        [Required]
        public string Description { get; set; }
        [BsonElement("status")]
        [Required]
        public int Status { get; set; }

        [BsonElement("fromDate")]
        [Required]
        public string FromDate { get; set; }
        [BsonElement("toDate")]
        [Required]
        public string ToDate { get; set; }
        [BsonElement("userId")]
        [Required]
        public string UserId { get; set; }
    }
}
