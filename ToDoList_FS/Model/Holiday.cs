using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

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

    public bool IsRecurring { get; set; } = false;
    }

public class HolidayDTO
{
    public string? Id { get; set; }

    [Required(ErrorMessage = "Holiday name is required")]
    [MinLength(2, ErrorMessage = "Holiday name must be at least 2 characters")]
    [MaxLength(200, ErrorMessage = "Holiday name must not exceed 200 characters")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Holiday name must not be blank or whitespace only")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "FromDate is required")]
    public DateTime FromDate { get; set; }

    [Required(ErrorMessage = "ToDate is required")]
    public DateTime ToDate { get; set; }

    [MaxLength(500, ErrorMessage = "Description must not exceed 500 characters")]
    public string? Description { get; set; }

    public bool IsAnnualHoliday { get; set; }

    [Required(ErrorMessage = "UserId is required")]
    public string UserId { get; set; } = null!;

    public bool IsRecurring { get; set; }
    }

public class HolidayQueryParams
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
}

public class HolidayPaginatedResult
{
    public List<Holiday> Items { get; set; } = new List<Holiday>();
    public int TotalRecord { get; set; }
} 
}
