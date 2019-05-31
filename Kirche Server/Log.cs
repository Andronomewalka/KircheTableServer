using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Kirche_Server
{
    public static class Log
    {
        static string folderPath;
        static string curDateLogFile;

        static Log()
        {
            folderPath = Directory.GetCurrentDirectory() + "\\Logs";
            curDateLogFile = $"{DateTime.Now:dd.MM.yyyy}" + ".txt";
        }

        public static void DefineFileToWrite()
        {
            if (!File.Exists(folderPath + "//" + curDateLogFile))
                File.Create(folderPath + "//" + curDateLogFile + ".txt");
        }
    }
}
