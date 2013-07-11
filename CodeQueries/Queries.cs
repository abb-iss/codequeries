using ABB.SrcML;
using ABB.SrcML.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABB.CodeQueries
{
    public class Queries
    {
        public static IEnumerable<MethodDefinition> GetAllMethods(SourceFolder folder) {
            return folder.Data.GlobalScope.GetChildScopes<MethodDefinition>();
        }

        public static Dictionary<string, IList<MethodDefinition>> GetAllMethodsByName(SourceFolder folder) {
            var results = new Dictionary<string, IList<MethodDefinition>>();
            var methodsByName = from method in GetAllMethods(folder)
                                group method by method.Name into g
                                select g;

            foreach(var group in methodsByName) {
                results[group.Key] = group.ToList<MethodDefinition>();
            }
            return results;
        }
    }
}
