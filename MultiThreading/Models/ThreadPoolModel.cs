using System;
using System.Threading;

namespace MultiThreading.Models
{
    public class ThreadPoolModel<TResult>
    {
        private readonly Func<TResult> _action;

        public bool Success { get; private set; }

        public bool Completed { get; private set; }

        public Exception Exception { get; private set; }

        public TResult Result { get; private set; }

        public ThreadPoolModel(Func<TResult> action)
        {
            if (action is null)
                throw new NotSupportedException("Your action is not correct!");

            _action = action;
            Success = false;
            Completed = false;
            Exception = null;
            Result = default;
        }

        public void Start(object state)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadException), state);
        }

        public void Wait()
        {
            while (Completed == false)
            {
                Thread.Sleep(150);
            }

            if (Exception != null)
                throw Exception;
        }

        private void ThreadException(object state)
        {
            try
            {
                Result = _action.Invoke();
                Success = true;
            }
            catch (Exception ex)
            {
                Exception = ex;
                Success = false;
            }
            finally
            {
                Completed = true;
            }
        }
    }
}
