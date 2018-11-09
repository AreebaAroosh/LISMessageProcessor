using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LISMessageProcessor.Test
{
    public static class Helpers
    {
        public static string GetStringFromFile(string FileName)
        {
            string path = Path.GetDirectoryName(
                typeof(Helpers).GetTypeInfo()
                .Assembly.Location)
                .Replace("\\bin\\Debug", "")
                .Replace("\\bin\\Release", "") + "/";
            return File.ReadAllText(path + FileName);
        }
    }
}
