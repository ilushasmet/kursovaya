using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace WebApplication1
{
    public class Pogoda_predstavlenie
    {
        public class weatherinfo
        {
            [JsonProperty("main")]
            public string main { get; set; }
            [JsonProperty("description")]
            public string description { get; set; }       
        }
        [JsonProperty("weather")]
        public List<weatherinfo> weather { get; set; }

        [JsonProperty("main")]
        public Dictionary<string,double> main { get; set; }
        //"main": 
        //    {
        //     "temp": 287.44,
        //     "feels_like": 284.34,
        //     "temp_min": 287.15,
        //     "temp_max": 288.15,
        //     "pressure": 1005,
        //     "humidity": 82
        //    },
    }
}
