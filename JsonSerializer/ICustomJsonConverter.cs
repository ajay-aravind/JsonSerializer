using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonSerializer
{
    public interface ICustomJsonConverter
    {
        public string WriteJson(object input);
    }

    internal class LemonJsonConverter : ICustomJsonConverter
    {
        public string WriteJson(object input)
        {
            if(input is Lemon)
            {
                Lemon inputLemon = (Lemon)input;
                return "Name:" + inputLemon.name + "," + "Color:" + inputLemon.color;
            }

            throw new InvalidCastException("Invalid type object, expected Lemon object");
        }
    }

    internal class Lemon
    {
        public string name { get; set; }
        public string price { get; set; }
        public string color { get; set; }
    }
}
