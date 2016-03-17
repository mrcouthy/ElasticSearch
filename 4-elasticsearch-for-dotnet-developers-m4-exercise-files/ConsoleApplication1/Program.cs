using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace ConsoleApplication1
{
    class Program
    {
        public static Uri node;
        public static ConnectionSettings settings;
        public static ElasticClient client;

        static void Main(string[] args)
        {

            node = new Uri("http://localhost:9200");
            settings = new ConnectionSettings(node, defaultIndex: "my_blog");
            client = new ElasticClient(settings);

            var indexSettings = new IndexSettings();
            indexSettings.NumberOfReplicas = 1;
            indexSettings.NumberOfShards = 1;

            //client.CreateIndex(c => c
            //    .Index("my_blog")
            //    .InitializeUsing(indexSettings)
            //    .AddMapping<Post>(m => m.MapFromAttributes()));

            // Uncomment these methods to perform operations
            // InsertData();
            PerformTermQuery();
            //PerformMatchPhrase();
            //PerformFilter();
            
        }

        public static void InsertData()
        {
            var newBlogPost = new Post
            {
                UserId = 1,
                PostDate = DateTime.Now,
                PostText = "This is another blog post."
            };

            var newBlogPost2 = new Post
            {
                UserId = 2,
                PostDate = DateTime.Now,
                PostText = "This is a third blog post."
            };

            var newBlogPost3 = new Post
            {
                UserId = 2,
                PostDate = DateTime.Now.AddDays(5),
                PostText = "This is a blog post from the future."
            };

            client.Index(newBlogPost);
            client.Index(newBlogPost2);
            client.Index(newBlogPost3);
        }

        public static void PerformTermQuery()
        {
            var result =
            client.Search<Post>(s => s
                .Query(p => p.Term(q => q.PostText, "blog")));
        }

        public static void PerformMatchPhrase()
        {
            var result = client.Search<Post>(s => s
                .Query(q => q.MatchPhrase(m => m.OnField("postText").Query("this is a third blog post"))));
        }

        public static void PerformFilter()
        {
            var result = client.Search<Post>(s => s
                .Query(q => q.Term(p => p.PostText, "blog"))
                .Filter(f => f.Range(r => r.OnField("postDate").Greater("2014-10-29"))));
        }
    }
}
