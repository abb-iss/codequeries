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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABB.CodeQueries {

    public class Queries {

        public static IEnumerable<MethodCall> GetAllCalls(SourceFolder folder) {
            var allCalls = from scope in folder.Data.GlobalScope.GetDescendantScopesAndSelf()
                           from call in scope.MethodCalls
                           select call;
            return allCalls;
        }

        public static int GetAllExpressions(SourceFolder folder) {
            var expressions = from file in folder.Archive.FileUnits
                              from expression in file.Descendants(SRC.Expression)
                              select expression;
            return expressions.Count();
        }

        public static IEnumerable<MethodDefinition> GetAllMethods(SourceFolder folder) {
            return folder.Data.GlobalScope.GetDescendantScopesAndSelf<MethodDefinition>();
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