using System;
using System.IO;

namespace Course.Debug {
    public static class Journal
    {
        private static String extensionString = "log";
        private static String pathwayString = "log";
        private static StreamWriter streamWriter = null;

        public static String FileName {
            get {
                return DateTime.Now.ToString("MM.dd.hh.mm.ss.ms");
            }
        }

        static Journal()
        {
            var folderPathwayString = Journal.pathwayString;
            var filePathwayString = String.Format(@"{0}.{1}", Journal.FileName, Journal.extensionString);
            var pathwayString = folderPathwayString + '\\' + filePathwayString;

            if (!Directory.Exists(folderPathwayString))
            {
                Directory.CreateDirectory(folderPathwayString);
            }
            if (File.Exists(pathwayString))
            {
                File.Delete(pathwayString);
            }
            Journal.streamWriter = new StreamWriter(pathwayString);
        }

        public static void Log(String textString)
        {
            Journal.streamWriter.Write(DateTime.Now.ToString(" [hh:mm.ss] "));
            Journal.streamWriter.WriteLine(textString);
            Journal.streamWriter.Flush();
        }
    }
}