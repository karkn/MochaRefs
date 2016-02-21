using Lucene.Net.Analysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Integration.Lucene
{
    internal class CaseInsensitiveWhitespaceAnalyzer: Analyzer
    {
        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            var t = default(TokenStream);
            t = new WhitespaceTokenizer(reader);
            t = new LowerCaseFilter(t);
            return t;
        }
    }
}
