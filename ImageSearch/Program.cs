using System;

namespace ImageSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            var controller = new Controller(
                @"C:\Git\EZI\ImageSearch\data\features\", 
                @"C:\Git\EZI\ImageSearch\data\images\"
            );
            controller.LoadFeatures();
            controller.NormalizeFeatures();
            while (true)
            {
                var query = Console.ReadLine();
                var results = controller.Search(query);
                foreach (var result in results)
                {
                    Console.WriteLine(result);
                }
            }
        }
    }
}
