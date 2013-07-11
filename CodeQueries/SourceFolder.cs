﻿using ABB.SrcML;
using ABB.SrcML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ABB.CodeQueries {
    public class SourceFolder {
        public DirectoryInfo Info { get; private set; }

        public string FullPath { get { return this.Info.FullName; } }

        public ISrcMLArchive Archive { get; private set; }

        public DataRepository Data { get; private set; }

        public SourceFolder(string path) {
            this.Info = new DirectoryInfo(path);
            var archive = new SrcMLArchive(Directory.GetCurrentDirectory(), String.Format("srcML-{0}", Path.GetFileName(this.FullPath)));
            Archive = archive;
        }

        public IEnumerable<string> GetFiles() {
            var files = from fileInfo in this.Info.EnumerateFiles("*", SearchOption.AllDirectories)
                        select fileInfo.FullName;
            return files;
        }

        public IEnumerable<string> GetSourceFiles() {
            var sourceFiles = from file in GetFiles()
                              where Archive.SupportedExtensions.Contains(Path.GetExtension(file))
                              select file;
            return sourceFiles;
        }

        public void UpdateArchive() {
            foreach(var sourceFile in GetSourceFiles()) {
                Console.WriteLine("Adding {0}", sourceFile);
                Archive.AddOrUpdateFile(sourceFile);
            }
            this.Data = new DataRepository(Archive as SrcMLArchive);
        }
    }
}
