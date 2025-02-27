using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JsonSerializer
{
    public class FruitBasket
    {
        public Apple FirstApple { get; set; }
        public Apple SecondApple { get; set; }

        public List<Apple> ApplesList { get; set; }

        public FruitBasket InnerBasket { get; set; }

        public int basketCount { get; set; }

        [JsonIgnore]
        public string basektName { get; set; }
    }
}
