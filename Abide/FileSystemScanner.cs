using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Abide
{
    public class FileSystemScanner
    {
        public bool DatabaseExists(string name)
        {
            var workingDirectory = new DirectoryInfo(".");
            return workingDirectory.EnumerateFiles().Select(info => info.Name)
                .Any(n => Regex.IsMatch(n, $"^{name}.dat$"));
        }

        public IEnumerable<string> FilesMatching(string regex)
        {
            var workingDirectory = new DirectoryInfo(".");
            return workingDirectory.EnumerateFiles().Select(info => info.Name)
                .Where(name => Regex.IsMatch(name, regex));
        }
    }
}