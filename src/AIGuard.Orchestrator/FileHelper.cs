using System.IO;
using System.Threading;

namespace AIGuard.Orchestrator
{
    public static class FileHelper
    {
        public static bool IsFileClosed(string filepath, bool wait)
            {
                bool fileClosed = false;
                int retries = 20;
                const int delayMS = 100;

                if (!File.Exists(filepath))
                    return false;
                do
                {
                    try
                    {
                        FileStream fs = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        fs.Close();
                        fileClosed = true; // success
                    }
                    catch (IOException) { }

                    if (!wait) break;

                    retries--;

                    if (!fileClosed)
                        Thread.Sleep(delayMS);
                }
                while (!fileClosed && retries > 0);
                return fileClosed;
            }
    }
}