using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LAIR.Collections.Generic;
using LAIR.ResourceAPIs.WordNet;

namespace DocSearch
{
    class QueryExtender
    {
        private const string WordNetPath = @"C:/Git/EZI/dict/";
        private WordNetEngine wordNetEngine;
        private List<WordNetEngine.SynSetRelation> relations; 

        public QueryExtender()
        {
            wordNetEngine = new WordNetEngine(WordNetPath, false);
            relations = new List<WordNetEngine.SynSetRelation> {
                WordNetEngine.SynSetRelation.Hypernym,
                WordNetEngine.SynSetRelation.SimilarTo,
                WordNetEngine.SynSetRelation.Hyponym
            };
        }

        public List<string> Get(string query)
        {
            var words = query.Split(' ').ToList();
            var synsets = new List<Set<SynSet>>();
            var paths = new List<List<SynSet>>();

            foreach (var word in words)
            {
                synsets.Add(wordNetEngine.GetSynSets(word, WordNetEngine.POS.Noun));
            }

            if (words.Count == 1)
            {
                var sets = new List<SynSet>();
                foreach (var s in synsets[0])
                {
                    sets.Add(s);
                }
                paths.Add(sets);
            }
            else
            {
                for (int i = 0; i < synsets.Count; i++)
                {
                    for (int j = i + 1; j < synsets.Count; j++)
                    {
                        foreach (var x in synsets[i])
                        {
                            foreach (var y in synsets[j])
                            {
                                paths.Add(x.GetShortestPathTo(y, relations));
                            }
                        }
                    }
                }
            }

            var resultSet = new List<string>();
            paths = paths.OrderBy(p => p.Count).ToList();
            foreach (var path in paths)
            {
                if (resultSet.Count == 10)
                {
                    break;
                }
                int i = 0;
                int j = path.Count - 1;
                while ((i++ <= path.Count / 2) && (j-- >= i))
                {
                    var result = new List<string>();
                    result.Add(path[i].Words.First());
                    result.Add(path[j].Words.First());
                    result.AddRange(words);
                    resultSet.Add(String.Join(" ", result.OrderBy(x => x).Distinct()));
                    resultSet = resultSet.Distinct().ToList();
                }
            }
            return resultSet;
        }
    }
}
