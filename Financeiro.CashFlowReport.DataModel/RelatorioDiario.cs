using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class RelatorioDiario
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]  
    public string Id { get; set; }  

    public string? Data { get; set; }
    public double TotalCreditos { get; set; }
    public double TotalDebitos { get; set; }

    [BsonElement("Lancamentos")]  
    public List<LancamentoDataModel> Lancamentos { get; set; }
}
