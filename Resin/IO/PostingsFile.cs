using System;
using System.Collections.Generic;

namespace Resin.IO
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class PostingsFile
    {
        private readonly Dictionary<string, object> _postings;
        /// <summary>
        /// docids/term frequency
        /// </summary>
        public Dictionary<string, object> Postings
        {
            get { return _postings; }
        }

        private readonly string _field;
        public string Field { get { return _field; } }
        private readonly string _token;
        public string Token { get { return _token; } }

        public PostingsFile(string field, string token)
        {
            _field = field;
            _token = token;
            _postings = new Dictionary<string, object>();
        }

        public int NumDocs()
        {
            return Postings.Count;
        }
    }
}