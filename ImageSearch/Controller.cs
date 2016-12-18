using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageSearch
{
    class Controller
    {
        private List<Feature> features;
        private string featuresPath;
        private string imagesPath;

        public Controller(string featuresPath, string imagesPath)
        {
            this.featuresPath = featuresPath;
            this.imagesPath = imagesPath;
        }

        public void LoadFeatures()
        {
            features = new List<Feature>();
            foreach (var feature in Enum.GetValues(typeof(Feature.FeatureName)))
            {
                using (var reader = new StreamReader(featuresPath + Enum.GetName(typeof(Feature.FeatureName), feature) + ".dat"))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine().Split(' ');
                        var list = new List<double>();
                        for (var i = 1; i < line.Length - 1; i++)
                        {
                            list.Add(double.Parse(line[i], System.Globalization.CultureInfo.InvariantCulture));
                        }
                        features.Add(new Feature
                        {
                            Name = (Feature.FeatureName)feature,
                            FileName = line[0],
                            Ordinates = list
                        });
                    }
                }
            }
        }

        public void NormalizeFeatures()
        {
            foreach (var featureName in Enum.GetValues(typeof(Feature.FeatureName)))
            {
                var feature = features.Where(f => f.Name == (Feature.FeatureName)featureName).ToList(); 
                for (int i = 0; i < feature[0].Ordinates.Count; i++)
                {
                    var average = feature.Average(f => f.Ordinates[i]);
                    var standardDeviation = Math.Sqrt(feature.Sum(f => Math.Pow(average - f.Ordinates[i], 2.0)) / feature.Count);
                    foreach (var file in feature)
                    {
                        file.Ordinates[i] = (file.Ordinates[i] - average) / standardDeviation;
                    }
                }
            }
        }

        public List<string> Search(string query)
        {
            var search = new Search(query);
            return search.GetResults(features, 12);
        }
    }
}
