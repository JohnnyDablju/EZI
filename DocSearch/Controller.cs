using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DocSearch
{
    class Controller
    {
        PorterStemmer stemmer;
        QueryExtender queryExtender;
        TFIDF tfidf;
        List<string> terms;
        List<Tuple<string, List<string>>> documents;
        List<string> query;

        public Controller()
        {
            stemmer = new PorterStemmer();
            queryExtender = new QueryExtender();        
        }

        private string CleanAndStemm(string word)
        {
            word = word.ToLower().Trim(new Char[] { ',', '.', ';', ':', '(', ')', '|', '&', '!', '-' });
            word = stemmer.StemWord(word);
            return word;
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
                result += extension + "\n";
            }
            return result;
        }

        public void LoadTerms(string path)
        {
            terms = new List<string>();
            using (var sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    var word = sr.ReadLine();
                    // applying stemming
                    word = CleanAndStemm(word);
                    if (!String.IsNullOrWhiteSpace(word))
                    {
                        terms.Add(word);
                    }
                }
            }
        }

        public void LoadQuery(string query)
        {
            this.query = new List<string>();
            this.query = query.Split(' ').Select(q => CleanAndStemm(q)).ToList();
        }

        public void LoadDocuments(string path)
        {
            documents = new List<Tuple<string, List<string>>>();
            using (var sr = new StreamReader(path))
            {
                var document = new List<string>();
                var title = "";
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (line != "")
                    {
                        if (document.Count == 0)
                        {
                            title = line;
                        }
                        var words = line.Split(' ').ToList();
                        // applying stemming
                        foreach (var word in words)
                        {
                            var stemmedWord = CleanAndStemm(word);
                            if (!String.IsNullOrWhiteSpace(stemmedWord))
                            {
                                document.Add(stemmedWord);
                            }
                        }
                    }
                    else
                    {
                        documents.Add(new Tuple<string, List<string>>(title, document));
                        document = new List<string>();
                    }
                }
            }
        }

        public string GetDocumentsSimilarity()
        {
            var rank = CalculateDocumentsSimilarity();
            var result = "";
            foreach (var record in rank)
            {
                result += record.Item2 + "\n" + record.Item1 + "\n\n";
            }
            return result;
        }

        private List<Tuple<string, double>> CalculateDocumentsSimilarity()
        {
            tfidf = new TFIDF(terms, documents, query);
            var similarity = tfidf.CalculateSimilarity();

            var documentsRank = new List<Tuple<string, double>>();
            for (int i = 0; i < documents.Count; i++)
            {
                documentsRank.Add(new Tuple<string, double>(documents[i].Item1, similarity[i]));
            }
            return documentsRank.OrderByDescending(doc => doc.Item2).ToList();
        }
    }
}
