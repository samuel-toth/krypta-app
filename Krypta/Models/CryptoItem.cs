using Newtonsoft.Json;

namespace Krypta.Models
{
    public class CryptoItem
    {
        [JsonProperty("id")] public string Id { get; set; }

        [JsonProperty("symbol")] public string Symbol { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("image")] public string Image { get; set; }

        [JsonProperty("current_price")] public double CurrentPrice { get; set; }

        [JsonProperty("market_cap")] public double? MarketCap { get; set; }

        [JsonProperty("market_cap_rank")] public double? MarketCapRank { get; set; }
        
        [JsonProperty("price_change_percentage_24h")] public double? PriceChangePercentage24H { get; set; }

        [JsonProperty("circulating_supply")] public double? CirculatingSupply { get; set; }

        [JsonProperty("total_supply")] public double? TotalSupply { get; set; }

        [JsonProperty("max_supply")] public double? MaxSupply { get; set; }

        [JsonProperty("ath")] public double? Ath { get; set; }
    }
}