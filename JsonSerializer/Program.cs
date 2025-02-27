namespace JsonSerializer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            Apple apple = new Apple("red", 10);
            LemonJsonConverter lemonJsonConverter = new LemonJsonConverter();
            Dictionary<Type, ICustomJsonConverter> converters = new Dictionary<Type, ICustomJsonConverter>();
            converters.Add(typeof(Lemon), lemonJsonConverter);

            JsonSerializerSettings settings = new JsonSerializerSettings(10, converters);

            JsonSerializer serializer = new JsonSerializer(settings, new JsonSerializerFormatter());
            var serializedString = serializer.Serialize(apple);
            Console.WriteLine(serializedString);
            Console.WriteLine(serializer.Serialize("hi"));
            Console.WriteLine(serializer.Serialize(1));
            Console.WriteLine(serializer.Serialize(false));
            Console.WriteLine(serializer.Serialize('a'));
            Console.WriteLine(serializer.Serialize(1.1234));
            Console.WriteLine(serializer.Serialize(-234));

            List<Apple> apples = new List<Apple>
            {
                new Apple("white", 20),
                new Apple("red", 10),
                new Apple("black", 40)
            };

            Console.WriteLine(serializer.Serialize(apples));

            var applesList = new List<Apple>
                    {
                        new Apple("innerList", 100),
                        new Apple("innerList2", 200)
                    };

            FruitBasket basket = new FruitBasket
            {
                FirstApple = new Apple("red", 5),
                SecondApple = null,
                InnerBasket = new FruitBasket
                {
                    FirstApple = new Apple("innerRed", 10),
                    SecondApple = new Apple("innerBlack", 200),
                    InnerBasket = null,
                    ApplesList = applesList,
                    basektName = "innerBasket",
                    basketCount = -20
                },
                ApplesList = null,
                basketCount = 12,
                basektName = "OuterBasket"
            };

            Console.WriteLine(serializer.Serialize(basket));

            List<int> ints = new List<int>() { 1, 2, 34 };
            List<string> strings = new List<string>() { "abv", "asdf" };
            Dictionary<int, int> dict = new Dictionary<int, int>
            {
                {123, 123 },
                {879, 11232 }
            };
            Dictionary<string, int> dict2 = new Dictionary<string, int>
            {
                {"123", 123 },
                {"879", 11232 }
            };

            List<int> emptyList = new List<int>();
            Object obj = new object();

            Console.WriteLine(serializer.Serialize(ints));
            Console.WriteLine(serializer.Serialize(strings));
            Console.WriteLine(serializer.Serialize(dict));
            Console.WriteLine(serializer.Serialize(dict2));
            Console.WriteLine(serializer.Serialize(emptyList));
            Console.WriteLine(serializer.Serialize(obj));

            Lemon l1 = new Lemon();
            l1.price = "20";
            l1.name = "l1";
            l1.color = "yellow";

            Console.WriteLine(serializer.Serialize(l1));
        }
    }
}
