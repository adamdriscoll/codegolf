using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Models;

namespace CodeGolf.Test
{
    internal class Constants
    {
        public const string Database = "CodeGolfDB";
        public const string Collection = "CodeGolfCollection";
        public const string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        public const string EndpointUri = "https://localhost:8081";
        public static DocumentDbConfig DocumentDbConfig;

        static Constants()
        {
            DocumentDbConfig = new DocumentDbConfig
            {
                //These are the settings for the DocumentDB emulator
                Database = Database,
                DocumentCollection = Collection,
                EndpointUri = EndpointUri,
                PrimaryKey = PrimaryKey
            };
        }
    }
}
