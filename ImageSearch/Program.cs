using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
