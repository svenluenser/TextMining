using Lucene.Net.Documents;
using Lucene.Net.Index;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextMiningConsoleApp.Lucene;
using TextMiningConsoleApp.Util;
using TextMiningConsoleApp.Plot;

namespace TextMiningConsoleApp
{
    class Program
    {
        public static string getProjectPath()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public static void AddDocumentsToIndex(string docpath, LuceneIndex index, bool recreate)
        {
            ICollection<Document> docs = LuceneIndex.LoadTextFilesFromDirectory(docpath);
            index.AddToIndex(docs, recreate);
        }

        public static TermFrequencyVector getGlobalTermFrequenciesFromIndex(LuceneIndex index)
        {
            TermFrequencyVector global = new TermFrequencyVector();
            IndexReader indexReader = index.Open();
            int indexSize = indexReader.NumDocs();
            for(int i = 0; i < indexSize; i++)
            {
                ITermFreqVector vct = indexReader.GetTermFreqVector(i, "content");
                if (vct == null) continue;
                string[] terms = vct.GetTerms();
                global.AddTerms(terms);

                for(int t =0; t < terms.Length; t++)
                {
                    global.UpdateTermFrequency(terms[t], (uint) vct.GetTermFrequencies()[t]);
                }
            }
            return global;
        }

        static void Main(string[] args)
        {
            LuceneIndex index = new LuceneIndex(getProjectPath()+"/LuceneIndex", new StreamReader(getProjectPath()+"/data/stopwords_en.txt"));
            //AddDocumentsToIndex(getProjectPath() + "/" + "data/wiki/biology", index, true);
            //AddDocumentsToIndex(getProjectPath() + "/" + "data/wiki/mathematic", index, false);

            TermFrequencyVector globalTerms = getGlobalTermFrequenciesFromIndex(index);

            // get the most significant terms for the hole doc collection
            TermFrequencyVector bestTerms = globalTerms.Clip(100, 1000);

            uint[] data = new uint[bestTerms.GetTerms().Count];
            
            data = bestTerms.toArray();

            Array.Sort(data);
            Array.Reverse(data);
           

            OxiPlot plot = new OxiPlot(".");
            plot.Export(plot.BarSeries(data), "BestTermFrequency");

        }
    }
}
