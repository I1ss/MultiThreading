using MultiThreading.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.MultiThreadingUtils
{
    public static class MultiThreadingUtilities
    {
        private const int SIZE_FOR_WRITE = 100;

        private static void WriteSomething()
        {
            var random = new Random();

            for (var index = 0; index < SIZE_FOR_WRITE; index++)
            {
                var randomString = random.Next(-10000, 10000).ToString();
                Console.WriteLine($"{randomString} - it's our random string!");
            }
        }

        private static bool WriteSomethingBoolean()
        {
            var random = new Random();

            for (var index = 0; index < SIZE_FOR_WRITE; index++)
            {
                var randomString = random.Next(-10000, 10000).ToString();
                Console.WriteLine($"{randomString} - it's our random string!");
            }

            return true;
        }

        public static bool StartWorkingThread()
        {
            var threadWrite = new Thread(WriteSomething);
            threadWrite.Start();

            for (var index = 0; index < SIZE_FOR_WRITE; index++)
            {
                Console.WriteLine($"Current iteration: {index}; All iterations: {SIZE_FOR_WRITE}");
            }

            return threadWrite.IsAlive;
        }

        public static bool StartWorkingTask()
        {
            var taskWrite = new TaskFactory();
            taskWrite.StartNew(WriteSomething);

            for (var index = 0; index < SIZE_FOR_WRITE; index++)
            {
                Console.WriteLine($"Current iteration: {index}; All iterations: {SIZE_FOR_WRITE}");
            }

            return true;
        }

        public static bool StartWorkingPoolThread()
        {
            var poolThread = new ThreadPoolModel<bool>(new Func<bool>(WriteSomethingBoolean));
            poolThread.Start(1000);

            for (var index = 0; index < SIZE_FOR_WRITE; index++)
            {
                Console.WriteLine($"Current iteration: {index}; All iterations: {SIZE_FOR_WRITE}");
            }

            poolThread.Wait();

            Console.WriteLine($"We are waited! Our result: {poolThread.Result}");

            return true;
        }
    }
}
