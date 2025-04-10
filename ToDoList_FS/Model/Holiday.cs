using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace ToDoList_FS.Model {
    public class Holiday
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public string Description { get; set; } = null!;

    public bool IsAnnualHoliday { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}

public class HolidayDTO
{
    public string? Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string Description { get; set; } = null!;
    public bool IsAnnualHoliday { get; set; }
    public string UserId { get; set; } = null!;
} 
}
