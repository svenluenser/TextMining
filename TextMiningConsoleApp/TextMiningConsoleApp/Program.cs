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
using TextMiningConsoleApp.PrincipleComponentAnalysis;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;

namespace TextMiningConsoleApp
{
    class Program
    {
        public static string getProjectPath()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public static void AddDocumentsToIndex(string docpath, string label, LuceneIndex index, bool recreate)
        {
            ICollection<Document> docs = LuceneIndex.LoadTextFilesFromDirectory(docpath, label);
            index.AddToIndex(docs, recreate);
        }

        /// <summary>
        /// Get the global term-frequencies collection from the document index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>global term-frequencies collection</returns>
        public static FeatureVector<string> getGlobalTermFrequenciesFromIndex(LuceneIndex index)
        {
            FeatureVector<string> global = new FeatureVector<string>();
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
                    global[terms[t]] +=  vct.GetTermFrequencies()[t];
                }
            }
            return global;
        }

        public static List<DocTermFeatures> loadDocumentFeaturesFromIndex(LuceneIndex index, ICollection<string> features, string label, int max)
        {
            List<DocTermFeatures> docTermFrequencies = new List<DocTermFeatures>();
            IndexReader indexReader = index.Open();
            int indexSize = indexReader.NumDocs();
            int num = 0;
            for (int i = 0; i < indexSize && num <= max; i++)
            {
                // filter by label
                if (indexReader.Document(i).Get("label").ToLower().Equals(label.ToLower()))
                {
                    ITermFreqVector vct = indexReader.GetTermFreqVector(i, "content");
                    if (vct == null) continue;
                    
                    string[] terms = vct.GetTerms();
                    var doc = new DocTermFeatures(indexReader.Document(i).Get("filepath"), indexReader.Document(i).Get("label"), features);

                    for (int a = 0; a < terms.Length; a++)
                    {
                        doc[terms[a]] += vct.GetTermFrequencies()[a];
                    }
                    docTermFrequencies.Add(doc);
                    num++;
                }
            }

            return docTermFrequencies;
        }

        public static List<DocTermFeatures> getKeyTermFrequenciesFromIndex(LuceneIndex index, ICollection<string> features)
        {
            List<DocTermFeatures> docTermFrequencies = new List<DocTermFeatures>();
            IndexReader indexReader = index.Open();
            int indexSize = indexReader.NumDocs();
            for(int i=0; i < indexSize; i++)
            {
                ITermFreqVector vct = indexReader.GetTermFreqVector(i, "content");
                if (vct == null) continue;
               
                string[] terms = vct.GetTerms();
                var doc = new DocTermFeatures(indexReader.Document(i).Get("filepath"), indexReader.Document(i).Get("label"), features);

                for(int a = 0; a < terms.Length; a++)
                {
                    // if term exists increment
                    string t = terms[a];
                    if(doc.GetKeysOfFeatures().Contains(t))
                        doc[t] += vct.GetTermFrequencies()[a];
                }
                docTermFrequencies.Add(doc);
               
            }

            return docTermFrequencies;
        }

        static void Main(string[] args)
        {//MatheSpd
            LuceneIndex index = new LuceneIndex(getProjectPath()+"/AuthorLuceneIndex", new StreamReader(getProjectPath()+"/data/stopwords_en.txt"));
            //AddDocumentsToIndex(getProjectPath() + "/" + "data/mathematik.de", "mathe", index, true);
            //AddDocumentsToIndex(getProjectPath() + "/" + "data/spd.de", "spd", index, false);


            //AddDocumentsToIndex(getProjectPath() + "/" + "data/aristotele", "arsitotele", index, true);
            //AddDocumentsToIndex(getProjectPath() + "/" + "data/shakespeare", "shakespeare", index, false);

            Console.WriteLine("Calc Global terms");
           FeatureVector<string> globalTerms = getGlobalTermFrequenciesFromIndex(index);
            Console.WriteLine("done");

            Console.WriteLine("Calc best terms");
            
            Console.WriteLine(globalTerms.ToString());
            Console.ReadLine();
            OxiPlot plot = new OxiPlot(".");

            globalTerms.SortByValue(false);
            double[] d = globalTerms.toArray();

            int lowClip = 100;
            int highClip = 2000;

            var plotZipf = plot.LineSeries(d, lowClip, highClip);
            plot.Export(plotZipf, "GlobalTermFrequency");
            // get the most significant terms for the hole document collection
            globalTerms.ClipByValue(lowClip, highClip);


            // get the features (bestterms) for every document (its the fingerprint of the document)
            List<DocTermFeatures> list = getKeyTermFrequenciesFromIndex(index, globalTerms.GetKeysOfFeatures());

            //List<DocTermFeatures> sl = list.Take(6).ToList();
            //plot.Export(plot.ColumnSeries(sl), "Hist");
            //foreach (DocTermFeatures dl in sl)
            //{
            //    Console.Write(" " + dl.ToString());
            //    Console.ReadLine();
            //}

            // build FeatureMatrix
            FeatureMatrix fm = new FeatureMatrix(list);
            Console.WriteLine(fm.ToString());

            //KernelPCA pca = new KernelPCA(fm.Matrix);
            PCA pca = new PCA(fm.Matrix);
            double[,] pca_result = pca.Compute(2);
         
            Console.WriteLine("done");
    
            Console.WriteLine("Plott");
            globalTerms.SortByValue(false);
             d = globalTerms.toArray();
            plot.Export(plot.LineSeries(d), "BestTermFrequency");

            var plotmodel = plot.ScatterPlot2d(pca_result);
            plot.Export(plotmodel, "PCA-Result");


            Matrix<double> m = Matrix<double>.Build.DenseOfArray(pca_result).Transpose();
            ICA ica = new ICA(m.ToArray(), pca.getPrincipalEigenValues(2));

            double[,] ic = ica.Compute();
            Console.ReadLine();
            for (int i = 0; i < ic.GetLength(0); i++)
            {
                for (int j = 0; j < ic.GetLength(1); j++)
                    Console.Write(ic[i, j].ToString("#00.00") + " ");
                Console.WriteLine();
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(plotmodel));
            Application.Run(new Form1(plotZipf));
            
            
        }
    }
}
