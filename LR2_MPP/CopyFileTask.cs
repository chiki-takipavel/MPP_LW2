using System;
using System.IO;

namespace LR2_MPP
{
    public enum CopyStatus
    {
        Waiting,
        Successful,
        Error
    }

    public class CopyFileTask
    {
        public string Source { get; }
        public string Destination { get; }
        public CopyStatus Status { get; private set; }

        public CopyFileTask(string source, string destination)
        {
            Source = source;
            Destination = destination;
            Status = CopyStatus.Waiting;
        }

        public void Perform()
        {
            try
            {
                File.Copy(Source, Destination, true);
                Console.WriteLine($"Скопировано из {Source} в {Destination}");
                Status = CopyStatus.Successful;
            }
            catch (Exception exception)
            {
                Status = CopyStatus.Error;
                Console.WriteLine(exception.Message);
            }
        }
    }
}
