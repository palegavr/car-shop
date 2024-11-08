using System.Text.Json;
using System.Text.Json.Serialization;
using CarShop.CarStorageService.Grpc;

namespace CarShop.ApiGateway.Models;

public class CarEditProcessDataPayload
{
    [JsonRequired]
    [JsonPropertyName("brand")]
    public string Brand { get; set; }

    [JsonRequired]
    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonRequired]
    [JsonPropertyName("price")]
    public double Price { get; set; }

    [JsonRequired]
    [JsonPropertyName("color")]
    public string Color { get; set; }

    [JsonRequired]
    [JsonPropertyName("engineCapacity")]
    public double EngineCapacity { get; set; }

    [JsonRequired]
    [JsonPropertyName("corpusType")]
    public Car.Types.CorpusType CorpusType { get; set; }

    [JsonRequired]
    [JsonPropertyName("fuelType")]
    public Car.Types.FuelType FuelType { get; set; }

    [JsonRequired]
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonRequired]
    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; }

    [JsonRequired]
    [JsonPropertyName("bigImageUrls")]
    public List<string> BigImageUrls { get; set; }

    [JsonRequired]
    [JsonPropertyName("additionalCarOptions")]
    [JsonConverter(typeof(AdditionalCarOptionsConverter))]
    public List<AdditionalCarOption> AdditionalCarOptions { get; set; }
}

public class AdditionalCarOptionsConverter : JsonConverter<List<AdditionalCarOption>>
{
    public override List<AdditionalCarOption> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            string jsonString = reader.GetString() ?? throw new ArgumentNullException();
            if (string.IsNullOrEmpty(jsonString))
            {
                return [];
            }

            var result = JsonSerializer.Deserialize<List<AdditionalCarOption>>(jsonString, options) ??
                         throw new ArgumentNullException();
            return result;
        }
        catch
        {
            return [];
        }
    }

    public override void Write(Utf8JsonWriter writer, List<AdditionalCarOption> value, JsonSerializerOptions options)
    {
        string jsonString = JsonSerializer.Serialize(value, options);
        writer.WriteStringValue(jsonString);
    }
}