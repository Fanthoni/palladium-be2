using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PalladiumBE;
public class CatalogItem
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string id { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
}
