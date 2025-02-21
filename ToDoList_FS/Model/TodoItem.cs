using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace ToDoList_FS.Model
{
    public class TodoItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }
        [BsonElement("status")]
        public int Status { get; set; }

        [BsonElement("fromDate")]
        public string FromDate { get; set; }
        [BsonElement("toDate")]
        public string ToDate { get; set; }
    }
}
