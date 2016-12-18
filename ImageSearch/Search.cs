using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSearch
{
    class Search
    {
        private enum Aggregations { min, max, ave }
        private string fileName;
        private Feature.FeatureName featureName;
        private class Metric
        {
            public string FileName { get; set; }
            public double Value { get; set; }
        }

        public Search(string query)
        {
            ParseQuery(query);
        }

        private void ParseQuery(string query)
        {
            var splittedQuery = query.Split(' ');
            fileName = splittedQuery[1];
            featureName = (Feature.FeatureName)Enum.Parse(typeof(Feature.FeatureName), splittedQuery[2], true);
        }

        public List<string> GetResults(List<Feature> features, int limit)
        {
            features = features.Where(f => f.Name == featureName).ToList();
            var distances = CalculateDistances(features.Where(f => f.FileName == fileName).First(), features);
            var similarities = CalculateSimilarities(distances);

            var results = new List<string>();
            foreach (var similarity in similarities.OrderByDescending(s => s.Value).Take(limit))
            {
                results.Add(String.Format("{0}\t{1}", similarity.FileName, similarity.Value.ToString()));
            }
            return results;
        }

        private List<Metric> CalculateSimilarities(List<Metric> distances)
        {
            var max = distances.Select(d => d.Value).Max();
            var min = distances.Where(d => d.FileName == fileName).Select(d => d.Value).First();
            var a = 1 / (min - max);
            var b = -(max / (min - max));
            var similarities = new List<Metric>();
            foreach (var distance in distances)
            {
                similarities.Add(new Metric
                {
                    FileName = distance.FileName,
                    Value = a * distance.Value + b
                });
            }
            return similarities;
        }

        private List<Metric> CalculateDistances(Feature selectedFile, List<Feature> features)
        {
            var distances = new List<Metric>();
            foreach (var file in features)
            {
                var distance = 0.0;
                for (int i = 0; i < selectedFile.Ordinates.Count; i++)
                {
                    distance += Math.Abs(selectedFile.Ordinates[i] - file.Ordinates[i]);
                }
                distances.Add(new Metric
                {
                    FileName = file.FileName,
                    Value = distance
                });
            }
            return distances;
        }
    }
}
