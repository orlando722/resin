﻿using System;
using System.IO;
using System.Linq;

namespace DocumentTable
{
    public class ReadSessionFactory : IReadSessionFactory, IDisposable
    {
        private readonly string _directory;
        private readonly FileStream _compoundFile;

        public ReadSessionFactory(string directory)
        {
            _directory = directory;

            var version = Directory.GetFiles(directory, "*.ix")
                .Select(f => long.Parse(Path.GetFileNameWithoutExtension(f)))
                .OrderBy(v => v).First();

            var compoundFileName = Path.Combine(_directory, version + ".rdb");

            _compoundFile = new FileStream(
                compoundFileName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                4096 * 1,
                FileOptions.RandomAccess);
        }

        public IReadSession OpenReadSession(long version)
        {
            var ix = BatchInfo.Load(Path.Combine(_directory, version + ".ix"));
           
            return new ReadSession(
                ix,
                new PostingsReader(_compoundFile, ix.PostingsOffset),
                new DocHashReader(_compoundFile, ix.DocHashOffset),
                new DocumentAddressReader(_compoundFile, ix.DocAddressesOffset),
                _compoundFile);
        }

        public void Dispose()
        {
            _compoundFile.Dispose();
        }
    }
}
