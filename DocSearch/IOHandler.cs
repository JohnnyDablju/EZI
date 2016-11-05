using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace DocSearch
{
    class IOHandler
    {
        public List<string> LoadTerms(string path, PorterStemmer stemmer)
        {
            var terms = new List<string>();
            using (var sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    var word = sr.ReadLine();
                    // applying stemming
                    word = stemmer.CleanAndStemm(word);
                    if (!String.IsNullOrWhiteSpace(word))
                    {
                        terms.Add(word);
                    }
                }
            }
            return terms;
        }

        public List<Tuple<string, List<string>>> LoadDocuments(string path, PorterStemmer stemmer)
        {
            var documents = new List<Tuple<string, List<string>>>();
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
                            var stemmedWord = stemmer.CleanAndStemm(word);
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
            return documents;
        }
    }
}
