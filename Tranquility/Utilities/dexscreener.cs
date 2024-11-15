using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace Tranquility.Utilities
{
    public static class Dexscreener
    {
        public static async Task<decimal> GetPrice(string mint)
        {
            decimal price = 0;
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
                var BirdEyeAPIrequest = await httpClient.GetStringAsync(new Uri("https://api.dexscreener.com/latest/dex/tokens/" + mint));

                PriceData price_data = JsonConvert.DeserializeObject<PriceData>(BirdEyeAPIrequest);
                if (price_data.pairs != null)
                {
                    price = Convert.ToDecimal(price_data.pairs[0].priceUsd);
                }
            }
            return price;
        }
        public static async Task<PriceData> GetTokenData(string mint)
        {
            
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
                var BirdEyeAPIrequest = await httpClient.GetStringAsync(new Uri("https://api.dexscreener.com/latest/dex/tokens/" + mint));

                PriceData price_data = JsonConvert.DeserializeObject<PriceData>(BirdEyeAPIrequest);
                 return price_data;
            }
           
        }
    }

    public class PriceData
    {
        public string schemaVersion { get; set; }
        public Pair[] pairs { get; set; }
    }

    public class Pair
    {
        public string chainId { get; set; }
        public string dexId { get; set; }
        public string url { get; set; }
        public string pairAddress { get; set; }
        public string[] labels { get; set; }
        public Basetoken baseToken { get; set; }
        public Quotetoken quoteToken { get; set; }
        public string priceNative { get; set; }
        public string priceUsd { get; set; }
        public Txns txns { get; set; }
        public Volume volume { get; set; }
        public Pricechange priceChange { get; set; }
        public Liquidity liquidity { get; set; }
        public int fdv { get; set; }
        public int marketCap { get; set; }
        public long pairCreatedAt { get; set; }
    }

    public class Basetoken
    {
        public string address { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
    }

    public class Quotetoken
    {
        public string address { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
    }

    public class Txns
    {
        public M5 m5 { get; set; }
        public H1 h1 { get; set; }
        public H6 h6 { get; set; }
        public H24 h24 { get; set; }
    }

    public class M5
    {
        public int buys { get; set; }
        public int sells { get; set; }
    }

    public class H1
    {
        public int buys { get; set; }
        public int sells { get; set; }
    }

    public class H6
    {
        public int buys { get; set; }
        public int sells { get; set; }
    }

    public class H24
    {
        public int buys { get; set; }
        public int sells { get; set; }
    }

    public class Volume
    {
        public float h24 { get; set; }
        public float h6 { get; set; }
        public float h1 { get; set; }
        public int m5 { get; set; }
    }

    public class Pricechange
    {
        public int m5 { get; set; }
        public float h1 { get; set; }
        public float h6 { get; set; }
        public float h24 { get; set; }
    }

    public class Liquidity
    {
        public float usd { get; set; }
        public float _base { get; set; }
        public float quote { get; set; }
    }

}
