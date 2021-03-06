﻿using Sir.VectorSpace;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Sir.Search
{
    public class DebugIndexSession : IDisposable, ILogger
    {
        private readonly IndexSession _indexSession;
        private readonly ConcurrentDictionary<long, ConcurrentBag<IVector>> _debugWords;

        public DebugIndexSession(IndexSession indexSession)
        {
            _indexSession = indexSession;
            _debugWords = new ConcurrentDictionary<long, ConcurrentBag<IVector>>();
        }

        public void Put(long docId, long keyId, string value)
        {
            var tokens = _indexSession.Model.Tokenize(value);

            foreach (var vector in tokens)
            {
                _indexSession.Put(docId, keyId, vector);

                _debugWords.GetOrAdd(keyId, new ConcurrentBag<IVector>()).Add(vector);
            }
        }

        public IndexInfo GetIndexInfo()
        {
            return _indexSession.GetIndexInfo();
        }

        private void Debug()
        {
            var debugOutput = new StringBuilder();

            foreach (var column in _indexSession.Index)
            {
                var debugWords = _debugWords[column.Key];
                var wordSet = new HashSet<IVector>();

                foreach (var term in debugWords)
                {
                    if (wordSet.Add(term))
                    {
                        var hit = PathFinder.ClosestMatch(column.Value, term, _indexSession.Model);

                        if (hit != null && hit.Score >= _indexSession.Model.IdenticalAngle)
                        {
                            continue;
                        }

                        throw new Exception($"could not find {term}");
                    }
                }

                debugOutput.AppendLine($"{column.Key}: {wordSet.Count} words");

                foreach (var term in wordSet)
                {
                    debugOutput.AppendLine(term.ToString());
                }
            }

            this.Log(debugOutput);
        }

        public void Dispose()
        {
            _indexSession.Dispose();

            Debug();
        }
    }
}