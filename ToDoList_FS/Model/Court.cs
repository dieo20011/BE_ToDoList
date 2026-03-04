using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ToDoList_FS.Model
{
    /// <summary>
    /// Court entity stored in MongoDB.
    /// </summary>
    public class Court
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("createdDate")]
        public DateTime CreatedDate { get; set; }
    }

    /// <summary>
    /// Request DTO for creating a court.
    /// </summary>
    public class CreateCourtRequest
    {
        [Required(ErrorMessage = "Court name is required")]
        [MinLength(3, ErrorMessage = "Court name must be at least 3 characters")]
        [MaxLength(200, ErrorMessage = "Court name must not exceed 200 characters")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Court name must not be blank or whitespace only")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(4, ErrorMessage = "Password must be at least 4 characters")]
        [MaxLength(100, ErrorMessage = "Password must not exceed 100 characters")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Password must not be blank or whitespace only")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response DTO for court (includes Id and CreatedDate; exclude password when listing).
    /// </summary>
    public class CourtResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    /// <summary>
    /// Response when client needs to verify password (e.g. after create or verify).
    /// </summary>
    public class CourtWithPasswordResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    /// <summary>
    /// Request to verify court access.
    /// </summary>
    public class VerifyCourtPasswordRequest
    {
        [Required(ErrorMessage = "Password is required")]
        [MinLength(1, ErrorMessage = "Password must not be empty")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Password must not be blank or whitespace only")]
        public string Password { get; set; } = string.Empty;
    }
}
