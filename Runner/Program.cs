using ABB.SrcML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABB.CodeQueries.Runner {
    class Program {
        static void Main(string[] args) {
            if(args.Length < 2) {
                Console.WriteLine("Provide a path to a source folder and a srcML archive folder");
                return;
            }
            var archive = new SrcMLArchive(Path.GetDirectoryName(args[1]), Path.GetFileName(args[1]));
            var sourceFolder = new SourceFolder(args[0], archive);
            
           sourceFolder.UpdateArchive();
            foreach(var method in Queries.GetAllMethods(sourceFolder)) {
                Console.WriteLine("{0}:{1} - {2}", method.PrimaryLocation.SourceFileName, method.PrimaryLocation.StartingLineNumber, method.ToString());
            }
        }
    }
}
