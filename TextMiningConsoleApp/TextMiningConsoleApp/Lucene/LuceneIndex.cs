using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;

using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;


namespace TextMiningConsoleApp.Lucene
{
    class LuceneIndex
    {
        private string path;
        private TextReader stopwords;


        public LuceneIndex(string path, TextReader stopwords)
        {
            this.path = path;
            this.stopwords = stopwords;
        }


        public IndexReader Open()
        {
             return IndexReader.Open(FSDirectory.Open(path), true);
        }

        public void AddToIndex(ICollection<Document> docs, bool recreate)
        {
            this.create(docs, recreate);
        }



        private void create(ICollection<Document>  docs, bool recreate)
        {
            // Erstelle Index im aktuellen Arbeitsverzeichnis
            Directory directory = FSDirectory.Open(path);
            
            // Load stopwords for the analyzer or use the default one
            Analyzer analyzer;
            if(stopwords != null)
            {
                // load stopwords from file
                analyzer = new StandardAnalyzer(Version.LUCENE_30, stopwords);
            }
            else
            {
                // default analyzer
                analyzer = new StandardAnalyzer(Version.LUCENE_30);
            }   
              
            var writer = new IndexWriter(directory, analyzer, recreate, IndexWriter.MaxFieldLength.LIMITED);

            loadToIndex(writer, docs);

            writer.Optimize();
            writer.Dispose();
           
        }

  
        private void loadToIndex(IndexWriter writer, ICollection<Document> docs)
        {
            // all test data for author aristotle
            foreach (Document d in docs)
            {
                writer.AddDocument(d);
            }
        }


        public static ICollection<Document> LoadTextFilesFromDirectory(string path)
        {
            LinkedList<Document> list = new LinkedList<Document>();
            foreach (string s in System.IO.Directory.GetFiles(path))
            {
                Console.WriteLine("reading file :" + s);

                string text = readFromFile(s);

                Document doc = new Document();
                doc.Add(new Field("filepath", s, Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("content", text, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES));

                list.AddLast(doc);
            }

            return list;

        }

        private static string readFromFile(string file)
        {
            TextReader reader = new StreamReader(file);
            string line = "";
            string text = "";

            while ((line = reader.ReadLine()) != null)
                text += line;

            return text;
        }



        public TextReader Stopwords
        {
              get { return stopwords; }
              set { stopwords = value; }
        }

    }
}
