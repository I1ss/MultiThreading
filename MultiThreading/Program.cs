using System.Threading;

using MultiThreading.MultiThreadingUtils;

namespace MultiThreading
{
    class Program
    {
        static void Main(string[] args)
        {
            var threadFirst = MultiThreadingUtilities.StartWorkingThread();

            Thread.Sleep(2000);

            var taskFirst = MultiThreadingUtilities.StartWorkingTask();

            Thread.Sleep(2000);

            // Рассказать про Thread.Sleep().
            var poolThreadFirst = MultiThreadingUtilities.StartWorkingPoolThread();
        }
    }
}
