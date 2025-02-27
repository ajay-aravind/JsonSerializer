using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JsonSerializer
{
    public class Apple
    {
        [JsonPropertyNameAttribute("AppleColor")]
        public string Color { get; set; }

        [JsonPropertyNameAttribute("ApplePrice")]
        public int Price { get; set; }

        public Apple(string color, int price)
        {
            this.Color = color;
            this.Price = price;
        }

        public bool ShouldSerializePrice()
        {
            return true;
        }

        public bool ShouldSerializeColor()
        {
            return false;
        }
    }
}
