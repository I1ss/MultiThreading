using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MultiThreading.Models;
using MultiThreading.MultiThreadingUtils;

namespace MultiThreading
{
    class Program
    {
        private const string FIRST_LR = "FIRST";

        private const string SECOND_LR = "SECOND";

        static void Main(string[] args)
        {
            switch (SECOND_LR)
            {
                case FIRST_LR:
                    var threadFirst = MultiThreadingUtilities.StartWorkingThread();

                    Thread.Sleep(2000);

                    var taskFirst = MultiThreadingUtilities.StartWorkingTask();

                    Thread.Sleep(2000);

                    // Рассказать про Thread.Sleep(). Передаёт поток после завершения, нарушает асинхронность.
                    var poolThreadFirst = MultiThreadingUtilities.StartWorkingPoolThread();
                    break;
                case SECOND_LR:
                    var tasks = new Task[10];
                    var reviewTaskScheduler = new ReviewTaskScheduler();

                    QueueTaskTesting(tasks, reviewTaskScheduler);
                    Task.WaitAll(tasks);

                    // 2nd part.

                    var tasksList = new LinkedList<Task>();
                    
                    for (var i = 0; i < 10; i++)
                    {
                        tasks[i] = new Task(() =>
                        {
                            Thread.Sleep(2000);
                            Console.WriteLine($"Задача {Task.CurrentId} выполнилась в потоке {Thread.CurrentThread.ManagedThreadId}.\n");
                        });

                        tasksList.AddLast(tasks[i]);
                    }

                    reviewTaskScheduler = new ReviewTaskScheduler(tasksList);
                    reviewTaskScheduler.SetTaskPositionToLast(tasksList.FirstOrDefault(task => task.Id > 15));

                    reviewTaskScheduler = new ReviewTaskScheduler(tasksList);
                    reviewTaskScheduler.SetTaskPositionToFirst(tasksList.FirstOrDefault(task => task.Id > 12));

                    QueueTaskTesting(tasksList.ToArray(), reviewTaskScheduler);
                    Task.WaitAll(tasksList.ToArray());
                    break;
                default:
                    break;
            }
        }

        private static void QueueTaskTesting(Task[] tasks, ReviewTaskScheduler reviewTaskScheduler)
        {
            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() =>
                {
                    Thread.Sleep(2000);
                    Console.WriteLine($"Задача {Task.CurrentId} выполнилась в потоке {Thread.CurrentThread.ManagedThreadId}.\n");
                });

                tasks[i].Start(reviewTaskScheduler);
            }
        }
    }
}
