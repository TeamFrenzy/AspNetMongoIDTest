using System.Collections.Generic;
using MongoDB.Bson;

namespace AspNetMongoIDTest.Models
{
    public class ExplorerDbViewModel
    {
        public string Database { get; set; }
        public string Collection { get; set; }
        public BsonDocument Document { get; set; }
        public Dictionary<string, List<string>> DatabasesAndCollections { get; set; }
        public int Index { get; set; }
        public long CollectionCount { get; set; }
    }
}
