using Lucene.Net.Analysis;
using Lucene.Net.Analysis.CJK;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Practices.Unity;
using Mocha.Common.Unity;
using Mocha.Refs.Core.Entities;
using Mocha.Refs.Core.Search;
using Mocha.Refs.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Version = Lucene.Net.Util.Version;

namespace Mocha.Refs.Integration.Lucene
{
    public class SearchEngine: ISearchEngine
    {
        private static readonly string[] StopWords = {
            "a",
            "and",
            "are",
            "as",
            "at",
            "be",
            "but",
            "by",
            "for",
            "if",
            "in",
            "into",
            "is",
            "it",
            "no",
            "not",
            "of",
            "on",
            "or",
            "s",
            "such",
            "t",
            "that",
            "the",
            "their",
            "then",
            "there",
            "these",
            "they",
            "this",
            "to",
            "was",
            "will",
            "with",
            "www",
            "いう",
            "する",
            "人物",
            "さま",
            "すること",
            "ため",
            "もの",
            "おいて",
            "なる",
            "できる",
            "おく",
            "ある",
        };

        private string _indexPath;
        private Analyzer _analyzer;
        //private string _buildDatatimeFilePath;

        public SearchEngine()
        {
            {
                var analyzer = new PerFieldAnalyzerWrapper(new CJKAnalyzer(Version.LUCENE_30, StopWords));
                analyzer.AddAnalyzer("tags", new CaseInsensitiveWhitespaceAnalyzer());
                _analyzer = analyzer;
            }

            _indexPath = Path.Combine(
                AppDomain.CurrentDomain.GetData("DataDirectory").ToString(),
                "Index"
            );
            //_buildDatatimeFilePath = Path.Combine(_indexPath, "lastIndexBuild");

            /// HttpContext.Server.MapPath("~/App_Data/somedata.xml");
            if (!System.IO.Directory.Exists(_indexPath))
            {
                System.IO.Directory.CreateDirectory(_indexPath);
            }
        }

        //private void TouchFile(string path)
        //{
        //    if (File.Exists(path))
        //    {
        //        File.Create(path).Close();
        //    }
        //    else
        //    {
        //        File.SetLastWriteTime(path, DateTime.Now);
        //    }
        //}

        //private void RecordBuildDatetime()
        //{
        //    TouchFile(_buildDatatimeFilePath);
        //}

        //private DateTime GetLastBuildDatetime()
        //{
        //    return File.GetLastWriteTime(_buildDatatimeFilePath);
        //}

        public void BuildIndex()
        {
            //RecordBuildDatetime();

            using (var indexDirectory = FSDirectory.Open(_indexPath))
            using (var writer = new IndexWriter(indexDirectory, _analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            using (var refsContext = new RefsContext())
            {
                var lists = refsContext.RefLists.Include("Refs").Include("Author").Include("TagUses").AsNoTracking().
                    Where(l => l.PublishingStatus == Core.DataTypes.PublishingStatusKind.Publish);
                foreach (var list in lists)
                {
                    writer.AddDocument(CreateDocument(list));
                }
                writer.Optimize();
            }
        }

        //public void RebuildIndex()
        //{
        //    var lastBuild = GetLastBuildDatetime().Ticks;
        //    RecordBuildDatetime();

        //    var idParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "id", _analyzer);
        //    var deleteQuery = new BooleanQuery();

        //    _indexDirectory = FSDirectory.Open(_indexPath);
        //    using (var writer = new IndexWriter(_indexDirectory, _analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED))
        //    {
        //        var container = ContainerFactory.GetContainer();
        //        var refsContext = container.Resolve<IRefsContext>();
        //        var lists = refsContext.RefLists.Include("Refs").
        //            Where(l => l.PublishingStatus == Core.DataTypes.PublishingStatusKind.Publish);
        //        foreach (var list in lists)
        //        {
        //            if (list.UpdatedDate.Ticks > lastBuild)
        //            {
        //                writer.AddDocument(CreateDocument(list));
        //            }
        //            var idQuery = idParser.Parse(list.Id.ToString());
        //            deleteQuery.Add(idQuery, Occur.MUST_NOT);
        //        }

        //        writer.DeleteDocuments(deleteQuery);
        //        writer.Optimize();
        //    }
        //}

        private Document CreateDocument(RefList list)
        {
            var textBuf = new StringBuilder();
            textBuf.AppendLine(list.Title);
            textBuf.AppendLine(list.Comment);
            foreach (var re in list.Refs)
            {
                textBuf.AppendLine(re.Title);
                textBuf.AppendLine(re.Comment);
            }

            var tags = list.TagUses.Select(u => u.Name);
            var tagsText = string.Join(" ", tags);

            var doc = new Document();
            doc.Add(new Field("id", list.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("user", list.Author.UserName, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("tags", tagsText ?? "", Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("text", textBuf.ToString() ?? "", Field.Store.YES, Field.Index.ANALYZED));
            //doc.Add(new Field("modified", list.UpdatedDate.Ticks.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            return doc;
        }

        public IEnumerable<long> SearchRefList(string searchText, int max = 20)
        {
            var queryParser = new SearchQueryParser();
            queryParser.Parse(searchText);

            using (var indexDirectory = FSDirectory.Open(_indexPath))
            using (var reader = IndexReader.Open(indexDirectory, true))
            using (var searcher = new IndexSearcher(reader))
            {
                var query = new BooleanQuery();

                if (queryParser.Texts.Any())
                {
                    var textQueryParser = new QueryParser(Version.LUCENE_30, "text", _analyzer);
                    var textQueries = queryParser.Texts.Select(t => textQueryParser.Parse(t));
                    foreach (var textQuery in textQueries)
                    {
                        query.Add(textQuery, Occur.MUST);
                    }
                }

                if (queryParser.Tags.Any())
                {
                    //var tagQueryParser = new QueryParser(Version.LUCENE_30, "tags", _analyzer);
                    //var tagQueries = queryParser.Tags.Select(t => tagQueryParser.Parse(t));
                    var tagQueries = queryParser.Tags.Select(t => new TermQuery(new Term("tags", t.ToLower())));
                    foreach (var tagQuery in tagQueries)
                    {
                        query.Add(tagQuery, Occur.MUST);
                    }
                }

                if (queryParser.Users.Any())
                {
                    //var userQueryParser = new QueryParser(Version.LUCENE_30, "user", _analyzer);
                    //var userQuery = userQueryParser.Parse(queryParser.Users.Last());
                    var userQuery = new TermQuery(new Term("user", queryParser.Users.Last()));
                    query.Add(userQuery, Occur.MUST);
                }

                var hits = searcher.Search(query, max).ScoreDocs;
                var docs = hits.Select(h => searcher.Doc(h.Doc));
                var ids = docs.Select(d => Convert.ToInt64(d.Get("id")));
                return ids.ToArray();
            }
        }
    }
}
