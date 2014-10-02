using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextMiningConsoleApp.Util
{
    class DocumentTermFrequencyVector : TermFrequencyVector
    {
        private string title;

        public DocumentTermFrequencyVector(string title, ICollection<string> initialTermCollection)
        {
            this.title = title;
            this.AddTerms(initialTermCollection);
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

    }
}
