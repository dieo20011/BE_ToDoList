using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ToDoList_FS.Model
{
    public class CourtSessionPlayerSnapshot
    {
        [BsonElement("playerId")]
        public string PlayerId { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("checkedSets")]
        public int CheckedSets { get; set; }

        [BsonElement("isPaid")]
        public bool IsPaid { get; set; }
    }

    public class CourtSession
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("courtId")]
        public string CourtId { get; set; } = string.Empty;

        [BsonElement("sessionDate")]
        public DateTime SessionDate { get; set; }

        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;

        [BsonElement("courtFee")]
        public decimal CourtFee { get; set; }

        [BsonElement("shuttlecockCount")]
        public int ShuttlecockCount { get; set; }

        [BsonElement("shuttlecockPrice")]
        public decimal ShuttlecockPrice { get; set; }

        [BsonElement("waterFee")]
        public decimal WaterFee { get; set; }

        [BsonElement("maleCount")]
        public int MaleCount { get; set; }

        [BsonElement("femaleCount")]
        public int FemaleCount { get; set; }

        [BsonElement("maleFixedFee")]
        public decimal MaleFixedFee { get; set; }

        [BsonElement("femaleFixedFee")]
        public decimal FemaleFixedFee { get; set; }

        [BsonElement("malePerPerson")]
        public decimal MalePerPerson { get; set; }

        [BsonElement("femalePerPerson")]
        public decimal FemalePerPerson { get; set; }

        [BsonElement("totalCost")]
        public decimal TotalCost { get; set; }

        [BsonElement("players")]
        public List<CourtSessionPlayerSnapshot> Players { get; set; } = new();

        [BsonElement("createdDate")]
        public DateTime CreatedDate { get; set; }
    }

    public class SaveCourtSessionRequest
    {
        public DateTime SessionDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public decimal CourtFee { get; set; }
        public int ShuttlecockCount { get; set; }
        public decimal ShuttlecockPrice { get; set; }
        public decimal WaterFee { get; set; }
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
        public decimal MaleFixedFee { get; set; }
        public decimal FemaleFixedFee { get; set; }
        public decimal MalePerPerson { get; set; }
        public decimal FemalePerPerson { get; set; }
        public decimal TotalCost { get; set; }
        public List<CourtSessionPlayerSnapshotRequest> Players { get; set; } = new();
    }

    public class CourtSessionPlayerSnapshotRequest
    {
        public string PlayerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int CheckedSets { get; set; }
        public bool IsPaid { get; set; }
    }

    public class CourtSessionResponse
    {
        public string Id { get; set; } = string.Empty;
        public string CourtId { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public decimal CourtFee { get; set; }
        public int ShuttlecockCount { get; set; }
        public decimal ShuttlecockPrice { get; set; }
        public decimal WaterFee { get; set; }
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
        public decimal MaleFixedFee { get; set; }
        public decimal FemaleFixedFee { get; set; }
        public decimal MalePerPerson { get; set; }
        public decimal FemalePerPerson { get; set; }
        public decimal TotalCost { get; set; }
        public List<CourtSessionPlayerSnapshot> Players { get; set; } = new();
        public DateTime CreatedDate { get; set; }
    }
}
