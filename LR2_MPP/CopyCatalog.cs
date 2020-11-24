using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace LR2_MPP
{
    public class CopyCatalog
    {
        private readonly List<CopyFileTask> copyTasks = new List<CopyFileTask>();
        private readonly List<string> necessaryDirectories = new List<string>();
        private readonly TaskQueue taskQueue;

        public CopyCatalog(string source, string destination, TaskQueue taskQueue)
        {
            this.taskQueue = taskQueue;

            if (!Directory.Exists(source))
            {
                throw new Exception($"{Path.GetFullPath(source)} не является каталогом или такого пути не существует.");
            }

            if (!Directory.Exists(destination))
            {
                necessaryDirectories.Add(destination);
            }

            CreateCopyTasks(source, destination);
        }

        private void CreateCopyTasks(string source, string destination)
        {
            var sourceFiles = Directory.GetFiles(source);
            foreach (var filePath in sourceFiles)
            {
                string fileAbsolutePath = Path.GetFullPath(filePath);
                string destinationPath = Path.GetFullPath(destination) + @"\" + Path.GetFileName(filePath);

                CopyFileTask task = new CopyFileTask(fileAbsolutePath, destinationPath);
                copyTasks.Add(task);
            }

            string[] sourceDirectories = Directory.GetDirectories(source);
            foreach (var dirPath in sourceDirectories)
            {
                string dirAbsolutePath = Path.GetFullPath(dirPath);
                string destinationPath = Path.GetFullPath(destination) + @"\" + Path.GetFileName(dirPath);

                if (!Directory.Exists(destinationPath)) 
                    necessaryDirectories.Add(destinationPath);

                CreateCopyTasks(dirAbsolutePath, destinationPath);
            }
        }

        public void Start()
        {
            foreach (var directoryPath in necessaryDirectories) 
                Directory.CreateDirectory(directoryPath);

            foreach (var copyTask in copyTasks) 
                taskQueue.EnqueueTask(copyTask.Perform);

            SpinWait sw = new SpinWait();
            while (!TasksCompleted()) 
                sw.SpinOnce();
        }

        private bool TasksCompleted()
        {
            foreach (var task in copyTasks)
                if (task.Status == CopyStatus.Waiting)
                    return false;
            return true;
        }

        public int SuccessfulTasksCount => copyTasks.Count(task => task.Status == CopyStatus.Successful);

        public int ErrorTasksCount => copyTasks.Count(task => task.Status == CopyStatus.Error);
    }
}
