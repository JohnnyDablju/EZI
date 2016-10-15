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
        List<string> terms;
        List<Tuple<string, List<string>>> documents;

        public Controller()
        {
            stemmer = new PorterStemmer();
            terms = new List<string>();
            documents = new List<Tuple<string, List<string>>>();
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

        public void LoadTerms(string path)
        {
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

        public void LoadDocuments(string path)
        {
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

        public void Analyse()
        {

        }
    }
}
