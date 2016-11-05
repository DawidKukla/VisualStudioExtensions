using System;
using System.IO;

namespace FreeCommanderExtension.Utils
{
    public class Helpers
    {
        public static bool FileExists(string filePath)
        {
            if (Path.IsPathRooted(filePath))
                return File.Exists(filePath);

            foreach (var path in Environment.GetEnvironmentVariable("PATH").Split(';'))
                if (File.Exists(Path.Combine(path, filePath)))
                    return true;

            return false;
        }

        public static bool PathExists(string path)
        {
            var pathExpanded = Environment.ExpandEnvironmentVariables(path);
            return File.Exists(pathExpanded) || Directory.Exists(pathExpanded);
        }
    }
}