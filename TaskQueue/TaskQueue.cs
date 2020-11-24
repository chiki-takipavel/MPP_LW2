using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LR2_MPP
{
    public class TaskQueue
    {
        public delegate void TaskDelegate();

        private const string TasksCountExceptionMessage = "Количество потоков должно быть больше нуля.";
        private readonly ConcurrentQueue<TaskDelegate> queuedTasks = new ConcurrentQueue<TaskDelegate>();
        private bool isRunning = true;

        public TaskQueue(int threadCount)
        {
            if (threadCount <= 0) 
                throw new ArgumentException(TasksCountExceptionMessage, nameof(threadCount));

            Task[] tasks = new Task[threadCount];
            for (var i = 0; i < threadCount; i++)
            {
                Task task = new Task(ThreadLoop);
                tasks[i] = task;
                task.Start();
            }
        }

        private void ThreadLoop()
        {
            SpinWait sw = new SpinWait();

            while (isRunning)
                if (queuedTasks.TryDequeue(out var task))
                    task.Invoke();
                else
                    sw.SpinOnce();
        }

        public void EnqueueTask(TaskDelegate task) => queuedTasks.Enqueue(task);

        public void ForceStop() => isRunning = false;
    }
}
