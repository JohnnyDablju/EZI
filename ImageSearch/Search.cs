using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageSearch
{
    class Search
    {
        private enum Aggregation { min, max, ave }
        private Aggregation fileAggreagation;
        private List<string> fileNames;
        private Aggregation featureAggreation;
        private List<Feature.FeatureName> featureNames;
        private class Metric
        {
            public string FileName { get; set; }
            public double Value { get; set; }
        }

        public Search(string query)
        {
            fileNames = new List<string>();
            featureNames = new List<Feature.FeatureName>();
            fileAggreagation = Aggregation.max;
            featureAggreation = Aggregation.max;
            ParseQuery(query);
        }

        private void ParseQuery(string query)
        {
            var fileCount = 1;
            var featureCount = 1;
            var splittedQuery = query.Split(' ');
            if (splittedQuery.Length > 3)
            {
                fileCount = int.Parse(splittedQuery[2]);
                fileAggreagation = (Aggregation)Enum.Parse(typeof(Aggregation), splittedQuery[1], true);
                featureCount = int.Parse(splittedQuery[fileCount + 4]);
                featureAggreation = (Aggregation)Enum.Parse(typeof(Aggregation), splittedQuery[fileCount + 3], true);
                for (int i = 0; i < fileCount; i++)
                {
                    fileNames.Add(splittedQuery[i + 3]);
                }
                for (int i = 0; i < featureCount; i++)
                {
                    featureNames.Add((Feature.FeatureName)Enum.Parse(typeof(Feature.FeatureName), splittedQuery[i  + fileCount + 5], true));
                }
            }
            else
            {
                fileNames.Add(splittedQuery[1]);
                featureNames.Add((Feature.FeatureName)Enum.Parse(typeof(Feature.FeatureName), splittedQuery[2], true));
            }          
        }

        public List<string> GetResults(List<Feature> features, int limit)
        {
            var distances = GetDistances(features);
            var aggregatedDistances = CalculateAggregatedDistances(distances);
            var similarities = CalculateSimilarities(aggregatedDistances);
            var results = new List<string>();
            foreach (var similarity in similarities.OrderByDescending(s => s.Value).Take(limit))
            {
                results.Add(String.Format("{0}\t{1}", similarity.FileName, similarity.Value.ToString()));
            }
            return results;
        }

        private List<Metric> GetDistances(List<Feature> features)
        {
            var distances = new List<Metric>();
            foreach (var featureName in featureNames)
            {
                var imageOrdinates = CalculateAggregatedOrdinates(features
                    .Where(f => f.Name == featureName && fileNames.Contains(f.FileName))
                    .Select(f => f.Ordinates)
                    .ToList()
                );
                distances.AddRange(CalculateDistances(
                    imageOrdinates,
                    features
                        .Where(f => f.Name == featureName)
                        .ToList()
                    )
                );
            }
            return distances;
        }

        private List<Metric> CalculateAggregatedDistances(List<Metric> distances)
        {
            var aggregatedDistances = new List<Metric>();
            foreach (var fileName in distances.Select(d => d.FileName).ToList())
            {
                var imageDistances = distances.Where(d => d.FileName == fileName).ToList();
                var distance = 0.0;
                switch (featureAggreation)
                {
                    case Aggregation.ave: distance = imageDistances.Average(d => d.Value); break;
                    case Aggregation.max: distance = imageDistances.Max(d => d.Value); break;
                    case Aggregation.min: distance = imageDistances.Min(d => d.Value); break;
                }
                aggregatedDistances.Add(new Metric
                {
                    FileName = fileName,
                    Value = distance
                });
            }
            return aggregatedDistances;
        }

        private List<double> CalculateAggregatedOrdinates(List<List<double>> ordinates)
        {
            var aggregatedOrdinates = new List<double>();
            for (int i = 0; i < ordinates[0].Count; i++)
            {
                var ordinate = 0.0;
                switch (fileAggreagation)
                {
                    case Aggregation.ave: ordinate = ordinates.Average(o => o[i]); break;
                    case Aggregation.max: ordinate = ordinates.Max(o => o[i]); break;
                    case Aggregation.min: ordinate = ordinates.Min(o => o[i]); break;
                }
                aggregatedOrdinates.Add(ordinate);
            }
            return aggregatedOrdinates;
        }

        private List<Metric> CalculateSimilarities(List<Metric> distances)
        {
            var max = distances.Max(d => d.Value);
            var min = distances.Min(d => d.Value);//Where(d => d.FileName == fileName).Select(d => d.Value).First();
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

        private List<Metric> CalculateDistances(List<double> ordinates, List<Feature> features)
        {
            var distances = new List<Metric>();
            foreach (var file in features)
            {
                var distance = 0.0;
                for (int i = 0; i < ordinates.Count; i++)
                {
                    distance += Math.Abs(ordinates[i] - file.Ordinates[i]);
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
