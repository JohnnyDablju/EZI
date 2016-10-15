using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocSearch
{
    class TFIDF
    {
        private List<List<string>> documents;
        private List<List<double>> documentsIDF;
        private List<string> terms;
        private List<double> termsIDF;

        public TFIDF(List<string> terms, List<Tuple<string, List<string>>> documents)
        {
            this.terms = terms;
            this.documents = documents.Select(d => d.Item2).ToList();
            termsIDF = new List<double>();
            documentsIDF = new List<List<double>>();
        }

        private void CalculateVectors()
        {
            foreach (var term in terms)
            {
                double numberOfDocsContainingTerm = documents.Where(d => d.Contains(term)).Count();
                termsIDF.Add(Math.Log((double)documents.Count / ((double)1 + numberOfDocsContainingTerm)));
            }

            foreach (var document in documents)
            {
                var documentIDF = new List<double>();
                for (var i = 0; i < terms.Count; i++)
                {
                    double tf = document.Where(d => d == terms[i]).Count();
                    documentIDF.Add(tf * termsIDF[i]);
                }
                documentsIDF.Add(documentIDF);
            }
        }

        public List<double> CalculateSimilarity()
        {
            CalculateVectors();
            var similarityVector = new List<double>();
            foreach (var documentIDF in documentsIDF)
            {
                double numerator = 0.0;
                double denominator = CalculateVectorLength(termsIDF) * CalculateVectorLength(documentIDF);
                for (int i = 0; i < termsIDF.Count; i++)
                {
                    numerator += termsIDF[i] * documentIDF[i];
                }
                var similarity = 0.0;
                if (denominator != 0.0)
                {
                    similarity = numerator / denominator;
                }
                similarityVector.Add(similarity);
            }
            return similarityVector;
        }

        private double CalculateVectorLength(List<double> vector)
        {
            double result = 0.0;
            for (int i = 0; i < vector.Count; i++)
            {
                result += Math.Pow(vector[i], 2.0);
            }
            return Math.Sqrt(result);
        }
    }
}
