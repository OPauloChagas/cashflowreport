using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class LancamentoDataModel
{
    [BsonElement("Id")]
    [BsonRepresentation(BsonType.String)]  
    public Guid Id { get; set; } 

    public string? Tipo { get; set; }
    public double Valor { get; set; }
    public string? Descricao { get; set; }
    public string? Data { get; set; }
}
