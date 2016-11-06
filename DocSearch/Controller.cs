using System;
using System.Collections.Generic;
using System.Linq;

namespace DocSearch
{
    class Controller
    {
        private PorterStemmer stemmer;
        private QueryExtender queryExtender;
        private IOHandler ioHandler;
        private TFIDF tfidf;
        private KMeans kMeans;

        private List<string> terms;
        private List<Tuple<string, List<string>>> documents;
        private List<string> query;

        public Controller()
        {
            stemmer = new PorterStemmer();
            queryExtender = new QueryExtender();
            ioHandler = new IOHandler();        
        }

        public string GetTermsPreview()
        {
            return String.Join("\n", terms);
        }

        public string GetDocumentsPreview()
        {
            var preview = "";
            for (var i = 0; i < documents.Count; i++)
            {
                preview =
                    preview
                    + documents[i].Item1 + "\n\n"
                    + String.Join("\n", documents[i].Item2)
                    + "\n\n-----------\n\n";
            }
            return preview;
        }

        public string GetQueryExtensions(string query)
        {
            var result = "";
            foreach (var extension in queryExtender.Get(query))
            {
                result += extension + "\n\n";
            }
            return result;
        }

        public string GetDocumentsSimilarity()
        {
            tfidf = new TFIDF(terms, documents, query);
            var similarity = tfidf.GetDocumentQuery();

            var rank = new List<Tuple<string, double>>();
            for (int i = 0; i < documents.Count; i++)
            {
                rank.Add(new Tuple<string, double>(documents[i].Item1, similarity[i]));
            }
            rank = rank.OrderByDescending(doc => doc.Item2).ToList();

            var result = "";
            foreach (var record in rank)
            {
                result += record.Item2 + "\n" + record.Item1 + "\n\n";
            }
            return result;
        }

        public string GetGroups(int seed, int iterations)
        {
            tfidf = new TFIDF(terms, documents);
            var similarityMatrix = tfidf.GetDocumentTerm();
            kMeans = new KMeans(similarityMatrix);
            var groups = kMeans.GetGroups(seed, iterations);
            var result = "";
            for (int i = 0; i < seed; i++)
            {
                result += "GROUP #" + (i + 1).ToString() + "\n";
                foreach (var document in groups[i])
                {
                    result += documents[document].Item1 + "\n";
                }
                result += "\n\n";
            }
            return result;
        }

        public void LoadQuery(string query)
        {
            this.query = new List<string>();
            this.query = query.Split(' ').Select(q => stemmer.CleanAndStemm(q)).ToList();
        }

        public void LoadTerms(string path)
        {
            terms = ioHandler.LoadTerms(path, stemmer);
        }

        public void LoadDocuments(string path)
        {
            documents = ioHandler.LoadDocuments(path, stemmer);
        }
    }
}
