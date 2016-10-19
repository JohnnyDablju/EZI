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
        private List<string> question;
        private List<double> questionIDF;

        public TFIDF(List<string> terms, List<Tuple<string, List<string>>> documents, List<string> question)
        {
            this.terms = terms;
            this.documents = documents.Select(d => d.Item2).ToList();
            this.question = question;
            termsIDF = new List<double>();
            documentsIDF = new List<List<double>>();
            questionIDF = new List<double>();
        }

        private double FindMax(List<double> list)
        {
            double max = 0;
            foreach(var item in list)
            {
                if(item > max)
                {
                    max = item;
                }
            }
            return max;
        }

        private void CalculateVectors()
        {

            double max = 0;
            double tf;


            foreach (var term in terms)
            {
                double numberOfDocsContainingTerm = documents.Where(d => d.Contains(term)).Count();
                if(numberOfDocsContainingTerm > 0)
                {
                    termsIDF.Add(Math.Log((double)documents.Count / (numberOfDocsContainingTerm)));
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
                    tf = document.Where(d => d == terms[i]).Count();
                    documentIDF.Add(tf);
                }

                max = FindMax(documentIDF);
                if (max > 0)
                {
                    for (var i = 0; i < terms.Count; i++)
                    {
                        if (termsIDF[i] > 0)
                        {
                            documentIDF[i] = (documentIDF[i] / max) * termsIDF[i];
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
                tf = question.Where(d => d == terms[i]).Count();
                questionIDF.Add(tf);
            }

            max = FindMax(questionIDF);
            if (max > 0)
            {
                for (var i = 0; i < terms.Count(); i++)
                {
                    if(termsIDF[i]>0)
                    {
                        questionIDF[i] = (questionIDF[i] / max) * termsIDF[i];
                    }
                    else
                    {
                        questionIDF[i] = 0;
                    }
                }
            }


        }

        public List<double> CalculateSimilarity()
        {
            CalculateVectors();
            var similarityVector = new List<double>();
            var QuestionVectorLength = CalculateVectorLength(questionIDF);

            foreach (var documentIDF in documentsIDF)
            {
                double numerator = 0.0;
                double denominator = QuestionVectorLength * CalculateVectorLength(documentIDF);
                for (int i = 0; i < questionIDF.Count; i++)
                {
                    numerator += questionIDF[i] * documentIDF[i];
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
                //result += Math.Pow(vector[i], 2.0);
                result += vector[i] * vector[i];
            }
            return Math.Sqrt(result);
        }
    }
}
