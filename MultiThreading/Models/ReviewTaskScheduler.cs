using System;
using System.Threading;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MultiThreading.Models
{
    internal class ReviewTaskScheduler : TaskScheduler
    {
        private LinkedList<Task> _tasksList;

        public ReviewTaskScheduler()
        {
            _tasksList = new LinkedList<Task>();
        }

        public ReviewTaskScheduler(LinkedList<Task> tasks)
        {
            _tasksList = tasks;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _tasksList;
        }

        protected override void QueueTask(Task task)
        {
            Console.WriteLine($"[Queue Task] Задача #{task.Id} поставлена в очередь...");
            if (_tasksList?.Count >= 10)
                _tasksList = new LinkedList<Task>();

            _tasksList.AddLast(task);
            ThreadPool.QueueUserWorkItem(ExecuteTasks, null);
            Thread.Sleep(2000);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            Console.WriteLine($"[TryExecuteTaskInline] Попытка выполнить задачу #{task.Id} синхронно...");

            lock (_tasksList)
            {
                _tasksList.Remove(task);
            }

            return base.TryExecuteTask(task);
        }

        protected override bool TryDequeue(Task task)
        {
            Console.WriteLine($"[TryDequeue] Попытка удалить задачу {task.Id} из очереди...");
            var result = false;

            lock (_tasksList)
            {
                result = _tasksList.Remove(task);
            }

            if (result == true)
                Console.WriteLine($"[TryDequeue] Задача {task.Id} была удалена из очереди на выполнение...");

            return result;
        }

        private void ExecuteTasks(object _)
        {
            while (true)
            {
                Task task;

                lock (_tasksList)
                {
                    if (_tasksList.Count == 0)
                        break;

                    task = _tasksList.First.Value;
                    _tasksList.RemoveFirst();
                }

                if (task == null)
                    break;

                base.TryExecuteTask(task);
            }
        }

        /// <summary>
        /// Нахождение задачи и перемещение её на последнее место в очереди.
        /// </summary>
        /// <param name="task"> Искомая задача. </param>
        /// <returns> True - операция завершилась удачно, false - неудачно. </returns>
        public bool SetTaskPositionToLast(Task task)
        {
            var neededTask = _tasksList?.FirstOrDefault(taskNum => taskNum.Id == task.Id);
            if (neededTask is null)
                return false;

            lock (_tasksList)
            {
                _tasksList?.AddLast(neededTask);
                _tasksList?.Remove(_tasksList?.FirstOrDefault(taskNum => taskNum.Id == task.Id));
            }

            return true;
        }

        /// <summary>
        /// Нахождение задачи и перемещение её на первое место в очереди.
        /// </summary>
        /// <param name="task"> Искомая задача. </param>
        /// <returns> True - операция завершилась удачно, false - неудачно. </returns>
        public bool SetTaskPositionToFirst(Task task)
        {
            var neededTask = _tasksList?.FirstOrDefault(taskNum => taskNum.Id == task.Id);
            if (neededTask is null)
                return false;

            lock (_tasksList)
            {
                _tasksList?.Remove(_tasksList?.FirstOrDefault(taskNum => taskNum.Id == task.Id));
                _tasksList?.AddFirst(neededTask);
            }

            return true;
        }
    }
}
