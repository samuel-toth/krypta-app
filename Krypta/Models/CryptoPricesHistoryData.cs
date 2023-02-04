using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Krypta.Models
{
    public class CryptoPricesHistoryData
    {
        [JsonProperty("prices")] public List<List<double>> Prices { get; set; }
        
        public List<CryptoDatePrice> GetChartData()
        {
            return (from entry in Prices let dateTime = 
                DateTimeOffset.FromUnixTimeMilliseconds((long)entry[0]).DateTime 
                select new CryptoDatePrice(dateTime, entry[1])).ToList();
        }
    }
}