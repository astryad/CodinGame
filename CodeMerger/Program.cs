using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMerger
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = Directory.EnumerateFiles(args[0], @"*.cs", SearchOption.AllDirectories);

            var usings = new List<string>();

            var classes = new List<string>();

            foreach (var file in files)
            {
                if (ShouldIgnoreFile(file))
                    continue;

                var lines = File.ReadAllLines(file);

                foreach (var line in lines)
                {
                    if (IsUsing(line))
                    {
                        usings.Add(line);
                    }
                    else
                    {
                        classes.Add(line);
                    }
                }
            }

            usings = usings.Distinct().ToList();

            usings.AddRange(classes);

            File.WriteAllLines(Path.Combine(args[0], "CodinGameSync.cs"), usings);
        }

        private static bool IsUsing(string line)
        {
            return line.StartsWith("using");
        }

        private static bool ShouldIgnoreFile(string file)
        {
            if (file.EndsWith("AssemblyInfo.cs"))
                return true;

            if (file.EndsWith("CodinGameSync.cs"))
                return true;

            return false;
        }
    }
}
