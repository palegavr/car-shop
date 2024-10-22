using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CarShop.ServiceDefaults.ServiceInterfaces.CarStorage
{
    public class UpdateCarRequest
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public double? PriceForStandartConfiguration { get; set; }
        public string? Color { get; set; }
        public double? EngineCapacity { get; set; }
        public CorpusType? CorpusType { get; set; }
        public FuelType? FuelType { get; set; }
        public int? Count { get; set; }
        public string? ImageUrl { get; set; }
        public string[]? BigImageURLs { get; set; }
        public string? AdditionalCarOptionsJson { get; set; }
        [BindNever]
        [JsonIgnore]
        [NotMapped]
        public List<AdditionalCarOption> AdditionalCarOptions {
            get
            {
                if (AdditionalCarOptionsJson is null)
                {
                    return [];
                }

                try
                {
                    return (JsonSerializer.Deserialize<AdditionalCarOption[]>(AdditionalCarOptionsJson) ??
                    [
                        new()
                        {
                            Id = -1
                        }
                    ]).ToList();
                }
                catch (JsonException)
                {
                    return [new()
                    {
                        Id = -1
                    }];
                }
            }
        }
    }
}
