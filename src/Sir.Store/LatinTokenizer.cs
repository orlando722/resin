﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Sir.Store
{
    public class LatinTokenizer : ITokenizer
    {
        private static char[] _phraseDelimiters = new char[] {
                            '.', ',', '?', '!',
                            ':', ';', '\\', '/',
                            '\n', '\r', '\t',
                            '(', ')', '[', ']',
                            '"', '`', '´', '-',
                            '=', '&', '\''
                            };

        private static char[] _delims = new char[] {
                            '.', ',', '?', '!',
                            ':', ';', '\\', '/',
                            '\n', '\r', '\t',
                            '(', ')', '[', ']',
                            '"', '`', '´', '-',
                            '=', '&', '\'', ' '
                            };

        public string ContentType => "*";

        public IEnumerable<string> Tokenize(string text)
        {
            return Normalize(text).Split(_delims, StringSplitOptions.None);
            //return Normalize(text).Split(_phraseDelimiters, StringSplitOptions.RemoveEmptyEntries);
        }

        public string Normalize(string text)
        {
            return text.ToLower(CultureInfo.CurrentCulture);
        }

        public void Dispose()
        {
        }
    }

    public static class StringExtensions
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(
        this IEnumerable<T> source, int size)
        {
            T[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new T[size];

                bucket[count++] = item;

                if (count != size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            // Return the last bucket with all remaining elements
            if (bucket != null && count > 0)
                yield return bucket.Take(count);
        }
    }
}
