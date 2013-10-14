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
using Mono.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABB.CodeQueries.Runner {

    internal delegate void Command(SourceFolder folder, TextWriter writer);

    internal class Program {

        private static Command GetCommand(string commandName) {
            if(commandName == "print-methods") {
                return PrintMethods;
            } else if(commandName == "print-calls") {
                return PrintCalls;
            }
            return null;
        }

        private static void Main(string[] args) {
            string archivePath = "srcml";
            string outputPath = null;
            Command command = null;

            bool shouldShowHelp = false;
            Dictionary<string, Language> extensionMapping = new Dictionary<string, Language>();

            OptionSet options = new OptionSet() {
                { "a|archive=", "the location of the srcML {ARCHIVE}", a => archivePath = a },
                { "c|command=", "the {COMMAND} to use (print-methods|print-calls)", c => command = GetCommand(c) },
                { "o|output=", "the location of the {OUTPUT} file", o => outputPath = o },
                { "e|extension=", "an extension mapping of the form \"EXT=Language\" (for example -e cs=C#)", e => {
                        var parts = e.Split(':');
                        if(parts.Length == 2) {
                            extensionMapping[parts[0]] = SrcMLElement.GetLanguageFromString(parts[1]);
                        }
                    }
                },
                { "h|?|help", o => shouldShowHelp = true }
            };

            var sourceFolderPaths = options.Parse(args);
            string errorMessage = String.Empty;

            if(sourceFolderPaths.Count == 0) {
                errorMessage = "Provide a source folder";
                shouldShowHelp = true;
            }

            if(command == null) {
                errorMessage = "Provide a command";
                shouldShowHelp = true;
            }

            if(shouldShowHelp) {
                if(!String.IsNullOrEmpty(errorMessage)) {
                    Console.Error.WriteLine(errorMessage);
                }
                ShowHelp(options);
            } else {
                var archive = new SrcMLArchive(Path.GetDirectoryName(archivePath), Path.GetFileName(archivePath));
                foreach(var key in extensionMapping.Keys) {
                    var finalKey = (key[0] != '.' ? "." + key : key);
                    Console.WriteLine("Mapping {0} files to {1}", finalKey, extensionMapping[key]);
                    archive.XmlGenerator.ExtensionMapping[finalKey] = extensionMapping[key];
                }

                var sourceFolder = new SourceFolder(sourceFolderPaths.First(), archive);

                sourceFolder.UpdateArchive();
                using(TextWriter output = (outputPath == null ? Console.Out : new StreamWriter(outputPath))) {
                    command(sourceFolder, output);
                }
            }
        }

        private static void PrintCalls(SourceFolder folder, TextWriter output) {
            output.WriteLine("Call File Number, Caller Name, Call Line Number, Call Column Number, Callee Name, Callee Definition File Name, Callee Definition Line Number");
            foreach(var call in Queries.GetAllCalls(folder)) {
                var caller = call.ParentScope.GetParentScopesAndSelf<NamedScope>().FirstOrDefault();
                string callerName = (caller == null ? String.Empty : caller.Name);
                string callInfo = String.Join(",", Q(call.Location.SourceFileName),
                    Q(callerName),
                    Q(call.Location.StartingLineNumber.ToString()),
                    Q(call.Location.StartingColumnNumber.ToString()),
                    Q(call.Name)
                );
                var matchingMethods = call.FindMatches();

                if(matchingMethods.Any()) {
                    foreach(var method in matchingMethods) {
                        foreach(var location in method.Locations) {
                            string methodInfo = String.Join(",", Q(location.SourceFileName), Q(location.StartingLineNumber.ToString()));
                            output.WriteLine("{0},{1}", callInfo, methodInfo);
                        }
                    }
                } else {
                    output.WriteLine("{0},,", callInfo);
                }
            }
        }

        private static void PrintMethods(SourceFolder folder, TextWriter output) {
            output.WriteLine("File Name,Line Number,Method Name, Method Signature");
            foreach(var method in Queries.GetAllMethods(folder)) {
                output.WriteLine("{0},{1},{2},{3}", Q(method.PrimaryLocation.SourceFileName), Q(method.PrimaryLocation.StartingLineNumber.ToString()), Q(method.Name), Q(method.ToString()));
            }
        }

        private static string Q(string str) {
            return '"' + str + '"';
        }

        private static void ShowHelp(OptionSet p) {
            Console.WriteLine("Usage: cq [OPTIONS]+ [folders]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}