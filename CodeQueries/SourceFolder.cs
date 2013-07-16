/******************************************************************************
* Copyright (c) 2013 ABB Group
* All rights reserved. This program and the accompanying materials
* are made available under the terms of the Eclipse Public License v1.0
* which accompanies this distribution, and is available at
* http://www.eclipse.org/legal/epl-v10.html
*
* Contributors:
* Vinay Augustine (ABB Group) - Initial implementation
*****************************************************************************/

using ABB.SrcML;
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

        public SourceFolder(string path, ISrcMLArchive archive) {
            this.Info = new DirectoryInfo(path);
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
            var outdatedFiles = from file in GetSourceFiles()
                                where Archive.IsOutdated(file)
                                select file;
            foreach(var sourceFile in outdatedFiles) {
                Console.WriteLine("Adding {0}", sourceFile);
                Archive.AddOrUpdateFile(sourceFile);
            }
            this.Data = new DataRepository(Archive);
        }
    }
}
