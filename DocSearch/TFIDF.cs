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
        private List<string> query;
        private List<double> queryIDF;

        public TFIDF(List<string> terms, List<Tuple<string, List<string>>> documents, List<string> question)
        {
            this.terms = terms;
            this.documents = documents.Select(d => d.Item2).ToList();
            this.query = question;
            termsIDF = new List<double>();
            documentsIDF = new List<List<double>>();
            queryIDF = new List<double>();
        }

        private void CalculateVectors()
        {
            foreach (var term in terms)
            {
                double numberOfDocsContainingTerm = documents.Where(d => d.Contains(term)).Count();
                if (numberOfDocsContainingTerm > 0)
                {
                    termsIDF.Add(Math.Log10((double)documents.Count / (numberOfDocsContainingTerm)));
                }
                else
                {
                    termsIDF.Add(0);
                }
            }

            foreach (var document in documents)
            {
                var documentIDF = new List<double>();
                for (var i = 0; i < terms.Count; i++)
                {
                    documentIDF.Add(document.Where(d => d == terms[i]).Count());
                }

                if (documentIDF.Max() > 0)
                {
                    for (var i = 0; i < terms.Count; i++)
                    {
                        if (termsIDF[i] > 0)
                        {
                            documentIDF[i] = (documentIDF[i] / documentIDF.Max()) * termsIDF[i];
                        }
                        else
                        {
                            documentIDF[i] = 0;
                        }
                    }
                }
                
                documentsIDF.Add(documentIDF);
            }

            for (var i = 0; i < terms.Count(); i++)
            {
                queryIDF.Add(query.Where(d => d == terms[i]).Count());
            }

            if (queryIDF.Max() > 0)
            {
                for (var i = 0; i < terms.Count(); i++)
                {
                    if (termsIDF[i] > 0)
                    {
                        queryIDF[i] = (queryIDF[i] / queryIDF.Max()) * termsIDF[i];
                    }
                    else
                    {
                        queryIDF[i] = 0;
                    }
                }
            }
        }

        public List<double> CalculateSimilarity()
        {
            CalculateVectors();
            var similarityVector = new List<double>();
            var queryVectorLength = CalculateVectorLength(queryIDF);

            foreach (var documentIDF in documentsIDF)
            {
                var numerator = 0.0;
                var denominator = queryVectorLength * CalculateVectorLength(documentIDF);
                for (int i = 0; i < queryIDF.Count; i++)
                {
                    numerator += queryIDF[i] * documentIDF[i];
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
                result += vector[i] * vector[i];
            }
            return Math.Sqrt(result);
        }
    }
}
