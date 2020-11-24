using System;

namespace LR2_MPP
{
    class Program
    {
        private const string InvalidArgsExceptionMessage = "Неверно введены аргументы!";

        private static void Main(string[] args)
        {
            if (!(args.Length == 3 && int.TryParse(args[2], out var threadCount)))
            {
                Console.WriteLine(InvalidArgsExceptionMessage);
                return;
            }

            TaskQueue taskQueue;
            try
            {
                taskQueue = new TaskQueue(threadCount);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return;
            }

            try
            {
                CopyCatalog copier = new CopyCatalog(args[0], args[1], taskQueue);
                copier.Start();
                Console.WriteLine();
                Console.WriteLine($"Успешно скопировано: {copier.SuccessfulTasksCount}");
                Console.WriteLine($"Ошибок: {copier.ErrorTasksCount}");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                taskQueue.ForceStop();
            }
        }
    }
}
