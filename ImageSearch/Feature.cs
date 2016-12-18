using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSearch
{
    class Feature
    {
        public enum FeatureName { ColorRGBCov, ColorHCLCov, ColorLabCov, ColorHist64, ColorHist256, TextureLumGabor }
        public FeatureName Name { get; set; }
        public string FileName { get; set; }
        public List<double> Ordinates { get; set; }
    }
}
