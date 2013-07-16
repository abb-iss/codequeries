# CodeQueries Readme

This is a playground for experimenting with queries against SrcML.NET, SrcML Data, and SWUM.

You must set the `SRCMLBINDIR` environment variable so that it points to the directory with all of the `src2srcml` executables before running.

The primary tool is "cq.exe". In order to print a .CSV file with all of the method info for the Notepad++ source code, you would do this:

    cq -a srcml-npp-6.2 -c=print-methods -o=methods.csv -e .cxx:C++ -e h:C++ c:\Source\Notepad++-6.2

You can also print method call information:

    cq -a srcml-npp-6.2 -c=print-calls -o=calls.csv -e .cxx:C++ -e h:C++ c:\Source\Notepad++-6.2

Here's an explanation of the arguments:

* `-a` is the path to srcML archive to be created or updated
* `-c` the command to use (currently either `print-methods` or `print-calls`)
* `-e` map an extension to a language these two arguments map both `.cxx` and `.h` files to C++
* `c:\Source\Notepad++-6.2` is the path for the Notepad++ source code

You can also get this information from `cq -h`.