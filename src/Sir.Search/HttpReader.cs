﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Sir.Search
{
    /// <summary>
    /// Query a collection.
    /// </summary>
    public class HttpReader : IHttpReader
    {
        public string ContentType => "application/json";

        private readonly SessionFactory _sessionFactory;
        private readonly HttpQueryParser _httpQueryParser;
        private readonly IConfigurationProvider _config;

        public HttpReader(
            SessionFactory sessionFactory, 
            HttpQueryParser httpQueryParser,
            IConfigurationProvider config)
        {
            _sessionFactory = sessionFactory;
            _httpQueryParser = httpQueryParser;
            _config = config;
        }

        public void Dispose()
        {
        }

        public ResponseModel Read(HttpRequest request, IStringModel model)
        {
            var timer = Stopwatch.StartNew();
            var collectionName = request.Query.ContainsKey("collection") ?
                request.Query["collection"].ToString() :
                null;

            var take = 100;
            var skip = 0;

            if (request.Query.ContainsKey("take"))
                take = int.Parse(request.Query["take"]);

            if (request.Query.ContainsKey("skip"))
                skip = int.Parse(request.Query["skip"]);

            var query = _httpQueryParser.Parse(request);

            if (query == null)
            {
                return new ResponseModel { MediaType = "application/json", Total = 0 };
            }

            ReadResult result = null;

            using (var readSession = _sessionFactory.CreateReadSession())
            {
                if (request.Query.ContainsKey("id") && request.Query.ContainsKey("collection"))
                {
                    var collectionId = request.Query["collection"].ToString().ToHash();
                    var ids = request.Query["id"].ToDictionary(s => (collectionId, long.Parse(s)), x => (double)1);
                    var docs = readSession.ReadDocs(ids);

                    result = new ReadResult { Docs = docs, Total = docs.Count };
                }
                else
                {
                    result = readSession.Read(query, skip, take);
                }
            }

            string newCollectionName = null;

            if (request.Query.ContainsKey("target"))
            {
                newCollectionName = request.Query["target"].ToString();

                if (string.IsNullOrWhiteSpace(newCollectionName))
                {
                    newCollectionName = Guid.NewGuid().ToString();
                }

                _sessionFactory.WriteConcurrent(new Job(newCollectionName.ToHash(), result.Docs, model));
            }

            using (var mem = new MemoryStream())
            {
                Serialize(result.Docs, mem);

                return new ResponseModel
                {
                    MediaType = "application/json",
                    Documents = result.Docs,
                    Total = result.Total,
                    Body = mem.ToArray(),
                    Target = newCollectionName
                };
            }
        }

        private void Serialize(IEnumerable<IDictionary<string, object>> docs, Stream stream)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer ser = new JsonSerializer();
                ser.Serialize(jsonWriter, docs);
                jsonWriter.Flush();
            }
        }
    }
}