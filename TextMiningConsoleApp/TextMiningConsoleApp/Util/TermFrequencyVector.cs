using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextMiningConsoleApp.Util
{
    class TermFrequencyVector
    {
        private Dictionary<string, uint> termfrequency;

        public TermFrequencyVector()
        {
            termfrequency = new Dictionary<string, uint>();
        }

        public void AddTerms(ICollection<string> terms)
        {
            foreach(string term in terms)
                addTerm(term, 0);
        }

        public uint[] toArray()
        {
            var result = from pair in termfrequency
                         orderby pair.Key
                         select pair.Value;

            return result.ToArray();
        }

        private void addTerm(string term, uint value)
        {
            if (!termfrequency.ContainsKey(term))
                termfrequency.Add(term, value);
        }

        public void UpdateTermFrequency(string term, uint value)
        {
            if (termfrequency.ContainsKey(term))
                termfrequency[term] += value;
        }

        public TermFrequencyVector Clip (uint low, uint high)
        {
            var result = from pair in termfrequency
                    where pair.Value < high && pair.Value > low
                    orderby pair.Key
                    select pair;

            TermFrequencyVector vector = new TermFrequencyVector();
            foreach(KeyValuePair<string, uint> pair in result)
                vector.addTerm(pair.Key, pair.Value);

            return vector;
        }

        public ICollection<string> GetTerms()
        {
            return termfrequency.Keys;
        }

        public override string ToString()
        {
            string result = "";
            foreach(string key in termfrequency.Keys)
            {
                result += key + " : " + termfrequency[key] + Environment.NewLine;
            }

            return result;
        }





    }
}
