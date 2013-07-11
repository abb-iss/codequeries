using ABB.SrcML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABB.CodeQueries.Runner {
    class Program {
        static void Main(string[] args) {
            if(args.Length < 1) {
                Console.WriteLine("Provide a path to a source folder");
                return;
            }
            var sourceFolder = new SourceFolder(args[0]);
           sourceFolder.UpdateArchive();
            foreach(var method in Queries.GetAllMethods(sourceFolder)) {
                Console.WriteLine("{0}:{1} - {2}", method.PrimaryLocation.SourceFileName, method.PrimaryLocation.StartingLineNumber, method.ToString());
            }
        }
    }
}
