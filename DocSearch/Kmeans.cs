using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocSearch
{
    class KMeans
    {
        private int documentsCount;
        private int termsCount;
        private double[,] documentTermIdf;
        private double[,] similarityMatrix;
        private List<int>[] groups;
        private double[,] centroids;

        public KMeans(List<List<double>> documentTermIdf)
        {
            documentsCount = documentTermIdf.Count;
            termsCount = documentTermIdf[0].Count;
            this.documentTermIdf = new double[documentsCount, termsCount];
            for (int d = 0; d < documentsCount; d++)
            {
                for (int t = 0; t < termsCount; t++)
                {
                    this.documentTermIdf[d, t] = documentTermIdf[d][t];
                }
            }
            similarityMatrix = new double[documentsCount, documentsCount];
        }

        public List<int>[] GetGroups(int groupsCount, int iterations)
        {
            groups = new List<int>[groupsCount];
            var previousGroups = new List<int>[groupsCount];
            for (int g = 0; g < groupsCount; g++) previousGroups[g] = new List<int>();
            centroids = new double[groupsCount, termsCount];

            // get similarity matrix from tf-idf representation
            CalculateSimilarityMatrix();
            // create groups with one random document per each, exclude assigned documents
            var availableDocuments = Group(true, groupsCount);
            // iterate
            for (int i = 0; i < iterations; i++)
            {
                foreach (var document in availableDocuments)
                {
                    var maxSimilarity = 0.0;
                    var chosenGroup = -1;
                    // for each left document look for the group which is the most simmilar
                    for (int g = 0; g < groupsCount; g++)
                    {
                        var similarity = similarityMatrix[document, groups[g][0]];
                        if (similarity >= maxSimilarity)
                        {
                            maxSimilarity = similarity;
                            chosenGroup = g;
                        }
                    }
                    groups[chosenGroup].Add(document);
                }
                // check if groups have changed
                if (CompareGroups(groupsCount, groups, previousGroups))
                {
                    break;
                }
                previousGroups = groups;
                // create new groups with one medoid document per each, exclude assigned documents
                availableDocuments = Group(false, groupsCount);
            }
            return groups;
        }

        private void CalculateSimilarityMatrix()
        {
            for (int d1 = 0; d1 < documentsCount; d1++)
            {
                for (int d2 = d1 + 1; d2 < documentsCount; d2++)
                {
                    double numerator = 0.0;
                    double dd1 = 0.0;
                    double dd2 = 0.0;
                    for (int t = 0; t < termsCount; t++)
                    {
                        var dt1 = documentTermIdf[d1, t];
                        var dt2 = documentTermIdf[d2, t];
                        numerator += dt1 * dt2;
                        dd1 += Math.Pow(dt1, 2.0);
                        dd2 += Math.Pow(dt2, 2.0);
                    }
                    double denominator = 0.0;
                    dd1 = Math.Sqrt(dd1);
                    dd2 = Math.Sqrt(dd2);
                    denominator = Math.Sqrt(dd1 * dd2);
                    if (denominator != 0.0)
                    {
                        similarityMatrix[d1, d2] = numerator / denominator;
                    }
                    else
                    {
                        similarityMatrix[d1, d2] = 0.0;
                    }
                }
            }
        }

        private bool CompareGroups(int groupsCount, List<int>[] g1, List<int>[] g2)
        { 
            for (int g = 0; g < groupsCount; g++)
            {
                if (g1[g].Count != g2[g].Count)
                {
                    return false;
                }
                for (int i = 0; i < g1[g].Count; i++)
                {
                    if (g1[g][i] != g2[g][i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void CalculateCentroids(int groupsCount)
        {
            for (int t = 0; t < termsCount; t++)
            {
                for (int g = 0; g < groupsCount; g++)
                {
                    double s = 0.0;
                    int documentsInGroupCount = groups[g].Count;
                    for (int d = 0; d < documentsInGroupCount; d++)
                    {
                        s += documentTermIdf[d, t];
                    }
                    centroids[g, t] = s / (double)documentsInGroupCount;
                }
            }
        }

        private List<int> Group(bool randomly, int groupsCount)
        {
            var availableDocuments = GetDocumentsList();
            var random = new Random();
            for (int g = 0; g < groupsCount; g++)
            {
                int index;
                if (randomly)
                {
                    index = random.Next(availableDocuments.Count);
                }
                else
                {
                    index = GetMedoid(availableDocuments, g);
                }
                groups[g] = new List<int>();
                groups[g].Add(availableDocuments[index]);
                availableDocuments.RemoveAt(index);
            }
            return availableDocuments;
        }

        private int GetMedoid(List<int> availableDocuments, int groupIndex)
        {
            int d = -1;
            double similarity = 1000.0;
            for (int i = 0; i < availableDocuments.Count; i++)
            {
                var s = 0.0;
                for (int t = 0; t < termsCount; t++)
                {
                    s += Math.Pow(centroids[groupIndex, t] - documentTermIdf[availableDocuments[i], t], 2.0);
                }
                if (s < similarity)
                {
                    similarity = s;
                    d = i;
                }
            }
            return d;
        }

        private List<int> GetDocumentsList()
        {
            var list = new List<int>();
            for (int i = 0; i < documentsCount; i++)
            {
                list.Add(i);
            }
            return list;
        }
    }
}
